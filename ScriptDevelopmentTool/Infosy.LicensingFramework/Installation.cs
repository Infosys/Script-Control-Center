/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infosys.MTC.Licensing.Framework;
using Infosys.MTC.Licensing.Framework.Helpers;
using Infosys.MTC.Licensing.Framework.Common.ObjectModel;
using Infosys.MTC.Licensing.Framework.Common.Helpers.CryptoServices;
using Infosys.MTC.Licensing.Framework.Common.Helpers.RegistryUtils;
using System.IO;
using Infosys.MTC.Licensing.Framework.Common;

using System.Security.AccessControl;
using System.Security.Principal;

namespace Infosys.LicensingFramework
{
    public class Installation
    {
        private static string _registryHive = "HKEY_LOCAL_MACHINE";
        private static string _registryValue = "License";
        private static string _registryAppDataPathValue = "AppDataPath";
        private static string _registryInstallPathPathValue = "InstallPath";
        private static string _dataFileSubDirectoryPath = @"\Infosys\IAP";
        private static LicenseDetails installedLicenseDetails = null;
        private static LicenseDetails newLicenseDetails = null;
        private static string newLicenseKey = "";

        private static string KEYVAL_DATA = @"KeyVal.data";
        private static string TIMESTAMP_VALIDATION_FILEPATH = @"Infosys.MTC.Licensing.System.Native.dll";

        public static LicenseInstalltionResult Start(string licenseKey, string installPath = "", string productRegistryKey = @"SOFTWARE\Infosys\IAP", string productName = "Infosys Automation Platform")
        {
            LicenseInstalltionResult result = new LicenseInstalltionResult();
            try
            {
                //get the installed license, if any
                try
                {
                    string installedLicenseKey = RegistryHelper.GetLicenseKey(_registryHive, productRegistryKey, _registryValue);
                    if (!string.IsNullOrEmpty(installedLicenseKey))
                    {
                        installedLicenseDetails = new LicenseDetails(installedLicenseKey);
                        result = ValidateLicenseKey(licenseKey, productName);
                        if (!result.IsSuccess)
                            return result;
                    }
                }
                catch
                {
                    //i.e. not earlier installed license
                }

                //then install the new one
                if (string.IsNullOrEmpty(newLicenseKey))
                    newLicenseKey = LicenseKeyHelper.DecryptLicenseKey(licenseKey);
                RegistryHelper.SetLicenseKey(newLicenseKey, _registryHive, productRegistryKey, _registryValue);
                if (newLicenseDetails == null)
                    newLicenseDetails = GetLicenseDetails(licenseKey);
                if (newLicenseDetails.InstalledLicenseType == LicenseType.Evaluation && installedLicenseDetails != null)
                {
                    string appDataPath = RegistryHelper.GetStringValue(_registryHive, productRegistryKey, _registryAppDataPathValue);
                    string dataFilePath = Path.Combine(appDataPath, "KeyVal.data");
                    DataFileHelper.SetLicenseLimit(dataFilePath, installedLicenseDetails.DataFilePassword, newLicenseDetails.EvaluationLimit,
                        newLicenseDetails.DataFilePassword);

                    if (!installedLicenseDetails.UseExpiry && newLicenseDetails.UseExpiry)
                    {
                        string timeStampPath = Path.Combine(appDataPath, TIMESTAMP_VALIDATION_FILEPATH);
                        File.WriteAllBytes(timeStampPath, DataProtector.ProtectString(CultureIndependentDateTime.Now.ToString()));
                    }
                }
                else
                {
                    //then create the AppDataPath and InstallPath registry string values
                    if (!string.IsNullOrEmpty(installPath))
                        RegistryHelper.SetStringValue(_registryHive, productRegistryKey, _registryInstallPathPathValue, installPath);

                    string appdataFilepath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + _dataFileSubDirectoryPath;
                    if (!System.IO.Directory.Exists(appdataFilepath))
                    {
                        //create it
                        System.IO.Directory.CreateDirectory(appdataFilepath);
                    }
                    //give access to the local Users group to modify files under it
                    DirectorySecurity dirSec = System.IO.Directory.GetAccessControl(appdataFilepath);
                    SecurityIdentifier users = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null); //i.e. machinename\Users
                    dirSec.AddAccessRule(new FileSystemAccessRule(users, FileSystemRights.Modify | FileSystemRights.Synchronize, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                    System.IO.Directory.SetAccessControl(appdataFilepath, dirSec);

                    RegistryHelper.SetStringValue(_registryHive, productRegistryKey, _registryAppDataPathValue, appdataFilepath);

                    var keyVal = "WORKBENCHLAUNCH;Controlled;" + newLicenseDetails.EvaluationLimit + ";0";
                    File.WriteAllText(@"Temp.data", keyVal);
                    FileEncryptor.Encrypt(@"Temp.data", KEYVAL_DATA, "Infy@123");
                    File.Move(KEYVAL_DATA, Path.Combine(appdataFilepath, KEYVAL_DATA));
                    File.Delete(@"Temp.Data");

                    File.WriteAllBytes(TIMESTAMP_VALIDATION_FILEPATH, DataProtector.ProtectString(CultureIndependentDateTime.Now.ToString()));
                    File.Move(TIMESTAMP_VALIDATION_FILEPATH, Path.Combine(appdataFilepath, TIMESTAMP_VALIDATION_FILEPATH));

                }

                result.AnyInformation = "License Key installed/upgraded successfully";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.AnyInformation = ex.Message;
            }
            return result;
        }

        private static LicenseInstalltionResult ValidateLicenseKey(string licenseKey, string productName)
        {
            LicenseValidationResponse response = LicenseKeyValidationHelper.ValidateLicenseKey(productName, licenseKey);
            LicenseInstalltionResult result = new LicenseInstalltionResult();
            if (response.IsValid)
            {
                newLicenseKey = LicenseKeyHelper.DecryptLicenseKey(licenseKey);
                newLicenseDetails = new LicenseDetails(newLicenseKey);

                if (newLicenseDetails != null && installedLicenseDetails != null)
                {
                    if (!installedLicenseDetails.IsNodeLocked || (installedLicenseDetails.IsNodeLocked && string.Equals(installedLicenseDetails.HardwareId, Helper.GenerateIdentifier())))
                    {
                        if ((installedLicenseDetails.InstalledLicenseType == LicenseType.Evaluation)
                            && (newLicenseDetails.InstalledLicenseType == LicenseType.Evaluation) && (newLicenseDetails.EvaluationLimit <= installedLicenseDetails.EvaluationLimit))
                        {
                            result.AnyInformation = "Evaluation License cannot be replaced with an Evaluation License having lesser usage limit.\nPlease contact vendor";
                            result.IsSuccess = false;
                        }
                        else if (installedLicenseDetails.InstalledLicenseType == LicenseType.Licensed
                            && newLicenseDetails.InstalledLicenseType == LicenseType.Evaluation)
                        {
                            result.AnyInformation = "Full License cannot be replaced with an Evaluation license.";
                            result.IsSuccess = false;
                        }
                    }
                }
            }
            else
            {
                result.IsSuccess = false;
                result.AnyInformation = response.Message;
            }
            return result;
        }

        private static LicenseDetails GetLicenseDetails(string licenseKey)
        {
            LicenseDetails license = null;
            license = new LicenseDetails(LicenseKeyHelper.DecryptLicenseKey(licenseKey));
            return license;
        }
    }

    public class LicenseInstalltionResult
    {
        bool _isSuccess = true;
        string _anyInformation = "Success";
        public bool IsSuccess
        {
            get
            {
                return _isSuccess;
            }
            set
            {
                _isSuccess = value;
            }
        }
        public string AnyInformation
        {
            get
            {
                return _anyInformation;
            }
            set
            {
                _anyInformation = value;
            }
        }
    }
}

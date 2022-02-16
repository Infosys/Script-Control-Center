/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Infosys.MTC.Licensing.Framework;
using Infosys.MTC.Licensing.Framework.Interfaces;
using System.IO;

namespace Infosys.LicenseValidationClient
{
    public class Validator
    {
        private static string _dataFileSubDirectoryPath = @"\Infosys\IAP";
        private static string _validationFile = "skval.iap";
        const string WORKBENCHLAUNCH = "WORKBENCHLAUNCH";
        public static ValidationResult Validate(string keyvalSubDir = @"\Infosys\IAP\")
        {
            string dataFileName = "KeyVal.data";
            string skipLicenseValFile = "sklic.iap"; 
            string dataFileSubDirectory = keyvalSubDir;// @"\Infosys\IAP\";
            string dataFilePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + dataFileSubDirectory + dataFileName;
            string registryValue = "";
            bool result = false;
            ValidationResult valResult = new ValidationResult();
            valResult.FeaturesAllowed = new List<Feature>();
            try
            {
                //to skip all short of license validation
                string exeloc = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                if (System.IO.File.Exists(System.IO.Path.Combine(exeloc, skipLicenseValFile)))
                {
                    valResult.IsSuccess = true;
                    valResult.FeaturesAllowed.Add(Feature.ObjectModelExplorer);
                    valResult.FeaturesAllowed.Add(Feature.ScriptRepository);
                    valResult.FeaturesAllowed.Add(Feature.WorkflowDesigner);
                    return valResult;
                }


                //if (!Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE").ToString().Trim().ToLower().Equals("x86"))
                //{
                //    registryValue = @"Software\Wow6432Node" + keyvalSubDir;//\Infosys\IAP";
                //}
                //else
                //{
                //    registryValue = "Software" + keyvalSubDir; //\Infosys\IAP";
                //}

                //the above code is commented as:
                //for anytype of target platform type and build types,
                //the registry value wud be one of:
                //@"Software\Wow6432Node\Infosys\IAP" or
                //@"Software\Infosys\IAP"
                //as per new implementaion, first we will try for @"Software\Infosys\IAP", if fails as a retrial, will try with @"Software\Wow6432Node\Infosys\IAP"

                ILicensingProvider iLicensingProvider = null;
                try
                {
                    registryValue = "Software" + keyvalSubDir;
                    iLicensingProvider = LicenseProvider.GetLicenseProvider("Infosys Automation Platform", dataFilePath, "HKEY_LOCAL_MACHINE", registryValue, "License");
                }
                catch (Infosys.MTC.Licensing.Framework.Common.Exceptions.LicenseFrameworkException ex)
                {
                    registryValue = @"Software\Wow6432Node" + keyvalSubDir;
                    iLicensingProvider = LicenseProvider.GetLicenseProvider("Infosys Automation Platform", dataFilePath, "HKEY_LOCAL_MACHINE", registryValue, "License");
                }
                bool isLicensed = iLicensingProvider.ValidateLicense().IsValid;

                if (isLicensed)
                {
                    if (iLicensingProvider.LicenseInformation.InstalledLicenseType == LicenseType.Evaluation)
                    {
                        //when the license type is 'evaluation' then only we can have features specific validation
                        //this is by design behavior of the Infosys MTC licensing framework
                        
                        if (iLicensingProvider.ValidateFeature("WORKBENCHLAUNCH").IsValid)
                        {
                            iLicensingProvider.IncrementFeatureUsage("WORKBENCHLAUNCH", 1);
                            result = true;
                            valResult.FeaturesAllowed.Add(Feature.ObjectModelExplorer);
                            valResult.FeaturesAllowed.Add(Feature.ScriptRepository);
                            valResult.FeaturesAllowed.Add(Feature.WorkflowDesigner);
                        }
                        else
                        {
                            result = false;
                        }

                        //result = true;
                        //if (iLicensingProvider.ValidateFeature("WFDESIGNER").IsValid)
                        //    valResult.FeaturesAllowed.Add(Feature.WorkflowDesigner);
                        //if (iLicensingProvider.ValidateFeature("SCRIPTREPO").IsValid)
                        //    valResult.FeaturesAllowed.Add(Feature.ScriptRepository);
                        //if (iLicensingProvider.ValidateFeature("OBJECTMODEL").IsValid)
                        //    valResult.FeaturesAllowed.Add(Feature.ObjectModelExplorer);
                        ////but no increment to feature usage for licensed user as all the allowed fetaures
                        ////are to provided for any number of times
                    }
                    else
                    {
                        result = true;
                        //allow all the features
                        valResult.FeaturesAllowed.Add(Feature.ObjectModelExplorer);
                        valResult.FeaturesAllowed.Add(Feature.ScriptRepository);
                        valResult.FeaturesAllowed.Add(Feature.WorkflowDesigner);
                    }
                }
                else
                {
                    result = false;
                }
                valResult.IsSuccess = result;
            }
            catch
            {
                valResult.IsSuccess = false;
                valResult.FeaturesAllowed = null;
            }

            //if yet validation is false try for the alternate validation using skval.iap
            if (!valResult.IsSuccess && CheckAlternateValidation())
            {
                IncrementUsedCount();
                valResult.IsSuccess = true;
                valResult.FeaturesAllowed.Add(Feature.ObjectModelExplorer);
                valResult.FeaturesAllowed.Add(Feature.ScriptRepository);
                valResult.FeaturesAllowed.Add(Feature.WorkflowDesigner);
            }

            return valResult;
        }

        private static bool CheckAlternateValidation()
        {
            bool isSuccess = false;
            try
            {
                string appdataFilepath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + _dataFileSubDirectoryPath;
                string validationFilePath = Path.Combine(appdataFilepath, _validationFile);
                if (File.Exists(validationFilePath))
                {
                    string encryptedtext = File.ReadAllText(validationFilePath);
                    string[] lines = EncryptDecrypt.Decrypt(encryptedtext).Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (lines != null && lines.Length > 0)
                    {
                        foreach (string line in lines)
                        {
                            if (!string.IsNullOrEmpty(line))
                            {
                                string[] lineparts = line.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
                                if (lineparts != null && lineparts.Length == 3 && lineparts[0] == WORKBENCHLAUNCH && int.Parse(lineparts[1]) > int.Parse(lineparts[2]))
                                {
                                    //i.e. check for WORKBENCHLAUNCH count. lineparts[1] is allowed count, lineparts[2] used count
                                    isSuccess = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("The encrypted license is not of the correct format, please upgrade the license.");
            }
            return isSuccess;
        }

        private static bool IncrementUsedCount()
        {
            bool isSuccess = false;
            string appdataFilepath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + _dataFileSubDirectoryPath;
            string validationFilePath = Path.Combine(appdataFilepath, _validationFile);
            if (File.Exists(validationFilePath))
            {
                string encryptedtext = File.ReadAllText(validationFilePath);
                string[] lines = EncryptDecrypt.Decrypt(encryptedtext).Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                
                string newlines = "";
                if (lines != null && lines.Length > 0)
                {                    
                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            string[] lineparts = line.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
                            if (lineparts != null && lineparts.Length == 3)
                            {
                                if (lineparts[0] == WORKBENCHLAUNCH)
                                {
                                    newlines = newlines + lineparts[0] + "#" + lineparts[1] + "#" + (int.Parse(lineparts[2]) + 1).ToString() + Environment.NewLine;
                                    isSuccess = true;
                                }
                                else
                                    newlines = newlines + lineparts[0] + "#" + lineparts[1] + "#" + lineparts[2] + Environment.NewLine;
                            }
                        }
                    }
                }
                if (isSuccess)
                {
                    //write the new lines
                    File.WriteAllText(validationFilePath, EncryptDecrypt.Encrypt(newlines));
                }
            }
            return isSuccess;
        }
    }

    public class ValidationResult
    {
        public bool IsSuccess { get; set; }
        public List<Feature> FeaturesAllowed { get; set; }
    }

    public enum Feature
    {
        WorkflowDesigner,
        ScriptRepository,
        ObjectModelExplorer
    }
}

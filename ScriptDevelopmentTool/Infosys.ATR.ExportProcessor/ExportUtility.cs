/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.ATR.ExportUtility.Models;
using Infosys.WEM.Export.Service.Contracts.Data;
using Infosys.WEM.Export.Service.Contracts.Message;
using Infosys.WEM.SecureHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WEMClient = Infosys.WEM.Client;
using Infosys.ATR.ExportUtility;
using Infosys.ATR.ExportUtility.Models.ECR;
using Infosys.ATR.ExportUtility.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace Infosys.ATR.ExportProcessor
{
    public static class ExportUtility
    {
        static WEMClient.CommonRepository commonRepositoryClient = new WEMClient.CommonRepository();
        static WEMClient.ScriptRepository scriptClient = new WEMClient.ScriptRepository();
        static WEMClient.ExportRepository exportClient = new WEMClient.ExportRepository();
        static WEMClient.SecurityAccess securityClient = new WEMClient.SecurityAccess();
        static Infosys.ATR.ExportUtility.Service.ECRService ecrService = null;
        static int sourceScriptVersion = 0;
        static List<ATR.ExportUtility.Models.NIACategory> ecrCategories = new List<ATR.ExportUtility.Models.NIACategory>();
        static List<Infosys.ATR.ScriptRepository.Models.Category> categories = new List<ScriptRepository.Models.Category>();

        static AddExportTransactionDetailsReqMsg exportTransactionScriptDetailsList = null;
        static List<ScriptRepository.Models.Category> childCatList = null;
        static string sourceParentCatPath = "$";
        static string logFilePath = "";
        private static object lockObject = new object();
        static string newScriptName = "";
        static string updatedTargetCategoryPath = "$";
        //static List<string> ecrCatNameList = null;
        static int previousSourceCatId = 0;
        static int previousTargetCatId = 0;
        static int newTargetCatId = 0;
        static List<ATR.ExportUtility.Models.NIACategory> ecrCategoriesWithParentId = new List<ATR.ExportUtility.Models.NIACategory>();
        static List<string> targetCatPathList = null;
        static string targetCatCompletePath = "";
        public static void RunExportJob()
        {
            logFilePath = System.Configuration.ConfigurationManager.AppSettings["LogFilePath"].ToString();
            LoadCategory();
            RunJob();
            ecrService = ATR.ExportUtility.Service.ECRService.InstanceCreation();
        }
        public static void LoadCategory()
        {
            LogEntry("Entering LoadCategory Method");
            string companyId = System.Configuration.ConfigurationManager.AppSettings["Company"];
            Infosys.WEM.Service.Common.Contracts.Message.GetAllCategoriesResMsg response =
                       commonRepositoryClient.ServiceChannel.GetAllCategoriesByCompany(companyId, Infosys.ATR.ExportUtility.Constants.Application.ModuleID);

            categories = Infosys.ATR.ScriptRepository.Translators.CategoryTreePE_SE.CategoryListSEtoPE(response.Categories);
            LogEntry("Exiting LoadCategory Method");
        }
        private static LoginDetails GetLoginDetails(string targetServerId)
        {
            LogEntry("Entering GetLoginDetails() Method");
            LoginDetails loginDetails = null;
            var exportServerDetails = exportClient.ServiceChannel.GetExportServerDetails("", targetServerId);
            var serverDetails = ATR.ExportUtility.Translators.ExportServerDetailsSE_PE.ExportServerDetailsSEtoPEList(exportServerDetails.ExportServerDetails.ToList());

            if (serverDetails != null && serverDetails.Count > 0)
            {
                var server = serverDetails[0];
                loginDetails = new LoginDetails();
                loginDetails.CasServerAddr = server.CasServer;
                loginDetails.ECRServerAddr = server.DNSServer;
                var details = exportClient.ServiceChannel.GetExportConfigurationMasterDetails(targetServerId);
                var response = Infosys.ATR.ExportUtility.Translators.ExportConfigurationMasterPE_SE.ExportConfigurationSEtoPE(details.ExportConfigurationMasterDetails);
                if (response != null)
                {
                    loginDetails.UserName = response.TargetSystemUserId;
                    loginDetails.Password = response.TargetSystemPassword;
                }
                else
                {
                    throw new Exception("Exception in GetLoginDetails Method: Invalid username/password");
                }
            }
            else
                throw new Exception("Exception in GetLoginDetails Method: Invalid username/password");

            LogEntry("Exiting GetLoginDetails() Method");
            return loginDetails;

        }
        private static void FindTargetPath(NIACategory cat)
        {
            NIACategory findCat = ecrCategoriesWithParentId.Where(c => c.Id == cat.ParentId.ToString()).FirstOrDefault();
            if (findCat != null)
            {
                targetCatPathList.Add(findCat.Name);
                FindTargetPath(findCat);
            }
            //return targetCatPathList;
        }
        private static string FindTargetScriptPath(int catId)
        {
            var parentIdCat = ecrCategoriesWithParentId.Where(i => int.Parse(i.Id) == catId).FirstOrDefault();
            FindTargetPath(parentIdCat);
            targetCatPathList.Reverse();
            foreach (var str in targetCatPathList)
            {
                targetCatCompletePath = targetCatCompletePath + "\\" + str;
            }

            targetCatCompletePath = targetCatCompletePath + "\\" + parentIdCat.Name;

            return targetCatCompletePath;
        }
        private static void RunJob()
        {
            Boolean final_result = false;
            Boolean pending_conflicts = false;
            int jobId = 0;
            try
            {
                newTargetCatId = 0;
                LogEntry("Entering RunJob() Method");
                var respExportConfigMaster = exportClient.ServiceChannel.GetAllExportConfigurationMasterDetails();
                if (respExportConfigMaster.ExportConfigurationMasterDetails != null)
                {
                    string serviceBaseUrl = System.Configuration.ConfigurationManager.AppSettings["ServiceBaseUrl"];
                    System.Uri uri = new Uri(serviceBaseUrl);
                    //string uriWithoutScheme = uri.Host + uri.PathAndQuery + uri.Fragment;

                    var master = respExportConfigMaster.ExportConfigurationMasterDetails;
                    string hostName = Dns.GetHostName();
                    string ipAddress = Dns.GetHostByName(hostName).AddressList[0].ToString();

                    ecrService = ECRService.InstanceCreation();

                    var login = GetLoginDetails(master.TargetServerId.ToString());

                    //Call Nia ECR API  to create category on Nia instance
                    var client = ecrService.getInstance(login.UserName, login.Password, login.CasServerAddr, login.ECRServerAddr);

                    if (ecrCategories.Count == 0)
                    {
                        var targetCatList = ecrService.BrowseScriptCategory(login.ECRServerAddr);
                        ecrCategories = ATR.ExportUtility.Translators.ECRCategorySE_PE.CategoryListSEtoPE(targetCatList.rootCategories);
                        ecrCategoriesWithParentId = ATR.ExportUtility.Translators.ECRCategorySE_PE.CategoryListSEtoPEWithParentId(targetCatList.rootCategories);

                        //ecrCategoriesWithParentId = ecrCategoriesWithParentId.OrderBy(k => k.Name).ToList();
                        ecrCategoriesWithParentId = ecrCategoriesWithParentId.OrderBy(y => PadNumbers(y.Name)).ToList();

                        //    List<NIACategory> list = ecrCategories;
                        //    ecrCatNameList = list.SelectManyRecursive(l => l.Children).Select(l => l.Name).ToList();
                        //    foreach (var c in ecrCategories)
                        //    {
                        //        ecrCatNameList.Add(c.Name);
                        //    }
                        //    ecrCatNameList = ecrCatNameList.OrderBy(n => n).ToList();
                        //    ecrCatNameList = ecrCatNameList.OrderBy(x => PadNumbers(x)).ToList();
                    }

                    AddExportJobProcessingHistoryReqMsg jobReq = new AddExportJobProcessingHistoryReqMsg();
                    jobReq.ExportJobProcessingHistory = new ExportJobProcessingHistory();

                    jobReq.ExportJobProcessingHistory.ExportConfigurationId = master.id;
                    jobReq.ExportJobProcessingHistory.StartedOn = System.DateTime.Now;
                    jobReq.ExportJobProcessingHistory.ProcessedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    jobReq.ExportJobProcessingHistory.ProcessingSystemIp = ipAddress;
                    jobReq.ExportJobProcessingHistory.ProcessingSystemName = hostName;
                    var result = exportClient.ServiceChannel.AddExportJobProcessingHistory(jobReq);
                    jobId = result.JobId;
                    // Check if ExportStatus is submitted
                    if (master.ExportStatus == (int)ExportStatus.Submitted)
                    {
                        // Update ExportConfigurationMaster
                        UpdateExportConfigurationMasterReqMsg reqMaster = new UpdateExportConfigurationMasterReqMsg();
                        reqMaster.ExportConfigurationMaster = new WEM.Export.Service.Contracts.Data.ExportConfigurationMaster();
                        reqMaster.ExportConfigurationMaster.id = master.id;
                        reqMaster.ExportConfigurationMaster.ExportStatus = (int)ExportStatus.InProgress;
                        reqMaster.ExportConfigurationMaster.ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                        reqMaster.ExportConfigurationMaster.ModifiedOn = System.DateTime.UtcNow;
                        reqMaster.ExportConfigurationMaster.ScriptRepositoryBaseServerAddress = uri.Host;
                        var exportUpdateStatus = exportClient.ServiceChannel.UpdateExportConfigurationMaster(reqMaster);

                        if (exportUpdateStatus.IsSuccess)
                        {
                            // Get all the records from ExportScriptConfigurationDetails based on  ExportConfigurationId.
                            var exportScriptConfig = exportClient.ServiceChannel.GetExportScriptConfigurationDetails(reqMaster.ExportConfigurationMaster.id.ToString());
                            foreach (var script in exportScriptConfig.ExportScriptConfigurationDetails)
                            {
                                // Check if single source script was selected
                                if (script.SourceScriptId > 0)
                                {
                                    var sourceScriptDetails = scriptClient.ServiceChannel.GetScriptDetails(script.SourceScriptId.ToString(), script.SourceCategoryId.ToString());
                                    var sourceScript = Infosys.ATR.ScriptRepository.Translators.ScriptPE_SE.ScriptSEtoPE(sourceScriptDetails.ScriptDetails);

                                    exportTransactionScriptDetailsList = new AddExportTransactionDetailsReqMsg();
                                    exportTransactionScriptDetailsList.ExportTransactionDetails = new List<ExportTransactionDetails>();
                                    ExportTransactionDetails record = new ExportTransactionDetails();
                                    record.ExportScriptConfigurationId = script.id;
                                    record.SourceCategoryId = script.SourceCategoryId;
                                    record.SourceScriptId = script.SourceScriptId;
                                    record.TargetCategoryId = script.TargetCategoryId;
                                    record.TargetScriptId = script.TargetCategoryId;
                                    if (sourceScriptDetails.ScriptDetails != null)
                                    {
                                        record.TargetScriptName = sourceScript.Name;
                                        if (!string.IsNullOrEmpty(sourceScript.Version))
                                            record.SourceScriptVersion = int.Parse(sourceScript.Version);
                                        else
                                            record.SourceScriptVersion = 1;
                                    }
                                    record.TargetScriptVersion = record.SourceScriptVersion;
                                    //if (script.SourceScriptPath.Equals("$") && script.TargetScriptPath.Equals("$"))
                                    if (script.SourceScriptPath.Equals(script.TargetScriptPath))
                                    {
                                        // Construct source script path
                                        //var cat = categories.Where(c => int.Parse(c.Id) == record.SourceCategoryId).FirstOrDefault();
                                        //record.SourceScriptPath = FindSourceParentCategoryPath(cat) + "\\" + cat.Name;

                                        record.SourceScriptPath = script.SourceScriptPath;

                                        // Construct target script path
                                        if (record.TargetCategoryId > 0)
                                        {
                                            targetCatPathList = new List<string>();
                                            record.TargetScriptPath = "$" + "\\" + FindTargetScriptPath(record.TargetCategoryId);
                                        }
                                        else
                                            record.TargetScriptPath = script.TargetScriptPath;
                                    }
                                    else
                                    {
                                        record.SourceScriptPath = script.SourceScriptPath;
                                        record.TargetScriptPath = script.TargetScriptPath;
                                    }
                                    record.Status = (short)TransactionStatus.NA;
                                    record.isActive = true;
                                    record.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                                    record.CreatedOn = System.DateTime.UtcNow;
                                    exportTransactionScriptDetailsList.ExportTransactionDetails.Add(record);
                                    //string data = JsonConvert.SerializeObject(exportTransactionScriptDetailsList);
                                    var exportScriptTrnsResponse = exportClient.ServiceChannel.AddExportTransactionDetails(exportTransactionScriptDetailsList);
                                    if (!exportScriptTrnsResponse.IsSuccess)
                                    {
                                        LogEntry("Unable to add data to ExportTransactionTable");
                                        throw new Exception("Unable to add data to ExportTransactionTable");
                                    }
                                }
                                // Check if multi-level source category was selected
                                else
                                {
                                    exportTransactionScriptDetailsList = new AddExportTransactionDetailsReqMsg();
                                    exportTransactionScriptDetailsList.ExportTransactionDetails = new List<ExportTransactionDetails>();
                                    childCatList = new List<ScriptRepository.Models.Category>();
                                    var cat = categories.Where(c => c.Id.Equals(script.SourceCategoryId.ToString())).FirstOrDefault();
                                    childCatList.Add(cat);
                                    FindAllSourceChildCategories(script.SourceCategoryId);
                                    if (childCatList != null && childCatList.Count > 0)
                                    {
                                        GetAllSourceScriptDetails(childCatList, script);
                                        exportClient.ServiceChannel.AddExportTransactionDetails(exportTransactionScriptDetailsList);
                                    }
                                }

                            }

                            foreach (var script in exportScriptConfig.ExportScriptConfigurationDetails)
                            {

                                var transDetails = exportClient.ServiceChannel.GetAllExportTransactionDetails(script.id.ToString(), ((short)TransactionStatus.NA).ToString());

                                // Loop through all ExportTransactionDetails where status = 0
                                foreach (var trans in transDetails.ExportTransactionDetails.ToList())
                                {
                                    //System.IO.File.AppendAllText(logFilePath, "RunJob() Method - Source Script Id:" + trans.SourceScriptId.ToString() + " source category:" + trans.SourceCategoryId.ToString());
                                    LogEntry("RunJob() Method - Source Script Id:" + trans.SourceScriptId.ToString() + " source category:" + trans.SourceCategoryId.ToString());
                                    UpdateExportTransactionDetailsReqMsg reqTrans = new UpdateExportTransactionDetailsReqMsg();
                                    reqTrans.ExportTransactionDetails = new ExportTransactionDetails();
                                    reqTrans.ExportTransactionDetails.id = trans.id;
                                    reqTrans.ExportTransactionDetails.Status = (int)TransactionStatus.InProgress;
                                    // Update status to 1-InProgress in ExportTransactionDetails. 
                                    var transResult = exportClient.ServiceChannel.UpdateExportTransactionDetails(reqTrans);

                                    if (transResult.IsSuccess)
                                    {

                                        // Check if this Nia category already exists in ExportSourceTargetMappingDetails table
                                        var targetCatExists = exportClient.ServiceChannel.GetExportSourceTargetMappingDetails(master.TargetServerId.ToString(), trans.TargetCategoryId.ToString(), trans.SourceCategoryId.ToString());

                                        //newTargetCatId = trans.TargetCategoryId;
                                        Boolean success = true;
                                        if (targetCatExists.ExportSourceTargetMapping == null)
                                        {
                                            // Check and add category
                                            if (trans.SourceCategoryId == previousSourceCatId && trans.TargetCategoryId == previousTargetCatId)
                                            {
                                                // Skip category creation
                                            }
                                            else
                                            {
                                                string catAdded = AddNiaCategory(trans.TargetScriptPath, trans.TargetCategoryId, login.ECRServerAddr);
                                                if (string.IsNullOrEmpty(catAdded))
                                                {
                                                    success = false;
                                                }
                                                else
                                                    newTargetCatId = Convert.ToInt32(catAdded);
                                            }
                                        }
                                        else
                                        {
                                            newTargetCatId = targetCatExists.ExportSourceTargetMapping.TargetScriptCategoryId;
                                            updatedTargetCategoryPath = "$";
                                            if (trans.TargetCategoryId > 0)
                                            {
                                                targetCatPathList = new List<string>();
                                                updatedTargetCategoryPath = "$" + "\\" + FindTargetScriptPath(trans.TargetCategoryId);
                                            }
                                        }
                                        previousSourceCatId = trans.SourceCategoryId;
                                        previousTargetCatId = trans.TargetCategoryId;
                                        if (success)
                                        {
                                            string scriptAddedResult = "";

                                            // Check if this script already exists in ExportSourceTargetMappingDetails table
                                            GetExportSourceTargetMappingDetailsResMsg targetScriptExists = exportClient.ServiceChannel.GetExportSourceTargetMappingDetails(master.TargetServerId.ToString(), newTargetCatId.ToString(), trans.SourceCategoryId.ToString(), trans.SourceScriptId.ToString());
                                            if (targetScriptExists.ExportSourceTargetMapping == null)
                                            {
                                                // Create Nia Script
                                                try
                                                {
                                                    scriptAddedResult = AddNiaScript(trans.SourceCategoryId, trans.SourceScriptId, newTargetCatId, login.ECRServerAddr);
                                                }
                                                catch (Exception ex)
                                                {
                                                    LogEntry(ex.Message);
                                                }
                                                if (!string.IsNullOrEmpty(scriptAddedResult))
                                                {
                                                    AddExportSourceTargetMappingResMsg exportScriptTrnsResponse = null;
                                                    if (newTargetCatId > 0)
                                                    {
                                                        //Create an entry into ExportSourceTargetMapping table
                                                        AddExportSourceTargetMappingReqMsg exportTargetDetails = new AddExportSourceTargetMappingReqMsg();
                                                        exportTargetDetails.ExportSourceTargetMapping = new ExportSourceTargetMapping();
                                                        exportTargetDetails.ExportSourceTargetMapping.SourceInstanceAddr = uri.Host;
                                                        exportTargetDetails.ExportSourceTargetMapping.SourceScriptCategoryId = trans.SourceCategoryId;
                                                        exportTargetDetails.ExportSourceTargetMapping.SourceScriptId = trans.SourceScriptId;
                                                        exportTargetDetails.ExportSourceTargetMapping.SourceScriptVersion = trans.SourceScriptVersion;
                                                        exportTargetDetails.ExportSourceTargetMapping.TargetInstanceId = master.TargetServerId;
                                                        exportTargetDetails.ExportSourceTargetMapping.TargetScriptCategoryId = newTargetCatId;
                                                        exportTargetDetails.ExportSourceTargetMapping.TargetScriptId = int.Parse(scriptAddedResult);
                                                        exportTargetDetails.ExportSourceTargetMapping.TargetScriptVersion = trans.SourceScriptVersion;
                                                        exportTargetDetails.ExportSourceTargetMapping.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                                                        exportTargetDetails.ExportSourceTargetMapping.CreatedOn = System.DateTime.UtcNow;
                                                        exportScriptTrnsResponse = exportClient.ServiceChannel.AddExportSourceTargetMapping(exportTargetDetails);
                                                    }
                                                    if (exportScriptTrnsResponse != null && exportScriptTrnsResponse.IsSuccess)
                                                    {
                                                        // Update ExportTransactionDetails with Status Success e.g. 2
                                                        UpdateExportTransactionDetailsReqMsg reqTransSuccessUpdate = new UpdateExportTransactionDetailsReqMsg();
                                                        reqTransSuccessUpdate.ExportTransactionDetails = new ExportTransactionDetails();
                                                        reqTransSuccessUpdate.ExportTransactionDetails.id = trans.id;
                                                        reqTransSuccessUpdate.ExportTransactionDetails.Status = (int)TransactionStatus.Success;
                                                        reqTransSuccessUpdate.ExportTransactionDetails.Action = (int)Infosys.ATR.ExportUtility.Models.Action.Default;  //(int)ExportUtility.Models.Action.Default;
                                                        reqTransSuccessUpdate.ExportTransactionDetails.Details = "Script has been exported successfully.";
                                                        reqTransSuccessUpdate.ExportTransactionDetails.TargetScriptPath = updatedTargetCategoryPath;
                                                        reqTransSuccessUpdate.ExportTransactionDetails.ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                                                        reqTransSuccessUpdate.ExportTransactionDetails.ModifiedOn = System.DateTime.UtcNow;
                                                        var transUpdateResult = exportClient.ServiceChannel.UpdateExportTransactionDetails(reqTransSuccessUpdate);
                                                    }
                                                }
                                                else
                                                {
                                                    // Update ExportTransactionDetails with Failed Success e.g. 2
                                                    UpdateExportTransactionDetailsReqMsg reqTransSuccessUpdate = new UpdateExportTransactionDetailsReqMsg();
                                                    reqTransSuccessUpdate.ExportTransactionDetails = new ExportTransactionDetails();
                                                    reqTransSuccessUpdate.ExportTransactionDetails.id = trans.id;
                                                    reqTransSuccessUpdate.ExportTransactionDetails.Status = (int)TransactionStatus.Failed;
                                                    reqTransSuccessUpdate.ExportTransactionDetails.Action = (int)Infosys.ATR.ExportUtility.Models.Action.Default;  //(int)ExportUtility.Models.Action.Default;
                                                    reqTransSuccessUpdate.ExportTransactionDetails.Details = "An error occurred while adding script. ";
                                                    reqTransSuccessUpdate.ExportTransactionDetails.TargetScriptPath = updatedTargetCategoryPath;
                                                    reqTransSuccessUpdate.ExportTransactionDetails.ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                                                    reqTransSuccessUpdate.ExportTransactionDetails.ModifiedOn = System.DateTime.UtcNow;
                                                    var transUpdateResult = exportClient.ServiceChannel.UpdateExportTransactionDetails(reqTransSuccessUpdate);
                                                }
                                            }
                                            else // If script exists
                                            {
                                                // Update the logic to handle script exists conditions                                             
                                                string timezone = FindScriptById(targetScriptExists.ExportSourceTargetMapping.TargetScriptId.ToString(), client, login.ECRServerAddr);
                                                DateTime modifiedDate = System.DateTime.Now.AddYears(-1);
                                                if (!string.IsNullOrEmpty(timezone))
                                                    modifiedDate = ConvertUnixTimeStampToDateTime(Double.Parse(timezone));

                                                DateTime serverModifiedDate = targetScriptExists.ExportSourceTargetMapping.CreatedOn;
                                                if (targetScriptExists.ExportSourceTargetMapping.ModifiedOn != null)
                                                    serverModifiedDate = targetScriptExists.ExportSourceTargetMapping.ModifiedOn.GetValueOrDefault();
                                                else
                                                    serverModifiedDate = targetScriptExists.ExportSourceTargetMapping.CreatedOn;

                                                //DateTime serverModifiedDate_local = TimeZone.CurrentTimeZone.ToLocalTime(serverModifiedDate);
                                                serverModifiedDate = serverModifiedDate.AddHours(5);
                                                serverModifiedDate = serverModifiedDate.AddMinutes(30);

                                                // Nia server verion has been modified
                                                if (modifiedDate > serverModifiedDate)
                                                {
                                                    UpdateExportTransactionDetailsReqMsg reqTransSuccessUpdate = new UpdateExportTransactionDetailsReqMsg();
                                                    reqTransSuccessUpdate.ExportTransactionDetails = new ExportTransactionDetails();
                                                    reqTransSuccessUpdate.ExportTransactionDetails.id = trans.id;
                                                    reqTransSuccessUpdate.ExportTransactionDetails.Status = (int)TransactionStatus.Exist;
                                                    reqTransSuccessUpdate.ExportTransactionDetails.ExistReasonCode = (int)ExistsReasonCode.VM;
                                                    reqTransSuccessUpdate.ExportTransactionDetails.Details = "Server version has been modified";
                                                    reqTransSuccessUpdate.ExportTransactionDetails.ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                                                    reqTransSuccessUpdate.ExportTransactionDetails.ModifiedOn = System.DateTime.UtcNow;
                                                    var transUpdateResult = exportClient.ServiceChannel.UpdateExportTransactionDetails(reqTransSuccessUpdate);

                                                    UpdateExportSourceTargetMappingReqMsg exportTargetDetails = new UpdateExportSourceTargetMappingReqMsg();
                                                    exportTargetDetails.ExportSourceTargetMapping = new ExportSourceTargetMapping();
                                                    exportTargetDetails.ExportSourceTargetMapping.id = targetScriptExists.ExportSourceTargetMapping.id;
                                                    exportTargetDetails.ExportSourceTargetMapping.SourceScriptVersion = trans.SourceScriptVersion;
                                                    exportTargetDetails.ExportSourceTargetMapping.TargetScriptVersion = trans.SourceScriptVersion;
                                                    exportTargetDetails.ExportSourceTargetMapping.ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                                                    exportTargetDetails.ExportSourceTargetMapping.ModifiedOn = System.DateTime.UtcNow;
                                                    var exportScriptTrnsResponse = exportClient.ServiceChannel.UpdateExportSourceTargetMapping(exportTargetDetails);

                                                }
                                                // If both the versions are equal, no action required
                                                else if (trans.SourceScriptVersion == targetScriptExists.ExportSourceTargetMapping.TargetScriptVersion)
                                                {
                                                    UpdateExportTransactionDetailsReqMsg reqTransSuccessUpdate = new UpdateExportTransactionDetailsReqMsg();
                                                    reqTransSuccessUpdate.ExportTransactionDetails = new ExportTransactionDetails();
                                                    reqTransSuccessUpdate.ExportTransactionDetails.id = trans.id;
                                                    reqTransSuccessUpdate.ExportTransactionDetails.Status = (int)TransactionStatus.Exist;
                                                    //reqTransSuccessUpdate.ExportTransactionDetails.Action = (int)Infosys.ATR.ExportUtility.Models.Action.Ignore;
                                                    reqTransSuccessUpdate.ExportTransactionDetails.ExistReasonCode = (int)ExistsReasonCode.VU;
                                                    reqTransSuccessUpdate.ExportTransactionDetails.Details = "Server Version Matches";
                                                    reqTransSuccessUpdate.ExportTransactionDetails.ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                                                    reqTransSuccessUpdate.ExportTransactionDetails.ModifiedOn = System.DateTime.UtcNow;
                                                    var transUpdateResult = exportClient.ServiceChannel.UpdateExportTransactionDetails(reqTransSuccessUpdate);

                                                    UpdateExportSourceTargetMappingReqMsg exportTargetDetails = new UpdateExportSourceTargetMappingReqMsg();
                                                    exportTargetDetails.ExportSourceTargetMapping = new ExportSourceTargetMapping();
                                                    exportTargetDetails.ExportSourceTargetMapping.id = targetScriptExists.ExportSourceTargetMapping.id;
                                                    exportTargetDetails.ExportSourceTargetMapping.ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                                                    exportTargetDetails.ExportSourceTargetMapping.ModifiedOn = System.DateTime.UtcNow;
                                                    var exportScriptTrnsResponse = exportClient.ServiceChannel.UpdateExportSourceTargetMapping(exportTargetDetails);
                                                }
                                                // If script version in script central is greater than Nia version
                                                else if (trans.SourceScriptVersion > targetScriptExists.ExportSourceTargetMapping.TargetScriptVersion)
                                                {
                                                    UpdateExportSourceTargetMappingReqMsg exportTargetDetails = new UpdateExportSourceTargetMappingReqMsg();
                                                    exportTargetDetails.ExportSourceTargetMapping = new ExportSourceTargetMapping();
                                                    exportTargetDetails.ExportSourceTargetMapping.id = targetScriptExists.ExportSourceTargetMapping.id;
                                                    //exportTargetDetails.ExportSourceTargetMapping.SourceScriptVersion = trans.SourceScriptVersion;
                                                    //exportTargetDetails.ExportSourceTargetMapping.TargetScriptVersion = trans.SourceScriptVersion;
                                                    exportTargetDetails.ExportSourceTargetMapping.ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                                                    exportTargetDetails.ExportSourceTargetMapping.ModifiedOn = System.DateTime.UtcNow;
                                                    var exportScriptTrnsResponse = exportClient.ServiceChannel.UpdateExportSourceTargetMapping(exportTargetDetails);

                                                    UpdateExportTransactionDetailsReqMsg reqTransSuccessUpdate = new UpdateExportTransactionDetailsReqMsg();
                                                    reqTransSuccessUpdate.ExportTransactionDetails = new ExportTransactionDetails();
                                                    reqTransSuccessUpdate.ExportTransactionDetails.id = trans.id;
                                                    reqTransSuccessUpdate.ExportTransactionDetails.Status = (int)TransactionStatus.Exist;
                                                    //reqTransSuccessUpdate.ExportTransactionDetails.Action = (int)Infosys.ATR.ExportUtility.Models.Action.Overwrite;
                                                    reqTransSuccessUpdate.ExportTransactionDetails.ExistReasonCode = (int)ExistsReasonCode.VO;
                                                    reqTransSuccessUpdate.ExportTransactionDetails.Details = "Server version is older";
                                                    reqTransSuccessUpdate.ExportTransactionDetails.ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                                                    reqTransSuccessUpdate.ExportTransactionDetails.ModifiedOn = System.DateTime.UtcNow;
                                                    var transUpdateResult = exportClient.ServiceChannel.UpdateExportTransactionDetails(reqTransSuccessUpdate);
                                                }
                                            }

                                        }

                                        else
                                        {
                                            // Update ExportTransactionDetails with Status Failed e.g. 4
                                            UpdateExportTransactionDetailsReqMsg reqTransSuccessUpdate = new UpdateExportTransactionDetailsReqMsg();
                                            reqTransSuccessUpdate.ExportTransactionDetails = new ExportTransactionDetails();
                                            reqTransSuccessUpdate.ExportTransactionDetails.id = trans.id;
                                            reqTransSuccessUpdate.ExportTransactionDetails.Status = (int)TransactionStatus.Failed;
                                            reqTransSuccessUpdate.ExportTransactionDetails.Action = (int)Infosys.ATR.ExportUtility.Models.Action.Default; //(int)ExportUtility.Models.Action.Default;
                                            reqTransSuccessUpdate.ExportTransactionDetails.Details = "An error has ocurred";
                                            var transUpdateResult = exportClient.ServiceChannel.UpdateExportTransactionDetails(reqTransSuccessUpdate);
                                        }

                                        //}
                                    }
                                }
                            }
                        }

                    }
                    // If export status is resubmitted
                    else if (master.ExportStatus == (int)ExportStatus.Resubmit)
                    {
                        // Set Export Status to In Progress
                        UpdateExportConfigurationMasterReqMsg reqMaster = new UpdateExportConfigurationMasterReqMsg();
                        reqMaster.ExportConfigurationMaster = new WEM.Export.Service.Contracts.Data.ExportConfigurationMaster();
                        reqMaster.ExportConfigurationMaster.id = master.id;
                        reqMaster.ExportConfigurationMaster.ExportStatus = (int)ExportStatus.InProgress;
                        reqMaster.ExportConfigurationMaster.ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                        reqMaster.ExportConfigurationMaster.ModifiedOn = System.DateTime.UtcNow;
                        //reqMaster.ExportConfigurationMaster.ScriptRepositoryBaseServerAddress = uri.Host;
                        var exportUpdateStatus = exportClient.ServiceChannel.UpdateExportConfigurationMaster(reqMaster);

                        // Get all the records from ExportScriptConfigurationDetails based on  ExportConfigurationId.
                        var exportScriptConfig = exportClient.ServiceChannel.GetExportScriptConfigurationDetails(reqMaster.ExportConfigurationMaster.id.ToString());
                        foreach (var script in exportScriptConfig.ExportScriptConfigurationDetails)
                        {
                            // Get all the transaction details based on exportScriptConfigId where status is exists
                            var transDetails = exportClient.ServiceChannel.GetAllExportTransactionDetails(script.id.ToString(), ((short)TransactionStatus.Exist).ToString());

                            // Loop through all ExportTransactionDetails where status = 0
                            foreach (var trans in transDetails.ExportTransactionDetails.ToList())
                            {
                                UpdateExportTransactionDetailsReqMsg reqTrans = new UpdateExportTransactionDetailsReqMsg();
                                reqTrans.ExportTransactionDetails = new ExportTransactionDetails();
                                reqTrans.ExportTransactionDetails.id = trans.id;
                                reqTrans.ExportTransactionDetails.Status = (int)TransactionStatus.InProgress;

                                // Update status to 1-InProgress in ExportTransactionDetails. 
                                var transResult = exportClient.ServiceChannel.UpdateExportTransactionDetails(reqTrans);

                                if (trans.Action == (short)ATR.ExportUtility.Models.Action.CreateNew)
                                {
                                    var scriptAddedResult = AddNiaScript(trans.SourceCategoryId, trans.SourceScriptId, trans.TargetCategoryId, login.ECRServerAddr, true);

                                    // If script not added successfully
                                    if (string.IsNullOrEmpty(scriptAddedResult))
                                    {
                                        UpdateExportTransactionDetailsReqMsg reqTransSuccessUpdate = new UpdateExportTransactionDetailsReqMsg();
                                        reqTransSuccessUpdate.ExportTransactionDetails = new ExportTransactionDetails();
                                        reqTransSuccessUpdate.ExportTransactionDetails.Status = (int)TransactionStatus.Failed;
                                        reqTransSuccessUpdate.ExportTransactionDetails.Details = "An error has occurred.";
                                        var transUpdateResult = exportClient.ServiceChannel.UpdateExportTransactionDetails(reqTransSuccessUpdate);
                                    }
                                    else
                                    {
                                        //Create an entry into ExportSourceTargetMapping table
                                        AddExportSourceTargetMappingReqMsg exportTargetDetails = new AddExportSourceTargetMappingReqMsg();
                                        exportTargetDetails.ExportSourceTargetMapping = new ExportSourceTargetMapping();
                                        exportTargetDetails.ExportSourceTargetMapping.SourceInstanceAddr = uri.Host;
                                        exportTargetDetails.ExportSourceTargetMapping.SourceScriptCategoryId = trans.SourceCategoryId;
                                        exportTargetDetails.ExportSourceTargetMapping.SourceScriptId = trans.SourceScriptId;
                                        exportTargetDetails.ExportSourceTargetMapping.SourceScriptVersion = trans.SourceScriptVersion;
                                        exportTargetDetails.ExportSourceTargetMapping.TargetInstanceId = master.TargetServerId;
                                        exportTargetDetails.ExportSourceTargetMapping.TargetScriptCategoryId = trans.TargetCategoryId;
                                        exportTargetDetails.ExportSourceTargetMapping.TargetScriptId = int.Parse(scriptAddedResult);
                                        exportTargetDetails.ExportSourceTargetMapping.TargetScriptVersion = trans.SourceScriptVersion;
                                        exportTargetDetails.ExportSourceTargetMapping.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                                        exportTargetDetails.ExportSourceTargetMapping.CreatedOn = System.DateTime.UtcNow;
                                        var exportScriptTrnsResponse = exportClient.ServiceChannel.AddExportSourceTargetMapping(exportTargetDetails);

                                        // Update existing transaction
                                        UpdateExportTransactionDetailsReqMsg reqTransSuccessUpdate = new UpdateExportTransactionDetailsReqMsg();
                                        reqTransSuccessUpdate.ExportTransactionDetails = new ExportTransactionDetails();
                                        reqTransSuccessUpdate.ExportTransactionDetails.id = trans.id;
                                        reqTransSuccessUpdate.ExportTransactionDetails.Action = (int)Infosys.ATR.ExportUtility.Models.Action.CreateNew;
                                        reqTransSuccessUpdate.ExportTransactionDetails.Status = (int)TransactionStatus.Success;
                                        reqTransSuccessUpdate.ExportTransactionDetails.Details = "Script created successfully.";
                                        reqTransSuccessUpdate.ExportTransactionDetails.TargetScriptName = newScriptName;
                                        var transUpdateResult = exportClient.ServiceChannel.UpdateExportTransactionDetails(reqTransSuccessUpdate);

                                    }
                                }
                                else if (trans.Action == (short)ATR.ExportUtility.Models.Action.Overwrite)
                                {
                                    GetExportSourceTargetMappingDetailsResMsg targetScriptExists = exportClient.ServiceChannel.GetExportSourceTargetMappingDetails(master.TargetServerId.ToString(), trans.TargetCategoryId.ToString(), trans.SourceScriptId.ToString());
                                    string scriptUpdatedResult = UpdateNiaScript(trans.SourceCategoryId, trans.SourceScriptId, trans.TargetCategoryId, login.ECRServerAddr, targetScriptExists.ExportSourceTargetMapping.TargetScriptId);

                                    UpdateExportSourceTargetMappingReqMsg exportTargetDetails = new UpdateExportSourceTargetMappingReqMsg();
                                    exportTargetDetails.ExportSourceTargetMapping = new ExportSourceTargetMapping();
                                    exportTargetDetails.ExportSourceTargetMapping.id = targetScriptExists.ExportSourceTargetMapping.id;
                                    exportTargetDetails.ExportSourceTargetMapping.SourceScriptVersion = trans.SourceScriptVersion;
                                    exportTargetDetails.ExportSourceTargetMapping.TargetScriptVersion = trans.SourceScriptVersion;
                                    exportTargetDetails.ExportSourceTargetMapping.ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                                    exportTargetDetails.ExportSourceTargetMapping.ModifiedOn = System.DateTime.UtcNow;
                                    var exportScriptTrnsResponse = exportClient.ServiceChannel.UpdateExportSourceTargetMapping(exportTargetDetails);

                                    // update transaction table
                                    UpdateExportTransactionDetailsReqMsg reqTransSuccessUpdate = new UpdateExportTransactionDetailsReqMsg();
                                    reqTransSuccessUpdate.ExportTransactionDetails = new ExportTransactionDetails();
                                    reqTransSuccessUpdate.ExportTransactionDetails.id = trans.id;
                                    reqTransSuccessUpdate.ExportTransactionDetails.Action = (int)Infosys.ATR.ExportUtility.Models.Action.Overwrite;
                                    reqTransSuccessUpdate.ExportTransactionDetails.Status = (int)TransactionStatus.Success;
                                    reqTransSuccessUpdate.ExportTransactionDetails.Details = "Script overwritten successfully.";
                                    var transUpdateResult = exportClient.ServiceChannel.UpdateExportTransactionDetails(reqTransSuccessUpdate);

                                }
                                else if (trans.Action == (short)ATR.ExportUtility.Models.Action.Ignore)
                                {
                                    UpdateExportTransactionDetailsReqMsg reqTransSuccessUpdate = new UpdateExportTransactionDetailsReqMsg();
                                    reqTransSuccessUpdate.ExportTransactionDetails = new ExportTransactionDetails();
                                    reqTransSuccessUpdate.ExportTransactionDetails.id = trans.id;
                                    reqTransSuccessUpdate.ExportTransactionDetails.Action = (int)Infosys.ATR.ExportUtility.Models.Action.Ignore;
                                    reqTransSuccessUpdate.ExportTransactionDetails.Status = (int)TransactionStatus.NA;
                                    reqTransSuccessUpdate.ExportTransactionDetails.Details = "No action taken on script.";
                                    //reqTransSuccessUpdate.ExportTransactionDetails.ExistReasonCode = trans.ExistReasonCode; 
                                    reqTransSuccessUpdate.ExportTransactionDetails.ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                                    reqTransSuccessUpdate.ExportTransactionDetails.ModifiedOn = System.DateTime.UtcNow;
                                    var transUpdateResult = exportClient.ServiceChannel.UpdateExportTransactionDetails(reqTransSuccessUpdate);
                                }
                            }
                        }

                    }
                    final_result = true;
                    //Update ExportConfigurationMaster CompletedOn and ExportStatus to Success or Failed 
                    UpdateExportConfigurationMasterReqMsg reqUpdateMaster = new UpdateExportConfigurationMasterReqMsg();
                    reqUpdateMaster.ExportConfigurationMaster = new WEM.Export.Service.Contracts.Data.ExportConfigurationMaster();
                    reqUpdateMaster.ExportConfigurationMaster.id = master.id;
                    if (final_result)
                    {
                        if (pending_conflicts == true)
                            reqUpdateMaster.ExportConfigurationMaster.ExportStatus = (int)ExportStatus.Pending_Conflicts;
                        else
                            reqUpdateMaster.ExportConfigurationMaster.ExportStatus = (int)ExportStatus.Success;
                    }
                    else
                        reqUpdateMaster.ExportConfigurationMaster.ExportStatus = (int)ExportStatus.Failed;
                    reqUpdateMaster.ExportConfigurationMaster.CompletedOn = System.DateTime.UtcNow;
                    reqUpdateMaster.ExportConfigurationMaster.ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    reqUpdateMaster.ExportConfigurationMaster.ModifiedOn = System.DateTime.UtcNow;
                    var exportUpdateFinalStatus = exportClient.ServiceChannel.UpdateExportConfigurationMaster(reqUpdateMaster);

                    // Update CompletedOn in ExportJobProcessingHistory
                    UpdateExportJobProcessingHistoryReqMsg updateJobReq = new UpdateExportJobProcessingHistoryReqMsg();
                    updateJobReq.ExportJobProcessingHistory = new ExportJobProcessingHistory();
                    updateJobReq.ExportJobProcessingHistory.JobId = jobId;
                    updateJobReq.ExportJobProcessingHistory.CompletedOn = System.DateTime.Now;
                    var resUpdateJob = exportClient.ServiceChannel.UpdateExportJobProcessingHistory(updateJobReq);
                }

            }
            //}
            catch (Exception ex)
            {
                LogEntry("Exception in RunJob Method:" + ex.Message + ex.StackTrace);
                if (ex.InnerException != null)
                    LogEntry("Inner Exception:" + ex.InnerException.Message + ex.InnerException.StackTrace);
                throw new Exception(ex.Message);
            }
            LogEntry("Exting RunJob() Method");
        }
        private static void GetAllSourceScriptDetails(List<ScriptRepository.Models.Category> sourceCats, ExportScriptConfigurationDetails scriptConfig)
        {
            string targetScriptPath = "";
            int lastIndex = scriptConfig.SourceScriptPath.LastIndexOf("\\");
            if (lastIndex > 0)
                targetScriptPath = scriptConfig.SourceScriptPath.Substring(lastIndex);

            foreach (var cat in sourceCats)
            {
                sourceParentCatPath = "$";
                sourceParentCatPath = FindSourceParentCategoryPath(cat) + "\\" + cat.Name;
                //int index = sourceParentCatPath.IndexOf(scriptConfig.SourceScriptPath);
                if (string.IsNullOrEmpty(targetScriptPath))
                    targetScriptPath = scriptConfig.TargetScriptPath;
                else
                    targetScriptPath = scriptConfig.TargetScriptPath + sourceParentCatPath.Substring(lastIndex);
                //targetScriptPath=targetScriptPath + "\\" + cat.Name;

                var scripts = scriptClient.ServiceChannel.GetAllScriptDetails(cat.Id);
                if (scripts.Scripts != null && scripts.Scripts.Count > 0)
                {
                    foreach (var script in scripts.Scripts)
                    {
                        ExportTransactionDetails record = new ExportTransactionDetails();
                        record.ExportScriptConfigurationId = scriptConfig.id;
                        record.SourceCategoryId = script.CategoryId;
                        record.SourceScriptId = script.ScriptId;
                        record.TargetCategoryId = scriptConfig.TargetCategoryId;
                        record.TargetScriptId = scriptConfig.SourceScriptId;
                        if (script.ScriptFileVersion > 0)
                            record.TargetScriptVersion = script.ScriptFileVersion;
                        else
                            record.TargetScriptVersion = 1;
                        record.TargetScriptName = script.Name;
                        record.SourceScriptVersion = script.ScriptFileVersion;
                        record.SourceScriptPath = sourceParentCatPath;
                        record.TargetScriptPath = targetScriptPath;
                        record.Status = (short)TransactionStatus.NA;
                        record.isActive = true;
                        record.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                        record.CreatedOn = System.DateTime.UtcNow;
                        exportTransactionScriptDetailsList.ExportTransactionDetails.Add(record);

                    }
                }
            }
        }
        private static string FindSourceParentCategoryPath(Infosys.ATR.ScriptRepository.Models.Category cat)
        {
            if (cat.ParentId > 0)
            {
                var findParentCategory = categories.Where(c => c.Id == cat.ParentId.ToString()).FirstOrDefault();
                if (findParentCategory != null)
                {
                    FindSourceParentCategoryPath(findParentCategory);
                    sourceParentCatPath = sourceParentCatPath + "\\" + findParentCategory.Name;
                }
            }
            return sourceParentCatPath;
        }

        private static void FindAllSourceChildCategories(int sourceCatId)
        {
            var resp = categories.Where(c => c.ParentId == sourceCatId).ToList();
            if (resp != null && resp.Count > 0)
            {
                resp.ForEach(cat =>
                {
                    childCatList.Add(cat);
                    FindAllSourceChildCategories(int.Parse(cat.Id));
                });
            }
            // return childCatList;
        }

        public static NIACategory FindCategoryUpdated(string catName, List<NIACategory> catList)
        {
            foreach (var cat in catList)
            {
                if (cat.Name.Equals(catName))
                {
                    return cat;
                }
            }

            foreach (var cat in catList)
            {
                if (cat.Children != null && cat.Children.Count > 0)
                {
                    //foreach (NIACategory child in cat.Children)
                    {
                        var result = FindCategoryUpdated(catName, cat.Children);
                        return result;
                    }
                }
            }


            return null;
        }

        private static NIACategory CheckCategoryExists(NIACategory category, string childCatName)
        {
            if (category.Name.Contains(childCatName))
            {
                return category;
            }

            NIACategory result = null;
            if (category.Children != null && category.Children.Count > 0)
            {
                foreach (NIACategory child in category.Children)
                {
                    result = CheckCategoryExists(child, childCatName);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        private static string AddNiaCategory(string scriptPath, int targetCatId, string serverAddress)
        {
            System.Threading.Thread.Sleep(2000);
            LogEntry("Entering AddNiaCategory() Method");
            string result = "";
            List<string> catList = scriptPath.Split('\\').ToList();
            int index = 0;
            NIACategory catExists = null;
            NIACategory parentCategory = null;
            updatedTargetCategoryPath = "$";
            Boolean catNameAlreadyExists = false;
            string newCatName = "";
            NIACategory parentCategoryHierarchy = null;
            List<string> last = null;

            foreach (string cat in catList)
            {
                // skip cat with $ name
                if (index > 0)
                {
                    catNameAlreadyExists = false;
                    newCatName = "";

                    if (index == 1)
                    {
                        //parentCategoryHierarchy = ecrCategories.Where(c => c.Name.Equals(cat)).FirstOrDefault();
                        var cats = ecrCategoriesWithParentId.FindAll(s => s.Name.IndexOf(cat, StringComparison.OrdinalIgnoreCase) >= 0 && s.ParentId == 0).Select(s => s).ToList();
                        if (cats.Count > 0)
                            parentCategoryHierarchy = cats[cats.Count - 1];
                    }
                    else if (index > 1)
                    {
                        parentCategory = parentCategoryHierarchy;
                        var findCat = CheckCategoryExists(parentCategoryHierarchy, cat);
                        parentCategoryHierarchy = null;
                        if (findCat != null && !string.IsNullOrEmpty(findCat.Id))
                            parentCategoryHierarchy = findCat;
                    }

                    if (parentCategoryHierarchy == null)
                    {
                        //if (ecrCatNameList.Any(s => s.Equals(cat, StringComparison.OrdinalIgnoreCase)))
                        if (ecrCategoriesWithParentId.Any(s => s.Name.Equals(cat, StringComparison.OrdinalIgnoreCase)))
                        {
                            catNameAlreadyExists = true;
                            //last = ecrCatNameList.Where(i => i.Contains(cat + "_")).ToList();
                            string searchKeyword = cat + "_";
                            last = ecrCategoriesWithParentId.FindAll(s => s.Name.IndexOf(searchKeyword, StringComparison.OrdinalIgnoreCase) >= 0).Select(s => s.Name).ToList();
                            if (last.Count > 0)
                                newCatName = last[last.Count - 1];
                            else
                                newCatName = cat;
                        }
                    }
                    else
                    {
                        updatedTargetCategoryPath = updatedTargetCategoryPath + "\\" + parentCategoryHierarchy.Name;
                    }

                    if (targetCatId == 0)
                    {
                        if (parentCategoryHierarchy == null)
                        {
                            string catName = cat;
                            if (catNameAlreadyExists)
                                catName = CreateCategoryName(newCatName);
                            updatedTargetCategoryPath = updatedTargetCategoryPath + "\\" + catName;
                            ATR.ExportUtility.Models.ECR.ECRCategory c = new ATR.ExportUtility.Models.ECR.ECRCategory();
                            c.categoryName = catName;
                            result = ecrService.AddScriptCategory(serverAddress, c);
                            if (result == null)
                            {
                                throw new Exception("Unable to add category " + c.categoryName);
                            }
                            parentCategoryHierarchy = new NIACategory { Id = result, Name = catName };
                            ecrCategories.Add(parentCategoryHierarchy);
                            ecrCategoriesWithParentId.Add(parentCategoryHierarchy);
                        }
                    }
                    else
                    {
                        if (parentCategoryHierarchy == null)
                        {

                            // Cat does not exist, create it in Nia
                            string catName = cat;
                            if (catNameAlreadyExists)
                            {
                                if (string.IsNullOrEmpty(newCatName))
                                    catName = CreateCategoryName(newCatName);
                                else
                                    catName = CreateCategoryName(newCatName);
                            }
                            updatedTargetCategoryPath = updatedTargetCategoryPath + "\\" + catName;
                            ATR.ExportUtility.Models.ECR.ECRCategory c = new ATR.ExportUtility.Models.ECR.ECRCategory();
                            c.parentCategoryId = targetCatId;
                            //c.categoryName = cat;
                            c.categoryName = catName;
                            c.description = catName;

                            result = ecrService.AddScriptCategory(serverAddress, c);
                            if (result == null)
                            {
                                LogEntry("AddNiaCategory() - Retrying:" + updatedTargetCategoryPath);
                                //catName = CreateCategoryName(newCatName);

                                //int i = updatedTargetCategoryPath.LastIndexOf("\\");
                                //updatedTargetCategoryPath = updatedTargetCategoryPath.Substring(0, i);
                                //updatedTargetCategoryPath = updatedTargetCategoryPath + "\\" + catName;
                                //c.categoryName = catName;
                                //c.description = catName;
                                result = ecrService.AddScriptCategory(serverAddress, c);
                                LogEntry("AddNiaCategory() - Retried worked successfully.");
                            }
                            if (result == null)
                                throw new Exception("An error occurred while adding category " + updatedTargetCategoryPath + " on Nia instance");

                            parentCategoryHierarchy = new NIACategory { Id = result, Name = catName };

                            if (parentCategory.Children == null)
                                parentCategory.Children = new List<NIACategory>();
                            parentCategory.Children.Add(parentCategoryHierarchy);
                            ecrCategoriesWithParentId.Add(parentCategoryHierarchy);
                        }
                        else
                        {
                            // Cat exists, skip this
                        }
                    }
                    if (parentCategoryHierarchy != null)
                    {
                        targetCatId = int.Parse(parentCategoryHierarchy.Id);
                        result = parentCategoryHierarchy.Id;
                        newTargetCatId = targetCatId;
                    }

                }
                index = index + 1;
            }
            LogEntry("AddNiaCategory() - Updated category path:" + updatedTargetCategoryPath);
            LogEntry("Exiting AddNiaCategory() Method");
            return result;
        }

        private static NIACategory FindCategory(NIACategory category, string childCatName)
        {
            if (category.Name.Equals(childCatName))
            //if (category.Name.Contains(childCatName))
            {
                return category;
            }

            NIACategory result = null;

            foreach (NIACategory child in category.Children)
            {
                result = FindCategory(child, childCatName);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
        private static string AddNiaScript(int sourceCatId, int scriptId, int targetCategoryId, string serverAddress, Boolean exists = false)
        {
            LogEntry("Entering AddNiaScript() Method");
            string result = "";
            var response = scriptClient.ServiceChannel.GetScriptDetails(scriptId.ToString(), sourceCatId.ToString());
            if (response.ScriptDetails != null)
            {
                var script = Infosys.ATR.ScriptRepository.Translators.ScriptPE_SE.ScriptSEtoPE(response.ScriptDetails);
                //string stoarageBaseUrl = WEMClient.CommonServices.Instance.StorageBaseURL;
                //string scriptUrl = stoarageBaseUrl + script.ScriptLocation;
                if (!string.IsNullOrEmpty(script.Version))
                    sourceScriptVersion = int.Parse(script.Version);
                else
                    sourceScriptVersion = 1;

                ScriptVO scriptVO = new ScriptVO();
                if (exists)
                {
                    newScriptName = GetScriptName(script.Name);
                    scriptVO.name = newScriptName;
                }
                else
                    scriptVO.name = script.Name;
                //scriptVO.description = script.Description;
                // As Nia does not support long description
                string desc = script.Description;
                if (string.IsNullOrEmpty(desc))
                {
                    scriptVO.description = scriptVO.name;
                }
                else
                {
                    if (desc != null && desc.Length > 100)
                        desc = desc.Substring(0, 100);

                    scriptVO.description = RemoveSpecialCharacters(desc);
                }

                scriptVO.scriptType = script.ScriptType;
                scriptVO.namedParamFormat = GetParameterType(script.ScriptType);
                scriptVO.argString = script.ArgumentString;
                //scriptVO.scriptBlob = GetScriptContent(script.ScriptLocation);
                scriptVO.scriptBlob = GetScriptContent(script);

                List<categoryVO> categoryVOList = new List<categoryVO>();
                categoryVO categoryVO = new categoryVO();
                categoryVO.categoryId = targetCategoryId; ;
                categoryVOList.Add(categoryVO);
                //scriptVO.categoryVOList = new List<categoryVO>();
                scriptVO.categoryVOList = categoryVOList;

                List<scriptParamsVO> scriptParamsVOList = new List<scriptParamsVO>();
                if (script.Parameters != null && script.Parameters.Count > 0)
                {
                    foreach (var parameter in script.Parameters)
                    {
                        scriptParamsVO scriptParamsVO = new scriptParamsVO();
                        if (parameter.IsMandatory)
                            scriptParamsVO.isMandatory = 1;
                        else
                            scriptParamsVO.isMandatory = 0;
                        scriptParamsVO.allowedValues = parameter.AllowedValues;
                        scriptParamsVO.defParamValue = parameter.DefaultValue;
                        scriptParamsVO.paramName = parameter.Name;
                        scriptParamsVOList.Add(scriptParamsVO);
                    }
                    scriptVO.scriptParamVOList = scriptParamsVOList;
                }

                List<string> serviceAreas = new List<string>();
                string serviceArea = ATR.ExportUtility.Constants.Application.ServiceArea;
                serviceAreas.Add(serviceArea);
                scriptVO.serviceAreas = serviceAreas;
                string postBody = Newtonsoft.Json.JsonConvert.SerializeObject(scriptVO);
                result = ecrService.AddScript(serverAddress, postBody);
            }
            LogEntry("Exiting AddNiaScript() Method");
            return result;
        }

        private static string UpdateNiaScript(int sourceCatId, int sourceScriptId, int targetCategoryId, string serverAddress, int targetScriptId)
        {
            LogEntry("Entering UpdateNiaScript() Method");
            string result = "";
            var response = scriptClient.ServiceChannel.GetScriptDetails(sourceScriptId.ToString(), sourceCatId.ToString());
            if (response.ScriptDetails != null)
            {
                var script = Infosys.ATR.ScriptRepository.Translators.ScriptPE_SE.ScriptSEtoPE(response.ScriptDetails);
                //if (!string.IsNullOrEmpty(script.Version))
                //    sourceScriptVersion = int.Parse(script.Version);

                ScriptVO scriptVO = new ScriptVO();
                scriptVO.id = targetScriptId;
                scriptVO.name = script.Name;
                //scriptVO.description = script.Description;
                // As Nia does not support long description
                string desc = script.Description;
                if (string.IsNullOrEmpty(desc))
                {
                    scriptVO.description = scriptVO.name;
                }
                else
                {
                    if (desc != null && desc.Length > 100)
                        desc = desc.Substring(0, 100);

                    scriptVO.description = RemoveSpecialCharacters(desc);
                }

                scriptVO.scriptType = script.ScriptType;
                scriptVO.namedParamFormat = GetParameterType(script.ScriptType);
                scriptVO.argString = script.ArgumentString;
                scriptVO.scriptBlob = GetScriptContent(script);

                List<categoryVO> categoryVOList = new List<categoryVO>();
                categoryVO categoryVO = new categoryVO();
                categoryVO.categoryId = targetCategoryId; ;
                categoryVOList.Add(categoryVO);
                //scriptVO.categoryVOList = new List<categoryVO>();
                scriptVO.categoryVOList = categoryVOList;

                List<scriptParamsVO> scriptParamsVOList = new List<scriptParamsVO>();
                if (script.Parameters != null && script.Parameters.Count > 0)
                {
                    foreach (var parameter in script.Parameters)
                    {
                        scriptParamsVO scriptParamsVO = new scriptParamsVO();
                        if (parameter.IsMandatory)
                            scriptParamsVO.isMandatory = 1;
                        else
                            scriptParamsVO.isMandatory = 0;
                        scriptParamsVO.allowedValues = parameter.AllowedValues;
                        scriptParamsVO.defParamValue = parameter.DefaultValue;
                        scriptParamsVO.paramName = parameter.Name;
                        scriptParamsVOList.Add(scriptParamsVO);
                    }
                    scriptVO.scriptParamVOList = scriptParamsVOList;
                }

                List<string> serviceAreas = new List<string>();
                string serviceArea = Infosys.ATR.ExportUtility.Constants.Application.ServiceArea;
                serviceAreas.Add(serviceArea);
                scriptVO.serviceAreas = serviceAreas;
                string postBody = Newtonsoft.Json.JsonConvert.SerializeObject(scriptVO);
                result = ecrService.UpdateScript(serverAddress, postBody);

            }
            LogEntry("Exiting UpdateNiaScript() Method");
            return result;
        }

        //private static string GetScriptContent(string scriptUrl)
        //{
        //    string content = "";
        //    scriptUrl = WEMClient.CommonServices.Instance.StorageBaseURL + scriptUrl; // TODO
        //    try
        //    {
        //        System.IO.File.AppendAllText(@"D:\Logs\GetScriptContent.txt", "url:" + scriptUrl);
        //        WebRequest request = WebRequest.Create(scriptUrl);
        //        CredentialCache credential = new CredentialCache();
        //        credential.Add(new Uri(scriptUrl), "NTLM", CredentialCache.DefaultNetworkCredentials);
        //        request.Credentials = credential;

        //        using (var response = (HttpWebResponse)request.GetResponse())
        //        {
        //            string data = JsonConvert.SerializeObject(response);
        //            System.IO.File.AppendAllText(@"D:\Logs\GetScriptContent.txt", data);
        //            //var encoding = Encoding.GetEncoding(response.CharacterSet);

        //            using (var responseStream = response.GetResponseStream())
        //            using (var reader = new System.IO.StreamReader(responseStream, Encoding.UTF8))
        //                content = reader.ReadToEnd();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string err = "There is an error downloading the Script.";
        //        err = err + "\nMore Infomation- " + ex.Message;
        //        if (ex.InnerException != null)
        //            err = err + ". \nInner Exception- " + ex.InnerException.Message;
        //        System.IO.File.AppendAllText(@"D:\Logs\GetScriptContent.txt", err + ex.StackTrace);
        //        //Loggging - TODO
        //        // MessageBox.Show(err, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //    return content;
        //}

        private static string GetScriptContent(ScriptRepository.Models.Script script)
        {
            LogEntry("Entering GetScriptContent() Method");
            string content = "";
            string scriptUrl = WEMClient.CommonServices.Instance.StorageBaseURL + script.SourceUrl;
            try
            {
                Infosys.ATR.RepositoryAccess.Entity.ScriptDoc scriptDoc = new RepositoryAccess.Entity.ScriptDoc();

                Infosys.WEM.Scripts.Service.Contracts.Data.Script scriptdata = new WEM.Scripts.Service.Contracts.Data.Script();

                scriptdata.CategoryId = int.Parse(script.CategoryId);
                scriptdata.ScriptId = int.Parse(script.Id);
                scriptdata.ScriptURL = script.ScriptLocation;
                scriptdata.Name = script.Name;
                int companyid = int.Parse(System.Configuration.ConfigurationManager.AppSettings["Company"]);
                scriptdata.BelongsToOrg = companyid.ToString("00000");
                scriptdata.StorageBaseUrl = WEMClient.CommonServices.Instance.StorageBaseURL;

                scriptdata.ScriptFileVersion = int.Parse(script.Version);

                WEM.ScriptExecutionLibrary.ScriptIndentifier identifier = new WEM.ScriptExecutionLibrary.ScriptIndentifier();
                identifier.SubCategoryId = int.Parse(script.CategoryId);
                identifier.ScriptId = int.Parse(script.Id);

                scriptDoc = WEM.ScriptExecutionLibrary.Translator.Script_DocumentEntity.ScriptToDocument(scriptdata, identifier);
                Infosys.ATR.RepositoryAccess.FileRepository.ScriptRepositoryDS scriptDocDs = new Infosys.ATR.RepositoryAccess.FileRepository.ScriptRepositoryDS();

                scriptDoc = scriptDocDs.Download(scriptDoc);

                if (scriptDoc.File != null)
                {
                    scriptDoc.File.Seek(0, SeekOrigin.Begin);
                    content = StreamToString(scriptDoc.File);
                }
                else
                {
                    LogEntry("Script cannot be downloaded");
                    throw new Exception("Script cannot be downloaded");
                }

            }
            catch (Exception ex)
            {
                string err = "There is an error downloading the Script.";
                err = err + "\nMore Infomation- " + ex.Message;
                if (ex.InnerException != null)
                    err = err + ". \nInner Exception- " + ex.InnerException.Message;
                LogEntry("Exception in GetScriptContent() Method:" + err);
                throw new Exception(err);
            }
            LogEntry("Exiting GetScriptContent() Method");
            return content;
        }

        private static string GetParameterType(string scriptType)
        {
            string parameterType = "";
            switch (scriptType)
            {
                case "ae":
                    parameterType = "0";
                    break;
                case "bat":
                    parameterType = "1";
                    break;
                case "vbs":
                    parameterType = "2";
                    break;
                case "ps1":
                    parameterType = "3";
                    break;
                case "sh":
                    parameterType = "4";
                    break;
                default:
                    parameterType = "-1";
                    break;
            }
            return parameterType;
        }

        private static string FindScriptById(string scriptId, Infosys.Nia.Services.RestClient client, string serverAddress)
        {
            LogEntry("Entering FindScriptById Method");
            string timeZone = "";
            List<string> serviceAreas = new List<string>();
            Dictionary<string, string> requestMap = new Dictionary<string, string>();
            requestMap.Add("Content-Type", "application/json");
            serviceAreas.Add(Infosys.ATR.ExportUtility.Constants.Application.ServiceArea);
            ATR.ExportUtility.Models.PathVariablesVO pathVO = new ATR.ExportUtility.Models.PathVariablesVO();
            Dictionary<string, string> map = new Dictionary<string, string>();
            map.Add("scriptId", scriptId);
            pathVO.pathVariableMap = map;
            pathVO.serviceAreas = serviceAreas;

            try
            {
                string data = JsonConvert.SerializeObject(pathVO);
                var resultsGet = ecrService.FindScriptById(serverAddress, data, requestMap);
                var json = (JObject)JsonConvert.DeserializeObject(resultsGet.ToString());
                timeZone = json["lastUpdateTime"].Value<string>();
            }
            catch (Exception ex)
            {
                LogEntry("FindScriptById() Exception:" + ex.Message);
                LogEntry(ex.Message);
            }
            LogEntry("Exiting FindScriptById Method");
            return timeZone;
        }

        private static DateTime ConvertUnixTimeStampToDateTime(double unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            //DateTime ut = DateTime.SpecifyKind(dtDateTime, DateTimeKind.Utc);
            return dtDateTime;
        }

        private static string GetScriptName(string scriptName)
        {
            string updatedScriptName = scriptName;
            string exists = scriptName.Substring(scriptName.Length - 3);
            if (exists.Contains("_"))
            {
                int index = exists.LastIndexOf("_");
                string version = exists.Substring(index);
                int n;
                bool isNumeric = int.TryParse(version, out n);
                if (isNumeric)
                    updatedScriptName = scriptName.Substring(0, scriptName.Length - 3) + "_" + (n + 1).ToString();
                else
                    updatedScriptName = updatedScriptName + "_1";
            }
            else
                updatedScriptName = updatedScriptName + "_1";
            return updatedScriptName;
        }
        public static void LogEntry(string message)
        {
            DateTime dt = DateTime.Now;
            String frmdt = dt.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture);
            lock (lockObject)
            {
                System.IO.File.AppendAllText(logFilePath, message + ", " + frmdt + Environment.NewLine);
            }
        }

        private static string StreamToString(Stream fileContent)
        {
            StreamReader reader = new StreamReader(fileContent);
            string fileString = reader.ReadToEnd();
            return fileString;
        }
        private static string PadNumbers(string input)
        {
            return Regex.Replace(input, "[0-9]+", match => match.Value.PadLeft(10, '0'));
        }
        private static string CreateCategoryName(string catName)
        {
            //string updatedCatName = catName;
            //string exists = catName.Substring(catName.Length - 3);
            //if (exists.Contains("_"))
            {
                int index = catName.LastIndexOf("_");
                if (index == -1)
                    catName = catName + "_1";
                else
                {
                    string version = catName.Substring(index + 1);
                    int n;
                    bool isNumeric = int.TryParse(version, out n);
                    if (isNumeric)
                        //updatedCatName = catName.Substring(0, catName.Length - 3) + "_" + (n + 1).ToString();
                        catName = catName.Substring(0, index) + "_" + (n + 1).ToString();
                    else
                        catName = catName + "_1";
                }
            }
            //else
            //updatedCatName = updatedCatName + "_1";
            return catName;
        }

        private static string RemoveSpecialCharacters(string desc)
        {
            if (desc.Contains("/"))
                desc = desc.Replace("/", " ");
            if (desc.Contains(","))
                desc = desc.Replace(",", " ");
            if (desc.Contains("'"))
                desc = desc.Replace("'", " ");

            return desc;
        }
    }
}



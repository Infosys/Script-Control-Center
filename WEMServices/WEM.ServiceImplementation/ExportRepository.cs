/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.WEM.Export.Service.Contracts;
using Infosys.WEM.Export.Service.Contracts.Data;
using Infosys.WEM.Export.Service.Contracts.Message;
using Infosys.WEM.Infrastructure.Common;
using Infosys.WEM.Resource.Export.DataAccess;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using DE = Infosys.WEM.Resource.Entity;
using SE = Infosys.WEM.Export.Service.Contracts.Data;

namespace Infosys.WEM.Service.Implementation
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public abstract class ExportRepository_ServiceBase : IExportRepository
    {
        //public virtual AddExportServerDetailsResMsg AddExportServerDetails(AddExportServerDetailsReqMsg value)
        //{
        //    return null;
        //}

        public virtual GetExportTargetSystemDetailsResMsg GetExportTargetSystemDetails()
        {
            return null;
        }
        public virtual GetExportServerDetailsResMsg GetExportServerDetails(string targetServerId, string id)
        {
            return null;
        }
        public virtual GetExportConfigurationMasterResMsg GetExportConfigurationMasterDetails(string targetServerId)
        {
            return null;
        }
        public virtual GetAllExportConfigurationMasterResMsg GetAllExportConfigurationMasterDetails()
        {
            return null;
        }
        //public virtual AddExportConfigurationMasterResMsg AddExportConfigurationMaster(AddExportConfigurationMasterReqMsg value)
        //{
        //    return null;
        //}

        //public virtual AddExportScriptConfigurationDetailsResMsg AddExportScriptConfigurationDetail(AddExportScriptConfigurationDetailsReqMsg value)
        //{
        //    return null;
        //}

        public virtual AddExportTransactionDetailsResMsg AddExportTransactionDetails(AddExportTransactionDetailsReqMsg value)
        {
            return null;
        }

        public virtual GetExportScriptConfigurationDetailsResMsg GetExportScriptConfigurationDetails(string value)
        {
            return null;
        }

        public virtual AddExportJobProcessingHistoryResMsg AddExportJobProcessingHistory(AddExportJobProcessingHistoryReqMsg value)
        {
            return null;
        }

        public virtual UpdateExportJobProcessingHistoryResMsg UpdateExportJobProcessingHistory(UpdateExportJobProcessingHistoryReqMsg value)
        {
            return null;
        }

        public virtual AddExportSourceTargetMappingResMsg AddExportSourceTargetMapping(AddExportSourceTargetMappingReqMsg value)
        {
            return null;
        }

        public virtual UpdateExportConfigurationMasterResMsg UpdateExportConfigurationMaster(UpdateExportConfigurationMasterReqMsg value)
        {
            return null;
        }

        public virtual GetExportTransactionDetailsResMsg GetAllExportTransactionDetails(string configId, string status)
        {
            return null;
        }

        public virtual UpdateExportTransactionDetailsResMsg UpdateExportTransactionDetails(UpdateExportTransactionDetailsReqMsg value)
        {
            return null;
        }

        public virtual GetExportSourceTargetMappingDetailsResMsg GetExportSourceTargetMappingDetails(string targetServerId, string targetCategoryId, string sourceCategoryId,string sourceScriptId = "")
        {
            return null;
        }
        public virtual GetPastExportConfigurationMasterResMsg GetPastExportConfigurationMasterDetails(string ExportDate, string exportStatus, string userName)
        {
            return null;
        }

        public virtual GetExportTransactionDetailsResMsg GetExportTransactionDetailsByUserName(string exportScriptConfigurationId, string userName)
        {
            return null;
        }

        //public virtual AddExportConfigurationMasterResMsg AddExportConfigurationDetails(AddExportConfigurationMasterdetailsReqMsg value)
        //{
        //    return null;
        //}

        public virtual UpdateExportSourceTargetMappingResMsg UpdateExportSourceTargetMapping(UpdateExportSourceTargetMappingReqMsg value)
        {
            return null;
        }

        public virtual AddExportConfigurationMasterResMsg AddExportConfigurationDetails(AddExportConfigurationMasterdetailsReqMsg data)
        {
            return null;
        }

        public virtual UpdatePastExportConfigurationDetailsResMsg UpdatePastExportConfigurationDetails(UpdatePastExportConfigurationDetailsReqMsg value)
        {
            return null;
        }
    }

    public partial class ExportRepository : ExportRepository_ServiceBase
    {
        //public override AddExportServerDetailsResMsg AddExportServerDetails(AddExportServerDetailsReqMsg value)
        //{
        //    AddExportServerDetailsResMsg response = new AddExportServerDetailsResMsg();
        //    try
        //    {

        //        ExportServerDetailsDS serverDS = new ExportServerDetailsDS();
        //        DE.ExportServerDetail server = Translators.Export.ExportServerSE_DE.ServerSEtoDE(value.ExportServerDetails);
        //        var entity = serverDS.Insert(server);
        //        response.IsSuccess = true;
        //        response.Id = entity.id;

        //    }
        //    catch (Exception wemExportException)
        //    {
        //        Exception ex = new Exception();
        //        bool rethrow = ExceptionHandler.HandleException(wemExportException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

        //        if (rethrow)
        //        {
        //            throw ex;
        //        }
        //    }
        //    return response;
        //}

        public override GetExportTargetSystemDetailsResMsg GetExportTargetSystemDetails()
        {
            GetExportTargetSystemDetailsResMsg response = new GetExportTargetSystemDetailsResMsg();
            try
            {
                ExportTargetSystemDetailsDS targetSystemDetails = new ExportTargetSystemDetailsDS();
                //var resp = targetSystemDetails.GetAll().ToList();
                response.ExportTargetSystemDetails = Translators.Export.TargetSystemDetailsDE_SE.TargetSystemDEtoSE(targetSystemDetails.GetAll().ToList());

            }
            catch (Exception wemExportException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemExportException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        public override GetExportConfigurationMasterResMsg GetExportConfigurationMasterDetails(string targetServerId)
        {
            GetExportConfigurationMasterResMsg response = new GetExportConfigurationMasterResMsg();
            try
            {
                ExportConfigurationMasterDS targetSystemDetails = new ExportConfigurationMasterDS();
                SE.ExportConfigurationMaster exportSE = new ExportConfigurationMaster() { TargetServerId = int.Parse(targetServerId) };

                //translater SE to DE
                DE.ExportConfigurationMaster exportDE = Translators.Export.ExportConfigurationMasterSE_DE.ExportConfigurationMasterSEtoDE(exportSE);
                response.ExportConfigurationMasterDetails = Translators.Export.ExportConfigurationMasterSE_DE.ExportConfigurationMasterDEtoSE(targetSystemDetails.GetOne(exportDE));
            }
            catch (Exception wemExportException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemExportException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        public override GetAllExportConfigurationMasterResMsg GetAllExportConfigurationMasterDetails()
        {
            GetAllExportConfigurationMasterResMsg response = new GetAllExportConfigurationMasterResMsg();
            try
            {
                ExportConfigurationMasterDS exportConfig = new ExportConfigurationMasterDS();

                //translater SE to DE
                //var res = Translators.Export.ExportConfigurationMasterSE_DE.ExportConfigurationMasterDEtoSEList(exportConfig.GetAll().ToList());
                var res = Translators.Export.ExportConfigurationMasterSE_DE.ExportConfigurationMasterDEtoSE(exportConfig.GetOne(null));
                response.ExportConfigurationMasterDetails = res;
            }
            catch (Exception wemExportException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemExportException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        public override GetExportServerDetailsResMsg GetExportServerDetails(string targetServerId, string id="")
        {
            GetExportServerDetailsResMsg response = new GetExportServerDetailsResMsg();
            try
            {
                ExportServerDetailsDS targetSystemDetails = new ExportServerDetailsDS();
                //SE.ExportServerDetails serverSE = new ExportServerDetails() { TargetSystemId = int.Parse(targetServerId) };
                SE.ExportServerDetails serverSE = new ExportServerDetails();
                if (!string.IsNullOrEmpty(targetServerId))
                    serverSE.TargetSystemId = int.Parse(targetServerId);

                if (!string.IsNullOrEmpty(id))
                    serverSE.Id = int.Parse(id);

                //translater SE to DE
                DE.ExportServerDetail serverDE = Translators.Export.ExportServerSE_DE.ServerSEtoDE(serverSE);
                response.ExportServerDetails = Translators.Export.ExportServerSE_DE.ServerDEtoSE(targetSystemDetails.GetAll(serverDE).ToList());
            }
            catch (Exception wemExportException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemExportException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        //public override AddExportConfigurationMasterResMsg AddExportConfigurationMaster(AddExportConfigurationMasterReqMsg value)
        //{
        //    AddExportConfigurationMasterResMsg response = new AddExportConfigurationMasterResMsg();
        //    try
        //    {
        //        ExportConfigurationMasterDS exportDS = new ExportConfigurationMasterDS();
        //        DE.ExportConfigurationMaster exportServer = Translators.Export.ExportConfigurationMasterSE_DE.ExportConfigurationMasterSEtoDE(value.ExportConfigurationMaster);
        //        exportDS.Insert(exportServer);
        //        response.IsSuccess = true;
        //        response.Id = exportServer.id;
        //    }
        //    catch (Exception wemExportException)
        //    {
        //        Exception ex = new Exception();
        //        bool rethrow = ExceptionHandler.HandleException(wemExportException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

        //        if (rethrow)
        //        {
        //            throw ex;
        //        }
        //    }
        //    return response;
        //}

        //public override AddExportScriptConfigurationDetailsResMsg AddExportScriptConfigurationDetail(AddExportScriptConfigurationDetailsReqMsg value)
        //{
        //    AddExportScriptConfigurationDetailsResMsg response = new AddExportScriptConfigurationDetailsResMsg();
        //    try
        //    {
        //        ExportScriptConfigurationDetailsDS exportDS = new ExportScriptConfigurationDetailsDS();
        //        List<DE.ExportScriptConfigurationDetail> exportServer = Translators.Export.ExportConfigurationMasterSE_DE.ExportScriptConfigurationSEtoDEList(value.ExportScriptConfigurationDetails.ToList());
        //        exportDS.InsertBatch(exportServer);
        //        response.IsSuccess = true;
        //        //response.Id = exportServer.id;
        //    }
        //    catch (Exception wemExportException)
        //    {
        //        Exception ex = new Exception();
        //        bool rethrow = ExceptionHandler.HandleException(wemExportException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

        //        if (rethrow)
        //        {
        //            throw ex;
        //        }
        //    }
        //    return response;
        //}

        public override AddExportTransactionDetailsResMsg AddExportTransactionDetails(AddExportTransactionDetailsReqMsg value)
        {
            AddExportTransactionDetailsResMsg response = new AddExportTransactionDetailsResMsg();
            try
            {
                string data = JsonConvert.SerializeObject(value);
                ExportTransactionDetailsDS exportDS = new ExportTransactionDetailsDS();
                List<DE.ExportTransactionDetail> trans = Translators.Export.ExportScriptTransactionSE_DE.ExportTransactionDetailsSEtoDEList(value.ExportTransactionDetails.ToList());
                exportDS.InsertBatch(trans);
                //if (!exportDS.IsDuplicateTransaction(trans))
                //    exportDS.Insert(trans);
                response.IsSuccess = true;
                //response.Id = trans.id;
            }
            catch (Exception wemExportException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemExportException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        public override GetExportScriptConfigurationDetailsResMsg GetExportScriptConfigurationDetails(string exportConfigurationId)
        {
            GetExportScriptConfigurationDetailsResMsg response = new GetExportScriptConfigurationDetailsResMsg();
            try
            {
                ExportScriptConfigurationDetailsDS scriptDetails = new ExportScriptConfigurationDetailsDS();
                SE.ExportScriptConfigurationDetails scriptSE = new ExportScriptConfigurationDetails() { ExportConfigurationId = int.Parse(exportConfigurationId) };

                //translater SE to DE
                DE.ExportScriptConfigurationDetail scriptDE = Translators.Export.ExportScriptConfigurationDetailsSE_DE.ExportScriptConfigurationSEtoDE(scriptSE);
                response.ExportScriptConfigurationDetails = Translators.Export.ExportScriptConfigurationDetailsSE_DE.ExportScriptConfigurationDEtoSEList(scriptDetails.GetAll(scriptDE).ToList());
            }
            catch (Exception wemExportException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemExportException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        public override AddExportJobProcessingHistoryResMsg AddExportJobProcessingHistory(AddExportJobProcessingHistoryReqMsg value)
        {
            AddExportJobProcessingHistoryResMsg response = new AddExportJobProcessingHistoryResMsg();
            try
            {

                ExportJobProcessingHistoryDS jobDS = new ExportJobProcessingHistoryDS();
                DE.ExportJobProcessingHistory exportServer = Translators.Export.ExportJobProcessingHistorySE_DE.ExportConfigurationMasterSEtoDE(value.ExportJobProcessingHistory);
                jobDS.Insert(exportServer);
                response.IsSuccess = true;
                response.JobId = exportServer.JobId;
            }
            catch (Exception wemExportException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemExportException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        public override UpdateExportJobProcessingHistoryResMsg UpdateExportJobProcessingHistory(UpdateExportJobProcessingHistoryReqMsg value)
        {
            UpdateExportJobProcessingHistoryResMsg response = new UpdateExportJobProcessingHistoryResMsg();
            try
            {
                ExportJobProcessingHistoryDS jobDS = new ExportJobProcessingHistoryDS();
                DE.ExportJobProcessingHistory exportServer = Translators.Export.ExportJobProcessingHistorySE_DE.ExportConfigurationMasterSEtoDE(value.ExportJobProcessingHistory);
                jobDS.Update(exportServer);
                response.IsSuccess = true;
            }
            catch (Exception wemExportException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemExportException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        public override AddExportSourceTargetMappingResMsg AddExportSourceTargetMapping(AddExportSourceTargetMappingReqMsg value)
        {
            AddExportSourceTargetMappingResMsg response = new AddExportSourceTargetMappingResMsg();
            try
            {

                ExportSourceTargetMappingDS exportTarget = new ExportSourceTargetMappingDS();
                DE.ExportSourceTargetMapping exportServer = Translators.Export.ExportSourceTargetMappingSE_DE.ExportSourceTargetMappingSEtoDE(value.ExportSourceTargetMapping);
                exportTarget.Insert(exportServer);
                response.IsSuccess = true;
            }
            catch (Exception wemExportException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemExportException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        public override UpdateExportConfigurationMasterResMsg UpdateExportConfigurationMaster(UpdateExportConfigurationMasterReqMsg value)
        {
            UpdateExportConfigurationMasterResMsg response = new UpdateExportConfigurationMasterResMsg();
            try
            {
                ExportConfigurationMasterDS exportDS = new ExportConfigurationMasterDS();
                SE.ExportConfigurationMaster serverSE = new ExportConfigurationMaster() { id = value.ExportConfigurationMaster.id, ExportStatus = value.ExportConfigurationMaster.ExportStatus, ModifiedBy = value.ExportConfigurationMaster.ModifiedBy, ModifiedOn = value.ExportConfigurationMaster.ModifiedOn, CompletedOn = value.ExportConfigurationMaster.CompletedOn, ScriptRepositoryBaseServerAddress = value.ExportConfigurationMaster.ScriptRepositoryBaseServerAddress };

                DE.ExportConfigurationMaster exportServer = Translators.Export.ExportConfigurationMasterSE_DE.ExportConfigurationMasterSEtoDE(serverSE);
                exportDS.Update(exportServer);
                response.IsSuccess = true;

            }
            catch (Exception wemExportException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemExportException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        public override GetExportTransactionDetailsResMsg GetAllExportTransactionDetails(string configId, string status)
        {
            GetExportTransactionDetailsResMsg response = new GetExportTransactionDetailsResMsg();
            try
            {
                ExportTransactionDetailsDS transDS = new ExportTransactionDetailsDS();
                SE.ExportTransactionDetails scriptSE = new ExportTransactionDetails { ExportScriptConfigurationId = int.Parse(configId), Status = short.Parse(status) };
                //translater DE to SE
                DE.ExportTransactionDetail scriptDE = Translators.Export.ExportScriptTransactionSE_DE.ExportTransactionDetailsSEtoDE(scriptSE);
                response.ExportTransactionDetails = Translators.Export.ExportScriptTransactionSE_DE.ExportTransactionDetailsDEtoSEList(transDS.GetAll(scriptDE).ToList());
            }
            catch (Exception wemExportException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemExportException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        public override UpdateExportTransactionDetailsResMsg UpdateExportTransactionDetails(UpdateExportTransactionDetailsReqMsg value)
        {
            UpdateExportTransactionDetailsResMsg response = new UpdateExportTransactionDetailsResMsg();
            try
            {
                ExportTransactionDetailsDS exportDS = new ExportTransactionDetailsDS();
                SE.ExportTransactionDetails serverSE = new ExportTransactionDetails() { id = value.ExportTransactionDetails.id, Status = value.ExportTransactionDetails.Status, Details = value.ExportTransactionDetails.Details, Action = value.ExportTransactionDetails.Action, ModifiedBy = value.ExportTransactionDetails.ModifiedBy, ModifiedOn = value.ExportTransactionDetails.ModifiedOn };
                if (value.ExportTransactionDetails.ExistReasonCode != null)
                    serverSE.ExistReasonCode = value.ExportTransactionDetails.ExistReasonCode;
                if (!String.IsNullOrEmpty(value.ExportTransactionDetails.TargetScriptName))
                    serverSE.TargetScriptName = value.ExportTransactionDetails.TargetScriptName;
                if (!String.IsNullOrEmpty(value.ExportTransactionDetails.TargetScriptPath))
                    serverSE.TargetScriptPath = value.ExportTransactionDetails.TargetScriptPath;

                DE.ExportTransactionDetail exportServer = Translators.Export.ExportScriptTransactionSE_DE.ExportTransactionDetailsSEtoDE(serverSE);
                exportDS.Update(exportServer);
                response.IsSuccess = true;

            }
            catch (Exception wemExportException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemExportException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        public override GetExportSourceTargetMappingDetailsResMsg GetExportSourceTargetMappingDetails(string targetServerId, string targetCategoryId, string sourceCategoryId, string sourceScriptId = "")
        {
            GetExportSourceTargetMappingDetailsResMsg response = new GetExportSourceTargetMappingDetailsResMsg();
            try
            {
                ExportSourceTargetMappingDS targetMapping = new ExportSourceTargetMappingDS();
                SE.ExportSourceTargetMapping targetSE = new ExportSourceTargetMapping() { TargetInstanceId = int.Parse(targetServerId), TargetScriptCategoryId = int.Parse(targetCategoryId),SourceScriptCategoryId=int.Parse(sourceCategoryId) };
                if (string.IsNullOrEmpty(sourceScriptId))
                    targetSE.SourceScriptId = 0;
                else
                    targetSE.SourceScriptId = int.Parse(sourceScriptId);

                //translater SE to DE
                DE.ExportSourceTargetMapping scriptDE = Translators.Export.ExportSourceTargetMappingSE_DE.ExportSourceTargetMappingSEtoDE(targetSE);
                response.ExportSourceTargetMapping = Translators.Export.ExportSourceTargetMappingSE_DE.ExportSourceTargetMappingDEtoSE(targetMapping.GetOne(scriptDE));
            }
            catch (Exception wemExportException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemExportException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        private List<PastExportConfigurationMasterDetails> GetPastExportScriptDetails(PastExportConfigurationMasterDetails master, string userName)
        {
            List<PastExportConfigurationMasterDetails> response = new List<PastExportConfigurationMasterDetails>();

            var scriptConfigDetails = GetExportScriptConfigurationDetails(master.masterExportId.ToString());
            foreach (var scriptConfig in scriptConfigDetails.ExportScriptConfigurationDetails)
            {
                var scriptTransactions = GetExportTransactionDetailsByUserName(scriptConfig.id.ToString(), userName);
                foreach (var scriptTransaction in scriptTransactions.ExportTransactionDetails)
                {
                    PastExportConfigurationMasterDetails details = new PastExportConfigurationMasterDetails();
                    details.masterExportId = master.masterExportId;
                    details.ExportStatus = master.ExportStatus;
                    details.CreatedBy = master.CreatedBy;
                    details.CreatedOn = master.CreatedOn;
                    details.ModifiedBy = master.ModifiedBy;
                    details.ModifiedOn = master.ModifiedOn;
                    details.CompletedOn = master.CompletedOn;
                    details.ScriptConfigurationId = scriptConfig.id;
                    details.ScriptTransactionId = scriptTransaction.id;
                    details.SourceCategoryId = scriptTransaction.SourceCategoryId;
                    details.TargetCategoryId = scriptTransaction.TargetCategoryId;
                    details.SourceScriptId = scriptTransaction.SourceScriptId;
                    details.SourceScriptPath = scriptTransaction.SourceScriptPath;
                    details.TargetScriptPath = scriptTransaction.TargetScriptPath;
                    details.TargetScriptName = scriptTransaction.TargetScriptName;
                    details.ExistReasonCode = scriptTransaction.ExistReasonCode;
                    details.Details = scriptTransaction.Details;
                    details.Action = scriptTransaction.Action;
                    details.Status = scriptTransaction.Status;
                    response.Add(details);
                }
            }

            return response;
        }

        public override GetPastExportConfigurationMasterResMsg GetPastExportConfigurationMasterDetails(string ExportDate, string exportStatus, string userName)
        {
            GetPastExportConfigurationMasterResMsg response = new GetPastExportConfigurationMasterResMsg();
            List<PastExportConfigurationMasterDetails> details = new List<PastExportConfigurationMasterDetails>();
            try
            {
                ExportConfigurationMasterDS exportConfig = new ExportConfigurationMasterDS();
                DateTime createdOn = Convert.ToDateTime(ExportDate);
                SE.ExportConfigurationMaster configMasterSE = new SE.ExportConfigurationMaster() { CreatedOn = createdOn, ExportStatus = int.Parse(exportStatus) };
                if (!string.IsNullOrEmpty(userName))
                    configMasterSE.CreatedBy = userName;
                //translater SE to DE
                DE.ExportConfigurationMaster exportDE = Translators.Export.ExportConfigurationMasterSE_DE.PastExportConfigurationMasterSEtoDE(configMasterSE);
                var result = Translators.Export.ExportConfigurationMasterSE_DE.PastExportConfigurationMasterDEtoSEList(exportConfig.GetAll(exportDE).ToList());

                foreach (var record in result)
                {
                    var scriptDetails = GetPastExportScriptDetails(record, userName);
                    if (scriptDetails != null && scriptDetails.Count > 0)
                    {
                        foreach (var script in scriptDetails)
                        {
                            details.Add(script);
                        }
                    }
                    else
                    {
                        var scriptConfigDetails = GetExportScriptConfigurationDetails(record.masterExportId.ToString());
                        foreach (var scriptConfig in scriptConfigDetails.ExportScriptConfigurationDetails)
                        {
                            PastExportConfigurationMasterDetails masterData = new PastExportConfigurationMasterDetails();
                            masterData.masterExportId = record.masterExportId;
                            masterData.ExportStatus = record.ExportStatus;
                            masterData.CreatedBy = record.CreatedBy;
                            masterData.CreatedOn = record.CreatedOn;
                            masterData.ModifiedBy = record.ModifiedBy;
                            masterData.ModifiedOn = record.ModifiedOn;
                            masterData.CompletedOn = record.CompletedOn;
                            masterData.ScriptConfigurationId = scriptConfig.id;
                            masterData.SourceScriptPath = scriptConfig.SourceScriptPath;
                            masterData.TargetScriptPath = scriptConfig.TargetScriptPath;
                            details.Add(masterData);
                        }
                    }
                }

                response.PastExportConfigurationMasterDetails = details;
            }
            catch (Exception wemExportException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemExportException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        public override GetExportTransactionDetailsResMsg GetExportTransactionDetailsByUserName(string exportScriptConfigurationId, string userName)
        {
            GetExportTransactionDetailsResMsg response = new GetExportTransactionDetailsResMsg();
            try
            {
                ExportTransactionDetailsDS transDetails = new ExportTransactionDetailsDS();
                SE.ExportTransactionDetails exportTrans = new SE.ExportTransactionDetails() { ExportScriptConfigurationId = int.Parse(exportScriptConfigurationId), CreatedBy = userName };
                DE.ExportTransactionDetail exportDE = Translators.Export.ExportScriptTransactionSE_DE.ExportTransactionDetailsSEtoDE(exportTrans);
                response.ExportTransactionDetails = Translators.Export.ExportScriptTransactionSE_DE.ExportTransactionDetailsDEtoSEList(transDetails.GetAll(exportDE).ToList());
            }
            catch (Exception wemExportException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemExportException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        //public override AddExportConfigurationMasterResMsg AddExportConfigurationDetails(AddExportConfigurationMasterdetailsReqMsg value)
        //{
        //    AddExportConfigurationMasterResMsg response = new AddExportConfigurationMasterResMsg();
        //    try
        //    {
        //        int exportMasterConfigId = 0;
        //        // Add export server details
        //        ExportServerDetailsDS serverDS = new ExportServerDetailsDS();
        //        DE.ExportServerDetail server = Translators.Export.ExportServerSE_DE.ExportServerSEtoDE(value.ExportConfigurationMasterDetails);
        //        var entity = serverDS.Insert(server);

        //        // Add Export Master Details and set Deleted to True
        //        ExportConfigurationMasterDS exportMasterDS = new ExportConfigurationMasterDS();
        //        DE.ExportConfigurationMaster exportConfigMaster = Translators.Export.ExportConfigurationMasterSE_DE.ExportMasterConfigurationDetailsSEtoDE(value.ExportConfigurationMasterDetails, entity.id);
        //        exportMasterDS.Insert(exportConfigMaster);
        //        exportMasterConfigId = exportConfigMaster.id;

        //        // Add configuration details
        //        ExportScriptConfigurationDetailsDS exportConfigDS = new ExportScriptConfigurationDetailsDS();
        //        List<DE.ExportScriptConfigurationDetail> exportConfigDetails = Translators.Export.ExportConfigurationMasterSE_DE.ExportScriptConfigurationSEtoDEList(value.ExportConfigurationMasterDetails, exportMasterConfigId);
        //        exportConfigDS.InsertBatch(exportConfigDetails);


        //        // Update Export Master Details and set Deleted to False
        //        DE.ExportConfigurationMaster exportServer = Translators.Export.ExportConfigurationMasterSE_DE.ExportMasterConfigurationDetailsSEtoDE(exportMasterConfigId);
        //        exportMasterDS.Update(exportServer);

        //        response.Id = exportMasterConfigId;
        //        response.IsSuccess = true;
        //    }
        //    catch (Exception wemExportException)
        //    {
        //        WEMValidationException validateException = new WEMValidationException(wemExportException.Message);
        //        List<ServiceFaultError> validateErrs = new List<ServiceFaultError>();
        //        ServiceFaultError validationErr = new ServiceFaultError();
        //        validationErr.ErrorCode = 1043;
        //        validationErr.Message = wemExportException.Message;
        //        validateErrs.Add(validationErr);
        //        if (validateErrs.Count > 0)
        //        {
        //            validateException.Data.Add("ValidationErrors", validateErrs);
        //            response.ServiceFaults = ExceptionHandler.ExtractServiceFaults(validateException);
        //        }
        //    }
        //    return response;
        //}

        public override UpdateExportSourceTargetMappingResMsg UpdateExportSourceTargetMapping(UpdateExportSourceTargetMappingReqMsg value)
        {
            UpdateExportSourceTargetMappingResMsg response = new UpdateExportSourceTargetMappingResMsg();
            try
            {

                ExportSourceTargetMappingDS exportTarget = new ExportSourceTargetMappingDS();
                DE.ExportSourceTargetMapping exportServer = Translators.Export.ExportSourceTargetMappingSE_DE.ExportSourceTargetMappingSEtoDE(value.ExportSourceTargetMapping);
                exportTarget.Update(exportServer);
                response.IsSuccess = true;
            }
            catch (Exception wemExportException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemExportException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        public override AddExportConfigurationMasterResMsg AddExportConfigurationDetails(AddExportConfigurationMasterdetailsReqMsg value)
        {
            AddExportConfigurationMasterResMsg response = new AddExportConfigurationMasterResMsg();
            try
            {
                //ValidateNameField(value.ExportConfigurationMasterDetails);
                int exportMasterConfigId = 0;
                // Add export server details
                ExportServerDetailsDS serverDS = new ExportServerDetailsDS();
                DE.ExportServerDetail server = Translators.Export.ExportServerSE_DE.ExportServerSEtoDE(value.ExportConfigurationMasterDetails);
                var entity = serverDS.Insert(server);

                IncomingWebRequestContext request = WebOperationContext.Current.IncomingRequest;
                WebHeaderCollection headers = request.Headers;
                string password = value.ExportConfigurationMasterDetails.TargetSystemPassword;
                foreach (string headerName in headers.AllKeys)
                {
                    if (headerName.Equals("AppAuthKey"))
                    {
                        string AppAuthKey = ConfigurationManager.AppSettings["AppAuthKey"];
                        if (headers[headerName].Equals(AppAuthKey))
                        {
                            password = Infosys.WEM.Infrastructure.SecurityCore.SecureData.Secure(password, "IAP2GO_SEC!URE");
                        }
                    }
                }
                value.ExportConfigurationMasterDetails.TargetSystemPassword = password;

                // Add Export Master Details and set Deleted to True
                ExportConfigurationMasterDS exportMasterDS = new ExportConfigurationMasterDS();
                DE.ExportConfigurationMaster exportConfigMaster = Translators.Export.ExportConfigurationMasterSE_DE.ExportMasterConfigurationDetailsSEtoDE(value.ExportConfigurationMasterDetails, entity.id);
                exportMasterDS.Insert(exportConfigMaster);
                exportMasterConfigId = exportConfigMaster.id;

                // Add configuration details
                ExportScriptConfigurationDetailsDS exportConfigDS = new ExportScriptConfigurationDetailsDS();
                List<DE.ExportScriptConfigurationDetail> exportConfigDetails = Translators.Export.ExportConfigurationMasterSE_DE.ExportScriptConfigurationSEtoDEList(value.ExportConfigurationMasterDetails, exportMasterConfigId);
                exportConfigDS.InsertBatch(exportConfigDetails);

                // Update Export Master Details and set Deleted to False
                DE.ExportConfigurationMaster exportServer = Translators.Export.ExportConfigurationMasterSE_DE.ExportMasterConfigurationDetailsSEtoDE(exportMasterConfigId);
                exportMasterDS.Update(exportServer);

                response.Id = exportMasterConfigId;
                response.IsSuccess = true;
            }
            catch (Exception wemScriptException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemScriptException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            //catch (Exception wemExportException)
            //{
            //    WEMValidationException validateException = new WEMValidationException(wemExportException.Message);
            //    List<ServiceFaultError> validateErrs = new List<ServiceFaultError>();
            //    ServiceFaultError validationErr = new ServiceFaultError();
            //    validationErr.ErrorCode = 1043;
            //    validationErr.Message = wemExportException.Message;
            //    validateErrs.Add(validationErr);
            //    if (validateErrs.Count > 0)
            //    {
            //        validateException.Data.Add("ValidationErrors", validateErrs);
            //        response.ServiceFaults = ExceptionHandler.ExtractServiceFaults(validateException);
            //    }
            //}
            return response;
        }

        public override UpdatePastExportConfigurationDetailsResMsg UpdatePastExportConfigurationDetails(UpdatePastExportConfigurationDetailsReqMsg value)
        {
            UpdatePastExportConfigurationDetailsResMsg response = new UpdatePastExportConfigurationDetailsResMsg();

            try
            {
                // Add Export Master Details and set Deleted to True
                ExportConfigurationMasterDS exportMasterDS = new ExportConfigurationMasterDS();
                foreach (var data in value.PastExportConfigurationMasterDetails)
                {
                    ExportTransactionDetailsDS exportTransactionDS = new ExportTransactionDetailsDS();
                    SE.ExportTransactionDetails transDetails = new ExportTransactionDetails() { id = data.ScriptTransactionId, Status = data.Status, Action = data.Action, ExistReasonCode = data.ExistReasonCode, ModifiedBy = data.ModifiedBy, ModifiedOn = data.ModifiedOn };

                    DE.ExportTransactionDetail exportServer = Translators.Export.ExportScriptTransactionSE_DE.PastExportTransactionDetailsSEtoDE(transDetails);
                    exportTransactionDS.Update(exportServer);

                    // Update Export Master Details
                    DE.ExportConfigurationMaster exportMaster = Translators.Export.ExportConfigurationMasterSE_DE.ExportMasterConfigurationDetailsSEtoDE(data.masterExportId, data.ExportStatus);
                    exportMasterDS.Update(exportMaster);
                }

                //response.Id = exportMasterConfigId;
                response.IsSuccess = true;
            }
            catch (Exception wemExportException)
            {
                WEMValidationException validateException = new WEMValidationException(wemExportException.Message);
                List<ServiceFaultError> validateErrs = new List<ServiceFaultError>();
                ServiceFaultError validationErr = new ServiceFaultError();
                validationErr.ErrorCode = 1043;
                validationErr.Message = wemExportException.Message;
                validateErrs.Add(validationErr);
                if (validateErrs.Count > 0)
                {
                    validateException.Data.Add("ValidationErrors", validateErrs);
                    //response.ServiceFaults = ExceptionHandler.ExtractServiceFaults(validateException);
                }
            }
            return response;
        }

        private void ValidateNameField(Infosys.WEM.Export.Service.Contracts.Data.ExportConfigurationMasterDetails exportDetails)
        {
            if (exportDetails != null)
            {
                //check for value invalidcharacters
                WEMValidationException validateException = new WEMValidationException(ErrorMessages.InvalidCharacter_Validation.ToString());
                List<ValidationError> validateErrs = new List<ValidationError>();

                if (ValidationUtility.InvalidCharacterValidator(exportDetails.AutomationServerIPAddress))
                {
                    ValidationError validationErr = new ValidationError();
                    validationErr.Code = Errors.ErrorCodes.InvalidCharacter_Validation.ToString();
                    validationErr.Description = string.Format(ErrorMessages.InvalidCharacter_Validation, exportDetails.AutomationServerIPAddress, "Export.AutomationServerIPAddress");
                    validateErrs.Add(validationErr);
                }

                if (ValidationUtility.InvalidCharacterValidator(exportDetails.CasServerIPAddress))
                {
                    ValidationError validationErr = new ValidationError();
                    validationErr.Code = Errors.ErrorCodes.InvalidCharacter_Validation.ToString();
                    validationErr.Description = string.Format(ErrorMessages.InvalidCharacter_Validation, exportDetails.CasServerIPAddress, "Export.CasServerIPAddress");
                    validateErrs.Add(validationErr);
                }
               
                if (exportDetails.AutomationServerTypeId ==0)
                {
                    ValidationError validationErr = new ValidationError();
                    validationErr.Code = Errors.ErrorCodes.InvalidDataType_Validation.ToString();
                    validationErr.Description = string.Format("InvalidDataType_Validation", exportDetails.AutomationServerTypeId, "Export.AutomationServerTypeId");
                    validateErrs.Add(validationErr);
                }

                //if (exportDetails.ExportConfigurationDetails!=null && exportDetails.ExportConfigurationDetails.Count > 0)
                //{
                //    foreach(var export in exportDetails.ExportConfigurationDetails)
                //    {
                //        if (ValidationUtility.InvalidCharacterValidator(export.SourceCategoryId.ToString()))
                //        {
                //            ValidationError validationErr = new ValidationError();
                //            validationErr.Code = Errors.ErrorCodes.InvalidCharacter_Validation.ToString();
                //            validationErr.Description = string.Format(ErrorMessages.InvalidCharacter_Validation, exportDetails.AutomationServerIPAddress, "Export.AutomationServerIPAddress");
                //            validateErrs.Add(validationErr);
                //        }
                //    }
                //}

                if (validateErrs.Count > 0)
                {
                    validateException.Data.Add("ValidationErrors", validateErrs);
                    throw validateException;
                }
            }
        }
    }
}

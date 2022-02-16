/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.WEM.Infrastructure.Common;
using Infosys.WEM.Scripts.Resource.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using Infosys.WEM.AutomationTracker.Contracts;
using Infosys.WEM.AutomationTracker.Contracts.Message;
using Infosys.WEM.Resource.DataAccess;
using DE = Infosys.WEM.Resource.Entity;
using Infosys.WEM.AutomationTracker.Contracts.Data;

namespace Infosys.WEM.Service.Implementation
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public abstract class AutomationRepository_ServiceBase : IAutomationTrackerRepository
    {
        #region IAutomationRepository Members
        public virtual AddRequestResMsg AddTransactionStatus(AddRequestReqMsg value)
        {
            return null;
        }

        public virtual GetTransactionStatusResMsg GetTransactionStatus(GetTransactionStatusReqMsg request)
        {
            return null;
        }

        public virtual UpdateTransactionStatusResMsg UpdateTransactionStatus(UpdateTransactionStatusReqMsg value)
        {
            return null;
        }

        public virtual UpdateNotificationDetailsResMsg UpdateNotificationDetails(UpdateNotificationDetailsReqMsg value)
        {
            return null;
        }

        public virtual GetTransactionsByStatusResMsg GetTransactionsByStatus(GetTransactionsByStatusReqMsg request)
        {
            return null;
        }
        #endregion
    }

    public partial class AutomationTrackerRepository : AutomationRepository_ServiceBase
    {
        public override AddRequestResMsg AddTransactionStatus(AddRequestReqMsg value)
        {
            AddRequestResMsg response = new AddRequestResMsg();
            try
            {
                using (LogHandler.TraceOperations("AutomationTrackerRepository:AddTransactionStatus", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
                {
                    ScriptExecuteRequestDSExt scriptExecuteRequestDSExt = new ScriptExecuteRequestDSExt();
                    ScriptExecuteRequestDS scriptExecuteRequest = new ScriptExecuteRequestDS();
                    DE.ScriptExecuteRequest executeRequest = Translators.AutomationTracker.ScriptExecuteRequestSE_DE.ScriptExecuteRequestSEtoDE(value.requestData);

                    //Block40
                    //DateTime processStartedTime = DateTime.Now;
                    executeRequest.createdby = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    //LogHandler.LogDebug(string.Format("Time taken by Block 40 : WEM-AddTransactionStatus()-GetLoggedinUser() :{0}", DateTime.Now.Subtract(processStartedTime).TotalSeconds), LogHandler.Layer.Business, null);
                    List<ScriptExecuteResponse> scriptExecuteResponsesSE = new List<ScriptExecuteResponse>();
                    if (!scriptExecuteRequestDSExt.IsDuplicate(executeRequest.transactionId.ToString()))
                    {
                        //Block41
                        //DateTime processStartedTime1 = DateTime.Now;
                        executeRequest = scriptExecuteRequest.Insert(executeRequest);

                        ScriptExecuteResponseDS scriptExecuteResponse = new ScriptExecuteResponseDS();
                        if (executeRequest.remoteservernames == null)
                        {
                            // initiate state entry in the ScriptExecuteResponse Table                        
                            DE.ScriptExecuteResponse executeResponse = Translators.AutomationTracker.ScriptExecuteResponseSE_DE.ScriptExecuteResponseSEtoDE(new AutomationTracker.Contracts.Data.ScriptExecuteResponse());
                            //executeResponse.issuccess = true;
                            executeResponse.transactionId = executeRequest.transactionId;
                            executeResponse.currentstate = Status.initial.ToString().ToUpper();
                            //Change ComputerName from Default to GetMachineName()
                            executeResponse.computername = Utility.GetMachineName();
                            executeResponse.createdby = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                            executeResponse.PartitionKey = executeRequest.PartitionKey;
                            scriptExecuteResponsesSE.Add(Translators.AutomationTracker.ScriptExecuteResponseSE_DE.ScriptExecuteResponseDEtoSE(scriptExecuteResponse.Insert(executeResponse)));
                            response.scriptExecuteResponseList = scriptExecuteResponsesSE;
                            //LogHandler.LogDebug(string.Format("Time taken by Block 41 : WEM-AddTransactionStatus()-Insert() :{0}", DateTime.Now.Subtract(processStartedTime1).TotalSeconds), LogHandler.Layer.Business, null);
                        }
                        else
                        {
                            //Block42
                            //DateTime processStartedTime2 = DateTime.Now;
                            string[] computernames = executeRequest.remoteservernames.Split(',');
                            foreach (string computername in computernames)
                            {
                                ScriptExecuteResponse scriptExecuteResponseSE = new ScriptExecuteResponse()
                                {
                                    ComputerName = computername.Trim(),
                                    //TransactionId= new Guid(executeRequest.transactionId),
                                    TransactionId = executeRequest.transactionId.Value,
                                    CurrentState = Status.initial.ToString().ToUpper(),
                                };
                                scriptExecuteResponsesSE.Add(scriptExecuteResponseSE);

                            }
                            List<DE.ScriptExecuteResponse> executeResponses = Translators.AutomationTracker.ScriptExecuteResponseSE_DE.ScriptExecuteResponseSEListtoDEList(scriptExecuteResponsesSE);
                            for (int index = 0; index < executeResponses.Count(); index++)
                            {
                                executeResponses[index].PartitionKey = executeRequest.PartitionKey;
                            }
                            response.scriptExecuteResponseList = Translators.AutomationTracker.ScriptExecuteResponseSE_DE.ScriptExecuteResponseDEListtoSEList(scriptExecuteResponse.InsertBatch(executeResponses));
                            //LogHandler.LogDebug(string.Format("Time taken by Block 42 : WEM-AddTransactionStatus():{0}", DateTime.Now.Subtract(processStartedTime2).TotalSeconds), LogHandler.Layer.Business, null);
                        }
                    }
                    else
                    {
                        //Block43
                        DateTime processStartedTime2 = DateTime.Now;
                        ScriptExecuteResponse executeResponse = new ScriptExecuteResponse()
                        {
                            IsSuccess = false,
                            ErrorMessage = "One or more provided request transactionid is same as any existing transactionid under the intended scriptexecute request."
                        };
                        scriptExecuteResponsesSE.Add(executeResponse);
                        response.scriptExecuteResponseList = scriptExecuteResponsesSE;
                        LogHandler.LogDebug(string.Format("Time taken by Block 43 : WEM-AddTransactionStatus() :{0}", DateTime.Now.Subtract(processStartedTime2).TotalSeconds), LogHandler.Layer.Business, null);
                        throw new Exception("One or more provided request transactionid is same as any existing transactionid under the intended scriptexecute request.");
                    }
                }
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
            return response;
        }

        public override GetTransactionStatusResMsg GetTransactionStatus(GetTransactionStatusReqMsg request)
        {
            GetTransactionStatusResMsg response = new GetTransactionStatusResMsg();
            try
            {
                ScriptExecuteRequestDSExt scriptExecuteRequestDSExt = new ScriptExecuteRequestDSExt();
                List<ScriptExecuteResponse> scriptExecuteResponsesSE = new List<ScriptExecuteResponse>();
                List<DE.ScriptExecuteResponse> scriptExecuteResponsesDE = scriptExecuteRequestDSExt.getTransactionStatus(request.TransactionId);
                scriptExecuteResponsesSE = Translators.AutomationTracker.ScriptExecuteResponseSE_DE.ScriptExecuteResponseDEListtoSEList(scriptExecuteResponsesDE);
                response.scriptExecuteResponse = scriptExecuteResponsesSE;
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
            return response;
        }        

        public override UpdateTransactionStatusResMsg UpdateTransactionStatus(UpdateTransactionStatusReqMsg value)
        {
            UpdateTransactionStatusResMsg response = new UpdateTransactionStatusResMsg();
            try
            {
                using (LogHandler.TraceOperations("AutomationTrackerRepository:UpdateTransactionStatus", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
                {
                    ScriptExecuteResponseDS executeResponseDS = new ScriptExecuteResponseDS();
                    ScriptExecuteResponseDSExt scriptExecuteResponseDSExt = new ScriptExecuteResponseDSExt();
                    DE.ScriptExecuteResponse objScriptExecuteResponse = Translators.AutomationTracker.ScriptExecuteResponseSE_DE.ScriptExecuteResponseSEtoDE(value.scriptExecuteResponse);
                    objScriptExecuteResponse.modifiedby = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

                    DE.ScriptExecuteResponse objresponse = executeResponseDS.Update(objScriptExecuteResponse);
                    response.isSuccess = true;
                }
                
               
            }

            catch (Exception wemScriptException)
            {
                response.isSuccess = false;
                response.Message = wemScriptException.Message + ".. Inner exception" + wemScriptException.InnerException;
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemScriptException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        public override UpdateNotificationDetailsResMsg UpdateNotificationDetails(UpdateNotificationDetailsReqMsg value)
        {
            UpdateNotificationDetailsResMsg response = new UpdateNotificationDetailsResMsg();
            try
            {
                ScriptExecuteResponseDS executeResponseDS = new ScriptExecuteResponseDS();
                ScriptExecuteResponseDSExt scriptExecuteResponseDSExt = new ScriptExecuteResponseDSExt();

                DE.ScriptExecuteResponse objScriptExecuteResponse = Translators.AutomationTracker.ScriptExecuteResponseSE_DE.ScriptExecuteResponseSEtoDE(value.scriptExecuteResponse);

                // object for ScriptExecuteRequestDS
                ScriptExecuteRequestDS objScriptExecuteRequestDS = new ScriptExecuteRequestDS();

                // query request table based on transaction id 
                // check if transaction id (in value) is available in ScriptExecuteRequest table
                DE.ScriptExecuteRequest objScriptExecuteRequest = new DE.ScriptExecuteRequest();
                objScriptExecuteRequest.transactionId = objScriptExecuteResponse.transactionId;
                DE.ScriptExecuteRequest objRequestData = objScriptExecuteRequestDS.GetOne(objScriptExecuteRequest);

                // if transaction id is available that means it is a valid transaction id 
                if (objRequestData != null)
                {
                    // check if transaction id and computer name combination is available in response table 
                    DE.ScriptExecuteResponse objResponseData = scriptExecuteResponseDSExt.GetOne(objScriptExecuteResponse);
                    if (objResponseData != null)
                    {
                            executeResponseDS.Update(objScriptExecuteResponse);
                            response.isSuccess = true;
                             response.Message = "details updated sucessfully";
                      
                    }
                    else
                    {
                        response.isSuccess = false;
                        response.Message = String.Format("Transaction id - {0} and computer name  - {1} combination is not available in response table", objScriptExecuteResponse.transactionId.ToString(), objScriptExecuteResponse.computername);
                    }
                }
                else
                {
                    response.isSuccess = false;
                    response.Message = String.Format("Transaction id - {0} )is not available in Request table", objScriptExecuteResponse.transactionId.ToString());

                }
                
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
            return response;
        }

        public override GetTransactionsByStatusResMsg GetTransactionsByStatus(GetTransactionsByStatusReqMsg request)
        {
            GetTransactionsByStatusResMsg response = new GetTransactionsByStatusResMsg();
            try
            {
                ScriptExecuteRequestDSExt scriptExecuteRequestDSExt = new ScriptExecuteRequestDSExt();
                List<TransactionByStatusResponse> scriptExecuteResponsesSE = new List<TransactionByStatusResponse>();
                List<TransactionStatusResponse> scriptExecuteResponsesDE =scriptExecuteRequestDSExt.getTransactionsByStatus(request.Status, request.Referencekey);
                scriptExecuteResponsesSE = Translators.AutomationTracker.ScriptExecuteResponseSE_DE.TransactionStatusResponsetoTransactionByStatusResponseList(scriptExecuteResponsesDE);
                response.StatusResponses = scriptExecuteResponsesSE;
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
            return response;
        }
    }

    public enum Status
    {
        initial,
        queued,
        success,
        failed
    };
}

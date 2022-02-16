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
using Infosys.WEM.Observer.Contracts;
using Infosys.WEM.Observer.Contracts.Message;
using Infosys.WEM.Infrastructure.Common;
using Infosys.WEM.AutomationTracker.Contracts.Message;
using Infosys.WEM.Service.Implementation.Translators.Observer;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.ServiceModel.Activation;
using Infosys.WEM.Client;
using DE = Infosys.WEM.Resource.Entity;
using Infosys.WEM.Resource.DataAccess;
using System.Configuration;

namespace Infosys.WEM.Service.Implementation
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]

    public abstract class ScriptExecuteObserver_ServiceBase : IScriptExecuteObserver
    {
        public virtual IList<UpdateExecutionStatusResMsg> UpdateExecutionStatus(IList<UpdateExecutionStatusReqMsg> value)
        {
            return null;
        }
    }

    public partial class ScriptExecuteObserver : ScriptExecuteObserver_ServiceBase
    {
        public override IList<UpdateExecutionStatusResMsg> UpdateExecutionStatus (IList<UpdateExecutionStatusReqMsg> valueList)
        {
            // create an object for response
            IList<UpdateExecutionStatusResMsg> objUpdateExecutionStatusResMsg = new List<UpdateExecutionStatusResMsg>();

            // create an object for AutomationTracker 
            Infosys.WEM.Client.AutomationTracker objAutomationTracker = new Infosys.WEM.Client.AutomationTracker();
           
            // browse through each input one by one 
            foreach (UpdateExecutionStatusReqMsg value in valueList)
            {
                // variable to check if database has to be udpated 
                bool bIsUpdateRequired = false;

                // if transaction id is available then only proceed 
                if (!string.IsNullOrEmpty(value.scriptExecuteResponse.TransactionId))
                {
                    try
                    {
                        
                        ScriptExecuteResponseDSExt scriptExecuteResponseDSExt = new ScriptExecuteResponseDSExt();
                        DE.ScriptExecuteResponse objScriptExecuteResponse = ObserverAutomationTrackerTransalator.ScriptExecuteResponseSEtoDE(value.scriptExecuteResponse);
                    
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
                            // check if computer name is available in the fetched record 
                            if (objRequestData.remoteservernames.ToString().Trim().ToLower().Contains(objScriptExecuteResponse.computername.Trim().ToLower()))
                            {
                                // check if status value is a valid one 
                                if (Enum.IsDefined(typeof(Status), objScriptExecuteResponse.currentstate.Trim().ToLower()))
                                {
                                    // check if transaction id and computer name combination is available in response table 
                                    DE.ScriptExecuteResponse objResponseData = scriptExecuteResponseDSExt.GetOne(objScriptExecuteResponse);
                                    if (objResponseData != null)
                                    {
                                        // check if status value is queued 
                                        if (objResponseData.currentstate.Trim().Equals(Status.queued.ToString(), StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            // if yes then only update the table 
                                            bIsUpdateRequired = true;
                                        }
                                        else
                                        {
                                            // check if status is same then update 
                                            if (objResponseData.currentstate.Trim().Equals(value.scriptExecuteResponse.CurrentState.Trim(), StringComparison.InvariantCultureIgnoreCase))
                                            {                                                
                                                bIsUpdateRequired = true;
                                            }
                                            else
                                            {
                                                objUpdateExecutionStatusResMsg.Add(new UpdateExecutionStatusResMsg { isSuccess = false, Message = String.Format("CurrentState is not - {0} in database for transaction id - {1} . There is state mismatch . Current state in database is - {2} whereas current state in the request is - {3}", Status.queued.ToString(), objScriptExecuteResponse.transactionId.ToString(), objResponseData.currentstate, objScriptExecuteResponse.currentstate.ToString()), TransactionId = value.scriptExecuteResponse.TransactionId, ComputerName = value.scriptExecuteResponse.ComputerName, CurrentState = value.scriptExecuteResponse.CurrentState });
                                            }
                                        }
                                    }
                                    else
                                    {                                        
                                        objUpdateExecutionStatusResMsg.Add(new UpdateExecutionStatusResMsg { isSuccess = false, Message = String.Format("Transaction id - {0} and computer name  - {1} combination is not available in response table", objScriptExecuteResponse.transactionId.ToString(), objScriptExecuteResponse.computername), TransactionId = value.scriptExecuteResponse.TransactionId, ComputerName = value.scriptExecuteResponse.ComputerName, CurrentState = value.scriptExecuteResponse.CurrentState });
                                    }
                                }
                                else
                                {
                                    objUpdateExecutionStatusResMsg.Add(new UpdateExecutionStatusResMsg { isSuccess = false, Message = String.Format("CurrentState value - {0} is not valid for transaction id - {1} ", objScriptExecuteResponse.currentstate.ToString(), objScriptExecuteResponse.transactionId.ToString()) , TransactionId = value.scriptExecuteResponse.TransactionId, ComputerName = value.scriptExecuteResponse.ComputerName, CurrentState = value.scriptExecuteResponse.CurrentState });
                                }
                            }
                            else
                            {
                                objUpdateExecutionStatusResMsg.Add(new UpdateExecutionStatusResMsg { isSuccess = false, Message = String.Format("Computer Name - {0} is not valid for transaction id - {1} ", objScriptExecuteResponse.computername, objScriptExecuteResponse.transactionId.ToString()), TransactionId = value.scriptExecuteResponse.TransactionId, ComputerName = value.scriptExecuteResponse.ComputerName, CurrentState = value.scriptExecuteResponse.CurrentState });
                            }
                        }
                        else
                        {
                            objUpdateExecutionStatusResMsg.Add(new UpdateExecutionStatusResMsg { isSuccess = false, Message = String.Format("Transaction id - {0} is not available in Request table", objScriptExecuteResponse.transactionId.ToString()), TransactionId = value.scriptExecuteResponse.TransactionId, ComputerName = value.scriptExecuteResponse.ComputerName, CurrentState = value.scriptExecuteResponse.CurrentState });
                        }

                       

                        // if update is required then update the table and proceed further
                        if (bIsUpdateRequired)
                        {
                            string isBypasscertificate = Convert.ToString(ConfigurationManager.AppSettings["ByPassCertificate"]);
                            if (isBypasscertificate.ToUpper() == "YES")
                            {
                                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                                ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;
                            }
                            UpdateTransactionStatusResMsg objUpdateTransactionStatusResMsg = new UpdateTransactionStatusResMsg();

                            // create an input object required for calling automation tracker web service 
                            UpdateTransactionStatusReqMsg objUpdateTransactionStatusReqMsg = new UpdateTransactionStatusReqMsg();
                            objUpdateTransactionStatusReqMsg = ObserverAutomationTrackerTransalator.ExecutionStatusMsgToTransactionStatusMsg(value);

                            //call update method of automation tracker to update value in database 
                            objUpdateTransactionStatusResMsg = objAutomationTracker.ServiceChannel.UpdateTransactionStatus(objUpdateTransactionStatusReqMsg);
                            
                            // if update is successful
                            if (objUpdateTransactionStatusResMsg.isSuccess)
                            {
                                UpdateNotificationDetailsReqMsg objUpdateNotificationDetailsReqMsg = new UpdateNotificationDetailsReqMsg();

                                // once the service is called use the notificationcallbackurl 
                                // check if notification callback url is avaulable or not 
                                if (!string.IsNullOrEmpty(objRequestData.ResponseNotificationCallbackURL))
                                {
                                    // an object of NotificationCallback class for calling java api 
                                    NotificationCallBack objNotificationCallBack = new NotificationCallBack();

                                    // object required for input 
                                    NotificationInput objNotificationInput = new NotificationInput(objRequestData.ResponseNotificationCallbackURL, value.scriptExecuteResponse.TransactionId, value.scriptExecuteResponse.CurrentState);

                                    //object received as output
                                    var objNotificationOutput = Task.Run(() => objNotificationCallBack.NotificationCall(objNotificationInput));
                                    objNotificationOutput.Wait();
                                    objUpdateNotificationDetailsReqMsg = ObserverAutomationTrackerTransalator.ExecutionStatusMsgNotificationDetailsMsg(value, objNotificationOutput.Result);

                                    //if (!objNotificationOutput.Result.IsSuccess)
                                    //{
                                    //    // assign the values received to the input object of updateNotificationDetails
                                    //    objUpdateNotificationDetailsReqMsg = ObserverAutomationTrackerTransalator.ExecutionStatusMsgNotificationDetailsMsg(value, objNotificationOutput.Result);

                                    //}
                                    //else
                                    //{
                                    //    // assign the values received to the input object of updateNotificationDetails
                                    //    objUpdateNotificationDetailsReqMsg = ObserverAutomationTrackerTransalator.ExecutionStatusMsgNotificationDetailsMsg(value, objNotificationOutput.Result);
                                    //    //objUpdateExecutionStatusResMsg.Add(new UpdateExecutionStatusResMsg { isSuccess = false, Message = "Some error in calling Notification Call Back URL", TransactionId = value.scriptExecuteResponse.TransactionId, ComputerName = value.scriptExecuteResponse.ComputerName, CurrentState = value.scriptExecuteResponse.CurrentState });
                                    //}


                                }
                                else
                                {
                                    objUpdateNotificationDetailsReqMsg = ObserverAutomationTrackerTransalator.ExecutionStatusMsgNotificationDetailsMsg(value);
                                    objUpdateNotificationDetailsReqMsg.scriptExecuteResponse.IsNotified = false;
                                    objUpdateNotificationDetailsReqMsg.scriptExecuteResponse.NotificationRemarks = "Call back Url is not available";

                                }
                                
                                // update notification details through automation tracker
                                //call updateTransactionStatus method of automationtracker 
                                UpdateNotificationDetailsResMsg objUpdateNotificationDetailsResMsg = objAutomationTracker.ServiceChannel.UpdateNotificationDetails(objUpdateNotificationDetailsReqMsg);
                                objUpdateExecutionStatusResMsg.Add(new UpdateExecutionStatusResMsg { isSuccess = objUpdateNotificationDetailsResMsg.isSuccess, Message = objUpdateNotificationDetailsResMsg.Message, TransactionId = value.scriptExecuteResponse.TransactionId, ComputerName = value.scriptExecuteResponse.ComputerName, CurrentState = value.scriptExecuteResponse.CurrentState });

                            }
                            else
                            {                                
                                objUpdateExecutionStatusResMsg.Add(new UpdateExecutionStatusResMsg { isSuccess = false, Message = objUpdateTransactionStatusResMsg.Message, TransactionId = value.scriptExecuteResponse.TransactionId, ComputerName = value.scriptExecuteResponse.ComputerName, CurrentState = value.scriptExecuteResponse.CurrentState });
                            }
                        }

                    }
                    catch (Exception wemObserverException)
                    {
                        objUpdateExecutionStatusResMsg.Add(new UpdateExecutionStatusResMsg { isSuccess = false, Message = wemObserverException.ToString() , TransactionId = value.scriptExecuteResponse.TransactionId, ComputerName = value.scriptExecuteResponse.ComputerName, CurrentState = value.scriptExecuteResponse.CurrentState });
                        Exception ex = new Exception();
                        bool rethrow = ExceptionHandler.HandleException(wemObserverException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);
                    }
                }
                else
                {
                    objUpdateExecutionStatusResMsg.Add(new UpdateExecutionStatusResMsg { isSuccess = false, Message = "Transaction id is not provided" , TransactionId = value.scriptExecuteResponse.TransactionId , ComputerName = value.scriptExecuteResponse.ComputerName , CurrentState = value.scriptExecuteResponse.CurrentState });
                }
            }

            return objUpdateExecutionStatusResMsg;
        }
    }


}

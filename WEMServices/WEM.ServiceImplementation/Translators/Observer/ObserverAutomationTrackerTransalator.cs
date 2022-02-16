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
using Infosys.WEM.AutomationTracker.Contracts.Message;
using SE = Infosys.WEM.Observer.Contracts.Data;
using Infosys.WEM.Observer.Contracts.Message;
using DE = Infosys.WEM.Resource.Entity;

namespace Infosys.WEM.Service.Implementation.Translators.Observer
{
    public class ObserverAutomationTrackerTransalator
    {
        /// <summary>
        /// method to convert the object received by observer to the one which is acceptabale by Automation tracker service
        /// </summary>
        /// <param name="updateExecutionStatusReqMsg">the object received by observer service</param>
        /// <returns>the object required by Automation tracker service</returns>
        public static UpdateTransactionStatusReqMsg ExecutionStatusMsgToTransactionStatusMsg(UpdateExecutionStatusReqMsg updateExecutionStatusReqMsg)
        {
            
            UpdateTransactionStatusReqMsg objUpdateTransactionStatusReqMsg = new UpdateTransactionStatusReqMsg();
           
          
            if (updateExecutionStatusReqMsg != null)
            {
                objUpdateTransactionStatusReqMsg.scriptExecuteResponse = new WEM.AutomationTracker.Contracts.Data.ScriptExecuteResponse();
                objUpdateTransactionStatusReqMsg.scriptExecuteResponse.TransactionId = new Guid(updateExecutionStatusReqMsg.scriptExecuteResponse.TransactionId);
                objUpdateTransactionStatusReqMsg.scriptExecuteResponse.ComputerName = updateExecutionStatusReqMsg.scriptExecuteResponse.ComputerName;
                objUpdateTransactionStatusReqMsg.scriptExecuteResponse.CurrentState = updateExecutionStatusReqMsg.scriptExecuteResponse.CurrentState;
                objUpdateTransactionStatusReqMsg.scriptExecuteResponse.ErrorMessage = updateExecutionStatusReqMsg.scriptExecuteResponse.ErrorMessage;
                objUpdateTransactionStatusReqMsg.scriptExecuteResponse.InputCommand = updateExecutionStatusReqMsg.scriptExecuteResponse.InputCommand;
                //objUpdateTransactionStatusReqMsg.scriptExecuteResponse.IsSuccess = updateExecutionStatusReqMsg.scriptExecuteResponse.IsSuccess;
                objUpdateTransactionStatusReqMsg.scriptExecuteResponse.LogData = updateExecutionStatusReqMsg.scriptExecuteResponse.LogData;
                objUpdateTransactionStatusReqMsg.scriptExecuteResponse.SourceTransactionId = updateExecutionStatusReqMsg.scriptExecuteResponse.SourceTransactionId;
                objUpdateTransactionStatusReqMsg.scriptExecuteResponse.SuccessMessage = updateExecutionStatusReqMsg.scriptExecuteResponse.SuccessMessage;
                if (updateExecutionStatusReqMsg.scriptExecuteResponse.OutParameters!= null)
                {
                    if(updateExecutionStatusReqMsg.scriptExecuteResponse.OutParameters.Count!=0)
                        objUpdateTransactionStatusReqMsg.scriptExecuteResponse.OutParameters = updateExecutionStatusReqMsg.scriptExecuteResponse.OutParameters.ConvertAll(
                        x => new Infosys.WEM.AutomationTracker.Contracts.Data.Parameter { ParameterName = x.ParameterName, ParameterValue = x.ParameterValue });
                }
                 
            }

            return objUpdateTransactionStatusReqMsg;
        }


        public static  UpdateNotificationDetailsReqMsg ExecutionStatusMsgNotificationDetailsMsg(UpdateExecutionStatusReqMsg updateExecutionStatusReqMsg,NotificationOutput notificationOutput)
        {
            UpdateNotificationDetailsReqMsg objUpdateNotificationDetailsReqMsg = new UpdateNotificationDetailsReqMsg();

            if(updateExecutionStatusReqMsg != null && notificationOutput !=null)
            {
                
                objUpdateNotificationDetailsReqMsg.scriptExecuteResponse = new WEM.AutomationTracker.Contracts.Data.ScriptExecuteResponse();
                objUpdateNotificationDetailsReqMsg.scriptExecuteResponse.TransactionId = new Guid(updateExecutionStatusReqMsg.scriptExecuteResponse.TransactionId);
                objUpdateNotificationDetailsReqMsg.scriptExecuteResponse.ComputerName = updateExecutionStatusReqMsg.scriptExecuteResponse.ComputerName;
                objUpdateNotificationDetailsReqMsg.scriptExecuteResponse.CurrentState = updateExecutionStatusReqMsg.scriptExecuteResponse.CurrentState;
                objUpdateNotificationDetailsReqMsg.scriptExecuteResponse.ErrorMessage = updateExecutionStatusReqMsg.scriptExecuteResponse.ErrorMessage;
                objUpdateNotificationDetailsReqMsg.scriptExecuteResponse.InputCommand = updateExecutionStatusReqMsg.scriptExecuteResponse.InputCommand;
                //objUpdateNotificationDetailsReqMsg.scriptExecuteResponse.IsSuccess = updateExecutionStatusReqMsg.scriptExecuteResponse.IsSuccess;
                objUpdateNotificationDetailsReqMsg.scriptExecuteResponse.LogData = updateExecutionStatusReqMsg.scriptExecuteResponse.LogData;
                objUpdateNotificationDetailsReqMsg.scriptExecuteResponse.SourceTransactionId = updateExecutionStatusReqMsg.scriptExecuteResponse.SourceTransactionId;
                objUpdateNotificationDetailsReqMsg.scriptExecuteResponse.SuccessMessage = updateExecutionStatusReqMsg.scriptExecuteResponse.SuccessMessage;
                objUpdateNotificationDetailsReqMsg.scriptExecuteResponse.IsNotified = notificationOutput.IsNotified;
                objUpdateNotificationDetailsReqMsg.scriptExecuteResponse.NotificationRemarks = notificationOutput.NotificationRemarks;
                
            }
            return objUpdateNotificationDetailsReqMsg;
        }

        public static UpdateNotificationDetailsReqMsg ExecutionStatusMsgNotificationDetailsMsg(UpdateExecutionStatusReqMsg updateExecutionStatusReqMsg)
        {
            UpdateNotificationDetailsReqMsg objUpdateNotificationDetailsReqMsg = new UpdateNotificationDetailsReqMsg();

            if (updateExecutionStatusReqMsg != null)
            {
                objUpdateNotificationDetailsReqMsg.scriptExecuteResponse = new WEM.AutomationTracker.Contracts.Data.ScriptExecuteResponse();
                objUpdateNotificationDetailsReqMsg.scriptExecuteResponse.TransactionId = new Guid(updateExecutionStatusReqMsg.scriptExecuteResponse.TransactionId);
                objUpdateNotificationDetailsReqMsg.scriptExecuteResponse.ComputerName = updateExecutionStatusReqMsg.scriptExecuteResponse.ComputerName;
                objUpdateNotificationDetailsReqMsg.scriptExecuteResponse.CurrentState = updateExecutionStatusReqMsg.scriptExecuteResponse.CurrentState;
                objUpdateNotificationDetailsReqMsg.scriptExecuteResponse.ErrorMessage = updateExecutionStatusReqMsg.scriptExecuteResponse.ErrorMessage;
                objUpdateNotificationDetailsReqMsg.scriptExecuteResponse.InputCommand = updateExecutionStatusReqMsg.scriptExecuteResponse.InputCommand;
                //objUpdateNotificationDetailsReqMsg.scriptExecuteResponse.IsSuccess = updateExecutionStatusReqMsg.scriptExecuteResponse.IsSuccess;
                objUpdateNotificationDetailsReqMsg.scriptExecuteResponse.LogData = updateExecutionStatusReqMsg.scriptExecuteResponse.LogData;
                objUpdateNotificationDetailsReqMsg.scriptExecuteResponse.SourceTransactionId = updateExecutionStatusReqMsg.scriptExecuteResponse.SourceTransactionId;
                objUpdateNotificationDetailsReqMsg.scriptExecuteResponse.SuccessMessage = updateExecutionStatusReqMsg.scriptExecuteResponse.SuccessMessage;

            }
            return objUpdateNotificationDetailsReqMsg;
        }

        public static DE.ScriptExecuteResponse ScriptExecuteResponseSEtoDE(SE.ScriptExecuteResponse scriptExecuteResponseSE)
        {
            DE.ScriptExecuteResponse scriptExecuteResponseDE = null;
            if (scriptExecuteResponseSE != null)
            {
                scriptExecuteResponseDE = new DE.ScriptExecuteResponse();
                //scriptExecuteResponseDE.transactionId = scriptExecuteResponseSE.TransactionId.ToString();
                scriptExecuteResponseDE.transactionId = Guid.Parse(scriptExecuteResponseSE.TransactionId);
                scriptExecuteResponseDE.currentstate = scriptExecuteResponseSE.CurrentState;
                scriptExecuteResponseDE.successmessage = scriptExecuteResponseSE.SuccessMessage;
                scriptExecuteResponseDE.errormessage = scriptExecuteResponseSE.ErrorMessage;
                //scriptExecuteResponseDE.issuccess = Convert.ToBoolean(scriptExecuteResponseSE.IsSuccess);
                scriptExecuteResponseDE.computername = scriptExecuteResponseSE.ComputerName;
                scriptExecuteResponseDE.inputcommand = scriptExecuteResponseSE.InputCommand;
                scriptExecuteResponseDE.IsNotified = scriptExecuteResponseSE.IsNotified;
                scriptExecuteResponseDE.NotificationRemarks = scriptExecuteResponseSE.NotificationRemarks;
                scriptExecuteResponseDE.LogData = scriptExecuteResponseSE.LogData;
                if (scriptExecuteResponseSE.OutParameters != null)
                    scriptExecuteResponseDE.OutParameters = Newtonsoft.Json.JsonConvert.SerializeObject(scriptExecuteResponseSE.OutParameters);
                //scriptExecuteResponseDE.createdby = scriptExecuteResponseSE.CreatedBy;
                //scriptExecuteResponseDE.createddate = scriptExecuteResponseSE.CreatedOn;
                //scriptExecuteResponseDE.modifiedby = scriptExecuteResponseSE.ModifiedBy;
                //scriptExecuteResponseDE.modifieddate = scriptExecuteResponseSE.ModifiedOn ;
            }

            return scriptExecuteResponseDE;
        }

    }
}

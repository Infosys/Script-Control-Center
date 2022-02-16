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

using WEM.ScriptExecution.Contracts;
using WEM.ScriptExecution.Message;
using Infosys.WEM.Infrastructure.Common;
using Infra = Infosys.WEM.ScriptExecutionLibrary;
using System.ServiceModel.Activation;
using WEM.ScriptExecution.Data;
using Infosys.WEM.Client;
using Infosys.WEM.AutomationTracker.Contracts.Data;
using AT=Infosys.WEM.AutomationTracker.Contracts.Message;

namespace WEM.ScriptExecution.Implementation
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public abstract class ScriptExecute_ServiceBase:IScriptExecute
    {
        #region IScriptExecute Members
        public virtual GetTransactionStatusResMsg GetTransactionStatus(GetTransactionStatusReqMsg value)
        {
            return null;
        }

        public virtual Task<InitiateExecutionResMsg> AsyncInitiateExecution(InitiateExecutionReqMsg value)
        {
            return null;
        }

        public virtual InitiateExecutionResMsg InitiateExecution(InitiateExecutionReqMsg value)
        {
            return null;
        }

        //public virtual UpdateTransactionStatusResMsg UpdateTransactionStatus(UpdateTransactionStatusReqMsg value)
        //{
        //    return null;
        //}

        #endregion
    }

    public partial class ScriptExecute : ScriptExecute_ServiceBase
    {
        public override InitiateExecutionResMsg InitiateExecution(InitiateExecutionReqMsg value)
        {
            //Block0_3
            DateTime processStartedTime0_3 = DateTime.Now;
            using (LogHandler.TraceOperations("ScriptExecute:InitiateExecutionCall", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
            {               
                InitiateExecutionResMsg response = new InitiateExecutionResMsg();
                try
                {
                    Infra.ScriptIndentifier infraScriptIden = Translator.ScriptIdentifier_SE_IE.ScriptIndentifierSEtoIE(value.ScriptIdentifier);
                    List<Infra.ExecutionResult> consolidatedResult = Infra.ScriptExecutionManager.Execute(infraScriptIden);

                    if (consolidatedResult != null)
                    {
                        response = Translator.ScriptIdentifier_SE_IE.ScriptIndentifierIEListtoSM(consolidatedResult);
                    }
                }
                catch (Exception wemScriptException)
                {
                    Exception ex = new Exception();
                    bool rethrow = ExceptionHandler.HandleException(wemScriptException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);
                    List<ScriptResponse> scriptResponses = new List<ScriptResponse>();
                    ScriptResponse scriptResponse = new ScriptResponse();
                    scriptResponse.ErrorMessage = wemScriptException.Message;
                    scriptResponses.Add(scriptResponse);
                    response.ScriptResponse = scriptResponses;
                    if (rethrow)
                    {
                        throw ex;
                    }
                }
                LogHandler.LogError(string.Format("Time taken by ScriptExecute:Block 0_3 (InitiateExecution) : {0}", DateTime.Now.Subtract(processStartedTime0_3).TotalSeconds), LogHandler.Layer.Business, null);

                return response;
            }
        }
        
        public override async Task<InitiateExecutionResMsg> AsyncInitiateExecution(InitiateExecutionReqMsg value)
        {
            var task = Task.Factory.StartNew(()=>
            {
                InitiateExecutionResMsg response = new InitiateExecutionResMsg();
                try
                {
                    Infra.ScriptIndentifier infraScriptIden = Translator.ScriptIdentifier_SE_IE.ScriptIndentifierSEtoIE(value.ScriptIdentifier);
                    List<Infra.ExecutionResult> consolidatedResult = Infra.ScriptExecutionManager.Execute(infraScriptIden);

                    if (consolidatedResult != null)
                    {
                        response = Translator.ScriptIdentifier_SE_IE.ScriptIndentifierIEListtoSM(consolidatedResult);
                    }

                }
                catch (Exception wemScriptException)
                {
                    Exception ex = new Exception();
                    bool rethrow = ExceptionHandler.HandleException(wemScriptException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);
                    List<ScriptResponse> scriptResponses = new List<ScriptResponse>();
                    ScriptResponse scriptResponse = new ScriptResponse();
                    scriptResponse.ErrorMessage = wemScriptException.Message;
                    scriptResponses.Add(scriptResponse);
                    response.ScriptResponse = scriptResponses;

                    if (rethrow)
                    {
                        throw ex;
                    }
                }
                return response;
            });
            
            return await task;
        }

        

        public override GetTransactionStatusResMsg GetTransactionStatus(GetTransactionStatusReqMsg value)
        {
            GetTransactionStatusResMsg response = new GetTransactionStatusResMsg();
            try
            {
                AutomationTracker automationTrackerclient = new AutomationTracker();
                var channel = automationTrackerclient.ServiceChannel;
                AT.GetTransactionStatusReqMsg reqMsg = new AT.GetTransactionStatusReqMsg();
                reqMsg.TransactionId = value.TransactionId;
                AT.GetTransactionStatusResMsg resMsg= channel.GetTransactionStatus(reqMsg);

                List<ScriptResponse> scriptResponses = new List<ScriptResponse>();
                foreach (ScriptExecuteResponse executeResponse in resMsg.scriptExecuteResponse)
                {
                    List<Data.Parameter> outParams = new List<Data.Parameter>();
                    if (executeResponse.OutParameters != null)
                    {
                        foreach (Infosys.WEM.AutomationTracker.Contracts.Data.Parameter parameter in executeResponse.OutParameters)
                        {
                            Data.Parameter param = new Data.Parameter();
                            param.ParameterName = parameter.ParameterName;
                            param.ParameterValue = parameter.ParameterValue;
                            outParams.Add(param);
                        }
                    }
                    ScriptResponse scriptResponse = new ScriptResponse()
                    {
                        TransactionId = executeResponse.TransactionId.ToString(),
                        Status = executeResponse.CurrentState,
                        SuccessMessage= executeResponse.SuccessMessage,
                        ErrorMessage = executeResponse.ErrorMessage,
                        IsSuccess = executeResponse.IsSuccess,
                        OutParameters = outParams,
                        LogData=executeResponse.LogData,
                        SourceTransactionId= executeResponse.SourceTransactionId,
                        ComputerName=executeResponse.ComputerName,
                        InputCommand=executeResponse.InputCommand
                    };
                    scriptResponses.Add(scriptResponse);
                }
                
                response.ScriptResponse = scriptResponses;
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

        //public override UpdateTransactionStatusResMsg UpdateTransactionStatus(UpdateTransactionStatusReqMsg value)
        //{
        //    UpdateTransactionStatusResMsg response = new UpdateTransactionStatusResMsg();
        //    try
        //    {
        //        AutomationTracker automationTrackerclient = new AutomationTracker();
        //        var channel = automationTrackerclient.ServiceChannel;
        //        AT.UpdateTransactionStatusReqMsg updateTransaction = new AT.UpdateTransactionStatusReqMsg();

        //        List<Infosys.WEM.AutomationTracker.Contracts.Data.Parameter> outParams = new List<Infosys.WEM.AutomationTracker.Contracts.Data.Parameter>();
        //        if (value.scriptResponse != null && value.scriptResponse.OutParameters != null)
        //        {
        //            foreach (Data.Parameter parameter in value.scriptResponse.OutParameters)
        //            {
        //                Infosys.WEM.AutomationTracker.Contracts.Data.Parameter param = new Infosys.WEM.AutomationTracker.Contracts.Data.Parameter();
        //                param.ParameterName = parameter.ParameterName;
        //                param.ParameterValue = parameter.ParameterValue;
        //                outParams.Add(param);
        //            }
        //        }

        //        ScriptExecuteResponse executeResponse = new ScriptExecuteResponse()
        //        {
        //            TransactionId=Guid.Parse(value.scriptResponse.TransactionId),
        //            CurrentState=value.scriptResponse.Status,
        //            SuccessMessage= value.scriptResponse.SuccessMessage,
        //            ErrorMessage= value.scriptResponse.ErrorMessage,
        //            IsSuccess= value.scriptResponse.IsSuccess,
        //            OutParameters= outParams,
        //            LogData= value.scriptResponse.LogData,
        //            SourceTransactionId = value.scriptResponse.SourceTransactionId,
        //            ComputerName=value.scriptResponse.ComputerName,
        //            InputCommand=value.scriptResponse.InputCommand
        //        };
        //        updateTransaction.scriptExecuteResponse = executeResponse;
        //        AT.UpdateTransactionStatusResMsg updateResmsg= channel.UpdateTransactionStatus(updateTransaction);
                
        //        response.isSuccess = updateResmsg.isSuccess;
        //    }
        //    catch (Exception wemScriptException)
        //    {
        //        Exception ex = new Exception();
        //        bool rethrow = ExceptionHandler.HandleException(wemScriptException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

        //        if (rethrow)
        //        {
        //            throw ex;
        //        }
        //    }
        //    return response;
        //}
    }
}

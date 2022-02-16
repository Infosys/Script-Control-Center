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

using IE = Infosys.WEM.ScriptExecutionLibrary;
using SE = WEM.ScriptExecution.Data;
using SM = WEM.ScriptExecution.Message;
using Infra = Infosys.WEM.ScriptExecutionLibrary;
using System.Security;
using Infosys.WEM.SecureHandler;
using Infosys.WEM.Infrastructure.Common;

namespace WEM.ScriptExecution.Translator
{
    public class ScriptIdentifier_SE_IE
    {
        public static IE.ScriptIndentifier ScriptIndentifierSEtoIE(SE.ScriptIdentifier scriptIdenSE)
        {
            //Block 0_1
            DateTime processStartedTime0_1 = DateTime.Now;
            using (LogHandler.TraceOperations("ScriptIdentifier_SE_IE:ScriptIndentifierSEtoIE", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
            {
               
                IE.ScriptIndentifier scriptIdenIE = null;
                if (scriptIdenSE != null)
                {
                    scriptIdenIE = new IE.ScriptIndentifier();
                    scriptIdenIE.CompanyId = scriptIdenSE.CompanyId;
                    scriptIdenIE.ScriptId = scriptIdenSE.ScriptId > 0 ? scriptIdenSE.ScriptId : 0;
                    scriptIdenIE.ScriptName = !string.IsNullOrEmpty(scriptIdenSE.ScriptName) ? scriptIdenSE.ScriptName : null;
                    scriptIdenIE.Parameters = Translator.Parameter_SE_IE.ParameterListSEtoIE(scriptIdenSE.Parameters);
                    scriptIdenIE.Path = scriptIdenSE.Path;
                    scriptIdenIE.RemoteServerNames = !string.IsNullOrEmpty(scriptIdenSE.RemoteServerNames) ? scriptIdenSE.RemoteServerNames : null;
                    scriptIdenIE.SubCategoryId = scriptIdenSE.CategoryId > 0 ? scriptIdenSE.CategoryId : 0;
                    scriptIdenIE.ResponseNotificationCallbackURL = scriptIdenSE.ResponseNotificationCallbackURL != null ? scriptIdenSE.ResponseNotificationCallbackURL : null;
                    scriptIdenIE.UserName = !string.IsNullOrEmpty(scriptIdenSE.UserName) ? scriptIdenSE.UserName : null;
                    var decryptedPassword = !string.IsNullOrEmpty(scriptIdenSE.Password) ? SecurePayload.UnSecure(scriptIdenSE.Password, "IAP2GO_SEC!URE") : null;
                    if (decryptedPassword != null)
                    {
                        var secStr = new SecureString();
                        decryptedPassword.ToList().ForEach(x => secStr.AppendChar(x));
                        scriptIdenIE.Password = secStr;
                    }
                    scriptIdenIE.ReferenceKey = !string.IsNullOrEmpty(scriptIdenSE.ReferenceKey) ? scriptIdenSE.ReferenceKey : null;
                    scriptIdenIE.ExecutionMode = scriptIdenSE.ExecutionMode > 0 ? (Infosys.WEM.ScriptExecutionLibrary.ExecutionModeType)((int)scriptIdenSE.ExecutionMode) : 0;
                    //Added Mapping for RemoteExecutionMode
                    scriptIdenIE.RemoteExecutionMode = scriptIdenSE.RemoteExecutionMode > 0 ? (Infosys.WEM.ScriptExecutionLibrary.ScriptIndentifier.RemoteExecutionHost)((int)scriptIdenSE.RemoteExecutionMode) : 0;
                    scriptIdenIE.Domain = !string.IsNullOrEmpty(scriptIdenSE.Domain) ? scriptIdenSE.Domain : null;
                    scriptIdenIE.IapNodeTransport = scriptIdenSE.IapNodeTransport > 0 ? (Infosys.WEM.ScriptExecutionLibrary.IapNodeTransportType)((int)scriptIdenSE.IapNodeTransport) : 0;
                    scriptIdenIE.WorkingDir = !string.IsNullOrEmpty(scriptIdenSE.WorkingDir) ? scriptIdenSE.WorkingDir : null;
                    //Removing the correlation id for iap and nia
                    if (scriptIdenIE.Parameters != null && (scriptIdenIE.ExecutionMode == Infosys.WEM.ScriptExecutionLibrary.ExecutionModeType.RunOnIAPNode || scriptIdenIE.ExecutionMode == Infosys.WEM.ScriptExecutionLibrary.ExecutionModeType.Delegate))
                    {
                        scriptIdenIE.Parameters.Remove(scriptIdenIE.Parameters.Where(p => p.ParameterName == "rpe_correlationid").FirstOrDefault());
                    }

                }
                LogHandler.LogError(string.Format("Time taken by ScriptExecution.Translator:Block (Request translator) : {0}", DateTime.Now.Subtract(processStartedTime0_1).TotalSeconds), LogHandler.Layer.Business, null);

                return scriptIdenIE;
            }
        }

        public static SM.InitiateExecutionResMsg ScriptIndentifierIEtoSM(IE.ExecutionResult result)
        {
            SM.InitiateExecutionResMsg response = new SM.InitiateExecutionResMsg();
            List<SE.ScriptResponse> scriptResponses = new List<SE.ScriptResponse>();
            SE.ScriptResponse scriptResponse = new SE.ScriptResponse();
            scriptResponse.IsSuccess = result.IsSuccess;
            scriptResponse.SuccessMessage = result.SuccessMessage;
            scriptResponse.ErrorMessage = result.ErrorMessage;
            scriptResponse.TransactionId = result.TransactionId.ToString();
            scriptResponse.Status = result.Status.ToUpper();
            scriptResponse.ComputerName = result.ComputerName;
            scriptResponse.InputCommand = result.InputCommand;

            scriptResponse.OutParameters = new List<SE.Parameter>();
            if (result.Output != null)
            {
                foreach (Infra.OutParameter resultParam in result.Output)
                {
                    SE.Parameter respParam = new SE.Parameter();
                    respParam.ParameterName = resultParam.ParameterName;
                    respParam.ParameterValue =Convert.ToString(resultParam.ParameterValue);
                    scriptResponse.OutParameters.Add(respParam);
                }
            }
            scriptResponse.SourceTransactionId = result.SourceTransactionId;
            scriptResponses.Add(scriptResponse);
            response.ScriptResponse = scriptResponses;
            return response;
        }


        public static SM.InitiateExecutionResMsg ScriptIndentifierIEListtoSM(List<IE.ExecutionResult> results)
        {
            //Block 0_1
            DateTime processStartedTime0_2 = DateTime.Now;
            using (LogHandler.TraceOperations("ScriptIdentifier_SE_IE:ScriptIndentifierIEListtoSM", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
            {
                
                SM.InitiateExecutionResMsg response = new SM.InitiateExecutionResMsg();
                List<SE.ScriptResponse> scriptResponses = new List<SE.ScriptResponse>();
                if (results != null)
                {
                    foreach (IE.ExecutionResult result in results)
                    {
                        SE.ScriptResponse scriptResponse = new SE.ScriptResponse();
                        scriptResponse.IsSuccess = result.IsSuccess;
                        scriptResponse.SuccessMessage = result.SuccessMessage;
                        scriptResponse.ErrorMessage = result.ErrorMessage;
                        scriptResponse.TransactionId = Convert.ToString(result.TransactionId);
                        if (!string.IsNullOrEmpty(result.Status))
                        {
                            scriptResponse.Status = result.Status.ToUpper();
                        }
                        scriptResponse.ComputerName = result.ComputerName;
                        scriptResponse.InputCommand = result.InputCommand;

                        scriptResponse.OutParameters = new List<SE.Parameter>();
                        if (result.Output != null)
                        {
                            foreach (Infra.OutParameter resultParam in result.Output)
                            {
                                SE.Parameter respParam = new SE.Parameter();
                                respParam.ParameterName = resultParam.ParameterName;
                                respParam.ParameterValue = Convert.ToString(resultParam.ParameterValue);
                                scriptResponse.OutParameters.Add(respParam);
                            }
                        }
                        scriptResponse.SourceTransactionId = result.SourceTransactionId;
                        scriptResponses.Add(scriptResponse);
                    }
                }
                response.ScriptResponse = scriptResponses;
                LogHandler.LogError(string.Format("Time taken by ScriptExecution.Translator:Block (Response translator) : {0}", DateTime.Now.Subtract(processStartedTime0_2).TotalSeconds), LogHandler.Layer.Business, null);

                return response;
            }
        }

    }
}

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

using Infosys.WEM.WorkflowExecutionLibrary;
using System.Configuration;
using Infosys.WEM.WorkflowExecutionLibrary.Entity;
using Infosys.WEM.ScriptExecutionLibrary;
using Infosys.WEM.SecureHandler;
using Infosys.ATR.AutomationEngine.Contracts;
using System.ServiceModel;

namespace Infosys.ATR.AutomationExecutionLib
{
    public class NodeService : INodeService
    {

        #region INodeService Members

        [OperationBehavior(Impersonation = ImpersonationOption.Required)]
        public Result ExecuteWf(ExecuteWfReq request)
        {
            //fetch the work flow based on the id
            //then execute it locally

            Result result = new Result();
            //check if the concerned node is configured to support work flow execution
            if (ExectutionSupportedFor(SupportedExecution.Workflow))
            {
                //to impersonate the caller
                using (ServiceSecurityContext.Current.WindowsIdentity.Impersonate())
                {
                    try
                    {
                        //string serviceUrl = System.Configuration.ConfigurationManager.AppSettings["WEMService"];
                        //if (!string.IsNullOrEmpty(serviceUrl))
                        //{

                        //reading the work flow service end points details from the client configuration instead of appsettings
                        WorkflowIndentifier wfid = new WorkflowIndentifier();
                        //wfid.WEMWorkflowServiceUrl = serviceUrl;
                        wfid.CategoryId = request.CategoryId;
                        wfid.WorkflowId = Guid.Parse(request.WorkflowId);
                        wfid.WorkflowVersion = request.WorkflowVer;
                        wfid.Parameters = Translator.WorkflowParameter_SE_IE.WorkflowParameterListSEtoIE(request.Parameters);

                        Infosys.WEM.WorkflowExecutionLibrary.Entity.ExecutionResult execResult =
                            new WorkflowExecutionManager().Execute(wfid);
                        result.IsSuccess = execResult.IsSuccess;
                        result.SuccessMessage = execResult.SuccessMessage;
                        result.ErrorMessage = execResult.ErrorMessage;

                        if (execResult.Output != null && execResult.Output.Count > 0)
                        {
                            execResult.Output.ForEach(outPara =>
                            {
                                if (outPara.ParameterValue is object && outPara.IsSecret)
                                    outPara.ParameterValue = SecurePayload.Secure((new WorkflowExecutionManager().JsonSerialize(outPara.ParameterValue)), "IAP2GO_SEC!URE");
                            });
                        }

                        result.Output = Translator.WF_OutputParameter_SE_IE.OutputParameterListIEToSE(execResult.Output);
                        //}
                        //else
                        //{
                        //    result.IsSuccess = false;
                        //    result.ErrorMessage = "No WEM Work flow service configured";
                        //}
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        string err = ex.Message;
                        if (ex.InnerException != null)
                            err = err + ". \nInner Exception- " + ex.InnerException.Message;
                        result.ErrorMessage = err;
                    }
                }
            }
            else
            {
                result.IsSuccess = false;
                result.ErrorMessage = "Work flow execution is not supported on this node- " + Environment.MachineName;
            }
            return result;
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Required)]
        public Result ExecuteScript(ExecuteScriptReq request)
        {
            //fetch the script based on the id
            //then execute it locally

            Result result = new Result();
            //check if the concerned node is configured to support script execution
            if (ExectutionSupportedFor(SupportedExecution.Script))
            {
                //to impersonate the caller
                using (ServiceSecurityContext.Current.WindowsIdentity.Impersonate())
                {
                    try
                    {
                        //string serviceUrl = System.Configuration.ConfigurationManager.AppSettings["WEMScriptService"];
                        //if (!string.IsNullOrEmpty(serviceUrl))
                        //{
                        //reading the script service end points details from the client configuration instead of appsettings
                        ScriptIndentifier scriptIden = new ScriptIndentifier();
                        scriptIden.ScriptId = request.ScriptId;
                        scriptIden.SubCategoryId = request.CategoryId;
                        //scriptIden.WEMScriptServiceUrl = serviceUrl;
                        scriptIden.Parameters = Translator.ScriptParameter_SE_IE.ScriptParameterListSEtoIE(request.Parameters);

                        //Infosys.WEM.ScriptExecutionLibrary.ExecutionResult execResult = ScriptExecutionManager.Execute(scriptIden);
                        List<Infosys.WEM.ScriptExecutionLibrary.ExecutionResult> consolidatedResult = ScriptExecutionManager.Execute(scriptIden);
                        Infosys.WEM.ScriptExecutionLibrary.ExecutionResult execResult = consolidatedResult[0];

                        result.IsSuccess = execResult.IsSuccess;
                        result.SuccessMessage = execResult.SuccessMessage;
                        result.ErrorMessage = execResult.ErrorMessage;
                        result.InputCommand = execResult.InputCommand;
                        //}
                        //else
                        //{
                        //    result.IsSuccess = false;
                        //    result.ErrorMessage = "No WEM Script service configured";
                        //}
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        string err = ex.Message;
                        if (ex.InnerException != null)
                            err = err + ". \nInner Exception- " + ex.InnerException.Message;
                        result.ErrorMessage = err;
                        result.ErrorMessage += "\n Stack trace- " + ex.StackTrace;
                    }
                }
            }
            else
            {
                result.IsSuccess = false;
                result.ErrorMessage = "Script execution is not supported on this node- " + Environment.MachineName;
            }
            return result;
        }

        public string Ping()
        {
            string message = "Success (200)- I am breathing perfectly at- " + DateTime.UtcNow.ToString() + "(UTC). My hosting Machine is- " + Environment.MachineName;
            return message;
        }

        #endregion

        private bool ExectutionSupportedFor(SupportedExecution execute)
        {
            bool isSupported = false;
            string supportedExecution = System.Configuration.ConfigurationManager.AppSettings["ExecutionEngineSupported"];
            int isupportedExecution;
            if (!string.IsNullOrEmpty(supportedExecution) && int.TryParse(supportedExecution, out isupportedExecution))
            {
                string binary = Convert.ToString(isupportedExecution, 2);
                switch (execute)
                {
                    case SupportedExecution.Workflow: //i.e. binary shud be 1, 11, 111, etc
                        if (binary.Length >= 1 && binary[binary.Length - 1] == '1')
                            isSupported = true;
                        break;
                    case SupportedExecution.Script: //i.e. binary shud be 10, 11, 110, 111, etc
                        if (binary.Length >= 2 && binary[binary.Length - 2] == '1')
                            isSupported = true;
                        break;
                    //similary add other cases as needed for newly supported execution engine in future
                }
            }
            return isSupported;
        }
    }

    public enum SupportedExecution
    {
        Workflow,
        Script
    }
}

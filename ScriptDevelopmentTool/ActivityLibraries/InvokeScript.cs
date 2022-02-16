/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

using System.ComponentModel;
using System.Activities.Presentation.Metadata;
using System.Activities.Presentation.PropertyEditing;
using Infosys.WEM.Infrastructure.Common;
using Designer = Infosys.WEM.AutomationActivity.Designers;
using Infosys.WEM.ScriptExecutionLibrary;
using System.Security;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Management.Automation.Runspaces;
using System.Management.Automation;
using Infosys.WEM.SecureHandler;

namespace Infosys.WEM.AutomationActivity.Libraries
{


    [Designer(typeof(Designer.InvokeScript))]
    public sealed class InvokeScript : NativeActivity
    {

        private const string ACTIVITY_NAME = "InvokeScript";
        public InvokeScript()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();
            //builder.AddCustomAttributes(typeof(InvokeScripts), "ScriptCategory", new EditorAttribute(typeof(ListSelectionEditor), typeof(PropertyValueEditor)));
            //builder.AddCustomAttributes(typeof(InvokeScripts), "one", new EditorAttribute(typeof(ListSelectionEditor), typeof(PropertyValueEditor)));
            //builder.AddCustomAttributes(typeof(InvokeScripts), "ScriptNames", new EditorAttribute(typeof(ListSelectionEditor), typeof(PropertyValueEditor)));

            //builder.AddCustomAttributes(typeof(SimpleCodeActivity), "FileName", new EditorAttribute(typeof(FilePickerEditor), typeof(DialogPropertyValueEditor)));
            MetadataStore.AddAttributeTable(builder.CreateTable());

            RemoteExecutionMode = 0;
        }

        //[RequiredArgument]
        //public InArgument<Dictionary<string, string>> MasterParams { get; set; }

        [RequiredArgument]
        [Description(@"Specify base uri of the script repository service")]
        public InArgument<string> ScriptRepositoryBaseURI { get; set; }

        [Description(@"Returns script execution standard output data")]
        public OutArgument<List<ScriptResult>> ScriptExecData { get; set; }

        [Description(@"Returns script execution standard error data")]
        public OutArgument<List<ScriptResult>> ScriptErrorData { get; set; }

        [Description(@"Returns script execution status true: Succeeded, False: Failed")]
        public OutArgument<bool> ScriptExecutionStatus { get; set; }

        [RequiredArgument]
        [DisplayName("Script Category")]
        public string ScriptCategory
        {
            get;
            set;

        }

        [RequiredArgument]
        [DisplayName("Script Category Name")]
        public string ScriptCategoryName
        {
            get;
            set;
        }

        //[RequiredArgument]
        ////[ReadOnly(true)]
        //[DisplayName("Script Sub-Category Id")]
        //public int ScriptSubCategoryId
        //{
        //    get;
        //    set;
        //}

        [RequiredArgument]
        //[ReadOnly(true)]
        [DisplayName("Script Id")]
        public int ScriptId
        {
            get;
            set;
        }

        [RequiredArgument]
        [DisplayName("Script Name")]
        [Description(@"Name of the script which is to be invoked")]
        public string ScriptName
        {
            get;
            set;
        }


        [EditorAttribute(typeof(Editors.ListSelectionEditor), typeof(PropertyValueEditor))]
        [Description(@"List of script parameters with the parameter direction")]
        public List<Parameter> Parameters
        {
            get;
            set;
        }

        [Description(@"List of Remote Servers separated by comma")]
        public InArgument<string> RemoteServerNames { get; set; }

        [Description(@"User Alias for Remote Execution")]
        public InArgument<string> RemoteUserName { get; set; }

        [Description(@"Enter IAP Encrypted User Password for Remote Execution")]
        public InArgument<string> RemotePassword { get; set; }

        [Description("Type 1 for PS, 2 for IAPNodes")]
        public InArgument<int> RemoteExecutionMode { get; set; }

        [Description(@"1 for HTTP, 2 for NetTCP, 0 when IAP node is not used")]
        public InArgument<int> IAPNodeTransport { get; set; }

        [Description("Port configured on servers, else use defaults")]
        public InArgument<int> IAPNodePort { get; set; }

        [Description(@"Workflow/Script scheduling options")]
        public InArgument<Infosys.WEM.AutomationActivity.Designers.ScheduleConfigurationSpec> ScheduleConfiguration { get; set; }

        protected override void Execute(NativeActivityContext context)
        {

            //check if stop requested, if show then throw exception
            if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

            /*TODO track script actions in scripttracking table Put logic to use context.track method and right custom trackingparticipants
            System.Guid workflowInstanceId = context.WorkflowInstanceId;
            string activityInstanceId = context.ActivityInstanceId;*/

            //Reading Variable values from the Workflow when variables where dynamically assigned at design time
            //Dictionary<string, string> paramsInputValue = new Dictionary<string, string>();

            bool success = true;
            List<ScriptResult> arrSuccessData = new List<ScriptResult>();
            List<ScriptResult> arrErrorData = new List<ScriptResult>();
            int count = 0;
            string[] arrServerName = null;
            int portHttp = 9001;
            int portNetTccp = 9002;

            Dictionary<string, string> dicWSManCreds = new Dictionary<string, string>();

            using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                context.ActivityInstanceId, ACTIVITY_NAME))
            {
                LogHandler.LogInfo(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId, ACTIVITY_NAME);

                List<Parameter> paramsInputValues = new List<Parameter>();
                string paramName = string.Empty;
                List<string> inParams = new List<string>();
                List<string> outParams = new List<string>();

                SetParameters(context, paramsInputValues, inParams, outParams);

                //Execute Script
                string serviceURI = context.GetValue(ScriptRepositoryBaseURI) + Designer.ApplicationConstants.SCRIPT_REPO_SERVICEINTERFACE;
                LogHandler.LogDebug("Script Repository URI {0}", LogHandler.Layer.Activity, serviceURI);

                ScriptIndentifier scriptToRun = new ScriptIndentifier();
                scriptToRun.ScriptId = ScriptId;
                scriptToRun.SubCategoryId = Convert.ToInt32(ScriptCategory);
                scriptToRun.Parameters = paramsInputValues;
                scriptToRun.WEMScriptServiceUrl = serviceURI;

                Designers.ScheduleConfigurationSpec spec = context.GetValue(ScheduleConfiguration);
                if (spec != null)
                {
                    if (spec.ModuleType == Designer.ModuleType.Script)
                    {
                        if (spec.RemoteExecutionMode == (int)Designers.RemoteExecMode.ScheduleOnCluster && (spec.CategoryId.ToString() != ScriptCategory))
                            throw new System.Exception("Category selected in Scheduler configuration should be same as workflow category");

                        if (spec.RemoteExecutionMode == (int)Designers.RemoteExecMode.ScheduleOnCluster)
                        {
                            scriptToRun.ExecutionMode = ExecutionModeType.ScheduledOnIAPCluster;
                            scriptToRun.ScheduleOnClusters = spec.ScheduleClusterName;
                        }
                        else if (spec.RemoteExecutionMode == (int)Designers.RemoteExecMode.ScheduleOnIAPnode)
                        {
                            scriptToRun.ExecutionMode = ExecutionModeType.ScheduledOnIAPNode;
                            if (spec.RemoteServerNames != null)
                                scriptToRun.RemoteServerNames = TranslateListToString(spec.RemoteServerNames);
                        }
                        if (spec.SchedulePattern == Designers.SchedulePatterns.ScheduleNow)
                            scriptToRun.ScheduledPattern = ScheduledPatternType.ScheduleNow;
                        else if (spec.SchedulePattern == Designers.SchedulePatterns.ScheduleWithRecurrence)
                            scriptToRun.ScheduledPattern = ScheduledPatternType.ScheduleWithRecurrence;
                        scriptToRun.ScheduleEndDateTime = Convert.ToDateTime(spec.ScheduleEndDate);
                        scriptToRun.ScheduleOccurences = spec.ScheduleOcurrences;
                        scriptToRun.SchedulePriority = spec.SchedulePriority;
                        scriptToRun.ScheduleStartDateTime = spec.ScheduleStartDateTime;
                        if (spec.ScheduleStopCriteria == Designers.ScheduleStopCriteria.EndDate)
                            scriptToRun.ScheduleStopCriteria = ScheduleStopCriteriaType.EndDate;
                        else if (spec.ScheduleStopCriteria == Designers.ScheduleStopCriteria.NoEndDate)
                            scriptToRun.ScheduleStopCriteria = ScheduleStopCriteriaType.NoEndDate;
                        else if (spec.ScheduleStopCriteria == Designers.ScheduleStopCriteria.OccurenceCount)
                            scriptToRun.ScheduleStopCriteria = ScheduleStopCriteriaType.OccurenceCount;

                        if (context.GetValue(IAPNodeTransport) == 2)
                        {
                            scriptToRun.IapNodeTransport = IapNodeTransportType.Nettcp;
                            if (context.GetValue(IAPNodePort) != 0)
                            {
                                scriptToRun.IapNodeNetTcpPort = context.GetValue(IAPNodePort);
                            }
                            else
                            {
                                scriptToRun.IapNodeNetTcpPort = portNetTccp;
                            }
                        }
                        else
                        {
                            scriptToRun.IapNodeTransport = IapNodeTransportType.Http;
                            if (context.GetValue(IAPNodePort) != 0)
                            {
                                scriptToRun.IapNodeHttpPort = context.GetValue(IAPNodePort);
                            }
                            else
                            {
                                scriptToRun.IapNodeHttpPort = portHttp;
                            }
                        }
                    }
                    else
                    {
                        throw new System.Exception("Scheduled Configuration specified is valid only for Script");
                    }
                }

                // Set remote parameters
                if (!string.IsNullOrEmpty(context.GetValue(RemoteServerNames)))
                {
                    arrServerName = context.GetValue(RemoteServerNames).Split(',');
                    foreach (string server in arrServerName)
                    {
                        //Call method to delegate credentials to remote machine
                        if (!CheckRemoteSettings(server))
                        {
                            string output = EnableRemoteSettings(server);
                            if (string.IsNullOrEmpty(output))
                            {
                                string message = "Credentials have been delegated to remote machine " + server + " for remote script execution.";
                                dicWSManCreds.Add(server, message);
                            }
                            else
                            {
                                dicWSManCreds.Add(server, output);
                            }
                        }
                    }
                    // Assign remote properties value
                    scriptToRun.RemoteServerNames = context.GetValue(RemoteServerNames);
                    if (context.GetValue(RemoteExecutionMode) == 2)
                        scriptToRun.RemoteExecutionMode = ScriptIndentifier.RemoteExecutionHost.IAPNodes;
                    else
                        scriptToRun.RemoteExecutionMode = ScriptIndentifier.RemoteExecutionHost.PS;
                }
                // Set Credentials
                if (!string.IsNullOrEmpty(context.GetValue(RemoteUserName)) && !string.IsNullOrEmpty(context.GetValue(RemotePassword)))
                {
                    scriptToRun.UserName = context.GetValue(RemoteUserName);
                    scriptToRun.Password = ConvertToSecureString(DecryptValue(context.GetValue(RemotePassword)));
                }

                LogHandler.LogInfo(InformationMessages.ACTIVITY_INVOKESCRIPT_BEGINSCRIPTINVOKE, LogHandler.Layer.Activity,
                    context.ActivityInstanceId, ScriptName, ScriptId, ScriptCategoryName);

                List<ExecutionResult> consolidatedResult = ScriptExecutionManager.Execute(scriptToRun);
                ScriptResult scriptResult = null;
                string localMachineName = System.Environment.MachineName;

                //Manipulate output
                foreach (ExecutionResult result in consolidatedResult)
                {
                    scriptResult = new ScriptResult();
                    string value = "";
                    string succMessage = "";

                    //Find machine value in dictionary and add WSManCred result (if any)
                    if (dicWSManCreds != null && dicWSManCreds.Count > 0)
                        value = dicWSManCreds[result.ComputerName];
                    //value = dicWSManCreds[arrServerName[count]];

                    //Process success Message List
                    if (result.IsSuccess)
                    {
                        success = true;

                        if (result.ScheduledRequestIds != null)
                        {
                            if (result.ScheduledRequestIds.Count > 0)
                            {
                                foreach (string id in result.ScheduledRequestIds)
                                {
                                    succMessage = "The script " + ScriptName + " (Id: " + scriptToRun.ScriptId + ") has been scheduled. The corresponding scheduled request id is " + id;
                                    scriptResult.Message = succMessage;
                                    if (!string.IsNullOrEmpty(result.ComputerName))
                                        scriptResult.MachineName = result.ComputerName;
                                    else
                                        scriptResult.MachineName = localMachineName;
                                    arrSuccessData.Add(scriptResult);
                                }
                            }
                        }
                        else
                        {
                            // Append WSManCred result value (if any)
                            if (string.IsNullOrEmpty(value))
                                scriptResult.Message = result.SuccessMessage;
                            else
                                scriptResult.Message = result.SuccessMessage + Environment.NewLine + value;

                            if (!string.IsNullOrEmpty(context.GetValue(RemoteServerNames)) && arrServerName != null)
                                scriptResult.MachineName = result.ComputerName;
                            else
                                scriptResult.MachineName = localMachineName;
                            arrSuccessData.Add(scriptResult);
                        }
                    }
                    //Process Error Message List
                    else if (!result.IsSuccess)
                    {
                        success = false;
                        //// Append WSManCred result value (if any)
                        //if (string.IsNullOrEmpty(value))
                        //{
                        if (!string.IsNullOrEmpty(result.ErrorMessage))
                            scriptResult.Message = result.ErrorMessage;

                        if (!string.IsNullOrEmpty(result.ComputerName))
                            scriptResult.MachineName = result.ComputerName;
                        else
                            scriptResult.MachineName = localMachineName;
                        arrErrorData.Add(scriptResult);

                    }
                    count = count + 1;
                }

                LogHandler.LogInfo(InformationMessages.ACTIVITY_INVOKESCRIPT_ENDSCRIPTINVOKE, LogHandler.Layer.Activity,
                    context.ActivityInstanceId, ScriptName, ScriptId, ScriptCategoryName);
                //Mapping Output parameters value directly at runtime to put variables
                foreach (string param in outParams)
                {

                    var variableOutProp = context.DataContext.GetProperties()[param];
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_INVOKESCRIPT_PARAMETERS_OUT, LogHandler.Layer.Activity,
                                 context.ActivityInstanceId, paramName, "");//todo set out values
                    variableOutProp.SetValue(context.DataContext, "");
                }

                ScriptExecData.Set(context, arrSuccessData);
                LogHandler.LogInfo(InformationMessages.ACTIVITY_EXIT_SUCCESS, LogHandler.Layer.Activity,
                    context.ActivityInstanceId, ACTIVITY_NAME);
                ScriptErrorData.Set(context, arrErrorData);

                LogHandler.LogError(InformationMessages.ACTIVITY_EXIT_FAILURE, LogHandler.Layer.Activity,
                    context.ActivityInstanceId, arrErrorData, ACTIVITY_NAME);

                ScriptExecutionStatus.Set(context, success);

            }
        }

        private string TranslateListToString(List<string> serverNames)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string name in serverNames)
            {
                sb.Append(name);
                sb.Append(",");
            }
            string value = sb.ToString();

            string final = value.Substring(0, value.Length - 1);
            return final;
        }

        private string SetParameters(NativeActivityContext context, List<Parameter> paramsInputValues, List<string> inParams, List<string> outParams)
        {
            string paramName = "";
            if (Parameters != null)
            {
                foreach (Parameter param in Parameters)
                {
                    string paramsFormatted = param.ParameterName.
                    Replace(Designer.ApplicationConstants.PARAMDIRECTION_IN, "").
                    Replace(Designer.ApplicationConstants.PARAMDIRECTION_OUT, "").
                    Replace(Designer.ApplicationConstants.PARAMDIRECTION_INANDOUT, "").Trim();
                    string scripNameFormatted = ScriptName.Replace(" ", "").Trim();
                    paramName = string.Format(Designer.ApplicationConstants.VARIABLENAMESCRIPT_FORMAT, scripNameFormatted, paramsFormatted);
                    string paramNameWithSpace = paramName;
                    if (paramName.Contains(" "))
                        paramNameWithSpace = paramName.Replace(" ", "___");

                    if (param.ParameterName.Contains(Designer.ApplicationConstants.PARAMDIRECTION_IN))
                    {

                        inParams.Add(paramName);

                        var variableProp = context.DataContext.GetProperties()[paramNameWithSpace];
                        var variableValue = variableProp != null ? variableProp.GetValue(context.DataContext) as string : "";
                        Parameter paramsInputValue = null;

                        Regex rgx = new Regex(@"[^\s""]+|""[^""]*""", RegexOptions.IgnoreCase);
                        MatchCollection matches = rgx.Matches(variableValue);

                        // If multiple command arguments
                        if (paramsFormatted.ToLower().Equals("arguments") && variableValue.Contains("\""))
                        {
                            //string[] paramList = variableValue.Split(new string[] { "\"\"" }, StringSplitOptions.None);
                            //if (paramList != null && paramList.Length > 0)
                            //{
                            //int paramCount = 1;
                            foreach (Match match in matches)
                            {
                                paramsInputValue = new Parameter();
                                //paramsInputValue.ParameterName = paramsFormatted + paramCount.ToString();
                                paramsInputValue.ParameterValue = match.Value.Replace("\"", "");
                                paramsInputValue.ParameterName = "";
                                paramsInputValue.IsSecret = param.IsSecret;
                                paramsInputValue.allowedValues = null;
                                paramsInputValue.IsPaired = false;
                                paramsInputValue.DataType = "";
                                paramsInputValues.Add(paramsInputValue);
                                //paramCount = paramCount + 1;
                            }
                            //}
                            // }
                        }
                        else
                        {
                            paramsInputValue = new Parameter();
                            paramsInputValue.ParameterName = paramsFormatted;// paramName.Replace(removeParam, "");
                            paramsInputValue.ParameterValue = variableValue;
                            paramsInputValue.IsSecret = param.IsSecret;
                            paramsInputValue.allowedValues = null;
                            paramsInputValue.IsPaired = false;
                            paramsInputValue.DataType = "";
                            paramsInputValues.Add(paramsInputValue);
                        }
                        LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                            context.ActivityInstanceId, paramName, Designer.ApplicationConstants.PARAMDIRECTION_IN, variableValue, ACTIVITY_NAME);

                    }
                    else if (param.ParameterName.Contains(Designer.ApplicationConstants.PARAMDIRECTION_OUT))
                    {

                        outParams.Add(paramName);
                        LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                            context.ActivityInstanceId, paramName, Designer.ApplicationConstants.PARAMDIRECTION_OUT, "", ACTIVITY_NAME);
                    }
                    else if (param.ParameterName.Contains(Designer.ApplicationConstants.PARAMDIRECTION_INANDOUT))
                    {

                        var variableProp = context.DataContext.GetProperties()[paramNameWithSpace];
                        var variableValue = variableProp != null ? variableProp.GetValue(context.DataContext) as string : "";
                        Parameter paramsInputValue = new Parameter();
                        paramsInputValue.ParameterName = paramsFormatted;
                        paramsInputValue.ParameterValue = variableValue;
                        paramsInputValue.IsSecret = param.IsSecret;
                        paramsInputValues.Add(paramsInputValue);
                        LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                            context.ActivityInstanceId, paramName, Designer.ApplicationConstants.PARAMDIRECTION_INANDOUT, variableValue, ACTIVITY_NAME);
                    }
                }
            }
            return paramName;
        }

        /// <summary>
        /// This method is used to convert plain string to secure string.
        /// </summary>
        /// <param name="password">string to convert</param>
        /// <returns>secured string</returns>
        private static SecureString ConvertToSecureString(string password)
        {
            SecureString securePassword = new SecureString();
            foreach (char c in password)
            {
                securePassword.AppendChar(c);
            }
            return securePassword;
        }

        /// <summary>
        /// This method is used to check if machine is configured to delegate credentials to remote machine.
        /// </summary>
        /// <param name="remoteMachineName">Name of remote machine</param>
        /// <returns>true if machine is configured to delegate credentials to remote machine</returns>
        private bool CheckRemoteSettings(string remoteMachineName)
        {
            Collection<PSObject> psOutput = null;
            StringBuilder result = new StringBuilder();
            bool enabled = false;
            using (Runspace runSpace = RunspaceFactory.CreateRunspace())
            {
                runSpace.Open();
                using (Pipeline pipeline = runSpace.CreatePipeline())
                {
                    PowerShell ps = PowerShell.Create();
                    ps.Runspace = runSpace;
                    ps.AddCommand("get-wsmancredssp");
                    psOutput = ps.Invoke();

                    if (ps.Streams.Error.Count == 0)
                    {
                        foreach (PSObject psObject in psOutput)
                        {
                            if (psObject != null)
                            {
                                result.AppendLine(psObject.BaseObject.ToString());
                            }
                        }
                    }
                    else
                    {
                        foreach (var errorRecord in ps.Streams.Error)
                        {
                            result.AppendLine(errorRecord.ToString());
                        }
                    }
                    if (result.ToString().Contains(remoteMachineName))
                        enabled = true;
                }
            }
            return enabled;
        }

        /// <summary>
        /// This method is used to delegate credentials to remote machine.
        /// </summary>
        /// <param name="remoteMachineName">Name of remote machine</param>
        /// <returns>Blank in case no error occurred</returns>
        private string EnableRemoteSettings(string remoteMachineName)
        {
            string enabled = "";
            Collection<PSObject> psOutput = null;
            StringBuilder result = new StringBuilder();
            using (Runspace runSpace = RunspaceFactory.CreateRunspace())
            {
                runSpace.Open();
                using (Pipeline pipeline = runSpace.CreatePipeline())
                {
                    PowerShell ps = PowerShell.Create();
                    ps.Runspace = runSpace;
                    ps.AddCommand("enable-wsmancredssp");
                    ps.AddParameter("role", "client");
                    ps.AddParameter("DelegateComputer", remoteMachineName);
                    ps.AddParameter("Force");
                    psOutput = ps.Invoke();

                    if (ps.Streams.Error.Count > 0)
                    {
                        foreach (var errorRecord in ps.Streams.Error)
                        {
                            result.AppendLine(errorRecord.ToString());
                        }

                        enabled = result.ToString();
                    }

                }
            }
            return enabled;
        }

        /// <summary>
        /// This methos is used to convert IAP encrypted value into plain Text
        /// </summary>
        /// <param name="encrypt">encrypted value which is to be decripted</param>
        /// <returns>decrypted value</returns>
        private string DecryptValue(string encrypt)
        {
            return SecurePayload.UnSecure(encrypt, "IAP2GO_SEC!URE");
        }
    }
    /// <summary>
    /// This class contains properties to store the result of script execution and will be used
    /// as OutArgument for InvokeScript properties.
    /// </summary>
    public class ScriptResult
    {
        public string MachineName { get; set; }
        public string Message { get; set; }
    }

}

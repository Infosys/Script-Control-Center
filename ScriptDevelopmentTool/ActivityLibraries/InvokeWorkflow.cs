/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;

using System.ComponentModel;
using System.Activities.Presentation.Metadata;
using System.Activities.Presentation.PropertyEditing;
using Infosys.WEM.Infrastructure.Common;
using Designer = Infosys.WEM.AutomationActivity.Designers;

using System.Text.RegularExpressions;
using System.Activities;
using Infosys.WEM.WorkflowExecutionLibrary.Entity;
using Infosys.WEM.WorkflowExecutionLibrary;
using System.Text;


namespace Infosys.WEM.AutomationActivity.Libraries
{
    [Designer(typeof(Designer.InvokeWorkflow))]
    public sealed class InvokeWorkflow : NativeActivity
    {
        private const string ACTIVITY_NAME = "InvokeWorkflow";

        public InvokeWorkflow()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }

        [RequiredArgument]
        [Description(@"Specify base uri of the workflow repository service")]
        public InArgument<string> WorkflowRepositoryBaseURI { get; set; }

        [Description(@"Returns workflow execution standard output data")]
        public OutArgument<List<WorkflowResult>> WorkflowExecData { get; set; }

        [Description(@"Returns workflow execution standard error data")]
        public OutArgument<List<WorkflowResult>> WorkflowErrorData { get; set; }

        [Description(@"Returns workflow execution status true: Succeeded, False: Failed")]
        public OutArgument<bool> WorkflowExecutionStatus { get; set; }

        [RequiredArgument]
        [DisplayName("Workflow Category")]
        public string WorkflowCategory
        {
            get;
            set;

        }

        [RequiredArgument]
        [DisplayName("Workflow Category Name")]
        public string WorkflowCategoryName
        {
            get;
            set;
        }

        [RequiredArgument]
        [DisplayName("Workflow Id")]
        public string WorkflowId
        {
            get;
            set;
        }

        [RequiredArgument]
        [DisplayName("Workflow Name")]
        [Description(@"Name of the Workflow which is to be invoked")]
        public string WorkflowName
        {
            get;
            set;
        }

        [Description(@"List of Remote Servers separated by comma")]
        public InArgument<string> RemoteServerNames { get; set; }

        [Description("Type 1 for PS, 2 for IAPNodes")]
        public InArgument<int> RemoteExecutionMode { get; set; }

        [Description(@"1 for HTTP, 2 for NetTCP, 0 when IAP node is not used")]
        public InArgument<int> IAPNodeTransport { get; set; }

        [Description("Port configured on servers, else leave blank")]
        public InArgument<int> IAPNodePort { get; set; }

        [EditorAttribute(typeof(Editors.ListSelectionEditor), typeof(PropertyValueEditor))]
        [Description(@"List of Workflow parameters with the parameter direction")]
        public List<Designer.WorkflowParam> Parameters
        {
            get;
            set;
        }

        [Description(@"Workflow/Script scheduling options")]
        public InArgument<Infosys.WEM.AutomationActivity.Designers.ScheduleConfigurationSpec> ScheduleConfiguration { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            //check if stop requested, if show then throw exception
            if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

            bool success = false;
            List<WorkflowResult> arrSuccessData = new List<WorkflowResult>();
            List<WorkflowResult> arrErrorData = new List<WorkflowResult>();
            bool wfAllParamsPassed = true;
            ExecutionResult result = null;
            int count = 0;

            {
                Dictionary<string, string> dicWSManCreds = new Dictionary<string, string>();
                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ACTIVITY_NAME))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId, ACTIVITY_NAME);

                    List<ExecutionResult> results = null;
                    WorkflowIndentifier2 wfToRun2 = null;
                    WorkflowIndentifier wfToRun = null;

                    List<Infosys.WEM.WorkflowExecutionLibrary.Entity.Parameter> paramsInputValues = new List<Infosys.WEM.WorkflowExecutionLibrary.Entity.Parameter>();
                    string paramName = string.Empty;
                    List<string> inParams = new List<string>();
                    List<string> outParams = new List<string>();
                    if (Parameters != null)
                    {
                        foreach (Designer.WorkflowParam param in Parameters)
                        {
                            string paramsFormatted = param.ParameterName.
                            Replace(Designer.ApplicationConstants.PARAMDIRECTION_IN, "").
                            Replace(Designer.ApplicationConstants.PARAMDIRECTION_OUT, "").
                            Replace(Designer.ApplicationConstants.PARAMDIRECTION_INANDOUT, "").Trim();
                            string scripNameFormatted = WorkflowName.Replace(" ", "").Trim();
                            paramName = string.Format(Designer.ApplicationConstants.VARIABLENAMEWF_FORMAT, scripNameFormatted, paramsFormatted);
                            string paramNameWithSpace = paramName;
                            if (paramName.Contains(" "))
                                paramNameWithSpace = paramName.Replace(" ", "___");

                            if (param.ParameterName.Contains(Designer.ApplicationConstants.PARAMDIRECTION_IN))
                            {

                                inParams.Add(paramName);

                                var variableProp = context.DataContext.GetProperties()[paramNameWithSpace];
                                var variableValue = variableProp != null ? variableProp.GetValue(context.DataContext) as object : "";
                                Infosys.WEM.WorkflowExecutionLibrary.Entity.Parameter paramsInputValue = new Infosys.WEM.WorkflowExecutionLibrary.Entity.Parameter();

                                paramsInputValue.ParameterName = paramsFormatted;
                                paramsInputValue.ParameterValue = variableValue;
                                paramsInputValues.Add(paramsInputValue);

                                LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                                    context.ActivityInstanceId, paramName, Designer.ApplicationConstants.PARAMDIRECTION_IN, variableValue, ACTIVITY_NAME);

                            }
                            else if (param.ParameterName.Contains(Designer.ApplicationConstants.PARAMDIRECTION_OUT))
                            {
                                var variableProp = context.DataContext.GetProperties()[paramNameWithSpace];
                                if (variableProp == null)
                                {
                                    wfAllParamsPassed = false;
                                    break;
                                }
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
                                paramsInputValues.Add(paramsInputValue);
                                LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                                    context.ActivityInstanceId, paramName, Designer.ApplicationConstants.PARAMDIRECTION_INANDOUT, variableValue, ACTIVITY_NAME);
                            }
                        }
                    }
                    if (wfAllParamsPassed)
                    {
                        //Execute Workflow
                        string serviceURI = context.GetValue(WorkflowRepositoryBaseURI) + Designer.ApplicationConstants.WORKFLOW_SERVICEINTERFACE;
                        //string serviceURI = context.GetValue(WorkflowRepositoryBaseURI) + "/WEMService.svc";
                        LogHandler.LogDebug("Workflow Repository URI {0}", LogHandler.Layer.Activity, serviceURI);

                        LogHandler.LogInfo(InformationMessages.ACTIVITY_INVOKESCRIPT_BEGINSCRIPTINVOKE, LogHandler.Layer.Activity,
                            context.ActivityInstanceId, WorkflowName, WorkflowId, WorkflowCategoryName);

                        Designers.ScheduleConfigurationSpec spec = context.GetValue(ScheduleConfiguration);
                        if (spec != null)
                        {
                            if (spec.ModuleType == Designer.ModuleType.Workflow)
                            {
                                wfToRun2 = GetWorkflow(spec, paramsInputValues, serviceURI, Convert.ToInt32(context.GetValue(IAPNodePort)), Convert.ToInt32(context.GetValue(IAPNodeTransport)));
                                //wfToRun.WorkflowVersion = -1;
                                results = new WorkflowExecutionManager().Execute(wfToRun2);
                            }
                            else
                            {
                                throw new System.Exception("Scheduled Configuration specified is valid only for Workflow");
                            }
                        }
                        else
                        {
                            wfToRun = new WorkflowIndentifier();
                            wfToRun.WorkflowId = new Guid(WorkflowId);
                            wfToRun.CategoryId = Convert.ToInt32(WorkflowCategory);
                            wfToRun.Parameters = paramsInputValues;
                            wfToRun.WEMWorkflowServiceUrl = serviceURI;
                            result = new WorkflowExecutionManager().Execute(wfToRun);
                        }
                    }
                    else
                    {
                        // Throw exception if out argument not specified by user
                        throw new Exception("Please add all the Out parameters");
                    }

                    string localMachineName = System.Environment.MachineName;
                    WorkflowResult wfResult = null;

                    //Manipulate output
                    if (results != null)
                    {
                        foreach (ExecutionResult execResult in results)
                        {
                            wfResult = new WorkflowResult();
                            string value = "";
                            string succMessage = "";

                            //value = dicWSManCreds[arrServerName[count]];

                            //Process success Message List
                            if (execResult.IsSuccess)
                            {
                                success = true;
                                if (execResult.ScheduledRequestIds != null)
                                {
                                    if (execResult.ScheduledRequestIds.Count > 0)
                                    {
                                        foreach (string id in execResult.ScheduledRequestIds)
                                        {
                                            succMessage = "The workflow " + WorkflowName + " (Id: " + wfToRun2.WorkflowId + ") has been scheduled. The corresponding scheduled request id is" + id;
                                            wfResult.Message = succMessage;
                                            if (!string.IsNullOrEmpty(execResult.MachineName))
                                                wfResult.MachineName = execResult.MachineName;
                                            else
                                                wfResult.MachineName = localMachineName;
                                            arrSuccessData.Add(wfResult);
                                        }
                                    }
                                }
                            }
                            //Process Error Message List
                            else if (!execResult.IsSuccess)
                            {
                                success = false;
                                // Append WSManCred result value (if any)
                                if (string.IsNullOrEmpty(value))
                                {
                                    if (!string.IsNullOrEmpty(execResult.ErrorMessage))
                                        wfResult.Message = execResult.ErrorMessage;
                                    else
                                    {
                                        if (execResult.ScheduledRequestIds != null)
                                            if (execResult.ScheduledRequestIds.Count > 0)
                                            {
                                                wfResult.Message = execResult.ScheduledRequestIds[0];
                                            }
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(execResult.ErrorMessage))
                                        wfResult.Message = execResult.ErrorMessage + Environment.NewLine + value;
                                    else
                                    {
                                        if (execResult.ScheduledRequestIds != null)
                                            if (execResult.ScheduledRequestIds.Count > 0)
                                            {
                                                wfResult.Message = execResult.ScheduledRequestIds[0] + Environment.NewLine + value;
                                            }
                                    }                                    
                                }
                                if (!string.IsNullOrEmpty(execResult.MachineName))
                                    wfResult.MachineName = execResult.MachineName;
                                else
                                    wfResult.MachineName = localMachineName;
                                arrErrorData.Add(wfResult);
                            }
                            count = count + 1;
                        }
                    }


                    if (result != null)
                    {
                        WorkflowResult wfResult2 = new WorkflowResult();

                        wfResult2.MachineName = System.Environment.MachineName;

                        if (string.IsNullOrEmpty(result.ErrorMessage))
                        {
                            success = true;
                            wfResult2.Message = result.SuccessMessage;
                            arrSuccessData.Add(wfResult2);
                        }
                        else
                        {
                            wfResult2.Message = result.ErrorMessage;
                            arrErrorData.Add(wfResult2);
                        }

                        if (string.IsNullOrEmpty(result.ErrorMessage))
                        {
                            int count2 = 0;
                            //Mapping Output parameters value directly at runtime to put variables
                            foreach (string param in outParams)
                            {
                                var variableOutProp = context.DataContext.GetProperties()[param];
                                LogHandler.LogInfo(InformationMessages.ACTIVITY_INVOKESCRIPT_PARAMETERS_OUT, LogHandler.Layer.Activity,
                                             context.ActivityInstanceId, paramName, result.Output[count2].ParameterValue);//todo set out values
                                variableOutProp.SetValue(context.DataContext, result.Output[count2].ParameterValue);
                                count2 = count2 + 1;
                            }
                        }
                    }

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_INVOKESCRIPT_ENDSCRIPTINVOKE, LogHandler.Layer.Activity,
                       context.ActivityInstanceId, WorkflowName, WorkflowId, WorkflowCategoryName);

                    WorkflowExecData.Set(context, arrSuccessData);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_EXIT_SUCCESS, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ACTIVITY_NAME);
                    WorkflowErrorData.Set(context, arrErrorData);

                    LogHandler.LogError(InformationMessages.ACTIVITY_EXIT_FAILURE, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, arrErrorData, ACTIVITY_NAME);

                    WorkflowExecutionStatus.Set(context, success);
                }
            }
        }

        private WorkflowIndentifier2 GetWorkflow(Designers.ScheduleConfigurationSpec spec, List<Infosys.WEM.WorkflowExecutionLibrary.Entity.Parameter> paramsInputValues, string serviceURI, int port, int transportMode)
        {
            WorkflowIndentifier2 wfToRun = new WorkflowIndentifier2();
            wfToRun.WorkflowId = new Guid(WorkflowId);
            wfToRun.CategoryId = Convert.ToInt32(WorkflowCategory);
            wfToRun.Parameters = paramsInputValues;
            wfToRun.WEMWorkflowServiceUrl = serviceURI;


            int portHttp = 9001;
            int portNetTccp = 9002;

            if (spec.RemoteExecutionMode == (int)Designers.RemoteExecMode.ScheduleOnCluster && (spec.CategoryId.ToString() != WorkflowCategory))
                throw new System.Exception("Category selected in Scheduler configuration should be same as workflow category");

            if (spec.RemoteExecutionMode == (int)Designers.RemoteExecMode.ScheduleOnCluster)
            {
                wfToRun.ExecutionMode = ExecutionModeType.ScheduledOnIAPCluster;
                wfToRun.ScheduleOnClusters = spec.ScheduleClusterName;
            }
            else if (spec.RemoteExecutionMode == (int)Designers.RemoteExecMode.ScheduleOnIAPnode)
            {
                wfToRun.ExecutionMode = ExecutionModeType.ScheduledOnIAPNode;
                if (spec.RemoteServerNames != null)
                    wfToRun.RemoteServerNames = TranslateListToString(spec.RemoteServerNames);
            }
            if (spec.SchedulePattern == Designers.SchedulePatterns.ScheduleNow)
                wfToRun.ScheduledPattern = ScheduledPatternType.ScheduleNow;
            else if (spec.SchedulePattern == Designers.SchedulePatterns.ScheduleWithRecurrence)
                wfToRun.ScheduledPattern = ScheduledPatternType.ScheduleWithRecurrence;
            wfToRun.ScheduleEndDateTime = Convert.ToDateTime(spec.ScheduleEndDate);
            wfToRun.ScheduleOccurences = spec.ScheduleOcurrences;

            wfToRun.SchedulePriority = spec.SchedulePriority;
            wfToRun.ScheduleStartDateTime = spec.ScheduleStartDateTime;
            if (spec.ScheduleStopCriteria == Designers.ScheduleStopCriteria.EndDate)
                wfToRun.ScheduleStopCriteria = ScheduleStopCriteriaType.EndDate;
            else if (spec.ScheduleStopCriteria == Designers.ScheduleStopCriteria.NoEndDate)
                wfToRun.ScheduleStopCriteria = ScheduleStopCriteriaType.NoEndDate;
            else if (spec.ScheduleStopCriteria == Designers.ScheduleStopCriteria.OccurenceCount)
                wfToRun.ScheduleStopCriteria = ScheduleStopCriteriaType.OccurenceCount;

            if (transportMode == 2)
            {
                wfToRun.IapNodeTransport = IapNodeTransportType.Nettcp;
                if (port != 0)
                {
                    wfToRun.IapNodeNetTcpPort = port;
                }
                else
                {
                    wfToRun.IapNodeNetTcpPort = portNetTccp;
                }
            }
            else
            {
                wfToRun.IapNodeTransport = IapNodeTransportType.Http;
                if (port != 0)
                {
                    wfToRun.IapNodeHttpPort = port;
                }
                else
                {
                    wfToRun.IapNodeHttpPort = portHttp;
                }
            }

            return wfToRun;
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
    }
    /// <summary>
    /// This class contains properties to store the result of script execution and will be used
    /// as OutArgument for InvokeScript properties.
    /// </summary>
    public class WorkflowResult
    {
        public string MachineName { get; set; }
        public string Message { get; set; }
    }
}

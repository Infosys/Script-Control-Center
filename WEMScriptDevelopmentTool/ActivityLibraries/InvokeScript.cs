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


        }

        //[RequiredArgument]
        //public InArgument<Dictionary<string, string>> MasterParams { get; set; }

        [RequiredArgument]
        [Description(@"Specify base uri of the script repository service")]
        public InArgument<string> ScriptRepositoryBaseURI { get; set; }

        public OutArgument<string> ScriptExecData { get; set; }


        [RequiredArgument]
        [DisplayName("Script Category")]
        public string ScriptCategory
        {
            get;
            set;

        }

        [RequiredArgument]
        [DisplayName("Script Sub-Category")]
        public string ScriptSubCategory
        {
            get;
            set;
        }

        [RequiredArgument]
        //[ReadOnly(true)]
        [DisplayName("Script Sub-Category Id")]
        public int ScriptSubCategoryId
        {
            get;
            set;
        }

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
        public List<string> Parameters
        {
            get;
            set;
        }

        protected override void Execute(NativeActivityContext context)
        {

            /*TODO track script actions in scripttracking table Put logic to use context.track method and right custom trackingparticipants
            System.Guid workflowInstanceId = context.WorkflowInstanceId;
            string activityInstanceId = context.ActivityInstanceId;*/

            //Reading Variable values from the Workflow when variables where dynamically assigned at design time
            //Dictionary<string, string> paramsInputValue = new Dictionary<string, string>();


            using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                context.ActivityInstanceId, ACTIVITY_NAME))
            {
                LogHandler.LogInfo(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId, ACTIVITY_NAME);

                List<Parameter> paramsInputValues = new List<Parameter>();
                string paramName = string.Empty;
                List<string> inParams = new List<string>();
                List<string> outParams = new List<string>();
                if (Parameters != null)
                {
                    foreach (string param in Parameters)
                    {
                        string paramsFormatted = param.
                        Replace(Designer.ApplicationConstants.PARAMDIRECTION_IN, "").
                        Replace(Designer.ApplicationConstants.PARAMDIRECTION_OUT, "").
                        Replace(Designer.ApplicationConstants.PARAMDIRECTION_INANDOUT, "").Trim();
                        string scripNameFormatted = ScriptName.Replace(" ", "").Trim();
                        paramName = string.Format(Designer.ApplicationConstants.VARIABLENAME_FORMAT, scripNameFormatted, paramsFormatted);

                        if (param.Contains(Designer.ApplicationConstants.PARAMDIRECTION_IN))
                        {

                            inParams.Add(paramName);
                            var variableProp = context.DataContext.GetProperties()[paramName];
                            var variableValue = variableProp != null ? variableProp.GetValue(context.DataContext) as string : "";
                            Parameter paramsInputValue = new Parameter();
                            paramsInputValue.ParameterName = paramName;
                            paramsInputValue.ParameterValue = variableValue;

                            paramsInputValues.Add(paramsInputValue);
                            LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, paramName, Designer.ApplicationConstants.PARAMDIRECTION_IN, variableValue, ACTIVITY_NAME);

                        }
                        else if (param.Contains(Designer.ApplicationConstants.PARAMDIRECTION_OUT))
                        {

                            outParams.Add(paramName);
                            LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, paramName, Designer.ApplicationConstants.PARAMDIRECTION_OUT, "", ACTIVITY_NAME);
                        }
                        else if (param.Contains(Designer.ApplicationConstants.PARAMDIRECTION_INANDOUT))
                        {

                            var variableProp = context.DataContext.GetProperties()[paramName];
                            var variableValue = variableProp != null ? variableProp.GetValue(context.DataContext) as string : "";
                            Parameter paramsInputValue = new Parameter();
                            paramsInputValue.ParameterName = paramName;
                            paramsInputValue.ParameterValue = variableValue;
                            paramsInputValues.Add(paramsInputValue);
                            LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, paramName, Designer.ApplicationConstants.PARAMDIRECTION_INANDOUT, variableValue, ACTIVITY_NAME);
                        }
                        //paramName = ScriptName + "_" + param;
                        //Variable<string> mySimpleVar = new Variable<string>
                        //{
                        //    Name = paramName
                        //};
                    }
                }

                //Execute Script
                string serviceURI = context.GetValue(ScriptRepositoryBaseURI) + Designer.ApplicationConstants.SCRIPT_REPO_SERVICEINTERFACE;
                LogHandler.LogDebug("Script Repository URI {0}", LogHandler.Layer.Activity, serviceURI);

                ScriptIndentifier scriptToRun = new ScriptIndentifier();
                scriptToRun.ScriptId = ScriptId;
                scriptToRun.SubCategoryId = ScriptSubCategoryId;
                scriptToRun.Parameters = paramsInputValues;
                scriptToRun.WEMScriptServiceUrl = serviceURI;
                LogHandler.LogInfo(InformationMessages.ACTIVITY_INVOKESCRIPT_BEGINSCRIPTINVOKE, LogHandler.Layer.Activity,
                    context.ActivityInstanceId, ScriptName, ScriptId, ScriptSubCategoryId);

                ExecutionResult result = ScriptExecutionManager.Execute(scriptToRun);

                LogHandler.LogInfo(InformationMessages.ACTIVITY_INVOKESCRIPT_ENDSCRIPTINVOKE, LogHandler.Layer.Activity,
                    context.ActivityInstanceId, ScriptName, ScriptId, ScriptSubCategoryId);
                //Mapping Output parameters value directly at runtime to put variables
                foreach (string param in outParams)
                {

                    var variableOutProp = context.DataContext.GetProperties()[param];
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_INVOKESCRIPT_PARAMETERS_OUT, LogHandler.Layer.Activity,
                                 context.ActivityInstanceId, paramName, "");//todo set out values
                    variableOutProp.SetValue(context.DataContext, "");

                }
                if (result.IsSuccess)
                {
                    ScriptExecData.Set(context, result.SuccessMessage);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_EXIT_SUCCESS, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ACTIVITY_NAME);
                }
                else
                {
                    ScriptExecData.Set(context, result.ErrorMessage);
                    LogHandler.LogError(InformationMessages.ACTIVITY_EXIT_FAILURE, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, result.ErrorMessage, ACTIVITY_NAME);
                }


            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Infosys.ATR.WinUIAutomationRuntimeWrapper;
using Infosys.WEM.Infrastructure.Common;
using Designer = Infosys.WEM.AutomationActivity.Designers;
using ATRWrapperControls = Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls;

namespace Infosys.WEM.AutomationActivity.Libraries
{

    public sealed class InitializeAutomationAPI : NativeActivity
    {
        //[RequiredArgument]
        public InArgument<string> AutomationConfigFilePath { get; set; }
        public InArgument<bool> IsLaunchApp { get; set; }
        public OutArgument<AutomationFacade> AutomationObject { get; set; }

        private string configFilePath;
        private bool isLaunchApp;
        private const string ACTIVITY_NAME = "InitializeAutomationAPI";

        protected override void Execute(NativeActivityContext context)
        {

            try
            {
                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId,ACTIVITY_NAME))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId,ACTIVITY_NAME);

                    configFilePath = context.GetValue(this.AutomationConfigFilePath);
                    isLaunchApp = context.GetValue(this.IsLaunchApp);

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "AutomationConfigFilePath", Designer.ApplicationConstants.PARAMDIRECTION_IN,
                               configFilePath,ACTIVITY_NAME);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "IsLaunchApp", Designer.ApplicationConstants.PARAMDIRECTION_IN, isLaunchApp,ACTIVITY_NAME);

                    AutomationFacade target = null;

                    if (!string.IsNullOrEmpty(configFilePath))
                        target = new AutomationFacade(configFilePath, isLaunchApp);
                    else
                        target = new AutomationFacade();
                    AutomationObject.Set(context, target);

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "AutomationObject", Designer.ApplicationConstants.PARAMDIRECTION_OUT,
                                AutomationObject.ToString(),ACTIVITY_NAME);
                }

                LogHandler.LogInfo(InformationMessages.ACTIVITY_EXIT_SUCCESS, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ACTIVITY_NAME);
            }
            catch (Exception ex)
            {
                LogHandler.LogError(InformationMessages.ACTIVITY_EXIT_FAILURE, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ex.Message, ACTIVITY_NAME);
                throw ex;
            }


        }
    }


}

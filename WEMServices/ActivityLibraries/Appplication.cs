using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Infosys.ATR.WinUIAutomationRuntimeWrapper;
using System.Workflow.ComponentModel.Design;
using Infosys.WEM.Infrastructure.Common;
using Designer = Infosys.WEM.AutomationActivity.Designers;
using ATRWrapperControls = Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls;

namespace Infosys.WEM.AutomationActivity.Libraries.Appplication
{
    public sealed class KeyPress : NativeActivity
    {
        [RequiredArgument]
        public InArgument<Application> ApplicationObj { get; set; }
        public InArgument<string> SendToText { get; set; }
        [System.ComponentModel.DisplayName("ModifierKey: 0 for shift, 1 for ctrl, 2 for meta, 3 for alt, 4 for caps, 5 for enter, 6 for tab.\n NB- in case of CAPS/ENTER, etc first call with text= blank or null and modifiers= 4 or 5, \nthen again call with the required text as neeeded and modifier key = -1.\n")]
        public InArgument<int> ModifierKey { get; set; }
        protected override void Execute(NativeActivityContext context)
        {

            try
            {
                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ActivityEvents.KEYPRESS, ActivityControls.TEXTBOX))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId,
                        ActivityEvents.KEYPRESS, ActivityControls.TEXTBOX);


                    Application app = context.GetValue(ApplicationObj);
                    string sendText = context.GetValue(SendToText);
                    int modifierkey = context.GetValue(ModifierKey);
                    string appName = app != null ? app.Name : "";

                    //in param
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "ControlObj", Designer.ApplicationConstants.PARAMDIRECTION_IN, appName);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "SendToText", Designer.ApplicationConstants.PARAMDIRECTION_IN, sendText);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "ModifierKey", Designer.ApplicationConstants.PARAMDIRECTION_IN, modifierkey);

                    app.KeyPress(sendText, modifierkey);

                }

                LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_EXIT_SUCCESS, LogHandler.Layer.Activity,
                        context.ActivityInstanceId);
            }
            catch (Exception ex)
            {
                LogHandler.LogError(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_EXIT_FAILURE, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ex.Message);
                throw ex;
            }

        }
    }
}

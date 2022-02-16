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
using Infosys.ATR.WinUIAutomationRuntimeWrapper;
using Infosys.WEM.Infrastructure.Common;
using Designer = Infosys.WEM.AutomationActivity.Designers;

namespace Infosys.WEM.AutomationActivity.Libraries
{
    public sealed class FindControl : NativeActivity
    {
        [RequiredArgument]
        public InArgument<string> ControlCanonicalPath { get; set; }
        [RequiredArgument]
        public InArgument<AutomationFacade> AutomationObject { get; set; }


        public OutArgument<Control> ControlObj { get; set; }

        private string controlPath;
        private AutomationFacade automationObject;
        private const string ACTIVITY_NAME = "FindControl";

        protected override void Execute(NativeActivityContext context)
        {

            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ACTIVITY_NAME))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId, ACTIVITY_NAME);

                    controlPath = context.GetValue(this.ControlCanonicalPath);
                    automationObject = context.GetValue(this.AutomationObject);

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "ControlCanonicalPath", Designer.ApplicationConstants.PARAMDIRECTION_IN, controlPath, ACTIVITY_NAME);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "AutomationObject", Designer.ApplicationConstants.PARAMDIRECTION_IN, automationObject.ToString(), ACTIVITY_NAME);//TODO


                    Control control = automationObject.FindControl(controlPath);
                    ControlObj.Set(context, control);

                    string controlName = control != null ? control.Name : "";

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "ControlObj", Designer.ApplicationConstants.PARAMDIRECTION_OUT, controlName, ACTIVITY_NAME);
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

    public sealed class FindControlWithAutoIdAndName : NativeActivity
    {
        [RequiredArgument]
        public InArgument<string> ControlCanonicalPath { get; set; }
        [RequiredArgument]
        public InArgument<AutomationFacade> AutomationObject { get; set; }

        public InArgument<string> AutomationId { get; set; }
        public InArgument<string> AutomationName { get; set; }


        public OutArgument<Control> ControlObj { get; set; }

        private string controlPath, autoName, autoId;
        private AutomationFacade automationObject;
        private const string ACTIVITY_NAME = "FindControlWithAutoIdAndName";

        protected override void Execute(NativeActivityContext context)
        {

            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ACTIVITY_NAME))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId, ACTIVITY_NAME);

                    controlPath = context.GetValue(this.ControlCanonicalPath);
                    automationObject = context.GetValue(this.AutomationObject);
                    autoId = context.GetValue(this.AutomationId);
                    autoName = context.GetValue(this.AutomationName);

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "ControlCanonicalPath", Designer.ApplicationConstants.PARAMDIRECTION_IN, controlPath, ACTIVITY_NAME);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "AutomationObject", Designer.ApplicationConstants.PARAMDIRECTION_IN, automationObject.ToString(), ACTIVITY_NAME);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "AutomationId", Designer.ApplicationConstants.PARAMDIRECTION_IN, autoId, ACTIVITY_NAME);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "AutomationName", Designer.ApplicationConstants.PARAMDIRECTION_IN, autoName, ACTIVITY_NAME);


                    Control control = automationObject.FindControl(controlPath,autoId,autoName);
                    ControlObj.Set(context, control);

                    string controlName = control != null ? control.Name : "";

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "ControlObj", Designer.ApplicationConstants.PARAMDIRECTION_OUT, controlName, ACTIVITY_NAME);
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

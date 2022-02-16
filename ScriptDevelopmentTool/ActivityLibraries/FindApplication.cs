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
using ATRWrapper = Infosys.ATR.WinUIAutomationRuntimeWrapper;
using System.ComponentModel;

namespace Infosys.WEM.AutomationActivity.Libraries
{
    public sealed class FindApplication : NativeActivity
    {
        [RequiredArgument]
        public InArgument<string> Name { get; set; }
        public InArgument<string> WindowsTitle { get; set; }
        [Description("TimeOut: In seconds, the duration for which it would be tried to get the application window based on the process id or windows title")]
        public InArgument<int> TimeOut { get; set; }
        [RequiredArgument]
        public InArgument<AutomationFacade> AutomationObject { get; set; }


        public OutArgument<ATRWrapper.Application> ApplicationObj { get; set; }

        private string name, windowsTitle;
        private int timeOut;
        private AutomationFacade automationObject;
        private const string ACTIVITY_NAME = "FindApplication";

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

                    name = context.GetValue(this.Name);
                    windowsTitle = context.GetValue(this.WindowsTitle);
                    timeOut = context.GetValue(this.TimeOut);
                    automationObject = context.GetValue(this.AutomationObject);

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "Name", Designer.ApplicationConstants.PARAMDIRECTION_IN, name, ACTIVITY_NAME);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "WindowsTitle", Designer.ApplicationConstants.PARAMDIRECTION_IN, windowsTitle, ACTIVITY_NAME);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "TimeOut", Designer.ApplicationConstants.PARAMDIRECTION_IN, timeOut, ACTIVITY_NAME);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "AutomationObject", Designer.ApplicationConstants.PARAMDIRECTION_IN, automationObject.ToString(), ACTIVITY_NAME);//TODO


                    ATRWrapper.Application app = automationObject.FindApplication(name, windowsTitle, timeOut);
                    ApplicationObj.Set(context, app);

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "ApplicationObj", Designer.ApplicationConstants.PARAMDIRECTION_OUT, name, ACTIVITY_NAME);
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

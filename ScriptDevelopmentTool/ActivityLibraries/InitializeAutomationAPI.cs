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
using ATRWrapperControls = Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls;
//using System.ComponentModel;

namespace Infosys.WEM.AutomationActivity.Libraries
{
    public sealed class InitializeAutomationAPI : NativeActivity
    {
        [RequiredArgument]
        public InArgument<string> AutomationConfigFilePath { get; set; }
        public InArgument<bool> IsLaunchApp { get; set; }
        public InArgument<string> FirstApplicationToLaunch { get; set; }
        public OutArgument<AutomationFacade> AutomationObject { get; set; }
        [RequiredArgument]
        public InArgument<bool> ShowApplicationStartingWaitBox { get; set; }

        private string configFilePath, firstAppToStart;
        private bool isLaunchApp, showWaitBox;
        private const string ACTIVITY_NAME = "InitializeAutomationAPI";

        protected override void Execute(NativeActivityContext context)
        {

            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId,ACTIVITY_NAME))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId,ACTIVITY_NAME);

                    configFilePath = context.GetValue(this.AutomationConfigFilePath);
                    isLaunchApp = context.GetValue(this.IsLaunchApp);
                    firstAppToStart = context.GetValue(this.FirstApplicationToLaunch);
                    showWaitBox = context.GetValue(this.ShowApplicationStartingWaitBox);

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "AutomationConfigFilePath", Designer.ApplicationConstants.PARAMDIRECTION_IN,
                               configFilePath,ACTIVITY_NAME);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "IsLaunchApp", Designer.ApplicationConstants.PARAMDIRECTION_IN, isLaunchApp,ACTIVITY_NAME);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "ShowApplicationStartingWaitBox", Designer.ApplicationConstants.PARAMDIRECTION_IN, showWaitBox, ACTIVITY_NAME);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "FirstApplicationToLaunch", Designer.ApplicationConstants.PARAMDIRECTION_IN,
                               firstAppToStart, ACTIVITY_NAME);

                    AutomationFacade target = null;

                    if (!string.IsNullOrEmpty(configFilePath))
                        target = new AutomationFacade(configFilePath, isLaunchApp, showWaitBox,firstAppToStart);
                    //else
                    //    target = new AutomationFacade(); //[09/12/2014, Rahul]-commented as AutomationConfigFilePath has been made mandatory
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

    //[DisplayName("InitializeAutomationAPI(Empty)")]
    public sealed class InitializeAutomationAPIEmpty : NativeActivity
    {
        public OutArgument<AutomationFacade> AutomationObject { get; set; }
        private const string ACTIVITY_NAME = "InitializeAutomationAPI(Empty)";

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

                    AutomationFacade target = new AutomationFacade(); 
                    AutomationObject.Set(context, target);

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "AutomationObject", Designer.ApplicationConstants.PARAMDIRECTION_OUT,
                                AutomationObject.ToString(), ACTIVITY_NAME);
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

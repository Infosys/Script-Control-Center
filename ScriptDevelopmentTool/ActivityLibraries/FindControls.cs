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
    public sealed class FindControls : NativeActivity
    {
        [RequiredArgument]
        public InArgument<string> ControlCanonicalPath { get; set; }
        [RequiredArgument]
        public InArgument<AutomationFacade> AutomationObject { get; set; }

        public OutArgument<List<Control>> ControlsObj { get; set; }

        private string controlPath;
        private AutomationFacade automationObject;
        private const string ACTIVITY_NAME = "FindControls";

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


                    List<Control> controls = automationObject.FindControls(controlPath);
                    if (controls != null)
                    {
                        ControlsObj.Set(context, controls);

                        if (controls.Count > 0)
                        {
                            string controlName = controls[0] != null ? controls[0].Name : "";

                            LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                                        context.ActivityInstanceId, "ControlObj", Designer.ApplicationConstants.PARAMDIRECTION_OUT, controlName, ACTIVITY_NAME);
                        }
                    }
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

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
    public sealed class HighlightElement : NativeActivity
    {
        [RequiredArgument]
        public InArgument<AutomationFacade> AutomationObject { get; set; }
        [RequiredArgument]
        public InArgument<bool> IsElementTobeHighlighted { get; set; }

        private AutomationFacade automationObject;
        private bool isElementTobeHighlighted;
        private const string ACTIVITY_NAME = "HighlightElement";

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

                    automationObject = context.GetValue(this.AutomationObject);
                    isElementTobeHighlighted = context.GetValue(this.IsElementTobeHighlighted);

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "AutomationObject", Designer.ApplicationConstants.PARAMDIRECTION_IN, automationObject.ToString(), ACTIVITY_NAME);//TODO
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "IsElementTobeHighlighted", Designer.ApplicationConstants.PARAMDIRECTION_IN, isElementTobeHighlighted.ToString(), ACTIVITY_NAME);//TODO


                    automationObject.HighlightElement = isElementTobeHighlighted;                
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

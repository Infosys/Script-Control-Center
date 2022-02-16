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
    public sealed class ImageRecognitionTimeout : NativeActivity
    {
        [RequiredArgument]
        public InArgument<AutomationFacade> AutomationObject { get; set; }
        [RequiredArgument]
        [System.ComponentModel.DisplayName("Timeout: (in seconds) the maximum time duration for which, \n the match for the template image will be searched.")]
        public InArgument<int> Timeout { get; set; }

        private AutomationFacade automationObject;
        private int imageRecognitionTimeout;
        private const string ACTIVITY_NAME = "ImageRecognitionTimeout";

        protected override void Execute(NativeActivityContext context)
        {

            try
            {
                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ACTIVITY_NAME))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId, ACTIVITY_NAME);

                    automationObject = context.GetValue(this.AutomationObject);
                    imageRecognitionTimeout = context.GetValue(this.Timeout);

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "AutomationObject", Designer.ApplicationConstants.PARAMDIRECTION_IN, automationObject.ToString(), ACTIVITY_NAME);//TODO
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "Timeout", Designer.ApplicationConstants.PARAMDIRECTION_IN, imageRecognitionTimeout.ToString(), ACTIVITY_NAME);//TODO


                    automationObject.ImageRecognitionTimeout = imageRecognitionTimeout;
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

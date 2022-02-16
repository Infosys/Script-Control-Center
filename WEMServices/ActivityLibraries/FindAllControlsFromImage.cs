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
    public sealed class FindAllControlsFromImage : NativeActivity
    {
        [RequiredArgument]
        public InArgument<string> ImagePath { get; set; }
        [RequiredArgument]
        public InArgument<AutomationFacade> AutomationObject { get; set; }
        [System.ComponentModel.DisplayName("AllControlObjects: List of all the controls matching the provided image template.")]
        public OutArgument<List<Control>> AllControlObjects { get; set; }

        private string imagePath;
        private AutomationFacade automationObject;
        private const string ACTIVITY_NAME = "FindAllControlsFromImage";

        protected override void Execute(NativeActivityContext context)
        {

            try
            {
                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ACTIVITY_NAME))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId, ACTIVITY_NAME);

                    imagePath = context.GetValue(this.ImagePath);
                    automationObject = context.GetValue(this.AutomationObject);

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "ImagePath", Designer.ApplicationConstants.PARAMDIRECTION_IN, imagePath, ACTIVITY_NAME);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "AutomationObject", Designer.ApplicationConstants.PARAMDIRECTION_IN, automationObject.ToString(), ACTIVITY_NAME);//TODO


                    List<Control> controls = automationObject.FindControls(imagePath);
                    AllControlObjects.Set(context, controls);

                    string controlcount = controls != null ? controls.Count.ToString() : "0";

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "AllControlObjects count", Designer.ApplicationConstants.PARAMDIRECTION_OUT, controlcount, ACTIVITY_NAME);
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

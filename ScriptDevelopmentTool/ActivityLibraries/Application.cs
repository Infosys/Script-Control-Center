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
using System.Workflow.ComponentModel.Design;
using Infosys.WEM.Infrastructure.Common;
using Designer = Infosys.WEM.AutomationActivity.Designers;
using ATRWrapperControls = Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls;
using ATRWrapper = Infosys.ATR.WinUIAutomationRuntimeWrapper;
using System.ComponentModel;

namespace Infosys.WEM.AutomationActivity.Libraries.Application
{
    public sealed class KeyPress : NativeActivity
    {
        [RequiredArgument]
        public InArgument<ATRWrapper.Application> ApplicationObj { get; set; }
        public InArgument<string> Text { get; set; }
        [Description("Refer to the API user guide for the list of valid key codes.\n")]
        public InArgument<int[]> ModifierKey { get; set; }
        protected override void Execute(NativeActivityContext context)
        {
            //check if stop requested, if show then throw exception
            if (ATRWrapper.Core.Utilities.IsStopRequested())
                throw new ATRWrapper.Core.IAPExceptions.StopRequested();
            try
            {
                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ActivityEvents.KEYPRESS, ActivityControls.TEXTBOX))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId,
                        ActivityEvents.KEYPRESS, ActivityControls.TEXTBOX);


                    ATRWrapper.Application app = context.GetValue(ApplicationObj);
                    string sendText = context.GetValue(Text);
                    int[] modifierkey = context.GetValue(ModifierKey);
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

        public Activity Create(System.Windows.DependencyObject target)
        {
            return new KeyPress
            {
                DisplayName = "KeyPress- Application",
            };
        }
    }

    public sealed class SendText : NativeActivity
    {
        [RequiredArgument]
        public InArgument<ATRWrapper.Application> ApplicationObj { get; set; }
        public InArgument<string> Text { get; set; }
        [Description("Refer to the API user guide for the list of valid key codes.\n")]
       
        protected override void Execute(NativeActivityContext context)
        {
            //check if stop requested, if show then throw exception
            if (ATRWrapper.Core.Utilities.IsStopRequested())
                throw new ATRWrapper.Core.IAPExceptions.StopRequested();
            try
            {
                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ActivityEvents.KEYPRESS, ActivityControls.TEXTBOX))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId,
                        ActivityEvents.KEYPRESS, ActivityControls.TEXTBOX);


                    ATRWrapper.Application app = context.GetValue(ApplicationObj);
                    string sendText = context.GetValue(Text);
                   
                    string appName = app != null ? app.Name : "";

                    //in param
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "ControlObj", Designer.ApplicationConstants.PARAMDIRECTION_IN, appName);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "SendToText", Designer.ApplicationConstants.PARAMDIRECTION_IN, sendText);
                    
                    app.SendText(sendText);

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

        public Activity Create(System.Windows.DependencyObject target)
        {
            return new SendText
            {
                DisplayName = "SendText- Application",
            };
        }
    }

    public sealed class KeyDown : NativeActivity
    {
        [RequiredArgument]
        public InArgument<ATRWrapper.Application> ApplicationObj { get; set; }
        [Description("Refer to the API user guide for the list of valid key codes.\n")]
        public InArgument<int> ModifierKey { get; set; }
        protected override void Execute(NativeActivityContext context)
        {
            //check if stop requested, if show then throw exception
            if (ATRWrapper.Core.Utilities.IsStopRequested())
                throw new ATRWrapper.Core.IAPExceptions.StopRequested();
            try
            {
                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ActivityEvents.KEYPRESS, ActivityControls.TEXTBOX))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId,
                        ActivityEvents.KEYPRESS, ActivityControls.TEXTBOX);


                    ATRWrapper.Application app = context.GetValue(ApplicationObj);
                    int modifierkey = context.GetValue(ModifierKey);
                    string appName = app != null ? app.Name : "";

                    //in param
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "ControlObj", Designer.ApplicationConstants.PARAMDIRECTION_IN, appName);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "ModifierKey", Designer.ApplicationConstants.PARAMDIRECTION_IN, modifierkey);

                    app.KeyDown(modifierkey);

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

        public Activity Create(System.Windows.DependencyObject target)
        {
            return new KeyDown
            {
                DisplayName = "KeyDown- Application",
            };
        }
    }

    public sealed class KeyUp : NativeActivity
    {
        [RequiredArgument]
        public InArgument<ATRWrapper.Application> ApplicationObj { get; set; }
        [Description("Refer to the API user guide for the list of valid key codes.\n")]
        public InArgument<int> ModifierKey { get; set; }
        protected override void Execute(NativeActivityContext context)
        {
            //check if stop requested, if show then throw exception
            if (ATRWrapper.Core.Utilities.IsStopRequested())
                throw new ATRWrapper.Core.IAPExceptions.StopRequested();
            try
            {
                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ActivityEvents.KEYPRESS, ActivityControls.TEXTBOX))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId,
                        ActivityEvents.KEYPRESS, ActivityControls.TEXTBOX);


                    ATRWrapper.Application app = context.GetValue(ApplicationObj);
                    int modifierkey = context.GetValue(ModifierKey);
                    string appName = app != null ? app.Name : "";

                    //in param
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "ControlObj", Designer.ApplicationConstants.PARAMDIRECTION_IN, appName);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "ModifierKey", Designer.ApplicationConstants.PARAMDIRECTION_IN, modifierkey);

                    app.KeyUp(modifierkey);

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

        public Activity Create(System.Windows.DependencyObject target)
        {
            return new KeyUp
            {
                DisplayName = "KeyUp- Application",
            };
        }
    }

    public sealed class ShowApplicationStartingWaitBox : NativeActivity
    {
        [RequiredArgument]
        public InArgument<ATRWrapper.Application> ApplicationObject { get; set; }
        [RequiredArgument]
        public InArgument<bool> ShowWaitBox { get; set; }

        private ATRWrapper.Application appObject;
        private bool showWaitBox;
        private const string ACTIVITY_NAME = "ShowApplicationStartingWaitBox";

        protected override void Execute(NativeActivityContext context)
        {
            try
            {
                //check if stop requested, if show then throw exception
                if (ATRWrapper.Core.Utilities.IsStopRequested())
                    throw new ATRWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ACTIVITY_NAME))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId, ACTIVITY_NAME);

                    appObject = context.GetValue(this.ApplicationObject);
                    showWaitBox = context.GetValue(this.ShowWaitBox);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "ApplicationObject", Designer.ApplicationConstants.PARAMDIRECTION_IN, appObject.Name, ACTIVITY_NAME);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "ShowWaitBox", Designer.ApplicationConstants.PARAMDIRECTION_IN, showWaitBox.ToString(), ACTIVITY_NAME);

                    appObject.ShowAppStartWaitBox = showWaitBox;
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

    public sealed class StartApplication : NativeActivity
    {
        [RequiredArgument]
        public InArgument<ATRWrapper.Application> ApplicationObject { get; set; }
        [Description("Result: true- if it has access to the windows of the application started, else false")]
        public OutArgument<bool> Result { get; set; }

        private ATRWrapper.Application appObject;
        private const string ACTIVITY_NAME = "StartApplication";

        protected override void Execute(NativeActivityContext context)
        {
            try
            {
                //check if stop requested, if show then throw exception
                if (ATRWrapper.Core.Utilities.IsStopRequested())
                    throw new ATRWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ACTIVITY_NAME))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId, ACTIVITY_NAME);

                    appObject = context.GetValue(this.ApplicationObject);

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "ApplicationObject", Designer.ApplicationConstants.PARAMDIRECTION_IN, appObject.Name, ACTIVITY_NAME);//TODO

                    Result.Set(context, appObject.StartApp());

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "Result", Designer.ApplicationConstants.PARAMDIRECTION_OUT, Result.ToString());
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

    public sealed class StartApplicationWithArg : NativeActivity
    {
        [RequiredArgument]
        public InArgument<ATRWrapper.Application> ApplicationObject { get; set; }
        [RequiredArgument]
        public InArgument<string> ApplicationArgument { get; set; }
        [Description("Result: true- if it has access to the windows of the application started, else false")]
        public OutArgument<bool> Result { get; set; }

        private ATRWrapper.Application appObject;
        private string arguement;
        private const string ACTIVITY_NAME = "StartApplicationWithArg";

        protected override void Execute(NativeActivityContext context)
        {
            try
            {
                //check if stop requested, if show then throw exception
                if (ATRWrapper.Core.Utilities.IsStopRequested())
                    throw new ATRWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ACTIVITY_NAME))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId, ACTIVITY_NAME);

                    appObject = context.GetValue(this.ApplicationObject);
                    arguement = context.GetValue(this.ApplicationArgument);

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "ApplicationObject", Designer.ApplicationConstants.PARAMDIRECTION_IN, appObject.Name, ACTIVITY_NAME);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "ApplicationArguement", Designer.ApplicationConstants.PARAMDIRECTION_IN, arguement, ACTIVITY_NAME);

                    Result.Set(context,appObject.StartApp(arguement));

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "Result", Designer.ApplicationConstants.PARAMDIRECTION_OUT, Result.ToString());
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

    public sealed class CloseApplication : NativeActivity
    {
        [RequiredArgument]
        public InArgument<ATRWrapper.Application> ApplicationObject { get; set; }

        private ATRWrapper.Application appObject;
        private const string ACTIVITY_NAME = "CloseApplication";

        protected override void Execute(NativeActivityContext context)
        {
            try
            {
                //check if stop requested, if show then throw exception
                if (ATRWrapper.Core.Utilities.IsStopRequested())
                    throw new ATRWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ACTIVITY_NAME))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId, ACTIVITY_NAME);

                    appObject = context.GetValue(this.ApplicationObject);

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "ApplicationObject", Designer.ApplicationConstants.PARAMDIRECTION_IN, appObject.Name, ACTIVITY_NAME);//TODO

                    appObject.CloseApp();
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

    public sealed class SetFocus : NativeActivity
    {
        [RequiredArgument]
        public InArgument<ATRWrapper.Application> ApplicationObject { get; set; }

        private ATRWrapper.Application appObject;
        private const string ACTIVITY_NAME = "SetFocus";

        protected override void Execute(NativeActivityContext context)
        {
            try
            {
                //check if stop requested, if show then throw exception
                if (ATRWrapper.Core.Utilities.IsStopRequested())
                    throw new ATRWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ACTIVITY_NAME))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId, ACTIVITY_NAME);

                    appObject = context.GetValue(this.ApplicationObject);

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "ApplicationObject", Designer.ApplicationConstants.PARAMDIRECTION_IN, appObject.Name, ACTIVITY_NAME);//TODO

                    appObject.SetFocus();
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

    public sealed class IsAvailable : NativeActivity
    {
        [RequiredArgument]
        public InArgument<ATRWrapper.Application> ApplicationObject { get; set; }
        public OutArgument<bool> Result { get; set; }

        private ATRWrapper.Application appObject;
        private const string ACTIVITY_NAME = "IsAvailable";

        protected override void Execute(NativeActivityContext context)
        {
            try
            {
                //check if stop requested, if show then throw exception
                if (ATRWrapper.Core.Utilities.IsStopRequested())
                    throw new ATRWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ACTIVITY_NAME))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId, ACTIVITY_NAME);

                    appObject = context.GetValue(this.ApplicationObject);

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "ApplicationObject", Designer.ApplicationConstants.PARAMDIRECTION_IN, appObject.Name, ACTIVITY_NAME);//TODO

                    Result.Set(context, appObject.IsAvailable);

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "Result", Designer.ApplicationConstants.PARAMDIRECTION_OUT, Result.ToString());
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

    public sealed class TimeOut : NativeActivity
    {
        [RequiredArgument]
        public InArgument<ATRWrapper.Application> ApplicationObject { get; set; }
        [Description("Seconds: the duration for which it would be tried to get the application window based on the process id or windows title")]
        public InArgument<int> Seconds { get; set; }

        private ATRWrapper.Application appObject;
        private int timeOut;
        private const string ACTIVITY_NAME = "TimeOut";

        protected override void Execute(NativeActivityContext context)
        {
            try
            {
                //check if stop requested, if show then throw exception
                if (ATRWrapper.Core.Utilities.IsStopRequested())
                    throw new ATRWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ACTIVITY_NAME))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId, ACTIVITY_NAME);

                    appObject = context.GetValue(this.ApplicationObject);
                    timeOut = context.GetValue(this.Seconds);

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "ApplicationObject", Designer.ApplicationConstants.PARAMDIRECTION_IN, appObject.Name, ACTIVITY_NAME);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "Seconds", Designer.ApplicationConstants.PARAMDIRECTION_IN, timeOut, ACTIVITY_NAME);

                    appObject.TimeOut = timeOut;
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

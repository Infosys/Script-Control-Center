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
using Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Base;
using Infosys.WEM.Infrastructure.Common;
using Designer = Infosys.WEM.AutomationActivity.Designers;
using ATRWrapperControls = Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls;
using System.Activities.Presentation;
using System.ComponentModel;

namespace Infosys.WEM.AutomationActivity.Libraries.Document
{
    public sealed class Click : NativeActivity, IActivityTemplateFactory
    {
        [RequiredArgument]
        public InArgument<Control> ControlObj { get; set; }
        protected override void Execute(NativeActivityContext context)
        {
            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ActivityEvents.CLICK, ActivityControls.DOCUMENT))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId,
                        ActivityEvents.CLICK, ActivityControls.DOCUMENT);


                    Control ctrl = context.GetValue(ControlObj);
                    string controlName = ctrl != null ? ctrl.Name : "";

                    //in param
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "ControlObj", Designer.ApplicationConstants.PARAMDIRECTION_IN, controlName);


                    ATRWrapperControls.Document documentToInvoke = (ATRWrapperControls.Document)ctrl;
                    documentToInvoke.Click();

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
            return new Click
            {
                DisplayName = "Click- Document",
            };
        }
    }

    public sealed class ClickWithOffset : NativeActivity, IActivityTemplateFactory
    {
        [RequiredArgument]
        public InArgument<Control> ControlObj { get; set; }
        [RequiredArgument]
        public InArgument<int> XAxis { get; set; }
        [RequiredArgument]
        public InArgument<int> YAxis { get; set; }
        protected override void Execute(NativeActivityContext context)
        {
            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ActivityEvents.CLICK, ActivityControls.DOCUMENT))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId,
                        ActivityEvents.CLICK, ActivityControls.DOCUMENT);


                    Control ctrl = context.GetValue(ControlObj);
                    int xaxis = context.GetValue(XAxis);
                    int yaxis = context.GetValue(YAxis);
                    string controlName = ctrl != null ? ctrl.Name : "";

                    //in param
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "ControlObj", Designer.ApplicationConstants.PARAMDIRECTION_IN, controlName);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "X-Axis", Designer.ApplicationConstants.PARAMDIRECTION_IN, xaxis);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "Y-Axis", Designer.ApplicationConstants.PARAMDIRECTION_IN, yaxis);

                    ATRWrapperControls.Document documentToInvoke = (ATRWrapperControls.Document)ctrl;
                    documentToInvoke.ClickWithOffset(xaxis, yaxis);

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
            return new ClickWithOffset
            {
                DisplayName = "ClickWithOffset- Document",
            };
        }
    }

    public sealed class Hover : NativeActivity, IActivityTemplateFactory
    {
        [RequiredArgument]
        public InArgument<Control> ControlObj { get; set; }
        protected override void Execute(NativeActivityContext context)
        {


            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ActivityEvents.HOVER, ActivityControls.DOCUMENT))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId,
                        ActivityEvents.HOVER, ActivityControls.DOCUMENT);


                    Control ctrl = context.GetValue(ControlObj);
                    string controlName = ctrl != null ? ctrl.Name : "";

                    //in param
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "ControlObj", Designer.ApplicationConstants.PARAMDIRECTION_IN, controlName);


                    ATRWrapperControls.Document documentToInvoke = (ATRWrapperControls.Document)ctrl;
                    documentToInvoke.Hover();

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
            return new Hover
            {
                DisplayName = "Hover- Document",
            };
        }
    }

    public sealed class SendText : NativeActivity, IActivityTemplateFactory
    {
        [RequiredArgument]
        public InArgument<Control> ControlObj { get; set; }
        [RequiredArgument]
        public InArgument<string> Text { get; set; }
        public OutArgument<bool> Result { get; set; }
        protected override void Execute(NativeActivityContext context)
        {

            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ActivityEvents.SEND_TEXT, ActivityControls.DOCUMENT))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId,
                        ActivityEvents.SEND_TEXT, ActivityControls.DOCUMENT);


                    Control ctrl = context.GetValue(ControlObj);
                    string sendText = context.GetValue(Text);
                    string controlName = ctrl != null ? ctrl.Name : "";

                    //in param
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "ControlObj", Designer.ApplicationConstants.PARAMDIRECTION_IN, controlName);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "SendToText", Designer.ApplicationConstants.PARAMDIRECTION_IN, sendText);


                    ATRWrapperControls.Document documentToInvoke = (ATRWrapperControls.Document)ctrl;
                    Result.Set(context, documentToInvoke.SendText(sendText));

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "Result", Designer.ApplicationConstants.PARAMDIRECTION_OUT, Result.ToString());
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
                DisplayName = "SendText- Document",
            };
        }
    }

    public sealed class KeyPress : NativeActivity, IActivityTemplateFactory
    {
        [RequiredArgument]
        public InArgument<Control> ControlObj { get; set; }
        public InArgument<string> Text { get; set; }
        [Description("Refer to the API user guide for the list of valid key codes.")]
        public InArgument<int[]> ModifierKey { get; set; }
        protected override void Execute(NativeActivityContext context)
        {

            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ActivityEvents.KEYPRESS, ActivityControls.DOCUMENT))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId,
                        ActivityEvents.KEYPRESS, ActivityControls.DOCUMENT);


                    Control ctrl = context.GetValue(ControlObj);
                    string sendText = context.GetValue(Text);
                    int[] modifierkey = context.GetValue(ModifierKey);
                    string controlName = ctrl != null ? ctrl.Name : "";

                    //in param
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "ControlObj", Designer.ApplicationConstants.PARAMDIRECTION_IN, controlName);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "SendToText", Designer.ApplicationConstants.PARAMDIRECTION_IN, sendText);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "ModifierKey", Designer.ApplicationConstants.PARAMDIRECTION_IN, modifierkey);


                    ATRWrapperControls.Document documentToInvoke = (ATRWrapperControls.Document)ctrl;
                    documentToInvoke.KeyPress(sendText, modifierkey);

                    
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
                DisplayName = "KeyPress- Document",
            };
        }
    }

    public sealed class ReceiveText : NativeActivity, IActivityTemplateFactory
    {
        [RequiredArgument]
        public InArgument<Control> ControlObj { get; set; }
        public OutArgument<string> Result { get; set; }
        protected override void Execute(NativeActivityContext context)
        {

            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ActivityEvents.RECEIVE_TEXT, ActivityControls.DOCUMENT))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId,
                        ActivityEvents.RECEIVE_TEXT, ActivityControls.DOCUMENT);


                    Control ctrl = context.GetValue(ControlObj);
                    string controlName = ctrl != null ? ctrl.Name : "";

                    //in param
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "ControlObj", Designer.ApplicationConstants.PARAMDIRECTION_IN, controlName);
                  

                    ATRWrapperControls.Document documentToInvoke = (ATRWrapperControls.Document)ctrl;
                    Result.Set(context, documentToInvoke.ReceiveText());

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "Result", Designer.ApplicationConstants.PARAMDIRECTION_OUT, Result);
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
            return new ReceiveText
            {
                DisplayName = "ReceiveText- Document",
            };
        }
    }
}

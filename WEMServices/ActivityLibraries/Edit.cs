﻿using System;
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

namespace Infosys.WEM.AutomationActivity.Libraries.Edit
{
    public sealed class Click : NativeActivity, IActivityTemplateFactory
    {
        [RequiredArgument]
        public InArgument<Control> ControlObj { get; set; }
        protected override void Execute(NativeActivityContext context)
        {

            try
            {
                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ActivityEvents.CLICK, ActivityControls.EDIT))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId,
                         ActivityEvents.CLICK, ActivityControls.EDIT);


                    Control ctrl = context.GetValue(ControlObj);
                    string controlName = ctrl != null ? ctrl.Name : "";

                    //in param
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "ControlObj", Designer.ApplicationConstants.PARAMDIRECTION_IN, controlName);


                    ATRWrapperControls.Edit editToInvoke = (ATRWrapperControls.Edit)ctrl;
                    editToInvoke.Click();

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
                DisplayName = "Click- Edit",
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
                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ActivityEvents.CLICK, ActivityControls.EDIT))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId,
                         ActivityEvents.CLICK, ActivityControls.EDIT);


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
                    
                    ATRWrapperControls.Edit editToInvoke = (ATRWrapperControls.Edit)ctrl;
                    editToInvoke.ClickWithOffset(xaxis, yaxis);

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
                DisplayName = "ClickWithOffset- Edit",
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
                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ActivityEvents.HOVER, ActivityControls.EDIT))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId, 
                        ActivityEvents.HOVER, ActivityControls.EDIT);


                    Control ctrl = context.GetValue(ControlObj);
                    string controlName = ctrl != null ? ctrl.Name : "";

 
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "ControlObj", Designer.ApplicationConstants.PARAMDIRECTION_IN, controlName);


                    ATRWrapperControls.Edit editToInvoke = (ATRWrapperControls.Edit)ctrl;
                    editToInvoke.Hover();

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
                DisplayName = "Hover- Edit",
            };
        }
    }

    public sealed class SendText : NativeActivity, IActivityTemplateFactory
    {
        [RequiredArgument]
        public InArgument<Control> ControlObj { get; set; }
        [RequiredArgument]
        public InArgument<string> SendToText { get; set; }
        public OutArgument<bool> Result { get; set; }
        protected override void Execute(NativeActivityContext context)
        {
            try
            {
                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ActivityEvents.SEND_TEXT, ActivityControls.EDIT))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId, 
                        ActivityEvents.SEND_TEXT, ActivityControls.EDIT
                        );

                    Control ctrl = context.GetValue(ControlObj);
                    string sendText = context.GetValue(SendToText);
                    string controlName = ctrl != null ? ctrl.Name : "";

                    //in param
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "ControlObj", Designer.ApplicationConstants.PARAMDIRECTION_IN, controlName);

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "SendToText", Designer.ApplicationConstants.PARAMDIRECTION_IN, sendText);

                    ATRWrapperControls.Edit editToInvoke = (ATRWrapperControls.Edit)ctrl;
                    Result.Set(context, editToInvoke.SendText(sendText));

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
                DisplayName = "SendText- Edit",
            };
        }
    }

    public sealed class KeyPress : NativeActivity, IActivityTemplateFactory
    {
        [RequiredArgument]
        public InArgument<Control> ControlObj { get; set; }
        public InArgument<string> SendToText { get; set; }
        [System.ComponentModel.DisplayName("ModifierKey: 0 for shift, 1 for ctrl, 2 for meta, 3 for alt, 4 for caps, 5 for enter, 6 for tab.\n NB- in case of CAPS/ENTER, etc first call with text= blank or null and modifiers= 4 or 5, \nthen again call with the required text as neeeded and modifier key = -1.\n")]
        public InArgument<int> ModifierKey { get; set; }
        
        protected override void Execute(NativeActivityContext context)
        {
            try
            {
                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ActivityEvents.KEYPRESS, ActivityControls.EDIT))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId,
                        ActivityEvents.KEYPRESS, ActivityControls.EDIT
                        );

                    Control ctrl = context.GetValue(ControlObj);
                    string sendText = context.GetValue(SendToText);
                    int modifierkey = context.GetValue(ModifierKey);
                    string controlName = ctrl != null ? ctrl.Name : "";

                    //in param
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "ControlObj", Designer.ApplicationConstants.PARAMDIRECTION_IN, controlName);

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "SendToText", Designer.ApplicationConstants.PARAMDIRECTION_IN, sendText);

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "ModifierKey", Designer.ApplicationConstants.PARAMDIRECTION_IN, modifierkey);

                    ATRWrapperControls.Edit editToInvoke = (ATRWrapperControls.Edit)ctrl;
                    editToInvoke.KeyPress(sendText, modifierkey);                    

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
                DisplayName = "KeyPress- Edit",
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
                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ActivityEvents.RECEIVE_TEXT, ActivityControls.EDIT))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId, 
                        ActivityEvents.RECEIVE_TEXT, ActivityControls.EDIT);


                    Control ctrl = context.GetValue(ControlObj);
                    string controlName = ctrl != null ? ctrl.Name : "";

                    //in param
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "ControlObj", Designer.ApplicationConstants.PARAMDIRECTION_IN, controlName);


                    ATRWrapperControls.Edit editToInvoke = (ATRWrapperControls.Edit)ctrl;
                    Result.Set(context, editToInvoke.ReceiveText());

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
                DisplayName = "ReceiveText- Edit",
            };
        }
    }
}

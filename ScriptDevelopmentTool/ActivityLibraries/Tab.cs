/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using Infosys.ATR.WinUIAutomationRuntimeWrapper;
using ATRWrapperControls = Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls;
using System.Workflow.ComponentModel.Design;
using Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Base;
using Infosys.WEM.Infrastructure.Common;
using Designer = Infosys.WEM.AutomationActivity.Designers;
using System.Activities.Presentation;

namespace Infosys.WEM.AutomationActivity.Libraries.Tab
{
    public sealed class SelectTabItem : NativeActivity, IActivityTemplateFactory
    {
        [RequiredArgument]
        public InArgument<Control> ControlObj { get; set; }
        [RequiredArgument]
        public InArgument<string> Name { get; set; }
        protected override void Execute(NativeActivityContext context)
        {

            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ActivityEvents.SELECT, ActivityControls.TAB))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId,
                        ActivityEvents.SELECT, ActivityControls.TAB);


                    Control ctrl = context.GetValue(ControlObj);
                    string name = context.GetValue(Name);
                    string controlName = ctrl != null ? ctrl.Name : "";

                    //in param
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "ControlObj", Designer.ApplicationConstants.PARAMDIRECTION_IN, controlName);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "SendToText", Designer.ApplicationConstants.PARAMDIRECTION_IN, name);


                    ATRWrapperControls.Tab tabToInvoke = (ATRWrapperControls.Tab)ctrl;
                    tabToInvoke.SelectItem(name);                    
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
            return new SelectTabItem
            {
                DisplayName = "SelectItem- Tab",
            };
        }
    }

    public sealed class GetSelectedTabItem : NativeActivity, IActivityTemplateFactory
    {
        [RequiredArgument]
        public InArgument<Control> ControlObj { get; set; }
        public OutArgument<ATRWrapperControls.TabItem> SelectedTabItem { get; set; }
        protected override void Execute(NativeActivityContext context)
        {

            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ActivityEvents.GET_SELECTED, ActivityControls.TAB))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId,
                        ActivityEvents.GET_SELECTED, ActivityControls.TAB);


                    Control ctrl = context.GetValue(ControlObj);
                    string controlName = ctrl != null ? ctrl.Name : "";

                    //in param
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "ControlObj", Designer.ApplicationConstants.PARAMDIRECTION_IN, controlName);
                    
                    ATRWrapperControls.Tab tabToInvoke = (ATRWrapperControls.Tab)ctrl;
                    ATRWrapperControls.TabItem tabItem = tabToInvoke.GetSelectedItem();
                    SelectedTabItem.Set(context,tabItem);

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "SelectedTab", Designer.ApplicationConstants.PARAMDIRECTION_OUT, tabItem.Name);
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
            return new GetSelectedTabItem
            {
                DisplayName = "GetSelectedItem- Tab",
            };
        }
    }
}

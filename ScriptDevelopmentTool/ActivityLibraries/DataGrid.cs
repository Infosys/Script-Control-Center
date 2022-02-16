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

namespace Infosys.WEM.AutomationActivity.Libraries.DataGrid
{
    public sealed class SelectDataItems : NativeActivity, IActivityTemplateFactory
    {
        [RequiredArgument]
        public InArgument<Control> ControlObj { get; set; }
        [RequiredArgument]
        public InArgument<string[]> Names { get; set; }
        protected override void Execute(NativeActivityContext context)
        {

            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ActivityEvents.SELECT, ActivityControls.DATAGRID))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId,
                        ActivityEvents.SELECT, ActivityControls.DATAGRID);


                    Control ctrl = context.GetValue(ControlObj);
                    string[] names = context.GetValue(Names);
                    string controlName = ctrl != null ? ctrl.Name : "";

                    //in param
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "ControlObj", Designer.ApplicationConstants.PARAMDIRECTION_IN, controlName);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "Total item count", Designer.ApplicationConstants.PARAMDIRECTION_IN, names.Length);


                    ATRWrapperControls.DataGrid datagridToInvoke = (ATRWrapperControls.DataGrid)ctrl;
                    datagridToInvoke.SelectItems(names.ToList());
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
            return new SelectDataItems
            {
                DisplayName = "SelectItems- DataGrid",
            };
        }
    }

    public sealed class SelectAllDataItems : NativeActivity, IActivityTemplateFactory
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
                    context.ActivityInstanceId, ActivityEvents.SELECT_ALL_ITEMS, ActivityControls.DATAGRID))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId,
                        ActivityEvents.SELECT_ALL_ITEMS, ActivityControls.DATAGRID);


                    Control ctrl = context.GetValue(ControlObj);
                    string controlName = ctrl != null ? ctrl.Name : "";

                    //in param
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "ControlObj", Designer.ApplicationConstants.PARAMDIRECTION_IN, controlName);
                    

                    ATRWrapperControls.DataGrid datagridToInvoke = (ATRWrapperControls.DataGrid)ctrl;
                    datagridToInvoke.SelectAllItems();
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
            return new SelectAllDataItems
            {
                DisplayName = "SelectAllItems- DataGrid",
            };
        }
    }

    public sealed class GetSelectedDataItems : NativeActivity, IActivityTemplateFactory
    {
        [RequiredArgument]
        public InArgument<Control> ControlObj { get; set; }
        public OutArgument<List<ATRWrapperControls.DataItem>> SelectedDataItems { get; set; }
        protected override void Execute(NativeActivityContext context)
        {

            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ActivityEvents.GET_SELECTED, ActivityControls.DATAGRID))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId,
                        ActivityEvents.GET_SELECTED, ActivityControls.DATAGRID);


                    Control ctrl = context.GetValue(ControlObj);
                    string controlName = ctrl != null ? ctrl.Name : "";

                    //in param
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "ControlObj", Designer.ApplicationConstants.PARAMDIRECTION_IN, controlName);

                    ATRWrapperControls.DataGrid datagridToInvoke = (ATRWrapperControls.DataGrid)ctrl;
                    List<ATRWrapperControls.DataItem> dataItems = datagridToInvoke.GetSelectedItems();
                    SelectedDataItems.Set(context, dataItems);

                    int dataItemCount = dataItems != null ? dataItems.Count : 0;

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "Selected data item count", Designer.ApplicationConstants.PARAMDIRECTION_OUT, dataItemCount);
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
            return new GetSelectedDataItems
            {
                DisplayName = "GetSelectedItems- DataGrid",
            };
        }
    }
}

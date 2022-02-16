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
using Infosys.WEM.Infrastructure.Common;
using Designer = Infosys.WEM.AutomationActivity.Designers;
using System.Activities.Presentation;

namespace Infosys.WEM.AutomationActivity.Libraries.TableRow
{
    public sealed class GetAllCells : NativeActivity
    {
        [RequiredArgument]
        public InArgument<ATRWrapperControls.TableRow> TableRowObj { get; set; }
        public OutArgument<List<ATRWrapperControls.TableCell>> Cells { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ActivityEvents.GET_ALL_CELLS, ActivityControls.TABLEROW))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId,
                         ActivityEvents.GET_ALL_CELLS, ActivityControls.TABLEROW);

                    ATRWrapperControls.TableRow rowtoInvoke = context.GetValue(TableRowObj);

                    List<ATRWrapperControls.TableCell> cellsFound = rowtoInvoke.GetAllCells();
                    Cells.Set(context, cellsFound);
                    int cellCount = cellsFound != null ? cellsFound.Count : 0;

                    //out param
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                              context.ActivityInstanceId, "Cell count", Designer.ApplicationConstants.PARAMDIRECTION_OUT, cellCount);

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
    }

    public sealed class GetCellAtIndex : NativeActivity
    {
        [RequiredArgument]
        public InArgument<ATRWrapperControls.TableRow> TableRowObj { get; set; }
        [RequiredArgument]
        public InArgument<int> Index { get; set; }
        public OutArgument<ATRWrapperControls.TableCell> Cell { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ActivityEvents.GET_CELL_AT_INDEX, ActivityControls.TABLEROW))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId,
                         ActivityEvents.GET_CELL_AT_INDEX, ActivityControls.TABLEROW);

                    ATRWrapperControls.TableRow rowtoInvoke = context.GetValue(TableRowObj);
                    int index = context.GetValue(Index);

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "Cell index", Designer.ApplicationConstants.PARAMDIRECTION_IN, index);

                    ATRWrapperControls.TableCell cellFound = rowtoInvoke.GetCellAt(index);
                    Cell.Set(context, cellFound);
                    bool cellIsFound = cellFound != null ? true : false;

                    //out param
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                              context.ActivityInstanceId, "is cell found?", Designer.ApplicationConstants.PARAMDIRECTION_OUT, cellIsFound.ToString());

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
    }

    public sealed class GetCellIndexesWithtext : NativeActivity
    {
        [RequiredArgument]
        public InArgument<ATRWrapperControls.TableRow> TableRowObj { get; set; }
        [RequiredArgument]
        public InArgument<string> CellText { get; set; }
        public OutArgument<List<int>> Indexes { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ActivityEvents.GET_CELL_INDEXES_WITH_TEXT, ActivityControls.TABLEROW))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId,
                         ActivityEvents.GET_CELL_INDEXES_WITH_TEXT, ActivityControls.TABLEROW);

                    ATRWrapperControls.TableRow rowtoInvoke = context.GetValue(TableRowObj);
                    string text = context.GetValue(CellText);

                    //in param
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "Cell text", Designer.ApplicationConstants.PARAMDIRECTION_IN, text);

                    List<int> indexes = rowtoInvoke.GetCellIndexesWithtext(text);
                    Indexes.Set(context, indexes);
                    int indexFound = indexes != null ? indexes.Count : 0;

                    //out param
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                              context.ActivityInstanceId, "indexes found", Designer.ApplicationConstants.PARAMDIRECTION_OUT, indexFound.ToString());

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
    }
}

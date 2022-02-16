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
using System.Windows.Forms;
using Infosys.ATR.WinUIAutomationRuntimeWrapper;
using Infosys.WEM.Infrastructure.Common;
using Designer = Infosys.WEM.AutomationActivity.Designers;
using System.ComponentModel;
using Infosys.WEM.AutomationActivity.Designers;
using System.Workflow.ComponentModel;

namespace Infosys.WEM.AutomationActivity.Libraries
{
    public sealed class Pause : NativeActivity
    {
        //public InArgument<string> BookMarkName { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                   context.ActivityInstanceId, ActivityEvents.PAUSEANDRESUME, ActivityControls.PAUSEANDRESUME))
            {
                //check if stop requested, if so then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                //string bookmark = string.IsNullOrEmpty(context.GetValue(BookMarkName)) ? Guid.NewGuid().ToString() : context.GetValue(BookMarkName);
                string bookmark = Guid.NewGuid().ToString();
                //Set the workflow state to the TLS store
                System.Threading.Thread.FreeNamedDataSlot("iapPausingAcivityBookMark");
                LocalDataStoreSlot localData = System.Threading.Thread.AllocateNamedDataSlot("iapPausingAcivityBookMark");
                System.Threading.Thread.SetData(localData, bookmark);

                context.CreateBookmark(bookmark, new BookmarkCallback(CallbackForResume));
            }
        }

        void CallbackForResume(NativeActivityContext context, Bookmark bookMark, object arguments)
        {
            LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId, Designer.ApplicationConstants.RESUME_WORKFLOW);

            //asign the input parameters to the global workflow arguments
            //'arguements' would be of type List<WorkflowParam>
            if (arguments != null)
            {
                List<WorkflowParam> inputParameters = arguments as List<WorkflowParam>;
                if (inputParameters == null)
                    throw new Exception("If provided, the input parameters are extected of type- List<WorkflowParam> only, please refer to the guide.");

                var properties = context.DataContext.GetProperties();

                foreach (var arg in inputParameters)
                {
                    for (int i = 0; i < properties.Count; i++)
                    {
                        if (((System.ComponentModel.MemberDescriptor)(properties[i])).DisplayName == arg.ParameterName)
                        {
                            properties[arg.ParameterName].SetValue(context.DataContext, arg.ParameterValue);
                            break;
                        }
                    }
                }
                //foreach (var arg in inputParameters)
                //{
                //    if (context.Properties.Find(arg.ParameterName) != null)
                //    {
                //        properties[arg.ParameterName].SetValue(context.DataContext, arg.ParameterValue);
                //    }
                //    else
                //    {
                //        throw new Exception(string.Format("No argument by name {0} is expected by the workflow being executed", arg.ParameterName));
                //    }
                //}
            }
        }

        protected override bool CanInduceIdle
        {
            get
            {
                return true; //to cause the workflow to become idle
            }
        }

    }

    public class ParameterMap
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public string Type { get; set; }
    }
}

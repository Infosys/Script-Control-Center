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

namespace Infosys.WEM.AutomationActivity.Libraries
{
    public sealed class Alert : NativeActivity
    {
        [RequiredArgument]
        public InArgument<string> Message { get; set; }
        [RequiredArgument]
        public InArgument<string> Title { get; set; }
        [RequiredArgument]
        [Description("Type : 0 for Information, 1 for Warning, 2 for Error")] //similary add any other types as needed and accordingly update the switch below
        public InArgument<int> MessageType { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                   context.ActivityInstanceId, ActivityEvents.ALERT, ActivityControls.ALERT))
            {
                string message = context.GetValue(Message);
                string title = context.GetValue(Title);
                string type = context.GetValue(MessageType).ToString();
                switch (type)
                {
                    case "1":
                        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    case "2":
                        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case "0":
                    default:
                        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                }
            }
        }
    }
}

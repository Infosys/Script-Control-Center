/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using IMSWorkBench.Infrastructure.Interface;
using Infosys.ATR.ProcessRecorder.Constants;
//using Infosys.ATR.ProcessRecorder.Services;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.CompositeUI.WPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Infosys.ATR.ProcessRecorder
{

    public class ModuleController : WorkItemController
    {
        Dictionary<string, string> appSettingCollection = new Dictionary<string, string>();

        Dictionary<String, ControlledWorkItem<ProcessRecorderWorkItem>> workitems
          = new Dictionary<string, ControlledWorkItem<ProcessRecorderWorkItem>>();

        ControlledWorkItem<ProcessRecorderWorkItem> _current = null;

        public override void Run()
        {

            //try
            //{
            //    string recorderConfigFilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Infosys.ATR.UIAutomation.Recorder.exe.config";
            //    appSettingCollection = RecorderService.GetRecorderAppSetting(recorderConfigFilePath);
            //}
            //catch (System.Exception ex)
            //{
            //    MessageBox.Show("Errorreading recorder config file", "Recorder Config Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}

        }
        private void ExtendMenu()
        {

        }

        [EventSubscription(EventTopicNames.RecorderView, ThreadOption.UserInterface)]
        public void RecorderView(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Infosys.ATR.UIAutomation.Recorder.exe";
                Process.Start(startInfo);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error occured when trying to launch the Recorder feature", "Recorder Load Fail", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private ControlledWorkItem<ProcessRecorderWorkItem> GetActiveControlledWorkitem()
        {
            foreach (KeyValuePair<string, WorkItem> wi in WorkItem.WorkItems)
            {
                if (wi.Value.Status == WorkItemStatus.Active)
                {
                    return workitems[wi.Value.ID];
                }
            }
            return null;
        }
        void wf_Activated(object sender, EventArgs e)
        {
            _current = GetActiveControlledWorkitem();
        }
        void wf_Activating(object sender, System.ComponentModel.CancelEventArgs e)
        {
        
        }
        private ProcessRecorderWorkItem AddWorkitem()
        {
            var pr_name = "PR" + DateTime.Now.Ticks.ToString();
            var pr = this.WorkItem.WorkItems.AddNew<ControlledWorkItem<ProcessRecorderWorkItem>>();
            pr.Activating += wf_Activating;
            pr.Activated += wf_Activated;
            pr.ID = pr_name;
            pr.Controller.Run();
            workitems.Add(pr_name, pr);
            pr.Activate();

            return pr.Controller;
    }

        [EventSubscription(EventTopicNames.RecordingView, ThreadOption.UserInterface)]
        public void RecordingView_Click(object sender, EventArgs e)
        {
            try
            {
                if (_current != null)
                    _current.Controller.RecordingView(sender, e);
                else
                {
                    var recordng = AddWorkitem();
                    recordng.RecordingView(sender, e);
                }
            }
            catch
            {
                if (_current != null)
                    _current.Controller.closeTab_Click(sender, e);
            }
        }

        [EventSubscription(EventTopicNames.CloseRecordingTab, ThreadOption.UserInterface)]
        public void closeTab_Click(object sender, EventArgs e)
        {
            if (_current != null)
                _current.Controller.closeTab_Click(sender, e);
            else
            {
                var recordng = GetActiveControlledWorkitem();

                if (recordng != null)
                {
                    recordng.Controller.closeTab_Click(sender, e);
                }
            }


        }

        [EventSubscription(EventTopicNames.GeneratePlaybackScript, ThreadOption.UserInterface)]
        public void GenerateScript_Click(object sender, EventArgs e)
        {
            if (_current != null)
                _current.Controller.GenerateScript_Handler(sender, e);
            else
            {
                var recordng = GetActiveControlledWorkitem();

                if (recordng != null)
                {
                    recordng.Controller.GenerateScript_Handler(sender, e);
                }
            }
        }

        [EventSubscription(EventTopicNames.RunPlaybackScript, ThreadOption.UserInterface)]
        public void ExecutePlaybackScript_Click(object sender, EventArgs e)
        {
            if (_current != null)
                _current.Controller.ExecutePlyabackScript_Handler(sender, e);
            else
            {
                var recordng = GetActiveControlledWorkitem();

                if (recordng != null)
                {
                    recordng.Controller.ExecutePlyabackScript_Handler(sender, e);
                }
            }
        }
    }
}

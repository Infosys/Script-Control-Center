/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using IMSWorkBench.Infrastructure.Interface;
using Infosys.ATR.ProcessRecorder.Views;
using Microsoft.Practices.CompositeUI.WinForms;
using IMSWorkBench.Infrastructure.Library.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.CompositeUI.EventBroker;
using System.Windows.Forms;

namespace Infosys.ATR.ProcessRecorder
{
    public class ProcessRecorderWorkItem : WorkItemController
    {
        [EventPublication(Constants.EventTopicNames.TabHoverSet, PublicationScope.Global)]
        public event EventHandler<EventArgs<String>> TabHoverSet;

        [EventPublication(Constants.EventTopicNames.DisableGeneratePlaybackScript, PublicationScope.Global)]
        public event EventHandler DisableGeneratePlaybackScript;

        internal void exit_Click(object sender, EventArgs e)
        {

            closeTab_Click(sender, e);
        }

        internal void closeTab_Click(object sender, EventArgs e)
        {
            var recoDe = GetActiveTab();
            IClose smartPart = recoDe as IClose;
            bool close = smartPart.Close();
            if (close)
                this.WorkItem.SmartParts.Remove(smartPart);

            DisableGeneratePlaybackScript(this, new EventArgs());
        }

        object GetActiveTab()
        {
            return this.WorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].ActiveSmartPart;
        }

        void CloseIfExists<T>(string smartPart) where T : IClose
        {
            var sp = this.WorkItem.SmartParts.Get<T>(smartPart);
            if (sp != null)
            {
                sp.Close();
                this.WorkItem.SmartParts.Remove(sp);
            }
        }

        private object GetCurrentTab()
        {
            return this.WorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].ActiveSmartPart;
        }
        internal void RecordingView(object sender, EventArgs e) 
        {
            AddPlaybackView("RecordingView");
        }
        private void AddPlaybackView(string smartpart) 
        {
            CloseIfExists<RecordingView>("RecordingView");
            var sp = this.WorkItem.SmartParts.Get<RecordingView>(smartpart);
            if (sp == null)
            {
                var recoView = this.WorkItem.SmartParts.AddNew<RecordingView>(smartpart);
                recoView.PopulateView();
                if (recoView.UseCases != null)
                {
                    WindowSmartPartInfo sp1 = new WindowSmartPartInfo();
                    sp1.MaximizeBox = false;
                    sp1.MinimizeBox = false;
                    sp1.Title = "Recording View";
                    this.WorkItem.RootWorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].Show(recoView, sp1);
                    Logger.Log("Recording", "RecordingView", "Recording View Launched");
                }
                else
                    MessageBox.Show("No Record Exists ", "IAP - Recording View", MessageBoxButtons.OK, MessageBoxIcon.Information);                
            }
        }

        [EventSubscription(Constants.EventTopicNames.TabHover, ThreadOption.UserInterface)]
        public void TabHover_Handler(object sender, EventArgs e)
        {
            var ide = GetCurrentTab();

            if (ide.GetType() == typeof(Views.RecordingView))
            {
                var p = ide as Views.RecordingView;

                if (TabHoverSet != null)
                    TabHoverSet(this, new EventArgs<string>("Recording View"));
            }
        }

        public void GenerateScript_Handler(object sender, EventArgs e)
        {
            var ide = GetCurrentTab();
            if (ide.GetType() == typeof(Views.RecordingView))
            {
                var p = ide as Views.RecordingView;

                p.btnGenPlaybackScript_Click(sender, e);
            }        
        }

        public void ExecutePlyabackScript_Handler(object sender, EventArgs e) 
        {
            var ide = GetCurrentTab();
            if (ide.GetType() == typeof(Views.RecordingView))
            {
                var p = ide as Views.RecordingView;

                p.RunPlaybackScript_Click(sender, e);
            }
        }    
    }
}

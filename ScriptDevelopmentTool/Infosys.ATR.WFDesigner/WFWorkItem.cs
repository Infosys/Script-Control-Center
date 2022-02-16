/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;

using Infosys.ATR.WFDesigner.Views;
using Infosys.ATR.WFDesigner.Entities;

using IMSWorkBench.Infrastructure.Interface;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;

using IMSWorkBench.Infrastructure.Library.UI;
using Microsoft.Practices.CompositeUI.EventBroker;
using System.Diagnostics;
using IMSWorkBench.Infrastructure.Library.Services;
using Infosys.ATR.WFDesigner.Constants;
using Infosys.LicenseValidationClient;
using Infosys.IAP.CommonClientLibrary;

namespace Infosys.ATR.WFDesigner
{
    public enum Mode
    {
        Online,
        Offline,
        Edit,
        New,
        Open
    }

    public class WFWorkItem : WorkItemController
    {
        static int _count = 1;
        Mode _mode;
        static Dictionary<WFDesigner.Views.WFDesigner, bool> wfWorkItems =
            new Dictionary<WFDesigner.Views.WFDesigner, bool>();

        [EventPublication(Constants.EventTopicNames.LoadGeneric, PublicationScope.Global)]
        public event EventHandler LoadWF;

        [EventPublication(Constants.EventTopicNames.TabHoverSet, PublicationScope.Global)]
        public event EventHandler<EventArgs<String>> TabHoverSet;

        [EventPublication(EventTopicNames.DeActivatePublish, PublicationScope.Global)]
        public event EventHandler DeActivatePublish;

        [EventPublication(EventTopicNames.ActivatePublish, PublicationScope.Global)]
        public event EventHandler ActivatePublish;

        public override void Run()
        {
            ValidationResult result = Validator.Validate();
            bool showError = false;
            if (result.IsSuccess && result.FeaturesAllowed != null && result.FeaturesAllowed.Count > 0)
            {
                if (result.FeaturesAllowed.Contains(Feature.WorkflowDesigner))
                {
                    _mode = (Mode)Enum.Parse(typeof(Mode), this.WorkItem.RootWorkItem.State["Mode"].ToString());

                    LoadWF(this, new EventArgs());

                }
                else
                    showError = true;
            }
            else
                showError = true;
            if (showError)
                MessageBox.Show("Invalid License to access this feature. Please contact for right license", "Invalid License", MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.WorkItem.Items.Add(wfWorkItems, "Workflows");
        }


        private WFDesigner.Views.WFDesigner AddView()
        {
            var title = "WF Designer" + _count;
            var _wfDe = this.WorkItem.SmartParts.AddNew<WFDesigner.Views.WFDesigner>(title);
            _wfDe.Dock = DockStyle.Fill;
            WindowSmartPartInfo sp = new WindowSmartPartInfo();
            sp.Title = _wfDe.Title = title;
            this.WorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].Show(_wfDe, sp);
            _count++;
            wfWorkItems.Add(_wfDe, false);
            return _wfDe;
        }


        internal void newWF_Click(object sender, EventArgs e)
        {

            var wfDe = AddView();
            wfDe.NewWorkFlow();
            wfDe.OpMode = Mode.New;
            LoadWF(this, new EventArgs());
            Logger.Log("Workflow", "New WF",
               "New WF " + wfDe.Title);
        }


        internal void openWF_Click(object sender, EventArgs e)
        {

            var wfDe = AddView();
            wfDe.promptToSave = false;
            wfDe.OpenWorkFlow();
            LoadWF(this, new EventArgs());
        }

        internal void openWF(string fileName)
        {

            var wfDe = AddView();
            wfDe.Title = System.IO.Path.GetFileNameWithoutExtension(fileName);
            wfDe.IsIapPackage = false;
            wfDe.OpenWorkFlow(fileName);
            wfDe.OpMode = Mode.New;
            LoadWF(this, new EventArgs());

            Logger.Log("Workflow", "OpenWF",
                "Edit WF " + wfDe.Title + " locally");
        }


        internal void openWFFromPackage(PackageMeta metaData)
        {
            var wfDe = AddView();
            wfDe.Title = metaData.Content.Name;
            wfDe.data=Translators.WorkflowPE_SE.WorkflowOEtoPE(metaData.Content);
            wfDe.IsIapPackage = true;
            string fileName = System.IO.Path.Combine(metaData.PackageExtractLoc, metaData.Content.Name, metaData.Content.Name + "." + metaData.Content.ContentType);
            wfDe.OpenWorkFlow(fileName);
            wfDe.OpMode = Mode.New;
            LoadWF(this, new EventArgs());

            Logger.Log("Workflow", "OpenWF",
                "Edit WF " + wfDe.Title + " locally");
            
        }


        internal void saveWF_Click(object sender, EventArgs e)
        {
            var wfDe = GetActiveTab() as WFDesigner.Views.WFDesigner;
            wfDe.Save();
        }

        internal void saveImageWF_Click(object sender, EventArgs e)
        {
            var wfDe =
                 GetActiveTab() as WFDesigner.Views.WFDesigner;
            wfDe.SaveWorkFlowAsImage();
        }


        internal void publish_Click(object sender, EventArgs e)
        {
            var wfDe =
            GetActiveTab() as WFDesigner.Views.WFDesigner;

            if (wfDe != null)
            {
                wfDe.AddPropertyTab(wfDe.data);
                wfDe.OpMode = Mode.Edit;
                DeActivatePublish(this, new EventArgs());

            }
        }

        internal void runWF_Click(object sender, EventArgs e)
        {
            WFDesigner.Views.WFDesigner wfDe
                = GetActiveTab() as WFDesigner.Views.WFDesigner;
            wfDe.Run();
        }

        internal void closeTab_Click(object sender, EventArgs e)
        {
            var wfDe = GetActiveTab();

            IClose smartPart = wfDe as IClose;
            bool close = smartPart.Close();
            if (close)
                this.WorkItem.SmartParts.Remove(smartPart);
        }




        internal void exit_Click(object sender, EventArgs e)
        {

            closeTab_Click(sender, e);
        }



        internal void settings_Click(object sender, EventArgs e)
        {

            CloseIfExists<Settings>("settings");

            var settings = this.WorkItem.SmartParts.AddNew<Views.Settings>("settings");
            // settings.Initialize();
            WindowSmartPartInfo sp = new WindowSmartPartInfo();
            sp.MaximizeBox = false;
            sp.MinimizeBox = false;
            sp.Title = "Settings";
            this.WorkItem.RootWorkItem.Workspaces[Constants.WorkspaceNames.ModalWindows].Show(settings, sp);


        }


        internal void openRepoWF_Click(object sender, EventArgs e)
        {
            AddViews();
        }

        internal void AddViews()
        {
            if (_mode == Mode.Online)
            {
                CloseIfExists<WFSelector>("WorkflowExplorer");

                var openwf = this.WorkItem.SmartParts.AddNew<Views.WFSelector>("WorkflowExplorer");
                openwf.AddSmartParts();
                openwf.Initialize(false);
                if (openwf.Categories != null)
                {
                    WindowSmartPartInfo sp = new WindowSmartPartInfo();
                    sp.MaximizeBox = false;
                    sp.MinimizeBox = false;
                    sp.Title = "Workflow Explorer";
                    this.WorkItem.RootWorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].Show(openwf, sp);
                }

                Logger.Log("Workflow", "WFExplorerView",
                    "WFExplorer View Launched");
            }
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

        [EventSubscription(Constants.EventTopicNames.OpenWFFromRepository, ThreadOption.UserInterface)]
        public void OpenWFFromRepositoryHandler(object sender, EventArgs<WorkflowPE> e)
        {
            OpenWFFromRepositoryHandler(e.Data);
            //  Showtoolbar.Invoke(sender, e);
        }

        //[EventSubscription(Constants.EventTopicNames.SaveAndClose, ThreadOption.UserInterface)]
        public void SaveAndClose_handler(object sender, EventArgs<FormClosingEventArgs> e)
        {
            List<WFDesigner.Views.WFDesigner> wfs = new List<Views.WFDesigner>();

            if (wfWorkItems != null && wfWorkItems.Count > 0)
            {
                foreach (KeyValuePair<WFDesigner.Views.WFDesigner, bool> kvp in wfWorkItems)
                {
                    wfs.Add(kvp.Key);
                }

                foreach (WFDesigner.Views.WFDesigner wf in wfs)
                {
                    IClose smartPart = wf as IClose;
                    bool close = smartPart.Close();
                    if (close)
                        this.WorkItem.SmartParts.Remove(smartPart);
                    else
                        e.Data.Cancel = true;
                }
            }
        }



        public void OpenWFFromRepositoryHandler(WorkflowPE data)
        {
            CloseIfExists<WFSelector>("OpenWorkflow");
            var wfDe = AddView();
            try
            {
                wfDe.OpenWorkFlowFromRepository(data);
                wfDe.Title = data.Name;
                wfDe.OpMode = Mode.Edit;
                LoadWF(this, new EventArgs());
                Logger.Log("Workflow", "OpenWFFromRepository",
                "Edit WF " + wfDe.Title + " from repository");
            }
            catch (Exception e)
            {
                wfDe.Close();
                throw;
            }
        }

        [EventSubscription(Constants.EventTopicNames.TabHover, ThreadOption.UserInterface)]
        public void TabHover_Handler(object sender, EventArgs e)
        {
            var ide = GetCurrentTab();

            if (ide.GetType() == typeof(Views.WFDesigner))
            {
                var p = ide as Views.WFDesigner;

                if (TabHoverSet != null)
                    TabHoverSet(this, new EventArgs<string>(p.Title));
            }
        }

        [EventSubscription(EventTopicNames.CurrentTabSelected, ThreadOption.UserInterface)]
        public void DisablePublish(object sender, EventArgs<string> e)
        {
            WFDesigner.Views.WFDesigner wd = null;

            foreach (KeyValuePair<WFDesigner.Views.WFDesigner, bool> kvp in wfWorkItems)
            {
                if (kvp.Key.Title == e.Data)
                {
                    wd = kvp.Key;
                    break;
                }
            }



            if (wd != null)
            {
                if (wd.OpMode == Mode.Edit)
                    DeActivatePublish(this, new EventArgs());
                else if (wd.OpMode == Mode.New)
                    ActivatePublish(this, new EventArgs());
            }

        }

        private object GetCurrentTab()
        {
            return this.WorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].ActiveSmartPart;
        }


        internal void TransactionView(object sender, EventArgs e)
        {
            AddTransactionView("TransactionView");
        }

        private void AddTransactionView(string smartpart)
        {
            CloseIfExists<TransactionView>("TransactionView");
            var sp = this.WorkItem.SmartParts.Get<TransactionView>(smartpart);
            if (sp == null)
            {
                if (_mode == Mode.Online)
                {
                    var transView = this.WorkItem.SmartParts.AddNew<TransactionView> (smartpart);
                    transView.PopulateView();
                    if (transView.Transactions != null)
                    {
                        WindowSmartPartInfo sp1 = new WindowSmartPartInfo();
                        sp1.MaximizeBox = false;
                        sp1.MinimizeBox = false;
                        sp1.Title = "Transaction View";
                        this.WorkItem.RootWorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].Show(transView, sp1);
                    }
                    else
                        MessageBox.Show("No Transaction Record Available For User", "IAP - Transaction View",MessageBoxButtons.OK, MessageBoxIcon.Information);

                    Logger.Log("Workflow", "TransactionView",
                        "Transaction View Launched");
                }
            }
        }

        internal void deleteWF_click(object sender, EventArgs<string> e)
        {
        }
    }
}

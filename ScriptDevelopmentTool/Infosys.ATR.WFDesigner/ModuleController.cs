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
    public class ModuleController : WorkItemController
    {
        Mode _mode;

        Dictionary<String, ControlledWorkItem<WFWorkItem>> workitems
          = new Dictionary<string, ControlledWorkItem<WFWorkItem>>();

        ControlledWorkItem<WFWorkItem> _current = null;

        //[EventPublication(Constants.EventTopicNames.LoadGeneric, PublicationScope.Global)]
        //public event EventHandler LoadWF;

        [EventPublication(Constants.EventTopicNames.Exit, PublicationScope.Global)]
        public event EventHandler Exit;

        //[EventPublication(EventTopicNames.Showtoolbar, PublicationScope.Global)]
        //public event EventHandler Showtoolbar;

        public override void Run()
        {
            //    ValidationResult result = Validator.Validate();
            //    bool showError = false;
            //    if (result.IsSuccess && result.FeaturesAllowed != null && result.FeaturesAllowed.Count > 0)
            //    {
            //        if (result.FeaturesAllowed.Contains(Feature.WorkflowDesigner))
            //        {
            //_mode = (Mode)Enum.Parse(typeof(Mode), this.WorkItem.RootWorkItem.State["Mode"].ToString());
            //              ExtendMenu();
            //            //LoadWF(this, new EventArgs());
            //              AddViews();
            //             ExtendToolStrip();
            //        }
            //        else
            //            showError = true;
            //    }
            //    else
            //        showError = true;
            //    if (showError)
            //    {
            //        MessageBox.Show("Invalid License to access this feature. Please contact for right license", "Invalid License", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        System.Windows.Forms.Application.Exit();
            //        System.Environment.Exit(1);
            //    }
        }


        private void ExtendMenu()
        {
            // File menu begins here
            ToolStripMenuItem newWorkflow = new ToolStripMenuItem("New Workflow");
            newWorkflow.Click += new EventHandler(newWF_Click);
            newWorkflow.ShortcutKeys = Keys.Control | Keys.N;
            this.WorkItem.RootWorkItem.UIExtensionSites["FileMenu"].Add<ToolStripMenuItem>(newWorkflow);

            ToolStripMenuItem openWF = new ToolStripMenuItem("Open Workflow");
            openWF.Click += new EventHandler(openWF_Click);
            openWF.ShortcutKeys = Keys.Control | Keys.O;
            openWF.ToolTipText = "Open existing Workflow";
            this.WorkItem.RootWorkItem.UIExtensionSites["FileMenu"].Add<ToolStripMenuItem>(openWF);

            this.WorkItem.RootWorkItem.UIExtensionSites["FileMenu"].Add<ToolStripSeparator>(new ToolStripSeparator());

            ToolStripMenuItem save = new ToolStripMenuItem("Save");
            save.Enabled = false;
            this.WorkItem.RootWorkItem.UIExtensionSites["FileMenu"].Add<ToolStripMenuItem>(save);

            ToolStripMenuItem saveWF = new ToolStripMenuItem("Workflow");
            saveWF.Click += new EventHandler(saveWF_Click);
            saveWF.ShortcutKeys = Keys.Control | Keys.S;
            saveWF.ToolTipText = "Save Current Workflow";
            save.DropDownItems.Add(saveWF);

            ToolStripMenuItem saveImageWF = new ToolStripMenuItem("Workflow As Image");
            saveImageWF.Click += new EventHandler(saveImageWF_Click); ;
            saveImageWF.ShortcutKeys = Keys.Control | Keys.I;
            saveImageWF.ToolTipText = "Save Current Workflow as Image";
            save.DropDownItems.Add(saveImageWF);

            this.WorkItem.RootWorkItem.UIExtensionSites["FileMenu"].Add<ToolStripSeparator>(new ToolStripSeparator());

            ToolStripMenuItem closeTab = new ToolStripMenuItem("Close Tab");
            closeTab.Click += new EventHandler(closeTab_Click);
            closeTab.ShortcutKeys = Keys.Control | Keys.T;
            closeTab.ToolTipText = "Close current tab";
            this.WorkItem.RootWorkItem.UIExtensionSites["FileMenu"].Add<ToolStripMenuItem>(closeTab);

            ToolStripMenuItem exit = new ToolStripMenuItem("Exit");
            exit.Click += new EventHandler(exit_Click);
            exit.ShortcutKeys = Keys.Control | Keys.Q;
            exit.ToolTipText = "Exit Application";
            this.WorkItem.RootWorkItem.UIExtensionSites["FileMenu"].Add<ToolStripMenuItem>(exit);

            // Workflow menu begins here
            ToolStripMenuItem wf = new ToolStripMenuItem("Workflow");
            this.WorkItem.RootWorkItem.UIExtensionSites["MainMenu"].Add<ToolStripMenuItem>(wf);

            ToolStripMenuItem runWF = new ToolStripMenuItem("Run");
            runWF.Click += new EventHandler(runWF_Click);
            runWF.ShortcutKeys = Keys.Control | Keys.R;
            runWF.Enabled = false;
            runWF.ToolTipText = "Run current workflow";
            wf.DropDownItems.Add(runWF);

            wf.DropDownItems.Add(new ToolStripSeparator());

            if (_mode == Mode.Online)
            {

                ToolStripMenuItem publish = new ToolStripMenuItem("Publish Workflow");
                publish.Click += new EventHandler(publish_Click);
                publish.ShortcutKeys = Keys.Control | Keys.P;
                publish.ToolTipText = "Publish current Workflow";
                publish.Enabled = false;
                wf.DropDownItems.Add(publish);

                ToolStripMenuItem openRepoWF = new ToolStripMenuItem("Workflow Explorer");
                openRepoWF.Click += new EventHandler(openRepoWF_Click);
                openRepoWF.ShortcutKeys = Keys.Control | Keys.Alt | Keys.O;
                openRepoWF.ToolTipText = "Open existing Workflow from repository";
                wf.DropDownItems.Add(openRepoWF);
            }
        }

        private void ExtendToolStrip()
        {
            ToolStripButton newWorkflowBtn = new ToolStripButton();
            newWorkflowBtn.Image = new System.Drawing.Bitmap(@"Images\add-script.png");
            newWorkflowBtn.ToolTipText = "New Workflow";
            newWorkflowBtn.Click += new EventHandler(newWF_Click);
            this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(newWorkflowBtn);

            ToolStripButton opeWorkflowBtn = new ToolStripButton();
            opeWorkflowBtn.Image = new System.Drawing.Bitmap(@"Images\open.png");
            opeWorkflowBtn.ToolTipText = "Open Workflow";
            opeWorkflowBtn.Click += new EventHandler(openWF_Click);
            this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(opeWorkflowBtn);

            ToolStripDropDownButton saveWFBtn = new ToolStripDropDownButton();
            saveWFBtn.Image = new System.Drawing.Bitmap(@"Images\save.png");
            saveWFBtn.ToolTipText = "Save";
            saveWFBtn.Enabled = false;
            this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripDropDownButton>(saveWFBtn);

            ToolStripButton saveWorkflowBtn = new ToolStripButton();
            saveWorkflowBtn.Image = new System.Drawing.Bitmap(@"Images\SaveWorkflow.png");
            saveWorkflowBtn.ToolTipText = "Save Workflow";
            saveWorkflowBtn.Click += new EventHandler(saveWF_Click);
            saveWFBtn.DropDownItems.Add(saveWorkflowBtn);

            ToolStripButton saveWorkflowAsImageBtn = new ToolStripButton();
            saveWorkflowAsImageBtn.Image = new System.Drawing.Bitmap(@"Images\WfAsImage.png");
            saveWorkflowAsImageBtn.ToolTipText = "Save Current Workflow As Image";
            saveWorkflowAsImageBtn.Click += new EventHandler(saveImageWF_Click);
            saveWFBtn.DropDownItems.Add(saveWorkflowAsImageBtn);

            this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripSeparator>(new ToolStripSeparator());

            ToolStripButton runWorkflowBtn = new ToolStripButton();
            runWorkflowBtn.Image = new System.Drawing.Bitmap(@"Images\play.png");
            runWorkflowBtn.ToolTipText = "Run Workflow Locally";
            runWorkflowBtn.Click += new EventHandler(runWF_Click);
            runWorkflowBtn.Enabled = false;
            this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(runWorkflowBtn);

            this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripSeparator>(new ToolStripSeparator());

            if (_mode == Mode.Online)
            {

                ToolStripButton publishWorkflowBtn = new ToolStripButton();
                publishWorkflowBtn.Image = new System.Drawing.Bitmap(@"Images\publish.png");
                publishWorkflowBtn.ToolTipText = "Publish Workflow";
                publishWorkflowBtn.Click += new EventHandler(publish_Click);
                publishWorkflowBtn.Enabled = false;
                this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(publishWorkflowBtn);

                ToolStripButton explorerWorkflowBtn = new ToolStripButton();
                explorerWorkflowBtn.Image = new System.Drawing.Bitmap(@"Images\explorer.png");
                explorerWorkflowBtn.ToolTipText = "Workflow Explorer";
                explorerWorkflowBtn.Click += new EventHandler(openRepoWF_Click);
                this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(explorerWorkflowBtn);

                this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripSeparator>(new ToolStripSeparator());
            }

            ToolStripButton closeButton = new ToolStripButton();
            closeButton.Image = new System.Drawing.Bitmap(@"Images\close.gif");
            closeButton.Click += new EventHandler(closeTab_Click);
            closeButton.ToolTipText = "Close current tab";
            this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(closeButton);

            ToolStripButton exitBtn = new ToolStripButton();
            exitBtn.Image = new System.Drawing.Bitmap(@"Images\exit.png");
            exitBtn.Click += new EventHandler(exit_Click);
            exitBtn.ToolTipText = "Exit Application";
            this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(exitBtn);

            //  Showtoolbar(this, new EventArgs());
        }

        private WFWorkItem AddWorkitem()
        {
            var wf_name = "WF" + DateTime.Now.Ticks.ToString();
            var wf = this.WorkItem.WorkItems.AddNew<ControlledWorkItem<WFWorkItem>>();
            wf.Activating += wf_Activating;
            wf.Activated += wf_Activated;
            wf.ID = wf_name;
            wf.Controller.Run();
            workitems.Add(wf_name, wf);
            wf.Activate();

            return wf.Controller;
        }

        void wf_Activated(object sender, EventArgs e)
        {
            _current = GetActiveControlledWorkitem();
        }

        void wf_Activating(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private ControlledWorkItem<WFWorkItem> GetActiveControlledWorkitem()
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


        private void AddViews()
        {
            var wf = AddWorkitem();
            wf.AddViews();
        }

        [EventSubscription(EventTopicNames.NeWWorkflow, ThreadOption.UserInterface)]
        public void newWF_Click(object sender, EventArgs e)
        {
            var wf = AddWorkitem();
            wf.newWF_Click(new object(), new EventArgs());
        }

        [EventSubscription(EventTopicNames.OpenWorkflow, ThreadOption.UserInterface)]
        public void openWF(object sender, EventArgs<String> e)
        {
            var wf = AddWorkitem();
            wf.openWF(e.Data);
        }



        [EventSubscription(EventTopicNames.OpenWFFromPackage, ThreadOption.UserInterface)]
        public void openWF(object sender, EventArgs<PackageMeta> e)
        {
            var wf = AddWorkitem();
            wf.openWFFromPackage(e.Data);
            wf.publish_Click(sender, e);
        }

        void openWF_Click(object sender, EventArgs e)
        {
            var wf = AddWorkitem();
            wf.openWF_Click(sender, e);
        }

        [EventSubscription(EventTopicNames.SaveWorkflow, ThreadOption.UserInterface)]
        public void saveWF_Click(object sender, EventArgs e)
        {
            if (_current != null)
            {
                _current.Controller.saveWF_Click(sender, e);
            }
            else
            {
                var wf = GetActiveControlledWorkitem();
                wf.Controller.saveWF_Click(sender, e);
            }
        }

        void saveImageWF_Click(object sender, EventArgs e)
        {
            _current.Controller.saveImageWF_Click(sender, e);
        }


        [EventSubscription(EventTopicNames.PublishWF, ThreadOption.UserInterface)]
        public void publish_Click(object sender, EventArgs e)
        {
            if (_current != null)
                _current.Controller.publish_Click(sender, e);
            else
            {
                var wf = GetActiveControlledWorkitem();
                wf.Controller.publish_Click(sender, e);
            }
        }

        [EventSubscription(EventTopicNames.RunWF, ThreadOption.UserInterface)]
        public void runWF_Click(object sender, EventArgs e)
        {
            if (_current != null)
                _current.Controller.runWF_Click(sender, e);
            else
            {
                var wf = GetActiveControlledWorkitem();
                wf.Controller.runWF_Click(sender, e);
            }
        }

        [EventSubscription(EventTopicNames.CloseWFTab, ThreadOption.UserInterface)]
        public void closeTab_Click(object sender, EventArgs e)
        {
            if (_current != null)
                _current.Controller.closeTab_Click(sender, e);
            else
            {
                var wf = GetActiveControlledWorkitem();

                if (wf != null)
                {
                    wf.Controller.closeTab_Click(sender, e);
                }
            }

            //_current.Deactivate();
            //_current.Dispose();
        }


        void exit_Click(object sender, EventArgs e)
        {
            if (_current != null)
                _current.Controller.exit_Click(sender, e);
            if (Exit != null)
                Exit(this, e);
        }



        void settings_Click(object sender, EventArgs e)
        {
            _current.Controller.settings_Click(sender, e);
        }

        [EventSubscription(EventTopicNames.WFView, ThreadOption.UserInterface)]
        public void openRepoWF_Click(object sender, EventArgs e)
        {
            try
            {
                if (_current != null)
                    _current.Controller.openRepoWF_Click(sender, e);
                else
                {
                    var wf = AddWorkitem();
                    wf.openRepoWF_Click(sender, e);
                }
            }
            catch
            {
                if (_current != null)
                    _current.Controller.closeTab_Click(sender, e);
            }
        }

        [EventSubscription(EventTopicNames.TransactionView, ThreadOption.UserInterface)]
        public void TransactionView_Click(object sender, EventArgs e)
        {
            try
            {
                if (_current != null)
                    _current.Controller.TransactionView(sender, e);
                else
                {
                    var wf = AddWorkitem();
                    wf.TransactionView(sender, e);
                }
            }
            catch
            {
                if (_current != null)
                    _current.Controller.closeTab_Click(sender, e);
            }
        }



        [EventSubscription(Constants.EventTopicNames.ActivateMenu, ThreadOption.UserInterface)]
        public void ActivateMenuHandler(object sender, EventArgs<bool> e)
        {
            //((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites["FileMenu"].ToList()[3]).Enabled = e.Data;
            //var workflowMenu = (ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites["MainMenu"].ToList()[0];
            //if (workflowMenu != null && workflowMenu.DropDownItems.Count > 0)
            //{
            //    workflowMenu.DropDownItems[0].Enabled = true;
            //    if(_mode == Mode.Online)
            //        workflowMenu.DropDownItems[2].Enabled = e.Data;
            //}

            //((ToolStripDropDownButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[2]).Enabled = e.Data;
            //((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[4]).Enabled = e.Data;
            //if(_mode == Mode.Online)
            //    ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[6]).Enabled = e.Data;


            //publish
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[1].Enabled = e.Data;
        }

        [EventSubscription(Constants.EventTopicNames.SaveAndClose, ThreadOption.UserInterface)]
        public void SaveAndClose_handler(object sender, EventArgs<FormClosingEventArgs> e)
        {
            if (_current != null)
                _current.Controller.SaveAndClose_handler(this, e);

        }

    }
}

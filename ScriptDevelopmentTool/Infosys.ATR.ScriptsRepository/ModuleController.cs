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

using Infosys.ATR.ScriptRepository.Views;
using Infosys.ATR.ScriptRepository.Constants;

using IMSWorkBench.Infrastructure.Interface;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;

using IMSWorkBench.Infrastructure.Library.UI;
using Microsoft.Practices.CompositeUI.EventBroker;
using System.Diagnostics;
using IMSWorkBench.Infrastructure.Library.Services;
using Infosys.LicenseValidationClient;
using Infosys.IAP.CommonClientLibrary;

namespace Infosys.ATR.ScriptRepository
{
    public class ModuleController : WorkItemController
    {
        MainRepositoryView _main = null;
        Dictionary<String, ControlledWorkItem<ScriptRepoWorkItem>> workitems
          = new Dictionary<string, ControlledWorkItem<ScriptRepoWorkItem>>();
        ControlledWorkItem<ScriptRepoWorkItem> _current = null;

        [EventPublication(Constants.EventTopicNames.Exit, PublicationScope.Global)]
        public event EventHandler Exit;

        [EventPublication(EventTopicNames.DeActivatePublish, PublicationScope.Global)]
        public event EventHandler DeActivatePublish;

        [EventPublication(EventTopicNames.ActivatePublish, PublicationScope.Global)]
        public event EventHandler ActivatePublish;

        [EventPublication(Constants.EventTopicNames.ShowRun, PublicationScope.Global)]
        public event EventHandler<EventArgs<bool>> ShowRun;

        //[EventPublication(EventTopicNames.Showtoolbar, PublicationScope.Global)]
        //public event EventHandler Showtoolbar;



        public override void Run()
        {
            //ValidationResult result = Validator.Validate();
            //bool showError = false;
            //if (result.IsSuccess && result.FeaturesAllowed != null && result.FeaturesAllowed.Count > 0)
            //{
            //    if (result.FeaturesAllowed.Contains(Feature.ScriptRepository))
            //    {
            AddViews();
            //        ExtendMenu();
            //        ExtendToolStrip();
            //    }
            //    else
            //        showError = true;
            //}
            //else
            //    showError = true;
            //if (showError)
            //{
            //    MessageBox.Show("Invalid License to access this feature. Please contact for right license", "Invalid License", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    System.Windows.Forms.Application.Exit();
            //    System.Environment.Exit(1);
            //}
        }

        private void ExtendToolStrip()
        {
            ToolStripButton newScriptBtn = new ToolStripButton();
            newScriptBtn.Image = new System.Drawing.Bitmap(@"Images\add-script.png");
            newScriptBtn.ToolTipText = "New Script";
            newScriptBtn.Click += new EventHandler(newScriptBtn_Click);
            this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(newScriptBtn);

            ToolStripButton editScript = new ToolStripButton();
            editScript.Image = new System.Drawing.Bitmap(@"Images\open.png");
            editScript.ToolTipText = "Open to edit an existing script";
           // editScript.Click += new EventHandler(editScript_Click);
            this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(editScript);

            ToolStripButton saveScript = new ToolStripButton();
            saveScript.Image = new System.Drawing.Bitmap(@"Images\SaveWorkflow.png");
            saveScript.ToolTipText = "Save Script Locally";
            saveScript.Click += new EventHandler(saveLocally_Click);

            this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(saveScript);
            ToolStripButton pubScript = new ToolStripButton();
            pubScript.Image = new System.Drawing.Bitmap(@"Images\publish.png");
            pubScript.ToolTipText = "Publish Script to Server";
            pubScript.Click += new EventHandler(publish_Click);
            this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(pubScript);

            this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripSeparator>(new ToolStripSeparator());

            ToolStripButton runScriptBtn = new ToolStripButton();
            runScriptBtn.Image = new System.Drawing.Bitmap(@"Images\play.png");
            runScriptBtn.ToolTipText = "Run Script Locally";
            runScriptBtn.Click += new EventHandler(runScriptBtn_Click);
            this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(runScriptBtn);

            this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripSeparator>(new ToolStripSeparator());


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

           // Showtoolbar(this, new EventArgs());
        }

        private void ExtendMenu()
        {
            ToolStripMenuItem newScript = new ToolStripMenuItem("New Script");
            newScript.Click += new EventHandler(newScript_Click);
            newScript.ShortcutKeys = Keys.Control | Keys.N;
            this.WorkItem.RootWorkItem.UIExtensionSites["FileMenu"].Add<ToolStripMenuItem>(newScript);

            ToolStripMenuItem editScript = new ToolStripMenuItem("Edit Script");
          //  editScript.Click += editScript_Click;
            editScript.ShortcutKeys = Keys.Control | Keys.O;
            this.WorkItem.RootWorkItem.UIExtensionSites["FileMenu"].Add<ToolStripMenuItem>(editScript);

            ToolStripMenuItem saveLocally = new ToolStripMenuItem("Save Script Locally");
            saveLocally.Click += saveLocally_Click;
            saveLocally.ShortcutKeys = Keys.Control | Keys.S;
            this.WorkItem.RootWorkItem.UIExtensionSites["FileMenu"].Add<ToolStripMenuItem>(saveLocally);

            ToolStripMenuItem publish = new ToolStripMenuItem("Publish Script to Server");
            publish.Click += publish_Click;
            publish.ShortcutKeys = Keys.Control | Keys.P;
            this.WorkItem.RootWorkItem.UIExtensionSites["FileMenu"].Add<ToolStripMenuItem>(publish);

            ToolStripSeparator sep = new ToolStripSeparator();
            this.WorkItem.RootWorkItem.UIExtensionSites["FileMenu"].Add<ToolStripSeparator>(sep);

            ToolStripMenuItem runLocally = new ToolStripMenuItem("Run Script Locally");
            runLocally.Click += new EventHandler(runLocally_Click);
            runLocally.ShortcutKeys = Keys.Control | Keys.R;
          //  runLocally.Enabled = false;
            this.WorkItem.RootWorkItem.UIExtensionSites["FileMenu"].Add<ToolStripMenuItem>(runLocally);


            ToolStripSeparator sep1 = new ToolStripSeparator();
            this.WorkItem.RootWorkItem.UIExtensionSites["FileMenu"].Add<ToolStripSeparator>(sep1);

            ToolStripMenuItem closeTab = new ToolStripMenuItem("Close Tab");
            closeTab.Click += closeTab_Click;
            closeTab.ShortcutKeys = Keys.Control | Keys.X;
            this.WorkItem.RootWorkItem.UIExtensionSites["FileMenu"].Add<ToolStripMenuItem>(closeTab);

            ToolStripMenuItem exit = new ToolStripMenuItem("Exit");
            exit.Click += new EventHandler(exit_Click);
            exit.ShortcutKeys = Keys.Control | Keys.Q;
            exit.ToolTipText = "Exit Application";
            this.WorkItem.RootWorkItem.UIExtensionSites["FileMenu"].Add<ToolStripMenuItem>(exit);
        }


        private ControlledWorkItem<ScriptRepoWorkItem> GetActiveControlledWorkitem()
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

        void runScriptBtn_Click(object sender, EventArgs e)
        {
            if (_current != null)
                _current.Controller.runLocally_Click(this, e);
        }

        [EventSubscription(Constants.EventTopicNames.NewScript,ThreadOption.UserInterface)]
        public void newScriptBtn_Click(object sender, EventArgs e)
        {
            if (_current != null)
            {
                _current.Controller.newScriptBtn_Click(this, e);
            }
        }

        [EventSubscription(Constants.EventTopicNames.ScriptRepositoryView, ThreadOption.UserInterface)]
        public void ScriptRepoView(object sender, EventArgs e)
        {
            if(_current != null)
                _current.Controller.ScriptRepoView();
        }

        [EventSubscription(Constants.EventTopicNames.OpenScript, ThreadOption.UserInterface)]
        public void editScript_Click(object sender, EventArgs<String> e)
        {
            if (_current != null)
                _current.Controller.Open(e.Data);
        }

        [EventSubscription(Constants.EventTopicNames.OpenScriptFromPackage, ThreadOption.UserInterface)]
        public void editScript_Click(object sender, EventArgs<PackageMeta> e)
        {
            if (_current != null)
            {
                _current.Controller.OpenFromPackage(e.Data);
                _current.Controller.PublishForPackage(e.Data);
            }
        }

        [EventSubscription(EventTopicNames.CloseTabScripts,ThreadOption.UserInterface)]
        public void closeTab_Click(object sender, EventArgs e)
        {
            if (_current != null)
                _current.Controller.closeTab_Click(this, e);
        }

        [EventSubscription(EventTopicNames.PublishScript, ThreadOption.UserInterface)]
        public void publish_Click(object sender, EventArgs e)
        {
            if (_current != null)
                _current.Controller.publish_Click(this, e);
        }



        [EventSubscription(EventTopicNames.SaveScript,ThreadOption.UserInterface)]
        public void saveLocally_Click(object sender, EventArgs e)
        {
            if (_current != null)
                _current.Controller.saveLocally_Click(this, e);
        }

        [EventSubscription(Constants.EventTopicNames.RunScripts,ThreadOption.UserInterface)]
        public void runLocally_Click(object sender, EventArgs e)
        {
            if (_current != null)
                _current.Controller.runLocally_Click(this, e);
        }

        [EventSubscription(Constants.EventTopicNames.DeleteScript, ThreadOption.UserInterface)]
        public void deleteScript_Click(object sender, EventArgs e)
        {
            if (_current != null)
                _current.Controller.deleteScript_Click(this, e);
        }

        [EventSubscription(Constants.EventTopicNames.RunScripts_View, ThreadOption.UserInterface)]
        public void run(object sender, EventArgs e)
        {
            if (_current != null)
                _current.Controller.runScriptBtn_Click(this, e);
        }

        void newScript_Click(object sender, EventArgs e)
        {
            if (_current != null)
                _current.Controller.newScript_Click(this, e);
        }

        private ScriptRepoWorkItem AddWorkitem()
        {
            var sr_name = "SR" + DateTime.Now.Ticks.ToString();
            var sr = this.WorkItem.WorkItems.AddNew<ControlledWorkItem<ScriptRepoWorkItem>>();
            sr.Activating += sr_Activating;
            sr.Activated += sr_Activated;
            sr.ID = sr_name;
            sr.Controller.Run();
            workitems.Add(sr_name, sr);
            sr.Activate();

            return sr.Controller;
        }

        void sr_Activated(object sender, EventArgs e)
        {
            _current = GetActiveControlledWorkitem();
        }

        void sr_Activating(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }


        private void AddViews()
        {
            var sr = AddWorkitem();
            sr.AddViews();
        }

        [EventSubscription(Constants.EventTopicNames.ActivateMenu, ThreadOption.UserInterface)]
        public void ActivateMenuHandler(object sender, EventArgs<bool> e)
        {
            //run
           ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[0].Enabled = 
                      

            //run
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[4]).Enabled = 

            //delete
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[2].Enabled =

            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[7]).Enabled = e.Data; ;

            ShowRun(this, new EventArgs<bool>(e.Data));
        }

       

        void exit_Click(object sender, EventArgs e)
        {
            if (_current != null)
                _current.Controller.exit_Click(sender, e);
            if (Exit != null)
                Exit(this, e);
        }

        [EventSubscription(Constants.EventTopicNames.SaveAndClose, ThreadOption.UserInterface)]
        public void SaveAndClose_handler(object sender, EventArgs<FormClosingEventArgs> e)
        {
            if (_current != null)
                _current.Controller.SaveAndClose_handler(this, e);
        }

      
    }
}

/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/

using System;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using IMSWorkBench.Infrastructure.Interface;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;

using IMSWorkBench.Infrastructure.Library.UI;
using Microsoft.Practices.CompositeUI.EventBroker;
using System.Diagnostics;
using IMSWorkBench.Infrastructure.Library.Services;
using Infosys.ATR.DevelopmentStudio;

using Infosys.ATR.Editor.Views;
using Infosys.ATR.Editor.Views.Create;
using Infosys.ATR.Editor.Services;
using Infosys.LicenseValidationClient;

namespace Infosys.ATR.Editor
{
    public class ModuleController : WorkItemController
    {

        Dictionary<String, ControlledWorkItem<EditorWorkItem>> workitems
            = new Dictionary<string, ControlledWorkItem<EditorWorkItem>>();

        ControlledWorkItem<EditorWorkItem> _current = null;


        [EventPublication(Constants.EventTopicNames.Exit, PublicationScope.Global)]
        public event EventHandler Exit;

        public override void Run()
        {
            //ValidationResult result = Infosys.LicenseValidationClient.Validator.Validate();
            //bool showError = false;
            //if (result.IsSuccess && result.FeaturesAllowed != null && result.FeaturesAllowed.Count > 0)
            //{
            //    if (result.FeaturesAllowed.Contains(Feature.ObjectModelExplorer))
            //    {
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

        private EditorWorkItem AddWorkitem()
        {
            var ewi_name = "EWI" + DateTime.Now.Ticks.ToString();
            var ewi = this.WorkItem.WorkItems.AddNew<ControlledWorkItem<EditorWorkItem>>();
            ewi.ID = ewi_name;
            ewi.Controller.Run();
            workitems.Add(ewi_name, ewi);
            ewi.Activated += new EventHandler(ewi_Activated);
            return ewi.Controller;
        }

        void ewi_Activated(object sender, EventArgs e)
        {
            _current = GetActiveControlledWorkitem();
        }

        private ControlledWorkItem<EditorWorkItem> GetActiveControlledWorkitem()
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

        private EditorWorkItem GetActiveWorkitem()
        {
            foreach (KeyValuePair<string, WorkItem> wi in WorkItem.WorkItems)
            {
                if (wi.Value.Status == WorkItemStatus.Active)
                {
                    var ewi = wi.Value;
                    return workitems[ewi.ID].Controller;
                }
            }

            return null;
        }

        private void ExtendMenu()
        {
            ToolStripMenuItem newFile = new ToolStripMenuItem("New");
            newFile.Click += new EventHandler(fileOpenBtn_Click);
            newFile.ShortcutKeys = Keys.Control | Keys.N;
            newFile.Image = new System.Drawing.Bitmap(@"Images\new.png");
            newFile.ToolTipText = "Start a new automation";
            this.WorkItem.RootWorkItem.UIExtensionSites["FileMenu"].Add<ToolStripMenuItem>(newFile);

            ToolStripMenuItem edit = new ToolStripMenuItem("Open");
            edit.Click += new EventHandler(fileEditBtn_Click);
            edit.ShortcutKeys = Keys.Control | Keys.O;
            edit.Image = new System.Drawing.Bitmap(@"Images\edit.png");
            edit.ToolTipText = "Open a saved configuration";
            this.WorkItem.RootWorkItem.UIExtensionSites["FileMenu"].Add<ToolStripMenuItem>(edit);

            ToolStripMenuItem save = new ToolStripMenuItem("Save");
            save.Click += new EventHandler(saveBtn_Click);
            save.ShortcutKeys = Keys.Control | Keys.S;
            save.Image = new System.Drawing.Bitmap(@"Images\save.png");
            save.ToolTipText = "Save the automation created";
            this.WorkItem.RootWorkItem.UIExtensionSites["FileMenu"].Add<ToolStripMenuItem>(save);

            ToolStripMenuItem close = new ToolStripMenuItem("Close Tab");
            close.Click += new EventHandler(close_Click);
            close.ShortcutKeys = Keys.Control | Keys.X;
            close.ToolTipText = "Close current tab";
            this.WorkItem.RootWorkItem.UIExtensionSites["FileMenu"].Add<ToolStripMenuItem>(close);


            ToolStripMenuItem exit = new ToolStripMenuItem("Exit");
            exit.Click += new EventHandler(exit_Click);
            exit.ShortcutKeys = Keys.Control | Keys.Q;
            exit.ToolTipText = "Exit the application";
            this.WorkItem.RootWorkItem.UIExtensionSites["FileMenu"].Add<ToolStripMenuItem>(exit);

            ToolStripMenuItem objectModel = new ToolStripMenuItem("ObjectTree");
            this.WorkItem.RootWorkItem.UIExtensionSites["MainMenu"].Add<ToolStripMenuItem>(objectModel);

            ToolStripMenuItem del = new ToolStripMenuItem("Delete");
            del.Click += new EventHandler(delBtn_Click);
            del.ShortcutKeys = Keys.Control | Keys.D;
            del.Image = new System.Drawing.Bitmap(@"Images\delete.png");
            del.ToolTipText = "Deletes the selected node and it's children";
            objectModel.DropDownItems.Add(del);

            ToolStripMenuItem script = new ToolStripMenuItem("Script");
            script.Click += new EventHandler(genScriptBtn_Click);
            script.ShortcutKeys = Keys.Control | Keys.R;
            script.Image = new System.Drawing.Bitmap(@"Images\generate-script.png");
            script.ToolTipText = "Generates Python script";
            objectModel.DropDownItems.Add(script);

            ToolStripMenuItem python = new ToolStripMenuItem("Python");
            this.WorkItem.RootWorkItem.UIExtensionSites["MainMenu"].Add<ToolStripMenuItem>(python);

            ToolStripMenuItem pyNew = new ToolStripMenuItem("New");
            pyNew.Click += new EventHandler(pythonBtn_Click);
            pyNew.ShortcutKeys = Keys.Control | Keys.Shift | Keys.N;
            pyNew.Image = new System.Drawing.Bitmap(@"Images\python.png");
            pyNew.ToolTipText = "Launch Python Editor";
            python.DropDownItems.Add(pyNew);

            ToolStripMenuItem pyOpen = new ToolStripMenuItem("Open");
            pyOpen.Click += new EventHandler(pythonOpenBtn_Click);
            pyOpen.ShortcutKeys = Keys.Control | Keys.Shift | Keys.O;
            pyOpen.Image = new System.Drawing.Bitmap(@"Images\add.png");
            pyOpen.ToolTipText = "Open Python file";
            python.DropDownItems.Add(pyOpen);

            ToolStripMenuItem pySave = new ToolStripMenuItem("Save");
            pySave.ToolTipText = "Save Python file";
            pySave.Image = new System.Drawing.Bitmap(@"Images\savepython.png");
            pySave.Click += new EventHandler(pythonSaveBtn_Click);
            pySave.ShortcutKeys = Keys.Control | Keys.Shift | Keys.S;
            python.DropDownItems.Add(pySave);

            ToolStripMenuItem pyRun = new ToolStripMenuItem("Run");
            pyRun.Click += new EventHandler(runBatBtn_Click);
            pyRun.ShortcutKeys = Keys.Control | Keys.Shift | Keys.R;
            pyRun.Image = new System.Drawing.Bitmap(@"Images\FormRunHS.png");
            pyRun.ToolTipText = "Execute Python Script";
            python.DropDownItems.Add(pyRun);

            ToolStripMenuItem pySettings = new ToolStripMenuItem("Settings");
            pySettings.Click += new EventHandler(pySettings_Click);
            pySettings.ShortcutKeys = Keys.Control | Keys.Shift | Keys.T;
            pySettings.ToolTipText = "Python Settings";
            python.DropDownItems.Add(pySettings);

        }

        private void ExtendToolStrip()
        {
            ToolStripButton fileOpenBtn = new ToolStripButton();
            fileOpenBtn.ToolTipText = "Start a new automation";
            fileOpenBtn.Image = new System.Drawing.Bitmap(@"Images\new.png");
            fileOpenBtn.Click += new EventHandler(fileOpenBtn_Click);
            this.WorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(fileOpenBtn);

            ToolStripButton fileEditBtn = new ToolStripButton();
            fileEditBtn.ToolTipText = "Open a saved configuration";
            fileEditBtn.Image = new System.Drawing.Bitmap(@"Images\edit.png");
            fileEditBtn.Click += new EventHandler(fileEditBtn_Click);
            this.WorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(fileEditBtn);

            ToolStripButton saveBtn = new ToolStripButton();
            saveBtn.ToolTipText = "Save the automation created";
            saveBtn.Image = new System.Drawing.Bitmap(@"Images\save.png");
            saveBtn.Click += new EventHandler(saveBtn_Click);
            this.WorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(saveBtn);

            ToolStripSeparator sep = new ToolStripSeparator();
            this.WorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripSeparator>(sep);

            ToolStripButton delBtn = new ToolStripButton();
            delBtn.ToolTipText = "Deletes the selected node and it's children";
            delBtn.Image = new System.Drawing.Bitmap(@"Images\delete.png");
            delBtn.Click += new EventHandler(delBtn_Click);
            this.WorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(delBtn);

            ToolStripButton genScriptBtn = new ToolStripButton();
            genScriptBtn.ToolTipText = "Generates Python script";
            genScriptBtn.Image = new System.Drawing.Bitmap(@"Images\generate-script.png");
            genScriptBtn.Click += new EventHandler(genScriptBtn_Click);
            this.WorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(genScriptBtn);

            ToolStripSeparator sep1 = new ToolStripSeparator();
            this.WorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripSeparator>(sep1);

            ToolStripButton pythonBtn = new ToolStripButton();
            pythonBtn.ToolTipText = "Launch Python Editor";
            pythonBtn.Image = new System.Drawing.Bitmap(@"Images\python.png");
            pythonBtn.Click += new EventHandler(pythonBtn_Click);
            this.WorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(pythonBtn);

            ToolStripButton pythonOpenBtn = new ToolStripButton();
            pythonOpenBtn.ToolTipText = "Open Python file";
            pythonOpenBtn.Image = new System.Drawing.Bitmap(@"Images\add.png");
            pythonOpenBtn.Click += new EventHandler(pythonOpenBtn_Click);
            this.WorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(pythonOpenBtn);

            ToolStripButton pythonSaveBtn = new ToolStripButton();
            pythonSaveBtn.ToolTipText = "Save Python file";
            pythonSaveBtn.Image = new System.Drawing.Bitmap(@"Images\savepython.png");
            pythonSaveBtn.Click += new EventHandler(pythonSaveBtn_Click);
            this.WorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(pythonSaveBtn);

            ToolStripButton runBatBtn = new ToolStripButton();
            runBatBtn.ToolTipText = "Execute Python Script";
            runBatBtn.Image = new System.Drawing.Bitmap(@"Images\FormRunHS.png");
            runBatBtn.Click += new EventHandler(runBatBtn_Click);
            this.WorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(runBatBtn);


        }

        void exit_Click(object sender, EventArgs e)
        {
            if (_current != null)
                _current.Controller.exit_Click(sender, e);
            if (Exit != null)
                Exit(this, e);
        }

        void pySettings_Click(object sender, EventArgs e)
        {
            _current.Controller.pySettings_Click(sender, e);
        }

        [EventSubscription(Constants.EventTopicNames.   CloseObjModelTab,ThreadOption.UserInterface)]
        public void close_Click(object sender, EventArgs e)
        {
            if (_current != null)
                _current.Controller.close_Click(sender, e);
            else
            {
                var objModel = GetActiveControlledWorkitem();
                
                objModel.Controller.close_Click(sender, e);
            }

        }

        [EventSubscription(Constants.EventTopicNames.Replay, ThreadOption.UserInterface)]
        public void replay_Click(object sender, EventArgs e)
        {
            if (_current != null)
                _current.Controller.replay_click(sender, e);

        }

        void genScriptBtn_Click(object sender, EventArgs e)
        {

            _current.Controller.genScriptBtn_Click(sender, e);
        }

        void runBatBtn_Click(object sender, EventArgs e)
        {

            _current.Controller.runBatBtn_Click(sender, e);
        }

        void pythonSaveBtn_Click(object sender, EventArgs e)
        {
            _current.Controller.pythonSaveBtn_Click(sender, e);
        }

        void pythonOpenBtn_Click(object sender, EventArgs e)
        {
            var ewi = AddWorkitem();
            ewi.pythonOpenBtn_Click(sender, e);
        }

        void pythonBtn_Click(object sender, EventArgs e)
        {
            var ewi = AddWorkitem();
            ewi.pythonBtn_Click(sender, e);
        }

        [EventSubscription(Constants.EventTopicNames.NewObjModel,ThreadOption.UserInterface)]
        public void fileOpenBtn_Click(object sender, EventArgs e)
        {
            var ewi = AddWorkitem();

            ewi.fileOpenBtn_Click(sender, e);
        }

        [EventSubscription(Constants.EventTopicNames.DeleteNode,ThreadOption.UserInterface)]
        public void delBtn_Click(object sender, EventArgs e)
        {
            _current.Controller.delBtn_Click(sender, e);
        }

        [EventSubscription(Constants.EventTopicNames.OpenAtr, ThreadOption.UserInterface)]
        public void Open(object sender, EventArgs<String> e)
        {
            var ewi = AddWorkitem();
            ewi.Open(e.Data);
        }

        void fileEditBtn_Click(object sender, EventArgs e)
        {
            var ewi = AddWorkitem();
            ewi.fileEditBtn_Click(sender, e);
        }

        [EventSubscription(Constants.EventTopicNames.SaveAtr,ThreadOption.UserInterface)]
        public void saveBtn_Click(object sender, EventArgs e)
        {
            _current.Controller.saveBtn_Click(sender, e);
        }

        [EventSubscription(Constants.EventTopicNames.SaveAndClose, ThreadOption.UserInterface)]
        public void SaveAndClose_handler(object sender, EventArgs<FormClosingEventArgs> e)
        {
            if (_current != null)
                _current.Controller.SaveAndClose_handler(this, e);


        }
    }
}

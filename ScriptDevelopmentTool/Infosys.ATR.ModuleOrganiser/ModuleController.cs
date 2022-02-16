/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Drawing;
using System.Configuration;
using System.Windows.Forms;

using IMSWorkBench.Infrastructure.Interface;
using IMSWorkBench.Infrastructure.Library.Services;
using Infosys.LicenseValidationClient;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.CompositeUI.Services;
using Infosys.IAP.CommonClientLibrary;
using Infosys.IAP.CommonClientLibrary.Models;

namespace Infosys.ATR.ModuleOrganiser
{
    public enum Workspace
    {
        Editor,
        MainRepositoryView,
        Explorer,
        WFDesigner,
        PythonIDE,
        WFSelector,
        ScriptDesigner,
        TransactionView,
        RecordingView,
        ExportScripts
    }

    public enum FileXtn
    {
        XAML,
        ATR
    }


    public enum Mode
    {
        Online,
        Offline
    }

    public class ModuleController : WorkItemController
    {
        bool _enableWF, _enableOM, _enableScripts, _enableatstart;
        bool _enablewfExplorer = true;
        bool _enableadmin = true;
        bool _enablepublish = true;
        Mode _mode;
        string _wffilter = "Windows Markup File|*.xaml";
        string _omfilter = "ATR File|*.atr";
        string _scriptFilter = "Script File|*.bat;*.vbs;*.iap;*.py;*.js;*.ps1;*.iapd;*.sh;";
        string _scriptFilterForSecureTrans = "Script File|*.sh;*.ps1";
        string _filter;
        string _noLicense = "You do not have license to use this module";
        string _packagefilter = "Package File|*.iapl";


        #region -- workflow events--
        [EventPublication(Constants.EventTopicNames.NeWWorkflow, PublicationScope.Global)]
        public event EventHandler NewWorkflow;

        [EventPublication(Constants.EventTopicNames.OpenWorkflow, PublicationScope.Global)]
        public event EventHandler<EventArgs<String>> OpenWorkflow;

        [EventPublication(Constants.EventTopicNames.SaveWorkflow, PublicationScope.Global)]
        public event EventHandler SaveWorkflow;

        [EventPublication(Constants.EventTopicNames.CloseWFTab, PublicationScope.Global)]
        public event EventHandler CloseWFTab;

        [EventPublication(Constants.EventTopicNames.CloseRecordingTab, PublicationScope.Global)]
        public event EventHandler CloseRecordingTab;

        [EventPublication(Constants.EventTopicNames.RunWF, PublicationScope.Global)]
        public event EventHandler RunWF;

        [EventPublication(Constants.EventTopicNames.RunWFSelector, PublicationScope.Global)]
        public event EventHandler RunWFSelector;

        [EventPublication(Constants.EventTopicNames.PublishWF, PublicationScope.Global)]
        public event EventHandler PublishWF;

        [EventPublication(Constants.EventTopicNames.WFView, PublicationScope.Global)]
        public event EventHandler WFView;

        [EventPublication(Constants.EventTopicNames.DeleteWF, PublicationScope.Global)]
        public event EventHandler DeleteWF;

        [EventPublication(Constants.EventTopicNames.OpenWFFromPackage, PublicationScope.Global)]
        public event EventHandler OpenWFFromPackage;



        #endregion

        #region --object model events--
        [EventPublication(Constants.EventTopicNames.NewObjModel, PublicationScope.Global)]
        public event EventHandler NewObjModel;

        [EventPublication(Constants.EventTopicNames.OpenAtr, PublicationScope.Global)]
        public event EventHandler<EventArgs<String>> OpenAtr;

        [EventPublication(Constants.EventTopicNames.SaveAtr, PublicationScope.Global)]
        public event EventHandler SaveAtr;

        [EventPublication(Constants.EventTopicNames.CloseObjModelTab, PublicationScope.Global)]
        public event EventHandler CloseObjModelTab;

        [EventPublication(Constants.EventTopicNames.DeleteNode, PublicationScope.Global)]
        public event EventHandler DeleteNode;

        [EventPublication(Constants.EventTopicNames.Replay, PublicationScope.Global)]
        public event EventHandler Replay;
        #endregion

        #region -- scripts events --

        [EventPublication(Constants.EventTopicNames.NewScript, PublicationScope.Global)]
        public event EventHandler NewScript;

        [EventPublication(Constants.EventTopicNames.OpenScript, PublicationScope.Global)]
        public event EventHandler OpenScript;

        [EventPublication(Constants.EventTopicNames.OpenScriptFromPackage, PublicationScope.Global)]
        public event EventHandler OpenScriptFromPackage;

        [EventPublication(Constants.EventTopicNames.SaveScript, PublicationScope.Global)]
        public event EventHandler SaveScript;

        [EventPublication(Constants.EventTopicNames.RunScripts, PublicationScope.Global)]
        public event EventHandler RunScripts;

        [EventPublication(Constants.EventTopicNames.ExportScriptsView, PublicationScope.Global)]
        public event EventHandler ExportScriptsView;

        [EventPublication(Constants.EventTopicNames.RunScripts_View, PublicationScope.Global)]
        public event EventHandler RunScripts_View;

        [EventPublication(Constants.EventTopicNames.ScriptRepositoryView, PublicationScope.Global)]
        public event EventHandler ScriptRepositoryView;

        [EventPublication(Constants.EventTopicNames.CloseTabScripts, PublicationScope.Global)]
        public event EventHandler CloseTabScripts;

        [EventPublication(Constants.EventTopicNames.PublishScript, PublicationScope.Global)]
        public event EventHandler PublishScript;

        [EventPublication(Constants.EventTopicNames.DeleteScript, PublicationScope.Global)]
        public event EventHandler DeleteScript;

        [EventPublication(Constants.EventTopicNames.GeneratePlaybackScript, PublicationScope.Global)]
        public event EventHandler GeneratePlaybackScript;

        [EventPublication(Constants.EventTopicNames.RunPlaybackScript, PublicationScope.Global)]
        public event EventHandler RunPlaybackScript;


        #endregion

        #region -- admin events--
        [EventPublication(Constants.EventTopicNames.AdminView, PublicationScope.Global)]
        public event EventHandler AdminView;
        [EventPublication(Constants.EventTopicNames.CloseAdminView, PublicationScope.Global)]
        public event EventHandler CloseAdminView;
        [EventPublication(Constants.EventTopicNames.DeleteAdminNode, PublicationScope.Global)]
        public event EventHandler DeleteAdminNode;
        [EventPublication(Constants.EventTopicNames.GroupExplorer, PublicationScope.Global)]
        public event EventHandler GroupExplorer;
        #endregion

        #region -- TransactionView--
        [EventPublication(Constants.EventTopicNames.TransactionView, PublicationScope.Global)]
        public event EventHandler TransactionView;
        #endregion

        #region -- Recording--
        [EventPublication(Constants.EventTopicNames.RecordingView, PublicationScope.Global)]
        public event EventHandler RecordingView;
        #endregion


        #region -- RecorderView--
        [EventPublication(Constants.EventTopicNames.RecorderView, PublicationScope.Global)]
        public event EventHandler RecorderView;
        #endregion

        #region -- RecorderView--
        [EventPublication(Constants.EventTopicNames.MinimizeShell, PublicationScope.Global)]
        public event EventHandler MinimizeShell;
        #endregion


        #region -- generic events --

        [EventPublication(Constants.EventTopicNames.Exit, PublicationScope.Global)]
        public event EventHandler Exit;

        [EventPublication(Constants.EventTopicNames.LoadGeneric, PublicationScope.Global)]
        public event EventHandler LoadGeneric;

        [EventPublication(Constants.EventTopicNames.ShowUserLogged, PublicationScope.Global)]
        public event EventHandler<EventArgs<String>> ShowUserLogged;

        [EventPublication(Constants.EventTopicNames.ToggleDeckPanel, PublicationScope.Global)]
        public event EventHandler<EventArgs<bool>> ToggleDeckPanel;
        #endregion

        public override void Run()
        {
            License();
            CheckRunningMode();
            ExtendMenu();
            ExtendToolStrip();

            //initialize WF toolbox

            var instance = WFServices.Instance;
            instance.InitialiseToolbox();

            instance.LoadAssembly();

            LoadGeneric(this, new EventArgs());
            ShowUserLogged(this, new EventArgs<string>(System.Threading.Thread.CurrentPrincipal.Identity.Name));
        }

        private void CheckRunningMode()
        {
            _mode = (Mode)Enum.Parse(typeof(Mode), this.WorkItem.RootWorkItem.State["Mode"].ToString());

            if (_mode == Mode.Offline)
            {
                //_enableScripts = false;
                //_filter = _wffilter + "|" + _omfilter;
                _enablewfExplorer = false;
                //_enablepublish = true;
                _enableadmin = false;
            }
        }

        private void License()
        {
            ValidationResult result = Validator.Validate();

            if (result.IsSuccess && result.FeaturesAllowed != null && result.FeaturesAllowed.Count > 0)
            {

                if (result.FeaturesAllowed.Contains(Feature.WorkflowDesigner))
                {
                    _enableWF = true;
                    _filter = _wffilter + "|";

                }
                _filter += _packagefilter + "|";
                if (result.FeaturesAllowed.Contains(Feature.ObjectModelExplorer))
                {
                    _enableOM = true;
                    _filter += _omfilter + "|";
                }
                if (result.FeaturesAllowed.Contains(Feature.ScriptRepository))
                {
                    _enableScripts = true;

                    string mode = ConfigurationManager.AppSettings["Mode"];

                    if (mode.ToLower().Equals("offline"))
                        _filter += _scriptFilter;
                    else if (Infosys.WEM.Client.CommonServices.Instance.EnableSecureTransactions)
                        _filter += _scriptFilterForSecureTrans;
                    else
                        _filter += _scriptFilter;
                }


                var c = _filter.Substring(_filter.Length - 1, 1);
                if (c == "|")
                {
                    _filter = _filter.Remove(_filter.Length, 1);
                }
            }
            else
            {

                _noLicense = "You do not have license to run this Application. Contact Administrator";

                //ShowNoLicense();
                throw new Exception(_noLicense);
            }
        }

        private void ExtendToolStrip()
        {
            ToolStripButton newBtn = new ToolStripButton();
            newBtn.Image = new System.Drawing.Bitmap(@"Images\add-script.png");
            newBtn.ToolTipText = "Start a new automation module";
            newBtn.Enabled = _enableOM | _enableScripts | _enableWF;
            newBtn.Click += new EventHandler(newFile_Click);
            this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(newBtn);

            ToolStripButton openBtn = new ToolStripButton();
            openBtn.Image = new System.Drawing.Bitmap(@"Images\open.png");
            openBtn.ToolTipText = "Open a Workflow/Script/Object Model file to edit";
            openBtn.Click += new EventHandler(open_Click);
            openBtn.Enabled = _enableOM | _enableScripts | _enableWF;
            this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(openBtn);

            ToolStripButton saveBtn = new ToolStripButton();
            saveBtn.Image = new System.Drawing.Bitmap(@"Images\save.png");
            saveBtn.ToolTipText = "Save a Workflow/Script/Object Model file locally";
            saveBtn.Enabled = (_enableOM | _enableScripts | _enableWF) & _enableatstart;
            saveBtn.Click += save_Click;
            this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(saveBtn);

            this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripSeparator>(new ToolStripSeparator());

            ToolStripButton runBtn = new ToolStripButton();
            runBtn.Image = new System.Drawing.Bitmap(@"Images\play.png");
            runBtn.ToolTipText = "Run a Workflow/Script";
            runBtn.Click += new EventHandler(run_Click);
            runBtn.Enabled = (_enableScripts | _enableWF) & _enableatstart;
            this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(runBtn);

            ToolStripButton publishBtn = new ToolStripButton();
            publishBtn.Image = new System.Drawing.Bitmap(@"Images\publish.png");
            publishBtn.ToolTipText = "Publish Workflow/Script/Object Model to server";
            publishBtn.Click += new EventHandler(publish_Click);
            publishBtn.Enabled = (_enableOM | _enableScripts | _enableWF) & (_enablepublish) & (_enableatstart);
            this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(publishBtn);

            ToolStripButton grBtn = new ToolStripButton();
            grBtn.Image = new System.Drawing.Bitmap(@"Images\groups.jpg");
            grBtn.ToolTipText = "Manage Groups";
            grBtn.Click += new EventHandler(grExplorer_Click);
            grBtn.Enabled = _enableadmin & _enableatstart;
            this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(grBtn);

            ToolStripButton delBtn = new ToolStripButton();
            delBtn.Image = new System.Drawing.Bitmap(@"Images\delete.png");
            delBtn.ToolTipText = "Delete selected node and all it's children from tree";
            delBtn.Click += new EventHandler(delete_Click);
            delBtn.Enabled = _enableadmin & _enableatstart;
            this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(delBtn);

            ToolStripButton generatePlaybackBtn = new ToolStripButton();
            generatePlaybackBtn.Image = new System.Drawing.Bitmap(@"Images\generate-script.png");
            generatePlaybackBtn.ToolTipText = "Generate the Playback Script";
            generatePlaybackBtn.Click += new EventHandler(GeneratePlaybackScript_Click);
            generatePlaybackBtn.Enabled = _enableatstart;
            this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(generatePlaybackBtn);

            this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripSeparator>(new ToolStripSeparator());

            ToolStripButton wfViewBtn = new ToolStripButton();
            wfViewBtn.Image = new System.Drawing.Bitmap(@"Images\wf_icon.jpg");
            wfViewBtn.ToolTipText = "Workflow Explorer View";
            wfViewBtn.Click += new EventHandler(wfView_Click);
            wfViewBtn.Enabled = _enableWF & _enablewfExplorer;
            this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(wfViewBtn);

            ToolStripButton scriptsViewBtn = new ToolStripButton();
            scriptsViewBtn.Image = new System.Drawing.Bitmap(@"Images\scripts_icon.jpg");
            scriptsViewBtn.ToolTipText = "Script Repository View";
            scriptsViewBtn.Click += new EventHandler(scriptView_Click);
            scriptsViewBtn.Enabled = (_mode == Mode.Offline) ? false : _enableScripts;
            this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(scriptsViewBtn);

            ToolStripButton adminViewBtn = new ToolStripButton();
            adminViewBtn.Image = new System.Drawing.Bitmap(@"Images\admin_icon.jpg");
            adminViewBtn.ToolTipText = "Admin View";
            adminViewBtn.Click += new EventHandler(adView_Click);
            adminViewBtn.Enabled = _enableadmin;
            this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(adminViewBtn);

            ToolStripButton TranViewBtn = new ToolStripButton();
            TranViewBtn.Image = new System.Drawing.Bitmap(@"Images\Application.png");
            TranViewBtn.ToolTipText = "WF/Script Transaction History View";
            TranViewBtn.Click += new EventHandler(TransactionView_Click);
            TranViewBtn.Enabled = (_mode == Mode.Offline) ? false : _enableScripts; ;
            this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(TranViewBtn);

            ToolStripButton RecordingViewBtn = new ToolStripButton();
            RecordingViewBtn.Image = new System.Drawing.Bitmap(@"Images\Control_Panel.png");
            RecordingViewBtn.ToolTipText = "Recording History View";
            RecordingViewBtn.Click += new EventHandler(RecordingView_Click);
            RecordingViewBtn.Enabled = _enableScripts; ;
            this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(RecordingViewBtn);

            this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripSeparator>(new ToolStripSeparator());

            ToolStripButton closeTabBtn = new ToolStripButton();
            closeTabBtn.Image = new System.Drawing.Bitmap(@"Images\close.gif");
            closeTabBtn.ToolTipText = "Close Current Tab";
            closeTabBtn.Click += new EventHandler(closeTab_Click);
            closeTabBtn.Enabled = _enableatstart;
            this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(closeTabBtn);
        }

        public void ExtendMenu()
        {
            ToolStripMenuItem newFile = new ToolStripMenuItem("New");
            newFile.Click += newFile_Click;
            newFile.ShortcutKeys = Keys.Control | Keys.N;
            newFile.ToolTipText = "Start a new automation module";
            newFile.Enabled = (_enableOM | _enableScripts | _enableWF);
            this.WorkItem.RootWorkItem.UIExtensionSites["FileMenu"].Add<ToolStripMenuItem>(newFile);

            ToolStripMenuItem open = new ToolStripMenuItem("Open");
            open.Click += open_Click;
            open.ShortcutKeys = Keys.Control | Keys.O;
            open.ToolTipText = "Open a Workflow/Script/Object Model file to edit";
            open.Enabled = _enableOM | _enableScripts | _enableWF;
            this.WorkItem.RootWorkItem.UIExtensionSites["FileMenu"].Add<ToolStripMenuItem>(open);

            ToolStripMenuItem save = new ToolStripMenuItem("Save");
            save.Click += save_Click;
            save.ShortcutKeys = Keys.Control | Keys.S;
            save.ToolTipText = "Save a Workflow/Script/Object Model file locally";
            save.Enabled = (_enableOM | _enableScripts | _enableWF) & _enableatstart;
            this.WorkItem.RootWorkItem.UIExtensionSites["FileMenu"].Add<ToolStripMenuItem>(save);

            this.WorkItem.RootWorkItem.UIExtensionSites["FileMenu"].Add<ToolStripSeparator>(new ToolStripSeparator());

            ToolStripMenuItem closeTab = new ToolStripMenuItem("Close Tab");
            closeTab.Click += closeTab_Click;
            closeTab.ShortcutKeys = Keys.Control | Keys.W;
            closeTab.ToolTipText = "Close current tab";
            closeTab.Enabled = _enableatstart;
            this.WorkItem.RootWorkItem.UIExtensionSites["FileMenu"].Add<ToolStripMenuItem>(closeTab);

            this.WorkItem.RootWorkItem.UIExtensionSites["FileMenu"].Add<ToolStripSeparator>(new ToolStripSeparator());

            ToolStripMenuItem exit = new ToolStripMenuItem("Exit");
            exit.Click += exit_Click;
            exit.ShortcutKeys = Keys.Control | Keys.Q;
            exit.ToolTipText = "Exit application";
            this.WorkItem.RootWorkItem.UIExtensionSites["FileMenu"].Add<ToolStripMenuItem>(exit);

            ToolStripMenuItem actions = new ToolStripMenuItem("Actions");
            this.WorkItem.RootWorkItem.UIExtensionSites["MainMenu"].Add<ToolStripMenuItem>(actions);

            ToolStripMenuItem run = new ToolStripMenuItem("Run");
            run.Click += run_Click;
            run.ShortcutKeys = Keys.Control | Keys.R;
            run.ToolTipText = "Run a Workflow/Script";
            run.Enabled = (_enableScripts | _enableWF) & _enableatstart;
            actions.DropDownItems.Add(run);

            //ToolStripMenuItem export = new ToolStripMenuItem("Export Script");
            //export.Click += Export_Click;
            //export.ShortcutKeys = Keys.Control | Keys.R;
            //export.ToolTipText = "Export Script";
            //export.Enabled = true; //(_enableScripts | _enableWF) & _enableatstart;
            //actions.DropDownItems.Add(export);

            ToolStripMenuItem publish = new ToolStripMenuItem("Publish / Export");
            publish.Click += publish_Click;
            publish.ShortcutKeys = Keys.Control | Keys.P;
            publish.ToolTipText = "Publish Workflow/Script/Object Model to server";
            publish.Enabled = (_enableOM | _enableScripts | _enableWF) & (_enablepublish) & (_enableatstart);
            actions.DropDownItems.Add(publish);

            ToolStripMenuItem delete = new ToolStripMenuItem("Delete Node");
            delete.Click += delete_Click;
            delete.ShortcutKeys = Keys.Control | Keys.D;
            delete.ToolTipText = "Delete selected node and all it's children from tree";
            delete.Enabled = _enableatstart;
            actions.DropDownItems.Add(delete);

            ToolStripMenuItem grExplorer = new ToolStripMenuItem("Group Manager");
            grExplorer.Click += grExplorer_Click;
            grExplorer.ShortcutKeys = Keys.Control | Keys.G;
            grExplorer.ToolTipText = "Manage Groups";
            grExplorer.Enabled = _enableadmin & _enableatstart;
            actions.DropDownItems.Add(grExplorer);
            actions.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem addAssembly = new ToolStripMenuItem("Load Assembly");
            addAssembly.Click += addAssembly_Click;
            addAssembly.ShortcutKeys = Keys.Control | Keys.E;
            addAssembly.ToolTipText = "Load any assembly to the current App Domain";
            addAssembly.Enabled = (_enableWF & _enableatstart);
            actions.DropDownItems.Add(addAssembly);

            ToolStripMenuItem genrateScript = new ToolStripMenuItem("Generate Playback Script");
            genrateScript.Click += GeneratePlaybackScript_Click;
            genrateScript.ShortcutKeys = Keys.Control | Keys.B;
            genrateScript.ToolTipText = "Generates the Playback Script";
            genrateScript.Enabled = _enableatstart;
            actions.DropDownItems.Add(genrateScript);

            ToolStripMenuItem replayScript = new ToolStripMenuItem("Run the Playback Script");
            replayScript.Click += RunPlaybackScript_Click;
            replayScript.ShortcutKeys = Keys.Control | Keys.Shift | Keys.R;
            replayScript.ToolTipText = "Run the Playback Script";
            replayScript.Enabled = _enableatstart;
            actions.DropDownItems.Add(replayScript);

            //ToolStripMenuItem exportIapd = new ToolStripMenuItem("Export as Iapd");
            //exportIapd.Click += ExportIapd_Click;
            //exportIapd.ShortcutKeys = Keys.Control | Keys.E;
            //exportIapd.ToolTipText = "Export as iapd package";
            //exportIapd.Enabled = (_enableOM | _enableScripts | _enableWF) & (_enablepublish) & (_enableatstart);
            //actions.DropDownItems.Add(exportIapd);


            ToolStripMenuItem views = new ToolStripMenuItem("Views");
            this.WorkItem.RootWorkItem.UIExtensionSites["MainMenu"].Add<ToolStripMenuItem>(views);

            ToolStripMenuItem wfView = new ToolStripMenuItem("Workflow Repository");
            wfView.Click += wfView_Click;
            wfView.ShortcutKeys = Keys.Control | Keys.F;
            wfView.ToolTipText = "Workflow Explorer View";
            wfView.Enabled = _enableWF & _enablewfExplorer;
            views.DropDownItems.Add(wfView);

            ToolStripMenuItem scriptView = new ToolStripMenuItem("Scripts Repository");
            scriptView.Click += scriptView_Click;
            scriptView.ShortcutKeys = Keys.Control | Keys.I;
            scriptView.ToolTipText = "Script Repository View";
            scriptView.Enabled = (_mode == Mode.Offline) ? false : _enableScripts;
            views.DropDownItems.Add(scriptView);
            views.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem exportScriptsView = new ToolStripMenuItem("Export Scripts");
            exportScriptsView.Click += ExportScriptsView_Click;
            exportScriptsView.ShortcutKeys = Keys.Control | Keys.I;
            exportScriptsView.ToolTipText = "Export Scripts View";
            exportScriptsView.Enabled = (_mode == Mode.Offline) ? false : _enableScripts;
            views.DropDownItems.Add(exportScriptsView);
            views.DropDownItems.Add(new ToolStripSeparator());


            ToolStripMenuItem tools = new ToolStripMenuItem("Tools");
            this.WorkItem.RootWorkItem.UIExtensionSites["MainMenu"].Add<ToolStripMenuItem>(tools);

            ToolStripMenuItem getSecureText = new ToolStripMenuItem("Secure Text");
            getSecureText.Click += GetSecureText_Click;
            getSecureText.ShortcutKeys = Keys.Control | Keys.T;
            getSecureText.ToolTipText = "Get Secure Text";
            getSecureText.Enabled = true; //(_enableWF & _enableatstart);
            tools.DropDownItems.Add(getSecureText);


            ToolStripMenuItem export = new ToolStripMenuItem("Export Script");
            export.Click += Export_Click;
            export.ShortcutKeys = Keys.Control | Keys.E;
            export.ToolTipText = "Export Script";
            export.Enabled = true; //(_enableScripts | _enableWF) & _enableatstart;
            tools.DropDownItems.Add(export);

            //ToolStripMenuItem exportScript = new ToolStripMenuItem("Export Script");
            //exportScript.Click += ExportScript_Click;
            //exportScript.ShortcutKeys = Keys.Control | Keys.T;
            //exportScript.ToolTipText = "Export Script";
            //exportScript.Enabled = true; //(_enableWF & _enableatstart);
            //tools.DropDownItems.Add(exportScript);

            //tools.DropDownItems.Add(new ToolStripSeparator());

            //ToolStripMenuItem replay = new ToolStripMenuItem("Replay");
            //replay.Click += replay_Click;
            //replay.ShortcutKeys = Keys.Control | Keys.Y;
            //replay.ToolTipText = "Replay";
            //replay.Enabled = true; 
            //tools.DropDownItems.Add(replay);



            ToolStripMenuItem adView = new ToolStripMenuItem("Administration");
            adView.Click += adView_Click;
            adView.ShortcutKeys = Keys.Control | Keys.M;
            adView.ToolTipText = "Admin View";
            adView.Enabled = _enableadmin;
            views.DropDownItems.Add(adView);
            views.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem transactionView = new ToolStripMenuItem("Transaction History");
            transactionView.Click += TransactionView_Click;
            transactionView.ShortcutKeys = Keys.Control | Keys.Shift | Keys.T;
            transactionView.ToolTipText = "WF/Script Transaction History View";
            transactionView.Enabled = (_mode == Mode.Offline) ? false : _enableScripts;
            views.DropDownItems.Add(transactionView);

            ToolStripMenuItem RecordingView = new ToolStripMenuItem("Recording History");
            RecordingView.Click += RecordingView_Click;
            RecordingView.ShortcutKeys = Keys.Control | Keys.Shift | Keys.R;
            RecordingView.ToolTipText = "Recording History View";
            RecordingView.Enabled = true;
            views.DropDownItems.Add(RecordingView);

            ToolStripMenuItem about = new ToolStripMenuItem("About");
            about.Click += about_Click;
            about.Enabled = true;
            this.WorkItem.RootWorkItem.UIExtensionSites["MainMenu"].Add<ToolStripMenuItem>(about);

            //commented as a part of security concern 

            ToolStripMenuItem recorder = new ToolStripMenuItem("Process Recorder");
            recorder.Click += recorder_Click;
            recorder.Enabled = true;
            recorder.ShortcutKeys = Keys.Control | Keys.Shift | Keys.P;
            recorder.ToolTipText = "Process Recorder";
            tools.DropDownItems.Add(recorder);
        }

        private void ExportScript_Click(object sender, EventArgs e)
        {
            ExportScriptUtility es = new ExportScriptUtility();
            es.ShowDialog();
            
        }

        private void ExportScriptsView_Click(object sender, EventArgs e)
        {
            ExportScriptsView(this, new EventArgs());
        }

        private void Export_Click(object sender, EventArgs e)
        {
            Infosys.ATR.ExportUtility.ExportUtility utility = new ExportUtility.ExportUtility();
            utility.ShowDialog();
        }

        private void GeneratePlaybackScript_Click(object sender, EventArgs e)
        {
            GeneratePlaybackScript(sender, e);
        }

        private void RunPlaybackScript_Click(object sender, EventArgs e)
        {
            RunPlaybackScript(sender, e);
        }

        void replay_Click(object sender, EventArgs e)
        {
            Replay(this, new EventArgs());
        }

        void TransactionView_Click(object sender, EventArgs e)
        {
            TransactionView(this, new EventArgs());
        }

        void RecordingView_Click(object sender, EventArgs e)
        {
            RecordingView(this, new EventArgs());
        }

        private void GetSecureText_Click(object sender, EventArgs e)
        {
            GetSecureText genText = new GetSecureText();
            genText.ShowDialog();
        }

        void about_Click(object sender, EventArgs e)
        {
            AboutBox1 about = new AboutBox1();
            about.ShowDialog();
        }


        void addAssembly_Click(object sender, EventArgs e)
        {

            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Assemblies|*.dll;*.exe";
            open.Multiselect = true;
            if (open.ShowDialog() == DialogResult.OK)
            {
                var wfSrvc = WFServices.Instance;
                var files = open.FileNames;

                for (int i = 0; i < files.Length; i++)
                {
                    wfSrvc.LoadAssembly(files[i]);
                }
            }

        }

        void newFile_Click(object sender, EventArgs e)
        {

            ModuleLauncher launchPad = new ModuleLauncher();

            launchPad.LaunchOM += launchPad_LaunchOM;
            launchPad.LaunchWF += launchPad_LaunchWF;
            launchPad.LaunchScript += launchPad_LaunchScript;

            launchPad.StartPosition = FormStartPosition.CenterScreen;
            launchPad.ShowDialog();
        }

        private void grExplorer_Click(object sender, EventArgs e)
        {
            var o = GetActiveTab();

            if (o != null)
            {
                if (o.GetType().Name == Workspace.Explorer.ToString())
                {
                    GroupExplorer(this, new EventArgs());
                }
                else
                {
                    MessageBox.Show("You can perform this operation only on an Admin View", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("You can perform this operation only on an Admin View", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        void launchPad_LaunchScript()
        {
            if (_enableScripts)
            {
                if (_mode == Mode.Offline)
                {
                    EnableMenuForScripts(true);
                    NewScript(this, new EventArgs());
                }
                else if (_mode == Mode.Online)
                {
                    EnableMenuForScripts(true);
                    //  ScriptRepositoryView(this, new EventArgs());
                    //newScript_Click(this, new EventArgs());
                    NewScript(this, new EventArgs());
                }
            }
            else
                ShowNoLicense();

        }

        void launchPad_LaunchWF()
        {
            if (_enableWF)
            {
                workFlow_Click(this, new EventArgs());
                //  EnableMenuForWF(true);
            }
            else
                ShowNoLicense();
        }

        void launchPad_LaunchOM()
        {
            if (_enableOM)
            {
                objModel_Click(this, new EventArgs());
                //EnableMenuForOM(true);
            }
            else
                ShowNoLicense();
        }



        private void ShowNoLicense()
        {
            MessageBox.Show(_noLicense, "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void adView_Click(object sender, EventArgs e)
        {
            AdminView(this, new EventArgs());
            EnableMenuForAdmin(true);
        }

        private void newScript_Click(object sender, EventArgs e)
        {
            var o = GetActiveTab();

            if (o != null)
            {
                if (o.GetType().Name == Workspace.MainRepositoryView.ToString())
                {
                    NewScript(this, new EventArgs());
                }
            }
        }

        void scriptView_Click(object sender, EventArgs e)
        {
            ScriptRepositoryView(this, new EventArgs());
        }

        void objView_Click(object sender, EventArgs e)
        {

        }

        void delete_Click(object sender, EventArgs e)
        {
            var o = GetActiveTab();

            if (o != null)
            {
                if (o.GetType().Name == Workspace.Editor.ToString())
                {
                    DeleteNode(this, new EventArgs());
                }
                else if (o.GetType().Name == Workspace.Explorer.ToString())
                {
                    DeleteAdminNode(this, new EventArgs());
                }
                else if (o.GetType().Name == Workspace.MainRepositoryView.ToString())
                {
                    DeleteScript(this, new EventArgs());
                }
                else if (o.GetType().Name == Workspace.WFSelector.ToString())
                {
                    DeleteWF(this, new EventArgs());
                }
                else
                {
                    MessageBox.Show("You can perform this operation only on an Object Model or Admin View", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("You can perform this operation only on an Object Model or Admin View", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        void objModel_Click(object sender, EventArgs e)
        {
            NewObjModel(this, e);
        }

        void wfView_Click(object sender, EventArgs e)
        {
            WFView(this, new EventArgs());
            // Showtoolbar(this, new EventArgs());
            EnableMenuForWFView(true);
        }

        void publish_Click(object sender, EventArgs e)
        {
            var o = GetActiveTab();

            if (o != null)
            {
                if (o.GetType().Name == Workspace.WFDesigner.ToString())
                {
                    PublishWF(this, new EventArgs());
                }
                if (o.GetType().Name == Workspace.PythonIDE.ToString()
                    || o.GetType().Name == Workspace.ScriptDesigner.ToString())
                {
                    PublishScript(this, new EventArgs());
                }
            }
        }

        //void ExportIapd_Click(object sender, EventArgs e) 
        //{
        //    var o = GetActiveTab();

        //    if (o != null)
        //    {
        //        //if (o.GetType().Name == Workspace.WFDesigner.ToString())
        //        //{
        //        //    PublishWF(this, new EventArgs());
        //        //}
        //        if (o.GetType().Name == Workspace.PythonIDE.ToString()
        //            || o.GetType().Name == Workspace.ScriptDesigner.ToString())
        //        {
        //            PublishScript(this, new EventArgs());
        //        }
        //    }
        //}

        void run_Click(object sender, EventArgs e)
        {
            var o = GetActiveTab();

            if (o != null)
            {
                if (o.GetType().Name == Workspace.WFDesigner.ToString())
                {
                    RunWF(this, new EventArgs());
                }
                else if (o.GetType().Name == Workspace.WFSelector.ToString())
                {
                    RunWFSelector(this, new EventArgs());
                }
                else if (o.GetType().Name == Workspace.PythonIDE.ToString())
                {
                    RunScripts(this, new EventArgs());
                }
                else if (o.GetType().Name == Workspace.RecordingView.ToString())
                {
                    RunPlaybackScript(this, new EventArgs());
                }
                else if (o.GetType().Name == Workspace.MainRepositoryView.ToString() ||
                    o.GetType().Name == Workspace.ScriptDesigner.ToString())
                {
                    RunScripts_View(this, new EventArgs());
                }
                else if (o.GetType().Name == Workspace.Editor.ToString())
                {
                    MessageBox.Show("This feature works only on Workflow and Scripts", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        void closeTab_Click(object sender, EventArgs e)
        {
            var o = GetActiveTab();

            if (o != null)
            {
                var t = o.GetType().Name;

                switch (t)
                {
                    case "WFDesigner":
                    case "WFSelector":
                    case "TransactionView":
                        CloseWFTab(this, new EventArgs());
                        break;
                    case "RecordingView":
                        CloseRecordingTab(this, new EventArgs());
                        break;
                    case "Editor":
                        CloseObjModelTab(this, new EventArgs());
                        break;
                    //admin module
                    case "Explorer":
                        CloseAdminView(this, new EventArgs());
                        break;
                    //scripts 
                    default:
                        CloseTabScripts(this, new EventArgs());
                        break;
                }


            }
        }



        void save_Click(object sender, EventArgs e)
        {
            var o = GetActiveTab();

            if (o != null)
            {
                if (o.GetType().Name == Workspace.WFDesigner.ToString())
                {
                    SaveWorkflow(this, new EventArgs());
                }
                else if (o.GetType().Name == Workspace.Editor.ToString())
                {
                    SaveAtr(this, new EventArgs());
                }
                else if (o.GetType().Name == Workspace.ScriptDesigner.ToString() ||
                    o.GetType().Name == "PythonIDE")
                {
                    SaveScript(this, new EventArgs());
                }
            }
        }

        void open_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = _filter;
            var result = open.ShowDialog();
            if (result == DialogResult.OK)
            {
                Open(open.FileName);
            }
        }

        void exit_Click(object sender, EventArgs e)
        {
            Exit(this, e);
        }

        void workFlow_Click(object sender, EventArgs e)
        {
            NewWorkflow(this, e);
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        private bool IsFromGeneratedScript(string fileName)
        {
            var folder = Path.GetDirectoryName(fileName);
            var targerFile = Path.Combine(folder, "iap_parameters.py");
            if (!File.Exists(targerFile))
                return false;

            return true;
        }
        private void Open(string fileName)
        {

            var xtn = Path.GetExtension(fileName).Remove(0, 1);
            ContentMeta content = new ContentMeta();
            string extractionLoc = string.Empty;
            Stream iaplStream = null;

            if (xtn.Equals("py", StringComparison.InvariantCultureIgnoreCase))
            {
                if (IsFromGeneratedScript(fileName))
                {
                    PublishPlaybackScript(fileName);
                    return;
                }
            }

            if (xtn.Equals("iapl", StringComparison.InvariantCultureIgnoreCase))
                IAPPackage.Import(fileName, out content, out iaplStream, out extractionLoc);
            //fileName = Path.Combine(extractionLoc,content.Name, content.Name + "." + content.ContentType);

            switch (xtn)
            {
                case "xaml":
                    OpenWorkflow(this, new EventArgs<string>(fileName));
                    //  EnableMenuForWF(true);
                    break;
                case "atr":
                    OpenAtr(this, new EventArgs<string>(fileName));
                    // EnableMenuForOM(true);
                    break;
                case "iapl":
                    PackageMeta package = new PackageMeta() { Content = content, FileStream = iaplStream, PackageExtractLoc = extractionLoc, PackagePath = fileName };
                    if (content.ModuleType.Equals(ModuleType.Script))
                        OpenScriptFromPackage(this, new EventArgs<PackageMeta>(package));
                    else if (content.ModuleType.Equals(ModuleType.Workflow))
                        OpenWFFromPackage(this, new EventArgs<PackageMeta>(package));
                    break;
                default:
                    OpenScript(this, new EventArgs<string>(fileName));
                    //EnableMenuForPythonIDE(true);
                    break;
            }
        }

        private object GetActiveTab()
        {
            return this.WorkItem.RootWorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].ActiveSmartPart;
        }

        private void EnableMenuForWF(bool e)
        {
            //save
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.FileMenu].ToList()[2]).Enabled = e;
            //closetab
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.FileMenu].ToList()[4]).Enabled = e;
            //run
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[0].Enabled = e;
            //publish
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[1].Enabled = e;
            //delete
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[2].Enabled = false;
            //manage groups
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[3].Enabled = false;
            //load assembly
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[5].Enabled = e;
            //Generate Playback Script
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[6].Enabled = false;

            //Run Playback Script
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[7].Enabled = false;

            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[2]).Enabled = e;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[6]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[7]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[16]).Enabled = e;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[4]).Enabled = e;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[5]).Enabled = e;


            if (_mode == Mode.Offline)
            {
                ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[5]).Enabled = false;
                ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[1].Enabled = false;
            }
        }

        private void EnableMenuForWFView(bool e)
        {
            //save
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.FileMenu].ToList()[2]).Enabled = false;
            //closetab
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.FileMenu].ToList()[4]).Enabled = e;
            //run
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[0].Enabled = false;
            //publish   
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[1].Enabled = false;
            //delete
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[2].Enabled = false;
            //manage groups
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[3].Enabled = false;
            //load assembly
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[5].Enabled = e;
            //Generate Playback Scropt
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[6].Enabled = false;

            //Execute Playback Scropt
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[7].Enabled = false;

            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[2]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[4]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[5]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[6]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[7]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[16]).Enabled = e;

        }


        private void EnableMenuForOM(bool e)
        {
            //save
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.FileMenu].ToList()[2]).Enabled = e;
            //closetab
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.FileMenu].ToList()[4]).Enabled = e;
            //run
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[0].Enabled = false;
            //publish   
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[1].Enabled = false;
            //delete node
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[2].Enabled = e;
            //manage groups
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[3].Enabled = false;
            //load assembly
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[5].Enabled = e;
            //Generate Playback Script
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[6].Enabled = false;

            //Execute Playback Script
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[7].Enabled = false;

            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[2]).Enabled = e;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[4]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[5]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[6]).Enabled = false;
            //delete node
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[7]).Enabled = e;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[16]).Enabled = e;

        }


        private void EnableMenuForScripts(bool e)
        {
            //save
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.FileMenu].ToList()[2]).Enabled = false;
            //closetab
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.FileMenu].ToList()[4]).Enabled = e;
            //run
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[1].Enabled = e;
            //publish   
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[1].Enabled = false;
            //delete
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[2].Enabled = false;
            //manage groups
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[3].Enabled = false;
            //load assembly
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[5].Enabled = e;
            //Generate Playback Script
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[6].Enabled = false;

            //Run Playback Script
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[7].Enabled = false;

            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[4]).Enabled = e;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[5]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[6]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[7]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[16]).Enabled = e;

        }

        private void EnableMenuForAdmin(bool e)
        {
            //save
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.FileMenu].ToList()[2]).Enabled = false;
            //closetab
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.FileMenu].ToList()[4]).Enabled = e;
            //run
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[0].Enabled = false;
            //publish   
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[1].Enabled = false;
            //delete node
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[2].Enabled = e;
            //manage groups
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[3].Enabled = e;
            //load assembly
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[5].Enabled = false;
            //Generate Playback Script
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[6].Enabled = false;

            //Run Playback Script
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[7].Enabled = false;

            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[2]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[4]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[5]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[6]).Enabled = e;
            //delete node
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[7]).Enabled = e;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[16]).Enabled = e;
        }

        private void EnableMenuForPythonIDE(bool e)
        {
            //save
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.FileMenu].ToList()[2]).Enabled = e;
            //closetab
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.FileMenu].ToList()[4]).Enabled = e;
            //run
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[0].Enabled = e;
            //publish   
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[1].Enabled = e;
            //delete
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[2].Enabled = false;
            //manage groups
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[3].Enabled = false;
            //load assembly
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[5].Enabled = false;
            //Generate Playback Script
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[6].Enabled = false;

            //Run Playback Script
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[7].Enabled = false;

            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[2]).Enabled = e;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[4]).Enabled = e;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[5]).Enabled = e;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[6]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[7]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[16]).Enabled = e;
        }

        private void EnableMenuForOfflinePythonIDE(bool e)
        {
            //save
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.FileMenu].ToList()[2]).Enabled = true;
            //closetab
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.FileMenu].ToList()[4]).Enabled = true;
            //run
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[0].Enabled = false;
            //publish   
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[1].Enabled = false;
            //delete
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[2].Enabled = false;
            //manage groups
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[3].Enabled = false;
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[1]).DropDownItems[1].Enabled = false;
            //load assembly
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[5].Enabled = false;
            //Generate Playback Script
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[6].Enabled = false;

            //Run Playback Script
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[7].Enabled = false;

            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[2]).Enabled = true;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[4]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[5]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[6]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[7]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[10]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[16]).Enabled = true;
        }

        private void DefaultMenu()
        {
            //save
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.FileMenu].ToList()[2]).Enabled = false;
            //closetab
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.FileMenu].ToList()[4]).Enabled = false;
            //run
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[0].Enabled = false;
            //publish   
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[1].Enabled = false;
            //delete
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[2].Enabled = false;
            //manage groups
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[3].Enabled = false;
            //load assembly
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[5].Enabled = false;

            //Generate Playback Scropt
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[5].Enabled = false;

            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[2]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[4]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[5]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[6]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[7]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[16]).Enabled = false;

            if (_mode == Mode.Offline)
            {
                //script repository
                ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[1]).DropDownItems[1].Enabled = false;
                ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[10]).Enabled = false;
            }
        }


        private void EnableMenuScriptDesignerView(bool p)
        {
            //save
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.FileMenu].ToList()[2]).Enabled = true;
            //closetab
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.FileMenu].ToList()[4]).Enabled = true;
            //run
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[0].Enabled = false;
            //publish   
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[1].Enabled = true;
            //delete
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[2].Enabled = false;
            //manage groups
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[3].Enabled = false;
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[1]).DropDownItems[1].Enabled = true;
            //load assembly
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[5].Enabled = false;
            //Generate Playback Scropt
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[6].Enabled = false;

            //Run Playback Scropt
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[7].Enabled = false;

            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[2]).Enabled = true;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[4]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[5]).Enabled = true;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[6]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[7]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[10]).Enabled = true;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[16]).Enabled = true;
        }

        private void EnableMenuTransactionView(bool p)
        {
            //save
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.FileMenu].ToList()[2]).Enabled = false;
            //closetab
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.FileMenu].ToList()[4]).Enabled = true;
            //run
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).
                DropDownItems[0].Enabled = false;
            //publish   
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).
                DropDownItems[1].Enabled = false;
            //delete
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).
                DropDownItems[2].Enabled = false;
            //manage groups
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).
                DropDownItems[3].Enabled = false;
            //Script Repository
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[1]).DropDownItems[1].
                Enabled = true;
            //load assembly
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[5].Enabled = false;
            //Generate Playback Script
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[6].Enabled = false;

            //Run Playback Script
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[7].Enabled = false;

            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[2]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[4]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[5]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[6]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[7]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[8]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[10]).Enabled = true;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[16]).Enabled = true;
        }



        private void EnableMenu(string type)
        {

            if (type == Workspace.MainRepositoryView.ToString())
            {
                EnableMenuForScripts(true);
            }
            else if (type == Workspace.PythonIDE.ToString() && _mode == Mode.Online)
            {
                EnableMenuForPythonIDE(true);
            }
            else if (type == Workspace.PythonIDE.ToString() && _mode == Mode.Offline)
            {
                EnableMenuForOfflinePythonIDE(true);
            }
            else if (type == Workspace.WFDesigner.ToString())
            {
                EnableMenuForWF(true);
            }
            else if (type == Workspace.Editor.ToString())
            {
                EnableMenuForOM(true);
            }
            else if (type == Workspace.Explorer.ToString())
            {
                EnableMenuForAdmin(true);
            }
            else if (type == Workspace.WFSelector.ToString())
            {
                EnableMenuForWFView(true);
            }
            else if (type == Workspace.ScriptDesigner.ToString())
            {
                EnableMenuScriptDesignerView(true);
            }
            else if (type == Workspace.TransactionView.ToString())
            {
                EnableMenuTransactionView(true);
            }
            else if (type == Workspace.RecordingView.ToString())
            {
                EnableMenuRecorderView();
            }
            else
            {
                DefaultMenu();
            }
        }

        private void EnableMenuRecorderView()
        {
            //save
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.FileMenu].ToList()[2]).Enabled = false;
            //closetab
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.FileMenu].ToList()[4]).Enabled = true;
            //run
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[0].Enabled = true;
            //publish   
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[1].Enabled = false;
            //delete
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[2].Enabled = false;
            //manage groups
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[3].Enabled = false;

            //load assembly
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[5].Enabled = false;
            //Generate Playback Script
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[6].Enabled = true;

            //Run Playback Script
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[7].Enabled = true;

            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[2]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[4]).Enabled = true;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[5]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[6]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[7]).Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[8]).Enabled = true;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[16]).Enabled = true;

            if (_mode == Mode.Offline)
            {
                //script repository
                ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[1]).DropDownItems[1].Enabled = false;
                ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[10]).Enabled = false;
            }
            else
            {
                //script repository
                ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[1]).DropDownItems[1].Enabled = true;
                ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[10]).Enabled = true;
            }
        }


        [EventSubscription(Constants.EventTopicNames.CurrentTab, ThreadOption.UserInterface)]
        public void DockPanels(object sender, EventArgs<String> e)
        {
            var t = e.Data;

            if (t == Workspace.Editor.ToString())
            {
                ToggleDeckPanel(this, new EventArgs<bool>(true));
            }
            else
            {
                ToggleDeckPanel(this, new EventArgs<bool>(false));
            }

            EnableMenu(t);
        }

        [EventSubscription(Constants.EventTopicNames.DeActivatePublish, ThreadOption.UserInterface)]
        public void DeActivatePublish(object sender, EventArgs e)
        {
            //publish   
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[1].Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[5]).Enabled = false;

            EnableRun(sender);
        }

        [EventSubscription(Constants.EventTopicNames.ActivatePublish, ThreadOption.UserInterface)]
        public void ActivatePublish(object sender, EventArgs e)
        {
            //publish   
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[1].Enabled = true;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[5]).Enabled = true;

            EnableRun(sender);
        }

        private void EnableRun(object sender)
        {
            var s = sender.GetType();

            if (s.FullName.Contains(Workspace.WFDesigner.ToString()))
            {
                ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[0].Enabled = true;
                ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[4]).Enabled = true;
            }
        }

        [EventSubscription(Constants.EventTopicNames.DisablePublish, ThreadOption.UserInterface)]
        public void DisablePublish(object sender, EventArgs e)
        {
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[1].Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[5]).Enabled = false;

            EnableRun(sender);
        }

        [EventSubscription(Constants.EventTopicNames.ShowRun, ThreadOption.UserInterface)]
        public void DisablePublish(object sender, EventArgs<bool> e)
        {
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[0].Enabled =

            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[4]).Enabled =
            e.Data;

            //((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[0].Enabled =
            //    ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[2].Enabled =
            //((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[4]).Enabled =
            //((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[7]).Enabled = e.Data;
        }

        [EventSubscription(Constants.EventTopicNames.EnableDeleteMenu, ThreadOption.UserInterface)]
        public void EnableDeleteMenu_Handler(object sender, EventArgs<bool> e)
        {
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[2].Enabled =

            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[7]).Enabled =
            e.Data;
        }
        private void recorder_Click(object sender, EventArgs e)
        {
            MinimizeShell(this, new EventArgs());
            RecorderView(this, new EventArgs());

        }

        [EventSubscription(Constants.EventTopicNames.PublishPlaybackScript, ThreadOption.UserInterface)]
        public void PublishPlaybackScript_Handler(object sender, EventArgs<string> e)
        {
            PublishPlaybackScript(e.Data);
        }

        private void PublishPlaybackScript(string filename)
        {
            PackageMeta metaData = CodeGeneration.CreatePlaybackScript(filename);
            OpenScriptFromPackage(this, new EventArgs<PackageMeta>(metaData));

            //CodeGeneration.CreatePlaybackScript(filename, out content, out iaplStream, out scriptPath);
            //PackageMeta metaData = new PackageMeta() { Content = content, FileStream = iaplStream, PackagePath = scriptPath };            
        }

        [EventSubscription(Constants.EventTopicNames.ExecutePlaybackScript, ThreadOption.UserInterface)]
        public void ExecutePlaybackScript_Handler(object sender, EventArgs<string> e)
        {
            ExecutePlaybackScript(e.Data);
        }
        private void ExecutePlaybackScript(string filename)
        {
            PlaybackResult result = CodeGeneration.ReplayScript(filename);
        }

        [EventSubscription(Constants.EventTopicNames.DisableGeneratePlaybackScript, ThreadOption.UserInterface)]
        public void DisableGeneratePlaybackScript(object sender, EventArgs e)
        {
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[6].Enabled = false;
            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[7].Enabled = false;
            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[8]).Enabled = false; ;
        }
    }
}

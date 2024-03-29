//----------------------------------------------------------------------------------------
// patterns & practices - Smart Client Software Factory - Guidance PackageAppendOutputViewHandler
//
// This file was generated by this guidance package as part of the solution template
//
// The FormShell class represent the main form of your application.
// 
// The default Form supplied in this guidance package provides basic UI elements 
// like:
//      - A MenuStrip
//      - A ToolStrip
//      - A StatusStrip
//      - 2 WorkSpaces (left and right) separated by a spliter
//
// There is also a subscription to the "StatusUpdate" event topic used to change the
// content of the StatusStrip
//
//  
//
//
// Latest version of this Guidance Package: http://go.microsoft.com/fwlink/?LinkId=62182
//----------------------------------------------------------------------------------------

using System;
using System.Windows.Forms;
using System.Linq;
using IMSWorkBench.Infrastructure.Interface;
using IMSWorkBench.Infrastructure.Shell.Constants;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.CompositeUI.WinForms;
using Infosys.ATR.DevelopmentStudio;
using IMSWorkBench.Infrastructure.Library.Services;
using Infosys.ATR.Editor.Services;
using System.Collections.Generic;
using Infosys.ATR.CommonViews;

namespace IMSWorkBench.Infrastructure.Shell
{

    /// <summary>
    /// Main application shell view.
    /// </summary>
    public partial class ShellForm : Form
    {
        bool _toggle = true;
        bool _hideDock = false;
        System.Diagnostics.Process stopperProcess = null;

        [EventPublication(EventTopicNames.SaveImageIntr, PublicationScope.Global)]
        public event EventHandler<EventArgs<System.Drawing.Bitmap>> SaveImageIntr;

        [EventPublication(EventTopicNames.SmartPartClosing, PublicationScope.Global)]
        public event EventHandler<EventArgs<Object>> SmartPartClosing;

        [EventPublication(EventTopicNames.CopyControl, PublicationScope.Global)]
        public event EventHandler<EventArgs<string>> CopyControl;

        [EventPublication(EventTopicNames.SetCurrentTab, PublicationScope.Global)]
        public event EventHandler<EventArgs<string>> SetCurrentTab;

        [EventPublication(EventTopicNames.CurrentTab, PublicationScope.Global)]
        public event EventHandler<EventArgs<string>> CurrentTab;

        [EventPublication(EventTopicNames.CurrentTabSelected, PublicationScope.Global)]
        public event EventHandler<EventArgs<string>> CurrentTabSelected;

        [EventPublication(EventTopicNames.TabHover, PublicationScope.Global)]
        public event EventHandler<EventArgs> TabHover;

        [EventPublication(EventTopicNames.SaveAndClose, PublicationScope.Global)]
        public event EventHandler<EventArgs<FormClosingEventArgs>> SaveAndClose;

        [EventPublication(EventTopicNames.RefreshExplore, PublicationScope.Global)]
        public event EventHandler RefreshExploreEvent;

        internal void RefreshExploreEvent_Handler()
        {
            RefreshExploreEvent(this, new EventArgs());
        }



        internal string AutomationParent { get; set; }
        internal string AuomationId { get; set; }
        internal string AutomationControlName { get; set; }
        internal string AutomationControlPath { get; set; }
        OutputView view = null;

        internal ToolStrip MainToolStrip
        {
            get
            {
                return this._mainToolStrip;
            }
        }

        internal ToolStrip MainToolBar
        {
            get
            {
                return this._mainToolBar;
            }
        }

        internal ToolStripItemCollection FileMenu
        {
            get
            {
                return this._fileMenu.DropDownItems;
            }
        }

        internal string AutomationChild
        {
            get
            {
                return txtAutomation.Text;
            }
            set
            {
                txtAutomation.Text = value;
            }
        }

        string _automationChildSubData;
        internal string AutomationChildSubData
        {
            get
            {
                return _automationChildSubData;
            }
            set
            {
                _automationChildSubData = value;
                UpdateControlProperties();
            }
        }

        //public event EventHandler<EventArgs> FormClosing;
        /// <summary>
        /// Default class initializer.
        /// </summary>
        public ShellForm()
        {
            InitializeComponent();
            _tabWorkspace.Name = WorkspaceNames.TabWorkSpace;
            _deckWorkspace.Name = WorkspaceNames.DeckWorkSpace;
            _mainToolStrip.Name = UIExtensionSiteNames.MainToolbar;
            _mainToolBar.Name = UIExtensionSiteNames.MainMenu;
            _mainStatusStrip.Name = UIExtensionSiteNames.MainStatus;
            _utilWorkspace.Name = WorkspaceNames.UtilWorkSpace;
            _automationWorkspace.Name = WorkspaceNames.AutomationWorkSpace;
            btnTransparent();
            btnCollapse.Hide();
            splitContainer1.Panel1Collapsed = true;

            string enableStopInititor = System.Configuration.ConfigurationManager.AppSettings["EnableStopInitiator"];
            bool stopInitor;
            if (!string.IsNullOrEmpty(enableStopInititor) && bool.TryParse(enableStopInititor, out stopInitor) && stopInitor)
            {
                System.Diagnostics.ProcessStartInfo processStart = new System.Diagnostics.ProcessStartInfo();
                processStart.FileName = "InitiateStop.exe";
                if (System.IO.File.Exists(processStart.FileName))
                {
                    stopperProcess = System.Diagnostics.Process.Start(processStart);
                }
            }
        }



        /// <summary>
        /// Status update handler. Updates the status strip on the main form.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        [EventSubscription(EventTopicNames.StatusUpdate, ThreadOption.UserInterface)]
        public void StatusUpdateHandler(object sender, EventArgs<string> e)
        {
            _statusLabel.Text += e.Data;
        }

        [EventSubscription(EventTopicNames.ToggleDeckPanel, ThreadOption.UserInterface)]
        public void ToggleDeckPanelHandler(object sender, EventArgs<bool> e)
        {
            var visible = (bool)e.Data;
            panel3.Visible = visible;
            pnlDock.Visible = visible;

            //if (this._tabWorkspace.SelectedTab != null)
            //{
            //    string text = this._tabWorkspace.SelectedTab.Text;
            //    if (text.Equals("Script Repository"))
            //    {
            //        if (view != null && view.consolidatedOutput != null && view.consolidatedOutput.Count > 0)
            //            pnlOutputView.Visible = true;
            //        else
            //            pnlOutputView.Visible = false;
            //    }
            //    else
            //        pnlOutputView.Visible = false;
            //}
            //else
            //    pnlOutputView.Visible = false;
        }

        [EventSubscription(EventTopicNames.ShowOutputView, ThreadOption.UserInterface)]
        public void ShowOutputViewHandler(object sender, EventArgs<List<ExecutionResultView>> e)
        {
            var visible = (List<ExecutionResultView>)e.Data;

            visible.ForEach(s => {

                if (s.Identifier == Guid.Empty)
                {
                    s.Identifier = Guid.NewGuid();
                }

            });
           
            if (pnlOutputView.Visible)
            {
                //OutputView.consolidatedOutput.Clear();
                OutputView v = pnlOutputView.Controls[0] as OutputView;
                view.Display(visible);
               //pnlOutputView.Controls.Remove(v);
                // view = null;
            }
            else
            {
                view = new OutputView();
                view.Display(visible);
                pnlOutputView.Controls.Add(view);
                splitContainer3.Panel2Collapsed = false;
            }

            view.CloseOutputView += new OutputView.CloseOutputViewHandler(CloseOutputViewHandler);
        }


        [EventSubscription(EventTopicNames.AppendOutputView, ThreadOption.UserInterface)]
        public void AppendOutputViewHandler(object sender, Infosys.ATR.ScriptRepository.Views.MainRepositoryView.AppendOutputViewArgs e)
        {
            Guid identifier = e.Identifier;
            string scriptID = e.scriptID;
            int progress = e.progress;            
           

            ExecutionResultView executionResultView = e.executionResultView;
            executionResultView.Identifier = e.Identifier;
            if (pnlOutputView.Visible)
            {
                //OutputView.consolidatedOutput.Clear();
                OutputView v = pnlOutputView.Controls[0] as OutputView;
                v.PopulateGrid(identifier, executionResultView,progress);
            }
            else
            { 
                view = new OutputView();
                view.PopulateGrid(identifier, executionResultView,progress);
                pnlOutputView.Controls.Add(view);
                splitContainer3.Panel2Collapsed = false;
                view.CloseOutputView += new OutputView.CloseOutputViewHandler(CloseOutputViewHandler);
            }
        }


        [EventSubscription(EventTopicNames.AppendOutputViewWF, ThreadOption.UserInterface)]
        public void wfAppendOutputViewHandler(object sender, Infosys.ATR.WFDesigner.Views.ExecuteWf.AppendOutputViewArgsWF e)
        {
            Guid identifier = e.Identifier;
            string scriptID = e.scriptID;
            int progress = e.progress;

            ExecutionResultView executionResultView = e.executionResultView;
            if (pnlOutputView.Visible)
            {
                //OutputView.consolidatedOutput.Clear();
                OutputView v = pnlOutputView.Controls[0] as OutputView;
                v.PopulateGrid(identifier, executionResultView, progress);
            }
            else
            {
                view = new OutputView();
                view.PopulateGrid(identifier, executionResultView, progress);
                pnlOutputView.Controls.Add(view);
                splitContainer3.Panel2Collapsed = false;
                view.CloseOutputView += new OutputView.CloseOutputViewHandler(CloseOutputViewHandler);
            }
        }

        // [EventSubscription(EventTopicNames.CloseOutputView, ThreadOption.UserInterface)]
        public void CloseOutputViewHandler()
        {
            if (pnlOutputView.Visible)
            {
                if (OutputView.consolidatedOutput != null)
                {
                    OutputView.consolidatedOutput.Clear();
                }
                OutputView v = pnlOutputView.Controls[0] as OutputView;
                pnlOutputView.Controls.Remove(v);
                view = null;
            }
            splitContainer3.Panel2Collapsed = true;
        }

        [EventSubscription(EventTopicNames.Maximize, ThreadOption.UserInterface)]
        public void MaximizeHandler(object sender, EventArgs e)
        {
            Win32.Maximize(this.Handle);
        }

        [EventSubscription(EventTopicNames.Exit, ThreadOption.UserInterface)]
        public void ExitHandler(object sender, EventArgs e)
        {
            //add logic
            this.Close();

        }

        [EventSubscription(EventTopicNames.LoadGeneric, ThreadOption.UserInterface)]
        public void LoadGeneric_Handler(object sender, EventArgs e)
        {
            this.panel3.Hide();
            pnlDock.Hide();
            // this._mainToolStrip.Hide();
            _hideDock = true;
        }

        [EventSubscription(EventTopicNames.Showtoolbar, ThreadOption.UserInterface)]
        public void Showtoolbar_Handler(object sender, EventArgs e)
        {
            // this._mainToolStrip.Show();
        }

        [EventSubscription(EventTopicNames.TerminateApp, ThreadOption.UserInterface)]
        public void TerminateApp_Handler(object sender, EventArgs e)
        {
            Application.Exit();
        }

        [EventSubscription(EventTopicNames.TabHoverSet, ThreadOption.UserInterface)]
        public void TabHoverSet_Handler(object sender, EventArgs<String> e)
        {
            this._tabWorkspace.SelectedTab.ToolTipText = e.Data;
        }

        [EventSubscription(EventTopicNames.ShowUserLogged, ThreadOption.UserInterface)]
        public void ShowUserLogged_Handler(object sender, EventArgs<String> e)
        {
            this._statusLabel.Text = e.Data;
            this.lblMode.Text = (String)System.Configuration.ConfigurationManager.AppSettings["Mode"] + " ";
        }

        [EventSubscription(EventTopicNames.ShowTransaction, ThreadOption.UserInterface)]
        public void ShowTransaction_Handler(object sender, EventArgs<String> e)
        {
            lblTransaction.Text = e.Data;
        }

        [EventSubscription(EventTopicNames.WFSave, ThreadOption.UserInterface)]
        public void ChangeTabText_WF(object sender, EventArgs<String> e)
        {
            ChangeTabText("WF - ", e.Data);
        }


        [EventSubscription(EventTopicNames.ScriptSave, ThreadOption.UserInterface)]
        public void ChangeTabText_Script(object sender, EventArgs<String> e)
        {
            ChangeTabText("Script - ", e.Data);
        }

        [EventSubscription(EventTopicNames.OMSave, ThreadOption.UserInterface)]
        public void ChangeTabText_OM(object sender, EventArgs<String> e)
        {
            ChangeTabText("Object Model - ", e.Data);
        }

        [EventSubscription(EventTopicNames.CollapseObjectExplorer, ThreadOption.UserInterface)]
        public void CollapseObjectExplorer(object sender, EventArgs e)
        {
            btnCollapse_Click(this, e);
        }


        void ChangeTabText(string mode,string text)
        {
            var name = System.IO.Path.GetFileNameWithoutExtension(text);
            string shortname = "";
            if (name.Length > 9)
            {
                shortname = name.Substring(0, 8);
            }
            else
            {
                shortname = name;
            }
            this._tabWorkspace.SelectedTab.Text = mode + shortname;
            this._tabWorkspace.SelectedTab.ToolTipText = name;
        }


        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExpand_Click(object sender, EventArgs e)
        {
            btnCollapse.Visible = true;
            btnExpand.Hide();
            splitContainer1.Panel1Collapsed = false;
        }


        private void btnTransparent()
        {
            btnExpand.FlatAppearance.MouseOverBackColor =
                btnExpand.FlatAppearance.MouseDownBackColor =
                 btnCollapse.FlatAppearance.MouseOverBackColor =
                btnCollapse.FlatAppearance.MouseDownBackColor =
               System.Drawing.Color.Transparent;
        }

        private void btnCollapse_Click(object sender, EventArgs e)
        {
            btnExpand.Visible = true;
            btnCollapse.Hide();
            splitContainer1.Panel1Collapsed = true;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (_toggle)
            {
                toolStripButton1.Image = imageList1.Images["hover"];
                _toggle = false;
                toolStripButton1.ToolTipText = "Stop capturing control details";

            }
            else
            {
                toolStripButton1.Image = imageList1.Images["normal"];
                _toggle = true;
                toolStripButton1.ToolTipText = "Start capturing the control details. Press 'SHIFT + move Mouse' on the control";
            }

            ShellApplication._explorer.Capture();

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            ShellApplication._explorer.Refresh();
        }

        private void linkParent_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CustomMessageBox newMsgBox = new CustomMessageBox();
            newMsgBox.Message = AutomationParent;
            newMsgBox.ShowDialog();
        }

        private void _tabWorkspace_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this._tabWorkspace.SelectedTab != null)
            {
                if (Utilities.SavedTabs.Keys.Count > 0)
                {
                    Utilities.CurrentTab = this._tabWorkspace.SelectedTab.Text;
                    //foreach (KeyValuePair<int, string> pair in Utilities.SequenceTimeStampsOM)
                    //{
                    //    if (pair.Value == Utilities.CurrentTab)
                    //        Utilities.SelectedSequenceNumber = pair.Key;
                    //}
                    if (SetCurrentTab != null)
                        SetCurrentTab(this, new EventArgs<string>(""));
                }
                //if (this._tabWorkspace.SelectedTab.Text.Contains("Python") || _hideDock)
                //    this.pnlDock.Visible = false;
                //else
                //    this.pnlDock.Visible = true;
                
                var t = this._tabWorkspace.SelectedTab;                
                CurrentTab(this, new EventArgs<string>(t.Controls[0].GetType().Name));
                System.Threading.Thread.Sleep(100);
                CurrentTabSelected(this, new EventArgs<string>(t.ToolTipText));   
            }
            else
            {
                CurrentTab(this, new EventArgs<string>(String.Empty));
            }

            if (this._tabWorkspace.TabCount == 0)
            {
                ToggleDeckPanelHandler(this, new EventArgs<bool>(false));
            }

            RefreshExploreEvent_Handler();
        }


        internal void SaveImageHandler(System.Drawing.Bitmap image)
        {
            if (SaveImageIntr != null)
                SaveImageIntr(this, new EventArgs<System.Drawing.Bitmap>(image));
        }

        internal void CloseSmartPart(object smartPart)
        {
            if (SmartPartClosing != null)
                SmartPartClosing(this, new EventArgs<Object>(smartPart));
        }

        public void UpdateControlProperties()
        {
            if (CopyControl != null)
                CopyControl(this, new EventArgs<string>(this.AutomationChildSubData));
        }

        private void _tabWorkspace_MouseHover(object sender, EventArgs e)
        {
            if (TabHover != null)
                TabHover(this, e);

        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void ShellForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveAndClose(this, new EventArgs<FormClosingEventArgs>(e));
        }

        private void ShellForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (stopperProcess != null && !stopperProcess.HasExited)
            {
                stopperProcess.CloseMainWindow();
                stopperProcess.Close();
            }
        }
        [EventSubscription(Constants.EventTopicNames.MinimizeShell, ThreadOption.UserInterface)]
        public void CloseRecorderView(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

        }
    }
}

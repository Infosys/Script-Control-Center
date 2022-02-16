/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

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

namespace Infosys.ATR.Editor
{
    public class EditorWorkItem : WorkItemController
    {
       FileCreate _fc = null;
        Settings _settings = null;
        int _script = 1;
        Dictionary<string, Views.Editor> _editors = new Dictionary<string, Views.Editor>();
        Dictionary<string, Views.ImageEditor.ImageEditor> _imageEditors = new Dictionary<string, Views.ImageEditor.ImageEditor>();
        Dictionary<String, Views.PythonIDE> _pythonEditors = new Dictionary<string, Views.PythonIDE>();


        [EventPublication(Constants.EventTopicNames.ToggleDeckPanel, PublicationScope.Global)]
        public event EventHandler<EventArgs<bool>> ToggleDeckPanel;

        [EventPublication(Constants.EventTopicNames.TabHoverSet, PublicationScope.Global)]
        public event EventHandler<EventArgs<String>> TabHoverSet;

        [EventPublication(Constants.EventTopicNames.CollapseObjectExplorer, PublicationScope.Global)]
        public event EventHandler CollapseObjectExplorer;

        public override void Run()
        {
            AddServices();
           // ExtendMenu();
           // ExtendToolStrip();
            AddViews();
            this.WorkItem.State["imageEditors"] = _imageEditors;
            this.WorkItem.State["editors"] = _editors;
            this.WorkItem.State["pyEditors"] = _pythonEditors;
        }

        private void AddViews()
        {
            _fc = this.WorkItem.SmartParts.AddNew<FileCreate>("fileCreate");
            _settings = this.WorkItem.SmartParts.AddNew<Settings>("settings");
        }

        private void AddServices()
        {
            //TODO: add services provided by the Module. See: Add or AddNew method in 
            //		WorkItem.Services collection
        }
       

        internal void exit_Click(object sender, EventArgs e)
        {

            while (Utilities.SavedTabs.Count > 0)
            {
                //var _editor = this.WorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].ActiveSmartPart as Views.Editor;
                //if (!Utilities.SavedTabs[_editor._ucName])
                //{
                //    _editor.GenerateObjectModel();
                //}
                CloseTab();
            }
           
        }

        internal void pySettings_Click(object sender, EventArgs e)
        {
            var settings = this.WorkItem.SmartParts.Get<Settings>("settings");
            WindowSmartPartInfo sp = new WindowSmartPartInfo();
            sp.Title = "Python Settings";
            sp.MaximizeBox = false;
            sp.MinimizeBox = false;
            this.WorkItem.RootWorkItem.Workspaces[Constants.WorkspaceNames.ModalWindows].Show(settings, sp);

        }

        internal void close_Click(object sender, EventArgs e)
        {
            CloseTab();
        }

        private void CloseTab()
        {
            var _editor = this.WorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].ActiveSmartPart as IClose;

            if (_editor != null)
            {
                if (_editor.GetType() == typeof(Views.Editor))
                {
                    var _editor1 = _editor as Views.Editor;

                    if (!Utilities.SavedTabs[_editor1._ucName])
                    {
                        // var name = _editor1._imageEditorTitle.Split('-')[1].Trim();
                        DialogResult result = MessageBox.Show("Do you want to save changes to Object Model - " + _editor1.ObjectModel + " before exiting?", "IAP", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                        if (result == DialogResult.Yes)
                        {
                            _editor1.GenerateObjectModel();
                            Close(_editor1);
                        }
                        else if (result == DialogResult.No)
                            Close(_editor1);
                    }
                    else
                        Close(_editor1);
                    
                }
                else
                {
                    _editor.Close();
                }
            }
        }

        private void Close(Views.Editor _editor1)
        {
            //var smartpart = this.WorkItem.RootWorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].ActiveSmartPart as IClose;
            _editor1.Close();
            this.WorkItem.SmartParts.Remove(_editor1);
            Utilities.SavedTabs.Remove(_editor1._ucName);
            Utilities.OpenedTabs.Remove(Constants.Application.ObjectModel + _editor1._objectModelfileName);
            Utilities.Editors.Remove(_editor1);
            CollapseObjectExplorer(this, new EventArgs());
        }

        internal void genScriptBtn_Click(object sender, EventArgs e)
        {
            var editor = this.WorkItem.RootWorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].ActiveSmartPart as Editor.Views.Editor;
            if (editor != null)
                editor.GenerateScript();
        }

        internal void runBatBtn_Click(object sender, EventArgs e)
        {
            var pyIDE = this.WorkItem.RootWorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].ActiveSmartPart as Infosys.ATR.Editor.Views.PythonIDE;

            if (pyIDE != null)
                pyIDE.Run();
        }

        internal void pythonSaveBtn_Click(object sender, EventArgs e)
        {
            var python = this.WorkItem.RootWorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].ActiveSmartPart as Views.PythonIDE;
            if (python != null)
            {
                SaveFileDialog save = new SaveFileDialog();
                save.Filter = "Python files (*.py)|*.py";


                if (save.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(save.FileName, python.RichText);
                    WindowSmartPartInfo sp = new WindowSmartPartInfo();
                    sp.Title = "Python - " + Path.GetFileNameWithoutExtension(save.FileName);
                    this.WorkItem.RootWorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].ApplySmartPartInfo(python, sp);
                }
            }
        }

        internal void pythonOpenBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Python files (*.py)|*.py";

            if (open.ShowDialog() == DialogResult.OK)
            {
                var ide = AddPythonView("Python - " + Path.GetFileNameWithoutExtension(open.FileName));
                ide.CurrentFile = open.FileName;
                ide.RichText = File.ReadAllText(open.FileName);

            }

        }

        internal void pythonBtn_Click(object sender, EventArgs e)
        {
            AddPythonView("Python Script - " + _script);
            _script++;
        }

        private Views.PythonIDE AddPythonView(string title)
        {
            Views.PythonIDE ide = new Views.PythonIDE();
            ide.Dock = DockStyle.Fill;
            ide.ucName = title;
            WindowSmartPartInfo smartPart = new WindowSmartPartInfo();
            smartPart.Title = title;
            this.WorkItem.SmartParts.Add(ide, title);
            var sp = this.WorkItem.SmartParts.Get(title);
            this.WorkItem.RootWorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].Show(sp, smartPart);
            _pythonEditors.Add(title, ide);
            ToggleDeck(false);
            return ide;
        }

        internal void fileOpenBtn_Click(object sender, EventArgs e)
        {
            if (_fc != null)
            {
                _fc.Clear();
                WindowSmartPartInfo sp = new WindowSmartPartInfo();
                sp.Title = "Set file name and base directory";
                sp.Modal = true;
                sp.MaximizeBox = false;
                sp.MinimizeBox = false;                
                this.WorkItem.RootWorkItem.Workspaces[Constants.WorkspaceNames.ModalWindows].Show(_fc, sp);                
            }

        }

       

        [EventSubscription(Constants.EventTopicNames.BaseDir, ThreadOption.UserInterface)]
        public void OnBaseDir(object sender, EventArgs<String[]> e)
        {
            var title = e.Data[0];
            var baseDir = e.Data[1];
            var updatedEditorTitle = String.Empty;
            var updatedImgTitle = String.Empty;

            if (title.Length > 9)
            {
                title = title.Substring(0, 8);
            }

            updatedEditorTitle = AddViews(title,false);
            var editor = this.WorkItem.SmartParts.Get<Views.Editor>(Constants.Application.ObjectModel + updatedEditorTitle);
            editor.BaseDir = baseDir;
            editor.ObjectModel = e.Data[0];

            var mode = (Entities.ProjectMode)Enum.Parse(typeof(Entities.ProjectMode), e.Data[2]);
            editor.Mode = mode;
            updatedImgTitle = Constants.Application.ImageEditor + updatedEditorTitle;

            editor._imageEditorTitle = updatedImgTitle;
            editor.AddRootNode();
            var imgEditor = _imageEditors[updatedImgTitle];
            imgEditor.Mode = mode;
        }

        internal void delBtn_Click(object sender, EventArgs e)
        {
            var _editor = this.WorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].ActiveSmartPart as Views.Editor;
            _editor.DeleteNode();
        }

        internal void Open(string file)
        {
            var title = Path.GetFileNameWithoutExtension(file);

            if (title.Length > 9)
            {
                title = title.Substring(0, 8);
            }

            string updatedTitle = AddViews(title,true);
            var editor = this.WorkItem.SmartParts.Get<Views.Editor>(Constants.Application.ObjectModel + updatedTitle);
            editor.BaseDir = Path.GetDirectoryName(file);
            editor.ObjectModel = Path.GetFileNameWithoutExtension(file);
            // editor._imageEditorTitle = Constants.Application.ImageEditor + updatedTitle;
            editor.Open(file);
            Logger.Log("ControlGraph", "Open",
             "Edit ObjectTree " + title);
        }

        internal void fileEditBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "ATR files (*.atr)|*.atr";
            var show = open.ShowDialog();

            if (show == DialogResult.OK)
            {
                Open(open.FileName);       
            }
            var fileName = Path.GetFileNameWithoutExtension(open.FileName);
            
        }

        internal void saveBtn_Click(object sender, EventArgs e)
        {
            var _editor = this.WorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].ActiveSmartPart as Views.Editor;
            _editor.GenerateObjectModel();

        }


        private string AddViews(string title, bool saved)
        {            
            string returnValue = String.Empty;
            ToggleDeck(true);
            var updatedEditorTitle = String.Empty;
            var updatedImageTitle = String.Empty;
            var editorTitle = Constants.Application.ObjectModel + title;
            var imgEditorTitle = Constants.Application.ImageEditor + title;

            if (Utilities.OpenedTabs.Contains(editorTitle))
            {
                Utilities.SequenceNumber += 1;
                updatedEditorTitle = editorTitle + "(" + Utilities.SequenceNumber + ")";
                updatedImageTitle = imgEditorTitle + "(" + Utilities.SequenceNumber + ")";
                Utilities.SelectedSequenceNumber = Utilities.SequenceNumber;
                returnValue = title + "(" + Utilities.SequenceNumber + ")";
            }
            else
            {
                Utilities.OpenedTabs.Add(Constants.Application.ObjectModel + title);
                returnValue = title;
            }
            if (updatedEditorTitle == String.Empty)
            {
                updatedEditorTitle = editorTitle;
                updatedImageTitle = imgEditorTitle;
            }

            var _editor = this.WorkItem.SmartParts.AddNew<Views.Editor>(updatedEditorTitle);
       
            WindowSmartPartInfo smartPart = new WindowSmartPartInfo();
            smartPart.Title = updatedEditorTitle;

            _editors.Add(updatedEditorTitle, _editor);
            _editor._ucName = updatedEditorTitle;
          
            var _imagestate = this.WorkItem.SmartParts.AddNew<Views.ImageEditor.ImageEditor>(updatedImageTitle);
            _imageEditors.Add(updatedImageTitle, _imagestate);
            _imagestate._ucName = updatedImageTitle;

            _editor._imageEditorTitle = updatedImageTitle;
            
            this.WorkItem.RootWorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].Show(_editor, smartPart);
            this.WorkItem.RootWorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].SmartPartActivated += new EventHandler<Microsoft.Practices.CompositeUI.SmartParts.WorkspaceEventArgs>(ModuleController_SmartPartActivated);

            Utilities.Editors.Add(_editor, saved);
            Utilities.CurrentTab = updatedEditorTitle;
            Utilities.SavedTabs.Add(updatedEditorTitle, saved);

            return returnValue;
        }

        void ModuleController_SmartPartActivated(object sender, Microsoft.Practices.CompositeUI.SmartParts.WorkspaceEventArgs e)
        {
            var _editor = this.WorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].ActiveSmartPart as Views.Editor;
            if (_editor != null)
            {
                _editor.InitializeSmartpart();
                _editor.UpdateSelectedNodeProperties();
                
            }
        }

        private void ToggleDeck(bool value)
        {
            if (ToggleDeckPanel != null)
            {
                ToggleDeckPanel(this, new EventArgs<bool>(value));
            }
        }

        [EventSubscription(Constants.EventTopicNames.TabHover, ThreadOption.UserInterface)]
        public void TabHover_Handler(object sender, EventArgs e)
        {
            var ide = GetCurrentTab();

            if (ide.GetType() == typeof(Views.Editor))
            {
                var p = ide as Views.Editor;

                if (TabHoverSet != null)
                    TabHoverSet(this, new EventArgs<string>(p.ObjectModel));
            }
        }

        private object GetCurrentTab()
        {
            return this.WorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].ActiveSmartPart;
        }

        //[EventSubscription(Constants.EventTopicNames.SaveAndClose, ThreadOption.UserInterface)]
        public void SaveAndClose_handler(object sender, EventArgs<FormClosingEventArgs> e)
        {
            List<Tuple<Editor.Views.Editor, bool>> savedTabs = new List<Tuple<Editor.Views.Editor, bool>>();

            foreach (KeyValuePair<Editor.Views.Editor, bool> kvp in Utilities.Editors)
            {
                savedTabs.Add(new Tuple<Editor.Views.Editor, bool>(kvp.Key, kvp.Value));
            }

            savedTabs.ForEach(s => {

                if (s.Item2 == false)
                {
                    DialogResult result = MessageBox.Show("Do you want to save changes to Object Model - " + s.Item1.ObjectModel + " before exiting?", "IAP", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                    if (result == DialogResult.Yes)
                    {
                        s.Item1.GenerateObjectModel();
                        Close(s.Item1);
                    }
                    else if (result == DialogResult.No)
                        Close(s.Item1);
                    else
                        e.Data.Cancel = true;
                }
                else
                    Close(s.Item1);

            });       
           

        }

        internal void replay_click(object sender, EventArgs e)
        {
            var _editor = this.WorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].ActiveSmartPart as Views.Editor;
            _editor.Replay();
        }
    }
}

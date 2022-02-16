using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.ComponentModel;
using System.Windows.Automation;

using Entity = Infosys.ATR.Editor.Entities;
using Infosys.ATR.UIAutomation.Entities;
using Infosys.ATR.DevelopmentStudio;
using IMSWorkBench.Infrastructure.Interface.Services;
using Infosys.ATR.Editor.Services;
using Microsoft.Practices.CompositeUI.WinForms;

namespace Infosys.ATR.Editor.Views
{
    public partial class Editor : UserControl, IEditor, IClose
    {
        ToolStripMenuItem _context = null;
        int _appCount = 1; int _screenCount = 1; int _controlCount = 1; int _ctrlName = 1;
        internal string _objectModelfileName;
        public string _imageEditorTitle;
        Entities.ProjectMode _mode;
        ImageEditor.ImageEditor _imgEditor = null;

        internal string _ucName;

        public string ucName { get { return _ucName; } set { _ucName = value; } }
        

        string _propertyEditorName = "";

        PropertyGrid propertyGrid1;
        TabControl tabControl1;

        PropertyEditor _propertyEditor;
        Infosys.ATR.Editor.Views.AutomationGrid _grid;
        private string _propertyGridName;

        string objectModelDeckName;
        AutomationConfig autoConfig = null;

        public Editor()
        {
            InitializeComponent();


            _context = new ToolStripMenuItem();
            _context.Click += new EventHandler(contextBtn_Click);

            contextMenuStrip1.ImageList = imageList1;

            this._objectModelDeck.Name = objectModelDeckName= Constants.WorkspaceNames.ObjectModelDeck+DateTime.Now.Millisecond;

        }


        internal void AddRootNode()
        {
            InitializeSmartpart();

            TreeNode root = new TreeNode("Desktop");
            root.ImageKey = root.SelectedImageKey = "desktop";
            this.treeView1.Nodes.Add(root);
            panel3.Visible = true;
            Entities.Root desktop = new Entity.Root();
            desktop.BaseDirectory = _baseDir;
            desktop.ObjectModelName = Path.GetFileNameWithoutExtension(_objectModelfileName);
            this.propertyGrid1.SelectedObject = desktop;
            root.Tag = desktop;
            AddImagePanel();
            this._presenter.ClearText();
            tabControl1.TabPages.RemoveAt(1);
            this._presenter.SetBaseDirectory();
            //this._presenter.Write(Path.Combine(_baseDir, _objectModelfileName), String.Empty);
        }

        internal void InitializeSmartpart()
        {           
            var sp = this._presenter.WorkItem.SmartParts.Get(_propertyEditorName);

            if (sp == null)
            {
                _propertyEditorName = "PropertyEditor" + DateTime.Now.Millisecond.ToString();
                _propertyGridName = "PropertyGrid" + DateTime.Now.Millisecond.ToString();

                this._propertyEditor = this._presenter.WorkItem.SmartParts.AddNew<PropertyEditor>(_propertyEditorName);
                this.tabControl1 = this._propertyEditor.TabControl;

                this._propertyEditor.AddWorkspace();
                this._presenter.WorkItem.Workspaces[objectModelDeckName].Show(this._propertyEditor);

                 this._grid = this._presenter.WorkItem.SmartParts.AddNew<AutomationGrid>(_propertyGridName);                
                 this.propertyGrid1 = this._grid.Grid;
                this.propertyGrid1.PropertyValueChanged+=new PropertyValueChangedEventHandler(propertyGrid1_PropertyValueChanged);

                 WindowSmartPartInfo sp1 = new WindowSmartPartInfo();
                 sp1.Title = "Win32";
                 this._presenter.WorkItem.RootWorkItem.Workspaces[this._propertyEditor.workspacename].Show(this._grid, sp1);

                 WindowSmartPartInfo sp2 = new WindowSmartPartInfo();
                 sp2.Title = "Images";

                 this._imgEditor = this._presenter.WorkItem.SmartParts.Get(_imageEditorTitle) as ImageEditor.ImageEditor;

                 this._presenter.WorkItem.RootWorkItem.Workspaces[this._propertyEditor.workspacename].Show(this._imgEditor, sp2);

                 
                 
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                contextMenuStrip1.Show();
                AddContextMenuItem();
                contextMenuStrip1.Items.Add(_context);
            }
        }

        private void AddContextMenuItem()
        {
            contextMenuStrip1.Items.Clear();
            if (treeView1.SelectedNode != null)
            {
                if (treeView1.SelectedNode.Text == "Desktop")
                {
                    _context.Text = "Add Application";
                    _context.ImageKey = "application";
                }
                else if (treeView1.SelectedNode.Text.Contains("Application"))
                {
                    _context.Text = "Add Screen";
                    _context.ImageKey = "screen";
                }
                else if (treeView1.SelectedNode.Text.Contains("Screen"))
                {
                    _context.Text = "Add Control";
                    _context.ImageKey = "control";
                }
                else
                {
                    _context.Text = "Add Control";
                    _context.ImageKey = "control";
                }
            }
        }

        void contextBtn_Click(object sender, EventArgs e)
        {
            panel3.Visible = true;

            var child = InitializeNode();

            this.treeView1.SelectedNode.Nodes.Add(child);

            AddImagePanel();
            this._presenter.ClearText();

            if (_mode == Entity.ProjectMode.ImageCapture)
            {
                if (tabControl1.TabPages.Count == 2)
                    tabControl1.TabPages.RemoveAt(0);
            }
            Utilities.SavedTabs[Utilities.CurrentTab] = false;
            var key = Utilities.GetKey();
            if(key != null)
                Utilities.Editors[key] =false;
        }


        private void AddImagePanel()
        {
            if (_imgEditor == null)
            {
                string imgEditor = String.Empty;
                //if (IsUpdatedEditorTitle(_imageEditorTitle))
                //{
                //    imgEditor = Utilities.SequenceTimeStampsIME[Utilities.SelectedSequenceNumber];
                //}
                //else
                //{
                imgEditor = _imageEditorTitle;
                //}

                var t = this._presenter.WorkItem.State["imageEditors"] as Dictionary<string, ImageEditor.ImageEditor>;

                _imgEditor = t[imgEditor];
            }


            _imgEditor.Dock = DockStyle.Fill;

            if (tabControl1.TabPages.Count == 1)
            {
                TabPage imageCtrl = new TabPage("Images");
                this.tabControl1.TabPages.Add(imageCtrl);                
                
            }


            if (this.tabControl1.TabPages.Count == 2)
            {
                this.tabControl1.TabPages[1].Controls.Add(_imgEditor);
                //WindowSmartPartInfo sp = new WindowSmartPartInfo();
                //sp.Title = "Images";
                //this._presenter.WorkItem.RootWorkItem.Workspaces[Constants.WorkspaceNames.ImageWorkspace].Show(_imgEditor);
            }
            else
            {
                TabPage imageCtrl = new TabPage("Images");
                this.tabControl1.TabPages.Add(imageCtrl);
                this.tabControl1.TabPages[0].Controls.Add(_imgEditor);
            }

            this._presenter.AddImagePanel(this.tabControl1);
            if (this.treeView1.SelectedNode != null)
                _imgEditor.ShowApplicationPanel(this.treeView1.SelectedNode.Text);
        }

        private TreeNode InitializeNode()
        {
            var selectedNode = treeView1.SelectedNode;
            TreeNode child = null;
            Entity.BaseProperties ctrlProp = null;

            if (selectedNode.Text == "Desktop")
            {
                child = new TreeNode("Application " + _appCount++);
                child.ImageKey = child.SelectedImageKey = "application";

                ctrlProp = new Entity.ApplicationProperties();
                ((Entities.ApplicationProperties)ctrlProp).ApplicationType = "WinDesktop";
            }
            else if (selectedNode.Text.Contains("Application"))
            {
                child = new TreeNode("Screen " + _screenCount++);
                child.ImageKey = child.SelectedImageKey = "screen";

                ctrlProp = new Entity.BaseProperties();


            }
            else if (selectedNode.Text.Contains("Screen") || selectedNode.Text.Contains("Control"))
            {
                child = new TreeNode("Control " + _controlCount++);
                child.ImageKey = child.SelectedImageKey = "control";

                Entity.ControlProperties cp = new Entity.ControlProperties();
                ctrlProp = cp;
                ctrlProp.Type = "NA";
            }


            ctrlProp.State = new List<Entity.State>();
            //ctrlProp.ControlName = "Control Name " + _ctrlName++;
            this.treeView1.SelectedNode = child;
            ctrlProp.Name = child.Text;
            child.Tag = ctrlProp;
            propertyGrid1.SelectedObject = ctrlProp;
            AddImagePanel();



            return child;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            UpdateSelectedNodeProperties();
        }

        void UpdateImageState(string imagePath)
        {
            var ctrlProp = this.treeView1.SelectedNode.Tag as Entity.BaseProperties;
            Entity.State s = ctrlProp.State.FirstOrDefault(s1 => s1.Name == State);

            if (s == null)
            {
                s = new Entity.State();
                s.Area = new Entity.ImageArea();
                s.Name = State;
                ctrlProp.State.Add(s);
            }

            switch (Area)
            {
                case Entity.Area.Center:
                    s.Area.Center.Path = imagePath;
                    break;
                case Entity.Area.Below:
                    s.Area.Below.Path = imagePath;
                    break;
                case Entity.Area.Left:
                    s.Area.Left.Path = imagePath;
                    break;
                case Entity.Area.Right:
                    s.Area.Right.Path = imagePath;
                    break;
                case Entity.Area.Above:
                    s.Area.Above.Path = imagePath;
                    break;
                case Entity.Area.Validate:
                    s.Area.Validate.Path = imagePath;
                    break;
            }
        }

        internal void GenerateObjectModel()
        {
            this._presenter.GenerateObjectModel(this.treeView1, this.ucName);
        }

        internal void GenerateScript()
        {
            if (_root == null)
                MessageBox.Show("Object model needs to be saved before generating script", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (_root.Application.Count > 0)
            {
                this._presenter.GenerateScript(_root);
            }
            else
                throw new Exception("No applications found in model to generate script");
        }

        internal void Open(string fileName)
        {
            InitializeSmartpart();

            autoConfig = this._presenter.Deserialize(fileName);
            _mode = (Entity.ProjectMode)Enum.Parse(typeof(Entity.ProjectMode), autoConfig.ProjectMode);
            _baseDir = (String.IsNullOrEmpty(autoConfig.AppConfigs[0].BaseImageDir) || autoConfig.AppConfigs[0].BaseImageDir == "$")
                ? Path.GetDirectoryName(fileName) : autoConfig.AppConfigs[0].BaseImageDir;
            autoConfig.AppConfigs[0].BaseImageDir = _baseDir;
            this._presenter.SetBaseDirectory();
            this._presenter.BuildTree(autoConfig);
            var root = this.treeView1.TopNode.Tag as Entities.Root;
            root.ObjectModelName = _objectModelfileName;
            if (_mode == Entity.ProjectMode.Win32)
                AddImagePanel();
            else if (_mode == Entity.ProjectMode.ImageCapture)
                this.tabControl1.TabPages.RemoveAt(0);

            Utilities.Editors[this] = true;
        
            Utilities.SavedTabs[this._ucName] =true;
        }

        internal void DeleteNode()
        {
            if (treeView1.SelectedNode != treeView1.TopNode)
            {
                var confirm =
                    MessageBox.Show("Are sure you want to delete?", "IAP", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                {
                    this._presenter.Delete(this.treeView1.SelectedNode); 
                    this.treeView1.SelectedNode.Remove();
                    var key = Utilities.GetKey();
                    Utilities.SavedTabs[Utilities.CurrentTab] =  false;
                    if (key != null)
                        Utilities.Editors[key] = false;
                }
            }
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (e.ChangedItem.Label == "Name")
            {
                UpdateTreeNodeText(e.ChangedItem.Value.ToString());
            }
            else if (e.ChangedItem.Label == "BaseDirectory")
            {
                _baseDir = e.ChangedItem.Value.ToString();
                this._presenter.SetBaseDirectory();
            }
        }

        private void UpdateTreeNodeText(string name)
        {
            var node = this.treeView1.SelectedNode;
            if (node != null)
            {
                if (node.Text.Contains("Application"))
                {
                    node.Text = "Application - " + name;
                }
                else if (node.Text.Contains("Screen"))
                {
                    node.Text = "Screen - " + name;
                }
                else if (node.Text.Contains("Control"))
                {
                    node.Text = "Control - " + name;
                }
            }
        }

        internal void Close()
        {
            this._presenter.OnCloseView();
        }

        void IClose.Close()
        {
            this._presenter.OnCloseView();
            _imgEditor.Close();
            this._presenter.WorkItem.SmartParts.Remove(_imgEditor);

            var t1 = this._presenter.WorkItem.State["editors"] as Dictionary<string, Editor>;
            t1.Remove(this.ucName);

            var t2 = this._presenter.WorkItem.State["imageEditors"] as Dictionary<string, ImageEditor.ImageEditor>;
            t2.Remove(_imgEditor.ucName);

            _propertyEditor.Close();
            _grid.Close();

            this._presenter.WorkItem.SmartParts.Remove(_propertyEditor);
            this._presenter.WorkItem.SmartParts.Remove(_grid);



            this._presenter.WorkItem.Workspaces[Constants.WorkspaceNames.UtilWorkSpace].Close(this.tabControl1);
            this._presenter.WorkItem.Workspaces.Remove(this._objectModelDeck);
    
        }

        internal void UpdateSelectedNodeProperties()
        {           

            this.treeView1.Focus();
            var selecteNode = this.treeView1.SelectedNode;
            Entity.BaseProperties ctrl = null;
            Entity.Root root = null;
            if (null != selecteNode)
            {
                if (selecteNode.Text.Contains("Control"))
                    ctrl = selecteNode.Tag as Entity.ControlProperties;

                else if (selecteNode.Text.Contains("Desktop"))
                {
                    root = selecteNode.Tag as Entity.Root;
                }
                else if (selecteNode.Text.Contains("Screen"))
                {
                    ctrl = selecteNode.Tag as Entity.BaseProperties;
                }
                else
                {
                    ctrl = selecteNode.Tag as Entity.ApplicationProperties;

                }
            }

            if (ctrl == null)
            {
                propertyGrid1.SelectedObject = root;
                if (tabControl1.TabPages.Count == 2 && _mode == Entity.ProjectMode.Win32)
                    tabControl1.TabPages.RemoveAt(1);
                else if (tabControl1.TabPages.Count == 1 && _mode == Entity.ProjectMode.Win32)
                {
                    tabControl1.TabPages.RemoveAt(0);
                    TabPage desktop = new TabPage("Win32");
                    desktop.Controls.Add(propertyGrid1);
                    tabControl1.TabPages.Add(desktop);
                }

                else if (_mode == Entity.ProjectMode.ImageCapture)
                {
                    if (tabControl1.TabPages[0].Text == "Images")
                    {
                        tabControl1.TabPages.RemoveAt(0);
                        TabPage desktop = new TabPage("Win32");
                        desktop.Controls.Add(propertyGrid1);
                        tabControl1.TabPages.Add(desktop);
                    }
                }
            }
            else
            {
                propertyGrid1.SelectedObject = ctrl;
                if (_mode == Entity.ProjectMode.ImageCapture)
                {
                    tabControl1.TabPages.RemoveAt(0);
                }
                AddImagePanel();
                this._presenter.UpdateImageProperties(new object[] { this.treeView1.SelectedNode.Text, ctrl.State });

                if (ctrl.GetType() == typeof(Entity.ApplicationProperties))
                {
                    this._presenter.UpdateApplicationProperties((Entities.ApplicationProperties)ctrl);
                }
            }
        }

        internal void Replay()
        {
            if (autoConfig != null)
            {
                frmReplay replay = new frmReplay();
                replay.StartPosition = FormStartPosition.CenterParent;
                replay.AutoConfig = this.autoConfig;
                replay.BaseDir = this.BaseDir;
                replay.ShowForm();
            }
        }
    }
}

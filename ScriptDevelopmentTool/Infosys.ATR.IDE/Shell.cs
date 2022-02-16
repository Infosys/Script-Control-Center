using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infosys.ATR.UIAutomation.Entities;

namespace Infosys.ATR.DevelopmentStudio
{
    public delegate void Refresh();
    public delegate void Capture();
    public delegate void Save();
    public delegate void Edit();
    public delegate void TakeSnap();
    public delegate void FormClose();

    public partial class Shell : Form
    {
        public event Refresh OnRefresh;
        public event Capture OnCapture;
        public event Save OnSave;
        public event Edit OnEdit;
        public event TakeSnap OnTakeSnap;
        public event FormClose OnFormClose;
        private uc_ControlEditor ctrlEditor;

        public string Mode { get; set; }

        public Shell()
        {
            InitializeComponent();
            uc_ControlExplorer explorer = new uc_ControlExplorer();
            explorer.Dock = DockStyle.Fill;
            this.tabPage2.Controls.Add(explorer);
        }

        private void toolStripbtnOK_Click(object sender, EventArgs e)
        {

        }

        private void toolStripbtnRefresh_Click(object sender, EventArgs e)
        {
            if(OnRefresh !=null)
            {
                OnRefresh();
            }
        }

        private void toolStripbtnCtlCaptureOnMove_Click(object sender, EventArgs e)
        {
            if(OnCapture !=null)
                OnCapture();          
            
        }

        private void toolStripButton_SaveOM_Click(object sender, EventArgs e)
        {
            if(OnSave != null)
                OnSave();
        }

        private void toolStripButtonEditOM_Click(object sender, EventArgs e)
        {
            if (OnEdit != null)
                OnEdit();
            
        }

        private void toolStripButtonTakeSnap_Click(object sender, EventArgs e)
        {
            if(OnTakeSnap != null)
                OnTakeSnap();
        }

        private void Shell_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (OnFormClose != null)
                OnFormClose();
        }

        public void ShowControlEditor(string path)
        {
            TabPage editor = new TabPage();            
            tabControl1.TabPages.Add(editor);
            ctrlEditor = new uc_ControlEditor();
            ctrlEditor.Dock = DockStyle.Fill;
            ctrlEditor.ObjectModel = path;
            ctrlEditor.Render();
            editor.Text = "Control Editor";
            editor.Controls.Add(ctrlEditor);
        }

        public void ShowControlEditor(Desktop desktop)
        {
            if (!IfEditorExists())
            {
                TabPage editor = new TabPage();
                tabControl1.TabPages.Add(editor);
                ctrlEditor = new uc_ControlEditor();
                editor.Text = "Control Editor";
                editor.Controls.Add(ctrlEditor);
                ctrlEditor.Dock = DockStyle.Fill;
                ctrlEditor.Desktop = desktop;
                ctrlEditor.Render();
            }
            else
            {
                ctrlEditor.Desktop = desktop;
                ctrlEditor.Reload();
            }
           
        }

        private bool IfEditorExists()
        {
            var tabpages = this.tabControl1.TabPages;

            foreach (TabPage t in tabpages)
            {
                if (t.Text == "Control Editor" && Mode == "Web")
                {
                    return true;
                }
            }
            return false;
        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Mode = toolStripComboBox1.SelectedItem.ToString();
        }
    }
}

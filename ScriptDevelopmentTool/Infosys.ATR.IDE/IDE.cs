using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Configuration;

namespace Infosys.ATR.DevelopmentStudio
{
    public partial class IDE : Form
    {
        int splitterDistance = 0;
        bool isSplitterCollapsed = false;
        string _filePath = "";
        public IDE()
        {
            InitializeComponent();
        }

        public IDE(string[] args)
        {
            InitializeComponent();
            if (args.Length > 0)
                _filePath = args[0];
        }

        private void IDE_Load(object sender, EventArgs e)
        {
            //lblFind.Click += new EventHandler(lbl_Click);
            //lblWait.Click += new EventHandler(lbl_Click);
            //lblContains.Click += new EventHandler(lbl_Click);

            //lblFind.MouseHover += new EventHandler(lbl_MouseHover);
            //lblFind.MouseLeave += new EventHandler(lbl_MouseLeave);
            //lblWait.MouseHover += new EventHandler(lbl_MouseHover);
            //lblWait.MouseLeave += new EventHandler(lbl_MouseLeave);
            //lblContains.MouseHover += new EventHandler(lbl_MouseHover);
            //lblContains.MouseLeave += new EventHandler(lbl_MouseLeave);
            PopulateFunctions();
            splitterDistance = splitContainer1.SplitterDistance;
            splitContainer1.SplitterWidth = 6;

            //load file if provided
            if (!string.IsNullOrEmpty(_filePath))
            {
                if (System.IO.File.Exists(_filePath))
                {
                    rtxtEditor.Text = System.IO.File.ReadAllText(_filePath);
                }
            }
        }

        void lbl_MouseLeave(object sender, EventArgs e)
        {
            (sender as Label).ForeColor = Color.Black;
        }

        void lbl_MouseHover(object sender, EventArgs e)
        {
            (sender as Label).ForeColor = Color.Blue;
        }

        void lbl_Click(object sender, EventArgs e)
        {
            string text = (sender as Label).Text;
            rtxtEditor.AddText(text, Color.Blue);
        }

        private void btnGetCtl_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!isSplitterCollapsed)
            {
                splitContainer1.SplitterDistance = 30;
                isSplitterCollapsed = true;
                lblFuns.Text = "F\nu\nn\nc\nt\ni\no\nn\ns";
                splitContainer1.Panel1.BackColor = Color.LightGray;
            }
            else
            {
                splitContainer1.SplitterDistance = splitterDistance;
                isSplitterCollapsed = false;
                lblFuns.Text = "Functions";
                splitContainer1.Panel1.BackColor = Color.White;
            }
        }

        private void splitContainer1_Panel1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!isSplitterCollapsed)
            {
                splitContainer1.SplitterDistance = 30;
                isSplitterCollapsed = true;
                lblFuns.Text = "F\nu\nn\nc\nt\ni\no\nn\ns";
                splitContainer1.Panel1.BackColor = Color.LightGray;
            }
            else
            {
                splitContainer1.SplitterDistance = splitterDistance;
                isSplitterCollapsed = false;
                lblFuns.Text = "Functions";
                splitContainer1.Panel1.BackColor = Color.White;
            }
        }

        private void toolStripbtnGetCtl_Click(object sender, EventArgs e)
        {
            ControlExplorer exp = new ControlExplorer();
            exp.AutoStartControlExp += new ControlExplorer.AutoStartControlExpEventHandler(exp_AutoStartControlExp);
            exp.ShowDialog();
            //rtxtEditor.SelectionLength = 0;
            if (exp.Result != null)
                rtxtEditor.AddText(exp.Result, Color.Green);
        }

        void exp_AutoStartControlExp()
        {
            System.Threading.Thread.Sleep(500);
            toolStripbtnGetCtl_Click(null, null);
        }

        private void toolStripbtnRun_Click(object sender, EventArgs e)
        {

        }

        private void toolStripbtnSave_Click(object sender, EventArgs e)
        {

        }

        private void PopulateFunctions()
        {
            string functionSetting = ConfigurationManager.AppSettings["Functions"];
            if (!string.IsNullOrEmpty(functionSetting))
            {
                int x = 36, y = 35, yOffset = 28;
                string[] functions = functionSetting.Split(';');
                for (int i = 0; i < functions.Length; i++)
                {
                    Label lblFunc = new Label();
                    lblFunc.Location = new Point(x, y + i * yOffset);
                    lblFunc.Click += new EventHandler(lbl_Click);
                    lblFunc.MouseHover += new EventHandler(lbl_MouseHover);
                    lblFunc.MouseLeave += new EventHandler(lbl_MouseLeave);
                    lblFunc.Text = functions[i] + "(...)";
                    lblFunc.Font = new Font(lblFunc.Font.Name, lblFunc.Font.Size, FontStyle.Bold);
                    lblFunc.AutoSize = true;
                    splitContainer1.Panel1.Controls.Add(lblFunc);
                }
            }
        }
    }

    public static class RichTextBoxExtensions
    {
        public static void AddText(this RichTextBox box, string text, Color color)
        {
            //box.SelectionStart = box.TextLength;
            //box.SelectionLength = 0;

            box.SelectionColor = color;
            box.SelectedText = text + " ";
            box.SelectionColor = box.ForeColor;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using IMSWorkBench.Infrastructure.Interface;
using IMSWorkBench.Infrastructure.Interface.Constants;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.CompositeUI.WinForms;
namespace Infosys.ATR.DevelopmentStudio
{
    public partial class IdeUC : UserControl
    {
        int splitterDistance = 0;
        bool isSplitterCollapsed = false;
        string _filePath = "";
        //public string Richtext { get; set; }
        private string richText;
        private string textToSave;

        public string TextToSave
        {
            get { return textToSave; }
            set {rtxtEditor.AddText(value, Color.Green); }
        }

        public string RichText
        {
            get { return richText; }
            set
            {
                rtxtEditor.AddText(value, Color.Green);
            }
        }

        public IdeUC(string[] args)
        {
            InitializeComponent();
            if (args.Length > 0)
                _filePath = args[0];
        }
        public IdeUC()
        {
            InitializeComponent();
        }

        private void IDE_Load(object sender, EventArgs e)
        {
            this.Dock = DockStyle.Fill;
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
        public void GetCtl_Click()
        {
            rtxtEditor.Name = "rtxtEditor";
            //ControlExplorer exp = new ControlExplorer();
            //exp.AutoStartControlExp += new ControlExplorer.AutoStartControlExpEventHandler(exp_AutoStartControlExp);
            //exp.ShowDialog();
            //rtxtEditor.SelectionLength = 0;
            //if (expRichtext != null)
            //    rtxtEditor.AddText(expRichtext, Color.Green);

        }

        private void splitContainer1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

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
                splitContainer1.SplitterDistance = 200;//splitterDistance;
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
        [EventSubscription(EventTopicNames.Save, ThreadOption.UserInterface)]
        public void Save(object sender, EventArgs e)
        {
            string text = rtxtEditor.Text;

        }
    }

    //public static class RichTextBoxExtensions
    //{
    //    public static void AddText(this RichTextBox box, string text, Color color)
    //    {
    //        //box.SelectionStart = box.TextLength;
    //        //box.SelectionLength = 0;

    //        box.SelectionColor = color;
    //        box.SelectedText = text + " ";
    //        box.SelectionColor = box.ForeColor;
    //    }
    //}
}

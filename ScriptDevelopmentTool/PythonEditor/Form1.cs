using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ScintillaNET.Design;

namespace PythonEditor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //ScintillaNET.Scintilla editor = new ScintillaNET.Scintilla();
            //editor.ConfigurationManager.Language = "python";
            ////editor.Margins.Margin0.Width = 35;
            //editor.Lines.Current.FoldExpanded = true;
            
            //editor.Dock = DockStyle.Fill;
            //this.Controls.Add(editor);
            scintilla1.ConfigurationManager.Language = "python";
            scintilla1.Margins.Margin0.Width = 25;
        }

        private void btnAddText_Click(object sender, EventArgs e)
        {
            scintilla1.AddText("Hello");
        }

        private void btnReadText_Click(object sender, EventArgs e)
        {
            MessageBox.Show(scintilla1.Text);
        }
    }
}

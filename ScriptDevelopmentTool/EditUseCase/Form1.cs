using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Infosys.ATR.UIAutomation.Entities;
using Infosys.ATR.UIAutomation.SEE;
using Infosys.ATR.ScriptEditor;

namespace EditUseCase
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string usecasePath = @"D:\IMS\TFS\Usecase\20140210174253.atrwb";
        private void Form1_Load(object sender, EventArgs e)
        {
            UseCaseEditor editor = new UseCaseEditor(usecasePath);
            editor.Dock= DockStyle.Fill;
            this.Controls.Add(editor);
        }
    }
}

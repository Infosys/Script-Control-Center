using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using IMSWorkBench.Infrastructure.Library.Services;

namespace Infosys.ATR.Editor.Views.Create
{
    public partial class FileCreate : UserControl,IFileCreate
    {
        public FileCreate()
        {
            InitializeComponent();
            Clear();
            pnlError.Hide();
            comboBox1.Text = comboBox1.Items[0].ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Clear();
        }

        internal void Clear()
        {
            txtDir.Text = txtObjectModel.Text = lblMessage.Text = String.Empty;
            pnlError.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtObjectModel.Text))
            {
                Error("Enter object model name");
                return;
            }

            if (String.IsNullOrEmpty(txtDir.Text))
            {
                Error("Set a base directory");
                return;
            }

            this._presenter.ToggleDeckPanel_handler();
            this._presenter.Update(new string[]{txtObjectModel.Text, txtDir.Text,comboBox1.Text});
            this._presenter.OnCloseView();
            Logger.Log("ControlGraph", "NewObjectTree",
              "New ObjectTree " + txtObjectModel.Text);
        }

        private void Error(string message)
        {
            pnlError.Show();
            lblMessage.Text = message;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog selectFolder = new FolderBrowserDialog();
            var select = selectFolder.ShowDialog();

            if (select == DialogResult.OK)
            {
                txtDir.Text = selectFolder.SelectedPath;
            }
        }
    }
}

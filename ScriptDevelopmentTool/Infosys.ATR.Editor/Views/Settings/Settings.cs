using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Infosys.ATR.Editor.Views
{
    public partial class Settings : UserControl,ISettings
    {
        public Settings()
        {
            InitializeComponent();
            textBox1.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Executables (*.exe)|*.exe";

            if (open.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = open.FileName;
                this._presenter.Set(open.FileName);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this._presenter.OnCloseView();
        }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Infosys.ATR.WFDesigner
{
    public partial class ModuleLauncher : Form
    {

        delegate void WF();
        event WF LaunchWF;

        delegate void OM();
        event OM LaunchOM;

        delegate void Script();
        event Script LaunchScript;

        public ModuleLauncher()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            LaunchWF();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            LaunchOM();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            LaunchScript();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Close();
        }
    }
}

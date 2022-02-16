using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Infosys.ATR.WinUIAutomationRuntimeWrapper.Views
{
    public partial class AppStarting : Form
    {
        public AppStarting()
        {
            InitializeComponent();
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AppStarting_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
        }
    }
}

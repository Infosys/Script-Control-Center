using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Infosys.ATR.WinUIAutomationRuntimeWrapper;

namespace TrackActivities
{
    public partial class Auxi : Form
    {
        public Auxi()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AutomationFacade facade = new AutomationFacade();
            string str = facade.Read();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InitiateStop
{
    public partial class Form1 : Form
    {
        const string STOP_FILE_LOCATION = "stop.iap";
        public Form1()
        {
            InitializeComponent();
        }

        private void stop_Click(object sender, EventArgs e)
        {
            System.IO.File.WriteAllText(STOP_FILE_LOCATION, string.Format("At {0}, stop requested", DateTime.Now.ToString()));            
        }

        //protected override void OnFormClosing(FormClosingEventArgs e)
        //{
        //    if(!ConfimClosing())
        //        e.Cancel = true;
        //    base.OnFormClosing(e);
        //}

        private bool ConfimClosing()
        {
            bool ok = false;
            DialogResult confirm = MessageBox.Show("It is advised not to close this application explicitly. If closed will not be able initiate stop of any long running activity. Do you yet want to close this?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == System.Windows.Forms.DialogResult.Yes)
                ok = true;
            return ok;
        }
    }
}

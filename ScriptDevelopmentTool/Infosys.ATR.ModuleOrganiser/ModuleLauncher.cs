using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Infosys.ATR.ModuleOrganiser
{
    public partial class ModuleLauncher : Form
    {
        public delegate void WF();
        public event WF LaunchWF;

        public delegate void OM();
        public event OM LaunchOM;

        public delegate void Script();
        public event Script LaunchScript;

        public ModuleLauncher()
        {
            InitializeComponent();
            this.panel1.Padding = new Padding(1);
            this.panel2.Padding = new Padding(1);
            this.panel3.Padding = new Padding(1);
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x0084;

            switch (m.Msg)
            {
                case WM_NCHITTEST:
                    return;
            }

            base.WndProc(ref m);
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
            LaunchWF();
            
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
            LaunchOM();
            
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
            LaunchScript();
            
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LaunchWF();
            this.Close();
        }


        private void pictureBox1_MouseHover(object sender, EventArgs e)
        {
            this.panel1.BackColor = System.Drawing.ColorTranslator.FromHtml("#0000ee"); ;
            this.Cursor = Cursors.Hand;
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            this.panel1.BackColor = Color.White;
            this.Cursor = Cursors.Arrow;
        }

        private void pictureBox2_MouseHover(object sender, EventArgs e)
        {
            this.panel2.BackColor = System.Drawing.ColorTranslator.FromHtml("#0000ee");
            this.Cursor = Cursors.Hand;
        }

        private void pictureBox2_MouseLeave(object sender, EventArgs e)
        {
            this.panel2.BackColor = Color.White;
            this.Cursor = Cursors.Arrow;
        }

        private void pictureBox3_MouseHover(object sender, EventArgs e)
        {
            this.panel3.BackColor = System.Drawing.ColorTranslator.FromHtml("#0000ee");
            this.Cursor = Cursors.Hand;
        }
         

        private void pictureBox3_MouseLeave(object sender, EventArgs e)
        {
            this.panel3.BackColor = Color.White;
            this.Cursor = Cursors.Arrow;
        }


    }
}

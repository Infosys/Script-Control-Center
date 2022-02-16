/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestColorBasedDetection
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Red Clicked");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("White Clicked");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Blue Clicked");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Blue Clicked");
        }

        private void chkHide_CheckedChanged(object sender, EventArgs e)
        {
            if (chkHide.Checked)
            {
                button4.Visible = false;
            }
            else
                button4.Visible = true;
        }
    }
}

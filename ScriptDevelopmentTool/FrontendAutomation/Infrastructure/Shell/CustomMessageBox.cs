using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IMSWorkBench.Infrastructure.Shell
{
    public partial class CustomMessageBox : Form
    {

        public string Message { get { return txtMessage.Text;} set { txtMessage.Text = value; } }

        public CustomMessageBox()
        {
            InitializeComponent();
        }
    }
}

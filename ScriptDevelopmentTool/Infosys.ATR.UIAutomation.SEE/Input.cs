using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Runtime.InteropServices;

namespace Infosys.ATR.UIAutomation.SEE
{
    public partial class Input : Form
    {
        //user32 interfaces
        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern IntPtr SetForegroundWindowNative(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        public Input()
        {
            InitializeComponent();
        }

        public string TextInput { get; set; }

        private void btnDone_Click(object sender, EventArgs e)
        {
            TextInput = txtText.Text;
            this.Close();
        }

        private void Input_Load(object sender, EventArgs e)
        {
            //SetForegroundWindowNative(this.Handle);
            txtText.Focus();
        }
    }
}

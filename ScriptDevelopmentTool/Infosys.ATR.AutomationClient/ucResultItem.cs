using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Infosys.ATR.AutomationEngine.Contracts;

namespace Infosys.ATR.AutomationClient
{
    public partial class ucResultItem : UserControl
    {
        string message = "";
        bool expanded = false;
        public ucResultItem(Result result, string nodeName)
        {
            InitializeComponent();
            lblNode.Text = nodeName;
            if (result.IsSuccess)
            {
                lblStatus.Text = "Success";
                lblStatus.ForeColor = Color.Green;
                message = result.SuccessMessage;
                if (!string.IsNullOrEmpty(message) && message.Length > 50)
                    lblMessage.Text = message.Substring(0, 50) + "...";
                else
                    lblMessage.Text = message;
            }
            else
            {
                lblStatus.Text = "Fail";
                lblStatus.ForeColor = Color.Red;
                 message = result.ErrorMessage;
                 if (!string.IsNullOrEmpty(message) && message.Length > 50)
                     lblMessage.Text = message.Substring(0, 50) + "...";
                 else
                     lblMessage.Text = message;
            }
            
        }

        private void lblMessage_DoubleClick(object sender, EventArgs e)
        {
            if (!expanded)
            {
                txtCompleteMsg.Text = message;
                this.Height = 100;
                expanded = true;
            }
            else
            {
                this.Height = 40;
                expanded = false;
            }
        }
    }
}

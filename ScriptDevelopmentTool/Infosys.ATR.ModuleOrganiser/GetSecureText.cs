using Infosys.WEM.SecureHandler;
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
    public partial class GetSecureText : Form
    {
        public GetSecureText()
        {
            InitializeComponent();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPlain.Text))
                return;

             Clipboard.SetText(txtResult.Text);  
        }

        private void btnGenText_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPlain.Text))
                return;
            else
                txtResult.Text=SecurePayload.Secure(Convert.ToString(txtPlain.Text), "IAP2GO_SEC!URE");
        }       
    }
}

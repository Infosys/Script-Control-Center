using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Infosys.ATR.ScriptsRepository.Views
{
    public partial class Info : Form
    {
        public Info()
        {
            InitializeComponent();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtOutput.Text);  
        }

        private void btnClose_Click(object sender, EventArgs e)
        {   
            this.Close();
        }
    }
}

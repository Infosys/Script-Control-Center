using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Infosys.ATR.WFDesigner.Views
{
    public partial class Transaction : UserControl, ITransaction,IClose
    {

        public Transaction()
        {

        }

        

        private void cmbState_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        public bool Close()
        {
            this._presenter.OnCloseView();
            return true;
        }
    }
}

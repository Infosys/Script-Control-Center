using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Practices.CompositeUI.EventBroker;
using Infosys.ATR.Editor.Constants;
using IMSWorkBench.Infrastructure.Interface;

namespace Infosys.ATR.WFDesigner.Views.Transaction
{
    public partial class uclTransStatus : UserControl
    {

        public delegate void delTrans(string text);

        public event delTrans TransactionState; 
       
       public uclTransStatus(Color pnlColor, string transNumber, Color numColor, string status, Color crlStatus)
        {
            InitializeComponent();
            this.pnlTransStatus.BackColor = pnlColor;
            this.btnTansNumber.Text = transNumber;
            this.btnTansNumber.ForeColor = numColor;
            this.BackColor = pnlColor;
            this.lblTransstatus.Text = status;
            this.lblTransstatus.ForeColor = numColor;
            this.toolTip1.SetToolTip(this.btnTansNumber, string.Format("Click here for details"));
        }

        private void btnTansNumber_Click(object sender, EventArgs e)
        {
            TransactionState(this.lblTransstatus.Text);
        }       
    }
} 

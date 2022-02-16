using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Infosys.ATR.AutomationClient
{
    public partial class IAPNodeResults : Form
    {
        public IAPNodeResults(List<IapNodeResultMapping> results)
        {
            InitializeComponent();
            if(results != null && results.Count>0)
            {
                foreach (var result in results)
                {
                    ucResultItem item = new ucResultItem(result.ExecResult, result.NodeName);
                    item.Dock = DockStyle.Top;
                    pnlResults.Controls.Add(item);
                }
            }
        }
    }
}

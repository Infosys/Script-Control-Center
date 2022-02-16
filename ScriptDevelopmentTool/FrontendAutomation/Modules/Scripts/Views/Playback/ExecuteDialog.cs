using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IMSWorkBench.Scripts.Constants;

namespace IMSWorkBench.Scripts
{
    public partial class ExecuteDialog : Form
    {
        string selectedRb = string.Empty;
        public string TheValue
        {
            get { return selectedRb; }
        }
        public ExecuteDialog()
        {
            InitializeComponent();
        }
        public ExecuteDialog(List<string> options)
        {
            InitializeComponent();
            if (options != null)
            {
                int i = 1;
               
                foreach (string s in options)
                {
                   
                    if (s ==Constants.ConstantNames.EXECUTE_SIKULI)
                    {
                        RadioButton rdo1 = new RadioButton();
                        rdo1.Name ="rbExecuteSikuli";
                        rdo1.Text = ConstantNames.EXECUTE_SIKULI;
                        rdo1.AutoSize = true;
                        rdo1.Location = new Point(5, 30* i+2);
                       this.groupBox1.Controls.Add(rdo1);
                       i++;
                    }
                    else if (s == ConstantNames.EXECUTE_PYTHON)
                    {
                        RadioButton rdo2 = new RadioButton();
                        rdo2.Name = "rbExecutePython";
                        rdo2.Text = ConstantNames.EXECUTE_PYTHON;
                        rdo2.Location = new Point(5, 30 * i+2);
                        this.groupBox1.Controls.Add(rdo2);
                        i++;
                       
                    }
                    else if (s == ConstantNames.EXECUTE_ATR)
                    {
                        RadioButton rdo3 = new RadioButton();
                        rdo3.Name = "rbExecuteATR";
                        rdo3.Text =ConstantNames.EXECUTE_ATR;
                        rdo3.AutoSize = true;
                         rdo3.Location =new Point(5, 30 * i+2);
                        this.groupBox1.Controls.Add(rdo3);
                        i++;
                    }
                }
            }
            
        }

        private void btnExceute_Click(object sender, EventArgs e)
        {
            var checkedRadio = new[] { groupBox1 }
                   .SelectMany(g => g.Controls.OfType<RadioButton>()
                                            .Where(r => r.Checked));
           
            foreach (var c in checkedRadio)
            {
                selectedRb = c.Text;
                break;
            }
            this.Close();

                
           
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

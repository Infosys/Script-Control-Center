using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Infosys.IAP.CommonClientLibrary
{
    public partial class ProgressStatus : Form
    {
        protected float percent = 0.0f; 
        public float Value
        {
            get { return percent; }
            set
            {
                if (value < 0) value = 0;
                else if (value > 100) value = 100;
                percent = value;
                uclProgressStatus1.Value = value;
                this.Update();
            }
        }

        public ProgressStatus()
        {
            InitializeComponent();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Infosys.ATR.Editor.Views
{
    public partial class AutomationGrid : UserControl,IAutomationGrid,IClose
    {
        public AutomationGrid()
        {
            InitializeComponent();
        }

        public PropertyGrid Grid
        {
            get { return propertyGrid1; }
            set { propertyGrid1 = value; }
        }

        public void Close()
        {
            this._presenter.OnCloseView();
        }

        public void Test()
        {
        }

        public string ucName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}

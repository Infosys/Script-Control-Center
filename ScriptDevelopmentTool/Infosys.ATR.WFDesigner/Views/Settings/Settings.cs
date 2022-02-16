using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;

using Infosys.ATR.WFDesigner.Entities;
using Infosys.WEM.Service.Contracts.Data;

namespace Infosys.ATR.WFDesigner.Views
{
    public partial class Settings : UserControl, ISettings, IClose
    {

        public List<Category> Categories { get; set; }

        public Settings()
        {
            InitializeComponent();        
        }       

        public bool Close()
        {
            this._presenter.OnCloseView();
            return true;
        }


        private Tuple<TreeNode, Category> _catDetails;
        public Tuple<TreeNode, Category> Catdetails
        {
            get
            {
                return _catDetails;
            }
            set
            {
                _catDetails = value;
            }
        }

        public void ShowCatDetails()
        {
            txtName.Text = "";
            txtDescription.Text = "";
            if (_catDetails != null)
            {
                Category c = _catDetails.Item2;
                if (c != null)
                {
                    txtName.Text = c.Name;
                    txtDescription.Text = c.Description;
                }
                //else
                //{
                //    txtName.Text = "";
                //    txtDescription.Text = "";
                //}
            }
        }
    }
}

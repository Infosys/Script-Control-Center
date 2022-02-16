using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Infosys.ATR.WFDesigner.Views
{
    public partial class WorkflowParam : UserControl
    {
        Entities.WorkflowParameterPE _entity;

        public WorkflowParam(Entities.WorkflowParameterPE entity)
        {
            InitializeComponent();
            _entity = entity;
            lblName.Text = entity.Name;
            txtValue.Text = entity.DefaultValue;
            lblMandatory.Visible = entity.IsMandatory;
            if (entity.IsSecret)
            {
                txtValue.UseSystemPasswordChar = true;
            }
        }

        public Entities.WorkflowParameterPE Entity
        {
            get
            {
                _entity.DefaultValue = txtValue.Text.Trim();
                return _entity;
            }
        }

        public void Reset()
        {
            txtValue.Text = "";
        }
    }
}

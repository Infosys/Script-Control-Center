using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;

using Infosys.LicenseValidationClient;

namespace Infosys.ATR.ModuleLoader.Views
{
    public partial class ModuleSelector : Form, IModuleSelector
    {
        List<Module> _modules = new List<Module>();
        internal string selected;
        bool terminate = true;

        public ModuleSelector()
        {
            InitializeComponent();

            //the below validation will be moved to the respective module controller-> run

            //ValidationResult result = Validator.Validate();
            //if (result.IsSuccess && result.FeaturesAllowed!= null && result.FeaturesAllowed.Count>0)
            //{
            //    foreach (Feature feature in result.FeaturesAllowed)
            //    {
            //        switch (feature)
            //        {
            //            case Feature.ObjectModelExplorer:
            //                _modules.Add(new Module { Name = "Object Model", Value = "Object Model" });
            //                break;
            //            case Feature.ScriptRepository:
            //                _modules.Add(new Module { Name = "Script Repository", Value = "Script Repository" });
            //                break;
            //            case Feature.WorkflowDesigner:
            //                _modules.Add(new Module { Name = "WF Designer", Value = "WF Designer" });
            //                break;
            //        }
            //    }
            //}

            var modulestoLoad = ConfigurationManager.AppSettings["ModulesToLoad"];
            var modules = modulestoLoad.Split(',');

            for (int i = 0; i < modules.Length; i++)
            {
                _modules.Add(new Module { Name = modules[i], Value = modules[i] });
            }
                
            LoadModules();
        }

        private void LoadModules()
        {
            cmbModules.DataSource = _modules;
            cmbModules.DisplayMember = "Name";
            cmbModules.ValueMember = "Value";

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            terminate = false;
            selected = cmbModules.SelectedValue.ToString();
        }

        private void ModuleSelector_Load(object sender, EventArgs e)
        {

        }

        private void ModuleSelector_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(terminate)
                this._presenter.TerminateApp_Handler();
        }
    }

    public class Module
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}

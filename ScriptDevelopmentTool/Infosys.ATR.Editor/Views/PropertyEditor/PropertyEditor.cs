using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.ComponentModel;
using System.Windows.Automation;

using Entity = Infosys.ATR.Editor.Entities;
using Infosys.ATR.UIAutomation.Entities;
using Infosys.ATR.DevelopmentStudio;
using IMSWorkBench.Infrastructure.Interface.Services;
using Infosys.ATR.Editor.Services;

namespace Infosys.ATR.Editor.Views
{
    public partial class PropertyEditor : UserControl, IPropertyEditor, IClose
    {
        internal string workspacename; 
        public PropertyEditor()
        {
            InitializeComponent();
            this._imgWorkspace.Name = workspacename = Constants.WorkspaceNames.ImageWorkspace + DateTime.Now.Millisecond;
        }

        public void AddWorkspace()
        {            
            this._presenter.WorkItem.RootWorkItem.Workspaces.Add(this._imgWorkspace,workspacename);
        }

        public void Close()
        {
            this._presenter.OnCloseView();            
            this._presenter.WorkItem.Workspaces.Remove(this._imgWorkspace);
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

        public TabControl TabControl
        {
            get { return this._imgWorkspace; }
            
        }
       
    }
}

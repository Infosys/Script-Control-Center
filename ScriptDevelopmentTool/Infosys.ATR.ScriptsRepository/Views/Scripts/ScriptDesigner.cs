using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Infosys.ATR.CommonViews;

namespace Infosys.ATR.ScriptRepository.Views
{
    public partial class ScriptDesigner : UserControl, IScriptDesigner, IClose
    {
        string _ucName;
        PythonIDE ide = null;
        Mode _opMode;
        string _title;

        public ScriptDesigner()
        {
            InitializeComponent();
        }

        public void Close()
        {
            this._presenter.OnCloseView();
        }

        public string ucName
        {
            get
            {
                return _ucName;
            }
            set
            {
                _ucName = value;
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
            }
        }

        internal void ShowEditor()
        {
            ide = new PythonIDE();
            ide.Dock = DockStyle.Fill;
            ide.TextChange_Handler += ide_TextChange_Handler;
            this.splitContainer1.Panel1.Controls.Add(ide);
        }

        void ide_TextChange_Handler(bool isDirty)
        {
            Infosys.ATR.ScriptRepository.Save.ScriptTabs[this] = isDirty;  
        }


        internal void New()
        {
            ShowScriptDetails();
        }

        internal void Edit()
        {
            ShowScriptDetails();
        }

        private void ShowScriptDetails()
        {
            this.splitContainer1.Panel2Collapsed = false;
            ScriptDetails sd = null;
            if (Script == null)
                sd = new ScriptDetails(null, "", true, scriptExtention, scriptPath);
            else
            {
                if(IsGeneratedScript)
                    sd = new ScriptDetails(Script, Script.Id, true, scriptExtention, scriptPath,false,true);
                else if(IsIapPackage)
                    sd = new ScriptDetails(Script, Script.Id, true, scriptExtention, scriptPath,true);
                else
                    sd = new ScriptDetails(Script, Script.Id, false, "");
            }

            sd.Dock = DockStyle.Fill;            
            sd.ShowScriptExtension();
            sd.DisableRun();
            sd.ReadScript_Handler += sd_ReadScript_Handler;
            sd.ScriptDesigner = this;
            this._opMode = Mode.Edit;
            this.splitContainer1.Panel2.Controls.Add(sd);

            this._presenter.DisablePublish();
        }

        byte[] sd_ReadScript_Handler()
        {
            if (!String.IsNullOrEmpty(this.ide.RichText))
            {
                string str = this.ide.RichText;
                return System.Text.ASCIIEncoding.UTF8.GetBytes(str);
            }
            else
                return null;
        }

        public void DisplayOutput(List<ExecutionResultView> e)
        {
            this._presenter.ShowOutputView_Handler(e);
        }

        internal string RichText
        {
            get
            {
                if (this.ide != null)
                    return this.ide.RichText; 
                else return String.Empty; 
            }
            set
            {
                if (this.ide == null)
                {
                    ShowEditor();
                }

                if (Script != null)
                {
                    if (!Script.ScriptType.ToLower().Equals("iap"))
                        this.ide.RichText = value;
                }
                else
                    this.ide.RichText = value;
            }
        }

        

        public Models.Script Script { get; set; }


        public Mode OpMode
        {
            get
            {
                return _opMode;
            }
            set
            {
                _opMode = value;
            }
        }

        public string scriptName { get; set; }

        public string scriptExtention { get; set; }
        public string scriptPath { get; set; }

        public bool IsIapPackage { get; set; }
        public bool IsGeneratedScript { get; set; }
    }
}

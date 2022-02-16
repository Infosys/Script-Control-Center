using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Infosys.ATR.UIAutomation.SEE
{
    public partial class PlayBackBase : UserControl
    { 
        //event to send the list of applications
        public class ApplicationEventArgs : EventArgs
        {
            public List<string> ApplicationList { get; set; }
            public Entities.UseCase UseCaseCaptured { get; set; }
        }
        public delegate void ApplicationListEventHandler(ApplicationEventArgs e);
        public event ApplicationListEventHandler ApplicationListFetched;

        public PlayBackBase()
        {
            InitializeComponent();            
        }

        protected virtual void OnApplicationListFetched(ApplicationEventArgs e)
        {
            if (ApplicationListFetched != null)
                ApplicationListFetched(e);
        }
    }
}

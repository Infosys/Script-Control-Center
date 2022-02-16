using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace Infosys.ATR.AutomationEngine
{
    [RunInstaller(true)]
    public partial class AutomationEngineInstaller : System.Configuration.Install.Installer
    {
        public AutomationEngineInstaller()
        {
            //InitializeComponent();
            serviceProcessInstaller1 = new ServiceProcessInstaller();
            serviceProcessInstaller1.Account = ServiceAccount.LocalSystem;
            serviceInstaller1 = new ServiceInstaller();
            serviceInstaller1.ServiceName = "IAP.AutomationEngine";
            serviceInstaller1.DisplayName = "IAP.AutomationEngine";
            serviceInstaller1.Description = "Service to host the interface for executing work flow and script in a node";
            serviceInstaller1.StartType = ServiceStartMode.Automatic;
            Installers.Add(serviceProcessInstaller1);
            Installers.Add(serviceInstaller1);

        }
    }
}

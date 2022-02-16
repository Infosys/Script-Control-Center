using System;
using System.Collections;
using System.Configuration.Install;
using System.ServiceProcess;
using System.ComponentModel;

namespace WEM.Host.ConsoleApp
{
    [RunInstaller(true)]
    public class MyProjectInstaller : Installer
    {
        private ServiceInstaller serviceInstaller1;
        private ServiceProcessInstaller processInstaller;

        public MyProjectInstaller()
        {
            // Instantiate installers for process and services.
            processInstaller = new ServiceProcessInstaller();
            serviceInstaller1 = new ServiceInstaller();

            // The services run under the system account.
            processInstaller.Account = ServiceAccount.LocalSystem;

            // The services are started manually.
            serviceInstaller1.StartType = ServiceStartMode.Manual;

            // ServiceName must equal those on ServiceBase derived classes.
            serviceInstaller1.Description = "Workflow Execution Manager services";
            serviceInstaller1.DisplayName = "WEM Script Executor";
            serviceInstaller1.ServiceName = "WEMHostService";

            // Add installers to collection. Order is not important.
            Installers.Add(serviceInstaller1);
            Installers.Add(processInstaller);
        }

    }
}

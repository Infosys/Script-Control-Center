using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

using Infosys.ATR.AutomationExecutionLib;
using Infosys.WEM.Client;
using Infosys.WEM.Node.Service.Contracts.Message;
using System.ServiceModel;
using System.Net;
using System.Net.Sockets;
using System.Configuration;
using System.ServiceModel.Description;
using System.Reflection;
using System.Management;

namespace Infosys.ATR.AutomationEngine
{
    public partial class AutomationEngine : ServiceBase
    {
        ServiceHost IAPHost = null; //to be used in the start and stop handler for the windows service
        
        public AutomationEngine()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //if the host is up then first stop it
            if (IAPHost != null)
                IAPHost.Close();
            #region approach 1- configure service end points thru the code
            /*
            //get the ip address and name of the hosting machine
            string hostName = Dns.GetHostName();
            string ipAddress = "";
            IPHostEntry hostEntry = Dns.GetHostEntry(hostName); // this returns all the Ip addresses assigned to the machine
            foreach (IPAddress ip in hostEntry.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipAddress = ip.ToString();
                    break;
                }
            }

            //get the port numbers
            string httpPort = ConfigurationManager.AppSettings["httpPort"], tcpPort = ConfigurationManager.AppSettings["tcpPort"];
            if (!string.IsNullOrEmpty(httpPort) && !string.IsNullOrEmpty(tcpPort))
            {
                string httpAddress = "http://" + hostName + ":" + httpPort + "/iap";
                string tcpAddress = "net.tcp://" + ipAddress + ":" + tcpPort + "/iap"; //check if even for tcp, hostname could be used, to avoid dynamic ip issue

                Uri[] addressbase = { new Uri(httpAddress), new Uri(tcpAddress) };
                IAPHost = new ServiceHost(typeof(NodeService), addressbase);

                //provide support for the publication of service metadata
                ServiceMetadataBehavior svcBehavior = new ServiceMetadataBehavior();
                svcBehavior.HttpGetEnabled = true;
                IAPHost.Description.Behaviors.Add(svcBehavior);

                NetTcpBinding tcpbind = new NetTcpBinding();
                IAPHost.AddServiceEndpoint(typeof(INodeService), tcpbind, tcpAddress);
                IAPHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexTcpBinding(), "mex");

                //WebHttpBinding restBind = new WebHttpBinding();
                //IAPHost.AddServiceEndpoint(typeof(INodeService), restBind, httpAddress);
                //IAPHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");

                BasicHttpBinding basicBind = new BasicHttpBinding();
                IAPHost.AddServiceEndpoint(typeof(INodeService), basicBind, httpAddress);
                IAPHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");

                IAPHost.Open();
                //then register the node
            }
            */
            #endregion

            #region approach 2- configure service end points using details in config file e.g. app.config
            IAPHost = new ServiceHost(typeof(NodeService));
            IAPHost.Open();
            #endregion

            //then register the node
            //string nodeRegisterSrv = ConfigurationManager.AppSettings["NodeRegisterService"];
            //if (!string.IsNullOrEmpty(nodeRegisterSrv))
            //{
            //reading the register node service end points details from the client configuration instead of appsettings  
            RegisteredNodes nodeclient = new RegisteredNodes();
            RegisterReqMsg req = new RegisterReqMsg();
            req.Node = new Infosys.WEM.Node.Service.Contracts.Data.RegisteredNode();
            req.Node.DotNetVersion = GetDotnetVersion();
            req.Node.ExecutionEngineSupported = int.Parse(ConfigurationManager.AppSettings["ExecutionEngineSupported"]);
            req.Node.HostMachineName = GetMachineName();
            req.Node.HostMachineDomain = GetMachineDomain(req.Node.HostMachineName);
            req.Node.HttpPort = int.Parse(ConfigurationManager.AppSettings["httpPort"]);
            req.Node.Is64Bit = Is64BitOS();
            req.Node.OSVersion = GetOSName();
            req.Node.State = Infosys.WEM.Node.Service.Contracts.Data.NodeState.Active;
            req.Node.TcpPort =int.Parse(ConfigurationManager.AppSettings["tcpPort"]);
            req.Node.WorkflowServiceVersion = GetServiceVersion();
            req.Node.CompanyId = int.Parse(ConfigurationManager.AppSettings["Company"]);

            RegisterResMsg res = nodeclient.ServiceChannel.Register(req);
            //}
        }

        protected override void OnStop()
        {
            //first un-register the node and then close the host
            //string nodeRegisterSrv = ConfigurationManager.AppSettings["NodeRegisterService"];
            //if (!string.IsNullOrEmpty(nodeRegisterSrv))
            //{
            //reading the register node service end points details from the client configuration instead of appsettings 
            RegisteredNodes nodeclient = new RegisteredNodes();
            UnRegisterReqMsg req = new UnRegisterReqMsg();                
            req.MachineName = GetMachineName();
            req.Domain = GetMachineDomain(req.MachineName);

            UnRegisterResMsg res = nodeclient.ServiceChannel.UnRegister(req);
            //}

            if (IAPHost != null)
            {
                IAPHost.Close();
                IAPHost = null;
            }
        }

        private string GetServiceVersion()
        {
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            return version;
        }

        private string GetOSName()
        {
            string osName = "";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");
            foreach (ManagementObject os in searcher.Get())
            {
                osName = os["Caption"].ToString();
                break;
            }

            //few more info
            osName += " (" + Environment.OSVersion.VersionString + ")";
            return osName;
        }

        private string GetDotnetVersion()
        {
            string dotnet = Environment.Version.ToString();
            return dotnet;
        }

        private string GetMachineName()
        {
            string machineName = Environment.MachineName;
            return machineName;
        }

        private string GetMachineDomain(string machineName)
        {
            string domain = Dns.GetHostEntry(machineName).HostName.ToLower().Replace(machineName.ToLower() + ".", "");
            return domain; 
        }

        private bool Is64BitOS()
        {
            return Environment.Is64BitOperatingSystem;
        }
    }
}

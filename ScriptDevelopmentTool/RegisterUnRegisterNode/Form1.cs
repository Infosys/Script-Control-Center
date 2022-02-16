using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Infosys.WEM.Client;
using Infosys.WEM.Node.Service.Contracts.Message;

namespace RegisterUnRegisterNode
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            RegisteredNodes nodeclient = new RegisteredNodes("http://localhost:58127/WEMNodeService.svc");
            RegisterReqMsg req = new RegisterReqMsg();
            req.Node = new Infosys.WEM.Node.Service.Contracts.Data.RegisteredNode();
            req.Node.DotNetVersion = "4.0";
            req.Node.ExecutionEngineSupported = 3;
            req.Node.HostMachineDomain = "DomainName";
            req.Node.HostMachineName = "localhost";
            req.Node.HttpPort = 8001;
            req.Node.Is64Bit = false;
            req.Node.OSVersion = "win 7";
            req.Node.State = Infosys.WEM.Node.Service.Contracts.Data.NodeState.Active;
            req.Node.TcpPort = 8002;
            req.Node.WorkflowServiceVersion = "1.1";

           RegisterResMsg res =  nodeclient.ServiceChannel.Register(req);
        }

        private void btnUnRegister_Click(object sender, EventArgs e)
        {
            RegisteredNodes nodeclient = new RegisteredNodes("http://localhost:58127/WEMNodeService.svc");
            UnRegisterReqMsg req = new UnRegisterReqMsg();
            req.Domain = "DomainName";
            req.MachineName = "Localhost";

            UnRegisterResMsg res = nodeclient.ServiceChannel.UnRegister(req);
        }

        private void btnGetSysInfo_Click(object sender, EventArgs e)
        {
            string os = GetOSName();
            string dotnetver = GetDotnetVersion();
            string srvver = GetServiceVersion();
            string machine = GetMachineName();
            string domain = GetMachineDomain(machine);
            bool is64bitos = Is64BitOS();
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
            var obj = Dns.GetHostEntry(machineName);
            string domain = Dns.GetHostEntry(machineName).HostName.ToLower().Replace(machineName.ToLower() + ".", "");
            return domain; 
        }

        private bool Is64BitOS()
        {
            return Environment.Is64BitOperatingSystem;
        }
    }
}

/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infosys.WEM.Client;
using Infosys.WEM.Node.Service.Contracts.Message;
using System.ServiceModel;
using System.Net;
using System.Net.Sockets;
using System.Configuration;
using System.ServiceModel.Description;
using System.Reflection;
using System.Management;
namespace Infosys.WEM.Utilities.ScriptExecuteUtil
{
    public class RegisterScript
    {
        ServiceHost IAPHost = null;
        public void RegisterNode()
        {
            RegisteredNodes nodeclient = new RegisteredNodes();
            RegisterReqMsg req = new RegisterReqMsg();
            req.Node = new Infosys.WEM.Node.Service.Contracts.Data.RegisteredNode();
            req.Node.DotNetVersion = GetDotnetVersion();
            req.Node.ExecutionEngineSupported = int.Parse(ConfigurationManager.AppSettings["ExecutionEngineSupported"]);
            req.Node.HostMachineName = "SC_" + GetMachineName();
            req.Node.HostMachineDomain = GetMachineDomain();
            req.Node.HttpPort = int.Parse(ConfigurationManager.AppSettings["httpPort"]); //9001;
            req.Node.Is64Bit = Is64BitOS();
            req.Node.OSVersion = GetOSName();
            req.Node.State = Infosys.WEM.Node.Service.Contracts.Data.NodeState.Active;
            req.Node.TcpPort = int.Parse(ConfigurationManager.AppSettings["tcpPort"]);//9002;
            req.Node.WorkflowServiceVersion = GetServiceVersion();
            string strCompId = ConfigurationManager.AppSettings["Company"];
            int iCompId;
            if (!string.IsNullOrEmpty(strCompId) && int.TryParse(strCompId, out iCompId))
            {
                req.Node.CompanyId = iCompId;
                RegisterResMsg res = nodeclient.ServiceChannel.Register(req);
            }
            else
                throw new Exception("Configuration error: Either Company Id is not provided or it is invalid");

        }
        public void UnRegisterNode()
        {
            RegisteredNodes nodeclient = new RegisteredNodes();
            UnRegisterReqMsg req = new UnRegisterReqMsg();
            req.MachineName = "SC_" + GetMachineName();
            req.Domain = GetMachineDomain();

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

        private string GetMachineDomain()
        {
            string machineName = Environment.MachineName;
            string domain = Dns.GetHostEntry(machineName).HostName.ToLower().Replace(machineName.ToLower() + ".", "");
            return domain; 
        }

        private bool Is64BitOS()
        {
            return Environment.Is64BitOperatingSystem;
        }
    }
}


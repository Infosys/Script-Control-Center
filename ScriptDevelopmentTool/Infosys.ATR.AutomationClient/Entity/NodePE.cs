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

namespace Infosys.ATR.AutomationClient.Entity
{
    public class NodePE
    {
        public string DotNetVersion { get; set; }
        public int ExecutionEngineSupported { get; set; }
        public string HostMachineDomain { get; set; }
        public string HostMachineName { get; set; }
        public int? HttpPort { get; set; }
        public bool Is64Bit { get; set; }
        public string OSVersion { get; set; }
        public int? TcpPort { get; set; }
        public string WorkflowServiceVersion { get; set; }
        public string HttpUrl
        {
            get
            {
                if (HttpPort != null && HttpPort != 0)
                    return "http://" + HostMachineName + "." + HostMachineDomain + ":" + HttpPort + "/iap/rest";
                else
                    return null;
            }
        }
        public string NettcpUrl
        {
            get
            {
                if (TcpPort != null && TcpPort != 0)
                    return "net.tcp://" + HostMachineName + "." + HostMachineDomain + ":" + TcpPort + "/iap";
                else
                    return null;
            }
        }
    }
}

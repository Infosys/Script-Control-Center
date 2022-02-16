/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.WEM.Node.Service.Contracts.Data
{
    [DataContract]
    public class RegisteredNode
    {
        [DataMember]
        public string HostMachineName { get; set; }
        [DataMember]
        public string HostMachineDomain { get; set; }
        [DataMember]
        public int? HttpPort { get; set; }
        [DataMember]
        public int? TcpPort { get; set; }
        [DataMember]
        public string OSVersion { get; set; }
        [DataMember]
        public bool Is64Bit { get; set; }
        [DataMember]
        public string WorkflowServiceVersion { get; set; }
        [DataMember]
        public string DotNetVersion { get; set; }
        [DataMember]
        public int ExecutionEngineSupported { get; set; }
        [DataMember]
        public NodeState State { get; set; }
        [DataMember]
        public int CompanyId { get; set; }
    }

    public enum NodeState
    {
        Active, InActive
    }

    [DataContract]
    public enum NodeType
    {
        [EnumMember]
        Workflow = 1,
        [EnumMember]
        Script = 2,
        [EnumMember]
        WorkflowAndScript = 3,
        [EnumMember]
        All = 4
    }
}

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

using SE = Infosys.WEM.Node.Service.Contracts.Data;
using DE = Infosys.WEM.Resource.Entity;
using Infosys.WEM.Node.Service.Contracts.Data;

namespace Infosys.WEM.Service.Implementation.Translators.RegisterredNodes
{
    public class RegisteredNodes_SE_DE
    {
        public static DE.RegisterredNodes RegisteredNodeSEtoDE(SE.RegisteredNode nodeSE)
        {
            DE.RegisterredNodes nodeDE = null;
            if (nodeSE != null)
            {
                nodeDE = new DE.RegisterredNodes();
                //nodeDE.PartitionKey = nodeSE.State.ToString();
                //nodeDE.RowKey =nodeSE.HostMachineDomain +"\\" + nodeSE.HostMachineName;
                nodeDE.PartitionKey = nodeSE.HostMachineDomain;
                nodeDE.RowKey = nodeSE.HostMachineName;
                nodeDE.DomainName = nodeSE.HostMachineDomain;
                nodeDE.MachineName = nodeSE.HostMachineName;
                nodeDE.HttpPort = nodeSE.HttpPort;
                nodeDE.TcpPort = nodeSE.TcpPort;
                nodeDE.State = nodeSE.State.ToString();
                nodeDE.DotNetVersion = nodeSE.DotNetVersion;
                nodeDE.ExecutionEngineSupported = nodeSE.ExecutionEngineSupported;
                nodeDE.Is64Bit = nodeSE.Is64Bit;
                nodeDE.WorkflowServiceVersion = nodeSE.WorkflowServiceVersion;
                nodeDE.OSVersion = nodeSE.OSVersion;
                nodeDE.CompanyId = nodeSE.CompanyId;
            }
            return nodeDE;
        }

        public static SE.RegisteredNode RegisterredNodeDEtoSE(DE.RegisterredNodes nodeDE)
        {
            SE.RegisteredNode nodeSE = null;
            if (nodeDE != null)
            {
                nodeSE = new SE.RegisteredNode();
                nodeSE.HostMachineDomain = nodeDE.DomainName;
                nodeSE.HostMachineName = nodeDE.MachineName;
                nodeSE.HttpPort = nodeDE.HttpPort;
                nodeSE.TcpPort = nodeDE.TcpPort;
                nodeSE.DotNetVersion = nodeDE.DotNetVersion;
                nodeSE.ExecutionEngineSupported = nodeDE.ExecutionEngineSupported;
                nodeSE.Is64Bit = nodeDE.Is64Bit;
                nodeSE.WorkflowServiceVersion = nodeDE.WorkflowServiceVersion;
                nodeSE.OSVersion = nodeDE.OSVersion;
                nodeSE.CompanyId = nodeDE.CompanyId.GetValueOrDefault();
                NodeState tempState;
                if(Enum.TryParse(nodeDE.State, out tempState))
                    nodeSE.State = tempState;
            }
            return nodeSE;
        }

        public static List<SE.RegisteredNode> RegisterredNodeDEListToSEList(List<DE.RegisterredNodes> nodeDEs)
        {
            List<SE.RegisteredNode> nodeSEs = null;
            if (nodeDEs != null && nodeDEs.Count > 0)
            {
                nodeSEs = new List<SE.RegisteredNode>();
                nodeDEs.ForEach(de => {
                    nodeSEs.Add(RegisterredNodeDEtoSE(de));
                });
            }
            return nodeSEs;
        }
    }
}

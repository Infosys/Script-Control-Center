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

namespace Infosys.ATR.AutomationClient.Translator
{
    public class Node_PE_SE
    {
        public static Entity.NodePE NodeSEtoPE(SE.RegisteredNode nodeSE)
        {
            Entity.NodePE nodePE = null;
            if (nodeSE != null)
            {
                nodePE = new Entity.NodePE();
                nodePE.DotNetVersion = nodeSE.DotNetVersion;
                nodePE.ExecutionEngineSupported = nodeSE.ExecutionEngineSupported;
                nodePE.HostMachineDomain = nodeSE.HostMachineDomain;
                nodePE.HostMachineName = nodeSE.HostMachineName;
                nodePE.HttpPort = nodeSE.HttpPort;
                nodePE.Is64Bit = nodeSE.Is64Bit;
                nodePE.OSVersion = nodeSE.OSVersion;
                nodePE.TcpPort = nodeSE.TcpPort;
                nodePE.WorkflowServiceVersion = nodeSE.WorkflowServiceVersion;
            }

            return nodePE;
        }

        public static List<Entity.NodePE> NodeListSEtoPE(List<SE.RegisteredNode> nodesSE)
        {
            List<Entity.NodePE> nodesPE = null;
            if (nodesSE != null && nodesSE.Count > 0)
            {
                nodesPE = new List<Entity.NodePE>();
                nodesSE.ForEach(se => {
                    nodesPE.Add(NodeSEtoPE(se));
                });
            }
            return nodesPE;
        }
    }
}

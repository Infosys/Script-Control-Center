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
    public class ActiveRegisteredNodes_SE_DE
    {
        public static DE.ActiveRegisteredNodes ActiveRegisteredNodesSEtoDe(SE.RegisteredNode nodeSE)
        {
            DE.ActiveRegisteredNodes anodeDE = null;
            if (nodeSE != null)
            {
                anodeDE = new DE.ActiveRegisteredNodes();
                anodeDE.PartitionKey = anodeDE.DomainName = nodeSE.HostMachineDomain;
                anodeDE.RowKey = anodeDE.MachineName = nodeSE.HostMachineName;
                anodeDE.CompanyId = nodeSE.CompanyId;
            }
            return anodeDE;
        }
    }
}

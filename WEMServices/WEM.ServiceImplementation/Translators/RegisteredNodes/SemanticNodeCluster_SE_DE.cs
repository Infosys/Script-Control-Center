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
using Infosys.WEM.Infrastructure.Common;
using SE = Infosys.WEM.Node.Service.Contracts.Data;
using DE = Infosys.WEM.Resource.Entity;
using Infosys.WEM.Node.Service.Contracts.Data;

namespace Infosys.WEM.Service.Implementation.Translators.RegisteredNodes
{
    public class SemanticNodeCluster_SE_DE
    {
        public static DE.SemanticNodeCluster SemanticNodeClusterSEtoDE(SE.SemanticNodeCluster cnodeSE)
        {
            DE.SemanticNodeCluster cnodeDE = null;
            if (cnodeSE != null)
            {
                cnodeDE = new DE.SemanticNodeCluster();
                cnodeDE.PartitionKey = cnodeDE.ClusterId = cnodeSE.ClusterId;
                cnodeDE.RowKey = cnodeSE.IapNodeId  + ";" + cnodeSE.Domain;
                cnodeDE.IapNodeId = cnodeSE.IapNodeId; 
                cnodeDE.CreatedBy = Utility.GetLoggedInUser();
                //cnodeDE.CreatedOn will be assigned in the data access
                cnodeDE.LastModifiedBy = Utility.GetLoggedInUser();
                cnodeDE.LastModifiedOn = DateTime.UtcNow;
                cnodeDE.IsDeleted = cnodeSE.IsDeleted;
            }
            return cnodeDE;
        }
    }
}

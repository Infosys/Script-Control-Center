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
    public class SemanticCluster_SE_DE
    {
        public static DE.SemanticCluster SemanticClusterSEtoDE(SE.SemanticCluster clusterSE)
        {
            DE.SemanticCluster clusterDE = null;
            if (clusterSE != null)
            {
                clusterDE = new DE.SemanticCluster();
                clusterDE.PartitionKey = clusterSE.CompanyId.ToString() ;
               
                //clusterDE.RowKey to be generated in the data access
                //clusterDE.Id to be assigned in the data access as the rowkey
                clusterDE.CompanyId = clusterSE.CompanyId;
                clusterDE.CreatedBy = Utility.GetLoggedInUser();                    
                clusterDE.ClusterName = String.IsNullOrEmpty(clusterSE.NewName) ? clusterSE.Name : clusterSE.NewName;
                clusterDE.Description = clusterSE.Description;
                clusterDE.LastModifiedBy = Utility.GetLoggedInUser();
                clusterDE.LastModifiedOn = DateTime.UtcNow;
                clusterDE.Priority = clusterSE.Priority;
                clusterDE.CompanyId = clusterSE.CompanyId;
                if (!string.IsNullOrEmpty(clusterSE.Id)) //i.e. not in case of insert/add new cluster operations
                {
                    clusterDE.Id = clusterDE.RowKey = clusterSE.Id;
                    clusterDE.IsDeleted = !clusterSE.IsDeleted;
                }
            }
            return clusterDE;
        }
    }
}

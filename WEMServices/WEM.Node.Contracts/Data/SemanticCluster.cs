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
using System.Runtime.Serialization;

namespace Infosys.WEM.Node.Service.Contracts.Data
{
    [DataContract]
    public class SemanticCluster
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string NewName { get; set; }
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public int Priority { get; set; }
        [DataMember]
        public int CompanyId { get; set; }
        [DataMember]
        public bool IsDeleted { get; set; }
        
    }

    [DataContract]
    public class SemanticNodeCluster
    {
        [DataMember]
        public string ClusterId { get; set; }
        [DataMember]
        public string IapNodeId { get; set; }
        [DataMember]
        public string Domain { get; set; }
        [DataMember]
        public bool IsDeleted { get; set; }
        
    }
}

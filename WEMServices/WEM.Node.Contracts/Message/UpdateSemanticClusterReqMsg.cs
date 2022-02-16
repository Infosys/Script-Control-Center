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

using Infosys.WEM.Node.Service.Contracts.Data;

namespace Infosys.WEM.Node.Service.Contracts.Message
{
    [DataContract]
    public class UpdateSemanticClusterReqMsg
    {
        [DataMember]
        public string ClusterId { get; set; }
        [DataMember]
        public string ClusterName { get; set; }
        [DataMember]
        public string ClusterNewName { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string LastModifiedBy { get; set; }
        [DataMember]
        public int CompanyId { get; set; }
        [DataMember]
        public int Priority { get; set; }
        [DataMember]
        public bool IsEnabled { get; set; }
    }
}

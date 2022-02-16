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

namespace Infosys.WEM.Service.Common.Contracts.Message
{
    [DataContract]
    public class GetAllClustersByCategoryResMsg
    {
        [DataMember]
        public List<SemanticNode> Nodes { get; set; }
        
    }

    [DataContract]
    public class SemanticNode
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]    
        public List<string> Nodes { get; set; }

    }
}

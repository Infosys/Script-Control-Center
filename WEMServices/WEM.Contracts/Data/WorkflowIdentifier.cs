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

namespace Infosys.WEM.Service.Contracts.Data
{
    [DataContract]
    public class WorkflowIdentifier
    {
        [DataMember]
        public int CategoryID { get; set; }
        [DataMember]
        public int SubCategoryID { get; set; }
        [DataMember]
        public Guid WorkflowID { get; set; }
        [DataMember]
        public int WorkflowVer { get; set; }
        [DataMember]
        public string RequestorID { get; set; }
        [DataMember]
        public string RequestorSourceIP { get; set; }

    }
}

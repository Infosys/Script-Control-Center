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

namespace Infosys.WEM.Service.Common.Contracts.Message
{
    [DataContract]
    public class GetReferenceDataReqMsg
    {
        [DataMember]
        public string ReferenceType { get; set; }
        [DataMember]
        public string ReferenceKey { get; set; }
        [DataMember]
        public string PartitionKey { get; set; }
    }
}

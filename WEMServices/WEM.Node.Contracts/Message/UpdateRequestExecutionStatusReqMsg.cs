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
    public class UpdateRequestExecutionStatusReqMsg
    {
        [DataMember]
        public RequestExecutionStatus ExecutionStatus { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string IAPNode { get; set; }
        [DataMember]
        public string OutputParameters { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; }
        [DataMember]
        public string AssignedTo { get; set; }
    }
}

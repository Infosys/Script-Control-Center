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
    public class ScheduledRequestActivity
    {
        [DataMember]
        public int CompanyId { get; set; }
        [DataMember]
        public string ParentScheduledRequestId  { get; set; }
        [DataMember]
        public string ScheduledRequestId { get; set; }
        [DataMember]
        //[Newtonsoft.Json.Converters.JsonConverter(typeof(StringEnumConverter))]
        public RequestExecutionStatus Status { get; set; }
        [DataMember]
        public string IterationSetRoot { get; set; }
    }

    [DataContract]
    public class ScheduledRequestDetails
    {
        [DataMember]
        public string ScheduledRequestId { get; set; }
        [DataMember]
        public List<ScheduledRequestActivity> Activities { get; set; }
        [DataMember]
        public List<string> ChildScheduledRequestIds { get; set; }
    }
}

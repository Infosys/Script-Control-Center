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
    public class ScheduledRequest
    {
        [DataMember]
        public string Id { get; set; } //guid
        [DataMember]
        public string AssignedTo { get; set; } //iapnode name or cluster id
        [DataMember]
        public string Executor { get; set; } //wud the one executing the request- iapnode name
        [DataMember]
        public int CategoryId { get; set; }
        [DataMember]
        public int RequestVersion { get; set; } //workflow/script version
        [DataMember]
        public string InputParameters { get; set; } //json data
        [DataMember]
        public string OutputParameters { get; set; } //json data
        [DataMember]
        public string Requestor { get; set; }
        [DataMember]
        public DateTime CreatedOn { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; }
        [DataMember]
        public DateTime? ModifiedOn { get; set; }
        [DataMember]
        public DateTime? ExecuteOn { get; set; }
        [DataMember]
        public string Message { get; set; } //status Message in case of success and failure
        [DataMember]
        public RequestExecutionStatus State { get; set; }
        [DataMember]
        public RequestTypeEnum RequestType { get; set; } //1- Workflow, 2 – Script
        [DataMember]
        public string RequestId { get; set; } //workflow or script id
        [DataMember]
        public int Priority { get; set; }
        [DataMember]
        public int Iterations { get; set; }
        [DataMember]
        public int CompanyId { get; set; }
        [DataMember]
        public string ParentId { get; set; } //guid
        [DataMember]
        public StopTypes StopType { get; set; }
        [DataMember]
        public string IterationSetRoot { get; set; }  //not to be used in the client
        [DataMember]
        public bool IsIterationSetRoot { get; set; }  //not to be used in the client
        [DataMember]
        public DateTime? EndDate { get; set; }
    }

    [DataContract]
    public enum RequestExecutionStatus
    {
        [EnumMember(Value="New")]
        New = 1,
        [EnumMember(Value = "Initiated")]
        Initiated = 2,
        [EnumMember]
        InProgress= 3,
        [EnumMember]
        Completed = 4,
        [EnumMember]
        Failed = 5,
        [EnumMember]
        ReSubmit = 6, //set the executor to null like new
        [EnumMember]
        UserActionPending = 7, // for the scenario where manual intervention needed
        [EnumMember]
        UserActionCompleted = 8
    }

    [DataContract]
    public enum RequestTypeEnum
    {
        [EnumMember]
        Workflow=1,
        [EnumMember]
        Script =2
    }

    [DataContract]
    public enum StopTypes
    {
        [EnumMember]
        Limited = 1,
        [EnumMember]
        Indefinite=2
    }
}

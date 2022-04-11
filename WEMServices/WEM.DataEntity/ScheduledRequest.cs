//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Infosys.WEM.Resource.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class ScheduledRequest
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public System.DateTime Timestamp { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string Id { get; set; }
        public string MachineName { get; set; }
        public int CategoryId { get; set; }
        public Nullable<int> RequestVersion { get; set; }
        public string InputParameters { get; set; }
        public string OutputParameters { get; set; }
        public Nullable<System.DateTime> ExecuteOn { get; set; }
        public string Message { get; set; }
        public Nullable<int> State { get; set; }
        public int RequestType { get; set; }
        public string RequestId { get; set; }
        public int Priority { get; set; }
        public string AssignedTo { get; set; }
        public string Executor { get; set; }
        public string ParentId { get; set; }
        public Nullable<int> StopType { get; set; }
        public string IterationSetRoot { get; set; }
        public Nullable<bool> IsIterationSetRoot { get; set; }
        public Nullable<int> CompanyId { get; set; }
    }
}
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
    
    public partial class TransactionInstance
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public System.DateTime Timestamp { get; set; }
        public int CategoryId { get; set; }
        public string WorkflowId { get; set; }
        public string ScriptId { get; set; }
        public string InstanceId { get; set; }
        public int CurrentState { get; set; }
        public string Executor { get; set; }
        public string MachineName { get; set; }
        public string InitiatedBy { get; set; }
        public string TriggeredBy { get; set; }
        public string CreatedBy { get; set; }
        public string LastModifiedBy { get; set; }
        public string IPAddress { get; set; }
        public string OSDetails { get; set; }
        public string ReferenceKey { get; set; }
        public string WorkflowVersion { get; set; }
        public Nullable<System.DateTime> LastModifiedOn { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string FileType { get; set; }
        public string ScriptVersion { get; set; }
        public string CategoryName { get; set; }
        public string WorkflowPersistedStateId { get; set; }
        public string ModuleName { get; set; }
        public string TransactionMetadata { get; set; }
        public string Description { get; set; }
        public string BookMark { get; set; }
    }
}
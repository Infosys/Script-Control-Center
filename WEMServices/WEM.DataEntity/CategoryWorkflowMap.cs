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
    
    public partial class CategoryWorkflowMap
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public System.DateTime Timestamp { get; set; }
        public int CategoryId { get; set; }
        public System.Guid WorkflowId { get; set; }
        public int ActiveWorkflowVersion { get; set; }
        public string WorkflowName { get; set; }
    }
}

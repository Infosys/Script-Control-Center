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
    
    public partial class SemanticCluster
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public System.DateTime Timestamp { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public string ClusterName { get; set; }
        public string Id { get; set; }
        public string Description { get; set; }
        public Nullable<int> Priority { get; set; }
        public Nullable<int> CompanyId { get; set; }
    }
}
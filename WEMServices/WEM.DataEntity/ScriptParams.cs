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
    
    public partial class ScriptParams
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public System.DateTime Timestamp { get; set; }
        public int ParamId { get; set; }
        public string Name { get; set; }
        public string DefaultValue { get; set; }
        public string AllowedValues { get; set; }
        public bool IsMandatory { get; set; }
        public string ParamType { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsSecret { get; set; }
        public bool IsUnnamed { get; set; }
        public string DataType { get; set; }
        public bool IsReferenceKey { get; set; }
    }
}

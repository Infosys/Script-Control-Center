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
    
    public partial class ObjectModelMaster
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public System.DateTime Timestamp { get; set; }
        public int CategoryId { get; set; }
        public System.Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ObjectModelVer { get; set; }
        public string ObjectModelUri { get; set; }
        public Nullable<System.DateTime> PublishedOn { get; set; }
        public string CreatedBy { get; set; }
        public string SourceIPAddress { get; set; }
        public string Client { get; set; }
        public string ClientVersion { get; set; }
        public string SourceMachineName { get; set; }
        public Nullable<int> FileSizeKB { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedOn { get; set; }
    }
}
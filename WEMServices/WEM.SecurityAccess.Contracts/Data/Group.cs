/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Runtime.Serialization;

namespace Infosys.WEM.SecurityAccess.Contracts.Data
{
    [DataContract]
    public class Group
    {
        [DataMember]
        public int GroupId { get; set; }
        [DataMember]
        public int CompanyID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public int Roles { get; set; }
        //[DataMember]
        //public string CreatedBy { get; set; }
        //[DataMember]
        //public string LastModifiedBy { get; set; }
        [DataMember]
        public bool IsActive { get; set; }
        [DataMember]
        public int ParentId { get; set; }
    }
}

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
    public class User
    {
        [DataMember]
        public int UserId { get; set; }
        [DataMember]
        public int CompanyId { get; set; }       
        [DataMember]        
        public string Alias { get; set; }
        [DataMember]
        public string DisplayName { get; set; }
        [DataMember]
        public int Role { get; set; }
        [DataMember]
        public int CategoryId { get; set; }
        //[DataMember]
        //public string CreatedBy { get; set; }
        //[DataMember]
        //public string LastModifiedBy { get; set; }
        [DataMember]
        public bool IsActive { get; set; }
        [DataMember]
        public bool IsActiveGroup { get; set; }
        [DataMember]
        public bool IsDL { get; set; }
        [DataMember]
        public int? GroupId { get; set; }

    }
}

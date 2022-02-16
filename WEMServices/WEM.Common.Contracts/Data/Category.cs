/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Infosys.WEM.Service.Common.Contracts.Data
{
    [DataContract]
    public class Category
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public int CategoryId { get; set; }
        [DataMember]
        public int ParentCategoryId { get; set; } //its value is zero in case of Category and non-zero in case of SubCategory       
        [DataMember]
        public bool IsDeleted { get; set; }
        [DataMember]
        public int? ParentId { get; set; }
        [DataMember]
        public int CompanyId { get; set; }
        [DataMember]
        public int ModuleID { get; set; }
        [DataMember]
        public string NewName { get; set; }

        [DataMember]
        public int NumberOfScripts { get; set; }
    }   
}

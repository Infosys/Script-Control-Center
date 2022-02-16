/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Infosys.WEM.Scripts.Service.Contracts.Data
{
    [DataContract]
    public class CategoryTree
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public int CategoryId { get; set; }        
        //[DataMember]
        //public string CreatedBy { get; set; }
        //[DataMember]
        //public string ModifiedBy { get; set; }
        [DataMember]
        public bool IsDeleted { get; set; }
        [DataMember]
        public int ParentId { get; set; }
        [DataMember]
        public List<Category> SubCategories { get; set; }
    }
}

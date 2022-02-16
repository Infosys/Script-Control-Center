/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Infosys.WEM.Service.Contracts.Data
{
    [DataContract]
    public class WorkflowCategoryMaster
    {
        [DataMember(IsRequired=true)]
        public int CategoryID { get; set; }
        [DataMember(IsRequired=true)]
        public int CompanyID { get; set; }
        [DataMember(Name="Category Name", IsRequired=true)]
        public string Name { get; set; }
        [DataMember]
        public bool IsActive { get; set; }
        [DataMember]
        public CategoryType Type { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public int ParentId { get; set; }

        public enum CategoryType
        {
            Public,
            Private
        };
    }
    
}

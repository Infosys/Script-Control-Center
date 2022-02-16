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
    public class WorkflowMaster
    {
        [DataMember]
        public Guid WorkflowID { get; set; }
        [DataMember]
        public int CategoryID { get; set; }
        //[DataMember]
        //public int SubCategoryID { get; set; }
        [DataMember(Name = "Workflow Name")]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public int WorkflowVersion { get; set; }
        [DataMember]
        public string WorkflowURI { get; set; }
        [DataMember]
        public string ImageURI { get; set; }        
        [DataMember]
        public string SrcIPAddr { get; set; }
        [DataMember]
        public string ClientId { get; set; }
        [DataMember]
        public string ClientVer { get; set; }
        [DataMember]
        public string SrcMachineName { get; set; }
        [DataMember]
        public int FileSize { get; set; }
        [DataMember]
        public bool IsActive { get; set; }
        [DataMember]
        public bool IncrementVersion { get; set; }
        [DataMember]
        public bool UsesUIAutomation { get; set; }

        [DataMember]
        public string LastModifiedBy { get; set; }

        //[DataMember]
        //public DateTime? PublishedOn { get; set; }

        [DataMember]
        public string CreatedBy { get; set; }

        [DataMember]
        public List<WorkflowParam> Parameters { get; set; }

        [DataMember]
        public DateTime? LastModifiedOn { get; set; }
        [DataMember]
        public DateTime? PublishedOn { get; set; }
        [DataMember]
        public bool IslongRunningWorkflow { get; set; }
        [DataMember]
        public int IdleStateTimeout { get; set; }
        [DataMember]
        public string Tags { get; set; }
        [DataMember]
        public string LicenseType { get; set; }
        [DataMember]
        public string SourceUrl { get; set; } 
    }
}

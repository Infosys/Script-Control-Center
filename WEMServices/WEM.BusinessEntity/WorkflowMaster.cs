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

namespace Infosys.WEM.Business.Entity
{
    public class WorkflowMaster
    {
        public Guid WorkflowID { get; set; }
        public int CategoryID { get; set; }
        //public int SubCategoryID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int WorkflowVersion { get; set; }
        public string WorkflowURI { get; set; }
        public string ImageURI { get; set; }
        public DateTime PublishedOn { get; set; }
        public string CreatedBy { get; set; }
        public string SrcIPAddr { get; set; }
        public string ClientId { get; set; }
        public string ClientVer { get; set; }
        public string SrcMachineName { get; set; }
        public int FileSize { get; set; }
        public bool IsActive { get; set; }
        public bool IncrementVersion { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public bool UsesUIAutomation { get; set; }
        public bool IslongRunningWorkflow { get; set; }        
        public int IdleStateTimeout { get; set; }
        public string Tags { get; set; }      
        public string LicenseType { get; set; }      
        public string SourceUrl { get; set; } 
    }
}

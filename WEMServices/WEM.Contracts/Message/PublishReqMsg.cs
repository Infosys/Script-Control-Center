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
using System.IO;
using Infosys.WEM.Service.Contracts.Data;

namespace Infosys.WEM.Service.Contracts.Message
{
    [DataContract]
    public class PublishReqMsg
    {
        [DataMember(IsRequired=true)]
        public Guid WorkflowID { get; set; }

        [DataMember (Name = "WorkflowCategoryID", IsRequired=true)]
        public int CategoryID { get; set; }

        [DataMember]
        public int SubCategoryID { get; set; }

        [DataMember(IsRequired = true)]
        public int WorkflowVer { get; set; }

        [DataMember(Name = "WorkflowName", IsRequired = true)]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember(IsRequired = true)]
        public string WorkflowURI { get; set; }
        [DataMember(IsRequired = true)]
        public string ImageURI { get; set; }
        [DataMember]
        public string ClientId { get; set; }
        [DataMember]
        public string ClientVer { get; set; }
        [DataMember]
        public string SrcMachineName { get; set; }
        [DataMember]
        public string SrcIPAddr { get; set; }
        [DataMember(IsRequired = true)]
        public int FileSizeInKb { get; set; }
        [DataMember]
        public bool IncrementVersion { get; set; }
        [DataMember]
        public List<WorkflowParam> Parameters { get; set; }
        [DataMember]
        public bool UsesUIAutomation { get; set; }
        [DataMember]
        public byte[] WFContent { get; set; }
        [DataMember]
        public string CompanyId { get; set; }
        [DataMember]
        public string StorageBaseURL { get; set; }
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public bool Modified { get; set; }
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

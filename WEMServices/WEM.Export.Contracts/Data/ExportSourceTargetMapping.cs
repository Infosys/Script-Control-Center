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
namespace Infosys.WEM.Export.Service.Contracts.Data
{
    [DataContract]
    public class ExportSourceTargetMapping
    {
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public string SourceInstanceAddr { get; set; }
        [DataMember]
        public int SourceScriptCategoryId { get; set; }
        [DataMember]
        public int SourceScriptId { get; set; }
        [DataMember]
        public int SourceScriptVersion { get; set; }
        [DataMember]
        public int TargetInstanceId { get; set; }
        [DataMember]
        public int TargetScriptCategoryId { get; set; }
        [DataMember]
        public int TargetScriptId { get; set; }
        [DataMember]
        public int TargetScriptVersion { get; set; }
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public System.DateTime CreatedOn { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; }
        [DataMember]
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        [DataMember]
        public bool IsActive { get; set; }
    }
}

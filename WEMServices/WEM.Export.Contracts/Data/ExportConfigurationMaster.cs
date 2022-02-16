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
    public class ExportConfigurationMaster
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public int TargetServerId { get; set; }

        [DataMember]
        public string TargetSystemUserId { get; set; }

        [DataMember]
        public string TargetSystemPassword { get; set; }

        [DataMember]
        public int ExportStatus { get; set; }

        [DataMember]
        public Nullable<System.DateTime> CompletedOn { get; set; }

        [DataMember]
        public string CreatedBy { get; set; }

        [DataMember]
        public System.DateTime CreatedOn { get; set; }

        [DataMember]
        public string ModifiedBy { get; set; }

        [DataMember]
        public Nullable<System.DateTime> ModifiedOn { get; set; }

        [DataMember]
        public Nullable<bool> IsDeleted { get; set; }

        [DataMember]
        public string ScriptRepositoryBaseServerAddress { get; set; }
    }
}

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
    public class ExportConfigurationMasterDetails
    {      
        [DataMember]
        public int AutomationServerTypeId { get; set; }
        [DataMember]
        public string AutomationServerIPAddress { get; set; }
        [DataMember]
        public string CasServerIPAddress { get; set; }
        [DataMember]
        public string TargetSystemUserName { get; set; }
        [DataMember]
        public string TargetSystemPassword { get; set; }

        [DataMember]
        public List<ExportConfigurationScriptDetails> ExportConfigurationDetails { get; set; }

        //[DataMember]
        //public string CreatedBy { get; set; }       
    }
}

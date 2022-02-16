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
    public class ExportJobProcessingHistory
    {
        [DataMember]
        public int JobId { get; set; }
        [DataMember]
        public int ExportConfigurationId { get; set; }
        [DataMember]
        public System.DateTime StartedOn { get; set; }
        [DataMember]
        public Nullable<System.DateTime> CompletedOn { get; set; }
        [DataMember]
        public string ProcessingSystemIp { get; set; }
        [DataMember]
        public string ProcessingSystemName { get; set; }
        [DataMember]
        public string ProcessedBy { get; set; }
    }
}

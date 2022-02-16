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
using System.Threading.Tasks;

namespace Infosys.WEM.Export.Service.Contracts.Data
{
    [DataContract]
   public class ExportServerDetails
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string DNSServer { get; set; }

        [DataMember]
        public string CasServer { get; set; }

        [DataMember]
        public int TargetSystemId { get; set; }

        [DataMember]
        public string CreatedBy { get; set; }

        [DataMember]
        public string ModifiedBy { get; set; }

        [DataMember]
        public DateTime CreatedOn { get; set; }

        [DataMember]
        public Nullable<System.DateTime> ModifiedOn { get; set; }

    }
}

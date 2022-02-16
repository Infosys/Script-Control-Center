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
    public class ExportTargetSystemDetails
    {
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int APIType { get; set; }
        [DataMember]
        public int Protocol { get; set; }
        [DataMember]
        public Boolean DefaultType { get; set; }

    }
}

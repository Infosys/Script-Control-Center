/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.WEM.Export.Service.Contracts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.WEM.Export.Service.Contracts.Message
{
    [DataContract]
    public class AddExportServerDetailsResMsg
    {
        [DataMember]
        public bool IsSuccess { get; set; }

        [DataMember]
        public int Id { get; set; }
    }
}

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
using Infosys.WEM.Nia.Service.Contracts.Data;

namespace Infosys.WEM.Nia.Service.Contracts.Message
{
    [DataContract]
    public class GetScriptByCategoryReqMsg
    {
        [DataMember]
        public string Data { get; set; }
        [DataMember]
        public Dictionary<string, string> RequestHeaderMap { get; set; }
        [DataMember]
        public LoginDetails Login { get; set; }
    }
}

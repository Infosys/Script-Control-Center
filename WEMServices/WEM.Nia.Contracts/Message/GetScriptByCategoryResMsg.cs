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
    public class GetScriptByCategoryResMsg
    {
        [DataMember]
        public List<NIAScript> NIAScripts { get; set; }
        [DataMember]
        public DateTime GetInstanceStartTime { get; set; }
        [DataMember]
        public DateTime GetInstanceEndTime { get; set; }
        [DataMember]
        public DateTime GetTicketStartTime { get; set; }
        [DataMember]
        public DateTime GetTicketEndTime { get; set; }
        [DataMember]
        public DateTime GetResponseOfPOSTStartTime { get; set; }
        [DataMember]
        public DateTime GetResponseOfPOSTEndTime { get; set; }
    }
}

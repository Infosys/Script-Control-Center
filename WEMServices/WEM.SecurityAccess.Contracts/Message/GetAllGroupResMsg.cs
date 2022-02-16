/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System.Collections.Generic;
using System.Runtime.Serialization;
using Infosys.WEM.SecurityAccess.Contracts.Data;

namespace Infosys.WEM.SecurityAccess.Contracts.Message
{
    [DataContract]
    public class GetAllGroupResMsg
    {
        [DataMember]
        public List<Group> Groups { get; set; }
    }
}

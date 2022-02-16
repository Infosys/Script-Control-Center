/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Infosys.WEM.Infrastructure.Common;

namespace Infosys.WEM.Scripts.Service.Contracts.Message
{
    [DataContract]
    public class UpdateScriptResMsg
    {
        [DataMember]
        public bool IsSuccess { get; set; }

        [DataMember]
        public List<ServiceFaultError> ServiceFaults;
    }
}

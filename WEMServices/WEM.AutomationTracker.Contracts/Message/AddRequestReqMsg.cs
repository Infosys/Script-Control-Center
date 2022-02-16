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
using Infosys.WEM.AutomationTracker.Contracts.Data;

namespace Infosys.WEM.AutomationTracker.Contracts.Message
{
    [DataContract]
    public class AddRequestReqMsg
    {
        [DataMember]
        public ScriptExecuteRequest requestData { get; set; }
    }
}

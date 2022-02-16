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
using WEM.ScriptExecution.Data;

namespace WEM.ScriptExecution.Message
{
    [DataContract]
    public class InitiateExecutionResMsg
    {
        [DataMember]
        public List<ScriptResponse> ScriptResponse { get; set; }
    }
    
}

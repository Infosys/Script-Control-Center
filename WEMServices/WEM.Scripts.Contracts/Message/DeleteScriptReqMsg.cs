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

namespace Infosys.WEM.Scripts.Service.Contracts.Message
{
    //creating smaller object from delete operation as the existing Script class has many fields 
    //which are not needef for delete operation
    [DataContract]
    public class DeleteScriptReqMsg
    {
        [DataMember]
        public int ScriptId { get; set; }
        [DataMember]
        public int CategoryId { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; }
    }
}

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

namespace Infosys.WEM.Nia.Service.Contracts.Data
{
    [DataContract]
    public class NIAScriptParamVOList
    {
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public int niaScriptId { get; set; }
        [DataMember]
        public string paramName { get; set; }
        [DataMember]
        public string defaultValue { get; set; }
        [DataMember]
        public Boolean isMandatory { get; set; }
    }
}

﻿/****************************************************************
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
    [DataContract]
    public class DeleteScriptResMsg
    {
        [DataMember]
        public bool IsSuccess { get; set; }
    }
}

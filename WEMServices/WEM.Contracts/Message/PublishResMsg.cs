﻿/****************************************************************
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
using Infosys.WEM.Service.Contracts.Data;
using Infosys.WEM.Infrastructure.Common;
namespace Infosys.WEM.Service.Contracts.Message
{
    [DataContract]
    public class PublishResMsg
    {
        [DataMember]
        public WorkflowMaster WorkflowDetails;
        
        [DataMember]
        public List<ServiceFaultError> ServiceFaults;

    }
}
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
using Infosys.WEM.SecurityAccess.Contracts.Data;
using Infosys.WEM.Infrastructure.Common.Validators;

namespace Infosys.WEM.SecurityAccess.Contracts.Message
{
    [DataContract]
    public class UpdateUserReqMsg
    {
        [DataMember]
        [ADValidator(MessageTemplate = "User not found in AD")]
        public User User { get; set; }
    }
}

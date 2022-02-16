/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System.Runtime.Serialization;
using Infosys.WEM.SecurityAccess.Contracts.Data;

using Infosys.WEM.Infrastructure.Common.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WCF;

namespace Infosys.WEM.SecurityAccess.Contracts.Message
{
    [DataContract]
    public class AddUserReqMsg
    {
        [DataMember]
        [ADValidator(MessageTemplate = "User not found in AD")]
        public User User { get; set; }
    }
}

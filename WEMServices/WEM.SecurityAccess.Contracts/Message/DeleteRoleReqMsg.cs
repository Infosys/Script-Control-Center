/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System.Runtime.Serialization;

namespace Infosys.WEM.SecurityAccess.Contracts.Message
{
    [DataContract]
    public class DeleteRoleReqMsg
    {
        [DataMember]
        public int RoleId { get; set; }

        [DataMember]
        public int CompanyId { get; set; }
    }
}

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
using Infosys.WEM.Service.Common.Contracts.Data;

namespace Infosys.WEM.Service.Common.Contracts.Message
{
    [DataContract]
    public class UpdateCategoryReqMsg
    {
        [DataMember]
        public List<Category> Categories { get; set; } //to hold one or more category or sub category
    }
}

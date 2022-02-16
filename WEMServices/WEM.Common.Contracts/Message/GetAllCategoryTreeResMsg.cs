/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System.Collections.Generic;
using System.Runtime.Serialization;

using Infosys.WEM.Service.Common.Contracts.Data;

namespace Infosys.WEM.Service.Common.Contracts.Message
{
  public class GetAllCategoryTreeResMsg
    {
        [DataMember]
        public List<CategoryTree> Categories { get; set; }
    }
}

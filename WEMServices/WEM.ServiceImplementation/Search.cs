/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using Infosys.WEM.Infrastructure.SecurityCore;
using Infosys.WEM.Business.Component;
using Infosys.WEM.Scripts.Service.Contracts;
using Infosys.WEM.Scripts.Service.Contracts.Data;
using Infosys.WEM.Scripts.Service.Contracts.Message;
using Infosys.WEM.Search.Contracts;
using Infosys.WEM.Infrastructure.Common;
using Infosys.WEM.Resource.DataAccess;

namespace Infosys.WEM.Service.Implementation
{
    public class Search_Base : ISearch
    {
        public virtual GetScriptDetailsResMsg SearchMeta(string data)
        {
            return null;
        }
    }

    public class Search : Search_Base
    {
        public override GetScriptDetailsResMsg SearchMeta(string data)
        {
            return base.SearchMeta(data);
        }
    }
}

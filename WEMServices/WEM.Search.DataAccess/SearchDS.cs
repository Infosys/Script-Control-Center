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

namespace Infosys.WEM.Search.Resource.DataAccess
{
    public class SearchRepository
    {
        public SearchRepository()
        {
        }

        public ISearchRepository Get()
        {
            //get repository info from config
            string config = "";
            switch (config)
            {
                case "Elastic":
                    return new ElasticSearch();                    
            }
            return null;
        }
    }
}

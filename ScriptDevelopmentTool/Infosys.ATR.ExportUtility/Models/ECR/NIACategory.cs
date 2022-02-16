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

namespace Infosys.ATR.ExportUtility.Models
{
    public class NIACategory
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<NIACategory> Children { get; set; }

        public int ParentId { get; set; }

       // public string TargetCategoryId {get;set;}
    }

    //public class CategoryTree
    //{
    //    public List<ECRCategory> rootCategories { get; set; }
    //}

    //public class LoginDetails
    //{
    //    public string CasServerAddr { get; set; }
    //    public string ECRServerAddr { get; set; }
    //    public string UserName { get; set; }
    //    public string Password { get; set; }

    //}
}

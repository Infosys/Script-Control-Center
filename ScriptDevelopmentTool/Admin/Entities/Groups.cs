/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infosys.ATR.Admin.Entities
{
    public class Groups
    {
        public string Name { get; set; }
        public string NewName { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public int CompanyId { get; set; }
        public int ParentId { get; set; }
        public int Roles { get; set; }
        public Action Action { get; set; }
        public List<int> Children { get; set; }
        public List<int> Parents { get; set; }
        public string ModuleID { get; set; }
        public bool AddCategory { get; set; }
    }


    public enum Action
    {
        Add,
        View,
        Delete
    }
}

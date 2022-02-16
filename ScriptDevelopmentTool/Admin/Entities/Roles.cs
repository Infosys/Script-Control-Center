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
    public class UserRoles
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }        
    }


    [Flags]
    public enum Roles
    {
        Adminstrator = 1,
        Manager,
        Analyst,
        Guest
    }
}

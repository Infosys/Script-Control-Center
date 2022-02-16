/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Infosys.ATR.Entities
{
    public class Users
    {
        public Image Delete { get; set; }
        public string Alias { get; set; }
        public string DisplayName { get; set; }        
        public int Id { get; set; }
        public string Role { get; set; }
        public int GroupId { get; set; }
        public int CategoryId { get; set; }
    }


    [Flags]
    public enum Roles
    {
        Adminstrator = 1,
        Manager,
        Analyst,
        Guest,
        Agent                  
    }

    public class CommonObjects
    {
        public static List<Users> Users;
        public static bool IsSuperAdmin;
    }
}

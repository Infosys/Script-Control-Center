using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Infosys.ATR.ModuleLoader.Entities
{
    public class Users
    {
        public Image Delete { get; set; }
        public string Alias { get; set; }
        public string DisplayName { get; set; }        
        public int Id { get; set; }
        public string Role { get; set; }
        public int GroupId { get; set; }
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

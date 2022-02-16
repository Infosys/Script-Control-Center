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

namespace Infosys.WEM.Business.Entity
{
    public class User
    {

        public int UserId { get; set; }

        public int CompanyId { get; set; }

        public string Alias { get; set; }

        public string DisplayName { get; set; }

        public int Role { get; set; }

        public int CategoryId { get; set; }

        public string CreatedBy { get; set; }

        public string LastModifiedBy { get; set; }

        public bool IsActive { get; set; }

        public bool IsDL { get; set; }

        public int? GroupId { get; set; }
    }
}

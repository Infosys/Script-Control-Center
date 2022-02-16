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
    public class ExportServerDetails
    {
        public int id { get; set; }
        public string DNSServer { get; set; }
        public string CasServer { get; set; }
        public int TargetSystemId { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }

        //public string ServerInstance { get; set; }
        public Nullable<System.DateTime> ModifiedOn
        {
            get; set;
        }
    }
}

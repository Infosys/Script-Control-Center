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
    public class CategoryWorkflowMap
    {
        public Guid WorkflowID { get; set; }

        public int CategoryID { get; set; }

        public string WorkflowName { get; set; }

        public int ActiveWorkflowVersion { get; set; }
    }
}

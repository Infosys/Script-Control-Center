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

namespace Infosys.ATR.WorkflowStateManagement.Entity
{
    public class Result
    {
        public Dictionary<string, object> Output { get; set; }
        public Guid PersistedWorkflowInstanceId { get; set; }
        public string BookMarkOfPausingActivity { get; set; }
    }
}

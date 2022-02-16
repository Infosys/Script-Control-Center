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

namespace Infosys.ATR.Admin.Entities
{
    public class SemanticGroup
    {
        public string Name { get; set; }
        public string NewName { get; set; }
        public string Id { get; set; }
        public string Description { get; set; }        
        public int Priority { get; set; }
        public string CreatedBy { get; set; }
        public Action Action { get; set; }
        public List<Nodes> ActiveNodes { get; set; }
        public bool IsActive { get; set; }
    }


    

    public class Nodes
    {
        public string Name { get; set; }
        public string Domain { get; set; }
        public string CompanyId { get; set; }       
        public string DotNetVersion { get; set; }       
        public int ExecutionEngineSupported { get; set; }
        public int? HttpPort { get; set; }       
        public bool Is64Bit { get; set; }       
        public string OSVersion { get; set; }       
        public string State { get; set; }       
        public int? TcpPort { get; set; }       
        public string WorkflowServiceVersion { get; set; }
    }
}

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

namespace Infosys.WEM.ScriptExecutionLibrary.Helper.Model
{
    public class ScriptExecutionVO
    {
        public int scriptId { get; set; }
        public String userId { get; set; }
        public Dictionary<String, String> scriptParams { get; set; }
        public List<NodeVO> nodeList { get; set; }
        public String arguments { get; set; }
        public int scriptExecTimeout { get; set; }
        public List<String> serviceAreas { get; set; }
    }

    public class NodeVO
    {
        public int id { get; set; }
        public String name { get; set; }
        public String os { get; set; }
        public String password { get; set; }
        public String stagePath { get; set; }
        public String userId { get; set; }
        public String workingDir { get; set; }
        public List<String> serviceAreas { get; set; }
    }
}

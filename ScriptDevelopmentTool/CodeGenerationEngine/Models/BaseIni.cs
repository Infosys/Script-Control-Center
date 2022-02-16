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

namespace CodeGenerationEngine.Model
{
    public class BaseIni
    {
        public List<ConfigInfo> Ini { get; set; }
    }

    public class ConfigInfo   
    {
        public string Attribute { get; set; }
        public List<ParamInfo> Parameters { get; set; }
    }

    public class ParamInfo
    {
        public string Comment { get; set; } 
        public string Name { get; set; }
        public string Value { get; set; }
    }
}

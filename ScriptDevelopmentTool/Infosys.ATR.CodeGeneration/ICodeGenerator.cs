/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infosys.ATR.CodeGeneration
{
    public interface ICodeGenerator
    {
        string Generate(string namespaceName, string className, List<PropertyDef> properties);
    }

    public class PropertyDef
    {
        public string PropertyName { get; set; }
        public Type PropertyType { get; set; }
        public bool IsCollection { get; set; }
        public string Comments { get; set; }
    }
}

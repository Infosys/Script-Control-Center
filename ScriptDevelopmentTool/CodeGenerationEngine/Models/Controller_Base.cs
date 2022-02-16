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
    public class Controller_Base
    {
        public string ClsName { get; set; }
        public Constructor ClsConstructor { get; set; }
        public IList<PyBaseAssembly> BaseAssemblies { get; set; }
        public IList<PyAddReference> AddReferences { get; set; }
        public IList<PyVariables> DeclareVariables { get; set; }
        public IList<PyMethod> Methods { get; set; }
    }

    public class PyMethod
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IList<PyVariables> Variables { get; set; }
        public IList<PyStatements> Statements { get; set; } 
    }

    public class PyStatements
    {
        public string Statement { get; set; }
    }

    public class Constructor
    {
        public IList<PyVariables> Initialized { get; set; }
    }

    public class PyVariables
    {
        public string Name { get; set; }
        public string value { get; set; }
    }

    public class PyBaseAssembly
    {
        public string Path { get; set; }
    }

    public class PyAddReference
    {
        public string Assembly { get; set; }
        public IList<ImpNamespace> Imports { get; set; }
    }

    public class ImpNamespace
    {
        public string ImportNamespace { get; set; }
        public string FromNamespace { get; set; }
    }
}

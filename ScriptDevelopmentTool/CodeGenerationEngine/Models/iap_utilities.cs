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
    public class Iap_Utilities  
    {
        public string AppIniPath { get; set; }
        public string AddReference { get; set; }

        public Iap_Utilities()
        {
            AddReference = GetReferences();
            AppIniPath = string.Format("r\"{0}\\App.ini\"",CodeGenerator.outputPath);
        }
        
        private string GetReferences() 
        {
            StringBuilder _references = new StringBuilder();
            foreach (var reference in CodeGenerator.AddReferences["Logger"])
            {
                if (reference.Assembly.Equals("mscorlib.dll", StringComparison.InvariantCultureIgnoreCase))
                    _references.Append(String.Format("\n\nclr.AddReference(\"{0}\")", reference.Assembly));
                else
                    _references.Append(String.Format("\n\nclr.AddReferenceToFileAndPath(Path.Combine(rootDirectory, \"{0}\"))", reference.Assembly));
                
                foreach (var import in reference.Imports)
                {
                    if (import.ImportNamespace != null)
                        _references.Append(String.Format("\n\nimport {0}", import.ImportNamespace));

                    _references.Append(String.Format("\nfrom {0} import *", import.FromNamespace));
                }
            }
            return _references.ToString();
        }
    }
}

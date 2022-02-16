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
    public class Iap_Resolver 
    {
        public string Methods { get; set; }
        public string BaseClsName { get; set; }
        public string BaseCls_FullPath{ get; set; } 
        string clsName = string.Empty;

        public Iap_Resolver()
        {
            clsName = "Iap_Resolver";
            BaseClsName = "Iap_Resolver_Base";
            BaseCls_FullPath = "iap_resolver_base.Iap_Resolver_Base";
            Methods = GenerateMethods();
        }

        private string GenerateMethods()
        {
            StringBuilder _result = new StringBuilder();
            foreach (var activity in CodeGenerator.useCase.Activities)
            {
                string Name = activity.Name.Replace(" ", "").Replace("-", "_");
                _result.Append("\n\n    ##############################################################################");
                _result.Append(string.Format("\n    ### Method         :  {0}", Name));
                _result.Append(string.Format("\n    ### Description    :  {0}", "This function calculates sum of two numbers and returns the sum"));
                _result.Append("\n    ##############################################################################");
                _result.Append(string.Format("\n    def {0}(self, automationFacade):", Name));
                _result.Append(string.Format("\n    \tself.logger.logAudit(\"{0} started execution\")", Name));
                _result.Append(string.Format("\n    \t#Call base class method"));
                _result.Append(string.Format("\n    \tresult = super({0}, self).{1}(automationFacade)", clsName, Name));
                _result.Append(string.Format("\n    \t#Add any new steps : TODO"));
                _result.Append(string.Format("\n    \tlogData = self.logger.CreateAuditData(\"Result\", result)"));
                _result.Append(string.Format("\n    \tself.logger.logAudit(\"{0} completed execution\", logData)", Name));
                _result.Append(string.Format("\n    \treturn result"));
            }
            return _result.ToString();
        }
    }
}

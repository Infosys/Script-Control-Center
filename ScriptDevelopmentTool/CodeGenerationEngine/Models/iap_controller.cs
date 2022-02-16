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
    public class Iap_Controller : Controller_Base 
    {
        public string BaseClsName { get; set; }
        public string ClsName { get; set; }
        public string Variable { get; set; }
        public string Method { get; set; }

        public Iap_Controller()
        {
            ClsName = "Iap_Controller";
            BaseClsName = "Iap_Controller_Base";
            Method=GetMethods();
            Variable = "\n\tscriptId = None";
        }       

        private string GetMethods()
        {
            StringBuilder _result = new StringBuilder();
            _result.Append(string.Format("\n\n\t##############################################################################"));
            _result.Append(string.Format("\n\t### Method         :  {0} Method", "Start"));
            _result.Append(string.Format("\n\t##############################################################################"));
            _result.Append(string.Format("\n\tdef {0}(self):", "Start"));
            _result.Append(string.Format("\n\t\t{0} = {1}", "result", "\"success\""));
            _result.Append(string.Format("\n\t\t{0} = {1}", "outMap", "{}"));
            _result.Append(string.Format("\n\t\t{0}", "try:"));
            _result.Append(string.Format("\n\t\t{0}", "\tself.logger.logAudit(self.workflowName + \" execution started\")"));
            _result.Append(string.Format("\n\t\t{0}", "\t#self.logger.notify(\"workflow with ScriptID: \" + self.scriptId + \" started execution\", IAP_NotificationType.Information, \"workflow with ScriptID: \" + self.scriptId + \" started execution\")"));
            _result.Append(string.Format("\n\t\t{0}", "\t#Do steps"));

            foreach (var activity in CodeGenerator.useCase.Activities)
            {
                string Name = activity.Name.Replace(" ", "").Replace("-", "_");                
                _result.Append(string.Format("\n\t\t\tself.{0}()", Name));
            }            
            _result.Append(string.Format("\n\t\t{0}", "except:"));
            _result.Append(string.Format("\n\t\t{0}", "\terrorStr = traceback.format_exc()"));
            _result.Append(string.Format("\n\t\t{0}", "\tprint \"errorStr: \", errorStr"));
            _result.Append(string.Format("\n\t\t{0}", "\tself.logger.logError(errorStr)"));
            _result.Append(string.Format("\n\t\t{0}", "\tresult=\"FAILURE\""));
            _result.Append(string.Format("\n\t\t{0}", "finally:"));
            _result.Append(string.Format("\n\t\t{0}", "\toutMap[\"result\"]=result"));
            _result.Append(string.Format("\n\t\t{0}", "\tlogData = self.logger.CreateAuditData(\"Result\", outMap[\"result\"])"));
            _result.Append(string.Format("\n\t\t{0}", "\tself.logger.logAudit(self.workflowName + \" execution completed\", logData)"));
            _result.Append(string.Format("\n\t\t{0}", "\t#self.logger.notify( \"workflow with ScriptID: \" + self.scriptId + \" completed execution\", IAP_NotificationType.Information, \"workflow with ScriptID: \" + self.scriptId + \" completed execution\")"));
            _result.Append(string.Format("\n\t\t{0}", "return outMap"));

            _result.Append(string.Format("\n\n\ndef test():"));
            _result.Append(string.Format("\n\t{0}={1}", "inputMap","{}"));
            _result.Append(string.Format("\n\t{0}={1}", "inputMap", "dict([arg.split(\":\") for arg in sys.argv[1:]])"));
            _result.Append(string.Format("\n\t{0}=\"{1}\"", "inputMap[\"workflowName\"]", Guid.NewGuid().ToString()));
            _result.Append(string.Format("\n\t{0}={1}", "inputMap[\"ticketNumber\"]","\"Tickt12\""));
            _result.Append(string.Format("\n\tobj = {0}(inputMap)", ClsName));
            _result.Append(string.Format("\n\tobj.Start()"));
            _result.Append(string.Format("\n\tprint \"Execution completed\""));
            _result.Append(string.Format("\n\nif __name__ == \"__main__\":"));
            _result.Append(string.Format("\n\ttest()"));
            return _result.ToString();
        }      
    }
}

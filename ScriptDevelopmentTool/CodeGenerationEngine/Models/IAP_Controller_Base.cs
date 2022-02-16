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
    public class Iap_Controller_Base : Controller_Base
    {
        //public string Assembly { get; set; }
        public string AddReference { get; set; }
        public string Variable { get; set; }
        public string InitVariable { get; set; }
        public string Method { get; set; }

        public Iap_Controller_Base()
        {
            ClsName = "Iap_Controller_Base";           
            AddReference = GetReferences();
            Variable = GetVariable();
            InitVariable = GetInitVar();
            Method = GetMethods();
        }        

        private string GetAssembly()
        {
            StringBuilder _assembly = new StringBuilder();
            foreach (var assembly in CodeGenerator.BaseAssemblies)
                _assembly.Append(String.Format("\nsys.path.append(r\"{0}\")", assembly.Path));
            return _assembly.ToString();
        }
        private string GetReferences()
        {
            StringBuilder _references = new StringBuilder();
            foreach (var reference in CodeGenerator.AddReferences["Controller"])
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
        private string GetMethods()
        {
            StringBuilder _result = new StringBuilder();            
            foreach (var activity in CodeGenerator.useCase.Activities)
            {
                string Name = activity.Name.Replace(" ", "").Replace("-", "_");
                _result.Append(string.Format("\n    \t##############################################################################"));
                _result.Append(string.Format("\n    \t### Method         :  {0}", Name));
                _result.Append(string.Format("\n    \t##############################################################################"));
                _result.Append(string.Format("\n    \tdef {0}(self):", Name));
                _result.Append(string.Format("\n    \t\tself.logger.logAudit(\"{0} started\")", Name));
                _result.Append(string.Format("\n    \t\t{0} = {1}", "logData", "None"));
                _result.Append(string.Format("\n    \t\t#TO DO"));
                _result.Append(string.Format("\n    \t\tobj = iap_resolver.Iap_Resolver(self.logger,self.inputDict)"));
                _result.Append(string.Format("\n    \t\tobj.{0}(self.automationFacade)", Name));
                _result.Append(string.Format("\n    \t\t#TO DO logData = CreateAuditData(\"result\", result)"));
                _result.Append(string.Format("\n    \t\tself.logger.logAudit(\"{0} completed\", logData)", Name));
            }
            return _result.ToString();
        }
        private string GetInitVar()
        { 
            StringBuilder _result = new StringBuilder();
            _result.Append(string.Format("\n\t\tself.{0} = {1}", "scriptId", "inputMap[\"workflowName\"]"));
            _result.Append(string.Format("\n\t\tself.{0} = {1}", "workflowName", "inputMap[\"workflowName\"]"));
            _result.Append(string.Format("\n\t\tself.{0} = {1}", "virtualUserId", "iap_utilities.getLoggedInUser()"));
            _result.Append(string.Format("\n\t\tself.{0} = {1}", "logger", "iap_utilities.create_iap_logger(self.scriptId)"));
            _result.Append(string.Format("\n\t\tself.{0} = {1}", "automationFacade", @"AutomationFacade(iap_constants.ATR_PATH, iap_constants.LAUNCH_APP, iap_constants.SHOW_APP_STARTING_WAIT_BOX,
                                                  iap_constants.FIRST_APPLICATION_TO_START, iap_constants.HIGHLIGHT_CONTROL,
                                                  iap_constants.MULTIPLE_SCALE_TEMPLATE_MATCHING , iap_constants.WAIT_FOREVER)"));

            _result.Append(string.Format("\n\t\tself.{0} = {1}", "inputDict", "inputMap"));            
             return _result.ToString();      
        }
        private string GetVariable()
        { 
            StringBuilder _result = new StringBuilder();
            _result.Append(string.Format("\n    \t{0} = None", "scriptId"));
            _result.Append(string.Format("\n    \t{0} = None", "automationFacade"));
            _result.Append(string.Format("\n    \t{0} = None", "logger"));
            _result.Append(string.Format("\n    \t{0} = None", "workflowName"));
            _result.Append(string.Format("\n    \t{0} = None", "inputDict"));
            return _result.ToString();        
        }
    }
}

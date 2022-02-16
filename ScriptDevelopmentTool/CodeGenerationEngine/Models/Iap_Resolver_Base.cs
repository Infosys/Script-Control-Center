/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.ATR.UIAutomation.ATRMapper;
using Infosys.ATR.UIAutomation.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerationEngine.Model
{
    public class Iap_Resolver_Base  
    {
        public string AddReference { get; set; }
        public string Method { get; set; }

        Dictionary<string, string> dictBtnEvent = new Dictionary<string, string>();
        public Iap_Resolver_Base() 
        {
            InitBtnEvent();
            AddReference = GetReferences();
            Method = GetMethods();
        }
        
        private string GetMethods()
        {
            StringBuilder _result = new StringBuilder();
            StringBuilder _resultParam = new StringBuilder();
            Dictionary<string, List<string>> _dctCanonical = CodeGenerator.CanonicalPaths;
            foreach (var activity in CodeGenerator.useCase.Activities)
            {
                string Name = activity.Name.Replace(" ", "").Replace("-", "_");
                _result.Append(Environment.NewLine);

                _result.Append("\n\n    ##############################################################################");
                _result.Append(string.Format("\n    ### Method         :  {0}", Name));
                _result.Append(string.Format("\n    ### Description    :  {0}", ""));
                _result.Append("\n    ##############################################################################");
                _result.Append(string.Format("\n    def {0}(self, automationFacade):", Name));
                _result.Append("\n    \tresult = None");

                foreach (var task in activity.Tasks)
                {
                    if (task.Event.ToString().Equals("Wait", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var keyData = task.TargetControlAttributes.SingleOrDefault(a => a.Name.Equals("Interval")).Value.ToString();
                        _result.Append(string.Format("\n    \tautomationFacade.Sleep({0})", keyData));
                    }
                    else if (!task.Event.ToString().Equals("keyboardkeypress", StringComparison.InvariantCultureIgnoreCase))
                    {
                        //string _ControlName = System.Text.RegularExpressions.Regex.Replace(task.ControlName.Trim(), @"[^0-9a-zA-Z]+", "_");

                        string _ControlName = string.Empty;

                        if (!string.IsNullOrEmpty(task.ControlType) && !string.IsNullOrEmpty(task.ControlName))
                            _ControlName = System.Text.RegularExpressions.Regex.Replace(
                                string.Format("{0}{1}{2}", task.ControlType.Replace("ControlType.", ""), task.ControlName, task.Order), @"[^0-9a-zA-Z]+", "_");
                        else if (!string.IsNullOrEmpty(task.ControlType) && !string.IsNullOrEmpty(task.ControlId))
                            _ControlName = System.Text.RegularExpressions.Regex.Replace(
                                string.Format("{0}{1}{2}", task.ControlType.Replace("ControlType.", ""), task.ControlId, task.Order), @"[^0-9a-zA-Z]+", "_");
                        else if (!string.IsNullOrEmpty(task.ControlType))
                            _ControlName = System.Text.RegularExpressions.Regex.Replace(
                                string.Format("{0}{1}", task.ControlType.Replace("ControlType.", ""), task.Order), @"[^0-9a-zA-Z]+", "_");
                        else
                            _ControlName = System.Text.RegularExpressions.Regex.Replace(
                                string.Format("{0}{1}", "Control_", task.Order), @"[^0-9a-zA-Z]+", "_");
                        var key = _dctCanonical.FirstOrDefault(x => x.Value[2].Equals(_ControlName,StringComparison.InvariantCultureIgnoreCase)).Key;

                        string canonicalPath = "";
                        string ctrlClass = "";
                        string outParam = ""; 
                        if (_dctCanonical.ContainsKey(key))
                        {
                            canonicalPath = Convert.ToString(_dctCanonical[key][0]);
                            ctrlClass = Convert.ToString(_dctCanonical[key][3]);
                            outParam = Convert.ToString(_dctCanonical[key][1]);
                        }

                        if (!string.IsNullOrEmpty(canonicalPath))
                        {
                            _result.Append(string.Format("\n    \tautomationFacade.Sleep(1)"));
                            _result.Append(string.Format("\n    \tctrl{0}  = automationFacade.FindControl(iapModel.{1})", _ControlName, canonicalPath));
                            _result.Append(string.Format("\n    \tctrl{0}.{1}", _ControlName, GetEventName(task.Name)));
                        }

                        /* Keypressed event tracker*/
                        var keyTasks = activity.Tasks.Where(ent => !string.IsNullOrEmpty(ent.GroupScriptId)).ToList().FindAll(x => x.GroupScriptId.Equals(task.GroupScriptId) && !x.Id.Equals(task.Id)).OrderBy(a => a.Order);

                        if (keyTasks != null)
                        {
                            if (keyTasks.Count() > 0)
                            {
                                _result.Append(string.Format("\n    \tautomationFacade.Sleep(1)"));

                                if (ctrlClass.ToUpper().Equals("EDIT"))
                                {
                                    //_result.Append(string.Format("\n    \tctrl{0}.KeyPress(self.inputDict[iapModel.{1}])", _ControlName, canonicalPath));//commented for replacing keypress with Paste
                                    _result.Append(string.Format("\n    \tautomationFacade.Paste(self.inputDict[iapModel.{0}])", canonicalPath));

                                    var paramValue = "";

                                    foreach (var keyTask in keyTasks)
                                    {
                                        var keyData = keyTask.TargetControlAttributes.SingleOrDefault(a => a.Name.Equals("KeyData")).Value;

                                        if (keyTask.Event.ToString().ToLower().Equals("keyboardkeypress", StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            if (keyTask.Name.Equals("GroupedKeys", StringComparison.InvariantCultureIgnoreCase))
                                                paramValue += keyData.Replace("NumPad", "");
                                            else if (!keyTask.Name.Equals("CollectiveKey", StringComparison.InvariantCultureIgnoreCase))
                                            {
                                                _result.Append(string.Format("\n    \tctrl{0}.KeyPress(\"\",KeyModifier.{1})", _ControlName, GetKeyName(keyData)));
                                                _result.Append(string.Format("\n    \tautomationFacade.Sleep(1)"));
                                            }
                                        }
                                    }
                                    _resultParam.Append(string.Format("\n    \tpDict[iapModel.{0}]=\"{1}\"", canonicalPath, paramValue));

                                    if (!string.IsNullOrEmpty(outParam))
                                        CodeGenerator.ParamNames.Add(outParam);
                                }
                                else
                                {
                                    List<string> skipTaskIds = new List<string>();

                                    foreach (var keyTask in keyTasks)
                                    {
                                        if (!skipTaskIds.Exists(ent => ent.Contains(keyTask.Id)))
                                        {
                                            var keyData = keyTask.TargetControlAttributes.SingleOrDefault(a => a.Name.Equals("KeyData")).Value;

                                            if (keyTask.Event.ToString().ToLower().Equals("keyboardkeypress", StringComparison.InvariantCultureIgnoreCase)
                                                && keyTask.Name.Equals("CollectiveKey", StringComparison.InvariantCultureIgnoreCase))
                                            {
                                                var nextKeyTask = keyTasks
                                                 .OrderBy(item => item.Order)
                                                 .First(item => item.Order > keyTask.Order);

                                                if (nextKeyTask.Name.Equals("GroupedKeys", StringComparison.InvariantCultureIgnoreCase))
                                                {
                                                    var nextKeyData = nextKeyTask.TargetControlAttributes.SingleOrDefault(a => a.Name.Equals("KeyData")).Value;
                                                    _result.Append(string.Format("\n    \tctrl{0}.KeyPress(\"{1}\",KeyModifier.{2})", _ControlName, nextKeyData[0], GetKeyName(keyData)));

                                                    if (nextKeyData.Substring(1).Length > 0)
                                                        //_result.Append(string.Format("\n    \tctrl{0}.KeyPress(\"{1}\")", _ControlName, nextKeyData.Replace("NumPad", "").Substring(1).ToLower()));//commented for replacing keypress with Paste
                                                        _result.Append(string.Format("\n    \tautomationFacade.Paste(\"{0}\")", nextKeyData.Replace("NumPad", "").Substring(1).ToLower()));

                                                    skipTaskIds.Add(nextKeyTask.Id);
                                                }
                                                else
                                                    _result.Append(string.Format("\n    \tctrl{0}.KeyPress(\"\",KeyModifier.{1})", _ControlName, GetKeyName(keyData)));
                                            }
                                            else if (keyTask.Event.ToString().ToLower().Equals("keyboardkeypress") && keyTask.Name.Equals("GroupedKeys"))
                                                //_result.Append(string.Format("\n    \tctrl{0}.KeyPress(\"{1}\")", _ControlName, keyData.Replace("NumPad", "")));//commented for replacing keypress with Paste
                                                _result.Append(string.Format("\n    \tautomationFacade.Paste(\"{0}\")", keyData.Replace("NumPad", "")));
                                            else
                                            {
                                                _result.Append(string.Format("\n    \tctrl{0}.KeyPress(\"\",KeyModifier.{1})", _ControlName, GetKeyName(keyData)));
                                                _result.Append(string.Format("\n    \tautomationFacade.Sleep(1)"));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        /* Keypressed event tracker*/
                    }
                }
                _result.Append(string.Format("\n    \tautomationFacade.Sleep(1)"));
                _result.Append(string.Format("\n    \tresult = \"SUCCESS\""));
                _result.Append(string.Format("\n    \treturn result"));
                //_result.Append(string.Format("\n    \tresult = iap_utilities.readDataFromClipboard(automationFacade)"));
                //_result.Append(string.Format("\n    \treturn result"));
            }


            _result.Append("\n\n    ##############################################################################");
            _result.Append(string.Format("\n    ### Method         :  {0}", "Arguments"));
            _result.Append(string.Format("\n    ### Description    :  {0}", ""));
            _result.Append("\n    ##############################################################################");
            _result.Append(string.Format("\n    def {0}(self):", "GetArguments"));
            _result.Append("\n    \tpDict = {}");
            _result.Append(_resultParam.ToString());
            _result.Append(string.Format("\n    \treturn pDict"));

            CodeGenerator.strParameter = Convert.ToString(_resultParam).Replace("\t", "");

            //_result.Append("\n\n##############################################################################");
            //_result.Append(string.Format("\n### Method         :  {0}", "Arguments for external use"));
            //_result.Append(string.Format("\n### Description    :  {0}", ""));
            //_result.Append("\n##############################################################################");
            //_result.Append(string.Format("\ndef {0}():", "GetArguments"));
            //_result.Append("\n    pDict = {}");
            //_result.Append(Convert.ToString(_resultParam).Replace("\t",""));
            //_result.Append(string.Format("\n    return pDict"));

            return _result.ToString();
        }

        private string GetReferences()
        {
            StringBuilder _references = new StringBuilder();
            foreach (var reference in CodeGenerator.AddReferences["Resolver"])
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

        private string GetEventName(string key)
        {
            string result = "Click()";

            if (dictBtnEvent.ContainsKey(key))
                result = dictBtnEvent[key];

            return result;
        }

        private string GetKeyName(string key)
        {
            string result = key;
            Dictionary<string, string> dictBtnEvent = new Dictionary<string, string>();
            dictBtnEvent.Add("Return", "Enter");
            if (dictBtnEvent.ContainsKey(key))
                result = dictBtnEvent[key];

            return result;
        }

        private void InitBtnEvent()
        {  
            dictBtnEvent.Add("MouseLeftClick", "Click()");
            dictBtnEvent.Add("MouseDoubleClick", "DoubleClick()");
            dictBtnEvent.Add("MouseRightClick", "RightClick()");
        }
    }
}

/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Infosys.ATR.UIAutomation.Entities;
using Entity = Infosys.ATR.Editor.Entities;
using Infosys.ATR.CodeGeneration;

namespace Infosys.ATR.Editor.Services
{
    internal partial class Translate
    {
        internal static Dictionary<string,string> ToScript(Entity.Root _root)
        {
            Dictionary<String, String> _scripts = new Dictionary<string, string>();

            _root.Application.ForEach(a =>
            {
                var scriptApp = new Script();
                string classname = "";
                scriptApp.ClassName = classname = GetClassName(a.Properties.Name);
                scriptApp.ClassType = Entities.ControlType.Application.ToString();

                a.Screens.ForEach(s =>
                {
                    string screenName = "";
                    var scriptScreen = new Script();

                    if (!String.IsNullOrEmpty(s.Properties.Name))
                    {
                        scriptScreen.ClassType = Entities.ControlType.Screen.ToString().ToString();
                        scriptScreen.ClassName = screenName = GetClassName(s.Properties.Name);

                        ControlClass cc = new ControlClass();
                        cc.ControlClassName = s.Properties.Name;
                        cc.ControlName = GetClassName(s.Properties.Name);
                        scriptApp.Imports.Add(new Import { ImportClass = cc.ControlName });
                        scriptApp.CtrlClasses.Add(cc);

                        AddControlElements(s.Controls, scriptScreen);

                        var scriptscreen = CodeGenerator.Generate(scriptScreen);
                        _scripts.Add(screenName + ".py", scriptscreen);
                    }

                    else
                    {
                        AddControlElements(s.Controls, scriptApp);
                    }

                });
                var scriptapp = CodeGenerator.Generate(scriptApp);
                _scripts.Add(classname + ".py",scriptapp);
            });

            return _scripts;
        }

        private static void AddControlElements(List<Entity.Control> controls, Script script)
        {
            controls.ForEach(c =>
            {

                ControlElement ctrlElement = new ControlElement();
                ctrlElement.ControlName = c.Properties.ControlName;
                ctrlElement.ControlType = c.Properties.Type;
                ctrlElement.Name = c.Properties.Name;
                script.CtrlElements.Add(ctrlElement);
                AddControlElements(c.InnerControls, script);

            });

        }

        private static string GetClassName(string s)
        {
            if (!String.IsNullOrEmpty(s))
            {
                string rest = s.Substring(1);
                string first = s[0].ToString().ToUpper();
                return first + rest;
            }
            return String.Empty;
        }
    }

   
}

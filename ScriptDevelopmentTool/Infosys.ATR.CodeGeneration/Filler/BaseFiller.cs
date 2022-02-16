/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infosys.ATR.UIAutomation.Entities;
using Infosys.Solutions.CodeGeneration;
using Infosys.Solutions.CodeGeneration.Framework;

namespace Infosys.ATR.CodeGeneration.Filler
{

    public class BaseFiller : ContentProvider
    {
        string _classname;
        List<ControlElement> _elements;
        List<ControlClass> _ctrlClass;
        List<Import> _imports;
        string _classType;        

        public BaseFiller()
        { }

        public BaseFiller(Script script)
        {
            this._classname = script.ClassName.Replace(" ", "");
            this._classType = script.ClassType.Replace(" ", "");
            this._elements = script.CtrlElements;
            this._ctrlClass = script.CtrlClasses;
            this._imports = script.Imports;
        }

        [PlaceHolder("className")]
        public string ClassName { get { return _classname; } }

        [PlaceHolder("classType")]
        public string ClassType { get { return _classType; } }     

        [PlaceHolder("RepeatImport")]
        public string RepeatImport
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                _imports.ForEach(i =>
                {
                    GenerateImportMembers gm =
                        new GenerateImportMembers(i.ImportClass);
                    gm.ContentTemplate = ContentTemplate.RepeatingTemplate("RepeatImport");
                    sb.Append(gm.GenerateContent());
                });
                return sb.ToString();
            }
        }

        [PlaceHolder("Repeater")]
        public string Repeater
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                _elements.ForEach(e =>
                {
                    GenerateBaseMembers gm =
                        new GenerateBaseMembers(e.Name, e.ControlType, e.ControlName);
                    gm.ContentTemplate = ContentTemplate.RepeatingTemplate("RepeaterTemplate");
                    sb.Append(gm.GenerateContent());
                });
                return sb.ToString();
            }
        }

        [PlaceHolder("RepeatClass")]
        public string Repeatclass
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                _ctrlClass.ForEach(c =>
                {
                    GenerateClassName gm =
                        new GenerateClassName(c.ControlName, c.ControlClassName);
                    gm.ContentTemplate = ContentTemplate.RepeatingTemplate("RepeatClass");
                    sb.Append(gm.GenerateContent());
                });
                return sb.ToString();
            }
        }
    }

    public class GenerateBaseMembers : ContentProvider
    {
        string _controlName;
        string _controlType;
        string _name;

        public GenerateBaseMembers()
        { }

        public GenerateBaseMembers(string name, string controlType, string controlName)
        {
            this._controlName = controlName == null ? " " : controlName.Replace(" ", "");
            this._controlType = controlType;
            this._name = name.Replace(" ", "");
        }

        [PlaceHolder("controlName")]
        public string controlName { get { return _controlName; } }

        [PlaceHolder("controlType")]
        public string ControlType { get { return _controlType; } }

        [PlaceHolder("name")]
        public string Name { get { return _name; } }
    }

    public class GenerateClassName : ContentProvider
    {
        string _ctrlName;
        string _ctrlclassName;

        public GenerateClassName()
        { }

        public GenerateClassName(string ctrlname, string ctrlclassName)
        {
            this._ctrlName = ctrlname.Replace(" ", "");
            this._ctrlclassName = ctrlclassName.Replace(" ", "");
        }

        [PlaceHolder("ctrlName")]
        public string CtrlName { get { return _ctrlName; } }

        [PlaceHolder("ctrlClassName")]
        public string CtrlClassName { get { return _ctrlclassName; } }

    }

    public class GenerateImportMembers : ContentProvider
    {
        string _importClass;

        public GenerateImportMembers(){}

        public GenerateImportMembers(string importClass)
        {
            this._importClass = importClass.Replace(" ", "");
        }

        [PlaceHolder("fromimportClass")]
        public string ImportClass
        {
            get
            {
                if (!String.IsNullOrEmpty(_importClass))
                {
                    return "from " + _importClass + " ";
                }
                else
                    return "";

            }
        }

        [PlaceHolder("importClass")]
        public string ImportClass_1
        {
            get
            {
                if (!String.IsNullOrEmpty(_importClass))
                {
                    return "import " + _importClass + " ";
                }
                else
                    return "";

            }
        }
    }
}

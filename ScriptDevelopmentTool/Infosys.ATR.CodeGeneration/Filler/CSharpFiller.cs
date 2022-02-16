/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infosys.Solutions.CodeGeneration;
using Infosys.Solutions.CodeGeneration.Framework;

namespace Infosys.ATR.CodeGeneration.Filler
{
    public class CSharpFiller : ContentProvider
    {        
        string _nameSpace;
        string _class;
        List<PropertyDef> _properties;

        public CSharpFiller(string nameSpace, string className, List<PropertyDef> properties)
        {   
            _nameSpace = nameSpace;
            _class = className;
            _properties = properties;
        }


        [PlaceHolder("Namespace")]
        public string Namespace
        {
            get
            {
                return _nameSpace;
            }

        }

        [PlaceHolder("ClassName")]
        public string ClassName
        {
            get
            {
                return _class;
            }

        }

        [PlaceHolder("Repeater")]
        public string Repeater
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                _properties.ForEach(p => {
                    GenerateMembers gm =
                        new GenerateMembers();
                    gm._comments = p.Comments;
                    gm._propertyName = p.PropertyName;
                    if (p.PropertyType.Name.Contains("List"))
                    {
                        var t = p.PropertyType.GetGenericArguments()[0].FullName;
                        gm._propertyType = "List<" + t + ">";
                    }
                    else
                        gm._propertyType = p.PropertyType.Name;

                    gm.ContentTemplate = ContentTemplate.RepeatingTemplate("RepeaterTemplate");
                    sb.Append(gm.GenerateContent());
                });

                return sb.ToString();
            }
        }
    }

    public class GenerateMembers : ContentProvider
    {
        internal static Template _repeatingTemplate;
        internal string _propertyName { get; set; }
        internal string _propertyType { get; set; }
        internal string _comments {get;set;}

        public GenerateMembers()
        {

        }

        public GenerateMembers(string pname,string ptype,string pcomments)
        {
            _propertyName = pname;
            _propertyType = ptype;
            _comments = pcomments;
            ContentTemplate = _repeatingTemplate;
        }

        
        [PlaceHolder("propertytype")]
        public string PropertyType
        {
            get
            {               
                return _propertyType;
            }
        }

        [PlaceHolder("propertyname")]
        public string PropertyName
        {
            get
            {
                return _propertyName;
            }
        }

        [PlaceHolder("comments")]
        public string Comments
        {
            get
            {
                return _comments;
            }
        }
    }   
}

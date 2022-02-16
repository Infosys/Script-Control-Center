/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.CodeDom;
using System.Reflection;
using System.CodeDom.Compiler;
using System.IO;

using Infosys.ATR.CodeGeneration.Filler;
using Infosys.Solutions.CodeGeneration.Framework;

namespace Infosys.ATR.CodeGeneration
{
    public class CSharpCodeGenerator //: ICodeGenerator
    {
        #region ICodeGenerator Members

        //public string Generate(string namespaceName, string className, List<PropertyDef> properties)
        //{
        //    StringBuilder generatedCode = new StringBuilder();
        //    CodeCompileUnit compileUnit = new CodeCompileUnit();           

            //define the namespace
            //CodeNamespace finalNamespace = new CodeNamespace(namespaceName);
            //finalNamespace.Imports.Add(new CodeNamespaceImport("System"));

        //    //define the class
        //    CodeTypeDeclaration finalClass = new CodeTypeDeclaration(className);
        //    finalClass.IsClass = true;
        //    finalClass.TypeAttributes = TypeAttributes.Public;

        //    //add class properties
        //    foreach (PropertyDef propertyDef in properties)
        //    {
        //        CodeMemberField member = new CodeMemberField();
        //        member.Attributes = MemberAttributes.Public| MemberAttributes.Final;
        //        member.Name = propertyDef.PropertyName + " { get; set; }";
        //        if (propertyDef.IsCollection)
        //        {
        //            member.Type = new CodeTypeReference("List<" + propertyDef.PropertyType + ">");
        //            finalNamespace.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
        //        }
        //        else
        //            member.Type = new CodeTypeReference(propertyDef.PropertyType);
        //        member.Comments.Add(new CodeCommentStatement(propertyDef.Comments));
        //        finalClass.Members.Add(member);          
        //    }            
            
        //    //add the class to the name space
        //    finalNamespace.Types.Add(finalClass);

        //    //add the name space to the compile unit
        //    compileUnit.Namespaces.Add(finalNamespace);

        //    //generate source code
        //    CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
        //    StringWriter wr = new StringWriter(generatedCode);
        //    provider.GenerateCodeFromCompileUnit(compileUnit, wr, new CodeGeneratorOptions());

        //    //because of the approach followed to add empty get/set, an extra ';' is added after each property, get rid of this extra ';'
        //    return generatedCode.ToString().Replace("};", "}");
        //}

        public static string Generate(string namespaceName, string className, List<PropertyDef> properties)
        {
            CSharpFiller cSharp = new CSharpFiller(namespaceName, className, properties);
            Template template = Template.FromFile(@"Templates\CSharpTemplate.txt");
            cSharp.ContentTemplate = template;
            return cSharp.GenerateContent();
        }

     
        #endregion
    }
}

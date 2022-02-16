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

namespace Infosys.ATR.ExportUtility.Models.ECR
{

    public class ECRCategory
    {
        public int parentCategoryId { get; set; }
        public string categoryName { get; set; }
        public string description { get; set; }
    }

    public class Category
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Category> Children { get; set; }
    }

    public class CategoryTree
    {
        public List<Category> rootCategories { get; set; }
    }

    public class NIAScript
    {
        public int scriptId { get; set; }
        public string scriptName { get; set; }
        public string description { get; set; }
        public string scriptType { get; set; }
        public List<NIAScriptParamVOList> niaScriptParamList { get; set; }
    }

    public class NIAScriptParamVOList
    {
        public int id { get; set; }
        public int niaScriptId { get; set; }
        public string paramName { get; set; }
        public string defaultValue { get; set; }
        public Boolean isMandatory { get; set; }
    }

    public class LoginDetails
    {
        public string CasServerAddr { get; set; }
        public string ECRServerAddr { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

    }

    public class PathVariablesVO
    {
        public Dictionary<string, string> PathVariableMap { get; set; }
        public List<String> ServiceAreas { get; set; }
    }
}

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

namespace Infosys.ATR.ExportUtility.Models
{
    public class categoryVO
    {
        public int categoryId { get; set; }
        public string categoryName { get; set; }
        public string description { get; set; }
        public List<string> scriptVOList { get; set; }
        public string parentCategoryId { get; set; }

    }
    public class ScriptVO
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string scriptType { get; set; }
        public string argString { get; set; }
        public string scriptBlob { get; set; }
        public string namedParamFormat { get; set; }
        public List<categoryVO> categoryVOList { get; set; }
        public List<scriptParamsVO> scriptParamVOList { get; set; }
        public List<string> serviceAreas { get; set; }
    }

    //public class CategoryVO
    //{
    //    public int CategoryId { get; set; }
    //}

    public class scriptParamsVO
    {
        public int isMandatory { get; set; }
        public string allowedValues { get; set; }
        public string defParamValue { get; set; }
        public string paramName { get; set; }

    }
}

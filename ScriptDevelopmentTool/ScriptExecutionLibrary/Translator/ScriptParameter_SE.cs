/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SE = Infosys.WEM.Scripts.Service.Contracts.Data;
using OE = Infosys.IAP.CommonClientLibrary.Models;

namespace Infosys.WEM.ScriptExecutionLibrary.Translator
{
    public class ScriptParameter_SE
    {
        public static Parameter ScriptParameterFromSE(SE.ScriptParam paramSE)
        {
            Parameter param = null;
            if (paramSE != null)
            {
                param = new Parameter();
                param.ParameterName = paramSE.Name;
                param.ParameterValue = paramSE.DefaultValue;
                param.IsSecret = paramSE.IsSecret;
                param.IsPaired = !paramSE.IsUnnamed;
                param.DataType = paramSE.DataType;
                param.allowedValues = paramSE.AllowedValues;
            }
            return param;
        }


        public static Parameter ScriptParameterFromOE(OE.ContentParameter paramSE) 
        {
            Parameter param = null;
            if (paramSE != null)
            {
                param = new Parameter();
                param.ParameterName = paramSE.Name;
                param.ParameterValue = paramSE.DefaultValue;
                param.IsSecret = paramSE.IsSecret;
                param.IsPaired = !paramSE.IsUnnamed;
                param.DataType = paramSE.DataType;
                param.allowedValues = paramSE.AllowedValues;
            }
            return param;
        }
        public static List<Parameter> ScriptParameterListFromSE(List<SE.ScriptParam> paramSEList)
        {
            List<Parameter> paramlist = null;
            if (paramSEList != null && paramSEList.Count > 0)
            {
                paramlist = new List<Parameter>();
                paramSEList.ForEach(p => 
                {
                    paramlist.Add(ScriptParameterFromSE(p));
                });
            }
            return paramlist;
        }

        public static List<Parameter> ScriptParameterListFromOE(List<OE.ContentParameter> paramSEList)
        {
            List<Parameter> paramlist = null;
            if (paramSEList != null && paramSEList.Count > 0)
            {
                paramlist = new List<Parameter>();
                paramSEList.ForEach(p =>
                {
                    paramlist.Add(ScriptParameterFromOE(p));
                });
            }
            return paramlist;
        }
    }
}

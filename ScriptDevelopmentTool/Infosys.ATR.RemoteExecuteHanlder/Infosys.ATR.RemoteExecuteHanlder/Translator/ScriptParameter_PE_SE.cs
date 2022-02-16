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
using SE = Infosys.ATR.AutomationEngine.Contracts;

namespace Infosys.ATR.RemoteExecute.Translator
{
    public class ScriptParameter_PE_SE
    {
        public static SE.ScriptParameter ScriptParameterPEtoSE(Parameter paramPE)
        {
            SE.ScriptParameter paramSE = null;
            if (paramPE != null)
            {
                paramSE = new SE.ScriptParameter();
                paramSE.ParameterName = paramPE.ParameterName;
                paramSE.ParameterValue = paramPE.ParameterValue;
            }

            return paramSE;
        }

        public static List<SE.ScriptParameter> ScriptParameterListPEtoSE(List<Parameter> paramsPE)
        {
            List<SE.ScriptParameter> paramsSE = null;
            if (paramsPE != null && paramsPE.Count > 0)
            {
                paramsSE = new List<SE.ScriptParameter>();
                paramsPE.ForEach(pe => {
                    paramsSE.Add(ScriptParameterPEtoSE(pe));
                });
            }

            return paramsSE;
        }
    }
}

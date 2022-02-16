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

using IE = Infosys.WEM.WorkflowExecutionLibrary;
using Infosys.ATR.AutomationEngine.Contracts;

namespace Infosys.ATR.AutomationExecutionLib.Translator
{
    public class WF_OutputParameter_SE_IE
    {
        public static List<Parameter> OutputParameterListIEToSE(List<IE.Entity.Parameter> paramsIE)
        {
            List<Parameter> paramsSE = null;
            if (paramsIE != null && paramsIE.Count > 0)
            {
                paramsSE = new List<Parameter>();
                paramsIE.ForEach(ie => {
                    paramsSE.Add(OutputParameterIEToSE(ie));
                });
            }
            return paramsSE;
        }

        public static Parameter OutputParameterIEToSE(IE.Entity.Parameter paramIE)
        {
            Parameter paramSE = null;
            if (paramIE != null)
            {
                paramSE = new Parameter();
                paramSE.ParameterName = paramIE.ParameterName;
                paramSE.ParameterValue = paramIE.ParameterValue;
            }
            return paramSE;
        }
    }
}

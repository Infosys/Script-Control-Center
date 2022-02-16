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

using IE = Infosys.WEM.WorkflowExecutionLibrary.Entity;
using Infosys.ATR.AutomationEngine.Contracts;

namespace Infosys.ATR.AutomationExecutionLib.Translator
{
    public class WorkflowParameter_SE_IE
    {
        public static IE.Parameter WorkflowParameterSEtoIE(WorkFlowParameter paramSE)
        {
            IE.Parameter paramIE = null;
            if (paramSE != null)
            {
                paramIE = new IE.Parameter();
                paramIE.ParameterName = paramSE.ParameterName;
                paramIE.ParameterValue = paramSE.ParameterValue;
            }
            return paramIE;
        }

        public static List<IE.Parameter> WorkflowParameterListSEtoIE(List<WorkFlowParameter> paramsSE)
        {
            List<IE.Parameter> paramsIE = null;
            if (paramsSE != null && paramsSE.Count > 0)
            {
                paramsIE = new List<IE.Parameter>();
                paramsSE.ForEach(se =>
                {
                    paramsIE.Add(WorkflowParameterSEtoIE(se));
                });
            }

            return paramsIE;
        }
    }
}

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

namespace Infosys.ATR.AutomationClient.Translator
{
    public class WorkflowParameter_PE_SE
    {
        public static SE.WorkFlowParameter WorkflowParameterPEtoSE(Parameter paramPE)
        {
            SE.WorkFlowParameter paramSE = null;
            if (paramPE != null)
            {
                paramSE = new SE.WorkFlowParameter();
                paramSE.ParameterName = paramPE.ParameterName;
                paramSE.ParameterValue = paramPE.ParameterValue;
            }

            return paramSE;
        }

        public static List<SE.WorkFlowParameter> WorkflowParameterListPEtoSE(List<Parameter> paramsPE)
        {
            List<SE.WorkFlowParameter> paramsSE = null;
            if (paramsPE != null && paramsPE.Count > 0)
            {
                paramsSE = new List<SE.WorkFlowParameter>();
                paramsPE.ForEach(pe =>
                {
                    paramsSE.Add(WorkflowParameterPEtoSE(pe));
                });
            }

            return paramsSE;
        }
    }
}

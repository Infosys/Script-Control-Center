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
using IE = Infosys.ATR.RemoteExecute;

namespace Infosys.ATR.RemoteExecute.Translator
{
    public class Result_SE_IE
    {
        public static IE.Result ResultSEtoIE(SE.Result resultSe)
        {
            IE.Result resultIe = null;
            if (resultSe != null)
            {
                resultIe = new Result();
                resultIe.ErrorMessage = resultSe.ErrorMessage;
                resultIe.InputCommand = resultSe.InputCommand;
                resultIe.IsSuccess = resultSe.IsSuccess;
                resultIe.SuccessMessage = resultSe.SuccessMessage;
                resultIe.Output = ParameterListSEtoIE(resultSe.Output);
            }
            return resultIe;
        }

        public static IE.OutParameter ParameterSEtoIE(SE.Parameter paramSe)
        {
            IE.OutParameter paramIe = null;
            if (paramSe != null)
            {
                paramIe = new OutParameter();
                paramIe.ParameterName = paramSe.ParameterName;
                paramIe.ParameterValue = paramSe.ParameterValue;
            }
            return paramIe;
        }

        public static List<IE.OutParameter> ParameterListSEtoIE(List<SE.Parameter> paramsSe)
        {
            List<IE.OutParameter> paramsIe = null;
            if (paramsSe != null && paramsSe.Count > 0)
            {
                paramsIe = new List<OutParameter>();
                paramsSe.ForEach(se => {
                    paramsIe.Add(ParameterSEtoIE(se));
                });
            }
            return paramsIe;
        }
    }
}

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
using ServiceAdapter;

namespace Infosys.WEM.ScriptExecutionLibrary.Translator
{
    public class ExecutionResult_SE
    {
        public static List<ExecutionResult> ExecutionResultFromServiceResponse(List<ServiceModel.ServiceResponse> serviceResponses)
        {
            List<ExecutionResult> result = new List<ExecutionResult>();
            ExecutionResult executionResult = null;
            foreach (ServiceModel.ServiceResponse serviceResponse in serviceResponses)
            {                
                if (serviceResponse != null)
                {
                    executionResult = new ExecutionResult();
                    executionResult.TransactionId = new Guid(serviceResponse.TransactionId);
                    executionResult.Status = serviceResponse.CurrentState;
                    executionResult.SuccessMessage = serviceResponse.SuccessMessage;
                    executionResult.ErrorMessage = serviceResponse.ErrorMessage;
                    executionResult.IsSuccess = serviceResponse.IsSuccess;
                    executionResult.ComputerName = serviceResponse.ComputerName;
                    executionResult.InputCommand = serviceResponse.InputCommand;
                    executionResult.LogData = serviceResponse.LogData;
                    if (serviceResponse.OutParameters != null)
                    {
                        List<OutParameter> outParams = new List<OutParameter>();
                        foreach (ServiceModel.Parameters param in serviceResponse.OutParameters)
                        {
                            OutParameter objParam = new OutParameter();
                            objParam.ParameterName = param.ParameterName;
                            objParam.ParameterValue = param.ParameterValue;
                            outParams.Add(objParam);
                        }
                        executionResult.Output = outParams;
                    }

                }
                result.Add(executionResult);
            }
            return result;
        }
    }
}

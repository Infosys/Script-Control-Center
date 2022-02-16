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
using SE = Infosys.WEM.AutomationTracker.Contracts.Data;
using DE = Infosys.WEM.Resource.Entity;
using Newtonsoft.Json;

namespace Infosys.WEM.Service.Implementation.Translators.AutomationTracker
{
    public class ScriptExecuteResponseSE_DE
    {
        public static SE.ScriptExecuteResponse ScriptExecuteResponseDEtoSE(DE.ScriptExecuteResponse scriptExecuteResponseDE)
        {
            SE.ScriptExecuteResponse scriptExecuteResponseSE = null;
            if (scriptExecuteResponseDE != null)
            {
                scriptExecuteResponseSE = new SE.ScriptExecuteResponse();
                scriptExecuteResponseSE.TransactionId =new Guid(scriptExecuteResponseDE.transactionId.ToString());
                scriptExecuteResponseSE.CurrentState = scriptExecuteResponseDE.currentstate;
                scriptExecuteResponseSE.SuccessMessage = scriptExecuteResponseDE.successmessage;
                scriptExecuteResponseSE.ErrorMessage = scriptExecuteResponseDE.errormessage;
                //scriptExecuteResponseSE.IsSuccess =Convert.ToBoolean(scriptExecuteResponseDE.issuccess);
                scriptExecuteResponseSE.ComputerName = scriptExecuteResponseDE.computername.Trim();
                scriptExecuteResponseSE.InputCommand = scriptExecuteResponseDE.inputcommand;                
                scriptExecuteResponseSE.LogData = scriptExecuteResponseDE.LogData;
                if(scriptExecuteResponseDE.OutParameters!=null)
                    scriptExecuteResponseSE.OutParameters= JsonConvert.DeserializeObject<List<SE.Parameter>>(scriptExecuteResponseDE.OutParameters);
                scriptExecuteResponseSE.IsNotified = Convert.ToBoolean(scriptExecuteResponseDE.IsNotified);
                scriptExecuteResponseSE.NotificationRemarks = scriptExecuteResponseDE.NotificationRemarks;
                scriptExecuteResponseSE.SourceTransactionId = scriptExecuteResponseDE.SourceTransactionId;
            }

            return scriptExecuteResponseSE;
        }

        public static List<SE.ScriptExecuteResponse> ScriptExecuteResponseDEListtoSEList(List<DE.ScriptExecuteResponse> scriptExecuteResponseDEList)
        {
            List<SE.ScriptExecuteResponse> scriptExecuteResponseSEList = null;
            if (scriptExecuteResponseDEList != null)
            {
                scriptExecuteResponseSEList = new List<SE.ScriptExecuteResponse>();
                scriptExecuteResponseDEList.ForEach(se =>
                {
                    scriptExecuteResponseSEList.Add(ScriptExecuteResponseDEtoSE(se));
                });
            }
            return scriptExecuteResponseSEList;
        }

        public static List<DE.ScriptExecuteResponse> ScriptExecuteResponseSEListtoDEList(List<SE.ScriptExecuteResponse> scriptExecuteResponseSEList)
        {
            List<DE.ScriptExecuteResponse> scriptExecuteResponseDEList = null;
            if (scriptExecuteResponseSEList != null)
            {
                scriptExecuteResponseDEList = new List<DE.ScriptExecuteResponse>();
                scriptExecuteResponseSEList.ForEach(se =>
                {
                    scriptExecuteResponseDEList.Add(ScriptExecuteResponseSEtoDE(se));
                });
            }
            return scriptExecuteResponseDEList;
        }


        public static DE.ScriptExecuteResponse ScriptExecuteResponseSEtoDE(SE.ScriptExecuteResponse scriptExecuteResponseSE)
        {
            DE.ScriptExecuteResponse scriptExecuteResponseDE = null;
            if (scriptExecuteResponseSE != null)
            {
                scriptExecuteResponseDE = new DE.ScriptExecuteResponse();
                //scriptExecuteResponseDE.transactionId = scriptExecuteResponseSE.TransactionId.ToString();
                scriptExecuteResponseDE.transactionId = scriptExecuteResponseSE.TransactionId;
                //Added null Checks
                if (scriptExecuteResponseSE.CurrentState!=null)
                    scriptExecuteResponseDE.currentstate = scriptExecuteResponseSE.CurrentState.ToUpper();
                scriptExecuteResponseDE.successmessage = scriptExecuteResponseSE.SuccessMessage;
                scriptExecuteResponseDE.errormessage = scriptExecuteResponseSE.ErrorMessage;
                //scriptExecuteResponseDE.issuccess = Convert.ToBoolean(scriptExecuteResponseSE.IsSuccess);
                if (scriptExecuteResponseSE.ComputerName != null)
                    scriptExecuteResponseDE.computername = scriptExecuteResponseSE.ComputerName.Trim();
                scriptExecuteResponseDE.inputcommand = scriptExecuteResponseSE.InputCommand;
                scriptExecuteResponseDE.IsNotified = scriptExecuteResponseSE.IsNotified;
                scriptExecuteResponseDE.NotificationRemarks = scriptExecuteResponseSE.NotificationRemarks;
                scriptExecuteResponseDE.LogData = scriptExecuteResponseSE.LogData;
                if(scriptExecuteResponseSE.OutParameters!=null)
                    scriptExecuteResponseDE.OutParameters = JsonConvert.SerializeObject(scriptExecuteResponseSE.OutParameters);
                scriptExecuteResponseDE.SourceTransactionId = scriptExecuteResponseSE.SourceTransactionId;
                //scriptExecuteResponseDE.createdby = scriptExecuteResponseSE.CreatedBy;
                //scriptExecuteResponseDE.createddate = scriptExecuteResponseSE.CreatedOn;
                //scriptExecuteResponseDE.modifiedby = scriptExecuteResponseSE.ModifiedBy;
                //scriptExecuteResponseDE.modifieddate = scriptExecuteResponseSE.ModifiedOn ;
            }

            return scriptExecuteResponseDE;
        }

        public static List<SE.TransactionByStatusResponse> TransactionStatusResponsetoTransactionByStatusResponseList(List<Resource.DataAccess.TransactionStatusResponse> scriptExecuteResponseDEList)
        {
            List<SE.TransactionByStatusResponse> scriptExecuteResponseSEList = null;
            if (scriptExecuteResponseDEList != null)
            {
                scriptExecuteResponseSEList = new List<SE.TransactionByStatusResponse>();
                scriptExecuteResponseDEList.ForEach(se =>
                {
                    scriptExecuteResponseSEList.Add(new SE.TransactionByStatusResponse()
                    {
                        CompanyId=se.CompanyId,
                        TransactionId = se.TransactionId,
                        CurrentState = se.CurrentState,
                        ComputerName = se.ComputerName,
                        SourceTransactionId = se.SourceTransactionId,
                        NiaServiceAccount = se.NiaServiceAccount,
                        NiaServiceAccPassword = se.NiaServiceAccPassword,
                        casServerUrl = se.casServerUrl,
                        casServiceUrl = se.casServiceUrl,
                        niaEcrScriptExecuteUrl = se.niaEcrScriptExecuteUrl,
                        niaEcrFindByActivityIdUrl = se.niaEcrFindByActivityIdUrl,
                        niaEcrFindAllNodesUrl = se.niaEcrFindAllNodesUrl,
                        serviceAreas = se.serviceAreas,
                        findbyIdUrl = se.findbyIdUrl
                    }
                    );
                });
            }
            return scriptExecuteResponseSEList;
        }
    }
}

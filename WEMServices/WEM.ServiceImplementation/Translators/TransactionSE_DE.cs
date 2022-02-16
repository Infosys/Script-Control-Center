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

using SE = Infosys.WEM.Node.Service.Contracts.Data;
using DE = Infosys.WEM.Resource.Entity;
using Infosys.WEM.Node.Service.Contracts.Data;
using Infosys.WEM.Infrastructure.Common;

namespace Infosys.WEM.Service.Implementation.Translators
{
    public class TransactionSE_DE
    {
        public static SE.Transaction TransactionDEToSE(DE.TransactionInstance paramDE)
        {
            //Block 4
            DateTime processStartedTime4 = DateTime.Now;
            SE.Transaction paramSE = null;
            if (paramDE != null)
            {
                paramSE = new SE.Transaction();
                paramSE.CategoryId = paramDE.CategoryId;
                //paramSE.CategoryName = paramDE.CategoryName; // no longer from the transaction-instance table and will be updated in the service method after look up to the category table
                paramSE.CurrentState = (StateType)paramDE.CurrentState;
                paramSE.Executor = paramDE.Executor;
                //paramSE.InitiatedBy = paramDE.InitiatedBy;
                paramSE.InstanceId = paramDE.InstanceId;
                paramSE.IPAddress = paramDE.IPAddress;
                paramSE.MachineName = paramDE.MachineName;
                paramSE.OSDetails = paramDE.OSDetails;
                paramSE.ReferenceKey = paramDE.ReferenceKey;
                if (!string.IsNullOrEmpty(paramDE.WorkflowId))
                {
                    paramSE.ModuleId = paramDE.WorkflowId;
                    paramSE.Module = ModuleType.Workflow;
                    paramSE.ModuleVersion = paramDE.WorkflowVersion;
                }
                else if (!string.IsNullOrEmpty(paramDE.ScriptId))
                {
                    paramSE.ModuleId = paramDE.ScriptId;
                    paramSE.Module = ModuleType.Script;
                    paramSE.ModuleVersion = paramDE.ScriptVersion;
                }
                //paramSE.TriggeredBy = paramDE.TriggeredBy;
                
                paramSE.CreatedBy = paramDE.CreatedBy;
                paramSE.CreatedOn = paramDE.CreatedOn;
                paramSE.WorkflowPersistedStateId = paramDE.WorkflowPersistedStateId;
                paramSE.FileType = paramDE.FileType;
                paramSE.ModuleName = paramDE.ModuleName;
                paramSE.TransactionMetadata = paramDE.TransactionMetadata;
                paramSE.Description = paramDE.Description;
                paramSE.WorkflowActivityBookmark = paramDE.BookMark;
            }
            LogHandler.LogError(string.Format("Time taken by Transaction:Block 4 (TransactionDEToSE) : {0}", DateTime.Now.Subtract(processStartedTime4).TotalSeconds), LogHandler.Layer.Business, null);

            return paramSE;
        }

        public static List<SE.Transaction> TransactionListDEToSE(List<DE.TransactionInstance> paramDEs)
        {
            List<SE.Transaction> paramSEs = null;
            if (paramDEs != null && paramDEs.Count > 0)
            {
                paramSEs = new List<SE.Transaction>();
                paramDEs.ForEach(p =>
                {
                    paramSEs.Add(TransactionDEToSE(p));
                });
            }

            return paramSEs;
        }

        public static DE.TransactionInstance TransactionSEToDE(SE.Transaction paramSE)
        {
            //Block 2
            DateTime processStartedTime2 = DateTime.Now;
            DE.TransactionInstance paramDE = null;
            if (paramSE != null)
            {
                paramDE = new DE.TransactionInstance();
                paramDE.PartitionKey = paramSE.CategoryId + "_" + paramSE.ModuleId;
                if (paramSE.Module == ModuleType.Script)
                {
                    paramDE.ScriptId = paramSE.ModuleId;
                    paramDE.ScriptVersion = paramSE.ModuleVersion;
                }
                else if (paramSE.Module == ModuleType.Workflow)
                {
                    paramDE.WorkflowId = paramSE.ModuleId;
                    paramDE.WorkflowVersion = paramSE.ModuleVersion;
                }
                if (string.IsNullOrEmpty(paramSE.InstanceId))
                    paramDE.InstanceId = paramDE.RowKey = Guid.NewGuid().ToString();
                else
                    paramDE.InstanceId = paramDE.RowKey = paramSE.InstanceId;
                paramDE.CategoryId = paramSE.CategoryId;
                //paramDE.CategoryName = paramSE.CategoryName;//as no longer the category name resides in the transaction-instance table
                //paramDE.CreatedBy = paramSE.CreatedBy;
                paramDE.CurrentState = (int)paramSE.CurrentState;
                paramDE.Executor = paramSE.Executor;
                switch (paramSE.CurrentState)
                {
                    case StateType.InProgress:
                        paramDE.InitiatedBy = paramDE.CreatedBy = Infosys.WEM.Infrastructure.Common.Utility.GetLoggedInUser();
                        break;
                    case StateType.Resumed:
                        paramDE.TriggeredBy = Infosys.WEM.Infrastructure.Common.Utility.GetLoggedInUser();
                        break;
                }
                paramDE.IPAddress = paramSE.IPAddress;
                paramDE.LastModifiedBy = Infosys.WEM.Infrastructure.Common.Utility.GetLoggedInUser();
                paramDE.MachineName = paramSE.MachineName;
                paramDE.OSDetails = paramSE.OSDetails;
                paramDE.ReferenceKey = paramSE.ReferenceKey;
                //paramDE.TriggeredBy = paramSE.TriggeredBy;
                paramDE.FileType = paramSE.FileType;
                paramDE.WorkflowPersistedStateId = paramSE.WorkflowPersistedStateId;
                paramDE.TransactionMetadata = paramSE.TransactionMetadata;
                paramDE.Description = paramSE.Description;
                paramDE.BookMark = paramSE.WorkflowActivityBookmark;
            }
            LogHandler.LogError(string.Format("Time taken by Transaction:Block 2 (TransactionSEToDE) : {0}", DateTime.Now.Subtract(processStartedTime2).TotalSeconds), LogHandler.Layer.Business, null);

            return paramDE;
        }
    }
}

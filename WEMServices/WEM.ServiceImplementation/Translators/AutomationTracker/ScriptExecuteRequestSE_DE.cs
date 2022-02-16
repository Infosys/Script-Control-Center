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
using Infosys.WEM.Infrastructure.Common;
using SE = Infosys.WEM.AutomationTracker.Contracts.Data;
using DE = Infosys.WEM.Resource.Entity;
using Newtonsoft.Json;

namespace Infosys.WEM.Service.Implementation.Translators.AutomationTracker
{
    public class ScriptExecuteRequestSE_DE
    {

        public static SE.ScriptExecuteRequest ScriptExecuteRequestDEtoSE(DE.ScriptExecuteRequest scriptExecuteRequestDE)
        {
            SE.ScriptExecuteRequest scriptExecuteRequestSE = null;
            using (LogHandler.TraceOperations("ScriptExecuteRequestSE_DE:ScriptExecuteRequestDEtoSE", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
            {
                if (scriptExecuteRequestDE != null)
                {
                    scriptExecuteRequestSE = new SE.ScriptExecuteRequest();
                    //scriptExecuteRequestSE.TransactionId =new Guid(scriptExecuteRequestDE.transactionId);
                    scriptExecuteRequestSE.TransactionId = scriptExecuteRequestDE.transactionId.Value;
                    scriptExecuteRequestSE.ScriptId = Convert.ToInt32(scriptExecuteRequestDE.scriptid);
                    scriptExecuteRequestSE.CategoryId = Convert.ToInt32(scriptExecuteRequestDE.categoryid);
                    scriptExecuteRequestSE.ScriptName = scriptExecuteRequestDE.scriptname;
                    scriptExecuteRequestSE.InParameters = JsonConvert.DeserializeObject<List<SE.Parameter>>(scriptExecuteRequestDE.inparams);
                    scriptExecuteRequestSE.UserName = scriptExecuteRequestDE.username;
                    scriptExecuteRequestSE.RemoteServerNames = scriptExecuteRequestDE.remoteservernames;
                    scriptExecuteRequestSE.RemoteExecutionMode = Convert.ToInt32(scriptExecuteRequestDE.remoteexecutionhost);
                    scriptExecuteRequestSE.ExecutionMode = Convert.ToInt32(scriptExecuteRequestDE.executionmodetype);
                    scriptExecuteRequestSE.ScheduledPattern = scriptExecuteRequestDE.scheduledpattern;
                    if (scriptExecuteRequestDE.schedulestartdatetime != null)
                        scriptExecuteRequestSE.ScheduleStartDateTime = Convert.ToDateTime(scriptExecuteRequestDE.schedulestartdatetime);
                    if (scriptExecuteRequestDE.scheduleenddatetime != null)
                        scriptExecuteRequestSE.ScheduleEndDateTime = Convert.ToDateTime(scriptExecuteRequestDE.scheduleenddatetime);
                    scriptExecuteRequestSE.ScheduleOccurences = Convert.ToInt32(scriptExecuteRequestDE.scheduleoccurences);
                    scriptExecuteRequestSE.SchedulePriority = Convert.ToInt32(scriptExecuteRequestDE.schedulepriority);
                    scriptExecuteRequestSE.ScheduleOnClusters = scriptExecuteRequestDE.scheduleonclusters.Split(',').ToList();
                    scriptExecuteRequestSE.IapNodeTransport = scriptExecuteRequestDE.iapnodetransport;
                    scriptExecuteRequestSE.Domain = scriptExecuteRequestDE.domain;
                    scriptExecuteRequestSE.ReferenceKey = scriptExecuteRequestDE.referencekey;
                    scriptExecuteRequestSE.Path = scriptExecuteRequestDE.path;
                    scriptExecuteRequestSE.ResponseNotificationCallbackURL = scriptExecuteRequestDE.ResponseNotificationCallbackURL;
                    //scriptExecuteRequestSE.CreatedBy = scriptExecuteRequestDE.createdby;
                    //scriptExecuteRequestSE.CreatedOn = Convert.ToDateTime(scriptExecuteRequestDE.createddate);
                    //scriptExecuteRequestSE.ModifiedBy = scriptExecuteRequestDE.modifiedby;
                    //scriptExecuteRequestSE.ModifiedOn = Convert.ToDateTime(scriptExecuteRequestDE.modifieddate);
                }

                return scriptExecuteRequestSE;
            }
        }


        public static DE.ScriptExecuteRequest ScriptExecuteRequestSEtoDE(SE.ScriptExecuteRequest scriptExecuteRequestSE)
        {
            DE.ScriptExecuteRequest scriptExecuteRequestDE = null;
            using (LogHandler.TraceOperations("ScriptExecuteRequestSE_DE:ScriptExecuteRequestSEtoDE", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
            {
                if (scriptExecuteRequestSE != null)
                {
                    scriptExecuteRequestDE = new DE.ScriptExecuteRequest();
                    //scriptExecuteRequestDE.transactionId = scriptExecuteRequestSE.TransactionId == Guid.Empty ? Guid.NewGuid() : scriptExecuteRequestSE.TransactionId;
                    //scriptExecuteRequestDE.transactionId =Convert.ToString(scriptExecuteRequestSE.TransactionId == Guid.Empty ? Guid.NewGuid() : scriptExecuteRequestSE.TransactionId);
                    scriptExecuteRequestDE.transactionId = scriptExecuteRequestSE.TransactionId == Guid.Empty ? Guid.NewGuid() : scriptExecuteRequestSE.TransactionId;
                    scriptExecuteRequestDE.scriptid = Convert.ToInt32(scriptExecuteRequestSE.ScriptId);
                    scriptExecuteRequestDE.categoryid = Convert.ToInt32(scriptExecuteRequestSE.CategoryId);
                    scriptExecuteRequestDE.PartitionKey = scriptExecuteRequestSE.CompanyId.ToString("00000");
                    scriptExecuteRequestDE.scriptname = scriptExecuteRequestSE.ScriptName;
                    scriptExecuteRequestDE.inparams = JsonConvert.SerializeObject(scriptExecuteRequestSE.InParameters);
                    scriptExecuteRequestDE.username = scriptExecuteRequestSE.UserName;
                    scriptExecuteRequestDE.remoteservernames = scriptExecuteRequestSE.RemoteServerNames;
                    scriptExecuteRequestDE.remoteexecutionhost = Convert.ToString(scriptExecuteRequestSE.RemoteExecutionMode);
                    scriptExecuteRequestDE.executionmodetype = Convert.ToInt32(scriptExecuteRequestSE.ExecutionMode);
                    scriptExecuteRequestDE.scheduledpattern = scriptExecuteRequestSE.ScheduledPattern;
                    if (scriptExecuteRequestSE.ScheduleStartDateTime != null)
                        scriptExecuteRequestDE.schedulestartdatetime = Convert.ToDateTime(scriptExecuteRequestSE.ScheduleStartDateTime);
                    if (scriptExecuteRequestSE.ScheduleEndDateTime != null)
                        scriptExecuteRequestDE.scheduleenddatetime = Convert.ToDateTime(scriptExecuteRequestSE.ScheduleEndDateTime);
                    scriptExecuteRequestDE.scheduleoccurences = Convert.ToInt32(scriptExecuteRequestSE.ScheduleOccurences);
                    //scriptExecuteRequestDE.schedulepriority = Convert.ToInt32(scriptExecuteRequestSE.SchedulePriority);
                    scriptExecuteRequestDE.scheduleonclusters = Convert.ToString(scriptExecuteRequestSE.ScheduleOnClusters);
                    scriptExecuteRequestDE.iapnodetransport = scriptExecuteRequestSE.IapNodeTransport;
                    scriptExecuteRequestDE.domain = scriptExecuteRequestSE.Domain;
                    scriptExecuteRequestDE.referencekey = scriptExecuteRequestSE.ReferenceKey;
                    scriptExecuteRequestDE.path = scriptExecuteRequestSE.Path;
                    scriptExecuteRequestDE.ResponseNotificationCallbackURL = scriptExecuteRequestSE.ResponseNotificationCallbackURL;
                    //scriptExecuteRequestDE.createdby = Utility.GetLoggedInUser(); ;
                    //scriptExecuteRequestDE.createddate = Convert.ToDateTime(scriptExecuteRequestSE.CreatedOn);
                    //scriptExecuteRequestDE.modifiedby = scriptExecuteRequestSE.ModifiedBy;
                    //scriptExecuteRequestDE.modifieddate = Convert.ToDateTime(scriptExecuteRequestSE.ModifiedOn);
                }
            }

            return scriptExecuteRequestDE;
        }

    }
}

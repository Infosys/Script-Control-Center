/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.WEM.AutomationTracker.Contracts.Data
{
    [DataContract]
    public class ScriptExecuteRequest
    {
        [DataMember]
        public Guid TransactionId { get; set; }
        [DataMember]
        public int ScriptId { get; set; }
        [DataMember]
        public int CategoryId { get; set; }
        [DataMember]
        public int CompanyId { get; set; }
        [DataMember]
        public string ScriptName { get; set; }
        [DataMember]
        public string Path { get; set; }
        [DataMember]
        public List<Parameter> InParameters { get; set; }
        [DataMember]
        public string RemoteServerNames { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public int RemoteExecutionMode;
        [DataMember]
        public int ExecutionMode { get; set; }
        [DataMember]
        public string ReferenceKey { get; set; }
        [DataMember]
        public string ScheduledPattern { get; set; }
        [DataMember]
        public DateTime? ScheduleStartDateTime { get; set; }
        [DataMember]
        public DateTime? ScheduleEndDateTime { get; set; }
        [DataMember]
        public int ScheduleOccurences { get; set; }
        [DataMember]
        public int SchedulePriority { get; set; }
        [DataMember]
        public List<string> ScheduleOnClusters { get; set; }
        [DataMember]
        public string IapNodeTransport { get; set; }
        [DataMember]
        public string Domain { get; set; }   
        [DataMember]
        public string ResponseNotificationCallbackURL { get; set; }
        
    }
    public enum RemoteExecutionHost
    {
        PS = 1,
        IAPNodes = 2,
        Linux = 3
    };
    public enum ExecutionModeType
    {
        Local = 1,
        RunOnIAPNode = 2,
        ScheduledOnIAPNode = 3,
        ScheduledOnIAPCluster = 4,
        PowerShell = 5,
        Linux = 6,
        Delegate = 7
    }

    [DataContract]
    public class Parameter
    {
        [DataMember]
        public string ParameterName { get; set; }
        [DataMember]
        public string ParameterValue { get; set; }
    }

    public enum IapNodeTransportType
    {
        Http = 1,
        Nettcp = 2
    }

    public enum ScheduleStopCriteriaType
    {
        NoEndDate = 1,
        OccurenceCount = 2,
        EndDate = 3
    }

    public enum ScheduledPatternType
    {
        ScheduleNow = 1,
        ScheduleWithRecurrence = 2
    }
    
}

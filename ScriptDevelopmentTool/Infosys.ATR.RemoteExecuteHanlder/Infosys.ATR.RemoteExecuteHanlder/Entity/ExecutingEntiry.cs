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

namespace Infosys.ATR.RemoteExecute
{
    public class ExecutingEntity
    {
        public ExecutionModeType ExecutionMode { get; set; }
        public ScheduledPatternType ScheduledPattern { get; set; }
        public DateTime ScheduleStartDateTime { get; set; }
        public DateTime ScheduleEndDateTime { get; set; }
        public ScheduleStopCriteriaType ScheduleStopCriteria { get; set; }
        public int ScheduleOccurences { get; set; }
        public int SchedulePriority { get; set; }
        public List<string> RemoteServerNames { get; set; }
        public List<string> ScheduleOnClusters { get; set; }
        public ExecutingEntityType EntityType { get; set; }
        public int CategoryId { get; set; }
        public string WorkflowId { get; set; }
        public int ScriptId { get; set; }
        public int WorkflowVersion { get; set; }
        public IapNodeTransportType IapNodeTransport { get; set; }
        public bool UsesUIAutomation { get; set; }
        public List<Parameter> Parameters { get; set; }
        public int IapNodeHttpPort { get; set; }
        public int IapNodeNetTcpPort { get; set; }
        public string Domain { get; set; }
        public int ComapnyId { get; set; }
    }

    [Serializable]
    public class Parameter
    {
        public string DataType { get; set; }
        public bool IsPaired { get; set; }
        public bool IsSecret { get; set; }
        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }
        public string allowedValues { get; set; }
    }

    public class OutParameter
    {
        public string ParameterName { get; set; }
        public object ParameterValue { get; set; }
    }

    public class Result
    {
        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }
        public string InputCommand { get; set; }
        public bool IsSuccess { get; set; }
        public List<OutParameter> Output { get; set; }
        public List<string> ScheduledRequestIds { get; set; }
        public string MachineName { get; set; }
    }

    public enum ExecutionModeType
    {
        Local = 1,
        RunOnIAPNode = 2,
        ScheduledOnIAPNode = 3,
        ScheduledOnIAPCluster = 4
    }

    public enum ScheduledPatternType
    {
        ScheduleNow = 1,
        ScheduleWithRecurrence = 2
    }

    public enum ScheduleStopCriteriaType
    {
        NoEndDate = 1,
        OccurenceCount = 2,
        EndDate = 3
    }

    public enum ExecutingEntityType
    {
        Workflow = 1,
        Script = 2
    }

    public enum IapNodeTransportType
    {
        Http = 1,
        Nettcp = 2
    }
}

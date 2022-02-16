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

namespace Infosys.ATR.ExportUtility.Models
{
    public class ExportConfigurationMaster
    {
        public int id { get; set; }
        public int TargetServerId { get; set; }
        public string TargetSystemUserId { get; set; }
        public string TargetSystemPassword { get; set; }
        public int ExportStatus { get; set; }
        public Nullable<System.DateTime> CompletedOn { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }

    public class ScriptData
    {
        public int SourceCategoryId { get; set; }
        public string SourceScriptPath { get; set; }
        public int TargetCategoryId { get; set; }
        public string TargetScriptPath { get; set; }
        public int SourceScriptId { get; set; }
        public string ScriptName { get; set; }
        public Boolean Added { get; set; }
        //public int NiaTargetCategoryId { get; set; }
        //public int ActualSourceCategoryId { get; set; }
        //public Boolean SourceParentCategory { get; set; }
    }

    public class ScriptDetails
    {
        public int SourceCategoryId { get; set; }
        public string SourceScriptPath { get; set; }
        public int TargetCategoryId { get; set; }
        public string TargetScriptPath { get; set; }
        public int SourceScriptId { get; set; }
        public string ScriptName { get; set; }
    }

    public enum ExportStatus
    {
        Submitted = 0,
        InProgress = 1,
        Success = 2,
        Failed = 3,
        Resubmit = 4,
        Pending_Conflicts = 5
    }

    public enum TransactionStatus
    {
        NA = 0,
        InProgress = 1,
        Success = 2,
        Exist = 3,
        Failed = 4
    }

    public enum Action
    {
        Default = 0,
        Overwrite = 1,
        CreateNew = 2,
        Ignore = 3
    }

    public enum ExistsReasonCode
    {
        VM = 1,  // version modified on nia server
        VO = 2, // version older on server
        VU = 3 // version matches e.g. unedited
    }

    public class PastExportAction
    {
    public int Id { get; set; }
        public string Action { get; set; }
    }

}

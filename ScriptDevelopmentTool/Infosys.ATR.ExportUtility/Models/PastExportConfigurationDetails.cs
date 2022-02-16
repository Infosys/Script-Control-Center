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
   public class PastExportConfigurationDetails
    {
        public int ScriptTransactionId { get; set; }
        public string SourceScriptPath { get; set; }
        public string TargetScriptPath { get; set; }
        public string TargetScriptName { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
        //public Boolean Select { get; set; }
        public int TargetCategoryId { get; set; }       
        public int SourceScriptId { get; set; }       
        public Nullable<short> ExistReasonCode { get; set; }
        public string Details { get; set; }
        public Nullable<int> Action { get; set; }
        public int masterExportId { get; set; }
        public Nullable<System.DateTime> CompletedOn { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public int ScriptConfigurationId { get; set; }
        public int SourceCategoryId { get; set; }
        public int ExportStatus { get; set; }
    
    }
}

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

namespace Infosys.ATR.ExportUtility.Models.ECR
{
  public  class PastExport
    {
        public int ConfigurationMasterId { get; set; }
        public int ScriptConfigurationId { get; set; }
        public int ScriptTransactionId { get; set; }

        public string SourceScriptPath { get; set; }
        public string TargetScriptPath { get; set; }
        public string ScriptName { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public Boolean Select { get; set; }
        public DateTime CompletedOn { get; set; }
        public string UserName { get; set; }
    }
}

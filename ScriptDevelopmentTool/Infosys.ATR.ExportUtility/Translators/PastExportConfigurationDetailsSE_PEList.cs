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
using SE = Infosys.WEM.Export.Service.Contracts.Data;
using PE = Infosys.ATR.ExportUtility.Models;


namespace Infosys.ATR.ExportUtility.Translators
{
    public class PastExportConfigurationDetailsSE_PEList
    {
        public static List<PE.PastExportConfigurationDetails> PastExportConfigurationDetailsSEtoPEList(List<SE.PastExportConfigurationMasterDetails> configSE)
        {
            List<PE.PastExportConfigurationDetails> configPE = null;
            if (configSE != null)
            {
                configPE = new List<PE.PastExportConfigurationDetails>();
                configSE.ForEach(de =>
                {
                    configPE.Add(PastExportConfigurationDetailsSEtoPE(de));
                });
            }
            return configPE;
        }

        public static PE.PastExportConfigurationDetails PastExportConfigurationDetailsSEtoPE(SE.PastExportConfigurationMasterDetails configSE)
        {
            PE.PastExportConfigurationDetails configPE = null;
            if (configSE != null)
            {
                configPE = new PE.PastExportConfigurationDetails();
                configPE.masterExportId = configSE.masterExportId;
                configPE.CreatedBy = configSE.CreatedBy;
                configPE.CreatedOn = configSE.CreatedOn;
                configPE.CompletedOn = configSE.CompletedOn;
                configPE.ScriptConfigurationId = configSE.ScriptConfigurationId;
                configPE.SourceCategoryId = configSE.SourceCategoryId;
                configPE.TargetCategoryId = configSE.TargetCategoryId;
                configPE.SourceScriptId = configSE.SourceScriptId;
                configPE.SourceScriptPath = configSE.SourceScriptPath;
                configPE.TargetScriptPath = configSE.TargetScriptPath;
                configPE.TargetScriptName = configSE.TargetScriptName;
                configPE.ExistReasonCode = configSE.ExistReasonCode;
                if (configSE.ExistReasonCode != null)
                {
                    configPE.Remark = configSE.Details;
                    //if (configSE.ExistReasonCode == 1)
                    //{
                    //    if (configSE.Action == 2)
                    //        configPE.Remark = configSE.Details;
                    //    else
                    //        configPE.Remark = "Server version modified";
                    //}
                    //else if (configSE.ExistReasonCode == 2)
                    //{
                    //    if (configSE.Action == 2)
                    //        configPE.Remark = configSE.Details;
                    //    else
                    //        configPE.Remark = "Server version is older";
                    //}
                    //else if (configSE.ExistReasonCode == 3)
                    //{
                    //    if (configSE.Action == 2)
                    //        configPE.Remark = configSE.Details;
                    //    else
                    //        configPE.Remark = "Server version edited";

                    //}
                    //configPE.Status= ((PE.TransactionStatus)configSE.Status).ToString();
                    //configPE.Select = true;
                }
                else
                {
                    //configPE.Status = ((PE.TransactionStatus)configSE.Status).ToString();
                    configPE.Remark = "NA";
                    //configPE.Select = false;
                }

                if (configSE.Status == 0)
                    configPE.Status = "NA";
                else if (configSE.Status == 1)
                    configPE.Status = "InProgress";
                else if (configSE.Status == 2)
                    configPE.Status = "Uploaded";
                else if (configSE.Status == 3)
                    configPE.Status = "Exists";
                else if (configSE.Status == 4)
                    configPE.Status = "Failed";

                configPE.Details = configSE.Details;
                configPE.Action = configSE.Action;
                configPE.ExportStatus = configSE.ExportStatus;
                configPE.ScriptTransactionId = configSE.ScriptTransactionId;
            }
            return configPE;
        }
    }
}

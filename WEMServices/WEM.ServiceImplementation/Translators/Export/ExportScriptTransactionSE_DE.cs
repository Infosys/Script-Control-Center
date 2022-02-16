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
using DE = Infosys.WEM.Resource.Entity;
using Infosys.WEM.Infrastructure.Common;

namespace Infosys.WEM.Service.Implementation.Translators.Export
{
    public class ExportScriptTransactionSE_DE
    {
        public static List<DE.ExportTransactionDetail> ExportTransactionDetailsSEtoDEList(List<SE.ExportTransactionDetails> exportSE)
        {
            List<DE.ExportTransactionDetail> exportDE = null;
            if (exportSE != null)
            {
                exportDE = new List<DE.ExportTransactionDetail>();
                exportSE.ForEach(de =>
                {
                    exportDE.Add(ExportTransactionDetailsSEtoDE(de));
                });
            }
            return exportDE;
        }


        public static DE.ExportTransactionDetail ExportTransactionDetailsSEtoDE(SE.ExportTransactionDetails exportTrnsSE)
        {
            DE.ExportTransactionDetail exportTrnsDE = null;
            if (exportTrnsSE != null)
            {
                exportTrnsDE = new DE.ExportTransactionDetail();

                if (exportTrnsSE.id != 0)
                {
                    exportTrnsDE.id = exportTrnsSE.id;
                }

                exportTrnsDE.ExportScriptConfigurationId = exportTrnsSE.ExportScriptConfigurationId;

                exportTrnsDE.SourceCategoryId = exportTrnsSE.SourceCategoryId;
                exportTrnsDE.SourceScriptId = exportTrnsSE.SourceScriptId;

                exportTrnsDE.SourceScriptVersion = exportTrnsSE.SourceScriptVersion;
                exportTrnsDE.TargetCategoryId = exportTrnsSE.TargetCategoryId;
                exportTrnsDE.TargetScriptId = exportTrnsSE.TargetScriptId;

                exportTrnsDE.TargetScriptVersion = exportTrnsSE.TargetScriptVersion;
                exportTrnsDE.SourceScriptPath = exportTrnsSE.SourceScriptPath;
                exportTrnsDE.TargetScriptPath = exportTrnsSE.TargetScriptPath;
                exportTrnsDE.TargetScriptName = exportTrnsSE.TargetScriptName;
                exportTrnsDE.Status = exportTrnsSE.Status;

                if (exportTrnsSE.ExistReasonCode != null)
                    exportTrnsDE.ExistReasonCode = exportTrnsSE.ExistReasonCode;
                if (!string.IsNullOrEmpty(exportTrnsSE.Details))
                    exportTrnsDE.Details = exportTrnsSE.Details;
                if (exportTrnsDE.Action != -1)
                    exportTrnsDE.Action = exportTrnsSE.Action;

                exportTrnsDE.CreatedBy = exportTrnsSE.CreatedBy;
                exportTrnsDE.CreatedOn = DateTime.UtcNow;
                exportTrnsDE.isActive = exportTrnsSE.isActive;
                exportTrnsDE.IsDeleted = false;

            }
            return exportTrnsDE;
        }

        public static List<SE.ExportTransactionDetails> ExportTransactionDetailsDEtoSEList(List<DE.ExportTransactionDetail> exportDE)
        {
            List<SE.ExportTransactionDetails> exportSE = null;
            if (exportDE != null)
            {
                exportSE = new List<SE.ExportTransactionDetails>();
                exportDE.ForEach(de =>
                {
                    exportSE.Add(ExportTransactionDetailsDEtoSE(de));
                });
            }
            return exportSE;
        }


        public static SE.ExportTransactionDetails ExportTransactionDetailsDEtoSE(DE.ExportTransactionDetail exportTrnsDE)
        {
            SE.ExportTransactionDetails exportTrnsSE = null;
            if (exportTrnsDE != null)
            {
                exportTrnsSE = new SE.ExportTransactionDetails();
                exportTrnsSE.id = exportTrnsDE.id;
                //if (exportTrnsDE.id != 0)
                //{
                //    exportTrnsSE.id = exportTrnsDE.id;
                //}

                exportTrnsSE.ExportScriptConfigurationId = exportTrnsDE.ExportScriptConfigurationId;

                exportTrnsSE.SourceCategoryId = exportTrnsDE.SourceCategoryId;
                exportTrnsSE.SourceScriptId = exportTrnsDE.SourceScriptId;

                exportTrnsSE.SourceScriptVersion = exportTrnsDE.SourceScriptVersion;
                exportTrnsSE.TargetCategoryId = exportTrnsDE.TargetCategoryId;
                exportTrnsSE.TargetScriptId = exportTrnsDE.TargetScriptId;

                exportTrnsSE.SourceScriptPath = exportTrnsDE.SourceScriptPath;
                exportTrnsSE.TargetScriptPath = exportTrnsDE.TargetScriptPath;

                exportTrnsSE.TargetScriptVersion = exportTrnsDE.TargetScriptVersion;
                exportTrnsSE.TargetScriptName = exportTrnsDE.TargetScriptName;
                exportTrnsSE.Status = exportTrnsDE.Status;

                exportTrnsSE.ExistReasonCode = exportTrnsDE.ExistReasonCode;
                exportTrnsSE.Details = exportTrnsDE.Details;
                exportTrnsSE.Action = exportTrnsDE.Action;

                exportTrnsSE.CreatedBy = exportTrnsDE.CreatedBy;
                exportTrnsSE.CreatedOn = DateTime.UtcNow;
                exportTrnsSE.isActive = exportTrnsDE.isActive;
                exportTrnsSE.IsDeleted = false;

            }
            return exportTrnsSE;
        }

        public static DE.ExportTransactionDetail PastExportTransactionDetailsSEtoDE(SE.ExportTransactionDetails exportTrnsSE)
        {
            DE.ExportTransactionDetail exportTrnsDE = null;
            if (exportTrnsSE != null)
            {
                exportTrnsDE = new DE.ExportTransactionDetail();

                if (exportTrnsSE.id != 0)
                {
                    exportTrnsDE.id = exportTrnsSE.id;
                }
              
                exportTrnsDE.Status = exportTrnsSE.Status;

                if (exportTrnsSE.ExistReasonCode != null)
                    exportTrnsDE.ExistReasonCode = exportTrnsSE.ExistReasonCode;
                if (!string.IsNullOrEmpty(exportTrnsSE.Details))
                    exportTrnsDE.Details = exportTrnsSE.Details;
                //if (exportTrnsDE.Action != -1)
                    exportTrnsDE.Action = exportTrnsSE.Action;

                exportTrnsDE.ModifiedBy = exportTrnsSE.ModifiedBy;
                exportTrnsDE.ModifiedOn = exportTrnsSE.ModifiedOn;
                //exportTrnsDE.isActive = exportTrnsSE.isActive;
                //exportTrnsDE.IsDeleted = false;

            }
            return exportTrnsDE;
        }

    }
}

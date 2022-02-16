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
    public class ExportScriptConfigurationDetailsSE_DE
    {
        public static DE.ExportScriptConfigurationDetail ExportScriptConfigurationSEtoDE(SE.ExportScriptConfigurationDetails serverSE)
        {
            DE.ExportScriptConfigurationDetail ServerDE = null;
            if (serverSE != null)
            {
                ServerDE = new DE.ExportScriptConfigurationDetail();

                if (serverSE.id != 0)
                {
                    ServerDE.id = serverSE.id;
                }

                ServerDE.ExportConfigurationId = serverSE.ExportConfigurationId;
                ServerDE.SourceCategoryId = serverSE.SourceCategoryId;
                ServerDE.SourceScriptPath = serverSE.SourceScriptPath;
                ServerDE.TargetCategoryId = serverSE.TargetCategoryId;
                ServerDE.TargetScriptPath = serverSE.TargetScriptPath;
                ServerDE.SourceScriptId = serverSE.SourceScriptId;
                ServerDE.CreatedBy = serverSE.CreatedBy;
                ServerDE.CreatedOn = DateTime.UtcNow;
                ServerDE.IsDeleted = false;
            }
            return ServerDE;
        }

        public static List<SE.ExportScriptConfigurationDetails> ExportScriptConfigurationDEtoSEList(List<DE.ExportScriptConfigurationDetail> exportDE)
        {
            List<SE.ExportScriptConfigurationDetails> exportSE = null;
            if (exportDE != null)
            {
                exportSE = new List<SE.ExportScriptConfigurationDetails>();
                exportDE.ForEach(de =>
                {
                    exportSE.Add(ExportScriptConfigurationDEtoSE(de));
                });
            }
            return exportSE;
        }

        public static SE.ExportScriptConfigurationDetails ExportScriptConfigurationDEtoSE(DE.ExportScriptConfigurationDetail serverDE)
        {
            SE.ExportScriptConfigurationDetails ServerSE = null;
            if (serverDE != null)
            {
                ServerSE = new SE.ExportScriptConfigurationDetails();

                if (serverDE.id != 0)
                {
                    ServerSE.id = serverDE.id;
                }

                ServerSE.ExportConfigurationId = serverDE.ExportConfigurationId;
                ServerSE.SourceCategoryId = serverDE.SourceCategoryId;
                ServerSE.SourceScriptPath = serverDE.SourceScriptPath;
                ServerSE.TargetCategoryId = serverDE.TargetCategoryId;
                ServerSE.TargetScriptPath = serverDE.TargetScriptPath;
                ServerSE.SourceScriptId = serverDE.SourceScriptId;
                ServerSE.CreatedBy = serverDE.CreatedBy;
                ServerSE.CreatedOn = serverDE.CreatedOn;
                ServerSE.IsDeleted = serverDE.IsDeleted;
            }
            return ServerSE;
        }
    }
}

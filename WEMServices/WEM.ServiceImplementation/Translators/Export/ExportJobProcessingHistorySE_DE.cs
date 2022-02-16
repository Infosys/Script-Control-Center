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

namespace Infosys.WEM.Service.Implementation.Translators.Export
{
    public class ExportJobProcessingHistorySE_DE
    {
        public static DE.ExportJobProcessingHistory ExportConfigurationMasterSEtoDE(SE.ExportJobProcessingHistory exportJobSE)
        {
            DE.ExportJobProcessingHistory exportJobDE = null;
            if (exportJobSE != null)
            {
                exportJobDE = new DE.ExportJobProcessingHistory();

                if (exportJobSE.JobId != 0)
                {
                    exportJobDE.JobId = exportJobSE.JobId;
                }

                exportJobDE.ExportConfigurationId = exportJobSE.ExportConfigurationId;
                exportJobDE.StartedOn = exportJobSE.StartedOn;
                if (exportJobSE.CompletedOn != null)
                    exportJobDE.CompletedOn = exportJobSE.CompletedOn;
                if (!string.IsNullOrEmpty(exportJobSE.ProcessingSystemIp))
                    exportJobDE.ProcessingSystemIp = exportJobSE.ProcessingSystemIp;
                exportJobDE.ProcessingSystemName = exportJobSE.ProcessingSystemName;
                if (!string.IsNullOrEmpty(exportJobSE.ProcessedBy))
                    exportJobDE.ProcessedBy = exportJobSE.ProcessedBy;
                if (exportJobSE.CompletedOn != null)
                    exportJobDE.CompletedOn = exportJobDE.CompletedOn;
            }
            return exportJobDE;
        }
    }
}

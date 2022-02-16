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
   public class ExportConfigurationMasterPE_SE
    {
        public static SE.ExportConfigurationMaster ExportConfigurationPEtoDE(PE.ExportConfigurationMaster exportPE)
        {
            SE.ExportConfigurationMaster exportSE = null;
            //if (exportPE != null)
            //{
            //    exportSE = new SE.ExportConfigurationMaster();       

            //    exportSE.TargetServerId = exportPE.TargetServerId;
            //    exportSE.TargetSystemUserId = exportPE.TargetSystemUserId;
            //    exportSE.TargetSystemPassword = exportPE.TargetSystemPassword;
            //    exportSE.ExportStatus = exportPE.ExportStatus;
            //    exportSE.TargetServerId = exportPE.TargetServerId;            
            //}
            return exportSE;
        }

        public static PE.ExportConfigurationMaster ExportConfigurationSEtoPE(SE.ExportConfigurationMaster exportSE)
        {
            PE.ExportConfigurationMaster exportPE = null;
            if (exportSE != null)
            {
                exportPE = new PE.ExportConfigurationMaster();

                exportPE.TargetServerId = exportSE.TargetServerId;
                exportPE.TargetSystemUserId = exportSE.TargetSystemUserId;
                exportPE.TargetSystemPassword = exportSE.TargetSystemPassword;
                exportPE.ExportStatus = exportSE.ExportStatus;
                exportPE.TargetServerId = exportSE.TargetServerId;
            }
            return exportPE;
        }


        //public static PE.ExportConfigurationMaster ExportConfigurationSEtoPE(SE.ExportConfigurationMaster exportSE)
        //{
        //    PE.ExportConfigurationMaster exportPE = null;
        //    if (exportSE != null)
        //    {
        //        exportPE = new PE.ExportConfigurationMaster();

        //        exportPE.TargetServerId = exportSE.TargetServerId;
        //        exportPE.TargetSystemUserId = exportSE.TargetSystemUserId;
        //        exportPE.TargetSystemPassword = exportSE.TargetSystemPassword;
        //        exportPE.ExportStatus = exportSE.ExportStatus;
        //        exportPE.TargetServerId = exportSE.TargetServerId;
        //    }
        //    return exportPE;
        //}
    }
}

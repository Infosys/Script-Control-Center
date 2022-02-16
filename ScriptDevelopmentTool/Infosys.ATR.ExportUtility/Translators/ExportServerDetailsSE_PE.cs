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
    public class ExportServerDetailsSE_PE
    {
        public static List<PE.ExportServerDetails> ExportServerDetailsSEtoPEList(List<SE.ExportServerDetails> targetSystemDEList)
        {
            List<PE.ExportServerDetails> targetSystemSEList = null;
            if (targetSystemDEList != null)
            {
                targetSystemSEList = new List<PE.ExportServerDetails>();
                targetSystemDEList.ForEach(de =>
                {
                    targetSystemSEList.Add(ExportServerDetailsSEtoPE(de));
                });
            }
            return targetSystemSEList;
        }

        public static PE.ExportServerDetails ExportServerDetailsSEtoPE(SE.ExportServerDetails exportSE)
        {
            PE.ExportServerDetails exportPE = null;
            if (exportSE != null)
            {
                exportPE = new PE.ExportServerDetails();
                exportPE.TargetSystemId = exportSE.TargetSystemId;
                exportPE.DNSServer = exportSE.DNSServer;
                exportPE.CasServer = exportSE.CasServer;
                exportPE.CreatedBy = exportPE.CreatedBy;
                //exportPE.ServerInstance = exportSE.DNSServer;
                //if (exportSE.CreatedBy.Contains("\\"))
                //{
                //    exportPE.ServerInstance = exportSE.DNSServer + "#" + exportSE.CreatedBy.Split('\\')[1];
                //}
                //else if (exportSE.CreatedBy.Contains("@"))
                //{
                //    exportPE.ServerInstance = exportSE.DNSServer + "#" + exportSE.CreatedBy.Split('@')[1];
                //}

            }
            return exportPE;
        }
    }
}

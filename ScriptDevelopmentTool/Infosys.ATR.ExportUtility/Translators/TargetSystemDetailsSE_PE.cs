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
    public class TargetSystemDetailsSE_PE
    {
        public static List<PE.TargetSystemDetails> TargetSystemDEtoSE(List<SE.ExportTargetSystemDetails> targetSystemSEList)
        {
            List<PE.TargetSystemDetails> targetSystemPEList = null;
            if (targetSystemSEList != null)
            {
                targetSystemPEList = new List<PE.TargetSystemDetails>();
                targetSystemSEList.ForEach(de =>
                {
                    targetSystemPEList.Add(TargetSystemDEtoSE(de));
                });
            }
            return targetSystemPEList;
        }

        public static PE.TargetSystemDetails TargetSystemDEtoSE(SE.ExportTargetSystemDetails targetServerSE)
        {
            PE.TargetSystemDetails targetServerPE = null;
            if (targetServerSE != null)
            {
                targetServerPE = new PE.TargetSystemDetails();
                targetServerPE.id = targetServerSE.id;
                targetServerPE.Name = targetServerSE.Name;
                targetServerPE.Protocol = targetServerSE.Protocol;
                targetServerPE.APIType = targetServerSE.APIType;
                targetServerPE.DefaultType = targetServerSE.DefaultType;
            }
            return targetServerPE;
        }

    }
}

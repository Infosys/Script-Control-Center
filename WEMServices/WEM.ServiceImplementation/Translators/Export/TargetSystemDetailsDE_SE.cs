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
    public class TargetSystemDetailsDE_SE
    {

        public static List<SE.ExportTargetSystemDetails> TargetSystemDEtoSE(List<DE.ExportTargetSystemDetail> targetSystemDEList)
        {
            List<SE.ExportTargetSystemDetails> targetSystemSEList = null;
            if (targetSystemDEList != null)
            {
                targetSystemSEList = new List<SE.ExportTargetSystemDetails>();
                targetSystemDEList.ForEach(de =>
                {
                    targetSystemSEList.Add(TargetSystemDEtoSE(de));
                });
            }
            return targetSystemSEList;
        }

        public static SE.ExportTargetSystemDetails TargetSystemDEtoSE(DE.ExportTargetSystemDetail targetServerDE)
        {
            SE.ExportTargetSystemDetails targetServerSE = null;
            if (targetServerDE != null)
            {
                targetServerSE = new SE.ExportTargetSystemDetails();
                targetServerSE.id = targetServerDE.id;
                targetServerSE.Name = targetServerDE.Name;
                targetServerSE.Protocol = targetServerDE.Protocol;
                targetServerSE.APIType = targetServerDE.APIType;
                targetServerSE.DefaultType = targetServerDE.DefaultType;
            }
            return targetServerSE;
        }
    }
}

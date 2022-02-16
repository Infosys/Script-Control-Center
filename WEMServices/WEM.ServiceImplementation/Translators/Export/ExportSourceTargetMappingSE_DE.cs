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
    public class ExportSourceTargetMappingSE_DE
    {
        public static DE.ExportSourceTargetMapping ExportSourceTargetMappingSEtoDE(SE.ExportSourceTargetMapping targetMappingSE)
        {
            DE.ExportSourceTargetMapping targetMappingDE = null;
            if (targetMappingSE != null)
            {
                targetMappingDE = new DE.ExportSourceTargetMapping();

                if (targetMappingSE.id != 0)
                {
                    targetMappingDE.id = targetMappingSE.id;
                }

                targetMappingDE.SourceInstanceAddr = targetMappingSE.SourceInstanceAddr;
                targetMappingDE.SourceScriptCategoryId = targetMappingSE.SourceScriptCategoryId;
                if (targetMappingSE.SourceScriptId > 0)
                    targetMappingDE.SourceScriptId = targetMappingSE.SourceScriptId;
                targetMappingDE.SourceScriptVersion = targetMappingSE.SourceScriptVersion;
                targetMappingDE.TargetInstanceId = targetMappingSE.TargetInstanceId;
                targetMappingDE.TargetScriptCategoryId = targetMappingSE.TargetScriptCategoryId;
                if (targetMappingSE.TargetScriptId > 0)
                    targetMappingDE.TargetScriptId = targetMappingSE.TargetScriptId;
                targetMappingDE.CreatedBy = targetMappingSE.CreatedBy;
                targetMappingDE.CreatedOn = targetMappingSE.CreatedOn;
                targetMappingDE.IsActive = targetMappingSE.IsActive;
                targetMappingDE.SourceInstanceAddr = targetMappingSE.SourceInstanceAddr;
                targetMappingDE.SourceScriptVersion = targetMappingSE.SourceScriptVersion;
                targetMappingDE.TargetScriptVersion = targetMappingSE.TargetScriptVersion;
                if (!string.IsNullOrEmpty(targetMappingSE.ModifiedBy))
                    targetMappingDE.ModifiedBy = targetMappingSE.ModifiedBy;
                if (targetMappingSE.ModifiedOn != null)
                    targetMappingDE.ModifiedOn = targetMappingSE.ModifiedOn;
            }
            return targetMappingDE;
        }


        public static SE.ExportSourceTargetMapping ExportSourceTargetMappingDEtoSE(DE.ExportSourceTargetMapping targetMappingDE)
        {
            SE.ExportSourceTargetMapping targetMappingSE = null;
            if (targetMappingDE != null)
            {
                targetMappingSE = new SE.ExportSourceTargetMapping();

                targetMappingSE.id = targetMappingDE.id;

                targetMappingSE.SourceInstanceAddr = targetMappingDE.SourceInstanceAddr;
                targetMappingSE.SourceScriptCategoryId = targetMappingDE.SourceScriptCategoryId;
                targetMappingSE.SourceScriptId = targetMappingDE.SourceScriptId;
                targetMappingSE.SourceScriptVersion = targetMappingDE.SourceScriptVersion;
                targetMappingSE.TargetScriptVersion = targetMappingDE.TargetScriptVersion;
                targetMappingSE.TargetInstanceId = targetMappingDE.TargetInstanceId;
                targetMappingSE.TargetScriptCategoryId = targetMappingDE.TargetScriptCategoryId;
                targetMappingSE.TargetScriptId = targetMappingDE.TargetScriptId;
                targetMappingSE.CreatedBy = targetMappingDE.CreatedBy;
                targetMappingSE.CreatedOn = targetMappingDE.CreatedOn;
                targetMappingSE.IsActive = targetMappingDE.IsActive;
                targetMappingSE.SourceInstanceAddr = targetMappingDE.SourceInstanceAddr;

                if (!string.IsNullOrEmpty(targetMappingDE.ModifiedBy))
                    targetMappingSE.ModifiedBy = targetMappingDE.ModifiedBy;
                if (targetMappingDE.ModifiedOn != null)
                    targetMappingSE.ModifiedOn = targetMappingDE.ModifiedOn;
            }
            return targetMappingSE;
        }
    }
}

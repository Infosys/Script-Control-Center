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
using SE = Infosys.WEM.Service.Contracts.Data;
using DE = Infosys.WEM.Resource.Entity;

namespace Infosys.WEM.Service.Implementation.Translators
{
    public class WorkflowParamsSE_DE
    {
        public static DE.WorkflowParams WorkflowParamsSEtoDE(SE.WorkflowParam paramSE)
        {
            DE.WorkflowParams paramDE = null;
            if (paramSE != null)
            {
                paramDE = new DE.WorkflowParams();
                paramDE.PartitionKey = paramSE.WorkflowId;
                paramDE.AllowedValues = paramSE.AllowedValues;
                paramDE.CreatedBy = paramSE.CreatedBy;
                paramDE.DefaultValue = paramSE.DefaultValue;
                paramDE.IsMandatory = paramSE.IsMandatory;
                paramDE.IsSecret = paramSE.IsSecret;
                paramDE.ModifiedBy = paramSE.ModifiedBy;
                paramDE.Name = paramSE.Name;
                paramDE.ParamId = paramSE.ParamId;
                if (paramSE.ParamId != 0)
                    paramDE.RowKey = paramSE.ParamId.ToString("00000");
                paramDE.ParamType = paramSE.ParamType.ToString();
                paramDE.IsDeleted = paramSE.IsDeleted;
                paramDE.IsReferenceKey = paramSE.IsReferenceKey;
            }
            return paramDE;
        }

        public static SE.WorkflowParam WorkflowParamsDEtoSE(DE.WorkflowParams paramDE)
        {
            SE.WorkflowParam paramSE = null;
            if (paramDE != null)
            {
                paramSE = new SE.WorkflowParam();
                paramSE.AllowedValues = paramDE.AllowedValues;
                paramSE.DefaultValue = paramDE.DefaultValue;
                paramSE.IsMandatory = paramDE.IsMandatory;
                paramSE.IsSecret = paramDE.IsSecret;
                paramSE.Name = paramDE.Name;
                paramSE.ModifiedBy = paramDE.ModifiedBy;
                paramSE.CreatedBy = paramDE.CreatedBy;
                paramSE.ParamId = paramDE.ParamId;
                //paramSE.ParamType = (SE.ParamDirection)Enum.Parse(typeof(SE.ParamDirection), paramDE.ParamType);
                SE.ParamDirection tempDir;
                Enum.TryParse<SE.ParamDirection>(paramDE.ParamType, out tempDir);
                paramSE.ParamType = tempDir;
                paramSE.WorkflowId = paramDE.PartitionKey;
                paramSE.IsNew = false; // to be used while entity is returned back during publish/update
                paramSE.IsReferenceKey = paramDE.IsReferenceKey;
            }
            return paramSE;
        }

        public static List<SE.WorkflowParam> WorkflowParamsListDEtoSE(List<DE.WorkflowParams> paramDEs)
        {
            List<SE.WorkflowParam> paramSEs = null;
            if (paramDEs != null)
            {
                paramSEs = new List<SE.WorkflowParam>();
                paramDEs.ForEach(de =>
                {
                    paramSEs.Add(WorkflowParamsDEtoSE(de));
                });
            }
            return paramSEs;
        }

        public static List<DE.WorkflowParams> WorkflowParamsListSEtoDE(List<SE.WorkflowParam> paramSEs)
        {
            List<DE.WorkflowParams> paramDEs = null;
            if (paramSEs != null)
            {
                paramDEs = new List<DE.WorkflowParams>();
                paramSEs.ForEach(se =>
                {
                    paramDEs.Add(WorkflowParamsSEtoDE(se));
                });
            }
            return paramDEs;
        }
    }
}

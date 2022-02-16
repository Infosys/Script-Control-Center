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

using SE = Infosys.WEM.Scripts.Service.Contracts.Data;
using DE = Infosys.WEM.Resource.Entity;
using Infosys.WEM.Infrastructure.Common;

namespace Infosys.WEM.Service.Implementation.Translators.Scripts
{
    public class ScriptParamsSE_DE
    {
        public static DE.ScriptParams ScriptParamsSEtoDE(SE.ScriptParam paramSE)
        {
            DE.ScriptParams paramDE = null;
            if(paramSE !=null)
            {
                paramDE = new DE.ScriptParams();
                paramDE.PartitionKey = paramSE.ScriptId.ToString("00000");
                paramDE.AllowedValues = paramSE.AllowedValues;
                paramDE.CreatedBy = Utility.GetLoggedInUser();//paramSE.CreatedBy;
                paramDE.DefaultValue = paramSE.DefaultValue;
                paramDE.IsMandatory = paramSE.IsMandatory;
                paramDE.IsSecret = paramSE.IsSecret;
                paramDE.ModifiedBy = Utility.GetLoggedInUser(); //paramSE.ModifiedBy;
                paramDE.Name = paramSE.Name;
                paramDE.ParamId = paramSE.ParamId;
                if(paramSE.ParamId != 0)
                    paramDE.RowKey = paramSE.ParamId.ToString("00000");
                paramDE.ParamType = paramSE.ParamType.ToString();
                paramDE.IsDeleted = paramSE.IsDeleted;
                paramDE.IsUnnamed = paramSE.IsUnnamed;
                paramDE.DataType = paramSE.DataType;
                paramDE.IsReferenceKey = paramSE.IsReferenceKey;
            }
            return paramDE; 
        }

        public static SE.ScriptParam ScriptParamsDEtoSE(DE.ScriptParams paramDE)
        {
            SE.ScriptParam paramSE = null;
            if (paramDE != null)
            {
                paramSE = new SE.ScriptParam();
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
                paramSE.ScriptId = int.Parse(paramDE.PartitionKey);
                paramSE.IsUnnamed = paramDE.IsUnnamed;
                paramSE.DataType = paramDE.DataType;
                paramSE.IsReferenceKey = paramDE.IsReferenceKey;
            }
            return paramSE;
        }

        public static List<SE.ScriptParam> ScriptParamsListDEtoSE(List<DE.ScriptParams> paramDEs)
        {
            List<SE.ScriptParam> paramSEs = null;
            if (paramDEs != null)
            {
                paramSEs = new List<SE.ScriptParam>();
                paramDEs.ForEach(de => {
                    paramSEs.Add(ScriptParamsDEtoSE(de));
                });
            }
            return paramSEs;
        }

        public static List<DE.ScriptParams> ScriptParamsListSEtoDE(List<SE.ScriptParam> paramSEs)
        {
            List<DE.ScriptParams> paramDEs = null;
            if (paramSEs != null)
            {
                paramDEs = new List<DE.ScriptParams>();
                paramSEs.ForEach(se => {
                    paramDEs.Add(ScriptParamsSEtoDE(se));
                });
            }
            return paramDEs;
        }
    }
}

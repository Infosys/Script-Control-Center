using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SE = Infosys.WEM.Scripts.Service.Contracts.Data;
using PE = Infosys.ATR.ScriptRepository.Models;

namespace Infosys.ATR.ScriptRepository.Translators
{
    public class ScriptParameterPE_SE
    {
        public static PE.ScriptParameter ScriptParameterSEtoPE(SE.ScriptParam paramSE)
        {
            PE.ScriptParameter paramPE = null;
            if (paramSE != null)
            {
                paramPE = new PE.ScriptParameter();
                paramPE.AllowedValues = paramSE.AllowedValues;
                PE.ParameterIOTypes tempIO;
                Enum.TryParse<PE.ParameterIOTypes>(paramSE.ParamType.ToString(), out tempIO);
                paramPE.IOType = tempIO;
                paramPE.IsMandatory = paramSE.IsMandatory;
                paramPE.IsSecret = paramSE.IsSecret;
                paramPE.Name = paramSE.Name;
                paramPE.ParamId = paramSE.ParamId.ToString();
                paramPE.DefaultValue = paramSE.DefaultValue;
                paramPE.IsPaired = paramSE.IsPaired;
            }
            return paramPE;
        }

        public static SE.ScriptParam ScriptParameterPEtoSE(PE.ScriptParameter paramPE)
        {
            SE.ScriptParam paramSE = null;
            if (paramPE != null)
            {
                paramSE = new SE.ScriptParam();
                paramSE.AllowedValues = paramPE.AllowedValues;
                paramSE.CreatedBy = paramPE.CreatedBy;
                paramSE.DefaultValue = paramPE.DefaultValue;
                paramSE.IsMandatory = paramPE.IsMandatory;
                paramSE.IsSecret = paramPE.IsSecret;
                paramSE.ModifiedBy = paramPE.ModifiedBy;
                paramSE.Name = paramPE.Name;
                int tempId;
                if (!string.IsNullOrEmpty(paramPE.ParamId) && int.TryParse(paramPE.ParamId, out tempId))
                    paramSE.ParamId = tempId;
                if (!string.IsNullOrEmpty(paramPE.ScriptId))
                    paramSE.ScriptId = int.Parse(paramPE.ScriptId);
                paramSE.ParamType = (SE.ParamDirection)paramPE.IOType;
                paramSE.IsPaired = paramPE.IsPaired;
            }
            return paramSE;
        }

        public static List<PE.ScriptParameter> ScriptParameterListSEtoPE(List<SE.ScriptParam> paramlistSE)
        {
            List<PE.ScriptParameter> paramlistPE = null;
            if (paramlistSE != null && paramlistSE.Count > 0)
            {
                paramlistPE = new List<PE.ScriptParameter>();
                paramlistSE.ForEach(p => {
                    paramlistPE.Add(ScriptParameterSEtoPE(p));
                });
            }
            return paramlistPE;
        }

        public static List<SE.ScriptParam> ScriptParameterListPEtoSE(List<PE.ScriptParameter> paramlistPE)
        {
            List<SE.ScriptParam> paramlistSE = null;
            if (paramlistPE != null && paramlistPE.Count > 0)
            {
                paramlistSE = new List<SE.ScriptParam>();
                paramlistPE.ForEach(pe =>
                {
                    paramlistSE.Add(ScriptParameterPEtoSE(pe));
                });
            }
            return paramlistSE;
        }
    }
}

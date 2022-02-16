/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SE = Infosys.WEM.Scripts.Service.Contracts.Data;
using PE = Infosys.ATR.ScriptRepository.Models;
using OE = Infosys.IAP.CommonClientLibrary.Models;

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
                paramPE.IsUnnamed = paramSE.IsUnnamed;
                paramPE.Name = paramSE.Name;
                paramPE.ParamId = paramSE.ParamId.ToString();
                paramPE.DefaultValue = paramSE.DefaultValue;
                paramPE.DataType = paramSE.DataType;
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
                paramSE.IsUnnamed = paramPE.IsUnnamed;
                paramSE.ModifiedBy = paramPE.ModifiedBy;
                paramSE.Name = paramPE.Name;
                int tempId;
                if (!string.IsNullOrEmpty(paramPE.ParamId) && int.TryParse(paramPE.ParamId, out tempId))
                    paramSE.ParamId = tempId;
                if (!string.IsNullOrEmpty(paramPE.ScriptId))
                    paramSE.ScriptId = int.Parse(paramPE.ScriptId);
                paramSE.ParamType = (SE.ParamDirection)paramPE.IOType;
                paramSE.DataType = paramPE.DataType;
            }
            return paramSE;
        }

        public static OE.ContentParameter ScriptParameterPEtoOE(PE.ScriptParameter paramPE)
        {
            OE.ContentParameter paramSE = null;
            if (paramPE != null)
            {
                paramSE = new OE.ContentParameter();
                paramSE.AllowedValues = paramPE.AllowedValues;               
                paramSE.DefaultValue = paramPE.DefaultValue;
                paramSE.IsMandatory = paramPE.IsMandatory;
                paramSE.IsSecret = paramPE.IsSecret;
                paramSE.IsUnnamed = paramPE.IsUnnamed;               
                paramSE.Name = paramPE.Name;
                paramSE.IOType = (OE.ParameterIOTypes)paramPE.IOType;
                paramSE.DataType = paramPE.DataType;
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

        public static List<OE.ContentParameter> ScriptParameterListPEtoOE(List<PE.ScriptParameter> paramlistPE)
        {
            List<OE.ContentParameter> paramlistSE = null;
            if (paramlistPE != null && paramlistPE.Count > 0)
            {
                paramlistSE = new List<OE.ContentParameter>();
                paramlistPE.ForEach(pe =>
                {
                    paramlistSE.Add(ScriptParameterPEtoOE(pe));
                });
            }
            return paramlistSE;
        }

        public static List<PE.ScriptParameter> ScriptParameterListOEtoPE(List<OE.ContentParameter> paramlistSE) 
        {
            List<PE.ScriptParameter> paramlistPE = null;
            if (paramlistSE != null && paramlistSE.Count > 0)
            {
                paramlistPE = new List<PE.ScriptParameter>();
                paramlistSE.ForEach(p =>
                {
                    paramlistPE.Add(ScriptParameterOEtoPE(p));
                });
            }
            return paramlistPE;
        }


        public static PE.ScriptParameter ScriptParameterOEtoPE(OE.ContentParameter paramSE)
        {
            PE.ScriptParameter paramPE = null;
            if (paramSE != null)
            {
                paramPE = new PE.ScriptParameter();
                paramPE.AllowedValues = paramSE.AllowedValues;
                PE.ParameterIOTypes tempIO;
                Enum.TryParse<PE.ParameterIOTypes>(paramSE.IOType.ToString(), out tempIO);
                paramPE.IOType = tempIO;
                paramPE.IsMandatory = paramSE.IsMandatory;
                paramPE.IsSecret = paramSE.IsSecret;
                paramPE.IsUnnamed = paramSE.IsUnnamed;
                paramPE.Name = paramSE.Name;              
                paramPE.DefaultValue = paramSE.DefaultValue;
                paramPE.DataType = paramSE.DataType;
            }
            return paramPE;
        }
    }
}

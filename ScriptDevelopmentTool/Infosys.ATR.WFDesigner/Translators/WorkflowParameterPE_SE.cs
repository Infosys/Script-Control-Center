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
using PE = Infosys.ATR.WFDesigner.Entities;
using OE = Infosys.IAP.CommonClientLibrary.Models;
using Infosys.WEM.SecureHandler;

namespace Infosys.ATR.WFDesigner.Translators
{
    public class WorkflowParameterPE_SE
    {
        public static SE.WorkflowParam WorkflowParameterPEtoSE(PE.WorkflowParameterPE paramPE)
        {
            SE.WorkflowParam paramSE = null;
            if (paramPE != null)
            {
                paramSE = new SE.WorkflowParam();
                paramSE.AllowedValues = paramPE.AllowedValues;
                paramSE.CreatedBy = paramPE.CreatedBy;
                paramSE.DefaultValue = (paramPE.IsSecret) ? SecurePayload.Secure(paramPE.DefaultValue, "IAP2GO_SEC!URE") : paramPE.DefaultValue;
                paramSE.IsDeleted = paramPE.IsDeleted;
                paramSE.IsMandatory = paramPE.IsMandatory;
                paramSE.IsSecret = paramPE.IsSecret;
                paramSE.ModifiedBy = paramPE.ModifiedBy;
                paramSE.Name = paramPE.Name;
                int id;
                if (int.TryParse(paramPE.Id, out id))
                    paramSE.ParamId = id;
                paramSE.ParamType = (SE.ParamDirection)Enum.Parse(typeof(SE.ParamDirection), paramPE.ParamIOType.ToString());
                paramSE.WorkflowId = paramPE.WorkflowId;
                paramSE.IsNew = paramPE.IsNew;
                paramSE.IsReferenceKey = paramPE.IsReferenceKey;
            }
            return paramSE;
        }

        public static List<SE.WorkflowParam> WorkflowParameterListPEtoSE(List<PE.WorkflowParameterPE> paramPEs)
        {
            List<SE.WorkflowParam> paramSes = null;
            if (paramPEs != null && paramPEs.Count > 0)
            {
                paramSes = new List<SE.WorkflowParam>();
                paramPEs.ForEach(pe => {
                    paramSes.Add(WorkflowParameterPEtoSE(pe));
                });
            }
            return paramSes;
        }

        public static OE.ContentParameter WorkflowParameterPEtoOE(PE.WorkflowParameterPE paramPE)
        {
            OE.ContentParameter paramSE = null;
            if (paramPE != null)
            {
                paramSE = new OE.ContentParameter();
                paramSE.AllowedValues = paramPE.AllowedValues;                
                paramSE.DefaultValue = (paramPE.IsSecret) ? SecurePayload.Secure(paramPE.DefaultValue, "IAP2GO_SEC!URE") : paramPE.DefaultValue;                
                paramSE.IsMandatory = paramPE.IsMandatory;
                paramSE.IsSecret = paramPE.IsSecret;               
                paramSE.Name = paramPE.Name;                
                paramSE.IOType = (OE.ParameterIOTypes)Enum.Parse(typeof(SE.ParamDirection), paramPE.ParamIOType.ToString());
                paramSE.IsReferenceKey = paramPE.IsReferenceKey;               
            }
            return paramSE;
        }

        public static List<OE.ContentParameter> WorkflowParameterListPEtoOE(List<PE.WorkflowParameterPE> paramPEs) 
        {
            List<OE.ContentParameter> paramSes = null;
            if (paramPEs != null && paramPEs.Count > 0)
            {
                paramSes = new List<OE.ContentParameter>();
                paramPEs.ForEach(pe =>
                {
                    paramSes.Add(WorkflowParameterPEtoOE(pe));
                });
            }
            return paramSes;
        }

        public static PE.WorkflowParameterPE WorkflowParameterSEtoPE(SE.WorkflowParam paramSE)
        {
            PE.WorkflowParameterPE paramPE = null;
            if (paramSE != null)
            {
                paramPE = new PE.WorkflowParameterPE();
                paramPE.AllowedValues = paramSE.AllowedValues;
                paramPE.CreatedBy = paramSE.CreatedBy;
                paramPE.DefaultValue = (paramSE.IsSecret)?SecurePayload.UnSecure(paramSE.DefaultValue, "IAP2GO_SEC!URE") : paramSE.DefaultValue;
                paramPE.IsDeleted = paramSE.IsDeleted;
                paramPE.IsMandatory = paramSE.IsMandatory;
                paramPE.IsSecret = paramSE.IsSecret;
                paramPE.ModifiedBy = paramSE.ModifiedBy;
                paramPE.Name = paramSE.Name;
                paramPE.ParamIOType = (PE.ParameterIOTypes)Enum.Parse(typeof(PE.ParameterIOTypes), paramSE.ParamType.ToString());
                paramPE.WorkflowId = paramSE.WorkflowId;
                paramPE.Id = paramSE.ParamId.ToString();
                paramPE.IsReferenceKey = paramSE.IsReferenceKey;                
            }
            return paramPE;
        }

        public static List<PE.WorkflowParameterPE> WorkflowParameterListSEtoPE(List<SE.WorkflowParam> paramSEs)
        {
            List<PE.WorkflowParameterPE> paramPEs = null;
            if (paramSEs != null && paramSEs.Count > 0)
            {
                paramPEs = new List<PE.WorkflowParameterPE>();
                paramSEs.ForEach(se => {
                    paramPEs.Add(WorkflowParameterSEtoPE(se));
                });
            }
            return paramPEs;
        }

        public static PE.WorkflowParameterPE WorkflowParameterOEtoPE(OE.ContentParameter paramSE)
        {
            PE.WorkflowParameterPE paramPE = null;
            if (paramSE != null)
            {
                paramPE = new PE.WorkflowParameterPE();
                paramPE.AllowedValues = paramSE.AllowedValues;                
                paramPE.DefaultValue = (paramSE.IsSecret) ? SecurePayload.UnSecure(paramSE.DefaultValue, "IAP2GO_SEC!URE") : paramSE.DefaultValue;
                paramPE.IsMandatory = paramSE.IsMandatory;
                paramPE.IsSecret = paramSE.IsSecret;
                paramPE.Name = paramSE.Name;
                paramPE.ParamIOType = (PE.ParameterIOTypes)Enum.Parse(typeof(PE.ParameterIOTypes), paramSE.IOType.ToString());
                paramPE.IsReferenceKey = paramSE.IsReferenceKey;
            }
            return paramPE;
        }

        public static List<PE.WorkflowParameterPE> WorkflowParameterListOEtoPE(List<OE.ContentParameter> paramSEs)
        {
            List<PE.WorkflowParameterPE> paramPEs = null;
            if (paramSEs != null && paramSEs.Count > 0)
            {
                paramPEs = new List<PE.WorkflowParameterPE>();
                paramSEs.ForEach(se =>
                {
                    paramPEs.Add(WorkflowParameterOEtoPE(se));
                });
            }
            return paramPEs;
        }
    }
}

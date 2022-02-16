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

namespace Infosys.ATR.WFDesigner.Translators
{
    public class WorkflowPE_SE
    {
        public static PE.WorkflowPE WorkflowOEtoPE(OE.ContentMeta wfOE)  
        { 
            PE.WorkflowPE wfPE = null;
            if (wfOE != null)
            {
                wfPE = new PE.WorkflowPE();
                wfPE.Name = wfOE.Name;
                wfPE.Description = wfOE.Description;
                wfPE.UsesUIAutomation = wfOE.UsesUIAutomation;
                wfPE.Tags=wfOE.Tags;
                wfPE.Parameters = WorkflowParameterPE_SE.WorkflowParameterListOEtoPE(wfOE.Parameters);
                wfPE.WorkflowID = wfOE.WorkflowID;
                wfPE.WorkflowVersion = wfOE.Version;
                wfPE.WorkflowURI = wfOE.WorkflowURI;
                wfPE.FileName = Convert.ToString(wfOE.Version) + "." + wfOE.ContentType;
            }
            return wfPE;
        }



        public static OE.ContentMeta WorkflowPEtoOE(PE.WorkflowPE wfPE) 
        {
            OE.ContentMeta wfOE = null;
            if (wfPE != null)
            {
                wfOE = new OE.ContentMeta();
                wfOE.Name = wfPE.Name;
                wfOE.Description = wfPE.Description;
                wfOE.UsesUIAutomation = wfPE.UsesUIAutomation;
                wfOE.Tags = wfPE.Tags;
                wfOE.Parameters = WorkflowParameterPE_SE.WorkflowParameterListPEtoOE(wfPE.Parameters);
                wfOE.WorkflowID = wfPE.WorkflowID;
                wfOE.Version = wfPE.WorkflowVersion;
                wfOE.WorkflowURI = wfPE.WorkflowURI;
                wfOE.ContentType = System.IO.Path.GetExtension(wfPE.WorkflowURI).Replace(".","");
                wfOE.ModuleType = OE.ModuleType.Workflow;      
            }
            return wfOE;
        }
    }
}

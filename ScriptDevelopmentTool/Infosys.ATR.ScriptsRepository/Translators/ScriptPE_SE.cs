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
    public class ScriptPE_SE
    {
        public static PE.Script ScriptSEtoPE(SE.Script scriptSE)
        {
            PE.Script scriptPE = null;
            if (scriptSE != null)
            {
                scriptPE = new PE.Script();
                scriptPE.Description = scriptSE.Description;
                scriptPE.CategoryId = scriptSE.CategoryId.ToString();
                scriptPE.Id = scriptSE.ScriptId.ToString();
                scriptPE.Name = scriptSE.Name;
                scriptPE.ScriptLocation = scriptSE.ScriptURL;
                scriptPE.ScriptType = scriptSE.ScriptType;
                scriptPE.ArgumentString = scriptSE.ArgString;
                scriptPE.TaskCommand = scriptSE.TaskCmd;
                scriptPE.TaskType = scriptSE.TaskType;
                scriptPE.WorkingDir = scriptSE.WorkingDir;
                scriptPE.UsesUIAutomation = scriptSE.UsesUIAutomation;
                scriptPE.Version = scriptSE.ScriptFileVersion.ToString();
                //scriptPE.SubCategory = scriptSE.SubCategoryId.ToString();
                scriptPE.SourceUrl = scriptSE.SourceUrl;
                scriptPE.Parameters = ScriptParameterPE_SE.ScriptParameterListSEtoPE(scriptSE.Parameters);
            }
            return scriptPE;
        }

        public static SE.Script ScriptPEtoSE(PE.Script scriptPE)
        {
            SE.Script scriptSE = null;
            if (scriptPE != null)
            {
                scriptSE = new SE.Script();
                scriptSE.ArgString = scriptPE.ArgumentString;
                scriptSE.BelongsToOrg = System.Configuration.ConfigurationManager.AppSettings["Company"];
                scriptSE.Description = scriptPE.Description;
                scriptSE.Name = scriptPE.Name;
                //scriptSE.Parameters
                if(! string.IsNullOrEmpty(scriptPE.Id))
                    scriptSE.ScriptId = int.Parse(scriptPE.Id);
                scriptSE.ScriptType = scriptPE.ScriptType;
                //scriptSE.ScriptURL
                scriptSE.TaskCmd = scriptPE.TaskCommand;
                scriptSE.TaskType = scriptPE.TaskType;
                scriptSE.WorkingDir = scriptPE.WorkingDir;
                scriptPE.UsesUIAutomation = scriptSE.UsesUIAutomation;
            }
            return scriptSE;
        }

        public static List<PE.Script> ScriptListSEtoPE(List<SE.Script> listscriptSE)
        {
            List<PE.Script> listscriptPE = null;
            if (listscriptSE != null)
            {
                listscriptPE = new List<PE.Script>();
                listscriptSE.ForEach(se =>
                {
                    listscriptPE.Add(ScriptSEtoPE(se));
                });
            }
            return listscriptPE;
        }

        public static List<SE.Script> ScriptListSEtoPE(List<PE.Script> listscriptPE)
        {
            List<SE.Script> listscriptSE = null;
            if (listscriptPE != null)
            {
                listscriptSE = new List<SE.Script>();
                listscriptPE.ForEach(pe =>
                {
                    listscriptSE.Add(ScriptPEtoSE(pe));
                });
            }            
            return listscriptSE;
        }

        public static PE.Script ScriptOEtoPE(OE.ContentMeta contentMeta)
        {
            PE.Script scriptPE = null;

            if (contentMeta != null)
            {
                scriptPE = new PE.Script();
                scriptPE.Description = contentMeta.Description;
                scriptPE.Name = contentMeta.Name;
                scriptPE.ScriptLocation = contentMeta.ModuleLocation;
                scriptPE.ScriptType = contentMeta.ContentType;
                scriptPE.ArgumentString = contentMeta.ArgumentString;
                scriptPE.TaskCommand = contentMeta.TaskCommand;
                scriptPE.TaskType = contentMeta.TaskType;
                scriptPE.WorkingDir = contentMeta.WorkingDir;
                scriptPE.UsesUIAutomation = contentMeta.UsesUIAutomation;
                scriptPE.Parameters = ScriptParameterPE_SE.ScriptParameterListOEtoPE(contentMeta.Parameters);                
                scriptPE.Id = "0";
            }
            return scriptPE;
        }
    }
}

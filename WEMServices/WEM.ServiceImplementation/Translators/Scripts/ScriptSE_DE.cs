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
using Infosys.WEM.Infrastructure.Common;

using SE = Infosys.WEM.Scripts.Service.Contracts.Data;
using DE = Infosys.WEM.Resource.Entity;
using Infosys.WEM.Resource.DataAccess;

namespace Infosys.WEM.Service.Implementation.Translators.Scripts
{
    public class ScriptSE_DE
    {
        public static DE.Script ScriptSEtoDE(SE.Script scriptSE)
        {
            DE.Script scriptDE = null;
            if (scriptSE != null)
            {
                scriptDE = new DE.Script();
                scriptDE.PartitionKey = scriptSE.CategoryId.ToString("00000");
                scriptDE.CategoryId = scriptSE.CategoryId;
                if (scriptSE.ScriptId != 0)
                {
                    //scriptDE.RowKey = scriptSE.ScriptId.ToString("00000");
                    scriptDE.ScriptId = scriptSE.ScriptId;
                }
                scriptDE.ArgString = scriptSE.ArgString;
                scriptDE.BelongsToAccount = scriptSE.BelongsToAccount;

                //the below part is commented, as from the client, the Id of the company wud be coming
                //instaed of the name of the company
                //if (!string.IsNullOrEmpty(scriptSE.BelongsToOrg))
                //{
                //    CompaniesDS companyDs = new CompaniesDS();
                //    Resource.Entity.Companies tempComp = companyDs.GetAll(new Resource.Entity.Companies() { PartitionKey = "IAP" }).ToList().Where(c => c.Name.ToLower() == scriptSE.BelongsToOrg.ToLower()).FirstOrDefault();
                //    if (tempComp != null)
                //        scriptDE.BelongsToOrg = tempComp.RowKey;
                //}
                int tempCompId = 0;
                if (int.TryParse(scriptSE.BelongsToOrg, out tempCompId))
                {
                    scriptDE.BelongsToOrg = tempCompId.ToString("00000");
                }

                scriptDE.BelongsToTrack = scriptSE.BelongsToTrack;
                scriptDE.CreatedBy = Utility.GetLoggedInUser();// scriptSE.CreatedBy;
                scriptDE.Description = scriptSE.Description;
                scriptDE.ModifiedBy = Utility.GetLoggedInUser();// scriptSE.ModifiedBy;
                scriptDE.Name = scriptSE.Name;
                if (string.IsNullOrEmpty(scriptSE.ScriptType) && !string.IsNullOrEmpty(scriptSE.ScriptURL))
                {
                    scriptDE.ScriptType = scriptSE.ScriptURL.Substring(scriptSE.ScriptURL.IndexOf('.') + 1);
                }
                else
                    scriptDE.ScriptType = scriptSE.ScriptType;
                if (scriptSE.ScriptURL != null)
                    scriptDE.ScriptURL = scriptSE.ScriptURL;
                scriptDE.TaskCmd = scriptSE.TaskCmd;
                scriptDE.TaskType = scriptSE.TaskType;
                scriptDE.Version = scriptSE.ScriptFileVersion;
                scriptDE.WorkingDir = scriptSE.WorkingDir;
                scriptDE.IsDeleted = scriptSE.IsDeleted;
                scriptDE.RunAsAdmin = scriptSE.RunAsAdmin;
                scriptDE.UsesUIAutomation = scriptSE.UsesUIAutomation;
                scriptDE.IfeaScriptName = scriptSE.IfeaScriptName;
                scriptDE.ModifiedOn = DateTime.UtcNow;// scriptSE.ModifiedOn;
                scriptDE.CreatedOn = DateTime.UtcNow; //scriptSE.CreatedOn;
                scriptDE.CallMethod = scriptSE.CallMethod;
                scriptDE.Tags = scriptSE.Tags;
                scriptDE.LicenseType = scriptSE.LicenseType;
                scriptDE.SourceUrl = scriptSE.SourceUrl;
                if (!string.IsNullOrEmpty(scriptSE.ExternalReferences))
                    scriptDE.ExternalReferences = scriptSE.ExternalReferences;

                //scriptRepoRequest.ScriptUrl = "/iapscriptstore/" + scriptRepoRequest.ScriptContainer + "/" + scriptRepoRequest.FileName;                
            }
            return scriptDE;
        }

        public static SE.Script ScriptDEtoSE(DE.Script scriptDE)
        {
            SE.Script scriptSE = null;
            if (scriptDE != null)
            {
                scriptSE = new SE.Script();
                //scriptSE.CategoryId = int.Parse(scriptDE.PartitionKey);
                scriptSE.CategoryId = scriptDE.CategoryId;
                scriptSE.ScriptId = scriptDE.ScriptId;
                scriptSE.ArgString = scriptDE.ArgString;
                scriptSE.BelongsToAccount = scriptDE.BelongsToAccount;
                scriptSE.BelongsToOrg = scriptDE.BelongsToOrg;
                scriptSE.BelongsToTrack = scriptDE.BelongsToTrack;
                scriptSE.Description = scriptDE.Description;
                scriptSE.Name = scriptDE.Name;
                scriptSE.ScriptType = scriptDE.ScriptType;
                scriptSE.ScriptURL = scriptDE.ScriptURL;
                scriptSE.TaskCmd = scriptDE.TaskCmd;
                scriptSE.TaskType = scriptDE.TaskType;
                scriptSE.WorkingDir = scriptDE.WorkingDir;
                scriptSE.ScriptFileVersion = scriptDE.Version;
                scriptSE.RunAsAdmin = scriptDE.RunAsAdmin == null ? false : (bool)scriptDE.RunAsAdmin;
                scriptSE.UsesUIAutomation = scriptDE.UsesUIAutomation.GetValueOrDefault();
                scriptSE.IfeaScriptName = scriptDE.IfeaScriptName;
                scriptSE.CreatedOn = scriptDE.CreatedOn;
                scriptSE.ModifiedOn = scriptDE.ModifiedOn;
                scriptSE.CreatedBy = scriptDE.CreatedBy;
                scriptSE.ModifiedBy = scriptDE.ModifiedBy;
                scriptSE.CallMethod = scriptDE.CallMethod;
                scriptSE.Tags = scriptDE.Tags;
                scriptSE.LicenseType = scriptDE.LicenseType;
                scriptSE.SourceUrl = scriptDE.SourceUrl;
                scriptSE.ScriptFileVersion = scriptDE.Version;
                if (!string.IsNullOrEmpty(scriptDE.ExternalReferences))
                    scriptSE.ExternalReferences = scriptDE.ExternalReferences;
            }

            return scriptSE;
        }

        public static List<SE.Script> ScriptDEListtoSEList(List<DE.Script> scriptDEList)
        {
            List<SE.Script> scriptSEList = null;
            if (scriptDEList != null)
            {
                scriptSEList = new List<SE.Script>();
                scriptDEList.ForEach(de =>
                {
                    scriptSEList.Add(ScriptDEtoSE(de));
                });
            }
            return scriptSEList;
        }
    }
}

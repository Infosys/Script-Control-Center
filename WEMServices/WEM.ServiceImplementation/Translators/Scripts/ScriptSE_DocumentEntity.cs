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
using DocE = Infosys.WEM.Resource.Entity.Document;
using Infosys.WEM.Resource.DataAccess;
using System.IO;

namespace Infosys.WEM.Service.Implementation.Translators.Scripts
{
    public class ScriptSE_DocumentEntity
    {
        public static DocE.Script ScriptSEtoDocumentEntity(SE.Script scriptSE)
        {
            DocE.Script scriptDoc = null;
            if (scriptSE != null)
            {
                scriptDoc = new DocE.Script();
                scriptDoc.File = new MemoryStream(scriptSE.ScriptContent);
                scriptDoc.FileName = "S" + scriptSE.ScriptId.ToString("00000") + "_" + scriptSE.ScriptFileVersion + "." + scriptSE.ScriptType;
                //scriptDoc.FileName = scriptSE.Name + "_" + scriptSE.ScriptFileVersion + "." + scriptSE.ScriptType;
                scriptDoc.ScriptVer = scriptSE.ScriptFileVersion;
                scriptDoc.UploadedBy = Utility.GetLoggedInUser();// !string.IsNullOrEmpty(scriptSE.CreatedBy) ? scriptSE.CreatedBy : scriptSE.ModifiedBy;
                int tempCompId = 0;
                if (int.TryParse(scriptSE.BelongsToOrg, out tempCompId))
                {
                    scriptDoc.CompanyId = tempCompId.ToString("00000");
                    scriptDoc.StorageBaseURL = scriptSE.StorageBaseUrl;

                    //CompaniesDS companyDs = new CompaniesDS();
                    //the below part is commented, as from the client, the Id of the company wud be coming
                    //instaed of the name of the company
                    //Resource.Entity.Companies tempComp = companyDs.GetAll(new Resource.Entity.Companies() { PartitionKey = "IAP" }).ToList().Where(c => c.Name.ToLower() == scriptSE.BelongsToOrg.ToLower()).FirstOrDefault();
                    
                    //Resource.Entity.Companies tempComp = companyDs.GetOne(new Resource.Entity.Companies() { PartitionKey = "IAP", RowKey = scriptDoc.CompanyId });
                    //if (tempComp != null)
                    //{
                    //    scriptDoc.StorageBaseURL = tempComp.StorageBaseUrl;
                    //}
                    scriptDoc.ScriptContainer = "S_" + scriptDoc.CompanyId + "_" + scriptSE.ScriptId;
                }
            }
            return scriptDoc;
        }
    }
}

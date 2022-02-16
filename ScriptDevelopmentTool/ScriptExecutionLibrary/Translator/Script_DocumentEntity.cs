/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.WEM.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SE = Infosys.WEM.Scripts.Service.Contracts.Data;

namespace Infosys.WEM.ScriptExecutionLibrary.Translator
{
    public class Script_DocumentEntity
    {
        public static Infosys.ATR.RepositoryAccess.Entity.ScriptDoc ScriptToDocument(SE.Script scriptSE, ScriptIndentifier scriptIdentifier)
        {
            using (LogHandler.TraceOperations("Script_DocumentEntity:ScriptToDocument", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
            {
                Infosys.ATR.RepositoryAccess.Entity.ScriptDoc scriptDoc = null;
                if (scriptSE != null)
                {
                    scriptDoc = new Infosys.ATR.RepositoryAccess.Entity.ScriptDoc();
                    //the below approach to create the file name is commented because of the scenario 
                    //where the script version is changed, but the factor whihc resulted in script version change in not the file content
                    //as a result during update of script, if no new file is sent but olde version has a file linked
                    //then the script url from old entry is copied to the new entry for new script version

                    //hence file name needs to be identified from the script url property of the scriptSE                
                    string[] scriptUrlParts = scriptSE.ScriptURL.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    if (scriptUrlParts != null && scriptUrlParts.Length > 0)
                    {
                        scriptDoc.FileName = scriptUrlParts[scriptUrlParts.Length - 1];
                    }
                    //scriptDoc.FileName = "S" + scriptSE.ScriptId.ToString("00000") + "_" + scriptSE.ScriptFileVersion + "." + scriptSE.ScriptType;

                    scriptDoc.ScriptVer = scriptSE.ScriptFileVersion;
                    //  scriptDoc.UploadedBy = !string.IsNullOrEmpty(scriptSE.CreatedBy) ? scriptSE.CreatedBy : scriptSE.ModifiedBy;
                    scriptDoc.StorageBaseURL = scriptSE.StorageBaseUrl;
                    scriptDoc.CompanyId = scriptSE.BelongsToOrg;
                    scriptDoc.ScriptContainer = "S_" + scriptDoc.CompanyId + "_" + scriptSE.ScriptId;
                }
                return scriptDoc;
            }
        }
    }
}

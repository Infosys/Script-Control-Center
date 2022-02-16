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

namespace Infosys.WEM.WorkflowExecutionLibrary.Translator
{
    public class Workflow_DocumentEntry
    {
        public static Infosys.ATR.RepositoryAccess.Entity.WorkflowDoc WorkflowToDocument(SE.WorkflowMaster workFlowSE, string storageBaseUrl, string companyId)
        {
            Infosys.ATR.RepositoryAccess.Entity.WorkflowDoc workflowDoc = null;
            if (workFlowSE != null)
            {
                workflowDoc = new Infosys.ATR.RepositoryAccess.Entity.WorkflowDoc();
                
                string[] workflowtUrlParts = workFlowSE.WorkflowURI.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                if (workflowtUrlParts != null && workflowtUrlParts.Length > 0)
                {
                    workflowDoc.FileName = workflowtUrlParts[workflowtUrlParts.Length - 1];
                }
                workflowDoc.WorkflowVer = workFlowSE.WorkflowVersion;
            //    workflowDoc.UploadedBy = !string.IsNullOrEmpty(workFlowSE.CreatedBy) ? workFlowSE.CreatedBy : workFlowSE.LastModifiedBy;
                workflowDoc.StorageBaseURL = storageBaseUrl;
                workflowDoc.CompanyId = companyId;
                workflowDoc.WorkflowContainer = workFlowSE.WorkflowID.ToString();
            }
            return workflowDoc;
        }
    }
}

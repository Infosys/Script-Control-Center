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
using SE = Infosys.WEM.Service.Contracts.Data;
using DE = Infosys.WEM.Resource.Entity;
using Infosys.WEM.Service.Contracts.Message;

namespace Infosys.WEM.Service.Implementation.Translators
{
    public static class WorkflowMasterSE_DE
    {
        public static SE.WorkflowMaster WorkflowMasterDEToWorkflowMasterSE(DE.WorkflowMaster source)
        {
            SE.WorkflowMaster destination = null;
            if (source != null)
            {
                destination = new SE.WorkflowMaster();
                destination.CategoryID = source.CategoryId;
                //destination.WorkflowID =new Guid(source.Id);
                destination.WorkflowID = source.Id;
                destination.WorkflowURI = source.WorkflowURI;
                destination.WorkflowVersion = source.WorkflowVer;
                destination.FileSize = source.FileSizeKB.Value;
                destination.ImageURI = source.ImageURI;
                destination.IsActive = source.IsActive.Value;
                destination.Name = source.Name;
              //  destination.SubCategoryID = source.SubCategoryId.GetValueOrDefault();
                if (!string.IsNullOrEmpty(source.Client))
                {
                    destination.ClientId = source.Client;
                }
                if (!string.IsNullOrEmpty(source.ClientVersion))
                {
                    destination.ClientVer = source.ClientVersion;
                }

                if (!string.IsNullOrEmpty(source.Description))
                {
                    destination.Description = source.Description;
                }

                          
                if (!string.IsNullOrEmpty(source.SourceIPAddress))
                {
                    destination.SrcIPAddr = source.SourceIPAddress;
                }
                if (!string.IsNullOrEmpty(source.SourceMachineName))
                {
                    destination.SrcMachineName = source.SourceMachineName;
                }
                destination.UsesUIAutomation = source.UsesUIAutomation.GetValueOrDefault();
                destination.LastModifiedBy = source.LastModifiedBy;
                destination.CreatedBy = source.CreatedBy;
                destination.LastModifiedOn = source.LastModifiedOn;
                destination.PublishedOn = source.PublishedOn;                
                destination.IslongRunningWorkflow = source.IslongRunningWorkflow;
                destination.IdleStateTimeout = source.IdleStateTimeout;
                destination.Tags = source.Tags;
                destination.LicenseType = source.LicenseType;
                destination.SourceUrl = source.SourceUrl;
            }
            return destination;
        }

        public static DE.WorkflowMaster WorkflowMasterSEToWorkflowMasterDE(GetWorkflowReqMsg source)
        {
            DE.WorkflowMaster destination = null;

            if (source != null)
            {
                destination = new DE.WorkflowMaster();
                destination.CategoryId = source.CategoryId;
               // destination.SubCategoryId = source.SubCategoryId;
            }

            return destination;
        }

        internal static List<SE.WorkflowMaster> WorkflowMasterDEToWorkflowMasterSE(List<DE.WorkflowMaster> workflows)
        {
            List<SE.WorkflowMaster> workflowDetails = new List<SE.WorkflowMaster>();
            
            workflows.ForEach(wf =>
            {
                workflowDetails.Add(WorkflowMasterDEToWorkflowMasterSE(wf));
            });

            return workflowDetails;
        }
    }
}

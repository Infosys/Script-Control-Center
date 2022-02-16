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
using BE = Infosys.WEM.Business.Entity;
using SE = Infosys.WEM.Service.Contracts.Data;
using SEMsg = Infosys.WEM.Service.Contracts.Message;

namespace Infosys.WEM.Service.Implementation.Translators
{
    public static class AutomationWorkflowSEToManageWorkflowBE
    {
        public static SEMsg.GetWorkflowDetailsResMsg WorkflowMasterBEToGetWorkflowDetailsResSE(BE.WorkflowMaster source)
        {
            SEMsg.GetWorkflowDetailsResMsg destination = null;

            if (source != null)
            {
                destination = new SEMsg.GetWorkflowDetailsResMsg();
                SE.WorkflowMaster destWM = new SE.WorkflowMaster();
                destWM = Translators.WorkflowMasterSE_BE.WorkflowMasterBEToWorkflowMasterSE(source);
                destination.WorkflowDetails = destWM;
            }
            return destination;

        }

        public static BE.WorkflowMaster PublishReqSEToWorkflowMasterBE(SEMsg.PublishReqMsg source)
        {
            BE.WorkflowMaster destination = null;

            if (source != null)
            {
                destination = new BE.WorkflowMaster();

                destination.CategoryID = source.CategoryID;
            //    destination.SubCategoryID = source.SubCategoryID;
                destination.WorkflowID = source.WorkflowID;
                destination.WorkflowVersion = source.WorkflowVer;
                destination.Name = source.Name;

                if (source.WorkflowVer.Equals(0))
                {
                    destination.CreatedBy = Utility.GetLoggedInUser();
                    destination.PublishedOn = DateTime.UtcNow;
                }

                destination.FileSize = source.FileSizeInKb;
                destination.ImageURI = source.ImageURI;
                destination.WorkflowURI = source.WorkflowURI;
                destination.IncrementVersion = source.IncrementVersion;
                if (!string.IsNullOrEmpty(source.ClientId))
                {
                    destination.ClientId = source.ClientId;
                }
                if (!string.IsNullOrEmpty(source.ClientVer))
                {
                    destination.ClientVer = source.ClientVer;
                }
                if (!string.IsNullOrEmpty(source.Description))
                {
                    destination.Description = source.Description;
                }
                //destination.IsActive = source.IS;
                if (!string.IsNullOrEmpty(source.Description))
                {
                    destination.SrcIPAddr = source.SrcIPAddr;
                }
                if (!string.IsNullOrEmpty(source.SrcMachineName))
                {
                    destination.SrcMachineName = source.SrcMachineName;
                }

                destination.LastModifiedBy = Utility.GetLoggedInUser();
               // if (source.LastModifiedOn != null)
                    destination.LastModifiedOn = DateTime.UtcNow;

                destination.UsesUIAutomation = source.UsesUIAutomation;
                destination.IslongRunningWorkflow = source.IslongRunningWorkflow;
                destination.IdleStateTimeout = source.IdleStateTimeout;
                destination.Tags = source.Tags;
                destination.LicenseType = source.LicenseType;
                destination.SourceUrl = source.SourceUrl;                
            }
            return destination;
        }
        public static SEMsg.PublishResMsg WorkflowMasterBEToPublishReqSE(BE.WorkflowMaster source)
        {
            SEMsg.PublishResMsg destination = null;

            if (source != null)
            {
                destination = new SEMsg.PublishResMsg();
                SE.WorkflowMaster destWM = new SE.WorkflowMaster();
                destWM = Translators.WorkflowMasterSE_BE.WorkflowMasterBEToWorkflowMasterSE(source);
                destination.WorkflowDetails = destWM;
            }
            return destination;
        }

    }
}

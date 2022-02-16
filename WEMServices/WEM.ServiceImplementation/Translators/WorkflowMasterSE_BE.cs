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



namespace Infosys.WEM.Service.Implementation.Translators
{
    public static class WorkflowMasterSE_BE
    {
        public static SE.WorkflowMaster WorkflowMasterBEToWorkflowMasterSE(BE.WorkflowMaster source)
        {
            SE.WorkflowMaster destination = null;
            if (source != null)
            {
                destination = new SE.WorkflowMaster();
                destination.CategoryID = source.CategoryID;
                destination.WorkflowID = source.WorkflowID;
                destination.WorkflowURI = source.WorkflowURI;
                destination.WorkflowVersion = source.WorkflowVersion;
                destination.FileSize = source.FileSize;
                destination.ImageURI = source.ImageURI;
                destination.IsActive = source.IsActive;
                destination.Name = source.Name;
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



                if (!string.IsNullOrEmpty(source.SrcIPAddr))
                {
                    destination.SrcIPAddr = source.SrcIPAddr;
                }
                if (!string.IsNullOrEmpty(source.SrcMachineName))
                {
                    destination.SrcMachineName = source.SrcMachineName;
                }
                destination.UsesUIAutomation = source.UsesUIAutomation;
                destination.IslongRunningWorkflow = source.IslongRunningWorkflow;
                destination.IdleStateTimeout = source.IdleStateTimeout;
                destination.Tags = source.Tags;
                destination.LicenseType = source.LicenseType;
                destination.SourceUrl = source.SourceUrl;
            }
            return destination;
        }

        public static BE.WorkflowMaster WorkflowMasterSEToWorkflowMasterBE(SE.WorkflowMaster source)
        {
            BE.WorkflowMaster destination = null;
            if (source != null)
            {
                destination = new BE.WorkflowMaster();
                destination.CategoryID = source.CategoryID;
                destination.WorkflowID = source.WorkflowID;
                destination.WorkflowURI = source.WorkflowURI;
                destination.WorkflowVersion = source.WorkflowVersion;
                destination.FileSize = source.FileSize;
                destination.ImageURI = source.ImageURI;
                destination.IsActive = source.IsActive;
                destination.Name = source.Name;
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

                //if (!string.IsNullOrEmpty(source.LastModifiedBy))
                {
                    destination.LastModifiedBy = Utility.GetLoggedInUser();
                }

                if (!string.IsNullOrEmpty(source.SrcIPAddr))
                {
                    destination.SrcIPAddr = source.SrcIPAddr;
                }
                if (!string.IsNullOrEmpty(source.SrcMachineName))
                {
                    destination.SrcMachineName = source.SrcMachineName;
                }

                destination.UsesUIAutomation = source.UsesUIAutomation;
                destination.IslongRunningWorkflow = source.IslongRunningWorkflow;
                destination.IdleStateTimeout = source.IdleStateTimeout;
                destination.Tags = source.Tags;
                destination.LicenseType = source.LicenseType;
                destination.SourceUrl = source.SourceUrl;
            }
            return destination;
        }

    }
}

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
using BE = Infosys.WEM.Business.Entity;
using DE = Infosys.WEM.Resource.Entity;

namespace Infosys.WEM.Business.Component.Translators
{
    public static class WorkflowMasterBE_DE
    {
        public static DE.WorkflowMaster WorkflowMasterBEToDE(BE.WorkflowMaster source)
        {
            DE.WorkflowMaster destination = null;
            if (source != null)
            {
                destination = new DE.WorkflowMaster();
                destination.CategoryId = source.CategoryID;
                //  destination.SubCategoryId = source.SubCategoryID;
                destination.Id =source.WorkflowID;
                destination.WorkflowURI = source.WorkflowURI;
                destination.WorkflowVer = source.WorkflowVersion;
                destination.CreatedBy = source.CreatedBy;
                destination.FileSizeKB = source.FileSize;
                destination.ImageURI = source.ImageURI;
                destination.IsActive = source.IsActive;
                destination.Name = source.Name;
                destination.PublishedOn = source.PublishedOn;
                destination.CreatedBy = source.CreatedBy;
                if (source.LastModifiedOn != null)
                {
                    destination.LastModifiedOn = source.LastModifiedOn;
                }
                if (!string.IsNullOrEmpty(source.ClientId))
                {
                    destination.Client = source.ClientId;
                }
                if (!string.IsNullOrEmpty(source.ClientVer))
                {
                    destination.ClientVersion = source.ClientVer;
                }

                if (!string.IsNullOrEmpty(source.Description))
                {
                    destination.Description = source.Description;
                }

                if (!string.IsNullOrEmpty(source.LastModifiedBy))
                {
                    destination.LastModifiedBy = source.LastModifiedBy;
                }

                if (!string.IsNullOrEmpty(source.SrcIPAddr))
                {
                    destination.SourceIPAddress = source.SrcIPAddr;
                }
                if (!string.IsNullOrEmpty(source.SrcMachineName))
                {
                    destination.SourceMachineName = source.SrcMachineName;
                }
                destination.UsesUIAutomation = source.UsesUIAutomation;
                destination.Tags = source.Tags;
                destination.LicenseType = source.LicenseType;
                destination.SourceUrl = source.SourceUrl;
                destination.IslongRunningWorkflow = source.IslongRunningWorkflow;
                destination.IdleStateTimeout = source.IdleStateTimeout;
            }
            return destination;
        }

        public static BE.WorkflowMaster WorkflowMasterDEToBE(DE.WorkflowMaster source)
        {
            BE.WorkflowMaster destination = null;
            if (source != null)
            {
                destination = new BE.WorkflowMaster();
                destination.CategoryID = source.CategoryId;
                //destination.SubCategoryID = source.SubCategoryId.Value;
               // destination.WorkflowID = new Guid(source.Id);
                destination.WorkflowID = source.Id;
                destination.WorkflowURI = source.WorkflowURI;
                destination.WorkflowVersion = source.WorkflowVer;
                destination.CreatedBy = source.CreatedBy;
                destination.FileSize = source.FileSizeKB.Value;
                destination.ImageURI = source.ImageURI;
                destination.IsActive = source.IsActive.Value;
                destination.Name = source.Name;
                destination.PublishedOn = source.PublishedOn.Value;
                if (destination.LastModifiedOn != null)
                {
                    destination.LastModifiedOn = source.LastModifiedOn;
                }
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

                if (!string.IsNullOrEmpty(source.LastModifiedBy))
                {
                    destination.LastModifiedBy = source.LastModifiedBy;
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
                destination.IslongRunningWorkflow = source.IslongRunningWorkflow;
                destination.IdleStateTimeout = source.IdleStateTimeout;
                destination.Tags = source.Tags;
                destination.LicenseType = source.LicenseType;
                destination.SourceUrl = source.SourceUrl;

            }
            return destination;
        }

        public static System.Collections.Generic.List<BE.WorkflowMaster> WorkflowMasterListDEToBE(System.Collections.Generic.List<DE.WorkflowMaster> source)
        {
            System.Collections.Generic.List<BE.WorkflowMaster> destination = new System.Collections.Generic.List<BE.WorkflowMaster>();
            foreach (DE.WorkflowMaster item in source)
            {
                destination.Add(WorkflowMasterDEToBE(item));
            }
            return destination;

        }
        public static System.Collections.Generic.List<DE.WorkflowMaster> WorkflowMasterListBEToDE(System.Collections.Generic.List<BE.WorkflowMaster> source)
        {
            System.Collections.Generic.List<DE.WorkflowMaster> destination = new System.Collections.Generic.List<DE.WorkflowMaster>();
            foreach (BE.WorkflowMaster item in source)
            {
                destination.Add(WorkflowMasterBEToDE(item));
            }
            return destination;

        }
    }
}

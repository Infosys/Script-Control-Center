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
using Infosys.WEM.Service.Contracts.Message;
using DE = Infosys.WEM.Resource.Entity;

namespace Infosys.WEM.Service.Implementation.Translators
{
    public static class AutomationWorkflowSEToWorkflowExecutionStoreDE
    {
        public static DE.WorkflowMaster PublishReqSEToWorkflowMasterDE(PublishReqMsg source, bool isActive)
        {
            DE.WorkflowMaster destination = null;

            if (source != null)
            {
                destination = new DE.WorkflowMaster();

                destination.CategoryId = source.CategoryID;
                //destination.Id =Convert.ToString(source.WorkflowID);
                destination.Id = source.WorkflowID;
                destination.WorkflowVer = source.WorkflowVer; 
                destination.Name = source.Name;
                destination.CreatedBy =Utility.GetLoggedInUser();
                destination.FileSizeKB = source.FileSizeInKb;
                destination.ImageURI = source.ImageURI;
                destination.WorkflowURI = source.WorkflowURI;
                destination.IslongRunningWorkflow = source.IslongRunningWorkflow;
                destination.IdleStateTimeout = source.IdleStateTimeout;
                
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
                destination.IsActive = isActive;
                if (!string.IsNullOrEmpty(source.SrcIPAddr))
                {
                    destination.SourceIPAddress = source.SrcIPAddr;
                }                
            }
            return destination;
        }

        public static PublishResMsg WorkflowMasterDEToPublishResSE(DE.WorkflowMaster source)
        {
            PublishResMsg destination = null;

            if (source != null)
            {
                destination = new PublishResMsg();

                SE.WorkflowMaster destWM = new SE.WorkflowMaster();
                destWM = Translators.WorkflowMasterSE_DE.WorkflowMasterDEToWorkflowMasterSE(source);
                destination.WorkflowDetails = destWM;
            }
            return destination;
        }

        public static DE.WorkflowMaster GetWorkflowDetailsReqSEToWorkflowMasterDE(GetWorkflowDetailsReqMsg source)
        {
            DE.WorkflowMaster destination = null;

            if (source != null)
            {
                destination = new DE.WorkflowMaster();

                destination.CategoryId = source.WorkflowIdentifier.CategoryID;
                //destination.Id =Convert.ToString(source.WorkflowIdentifier.WorkflowID);
                destination.Id = source.WorkflowIdentifier.WorkflowID;
                destination.WorkflowVer = source.WorkflowIdentifier.WorkflowVer;

            }
            return destination;
        }

        public static GetWorkflowDetailsResMsg WorkflowMasterDEToGetWorkflowDetailsResSE(DE.WorkflowMaster source)
        {
            GetWorkflowDetailsResMsg destination = null;

            if (source != null)
            {
                destination = new GetWorkflowDetailsResMsg();

                SE.WorkflowMaster destWM = new SE.WorkflowMaster();
                destWM = Translators.WorkflowMasterSE_DE.WorkflowMasterDEToWorkflowMasterSE(source);
                destination.WorkflowDetails = destWM;
            }
            return destination;
        }

        public static DE.WorkflowCategoryMaster GetAllActiveWorkflowCategoriesReqSEToWorkflowCategoryMasterDE(GetAllActiveWorkflowCategoriesReqMsg source)
        {
            DE.WorkflowCategoryMaster destination = null;

            if (source != null)
            {
                destination = new DE.WorkflowCategoryMaster();

                destination.CompanyId = source.CompanyID;

            }
            return destination;
        }

        public static GetAllActiveWorkflowCategoriesResMsg WorkflowCategoryMasterDEListToGetAllActiveWorkflowCategoriesResSE(System.Collections.Generic.IList<DE.WorkflowCategoryMaster> source)
        {
            GetAllActiveWorkflowCategoriesResMsg destination = new GetAllActiveWorkflowCategoriesResMsg();

            System.Collections.Generic.IList<SE.WorkflowCategoryMaster> destWCM = new List<SE.WorkflowCategoryMaster>();
            
            foreach (DE.WorkflowCategoryMaster item in source)
            {
                destWCM.Add(Translators.WorkflowCategoryMasterSE_DE.WorkflowCategoryMasterDEToWorkflowCategorySE(item));
            }
            destination.CategoryDetails = destWCM;
            return destination;

        }

        public static DE.CategoryWorkflowMap GetAllActiveWorkflowsByCategoryReqSEToCategoryWorkflowMapDE(GetAllActiveWorkflowsByCategoryReqMsg source)
        {
            DE.CategoryWorkflowMap destination = null;

            if (source != null)
            {
                destination = new DE.CategoryWorkflowMap();

                destination.CategoryId = source.CategoryID;

            }
            return destination;
        }

        //public static GetAllActiveWorkflowsByCategoryResMsg CategoryWorkflowMapDEListToGetAllActiveWorkflowsByCategoryResSE(System.Collections.Generic.IList<DE.CategoryWorkflowMap> source)
        //{
        //    GetAllActiveWorkflowsByCategoryResMsg destination = new GetAllActiveWorkflowsByCategoryResMsg();

        //    System.Collections.Generic.IList<SE.CategoryWorkflowMap> destCWM = new List<SE.CategoryWorkflowMap>();

        //    foreach (DE.CategoryWorkflowMap item in source)
        //    {
        //        destCWM.Add(Translators.CategoryWorkflowMapSE_DE.CategoryWorkflowMapDEToSE(item));
        //    }
        //    destination.CategoryWorkflowMapping = new List<SE.CategoryWorkflowMap>();
        //    destination.CategoryWorkflowMapping = destCWM;
        //    return destination;

        //}

        

    }
}

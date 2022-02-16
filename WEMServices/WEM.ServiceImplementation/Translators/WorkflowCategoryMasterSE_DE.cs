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
using DE = Infosys.WEM.Resource.Entity;

namespace Infosys.WEM.Service.Implementation.Translators
{
    public static class WorkflowCategoryMasterSE_DE
    {
        public static SE.WorkflowCategoryMaster WorkflowCategoryMasterDEToWorkflowCategorySE(DE.WorkflowCategoryMaster source)
        {
            SE.WorkflowCategoryMaster destination = null;
            if (source != null)
            {
                destination = new SE.WorkflowCategoryMaster();
                destination.CategoryID = source.Id;
                destination.CompanyID = source.CompanyId;
                destination.Description = source.Description;
                destination.IsActive = source.IsActive.Value;
                destination.Name = source.Name;
                destination.ParentId = source.ParentId.GetValueOrDefault();                
            }
            return destination;
        }
        public static System.Collections.Generic.List<SE.WorkflowCategoryMaster> TranslateActivityTimelineListDEToBE(System.Collections.Generic.List<DE.WorkflowCategoryMaster> source)
        {
            System.Collections.Generic.List<SE.WorkflowCategoryMaster> destination = new System.Collections.Generic.List<SE.WorkflowCategoryMaster>();
            foreach (DE.WorkflowCategoryMaster item in source)
            {
                destination.Add(WorkflowCategoryMasterDEToWorkflowCategorySE(item));
            }
            return destination;

        }

        public static DE.WorkflowCategoryMaster WorkflowCategoryMasterSEToWorkflowCategoryDE(SE.WorkflowCategoryMaster source)
        {
            DE.WorkflowCategoryMaster destination = null;
            if (source != null)
            {
                destination = new DE.WorkflowCategoryMaster();
                //destination.Id = source.CategoryID;
                destination.CompanyId = source.CompanyID;
              //  destination.IsActive = source.IsActive.Value;                
                destination.Name = source.Name;
                destination.Description = source.Description;
                destination.ParentId = source.ParentId;
            }
            return destination;
        }
    }
}

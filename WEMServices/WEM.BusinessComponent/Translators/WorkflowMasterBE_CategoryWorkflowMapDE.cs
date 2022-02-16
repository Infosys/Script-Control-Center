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
    public class WorkflowMasterBE_CategoryWorkflowMapDE
    {
        public static DE.CategoryWorkflowMap WorkflowMasterBEToCategoryWorkflowMapDE(BE.WorkflowMaster source)
        {
            DE.CategoryWorkflowMap destination = null;
            if (source != null)
            {
                destination = new DE.CategoryWorkflowMap();
                destination.CategoryId = source.CategoryID;
                destination.WorkflowId = source.WorkflowID;
                destination.WorkflowName = source.Name;
                destination.ActiveWorkflowVersion = source.WorkflowVersion;
                

            }
            return destination;
        }


        public static System.Collections.Generic.List<DE.CategoryWorkflowMap> WorkflowMasterListBEToCategoryWorkflowMapDE(System.Collections.Generic.List<BE.WorkflowMaster> source)
        {
            System.Collections.Generic.List<DE.CategoryWorkflowMap> destination = new System.Collections.Generic.List<DE.CategoryWorkflowMap>();
            foreach (BE.WorkflowMaster item in source)
            {
                destination.Add(WorkflowMasterBEToCategoryWorkflowMapDE(item));
            }
            return destination;

        }
    }
}

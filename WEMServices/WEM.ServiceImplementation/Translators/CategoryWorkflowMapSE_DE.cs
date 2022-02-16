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
    public class CategoryWorkflowMapSE_DE
    {
        public static DE.CategoryWorkflowMap CategoryWorkflowMapSEToDE(SE.CategoryWorkflowMap source)
        {
            DE.CategoryWorkflowMap destination = null;
            if (source != null)
            {
                destination = new DE.CategoryWorkflowMap();
                destination.CategoryId = source.CategoryID;
                //destination.WorkflowId =Convert.ToString(source.WorkflowID);
                destination.WorkflowId = source.WorkflowID;
                destination.WorkflowName = source.Name;
                destination.ActiveWorkflowVersion = source.WorkflowVersion;

            }
            return destination;
        }

        public static SE.CategoryWorkflowMap CategoryWorkflowMapDEToSE(DE.CategoryWorkflowMap source)
        {
            SE.CategoryWorkflowMap destination = null;
            if (source != null)
            {
                destination = new SE.CategoryWorkflowMap();
                destination.CategoryID = source.CategoryId;
                //destination.WorkflowID =new Guid(source.WorkflowId);
                destination.WorkflowID = source.WorkflowId;
                destination.Name = source.WorkflowName;
                destination.WorkflowVersion = source.ActiveWorkflowVersion;                
            }
            return destination;
        }

        public static System.Collections.Generic.List<SE.CategoryWorkflowMap> WorkflowMasterListDEToSE(System.Collections.Generic.List<DE.CategoryWorkflowMap> source)
        {
            System.Collections.Generic.List<SE.CategoryWorkflowMap> destination = new System.Collections.Generic.List<SE.CategoryWorkflowMap>();
            foreach (DE.CategoryWorkflowMap item in source)
            {
                destination.Add(CategoryWorkflowMapDEToSE(item));
            }
            return destination;

        }
        public static System.Collections.Generic.List<DE.CategoryWorkflowMap> WorkflowMasterListSEToDE(System.Collections.Generic.List<SE.CategoryWorkflowMap> source)
        {
            System.Collections.Generic.List<DE.CategoryWorkflowMap> destination = new System.Collections.Generic.List<DE.CategoryWorkflowMap>();
            foreach (SE.CategoryWorkflowMap item in source)
            {
                destination.Add(CategoryWorkflowMapSEToDE(item));
            }
            return destination;

        }   
    }
}

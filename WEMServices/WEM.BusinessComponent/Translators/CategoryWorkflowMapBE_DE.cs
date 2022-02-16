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
    public class CategoryWorkflowMapBE_DE
    {
        public static DE.CategoryWorkflowMap CategoryWorkflowMapBEToDE(BE.CategoryWorkflowMap source)
        {
            DE.CategoryWorkflowMap destination = null;
            if (source != null)
            {
                destination = new DE.CategoryWorkflowMap();
                destination.CategoryId = source.CategoryID;
                destination.WorkflowId = source.WorkflowID;
                destination.WorkflowName = source.WorkflowName;
                destination.ActiveWorkflowVersion = source.ActiveWorkflowVersion;

            }
            return destination;
        }

        public static BE.CategoryWorkflowMap CategoryWorkflowMapDEToBE(DE.CategoryWorkflowMap source)
        {
            BE.CategoryWorkflowMap destination = null;
            if (source != null)
            {
                destination = new BE.CategoryWorkflowMap();
                destination.CategoryID = source.CategoryId;
                //destination.WorkflowID = new Guid(source.WorkflowId);
                destination.WorkflowID = source.WorkflowId;
                destination.WorkflowName = source.WorkflowName;
                destination.ActiveWorkflowVersion = source.ActiveWorkflowVersion;
            }
            return destination;
        }

        public static System.Collections.Generic.List<BE.CategoryWorkflowMap> WorkflowMasterListDEToBE(System.Collections.Generic.List<DE.CategoryWorkflowMap> source)
        {
            System.Collections.Generic.List<BE.CategoryWorkflowMap> destination = new System.Collections.Generic.List<BE.CategoryWorkflowMap>();
            foreach (DE.CategoryWorkflowMap item in source)
            {
                destination.Add(CategoryWorkflowMapDEToBE(item));
            }
            return destination;

        }
        public static System.Collections.Generic.List<DE.CategoryWorkflowMap> WorkflowMasterListBEToDE(System.Collections.Generic.List<BE.CategoryWorkflowMap> source)
        {
            System.Collections.Generic.List<DE.CategoryWorkflowMap> destination = new System.Collections.Generic.List<DE.CategoryWorkflowMap>();
            foreach (BE.CategoryWorkflowMap item in source)
            {
                destination.Add(CategoryWorkflowMapBEToDE(item));
            }
            return destination;

        }   
    }
}

/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using Infosys.WEM.Resource.DataAccess;
using BE = Infosys.WEM.Business.Entity;
using DE = Infosys.WEM.Resource.Entity;

namespace Infosys.WEM.Business.Component
{
    public class ManageWorkflows
    {
        public BE.WorkflowMaster GetLatestActiveVersion(int categoryId, Guid workflowId, int workflowVersion)
        {
            WorkflowMasterDS workflowMasterDA = new WorkflowMasterDS();
            DE.WorkflowMaster workflowMasterDE = new DE.WorkflowMaster();
            workflowMasterDE.CategoryId = categoryId;
            workflowMasterDE.Id =workflowId;
            workflowMasterDE.WorkflowVer = workflowVersion;
            workflowMasterDE.PartitionKey = WorkflowMasterKeysExtension.FormPartitionKey(categoryId);
            workflowMasterDE = workflowMasterDA.GetLatestActiveVersion(workflowMasterDE);
            BE.WorkflowMaster workflowMasterBE = Translators.WorkflowMasterBE_DE.WorkflowMasterDEToBE(workflowMasterDE);
            return workflowMasterBE;
        }


        public BE.WorkflowMaster GetLatestActiveVersion(Guid workflowId, int workflowVersion)
        {

            WorkflowMasterDS workflowMasterDA = new WorkflowMasterDS();

            // string partitionKey = WorkflowMasterKeysExtension.FormPartitionKey(categoryId);
            string rowKey = WorkflowMasterKeysExtension.FormRowKey(workflowId, workflowVersion);

            //Retrieve All Workflow Entries by CategoryId and WorkflowId

            IQueryable<DE.WorkflowMaster> workflowMasterDE = (IQueryable<DE.WorkflowMaster>)workflowMasterDA.GetAny().
            Where(workflow => (workflow.RowKey.Contains(rowKey)));

            //Retrieve only active workflows
            List<BE.WorkflowMaster> workflowMasterBE = Translators.WorkflowMasterBE_DE.WorkflowMasterListDEToBE(
            (workflowMasterDE.
                Where(workflow => (workflow.IsActive == true)).
                OrderByDescending(workflow => workflow.WorkflowVer).
                ToList<DE.WorkflowMaster>())
                );

            if (workflowMasterBE.Count <= 1)
            {
                if (workflowMasterBE != null && workflowMasterBE.Count > 0)
                {
                    return workflowMasterBE[0];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return workflowMasterBE[0];
                //TODO: Log error on dsicrepancy of duplicate versions found
            }
        }

        public BE.WorkflowMaster Updateworkflow(BE.WorkflowMaster source)
        {
            BE.WorkflowMaster workflowMasterBE = null;
            CategoryWorkflowMapDS catWMMapDS = new CategoryWorkflowMapDS();
            #region Add workflow details to workflowmaster
            if (source.WorkflowID != null)
            {
                workflowMasterBE = GetLatestActiveVersion(source.WorkflowID, source.WorkflowVersion);

                if (workflowMasterBE != null)
                {
                    if (workflowMasterBE.WorkflowVersion != source.WorkflowVersion)
                    {
                        source.WorkflowVersion = workflowMasterBE.WorkflowVersion;
                    }
                    //Increment version
                    if (source.IncrementVersion)
                        source.WorkflowVersion = source.WorkflowVersion + 1;
                }
                //source.PublishedOn = DateTime.Now;
                source.IsActive = true; //This is assumed that the images and xaml have been successfully uploaded prior to publish            
                WorkflowMasterDS workflowMasterDS = new WorkflowMasterDS();

            #endregion

            #region Associate CategoryWorkflowMap

                //Create new workflow entry in DB               
                BE.WorkflowMaster newWorkflowEntryBE = (
                    Translators.WorkflowMasterBE_DE.WorkflowMasterDEToBE(
                    workflowMasterDS.Insert(
                    Translators.WorkflowMasterBE_DE.WorkflowMasterBEToDE(source)
                        )
                        ));
                catWMMapDS.Update(
                        Translators.WorkflowMasterBE_CategoryWorkflowMapDE.WorkflowMasterBEToCategoryWorkflowMapDE(newWorkflowEntryBE));


                #endregion

            #region De-activate old version if any pervious version of the workflow exists
                if (workflowMasterBE != null)
                {
                    workflowMasterBE.IsActive = false;
                    workflowMasterDS.Update(Translators.WorkflowMasterBE_DE.WorkflowMasterBEToDE(workflowMasterBE));
                }
            }
                #endregion

            return workflowMasterBE;
        }

        public BE.WorkflowMaster PublishWorkflow(BE.WorkflowMaster source)
        {
            //flag to indicate if the workflow isbeing published for the first as opposed to being an update
            bool isNewWorkflow = false;
            BE.WorkflowMaster workflowMasterBE = null;
            CategoryWorkflowMapDS catWMMapDS = new CategoryWorkflowMapDS();
            #region Add workflow details to workflowmaster
            if (source.WorkflowID != null)
            {
                workflowMasterBE = GetLatestActiveVersion(source.CategoryID, source.WorkflowID, source.WorkflowVersion);

                if (workflowMasterBE != null)
                {
                    if (workflowMasterBE.WorkflowVersion != source.WorkflowVersion)
                    {
                        source.WorkflowVersion = workflowMasterBE.WorkflowVersion;
                    }
                    //Increment version
                    // source.WorkflowVersion = source.WorkflowVersion + 1;
                    isNewWorkflow = false;
                }
                else
                {
                    Guid newWorkflowId = Guid.NewGuid();
                    //  source.WorkflowID = newWorkflowId;
                    if (source.IncrementVersion)
                        source.WorkflowVersion = source.WorkflowVersion + 1;
                    isNewWorkflow = true;
                }

            }
            else if (source.WorkflowID == null)
            {

                //Assign new unique workflowId
                Guid newWorkflowId = Guid.NewGuid();
                source.WorkflowID = newWorkflowId;
                source.WorkflowVersion = 1;
                isNewWorkflow = true;
            }

            source.IsActive = true; //This is assumed that the images and xaml have been successfully uploaded prior to publish
            //source.PublishedOn = DateTime.Now;
            WorkflowMasterDS workflowMasterDS = new WorkflowMasterDS();

            #endregion

            #region Associate CategoryWorkflowMap

            if (isNewWorkflow)
            {
                var de = Translators.WorkflowMasterBE_DE.WorkflowMasterBEToDE(source);
                //Create new workflow entry in DB
                BE.WorkflowMaster newWorkflowEntryBE = (
                    Translators.WorkflowMasterBE_DE.WorkflowMasterDEToBE(
                    workflowMasterDS.Insert(
                    de
                        )
                        ));
                catWMMapDS.Insert(
                    Translators.WorkflowMasterBE_CategoryWorkflowMapDE.WorkflowMasterBEToCategoryWorkflowMapDE(newWorkflowEntryBE));

                DeactivateOldWF(de);

                return newWorkflowEntryBE;

            }
            else
            {
                //Create new workflow entry in DB
                BE.WorkflowMaster newWorkflowEntryBE = (
                    Translators.WorkflowMasterBE_DE.WorkflowMasterDEToBE(
                    workflowMasterDS.Update(
                    Translators.WorkflowMasterBE_DE.WorkflowMasterBEToDE(source)
                        )
                        ));
                catWMMapDS.Update(
                        Translators.WorkflowMasterBE_CategoryWorkflowMapDE.WorkflowMasterBEToCategoryWorkflowMapDE(newWorkflowEntryBE));
                return newWorkflowEntryBE;
            }

            #endregion

            #region De-activate old version if any pervious version of the workflow exists
            //if (workflowMasterBE != null)
            //{                
            //    workflowMasterBE.IsActive = false;
            //    workflowMasterDS.Update(Translators.WorkflowMasterBE_DE.WorkflowMasterBEToDE(workflowMasterBE));
            //}
            #endregion
            // return workflowMasterBE;
        }

        private void DeactivateOldWF(DE.WorkflowMaster de)
        {
            WorkflowMasterDS wfds = new WorkflowMasterDS();
            var wfAll = wfds.GetAll();
            var wf = wfAll.FirstOrDefault(w => w.CategoryId != de.CategoryId && w.Id == de.Id && w.IsActive == true);

            if (wf != null)
            {
                var bewf = Translators.WorkflowMasterBE_DE.WorkflowMasterDEToBE(wf);
                bewf.IsActive = false;
                var dewf = Translators.WorkflowMasterBE_DE.WorkflowMasterListBEToDE(new List<BE.WorkflowMaster> { bewf });
                wfds.Update(dewf[0]);
            }
        }

        public bool Delete(BE.WorkflowMaster source)
        {
            WorkflowMasterDS wf = new WorkflowMasterDS();
            //wf.Delete(new DE.WorkflowMaster { Id =Convert.ToString(source.WorkflowID), CategoryId = source.CategoryID, WorkflowVer = source.WorkflowVersion });
            wf.Delete(new DE.WorkflowMaster { Id = source.WorkflowID, CategoryId = source.CategoryID, WorkflowVer = source.WorkflowVersion });
            return true;
        }

    }

    public class ManageCategory
    {
        public DE.WorkflowCategoryMaster Add(DE.WorkflowCategoryMaster entity)
        {
            WorkflowCategoryMasterDS categoryMaster = new WorkflowCategoryMasterDS();
            return categoryMaster.Insert(entity);
        }
    }

}

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
using Infosys.WEM.Resource.Entity;
using Infosys.WEM.Infrastructure.Common;

namespace Infosys.WEM.Resource.DataAccess
{

    /// <summary>
    /// /// <summary>
    /// Class definition to define the CRUD operation on WorkflowMaster data entity class
    /// </summary>
    /// </summary>

    public partial class WorkflowMasterDS : IDataAccess.IEntity<WorkflowMaster>
    {
        public static DataEntity dbCon;

        static WorkflowMasterDS()
        {
            dbCon = new DataEntity();
        }

        /// <summary>
        /// This method is to get one specific record from table
        /// </summary>
        /// <param name="entity">object of WorkflowMaster</param>
        /// <returns>object of WorkflowMaster</returns>
        public WorkflowMaster GetOne(WorkflowMaster entity)
        {
            WorkflowMaster _WorkflowMaster;
            //using (dbCon = new DataEntity())
            {
                entity = WorkflowMasterKeysExtension.GeneratePartitionKeyAndRowKey(entity);
                _WorkflowMaster = dbCon.WorkflowMaster.Where(ent => ent.PartitionKey.Equals(entity.PartitionKey) && ent.RowKey.Equals(entity.RowKey)).FirstOrDefault();
                return _WorkflowMaster;
            }
        }

        /// <summary>
        /// Fetches workflow from database
        /// Fetches workflow based on version if version is provided as input, else fetches latest active version
        /// </summary>
        /// <param name="entity">Workflow entity containing workflow id and version details</param>
        /// <returns>Workflow entity fetched from the database</returns>
        public WorkflowMaster GetLatestActiveVersion(WorkflowMaster entity)
        {
            if (entity.WorkflowVer > 0)
            {
                entity = WorkflowMasterKeysExtension.GeneratePartitionKeyAndRowKey(entity);
                entity = dbCon.WorkflowMaster.Where(ent => ent.PartitionKey.Equals(entity.PartitionKey) && ent.RowKey.Equals(entity.RowKey)).FirstOrDefault();
            }
            else
            {
                string workflowId = entity.Id.ToString();
                entity = dbCon.WorkflowMaster.Where(ent => ent.PartitionKey.Equals(entity.PartitionKey) && ent.RowKey.Contains(workflowId) && ent.IsActive == true).FirstOrDefault();
            }
            return entity;
        }

        /// <summary>
        /// This method is to get all records from table
        /// </summary>
        /// <param >none</param>
        /// <returns>List of WorkflowMaster</returns>
        public IList<WorkflowMaster> GetAll()
        {
            // using (dbCon = new DataEntity())
            {

                //List<WorkflowMaster> workflowMasterRows =
                //  (from WorkflowMastertable in dbCon.WorkflowMaster
                //   select WorkflowMastertable).ToList<WorkflowMaster>();
                //return workflowMasterRows;

                var wf = dbCon.WorkflowMaster.ToList();
                return wf.Where(w => w.IsActive.GetValueOrDefault() == true ).ToList();
            }
        }
        /// <summary>
        /// This method is to insert one record in table
        /// </summary>
        /// <param name="entity">WorkflowMasterobject of WorkflowMaster</param>
        /// <returns>bool</returns>
        public WorkflowMaster Insert(WorkflowMaster entity)
        {
            // using (dbCon = new DataEntity())
            {
                entity.CreatedBy = Utility.GetLoggedInUser();

                if (entity.PublishedOn == null || entity.PublishedOn == DateTime.MinValue)
                    entity.PublishedOn = DateTime.UtcNow;

                entity = WorkflowMasterKeysExtension.GeneratePartitionKeyAndRowKey(entity);
                dbCon.WorkflowMaster.Add(entity);
                dbCon.SaveChanges();
            }

            return entity;
        }
        /// <summary>
        /// This method is to update one record in table
        /// </summary>
        /// <param name="entity">object of WorkflowMaster</param>
        /// <returns>bool</returns>
        public WorkflowMaster Update(WorkflowMaster entity)
        {

            //using (dbCon = new DataEntity())
            {
                entity = WorkflowMasterKeysExtension.GeneratePartitionKeyAndRowKey(entity);
                WorkflowMaster entityItem = dbCon.WorkflowMaster.Single(c => c.PartitionKey == entity.PartitionKey && c.RowKey == entity.RowKey);

                dbCon.WorkflowMaster.Attach(entityItem);

                DateTime? lastModifiedOn = null;

                lastModifiedOn = entityItem.LastModifiedOn;

                if (entity.LastModifiedOn == null)
                    entity.LastModifiedOn = DateTime.UtcNow;
                if (lastModifiedOn == entity.LastModifiedOn)
                    entity.LastModifiedOn = DateTime.UtcNow;

                //entity = WorkflowMasterKeysExtension.GeneratePartitionKeyAndRowKey(entity);
                //dbCon.Entry(entityItem).CurrentValues.SetValues(entity);
                EntityExtension<WorkflowMaster>.ApplyOnlyChanges(entityItem, entity);
                dbCon.SaveChanges();
            }

            return entity;
        }
        /// <summary>
        /// This method is to delete one record from table
        /// </summary>
        /// <param name="entity">Object of WorkflowMaster</param>
        /// <returns>bool</returns>
        public bool Delete(WorkflowMaster entity)
        {
            bool result = false;
            // using (dbCon = new DataEntity())
            {
                entity = WorkflowMasterKeysExtension.GeneratePartitionKeyAndRowKey(entity);
                entity = dbCon.WorkflowMaster.FirstOrDefault(wf => wf.RowKey == entity.RowKey && wf.PartitionKey == entity.PartitionKey);
                entity.IsActive = false;
                dbCon.WorkflowMaster.Attach(entity);
                dbCon.Entry(entity).Property(e => e.IsActive).IsModified = true;
                dbCon.SaveChanges();
            }
            result = true;
            return result;
        }
        /// <summary>
        /// This method is to insert multiple records in table
        /// </summary>
        /// <param name="entities">List of WorkflowMaster entities</param>
        /// <returns>bool</returns>
        public IList<WorkflowMaster> InsertBatch(IList<WorkflowMaster> entities)
        {

            //  using (dbCon = new DataEntity())
            {
                foreach (WorkflowMaster entity in entities)
                {

                    if (entity.PublishedOn == null)
                    {
                        entity.PublishedOn = DateTime.UtcNow;
                    }
                    WorkflowMaster updatedEntity = WorkflowMasterKeysExtension.GeneratePartitionKeyAndRowKey(entity);
                    dbCon.WorkflowMaster.Add(updatedEntity);
                }

                dbCon.SaveChanges();
            }


            return entities;
        }

        /// <summary>
        /// This method is to update multiple records in table
        /// </summary>
        /// <param name="entities">List of WorkflowMaster entities</param>
        /// <returns>bool</returns>
        public IList<WorkflowMaster> UpdateBatch(IList<WorkflowMaster> entities)
        {

            // using (dbCon = new DataEntity())
            {
                foreach (WorkflowMaster entity in entities)
                {
                    WorkflowMaster entityItem = dbCon.WorkflowMaster.Single(c => c.PartitionKey == entity.PartitionKey &&
                   c.RowKey == entity.RowKey);

                    dbCon.WorkflowMaster.Attach(entityItem);

                    DateTime? lastModifiedOn = null;

                    lastModifiedOn = entityItem.LastModifiedOn;

                    if (entity.LastModifiedOn == null)
                    {
                        entity.LastModifiedOn = DateTime.UtcNow;
                    }
                    if (lastModifiedOn == entity.LastModifiedOn)
                    {
                        entity.LastModifiedOn = DateTime.UtcNow;
                    }

                    WorkflowMaster updatedEntity = WorkflowMasterKeysExtension.GeneratePartitionKeyAndRowKey(entity);
                    //dbCon.Entry(entityItem).CurrentValues.SetValues(updatedEntity);
                    EntityExtension<WorkflowMaster>.ApplyOnlyChanges(entityItem, updatedEntity);
                }
                dbCon.SaveChanges();
            }


            return entities;
        }

        /// <summary>
        /// This method is to get an entity
        /// </summary>
        /// <returns>IQueryable<WorkflowMaster></returns>
        public IQueryable<WorkflowMaster> GetAny()
        {
            return dbCon.WorkflowMaster;
        }

        /// <summary>
        /// This method is to get multiple records from table 
        /// based on partitionkey 
        /// </summary>
        /// <param name="entity">WorkflowMaster entities</param>
        /// <returns>List of WorkflowMaster</returns>
        public IList<WorkflowMaster> GetAll(WorkflowMaster entity)
        {
            // using (dbCon = new DataEntity())
            {
                List<WorkflowMaster> workflowMaster = null;
                entity = WorkflowMasterKeysExtension.GeneratePartitionKeyAndRowKey(entity);
                if (!string.IsNullOrEmpty(entity.PartitionKey))// && string.IsNullOrEmpty(entity.RowKey)))
                {

                    workflowMaster = dbCon.WorkflowMaster.Where(ent => ent.PartitionKey.Equals(entity.PartitionKey) && ent.IsActive == true).ToList<WorkflowMaster>();
                }
                return workflowMaster;
            }
        }

        public IList<WorkflowMaster> GetWorkflowsByCategoryId(WorkflowMaster entity)
        {
            List<WorkflowMaster> workflowMaster = null;
            //workflowMaster = dbCon.WorkflowMaster.Where(ent => ent.CategoryId == entity.CategoryId && ent.SubCategoryId==entity.SubCategoryId && ent.IsActive == true).ToList<WorkflowMaster>();
            workflowMaster = dbCon.WorkflowMaster.Where(ent => ent.CategoryId == entity.CategoryId && ent.IsActive == true).ToList<WorkflowMaster>();
            workflowMaster.ForEach(workflow =>
            {
                WorkflowMaster _Workflow = dbCon.WorkflowMaster.Where(ent => ent.CategoryId.Equals(workflow.CategoryId) && ent.Id.Equals(workflow.Id))
                                            .OrderBy(ent => ent.PublishedOn).FirstOrDefault<WorkflowMaster>();

                workflow.CreatedBy = _Workflow.CreatedBy;
                workflow.PublishedOn = _Workflow.PublishedOn;
            });
            return workflowMaster;
        }
    }

    public static class WorkflowMasterKeysExtension
    {
        public static WorkflowMaster GeneratePartitionKey(this WorkflowMaster entity)
        {

            entity.PartitionKey = entity.CategoryId.ToString();

            return entity;

        }

        public static WorkflowMaster GenerateRowKey(this WorkflowMaster entity)
        {
            if (entity.Id != null)
            {
                string rowKey = ApplicationConstants.TWOFIELD_KEY_FORMAT;
                entity.RowKey = string.Format(rowKey, entity.Id.ToString(), entity.WorkflowVer.ToString());
            }
            return entity;

        }

        public static WorkflowMaster GeneratePartitionKeyAndRowKey(this WorkflowMaster entity)
        {
            return entity.GeneratePartitionKey().GenerateRowKey();
        }

        public static string FormPartitionKey(int categoryId)
        {
            string key = "";
            if (categoryId != null)
            {
                key = categoryId.ToString();
            }
            return key;

        }

        public static string FormRowKey(Guid workflowId, int workflowVersion)
        {
            string key = "";
            if (workflowId != null)
            {
                key = string.Format(ApplicationConstants.TWOFIELD_KEY_FORMAT, workflowId.ToString(), (workflowVersion == null || workflowVersion == 0) ? "" : workflowVersion.ToString());
            }
            return key;

        }

        public static bool IsDuplicate(WorkflowMaster entity)
        {
            bool isDuplicate = false;
            using (DataEntity dbCon = new DataEntity())
            {
                var workflows = from Workflow in dbCon.WorkflowMaster
                                where Workflow.CategoryId == entity.CategoryId
                                      && Workflow.IsActive == true
                                select Workflow;

                if (workflows != null)
                {
                    var existingWfs = workflows.ToList();
                    if (existingWfs.Exists(x => x.Name.ToLower().Equals(entity.Name.ToLower())))
                    {
                        List<WorkflowMaster> lstWFMaster = existingWfs.Where(ent => ent.Name.ToLower().Equals(entity.Name.ToLower())).ToList();
                        lstWFMaster.ForEach(ent =>
                        {
                            if (!ent.Id.Equals(entity.Id))
                                isDuplicate = true;
                        });
                    }
                }
            }
            return isDuplicate;
        }
    }
}

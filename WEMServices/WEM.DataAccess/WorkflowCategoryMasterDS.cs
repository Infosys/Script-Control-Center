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

namespace Infosys.WEM.Resource.DataAccess
{

    /// <summary>
    /// /// <summary>
    /// Class definition to define the CRUD operation on WorkflowCategoryMaster data entity class
    /// </summary>
    /// </summary>

    public partial class WorkflowCategoryMasterDS : IDataAccess.IEntity<WorkflowCategoryMaster>
    {
        public DataEntity dbCon;

        /// <summary>
        /// This method is to get one specific record from table
        /// </summary>
        /// <param name="entity">object of WorkflowCategoryMaster</param>
        /// <returns>object of WorkflowCategoryMaster</returns>
        public WorkflowCategoryMaster GetOne(WorkflowCategoryMaster entity)
        {
            WorkflowCategoryMaster _WorkflowCategoryMaster;
            using (dbCon = new DataEntity())
            {
                entity = WorkflowCategoryMasterKeysExtension.GeneratePartitionKeyAndRowKey(entity);
                _WorkflowCategoryMaster = dbCon.WorkflowCategoryMaster.Where(ent => ent.PartitionKey.Equals(entity.PartitionKey) && ent.RowKey.Equals(entity.RowKey)).FirstOrDefault();
                return _WorkflowCategoryMaster;
            }
        }
        /// <summary>
        /// This method is to get all records from table
        /// </summary>
        /// <param >none</param>
        /// <returns>List of WorkflowCategoryMaster</returns>
        public IList<WorkflowCategoryMaster> GetAll()
        {
            using (dbCon = new DataEntity())
            {
                List<WorkflowCategoryMaster> workflowCategoryMasterRows =
                  (from WorkflowCategoryMastertable in dbCon.WorkflowCategoryMaster
                   select WorkflowCategoryMastertable).ToList<WorkflowCategoryMaster>();
                return workflowCategoryMasterRows;
            }
        }
        /// <summary>
        /// This method is to insert one record in table
        /// </summary>
        /// <param name="entity">WorkflowCategoryMasterobject of WorkflowCategoryMaster</param>
        /// <returns>bool</returns>
        public WorkflowCategoryMaster Insert(WorkflowCategoryMaster entity)
        {          

            using (dbCon = new DataEntity())
            {
                var t = this.GetAny();

                int seed =0;

                if (t != null)
                    seed = t.Count();

                if (entity.CreatedOn == null)
                {
                    entity.CreatedOn = DateTime.UtcNow;
                    entity.Id = ++seed;
                    entity.IsActive = true;
                }
                entity = WorkflowCategoryMasterKeysExtension.GeneratePartitionKeyAndRowKey(entity);
                dbCon.WorkflowCategoryMaster.Add(entity);
                dbCon.SaveChanges();
               
            }

            return entity;
        }
        /// <summary>
        /// This method is to update one record in table
        /// </summary>
        /// <param name="entity">object of WorkflowCategoryMaster</param>
        /// <returns>bool</returns>
        public WorkflowCategoryMaster Update(WorkflowCategoryMaster entity)
        {

            using (dbCon = new DataEntity())
            {
                WorkflowCategoryMaster entityItem = dbCon.WorkflowCategoryMaster.Single(c => c.PartitionKey == entity.PartitionKey &&
                    c.RowKey == entity.RowKey);

                dbCon.WorkflowCategoryMaster.Attach(entityItem);

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
                entity = WorkflowCategoryMasterKeysExtension.GeneratePartitionKeyAndRowKey(entity);
                //dbCon.Entry(entityItem).CurrentValues.SetValues(entity);
                EntityExtension<WorkflowCategoryMaster>.ApplyOnlyChanges(entityItem, entity);
                dbCon.SaveChanges();
            }

            return entity;
        }
        /// <summary>
        /// This method is to delete one record from table
        /// </summary>
        /// <param name="entity">Object of WorkflowCategoryMaster</param>
        /// <returns>bool</returns>
        public bool Delete(WorkflowCategoryMaster entity)
        {
            bool result = false;
            using (dbCon = new DataEntity())
            {
                entity = WorkflowCategoryMasterKeysExtension.GeneratePartitionKeyAndRowKey(entity);
                dbCon.WorkflowCategoryMaster.Attach(entity);
                dbCon.WorkflowCategoryMaster.Remove(entity);
                dbCon.SaveChanges();
            }
            result = true;
            return result;
        }
        /// <summary>
        /// This method is to insert multiple records in table
        /// </summary>
        /// <param name="entities">List of WorkflowCategoryMaster entities</param>
        /// <returns>bool</returns>
        public IList<WorkflowCategoryMaster> InsertBatch(IList<WorkflowCategoryMaster> entities)
        {

            using (dbCon = new DataEntity())
            {
                foreach (WorkflowCategoryMaster entity in entities)
                {

                    if (entity.CreatedOn == null)
                    {
                        entity.CreatedOn = DateTime.UtcNow;
                    }
                    WorkflowCategoryMaster updatedEntity = WorkflowCategoryMasterKeysExtension.GeneratePartitionKeyAndRowKey(entity);
                    dbCon.WorkflowCategoryMaster.Add(entity);
                }

                dbCon.SaveChanges();
            }


            return entities;
        }

        /// <summary>
        /// This method is to update multiple records in table
        /// </summary>
        /// <param name="entities">List of WorkflowCategoryMaster entities</param>
        /// <returns>bool</returns>
        public IList<WorkflowCategoryMaster> UpdateBatch(IList<WorkflowCategoryMaster> entities)
        {

            using (dbCon = new DataEntity())
            {
                foreach (WorkflowCategoryMaster entity in entities)
                {
                    WorkflowCategoryMaster entityItem = dbCon.WorkflowCategoryMaster.Single(c => c.PartitionKey == entity.PartitionKey &&
                   c.RowKey == entity.RowKey);

                    dbCon.WorkflowCategoryMaster.Attach(entityItem);

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

                    WorkflowCategoryMaster updatedEntity = WorkflowCategoryMasterKeysExtension.GeneratePartitionKeyAndRowKey(entity);
                    //dbCon.Entry(entityItem).CurrentValues.SetValues(updatedEntity);
                    EntityExtension<WorkflowCategoryMaster>.ApplyOnlyChanges(entityItem, updatedEntity);
                }
                dbCon.SaveChanges();
            }


            return entities;
        }

        /// <summary>
        /// This method is to get an entity
        /// </summary>
        /// <returns>IQueryable<WorkflowCategoryMaster></returns>
        public IQueryable<WorkflowCategoryMaster> GetAny()
        {
            return dbCon.WorkflowCategoryMaster;
        }

        /// <summary>
        /// This method is to get multiple records from table 
        /// based on a combination of partitionkey and rowkey in partial or complete value
        /// </summary>
        /// <param name="entity">WorkflowCategoryMaster entities</param>
        /// <returns>List of WorkflowCategoryMaster</returns>
        public IList<WorkflowCategoryMaster> GetAll(WorkflowCategoryMaster entity)
        {
            using (dbCon = new DataEntity())
            {
                List<WorkflowCategoryMaster> workflowCategoryMaster = null;
                entity = WorkflowCategoryMasterKeysExtension.GeneratePartitionKeyAndRowKey(entity);
                if ((!string.IsNullOrEmpty(entity.PartitionKey) && !string.IsNullOrEmpty(entity.RowKey)))
                {
                   workflowCategoryMaster = dbCon.WorkflowCategoryMaster.Where(ent => ent.PartitionKey.Equals(entity.PartitionKey) && ent.IsActive.Value == true ).ToList<WorkflowCategoryMaster>();
                }
                return workflowCategoryMaster;
            }
        }

    }

    public static class WorkflowCategoryMasterKeysExtension
    {
        //todo: add logic to consider parent child relationship
        public static WorkflowCategoryMaster GeneratePartitionKey(this WorkflowCategoryMaster entity)
        {
            if (entity.CompanyId != null || entity.CompanyId != 0)
            {
                entity.PartitionKey = entity.CompanyId.ToString();
            }
            return entity;

        }
        public static WorkflowCategoryMaster GenerateRowKey(this WorkflowCategoryMaster entity)
        {

            if (entity.Id != null || entity.Id != 0)
            {
                string rowKey = entity.Id.ToString("00000");
                entity.RowKey = rowKey;
            }
            return entity;

        }
        public static WorkflowCategoryMaster GeneratePartitionKeyAndRowKey(this WorkflowCategoryMaster entity)
        {
            return entity.GeneratePartitionKey().GenerateRowKey();
        }
    }
}

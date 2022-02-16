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
    /// Class definition to define the CRUD operation on workflowTrackingMaster data entity class
    /// </summary>
    /// </summary>

    public partial class WorkflowTrackingMasterDS : IDataAccess.IEntity<WorkflowTrackingMaster>
    {
        public DataEntity dbCon;

        /// <summary>
        /// This method is to get one specific record from table
        /// </summary>
        /// <param name="entity">object of WorkflowTrackingMaster</param>
        /// <returns>object of WorkflowTrackingMaster</returns>
        public WorkflowTrackingMaster GetOne(WorkflowTrackingMaster entity)
        {
            WorkflowTrackingMaster _WorkflowTrackingMaster;
            using (dbCon = new DataEntity())
            {
                _WorkflowTrackingMaster = dbCon.WorkflowTrackingMaster.Where(ent => ent.PartitionKey.Equals(entity.PartitionKey) && ent.RowKey.Equals(entity.RowKey)).FirstOrDefault();
                return _WorkflowTrackingMaster;
            }
        }
        /// <summary>
        /// This method is to get all records from table
        /// </summary>
        /// <param >none</param>
        /// <returns>List of WorkflowTrackingMaster</returns>
        public IList<WorkflowTrackingMaster> GetAll()
        {
            using (dbCon = new DataEntity())
            {
                List<WorkflowTrackingMaster> workflowTrackingMasterRows =
                  (from WorkflowTrackingMastertable in dbCon.WorkflowTrackingMaster
                   select WorkflowTrackingMastertable).ToList<WorkflowTrackingMaster>();
                return workflowTrackingMasterRows;
            }
        }
        /// <summary>
        /// This method is to insert one record in table
        /// </summary>
        /// <param name="entity">WorkflowTrackingMaster object of WorkflowTrackingMaster</param>
        /// <returns>bool</returns>
        public WorkflowTrackingMaster Insert(WorkflowTrackingMaster entity)
        {
            using (dbCon = new DataEntity())
            {

                if (entity.StartedOn == null)
                {
                    entity.StartedOn = DateTime.UtcNow;
                }
                dbCon.WorkflowTrackingMaster.Add(entity);
                dbCon.SaveChanges();
            }

            return entity;
        }
        /// <summary>
        /// This method is to update one record in table
        /// </summary>
        /// <param name="entity">object of WorkflowTrackingMaster</param>
        /// <returns>bool</returns>
        public WorkflowTrackingMaster Update(WorkflowTrackingMaster entity)
        {

            using (dbCon = new DataEntity())
            {
                WorkflowTrackingMaster entityItem = dbCon.WorkflowTrackingMaster.Single(c => c.PartitionKey == entity.PartitionKey &&
                    c.RowKey == entity.RowKey);

                dbCon.WorkflowTrackingMaster.Attach(entityItem);

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

                //dbCon.Entry(entityItem).CurrentValues.SetValues(entity);
                EntityExtension<WorkflowTrackingMaster>.ApplyOnlyChanges(entityItem, entity);
                dbCon.SaveChanges();
            }

            return entity;
        }
        /// <summary>
        /// This method is to delete one record from table
        /// </summary>
        /// <param name="entity">Object of WorkflowTrackingMaster</param>
        /// <returns>bool</returns>
        public bool Delete(WorkflowTrackingMaster entity)
        {
            bool result = false;
            using (dbCon = new DataEntity())
            {
                dbCon.WorkflowTrackingMaster.Attach(entity);
                dbCon.WorkflowTrackingMaster.Remove(entity);
                dbCon.SaveChanges();
            }
            result = true;
            return result;
        }
        /// <summary>
        /// This method is to insert multiple records in table
        /// </summary>
        /// <param name="entities">List of WorkflowTrackingMaster entities</param>
        /// <returns>bool</returns>
        public IList<WorkflowTrackingMaster> InsertBatch(IList<WorkflowTrackingMaster> entities)
        {

            using (dbCon = new DataEntity())
            {
                foreach (WorkflowTrackingMaster entity in entities)
                {

                    if (entity.StartedOn == null)
                    {
                        entity.StartedOn = DateTime.UtcNow;
                    }
                    dbCon.WorkflowTrackingMaster.Add(entity);
                }

                dbCon.SaveChanges();
            }


            return entities;
        }

        /// <summary>
        /// This method is to update multiple records in table
        /// </summary>
        /// <param name="entities">List of WorkflowTrackingMaster entities</param>
        /// <returns>bool</returns>
        public IList<WorkflowTrackingMaster> UpdateBatch(IList<WorkflowTrackingMaster> entities)
        {

            using (dbCon = new DataEntity())
            {
                foreach (WorkflowTrackingMaster entity in entities)
                {
                    WorkflowTrackingMaster entityItem = dbCon.WorkflowTrackingMaster.Single(c => c.PartitionKey == entity.PartitionKey &&
                   c.RowKey == entity.RowKey);

                    dbCon.WorkflowTrackingMaster.Attach(entityItem);

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

                    EntityExtension<WorkflowTrackingMaster>.ApplyOnlyChanges(entityItem, entity);
                    //dbCon.Entry(entityItem).CurrentValues.SetValues(entity);
                }
                dbCon.SaveChanges();
            }


            return entities;
        }

        /// <summary>
        /// This method is to get an entity
        /// </summary>
        /// <returns>IQueryable<WorkflowTrackingMaster></returns>
        public IQueryable<WorkflowTrackingMaster> GetAny()
        {
            return dbCon.WorkflowTrackingMaster;
        }

        /// <summary>
        /// This method is to get multiple records from table 
        /// based on a combination of partitionkey and rowkey in partial or complete value
        /// </summary>
        /// <param name="entity">WorkflowTrackingMaster Entity</param>
        /// <returns>List of WorkflowTrackingMaster</returns>
        public IList<WorkflowTrackingMaster> GetAll(WorkflowTrackingMaster entity)
        {
            using (dbCon = new DataEntity())
            {
                List<WorkflowTrackingMaster> workflowTrackingMaster = null;
                if ((!string.IsNullOrEmpty(entity.PartitionKey) && string.IsNullOrEmpty(entity.RowKey)))
                {
                    workflowTrackingMaster = dbCon.WorkflowTrackingMaster.Where(ent => ent.PartitionKey.Equals(entity.PartitionKey)).ToList<WorkflowTrackingMaster>();
                }
                return workflowTrackingMaster;
            }
        }

    }
}

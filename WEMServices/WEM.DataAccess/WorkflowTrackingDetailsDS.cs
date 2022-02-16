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
    /// Class definition to define the CRUD operation on WorkflowTrackingDetails data entity class
    /// </summary>
    /// </summary>

    public partial class WorkflowTrackingDetailsDS : IDataAccess.IEntity<WorkflowTrackingDetails>
    {
        public DataEntity dbCon;

        /// <summary>
        /// This method is to get one specific record from table
        /// </summary>
        /// <param name="entity">object of WorkflowTrackingDetails</param>
        /// <returns>object of WorkflowTrackingDetails</returns>
        public WorkflowTrackingDetails GetOne(WorkflowTrackingDetails entity)
        {
            WorkflowTrackingDetails _WorkflowTrackingDetails;
            using (dbCon = new DataEntity())
            {
                _WorkflowTrackingDetails = dbCon.WorkflowTrackingDetails.Where(ent => ent.PartitionKey.Equals(entity.PartitionKey) && ent.RowKey.Equals(entity.RowKey)).FirstOrDefault();
                return _WorkflowTrackingDetails;
            }
        }
        /// <summary>
        /// This method is to get all records from table
        /// </summary>
        /// <param >none</param>
        /// <returns>List of WorkflowTrackingDetails</returns>
        public IList<WorkflowTrackingDetails> GetAll()
        {
            using (dbCon = new DataEntity())
            {
                List<WorkflowTrackingDetails> workflowTrackingDetailsRows =
                  (from WorkflowTrackingDetailstable in dbCon.WorkflowTrackingDetails
                   select WorkflowTrackingDetailstable).ToList<WorkflowTrackingDetails>();
                return workflowTrackingDetailsRows;
            }
        }
        /// <summary>
        /// This method is to insert one record in table
        /// </summary>
        /// <param name="entity">WorkflowTrackingDetailsobject of WorkflowTrackingDetails</param>
        /// <returns>bool</returns>
        public WorkflowTrackingDetails Insert(WorkflowTrackingDetails entity)
        {
            using (dbCon = new DataEntity())
            {

                if (entity.StartedOn == null)
                {
                    entity.StartedOn = DateTime.UtcNow;
                }
                dbCon.WorkflowTrackingDetails.Add(entity);
                dbCon.SaveChanges();
            }

            return entity;
        }
        /// <summary>
        /// This method is to update one record in table
        /// </summary>
        /// <param name="entity">object of WorkflowTrackingDetails</param>
        /// <returns>bool</returns>
        public WorkflowTrackingDetails Update(WorkflowTrackingDetails entity)
        {

            using (dbCon = new DataEntity())
            {
                WorkflowTrackingDetails entityItem = dbCon.WorkflowTrackingDetails.Single(c => c.PartitionKey == entity.PartitionKey &&
                    c.RowKey == entity.RowKey);

                dbCon.WorkflowTrackingDetails.Attach(entityItem);

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
                EntityExtension<WorkflowTrackingDetails>.ApplyOnlyChanges(entityItem, entity);
                dbCon.SaveChanges();
            }

            return entity;
        }
        /// <summary>
        /// This method is to delete one record from table
        /// </summary>
        /// <param name="entity">Object of WorkflowTrackingDetails</param>
        /// <returns>bool</returns>
        public bool Delete(WorkflowTrackingDetails entity)
        {
            bool result = false;
            using (dbCon = new DataEntity())
            {
                dbCon.WorkflowTrackingDetails.Attach(entity);
                dbCon.WorkflowTrackingDetails.Remove(entity);
                dbCon.SaveChanges();
            }
            result = true;
            return result;
        }
        /// <summary>
        /// This method is to insert multiple records in table
        /// </summary>
        /// <param name="entities">List of WorkflowTrackingDetails entities</param>
        /// <returns>bool</returns>
        public IList<WorkflowTrackingDetails> InsertBatch(IList<WorkflowTrackingDetails> entities)
        {

            using (dbCon = new DataEntity())
            {
                foreach (WorkflowTrackingDetails entity in entities)
                {

                    if (entity.StartedOn == null)
                    {
                        entity.StartedOn = DateTime.UtcNow;
                    }
                    dbCon.WorkflowTrackingDetails.Add(entity);
                }

                dbCon.SaveChanges();
            }


            return entities;
        }

        /// <summary>
        /// This method is to update multiple records in table
        /// </summary>
        /// <param name="entities">List of WorkflowTrackingDetails entities</param>
        /// <returns>bool</returns>
        public IList<WorkflowTrackingDetails> UpdateBatch(IList<WorkflowTrackingDetails> entities)
        {

            using (dbCon = new DataEntity())
            {
                foreach (WorkflowTrackingDetails entity in entities)
                {
                    WorkflowTrackingDetails entityItem = dbCon.WorkflowTrackingDetails.Single(c => c.PartitionKey == entity.PartitionKey &&
                   c.RowKey == entity.RowKey);

                    dbCon.WorkflowTrackingDetails.Attach(entityItem);

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

                    EntityExtension<WorkflowTrackingDetails>.ApplyOnlyChanges(entityItem, entity);
                    //dbCon.Entry(entityItem).CurrentValues.SetValues(entity);
                }
                dbCon.SaveChanges();
            }


            return entities;
        }

        /// <summary>
        /// This method is to get an entity
        /// </summary>
        /// <returns>IQueryable<WorkflowTrackingDetails></returns>
        public IQueryable<WorkflowTrackingDetails> GetAny()
        {
            return dbCon.WorkflowTrackingDetails;
        }

        /// <summary>
        /// This method is to get multiple records from table 
        /// based on a combination of partitionkey and rowkey in partial or complete value
        /// </summary>
        /// <param name="entity">WorkflowTrackingDetails entities</param>
        /// <returns>List of WorkflowTrackingDetails</returns>
        public IList<WorkflowTrackingDetails> GetAll(WorkflowTrackingDetails entity)
        {
            using (dbCon = new DataEntity())
            {
                List<WorkflowTrackingDetails> workflowTrackingDetails = null;
                if ((!string.IsNullOrEmpty(entity.PartitionKey) && string.IsNullOrEmpty(entity.RowKey)))
                {
                    workflowTrackingDetails = dbCon.WorkflowTrackingDetails.Where(ent => ent.PartitionKey.Equals(entity.PartitionKey)).ToList<WorkflowTrackingDetails>();
                }
                return workflowTrackingDetails;
            }
        }

    }
}

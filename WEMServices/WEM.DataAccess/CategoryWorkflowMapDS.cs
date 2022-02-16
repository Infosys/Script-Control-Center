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
    /// Class definition to define the CRUD operation on CategoryWorkflowMap data entity class
    /// </summary>
    /// </summary>

    public partial class CategoryWorkflowMapDS : IDataAccess.IEntity<CategoryWorkflowMap>
    {
        public DataEntity dbCon;

        /// <summary>
        /// This method is to get one specific record from table
        /// </summary>
        /// <param name="entity">object of WorkflowMaster</param>
        /// <returns>object of WorkflowMaster</returns>
        public CategoryWorkflowMap GetOne(CategoryWorkflowMap entity)
        {
            CategoryWorkflowMap _CategoryWorkflowMap;
            using (dbCon = new DataEntity())
            {
                entity = CategoryWorkflowMapKeysExtension.GeneratePartitionKeyAndRowKey(entity);
                _CategoryWorkflowMap = dbCon.CategoryWorkflowMap.Where(ent => ent.PartitionKey.Equals(entity.PartitionKey) && ent.RowKey.Equals(entity.RowKey)).FirstOrDefault();
                return _CategoryWorkflowMap;
            }
        }
        /// <summary>
        /// This method is to get all records from table
        /// </summary>
        /// <param >none</param>
        /// <returns>List of WorkflowMaster</returns>
        public IList<CategoryWorkflowMap> GetAll()
        {
            using (dbCon = new DataEntity())
            {

                List<CategoryWorkflowMap> categoryWorkflowMapRows =
                  (from CategoryWorkflowMaptable in dbCon.CategoryWorkflowMap
                   select CategoryWorkflowMaptable).ToList<CategoryWorkflowMap>();
                return categoryWorkflowMapRows;
            }
        }
        /// <summary>
        /// This method is to insert one record in table
        /// </summary>
        /// <param name="entity">WorkflowMasterobject of WorkflowMaster</param>
        /// <returns>bool</returns>
        public CategoryWorkflowMap Insert(CategoryWorkflowMap entity)
        {
            using (dbCon = new DataEntity())
            {
                entity = CategoryWorkflowMapKeysExtension.GeneratePartitionKeyAndRowKey(entity);
                dbCon.CategoryWorkflowMap.Add(entity);
                dbCon.SaveChanges();
            }

            return entity;
        }
        /// <summary>
        /// This method is to update one record in table
        /// </summary>
        /// <param name="entity">object of WorkflowMaster</param>
        /// <returns>bool</returns>
        public CategoryWorkflowMap Update(CategoryWorkflowMap entity)
        {

            using (dbCon = new DataEntity())
            {
                CategoryWorkflowMap entityItem = dbCon.CategoryWorkflowMap.FirstOrDefault(c => c.WorkflowId == entity.WorkflowId);

                if (entity != null)
                {

                    dbCon.CategoryWorkflowMap.Attach(entityItem);

                    //entity = CategoryWorkflowMapKeysExtension.GeneratePartitionKeyAndRowKey(entity);
                    //dbCon.Entry(entityItem).CurrentValues.SetValues(entity);
                    EntityExtension<CategoryWorkflowMap>.ApplyOnlyChanges(entityItem, entity);
                    dbCon.SaveChanges();
                }
            }

            return entity;
        }
        /// <summary>
        /// This method is to delete one record from table
        /// </summary>
        /// <param name="entity">Object of WorkflowMaster</param>
        /// <returns>bool</returns>
        public bool Delete(CategoryWorkflowMap entity)
        {
            bool result = false;
            using (dbCon = new DataEntity())
            {
                entity = dbCon.CategoryWorkflowMap.FirstOrDefault(wf => wf.WorkflowId == entity.WorkflowId);
                
                //entity = CategoryWorkflowMapKeysExtension.GeneratePartitionKeyAndRowKey(entity);

                if (entity != null)
                {
                    dbCon.CategoryWorkflowMap.Attach(entity);
                    dbCon.CategoryWorkflowMap.Remove(entity);
                    dbCon.SaveChanges();
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// This method is to insert multiple records in table
        /// </summary>
        /// <param name="entities">List of WorkflowMaster entities</param>
        /// <returns>bool</returns>
        public IList<CategoryWorkflowMap> InsertBatch(IList<CategoryWorkflowMap> entities)
        {

            using (dbCon = new DataEntity())
            {
                foreach (CategoryWorkflowMap entity in entities)
                {

                    CategoryWorkflowMap updatedEntity = CategoryWorkflowMapKeysExtension.GeneratePartitionKeyAndRowKey(entity);
                    dbCon.CategoryWorkflowMap.Add(updatedEntity);
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
        public IList<CategoryWorkflowMap> UpdateBatch(IList<CategoryWorkflowMap> entities)
        {

            using (dbCon = new DataEntity())
            {
                foreach (CategoryWorkflowMap entity in entities)
                {
                    CategoryWorkflowMap entityItem = dbCon.CategoryWorkflowMap.Single(c => c.PartitionKey == entity.PartitionKey &&
                   c.RowKey == entity.RowKey);

                    dbCon.CategoryWorkflowMap.Attach(entityItem);

                    CategoryWorkflowMap updatedEntity = CategoryWorkflowMapKeysExtension.GeneratePartitionKeyAndRowKey(entity);
                    //dbCon.Entry(entityItem).CurrentValues.SetValues(updatedEntity);
                    EntityExtension<CategoryWorkflowMap>.ApplyOnlyChanges(entityItem, updatedEntity);
                }
                dbCon.SaveChanges();
            }


            return entities;
        }

        /// <summary>
        /// This method is to get an entity
        /// </summary>
        /// <returns>IQueryable<WorkflowMaster></returns>
        public IQueryable<CategoryWorkflowMap> GetAny()
        {
            return dbCon.CategoryWorkflowMap;
        }

        /// <summary>
        /// This method is to get multiple records from table 
        /// based on partitionkey 
        /// </summary>
        /// <param name="entity">WorkflowMaster entities</param>
        /// <returns>List of WorkflowMaster</returns>
        public IList<CategoryWorkflowMap> GetAll(CategoryWorkflowMap entity)
        {
            using (dbCon = new DataEntity())
            {
                List<CategoryWorkflowMap> categoryWorkflowMap = null;
                entity = CategoryWorkflowMapKeysExtension.GeneratePartitionKeyAndRowKey(entity);
                if ((!string.IsNullOrEmpty(entity.PartitionKey) || string.IsNullOrEmpty(entity.RowKey)))
                {

                    categoryWorkflowMap = dbCon.CategoryWorkflowMap.Where(ent => ent.PartitionKey.Equals(entity.PartitionKey)).ToList<CategoryWorkflowMap>();
                }
                return categoryWorkflowMap;
            }
        }

    }

    public static class CategoryWorkflowMapKeysExtension
    {
        public static CategoryWorkflowMap GeneratePartitionKey(this CategoryWorkflowMap entity)
        {

            entity.PartitionKey = entity.CategoryId.ToString();

            return entity;

        }

        public static CategoryWorkflowMap GenerateRowKey(this CategoryWorkflowMap entity)
        {
            if (entity.WorkflowId != null)
            {
                entity.RowKey = entity.WorkflowId.ToString();
            }
            return entity;

        }

        public static CategoryWorkflowMap GeneratePartitionKeyAndRowKey(this CategoryWorkflowMap entity)
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

        public static string FormRowKey(int workflowId)
        {
            string key = "";
            if (workflowId != null)
            {
                key = workflowId.ToString();
            }
            return key;

        }

    }

}

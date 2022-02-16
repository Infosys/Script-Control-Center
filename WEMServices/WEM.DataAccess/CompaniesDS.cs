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
    /// Class definition to define the CRUD operation on Companies data entity class
    /// </summary>
    /// </summary>

    public partial class CompaniesDS : IDataAccess.IEntity<Companies>
    {
        public DataEntity dbCon;

        /// <summary>
        /// This method is to get one specific record from table
        /// </summary>
        /// <param name="entity">object of Companies</param>
        /// <returns>object of Companies</returns>
        public Companies GetOne(Companies entity)
        {
            Companies _Companies;
            using (dbCon = new DataEntity())
            {
                Companies rowKey = GenerateRowKey(entity);
                entity.PartitionKey = "IAP";
                _Companies = dbCon.Companies.Where(ent => ent.PartitionKey.Equals(entity.PartitionKey) && ent.RowKey.Equals(rowKey.RowKey)).FirstOrDefault();
                return _Companies;
            }
        }
        /// <summary>
        /// This method is to get all records from table
        /// </summary>
        /// <param >none</param>
        /// <returns>List of Companies</returns>
        public IList<Companies> GetAll()
        {
            using (dbCon = new DataEntity())
            {
                List<Companies> CompaniesRows =
                  (from Companiestable in dbCon.Companies
                   select Companiestable).ToList<Companies>();
                return CompaniesRows;
            }
        }
        /// <summary>
        /// This method is to insert one record in table
        /// </summary>
        /// <param name="entity">Companiesobject of Companies</param>
        /// <returns>bool</returns>
        public Companies Insert(Companies entity)
        {
            using (dbCon = new DataEntity())
            {

                if (entity.CreatedOn == null)
                {
                    entity.CreatedOn = DateTime.UtcNow;
                }
                dbCon.Companies.Add(entity);
                dbCon.SaveChanges();
            }

            return entity;
        }
        /// <summary>
        /// This method is to update one record in table
        /// </summary>
        /// <param name="entity">object of Companies</param>
        /// <returns>bool</returns>
        public Companies Update(Companies entity)
        {

            using (dbCon = new DataEntity())
            {
                Companies entityItem = dbCon.Companies.Single(c => c.PartitionKey == entity.PartitionKey &&
                    c.RowKey == entity.RowKey);

                dbCon.Companies.Attach(entityItem);

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
                EntityExtension<Companies>.ApplyOnlyChanges(entityItem, entity);
                dbCon.SaveChanges();
            }

            return entity;
        }
        /// <summary>
        /// This method is to delete one record from table
        /// </summary>
        /// <param name="entity">Object of Companies</param>
        /// <returns>bool</returns>
        public bool Delete(Companies entity)
        {
            bool result = false;
            using (dbCon = new DataEntity())
            {
                dbCon.Companies.Attach(entity);
                dbCon.Companies.Remove(entity);
                dbCon.SaveChanges();
            }
            result = true;
            return result;
        }
        /// <summary>
        /// This method is to insert multiple records in table
        /// </summary>
        /// <param name="entities">List of Companies entities</param>
        /// <returns>bool</returns>
        public IList<Companies> InsertBatch(IList<Companies> entities)
        {

            using (dbCon = new DataEntity())
            {
                foreach (Companies entity in entities)
                {

                    if (entity.CreatedOn == null)
                    {
                        entity.CreatedOn = DateTime.UtcNow;
                    }
                    dbCon.Companies.Add(entity);
                }

                dbCon.SaveChanges();
            }


            return entities;
        }

        /// <summary>
        /// This method is to update multiple records in table
        /// </summary>
        /// <param name="entities">List of Companies entities</param>
        /// <returns>bool</returns>
        public IList<Companies> UpdateBatch(IList<Companies> entities)
        {

            using (dbCon = new DataEntity())
            {
                foreach (Companies entity in entities)
                {
                    Companies entityItem = dbCon.Companies.Single(c => c.PartitionKey == entity.PartitionKey &&
                   c.RowKey == entity.RowKey);

                    dbCon.Companies.Attach(entityItem);

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

                    EntityExtension<Companies>.ApplyOnlyChanges(entityItem, entity);
                    //dbCon.Entry(entityItem).CurrentValues.SetValues(entity);
                }
                dbCon.SaveChanges();
            }


            return entities;
        }

        /// <summary>
        /// This method is to get an entity
        /// </summary>
        /// <returns>IQueryable<Companies></returns>
        public IQueryable<Companies> GetAny()
        {
            return dbCon.Companies;
        }

        /// <summary>
        /// This method is to get multiple records from table 
        /// based on a combination of partitionkey and rowkey in partial or complete value
        /// </summary>
        /// <param name="entity">Companies entities</param>
        /// <returns>List of Companies</returns>
        public IList<Companies> GetAll(Companies entity)
        {
            using (dbCon = new DataEntity())
            {
                List<Companies> companies = null;
                if ((!string.IsNullOrEmpty(entity.PartitionKey) && string.IsNullOrEmpty(entity.RowKey)))
                {
                    companies = dbCon.Companies.Where(ent => ent.PartitionKey.Equals(entity.PartitionKey)).ToList<Companies>();
                }
                return companies;
            }
        }

        public static Companies GenerateRowKey(Companies entity)
        {
            entity.RowKey = entity.CompanyId.ToString("00000");
            return entity;
        }

    }
}

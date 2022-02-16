/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.IAP.Infrastructure.Common.HttpModule.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.IAP.Infrastructure.Common.HttpModule.Entity
{
    /// <summary>
    /// /// <summary>
    /// Class definition to define the CRUD operation on file data entity class
    /// </summary>
    public partial class FileDS 
    {
        private static ContentStoreEntities dbCon;

        /// <summary>
        /// This method is to get one specific record from table
        /// </summary>
        /// <param name="entity">object of Files</param>
        /// <returns>object of Files</returns>
        public static ContentFile GetOne(ContentFile entity)
        {
            ContentFile _File = new ContentFile();
            using (dbCon = new ContentStoreEntities())
            {
                _File = dbCon.ContentFiles.Where(s => s.PartitionKey.Equals(entity.PartitionKey) && s.RowKey.Equals(entity.RowKey) && s.IsDeleted.Equals(false)).FirstOrDefault();
            }
            return _File;
        }
        /// <summary>
        /// This method is to get all records from table 
        /// </summary>
        /// <param >none</param>
        /// <returns>List of File</returns>
        public static IList<ContentFile> GetAll()
        {
            using (dbCon = new ContentStoreEntities())
            {
                List<ContentFile> _Files = (from fileTable in dbCon.ContentFiles
                                            where fileTable.IsDeleted == false
                                            select fileTable).ToList<ContentFile>();
                return _Files;
            }
        }
        /// <summary>
        /// This method is to insert one record in table
        /// </summary>
        /// <param name="entity">object of Files</param>
        /// <returns>bool</returns>
        public static ContentFile Insert(ContentFile content)
        {
            ContentFile entity = new ContentFile();
            entity = content;
            using (dbCon = new ContentStoreEntities())
            {
                //reset modified by, if set during version update
                entity.LastModifiedBy = "";
                if (entity.CreatedOn == null || entity.CreatedOn == DateTime.MinValue)
                {
                    entity.CreatedOn = DateTime.UtcNow;
                    entity.Timestamp = DateTime.UtcNow;
                }
                dbCon.ContentFiles.Add(entity);
                dbCon.SaveChanges();
            }
            return entity;
        }
        /// <summary>
        /// This method is to update one record in table
        /// </summary>
        /// <param name="entity">object of Files</param>
        /// <returns>bool</returns>
        public static ContentFile Update(ContentFile entity)
        {
            ContentFile entityItem = GetOne(entity);
            using (dbCon = new ContentStoreEntities())
            {
                dbCon.ContentFiles.Attach(entityItem);

                 if (entity.LastModifiedOn == null || entity.LastModifiedOn == DateTime.MinValue)
                {
                    entity.LastModifiedOn = DateTime.UtcNow;
                    entity.LastModifiedBy = entity.CreatedBy;
                }
                             
                EntityExtension<ContentFile>.ApplyOnlyChanges(entityItem, entity);
                dbCon.SaveChanges();
            }

            return entity;
        }
        /// <summary>
        /// This method is to delete one record from table
        /// </summary>
        /// <param name="entity">Object of Files</param>
        /// <returns>bool</returns>
        public static bool Delete(ContentFile entity)
        {
            bool result = false;
            using (dbCon = new ContentStoreEntities())
            {
                dbCon.ContentFiles.Attach(entity);
                dbCon.ContentFiles.Remove(entity);
                dbCon.SaveChanges();
            }
            result = true;
            return result;
        }
        /// <summary>
        /// This method is to insert multiple records in table
        /// </summary>
        /// <param name="entities">List of Files entities</param>
        /// <returns>bool</returns>
        public static IList<ContentFile> InsertBatch(IList<ContentFile> entities)
        {
            using (dbCon = new ContentStoreEntities())
            {
                foreach (ContentFile entity in entities)
                {
                    if (entity.CreatedOn == null || entity.CreatedOn == DateTime.MinValue)
                    {
                        entity.CreatedOn = DateTime.UtcNow;
                    }
                    dbCon.ContentFiles.Add(entity);
                }

                dbCon.SaveChanges();
            }

            return entities;
        }
        /// <summary>
        /// This method is to update multiple records in table
        /// </summary>
        /// <param name="entities">List of Content File entities</param>
        /// <returns>bool</returns>
        public static IList<ContentFile> UpdateBatch(IList<ContentFile> entities)
        {
            using (dbCon = new ContentStoreEntities())
            {
                foreach (ContentFile entity in entities)
                {
                    ContentFile entityItem = GetOne(entity);

                    dbCon.ContentFiles.Attach(entityItem);

                    DateTime? lastModifiedOn = null;

                    lastModifiedOn = entityItem.LastModifiedOn;

                    if (entity.LastModifiedOn == null || entity.LastModifiedOn == DateTime.MinValue)
                    {
                        entity.LastModifiedOn = DateTime.UtcNow;
                    }
                    if (lastModifiedOn == entity.LastModifiedOn)
                    {
                        entity.LastModifiedOn = DateTime.UtcNow;
                    }
                    EntityExtension<ContentFile>.ApplyOnlyChanges(entityItem, entity);                    
                }
                dbCon.SaveChanges();
            }


            return entities;
        }
        /// <summary>
        /// This method is to get multiple records from table 
        /// based on partitionkey 
        /// </summary>
        /// <param name="entity">Files entity having the partitonkey value</param>
        /// <returns>List of Files</returns>
        public static IList<ContentFile> GetAll(ContentFile entity)
        {
            using (dbCon = new ContentStoreEntities())
            {
                List<ContentFile> scripts = null;
                if ((!string.IsNullOrEmpty(entity.PartitionKey) && string.IsNullOrEmpty(entity.RowKey)))
                {
                    scripts = dbCon.ContentFiles.Where(ent => ent.PartitionKey.Equals(entity.PartitionKey) && !ent.IsDeleted).ToList<ContentFile>();                   
                }
                return scripts;
            }
        }
    }
}

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

    public partial class ObjectModelMasterDS : IDataAccess.IEntity<ObjectModelMaster>
    {
        public static DataEntity dbCon;

        static ObjectModelMasterDS()
        {
            dbCon = new DataEntity();
        }

        public ObjectModelMaster GetOne(ObjectModelMaster entity)
        {
            ObjectModelMaster _ObjectModelMaster;
            //using (dbCon = new DataEntity())
            {
                entity = ObjectModelMasterKeysExtension.GeneratePartitionKeyAndRowKey(entity);
                _ObjectModelMaster = dbCon.ObjectModelMaster
                    .Where(ent => ent.PartitionKey.Equals(entity.PartitionKey) && ent.RowKey.Equals(entity.RowKey)).FirstOrDefault();
                return _ObjectModelMaster;
            }
        }

        public IList<ObjectModelMaster> GetAll()
        {
           // using (dbCon = new DataEntity())
            {

                List<ObjectModelMaster> objectModelMasterRows =
                  (from objectMastertable in dbCon.ObjectModelMaster
                   select objectMastertable).ToList<ObjectModelMaster>();
                return objectModelMasterRows;
            }
        }

        public ObjectModelMaster Insert(ObjectModelMaster entity)
        {
           // using (dbCon = new DataEntity())
            {
                if (entity.PublishedOn == null)
                {
                    entity.PublishedOn = DateTime.UtcNow;
                }
                entity = ObjectModelMasterKeysExtension.GeneratePartitionKeyAndRowKey(entity);
                dbCon.ObjectModelMaster.Add(entity);
                dbCon.SaveChanges();
            }

            return entity;
        }

        public ObjectModelMaster Update(ObjectModelMaster entity)
        {

            //using (dbCon = new DataEntity())
            {
                ObjectModelMaster entityItem = dbCon.ObjectModelMaster.Single(c => c.PartitionKey == entity.PartitionKey &&
                    c.RowKey == entity.RowKey);

                dbCon.ObjectModelMaster.Attach(entityItem);

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
                entity = ObjectModelMasterKeysExtension.GeneratePartitionKeyAndRowKey(entity);
                //dbCon.Entry(entityItem).CurrentValues.SetValues(entity);
                EntityExtension<ObjectModelMaster>.ApplyOnlyChanges(entityItem, entity);
                dbCon.SaveChanges();
            }

            return entity;
        }


        public bool Delete(ObjectModelMaster entity)
        {
            bool result = false;
           // using (dbCon = new DataEntity())
            {
                entity = ObjectModelMasterKeysExtension.GeneratePartitionKeyAndRowKey(entity);
                dbCon.ObjectModelMaster.Attach(entity);
                dbCon.ObjectModelMaster.Remove(entity);
                dbCon.SaveChanges();
            }
            result = true;
            return result;
        }


        public IList<ObjectModelMaster> InsertBatch(IList<ObjectModelMaster> entities)
        {

          //  using (dbCon = new DataEntity())
            {
                foreach (ObjectModelMaster entity in entities)
                {

                    if (entity.PublishedOn == null)
                    {
                        entity.PublishedOn = DateTime.UtcNow;
                    }
                    ObjectModelMaster updatedEntity = ObjectModelMasterKeysExtension.GeneratePartitionKeyAndRowKey(entity);
                    dbCon.ObjectModelMaster.Add(updatedEntity);
                }

                dbCon.SaveChanges();
            }


            return entities;
        }

        public IList<ObjectModelMaster> UpdateBatch(IList<ObjectModelMaster> entities)
        {

           // using (dbCon = new DataEntity())
            {
                foreach (ObjectModelMaster entity in entities)
                {
                    ObjectModelMaster entityItem = dbCon.ObjectModelMaster.Single(c => c.PartitionKey == entity.PartitionKey &&
                   c.RowKey == entity.RowKey);

                    dbCon.ObjectModelMaster.Attach(entityItem);

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

                    ObjectModelMaster updatedEntity = ObjectModelMasterKeysExtension.GeneratePartitionKeyAndRowKey(entity);
                    //dbCon.Entry(entityItem).CurrentValues.SetValues(updatedEntity);
                    EntityExtension<ObjectModelMaster>.ApplyOnlyChanges(entityItem, updatedEntity);
                }
                dbCon.SaveChanges();
            }


            return entities;
        }

        public IQueryable<ObjectModelMaster> GetAny()
        {
            return dbCon.ObjectModelMaster;
        }

        public IList<ObjectModelMaster> GetAll(ObjectModelMaster entity)
        {
           // using (dbCon = new DataEntity())
            {
                List<ObjectModelMaster> workflowMaster = null;
                entity = ObjectModelMasterKeysExtension.GeneratePartitionKeyAndRowKey(entity);
                if ((!string.IsNullOrEmpty(entity.PartitionKey) && string.IsNullOrEmpty(entity.RowKey)))
                {

                    workflowMaster = dbCon.ObjectModelMaster.Where(ent => ent.PartitionKey.Equals(entity.PartitionKey)).ToList<ObjectModelMaster>();
                }
                return workflowMaster;
            }
        }

    }

    public static class ObjectModelMasterKeysExtension
    {
        public static ObjectModelMaster GeneratePartitionKey(this ObjectModelMaster entity)
        {

            entity.PartitionKey = entity.CategoryId.ToString();

            return entity;

        }
        public static ObjectModelMaster GenerateRowKey(this ObjectModelMaster entity)
        {
            if (entity.Id != null)
            {
                string rowKey = ApplicationConstants.TWOFIELD_KEY_FORMAT;
                entity.RowKey = string.Format(rowKey, entity.Id.ToString(), entity.ObjectModelVer.ToString());
            }
            return entity;

        }
        public static ObjectModelMaster GeneratePartitionKeyAndRowKey(this ObjectModelMaster entity)
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
    }


}

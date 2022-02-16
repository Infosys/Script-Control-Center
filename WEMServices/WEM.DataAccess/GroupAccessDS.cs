/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infosys.WEM.Resource.Entity;
using Infosys.WEM.Infrastructure.Common;


namespace Infosys.WEM.Resource.DataAccess
{
    public class GroupAccessDS : IDataAccess.IEntity<GroupAccess>
    {
        DataEntity _dbCon = null;

        public GroupAccess GetOne(GroupAccess entity)
        {
            GroupAccess groupAccess;
            using (_dbCon = new DataEntity())
            {
                entity = entity.GeneratePartitionKeyAndRowKey();
                groupAccess =
                    _dbCon.GroupAccess.Where(ent => ent.PartitionKey.Equals(entity.PartitionKey) && ent.RowKey.Equals(entity.RowKey)).FirstOrDefault();
                return groupAccess;
            }
        }


        public IQueryable<GroupAccess> GetAny()
        {
            return null;
        }

        public IList<GroupAccess> GetAll()
        {
            using (_dbCon = new DataEntity())
            {
                return _dbCon.GroupAccess.ToList();
            }
        }

        //public IList<GroupAccess> GetAll()
        //{
        //    using (_dbCon = new DataEntity())
        //    {
        //        return _dbCon.GroupAccess().ToList();
        //    }
        //}

        public GroupAccess Insert(GroupAccess entity)
        {
            using (_dbCon = new DataEntity())
            {
                 entity = entity.GeneratePartitionKeyAndRowKey();
                _dbCon.GroupAccess.Add(entity);
                _dbCon.SaveChanges();
            }

            return entity;
        }

        public GroupAccess Update(GroupAccess entity)
        {
            using (_dbCon = new DataEntity())
            {
                GroupAccess entityItem = _dbCon.GroupAccess.Single(c => c.GroupId == entity.GroupId);

                _dbCon.GroupAccess.Attach(entityItem);

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

                entity.GeneratePartitionKeyAndRowKey();
                _dbCon.Entry(entityItem).CurrentValues.SetValues(entity);
                //EntityExtension<GroupAccess>.ApplyOnlyChanges(entityItem, entity);
                _dbCon.SaveChanges();
            }

            return entity;
        }

        public bool Delete(GroupAccess entity)
        {
            bool result = false;
            using (_dbCon = new DataEntity())
            {
                entity = entity.GeneratePartitionKeyAndRowKey();
                _dbCon.GroupAccess.Attach(entity);
                _dbCon.GroupAccess.Remove(entity);
                _dbCon.SaveChanges();
            }
            result = true;
            return result;
        }

        public IList<GroupAccess> InsertBatch(IList<GroupAccess> entities)
        {
            return null;
        }

        public IList<GroupAccess> UpdateBatch(IList<GroupAccess> entities)
        {
            return null;
        }

        public IList<GroupAccess> GetAll(GroupAccess Entity)
        {
            return null;
        }
    }

    public static class GroupAccessExtension
    {
        public static GroupAccess GeneratePartitionKey(this GroupAccess entity)
        {
            entity.PartitionKey = entity.GroupId.ToString();
            return entity;

        }

        public static GroupAccess GenerateRowKey(this GroupAccess entity)
        {
            entity.RowKey = entity.GroupId.ToString();
            return entity;

        }

        public static GroupAccess GeneratePartitionKeyAndRowKey(this GroupAccess entity)
        {
            return entity.GeneratePartitionKey().GenerateRowKey();
        }

        public static string FormPartitionKey(int groupId)
        {
            return groupId.ToString();
        }

        public static string FormRowKey(int groupId)
        {
            return groupId.ToString();
        }

    }
}

/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.WEM.Resource.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infosys.WEM.Resource.DataAccess
{
    public class GroupDS : IDataAccess.IEntity<Group>
    {
        public DataEntity dbCon;

        public Group GetOne(Group entity)
        {
            Group _Group;
            using (dbCon = new DataEntity())
            {
                entity = GroupExtension.GeneratePartitionKeyAndRowKey(entity);
                _Group = dbCon.Group.Where(ent => ent.PartitionKey.Equals(entity.PartitionKey) && ent.RowKey.Equals(entity.RowKey)).FirstOrDefault();
                return _Group;
            }
        }

        public int GetAny()
        {
            return dbCon.Group.Max(g => g.Id);
        }

        public IList<Group> GetAll(Group group)
        {
            return null;
        }

        public Group Insert(Group entity)
        {
            using (dbCon = new DataEntity())
            {
                int seed = this.GetAny();
                // if (entity.CreatedOn == null)
                {
                    entity.CreatedOn = DateTime.UtcNow;
                    entity.Id = ++seed;
                    // entity.IsActive = true;
                }
                entity = GroupExtension.GeneratePartitionKeyAndRowKey(entity);
                dbCon.Group.Add(entity);
                dbCon.SaveChanges();
            }

            return entity;
        }

        public Group Update(Group entity)
        {
            using (dbCon = new DataEntity())
            {
                entity = GroupExtension.GeneratePartitionKeyAndRowKey(entity);
                Group entityItem = dbCon.Group.Single(c => c.PartitionKey == entity.PartitionKey &&
                     c.RowKey == entity.RowKey);

                dbCon.Group.Attach(entityItem);
                entity.LastModifiedOn = DateTime.UtcNow;
                entity.PartitionKey = null;
                entity.RowKey = null;
                EntityExtension<Group>.ApplyOnlyChanges(entityItem, entity);
                dbCon.SaveChanges();
            }

            return entity;
        }

        public bool Delete(Group entity)
        {
            bool result = false;
            using (dbCon = new DataEntity())
            {
                entity = GroupExtension.GeneratePartitionKeyAndRowKey(entity);
                Group entityItem = dbCon.Group.Single(c => c.PartitionKey == entity.PartitionKey &&
                    c.RowKey == entity.RowKey);
                entityItem.IsActive = false;
                entityItem.LastModifiedOn = DateTime.UtcNow;
                dbCon.Group.Attach(entityItem);
                var e = dbCon.Entry(entityItem);
                e.Property(u => u.IsActive).IsModified = true;
                e.Property(u => u.LastModifiedOn).IsModified = true;
                dbCon.SaveChanges();
            }
            result = true;
            return result;
        }     

        public IList<Group> InsertBatch(IList<Group> entities)
        {
            using (dbCon = new DataEntity())
            {
                foreach (Group entity in entities)
                {
                    if (entity.CreatedOn == null)
                    {
                        entity.CreatedOn = DateTime.UtcNow;
                    }
                    dbCon.Group.Add(entity);
                }

                dbCon.SaveChanges();
            }
            return entities;
        }

        public IList<Group> UpdateBatch(IList<Group> entities)
        {
            using (dbCon = new DataEntity())
            {
                foreach (Group entity in entities)
                {
                    Group entityItem = dbCon.Group.Single(c => c.PartitionKey == entity.PartitionKey &&
                   c.RowKey == entity.RowKey);
                    dbCon.Group.Attach(entityItem);
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
                    EntityExtension<Group>.ApplyOnlyChanges(entityItem, entity);
                }
                dbCon.SaveChanges();
            }

            return entities;
        }

        public IList<Group> GetAll(int companyId)
        {
            using (dbCon = new DataEntity())
            {
                //Entity = GroupExtension.GeneratePartitionKeyAndRowKey(Entity);
                List<Group> groups = null;
                //if ((!string.IsNullOrEmpty(Entity.PartitionKey) && string.IsNullOrEmpty(Entity.RowKey)))
                {
                    // groups = dbCon.Group.Where(ent => ent.PartitionKey.Equals(Entity.PartitionKey)).ToList<Group>();
                    groups = dbCon.Group.Where(g => g.CompanyId == companyId && g.IsActive == true).ToList<Group>();
                }
                return groups;
            }
        }


        public IList<Group> GetAll()
        {
            return null;
        }


        IQueryable<Group> IDataAccess.IEntity<Group>.GetAny()
        {
            throw new NotImplementedException();
        }
    }
}

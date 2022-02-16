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
    public class RoleDS : IDataAccess.IEntity<Role>
    {
        public DataEntity dbCon;

        public Role GetOne(Role Entity)
        {
            Role _Role;
            if (string.IsNullOrEmpty(Entity.PartitionKey) && string.IsNullOrEmpty(Entity.RowKey))
                Entity = RoleExtension.GeneratePartitionKeyAndRowKey(Entity);
            using (dbCon = new DataEntity())
            {
                _Role = dbCon.Role.Where(ent => ent.PartitionKey.Equals(Entity.PartitionKey) && ent.RowKey.Equals(Entity.RowKey)).FirstOrDefault();
                return _Role;
            }
        }

        public IQueryable<Role> GetAny()
        {
            return dbCon.Role;
        }

        public IList<Role> GetAll()
        {
            using (dbCon = new DataEntity())
            {
                List<Role> Roles = (from u in dbCon.Role
                                      select u).ToList<Role>();
                return Roles;
            }
        }

        public Role Insert(Role entity)
        {
            using (dbCon = new DataEntity())
            {
                int seed = this.GetAny().Count();
                //if (entity.CreatedOn == null)
                {
                    entity.CreatedOn = DateTime.UtcNow;
                    entity.Id = ++seed;
                    entity.IsActive = true;
                }
                entity = RoleExtension.GeneratePartitionKeyAndRowKey(entity);
                dbCon.Role.Add(entity);
                dbCon.SaveChanges();
            }

            return entity;
        }

        public Role Update(Role entity)
        {
            using (dbCon = new DataEntity())
            {
                entity = RoleExtension.GeneratePartitionKeyAndRowKey(entity);
                Role entityItem = dbCon.Role.Single(c => c.PartitionKey == entity.PartitionKey &&
                    c.RowKey == entity.RowKey);

                dbCon.Role.Attach(entityItem);

                DateTime? lastModifiedOn = entityItem.LastModified;

                if (entity.LastModified == null)
                {
                    entity.LastModified = DateTime.UtcNow;
                }
                //if (lastModifiedOn == entity.LastModifiedOn)
                //{
                //    entity.LastModifiedOn = DateTime.UtcNow;
                //}
                entity.PartitionKey = null;
                entity.RowKey = null;
                EntityExtension<Role>.ApplyOnlyChanges(entityItem, entity);
                dbCon.SaveChanges();
            }

            return entity;
        }

        public bool Delete(Role entity)
        {
            bool result = false;
            using (dbCon = new DataEntity())
            {
                entity = RoleExtension.GeneratePartitionKeyAndRowKey(entity);
                dbCon.Role.Attach(entity);
                dbCon.Role.Remove(entity);
                dbCon.SaveChanges();
            }
            result = true;
            return result;
        }

        public IList<Role> InsertBatch(IList<Role> entities)
        {
            using (dbCon = new DataEntity())
            {
                foreach (Role entity in entities)
                {
                    if (entity.CreatedOn == null)
                    {
                        entity.CreatedOn = DateTime.UtcNow;
                    }
                    dbCon.Role.Add(entity);
                }

                dbCon.SaveChanges();
            }
            return entities;
        }

        public IList<Role> UpdateBatch(IList<Role> entities)
        {
            using (dbCon = new DataEntity())
            {
                foreach (Role entity in entities)
                {
                    Role entityItem = dbCon.Role.Single(c => c.PartitionKey == entity.PartitionKey &&
                   c.RowKey == entity.RowKey);
                    dbCon.Role.Attach(entityItem);
                    DateTime? lastModifiedOn = null;
                    lastModifiedOn = entityItem.LastModified;
                    if (entity.LastModified == null)
                    {
                        entity.LastModified = DateTime.UtcNow;
                    }
                    if (lastModifiedOn == entity.LastModified)
                    {
                        entity.LastModified = DateTime.UtcNow;
                    }
                    EntityExtension<Role>.ApplyOnlyChanges(entityItem, entity);
                }
                dbCon.SaveChanges();
            }

            return entities;
        }

        public IList<Role> GetAll(Role Entity)
        {
            using (dbCon = new DataEntity())
            {
                List<Role> Roles = null;
                if ((!string.IsNullOrEmpty(Entity.PartitionKey) && string.IsNullOrEmpty(Entity.RowKey)))
                {
                    Roles = dbCon.Role.Where(ent => ent.PartitionKey.Equals(Entity.PartitionKey)).ToList<Role>();
                }
                return Roles;
            }
        }


        public IList<Role> GetAll(int companyId)
        {
            using (dbCon = new DataEntity())
            {
                return dbCon.Role.Where(r => r.CompanyId == companyId).ToList();
            }
        }
        public Role GetOneIfActive(Role Entity)
        {
            Role _Role;
            if (string.IsNullOrEmpty(Entity.PartitionKey) && string.IsNullOrEmpty(Entity.RowKey))
                Entity = RoleExtension.GeneratePartitionKeyAndRowKey(Entity);
            using (dbCon = new DataEntity())
            {
                _Role = dbCon.Role.Where(ent => ent.PartitionKey.Equals(Entity.PartitionKey) && ent.RowKey.Equals(Entity.RowKey) && ent.IsActive == true).FirstOrDefault();
                return _Role;
            }
        }
    }
}

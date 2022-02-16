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
    public class UserDS : IDataAccess.IEntity<User>
    {
        public DataEntity dbCon;

        public User GetOne(User entity)
        {
            User _User;
            using (dbCon = new DataEntity())
            {
                entity = UserExtension.GeneratePartitionKeyAndRowKey(entity);
                _User = dbCon.User.Where(ent => ent.PartitionKey.Equals(entity.PartitionKey) && ent.RowKey.Equals(entity.RowKey)).FirstOrDefault();
                return _User;
            }
        }


        public List<User> GetAllUsers(string alias, string companyId)
        {
            
            using (dbCon = new DataEntity())
            {
                var entity = UserExtension.GeneratePartitionKey(new User
                {
                    Alias = alias,
                    CompanyId = Convert.ToInt32(companyId)
                });
                
                
                List<User> userDets = dbCon.User.Where(u => (u.PartitionKey == entity.PartitionKey && u.IsActive == true) || (u.IsDL == true && u.IsActive == true) || (u.Alias == Infosys.WEM.Infrastructure.Common.ApplicationConstants.DOMAIN_USERS && u.IsActive == true)).ToList();
              
                foreach (User user in userDets.ToList())
                {
                    if (dbCon.Category.Where(c => c.CategoryId == user.CategoryId && c.IsDeleted == false).ToList().Count > 0)
                    {
                        if (!Infosys.WEM.Service.Implementation.Security.Access.Check(user.CategoryId.ToString(), user.Alias.Trim()))
                            userDets.Remove(user);
                    }
                    else
                        userDets.Remove(user);
                }
              
                return userDets.ToList();
            }
        }

        public List<User> GetAllUsersbyCategory(string alias, string companyId, int categoryId)
        {

            using (dbCon = new DataEntity())
            {
                var entity = UserExtension.GeneratePartitionKey(new User
                {
                    Alias = alias,
                    CompanyId = Convert.ToInt32(companyId)
                });
                
                //Block46
                //DateTime processStartedTime = DateTime.Now;
                List<User> userDets = dbCon.User.Where(u => (u.PartitionKey == entity.PartitionKey && u.IsActive == true && u.CategoryId==categoryId) || (u.IsDL == true && u.IsActive == true && u.CategoryId == categoryId) || (u.Alias == Infosys.WEM.Infrastructure.Common.ApplicationConstants.DOMAIN_USERS && u.IsActive == true && u.CategoryId == categoryId)).ToList();
                //LogHandler.LogDebug(string.Format("Time taken by Block 46 : WEM-GetAllUsers-DB :{0}", DateTime.Now.Subtract(processStartedTime).TotalSeconds), LogHandler.Layer.Business, null);
                //Block47
                //DateTime processStartedTime1 = DateTime.Now;
                foreach (User user in userDets.ToList())
                {
                    if (dbCon.Category.Where(c => c.CategoryId == user.CategoryId && c.IsDeleted == false).ToList().Count > 0)
                    {
                        if (!Infosys.WEM.Service.Implementation.Security.Access.Check(user.CategoryId.ToString(), user.Alias.Trim()))
                            userDets.Remove(user);
                    }
                    else
                        userDets.Remove(user);
                }
               // LogHandler.LogDebug(string.Format("Time taken by Block 47 : WEM-GetAllUsers-Loop :{0}", DateTime.Now.Subtract(processStartedTime1).TotalSeconds), LogHandler.Layer.Business, null);
                return userDets.ToList();
            }
        }

        public int? GetMaxId()
        {
            return dbCon.User.Max(u => (int?)u.Id);
        }

        public IList<User> GetAll()
        {
            using (dbCon = new DataEntity())
            {
                List<User> Users = (from u in dbCon.User
                                    where u.IsActive == true
                                    select u).ToList<User>();
                return Users;
            }
        }

        public User Insert(User entity)
        {
            using (dbCon = new DataEntity())
            {
                var alias = entity.Alias.Trim();

                var existing = dbCon.User.FirstOrDefault(u => u.CompanyId == entity.CompanyId &&
                    u.Alias == alias &&
                    u.CategoryId == entity.CategoryId &&
                    u.IsActive == true);

                if (existing == null)
                {
                    int seed = this.GetMaxId().GetValueOrDefault();
                    // if (entity.CreatedOn == null)  
                    {
                        entity.CreatedOn = DateTime.UtcNow;
                        entity.Id = ++seed;
                        entity.IsActive = true;

                    }
                    entity = UserExtension.GeneratePartitionKeyAndRowKey(entity);
                    dbCon.User.Add(entity);
                    dbCon.SaveChanges();
                }
                else
                {
                    Exception duplicateUserEx = new Exception();
                    duplicateUserEx.Data["StatusCode"] = Infosys.WEM.Infrastructure.Common.Errors.ErrorCodes.User_Exists;
                    duplicateUserEx.Data["StatusDescription"] = String.Format(Infosys.WEM.Infrastructure.Common.ErrorMessages.User_Exists, entity.Alias);
                    throw duplicateUserEx;
                }
                //throw new Exception(String.Format("User {0} in role {1} for category {2} already present",entity.Alias,entity.Role,entity.GroupId));
            }

            return entity;
        }

        public User InsertUserToAGroup(User entity)
        {
            using (dbCon = new DataEntity())
            {
                var alias = entity.Alias.Trim();

                var existing = dbCon.User.FirstOrDefault(u => u.CompanyId == entity.CompanyId &&
                    u.Alias == alias &&
                    u.GroupId == entity.GroupId &&
                    u.IsActive == true);

                if (existing == null)
                {
                    int seed = this.GetMaxId().GetValueOrDefault();
                    // if (entity.CreatedOn == null)
                    {
                        entity.CreatedOn = DateTime.UtcNow;
                        entity.Id = ++seed;
                        entity.IsActive = true;
                        entity.IsActiveGroup = true;
                    }
                    entity = UserExtension.GeneratePartitionKeyAndRowKey(entity);
                    dbCon.User.Add(entity);
                    dbCon.SaveChanges();
                }
                //else
                //    throw new Exception(String.Format("User {0} in role {1} for category {2} already present",entity.Alias,entity.Role,entity.GroupId));
            }

            return entity;
        }



        public bool UpdateUserCategory(int categoryId, int groupId)
        {
            using (dbCon = new DataEntity())
            {
                var users = dbCon.User.Where(c => c.GroupId == groupId).ToList();

                users.ForEach(u =>
                {
                    u.CategoryId = categoryId;
                    u.LastModifiedOn = DateTime.UtcNow;
                    dbCon.User.Attach(u);
                    var entry = dbCon.Entry(u);
                    entry.Property(e => e.CategoryId).IsModified = true;
                    entry.Property(e => e.LastModifiedOn).IsModified = true;
                });
                dbCon.SaveChanges();
            }

            return true;

        }

        public User Update(User entity)
        {
            using (dbCon = new DataEntity())
            {
                entity = UserExtension.GeneratePartitionKeyAndRowKey(entity);
                User entityItem = dbCon.User.Single(c => c.PartitionKey == entity.PartitionKey &&
                     c.RowKey == entity.RowKey);

                dbCon.User.Attach(entityItem);

                //DateTime? lastModifiedOn = entityItem.LastModifiedOn;

                // if (entity.LastModifiedOn == null)
                {
                    entity.LastModifiedOn = DateTime.UtcNow;
                }
                //if (lastModifiedOn == entity.LastModifiedOn)
                //{
                //    entity.LastModifiedOn = DateTime.UtcNow;
                //}
                //entity = UserExtension.GeneratePartitionKeyAndRowKey(entity);
                entity.PartitionKey = null;
                entity.RowKey = null;
                EntityExtension<User>.ApplyOnlyChanges(entityItem, entity);
                dbCon.SaveChanges();
            }

            return entity;
        }

        public bool Delete(User entity)
        {
            bool result = false;
            using (dbCon = new DataEntity())
            {
                entity = UserExtension.GeneratePartitionKeyAndRowKey(entity);
                User entityItem = dbCon.User.Single(c => c.PartitionKey == entity.PartitionKey &&
                    c.RowKey == entity.RowKey);
                entityItem.IsActive = false;
                entityItem.LastModifiedOn = DateTime.UtcNow;
                dbCon.User.Attach(entityItem);
                var e = dbCon.Entry(entityItem);
                e.Property(u => u.IsActive).IsModified = true;
                e.Property(u => u.LastModifiedOn).IsModified = true;
                dbCon.SaveChanges();
            }
            result = true;
            return result;
        }

        public bool DeleteUserInAGroup(User entity)
        {
            bool result = false;
            using (dbCon = new DataEntity())
            {
                entity = UserExtension.GeneratePartitionKeyAndRowKey(entity);
                User entityItem = dbCon.User.Single(c => c.PartitionKey == entity.PartitionKey &&
                    c.RowKey == entity.RowKey);
                entityItem.IsActiveGroup = false;
                entityItem.LastModifiedOn = DateTime.UtcNow;
                dbCon.User.Attach(entityItem);
                var e = dbCon.Entry(entityItem);
                e.Property(u => u.IsActive).IsModified = true;
                e.Property(u => u.IsActiveGroup).IsModified = true;
                e.Property(u => u.LastModifiedOn).IsModified = true;
                dbCon.SaveChanges();
            }
            result = true;
            return result;
        }

        public IList<User> InsertBatch(IList<User> entities)
        {
            using (dbCon = new DataEntity())
            {
                foreach (User entity in entities)
                {
                    if (entity.CreatedOn == null)
                    {
                        entity.CreatedOn = DateTime.UtcNow;
                    }
                    dbCon.User.Add(entity);
                }

                dbCon.SaveChanges();
            }
            return entities;
        }

        public IList<User> UpdateBatch(IList<User> entities)
        {
            using (dbCon = new DataEntity())
            {
                foreach (User entity in entities)
                {
                    User entityItem = dbCon.User.Single(c => c.PartitionKey == entity.PartitionKey &&
                   c.RowKey == entity.RowKey);
                    dbCon.User.Attach(entityItem);
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
                    EntityExtension<User>.ApplyOnlyChanges(entityItem, entity);
                }
                dbCon.SaveChanges();
            }

            return entities;
        }

        public IList<User> GetAll(int companyId)
        {
            using (dbCon = new DataEntity())
            {
                //Entity = UserExtension.GeneratePartitionKeyAndRowKey(Entity);
                List<User> Users = null;
                //if ((!string.IsNullOrEmpty(Entity.PartitionKey) && string.IsNullOrEmpty(Entity.RowKey)))
                {
                    // Users = dbCon.User.Where(ent => ent.PartitionKey.Equals(Entity.PartitionKey)).ToList<User>();
                    Users = dbCon.User.Where(u => u.CompanyId == companyId).ToList<User>();
                }
                return Users;
            }
        }

        public IList<User> GetAll(User Entity)
        {
            using (dbCon = new DataEntity())
            {
                //Entity = UserExtension.GeneratePartitionKeyAndRowKey(Entity);
                List<User> Users = null;
                //if ((!string.IsNullOrEmpty(Entity.PartitionKey) && string.IsNullOrEmpty(Entity.RowKey)))
                {
                    // Users = dbCon.User.Where(ent => ent.PartitionKey.Equals(Entity.PartitionKey)).ToList<User>();
                    Users = dbCon.User.Where(u => u.Alias == Entity.Alias).ToList();
                }
                return Users;
            }
        }

        public IList<User> GetAnyUser(User entity)
        {
            using (dbCon = new DataEntity())
            {
                //entity = UserExtension.GeneratePartitionKeyAndRowKey(entity);
                var users = dbCon.User.Where(ent => ent.CompanyId == entity.CompanyId && ent.CategoryId == entity.CategoryId && ent.IsActive == true);
                return users.ToList();
            }
        }

        public IList<User> GetAllUsersInAGroup(User entity)
        {
            using (dbCon = new DataEntity())
            {
                var users = dbCon.User.Where(ent => ent.CompanyId == entity.CompanyId && ent.GroupId == entity.GroupId && ent.IsActiveGroup == true).Distinct();
                return users.ToList();
            }
        }


        IQueryable<User> IDataAccess.IEntity<User>.GetAny()
        {
            throw new NotImplementedException();
        }


        public bool IsBelongsToUser(string dl, int categoryId)
        {
            using (dbCon = new DataEntity())
            {
                var user = dbCon.User.FirstOrDefault(u => u.Alias.ToLower() == dl.ToLower() && u.IsDL == true && u.IsActive == true && u.CategoryId == categoryId);
                if (user != null)
                    return true;
            }

            return false;
        }

        public List<string> GetAllActiveDLs()
        {
            using (dbCon = new DataEntity())
            {
                List<string> Users = (from u in dbCon.User
                                      where u.IsActive == true && u.IsDL == true
                                      select u.Alias).Distinct().ToList<string>();
                return Users;
            }
        }
    }
}

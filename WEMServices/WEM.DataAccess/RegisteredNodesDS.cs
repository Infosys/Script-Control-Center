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
using IDataAccess = Infosys.WEM.Resource.IDataAccess;

namespace Infosys.WEM.Resource.DataAccess
{
    public class RegisteredNodesDS : IDataAccess.IEntity<RegisterredNodes>
    {
        public DataEntity dbCon;
        #region IEntity<RegisterredNodes> Members

        public RegisterredNodes GetOne(RegisterredNodes entity)
        {
            RegisterredNodes node = null;
            using (dbCon = new DataEntity())
            {
                node = dbCon.RegisterredNodes.Where(s => s.PartitionKey.ToLower().Equals(entity.PartitionKey.ToLower()) && s.RowKey.ToLower().Equals(entity.RowKey.ToLower())).FirstOrDefault();
            }
            return node;
        }

        public IQueryable<RegisterredNodes> GetAny()
        {
            throw new NotImplementedException();
        }

        public IList<RegisterredNodes> GetAll()
        {
            using (dbCon = new DataEntity())
            {
                return dbCon.RegisterredNodes.ToList();
            }            
        }

        public RegisterredNodes Insert(RegisterredNodes entity)
        {
            //first check if there is entry for the same machine and domain, if so then update it, else insert a new one
            RegisterredNodes entityItem = GetOne(entity);
            if (entityItem != null)
            {
                entity.IsActive = true;
                entity.LastModifiedOn = DateTime.UtcNow;
                entity.RegisteredOn = DateTime.UtcNow;
                entity = Update(entity);
            }
            else
            {
                using (dbCon = new DataEntity())
                {
                    entity.IsActive = true;
                    if (entity.RegisteredOn == null || entity.RegisteredOn == DateTime.MinValue)
                    {
                        entity.RegisteredOn = DateTime.UtcNow;
                    }

                    dbCon.RegisterredNodes.Add(entity);
                    dbCon.SaveChanges();
                }
            }

            return entity;
        }

        public RegisterredNodes Update(RegisterredNodes entity)
        {
            RegisterredNodes entityItem = GetOne(entity);
            using (dbCon = new DataEntity())
            {
                dbCon.RegisterredNodes.Attach(entityItem);
                if (entity.LastModifiedOn == null || entity.LastModifiedOn == DateTime.MinValue)
                {
                    entity.LastModifiedOn = DateTime.UtcNow;
                }

                EntityExtension<RegisterredNodes>.ApplyOnlyChanges(entityItem, entity);

                dbCon.SaveChanges();
            }
            return entityItem;
        }

        public bool Delete(RegisterredNodes entity)
        {
            bool result = false;
            RegisterredNodes entityItem = GetOne(entity);
            using (dbCon = new DataEntity())
            {
                dbCon.RegisterredNodes.Attach(entityItem);
                entity.IsActive = false;
                //entity.PartitionKey = NodeState.InActive.ToString();
                entity.State = NodeState.InActive.ToString();
                if (entity.UnRegisteredOn == null || entity.UnRegisteredOn == DateTime.MinValue)
                {
                    entity.UnRegisteredOn = DateTime.UtcNow;
                }
                if (entity.LastModifiedOn == null || entity.LastModifiedOn == DateTime.MinValue)
                {
                    entity.LastModifiedOn = DateTime.UtcNow;
                }
                EntityExtension<RegisterredNodes>.ApplyOnlyChanges(entityItem, entity);

                dbCon.SaveChanges();
            }
            result = true;
            return result;
        }

        public IList<RegisterredNodes> InsertBatch(IList<RegisterredNodes> entities)
        {
            throw new NotImplementedException();
        }

        public IList<RegisterredNodes> UpdateBatch(IList<RegisterredNodes> entities)
        {
            throw new NotImplementedException();
        }

        public IList<RegisterredNodes> GetAll(RegisterredNodes Entity)
        {
            using (dbCon = new DataEntity())
            {
                List<RegisterredNodes> nodes = null;
                ActiveRegisteredNodesDS_Ext anodeDs = new ActiveRegisteredNodesDS_Ext();
                List<ActiveRegisteredNodes> aNodes = anodeDs.GetAll(Entity.PartitionKey).ToList();
                if (aNodes != null && aNodes.Count > 0)
                {
                    nodes = new List<RegisterredNodes>();
                    aNodes.ForEach(anode => {
                        nodes.Add(GetOne(new RegisterredNodes() { PartitionKey = anode.PartitionKey, RowKey = anode.RowKey }));
                    });
                }
                return nodes;
            }
        }

        #endregion
    }

    public enum NodeState
    {
        InActive=0,
        Active
    }
}

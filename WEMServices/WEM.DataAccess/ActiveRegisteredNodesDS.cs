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
    public class ActiveRegisteredNodesDS : IDataAccess.IEntity<ActiveRegisteredNodes>
    {
        public DataEntity dbCon;
        #region IEntity<ActiveRegisteredNodes> Members

        public ActiveRegisteredNodes GetOne(ActiveRegisteredNodes entity)
        {
            ActiveRegisteredNodes node = null;
            using (dbCon = new DataEntity())
            {
                node = dbCon.ActiveRegisteredNodes.Where(s => s.PartitionKey.ToLower().Equals(entity.PartitionKey.ToLower()) && s.RowKey.ToLower().Equals(entity.RowKey.ToLower())).FirstOrDefault();
            }
            return node;
        }

        public IQueryable<ActiveRegisteredNodes> GetAny()
        {
            throw new NotImplementedException();
        }

        public IList<ActiveRegisteredNodes> GetAll()
        {
            throw new NotImplementedException();
        }

        public ActiveRegisteredNodes Insert(ActiveRegisteredNodes entity)
        {
            using (dbCon = new DataEntity())
            {
                dbCon.ActiveRegisteredNodes.Add(entity);
                dbCon.SaveChanges();
            }

            return entity;
        }

        public ActiveRegisteredNodes Update(ActiveRegisteredNodes entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(ActiveRegisteredNodes entity)
        {
            bool result = false;
            //ActiveRegisteredNodes entityItem = GetOne(entity);
            //if (entityItem != null)
            //{
                using (dbCon = new DataEntity())
                {
                    dbCon.ActiveRegisteredNodes.Attach(entity);
                    dbCon.ActiveRegisteredNodes.Remove(entity);
                    dbCon.SaveChanges();
                }
            //}

            result = true;
            return result;
        }

        public IList<ActiveRegisteredNodes> InsertBatch(IList<ActiveRegisteredNodes> entities)
        {
            throw new NotImplementedException();
        }

        public IList<ActiveRegisteredNodes> UpdateBatch(IList<ActiveRegisteredNodes> entities)
        {
            throw new NotImplementedException();
        }

        public IList<ActiveRegisteredNodes> GetAll(ActiveRegisteredNodes Entity)
        {
            using (dbCon = new DataEntity())
            {
                List<ActiveRegisteredNodes> nodes = null;
                if (Entity != null && !string.IsNullOrEmpty(Entity.PartitionKey))
                {
                    nodes = dbCon.ActiveRegisteredNodes.Where(node => node.PartitionKey.ToLower() == Entity.PartitionKey.ToLower()).ToList();
                }
                return nodes;
            }
        }        

        #endregion
    }

    public class ActiveRegisteredNodesDS_Ext
    {
        public DataEntity dbCon;
        public IList<ActiveRegisteredNodes> GetAll(string domain)
        {
            using (dbCon = new DataEntity())
            {
                List<ActiveRegisteredNodes> nodes = null;
                if (!string.IsNullOrEmpty(domain))
                {
                    nodes = dbCon.ActiveRegisteredNodes.Where(node => node.PartitionKey.ToLower() == domain.ToLower()).ToList();
                }
                return nodes;
            }
        }
    }
}

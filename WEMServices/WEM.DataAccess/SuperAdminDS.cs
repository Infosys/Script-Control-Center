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

namespace Infosys.WEM.Resource.DataAccess
{
    public class SuperAdminDS : IDataAccess.IEntity<SuperAdmin>
    {
        public DataEntity dbCon;

        public bool IsSuperAdmin(SuperAdmin entity)
        {            
            using (dbCon = new DataEntity())
            {
                entity = entity.GeneratePartitionKeyAndRowKey();
                var admin = dbCon.SuperAdmin.FirstOrDefault(ent => ent.PartitionKey.Equals(entity.PartitionKey) && ent.RowKey.Equals(entity.RowKey));
                if (admin != null) return true;
            }
            return false;
        }

        public SuperAdmin GetOne(SuperAdmin Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<SuperAdmin> GetAny()
        {
            throw new NotImplementedException();
        }

        public IList<SuperAdmin> GetAll()
        {
            using (dbCon = new DataEntity())
            {
                return dbCon.SuperAdmin.ToList();
            }
        }

        public SuperAdmin Insert(SuperAdmin entity)
        {
            throw new NotImplementedException();
        }

        public SuperAdmin Update(SuperAdmin entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(SuperAdmin entity)
        {
            throw new NotImplementedException();
        }

        public IList<SuperAdmin> InsertBatch(IList<SuperAdmin> entities)
        {
            throw new NotImplementedException();
        }

        public IList<SuperAdmin> UpdateBatch(IList<SuperAdmin> entities)
        {
            throw new NotImplementedException();
        }

        public IList<SuperAdmin> GetAll(SuperAdmin Entity)
        {
            throw new NotImplementedException();
        }
    }

    public static class SuperAdminExtension
    {
        public static SuperAdmin GeneratePartitionKey(this SuperAdmin entity)
        {
            entity.PartitionKey = "IAP";
            return entity;

        }

        public static SuperAdmin GenerateRowKey(this SuperAdmin entity)
        {
            entity.RowKey = entity.CompanyId.ToString("00000") + ";" + entity.Alias;
            return entity;

        }

        public static SuperAdmin GeneratePartitionKeyAndRowKey(this SuperAdmin entity)
        {
            return entity.GeneratePartitionKey().GenerateRowKey();
        }

    

    }
}

/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.WEM.Export.Service.Contracts.Data;
using Infosys.WEM.Resource.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infosys.WEM.Resource.IDataAccess;

namespace Infosys.WEM.Resource.Export.DataAccess
{
    public class ExportServerDetailsDS : IDataAccess.IEntity<Infosys.WEM.Resource.Entity.ExportServerDetail>
    {
        public DataEntity dbCon;
        //public bool Delete(ServerDetail entity)
        //{
        //    throw new NotImplementedException();
        //}

        //public IList<ServerDetail> GetAll()
        //{
        //    throw new NotImplementedException();
        //}

        //public IList<ServerDetail> GetAll(ServerDetail Entity)
        //{
        //    throw new NotImplementedException();
        //}

        //public IQueryable<ServerDetail> GetAny()
        //{
        //    return dbCon.ServerDetails;
        //}

        //public ServerDetail GetOne(ServerDetail Entity)
        //{
        //    throw new NotImplementedException();
        //}

        //public ServerDetail Insert(ServerDetail entity)
        //{
        //    using (dbCon = new DataEntity())
        //    {
        //        var servers = this.GetAny();
        //        int? max = servers.Max(c => (int?)c.id);
        //        int idSeed = this.GetAny().Count();
        //        if (max.GetValueOrDefault() > idSeed)
        //            idSeed = max.GetValueOrDefault() + 1;
        //        else
        //            idSeed++;
        //        entity.id = idSeed;
        //        if (entity.CreatedOn == null || entity.CreatedOn == DateTime.MinValue)
        //        {
        //            entity.CreatedOn = DateTime.UtcNow;
        //        }
        //        dbCon.ServerDetails.Add(entity);
        //        dbCon.SaveChanges();
        //    }
        //    return entity;
        //}

        //public IList<ServerDetail> InsertBatch(IList<ServerDetail> entities)
        //{
        //    throw new NotImplementedException();
        //}

        //public ServerDetail Update(ServerDetail entity)
        //{
        //    throw new NotImplementedException();
        //}

        //public IList<ServerDetail> UpdateBatch(IList<ServerDetail> entities)
        //{
        //    throw new NotImplementedException();
        //}
        public bool Delete(ExportServerDetail entity)
        {
            throw new NotImplementedException();
        }

        public IList<ExportServerDetail> GetAll()
        {
            throw new NotImplementedException();
        }

        public IList<ExportServerDetail> GetAll(ExportServerDetail Entity)
        {
            List<ExportServerDetail> entity = null;

            using (dbCon = new DataEntity())
            {
                if (Entity.TargetSystemId > 0)
                    entity = dbCon.ExportServerDetails.Where(e => e.TargetSystemId == Entity.TargetSystemId).ToList();
                else if (Entity.id > 0)
                    entity = dbCon.ExportServerDetails.Where(e => e.id == Entity.id).ToList();
            }
            return entity;
        }

        public IQueryable<ExportServerDetail> GetAny()
        {
            return dbCon.ExportServerDetails ;
        }

        public ExportServerDetail GetOne(ExportServerDetail Entity)
        {
            ExportServerDetail entity = null;

            using (dbCon = new DataEntity())
            {
                entity = dbCon.ExportServerDetails.Where(e => e.TargetSystemId == Entity.TargetSystemId).FirstOrDefault();
            }
            return entity;
        }

        public ExportServerDetail Insert(ExportServerDetail entity)
        {
            ExportServerDetail existing = null;
            using (dbCon = new DataEntity())
            {
                //existing = dbCon.ExportServerDetails.Where(e => e.TargetSystemId == entity.TargetSystemId && e.CasServer.Equals(entity.CasServer) && e.DNSServer.Equals(entity.DNSServer) && e.CreatedBy.Equals(entity.CreatedBy)).FirstOrDefault();
                existing = dbCon.ExportServerDetails.Where(e => e.TargetSystemId == entity.TargetSystemId && e.CasServer.Equals(entity.CasServer) && e.DNSServer.Equals(entity.DNSServer)).FirstOrDefault();
                if (existing == null)
                {
                    var servers = this.GetAny();
                    int? max = servers.Max(c => (int?)c.id);
                    int idSeed = this.GetAny().Count();
                    if (max.GetValueOrDefault() > idSeed)
                        idSeed = max.GetValueOrDefault() + 1;
                    else
                        idSeed++;
                    entity.id = idSeed;
                    if (entity.CreatedOn == null || entity.CreatedOn == DateTime.MinValue)
                    {
                        entity.CreatedOn = DateTime.UtcNow;
                    }
                    dbCon.ExportServerDetails.Add(entity);
                    dbCon.SaveChanges();
                }
                else
                    entity = existing;
            }
            return entity;
        }

        public IList<ExportServerDetail> InsertBatch(IList<ExportServerDetail> entities)
        {
            throw new NotImplementedException();
        }

        public ExportServerDetail Update(ExportServerDetail entity)
        {
            throw new NotImplementedException();
        }

        public IList<ExportServerDetail> UpdateBatch(IList<ExportServerDetail> entities)
        {
            throw new NotImplementedException();
        }
    }
}

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

namespace Infosys.WEM.Resource.Export.DataAccess
{
    public class ExportTargetSystemDetailsDS : IDataAccess.IEntity<Infosys.WEM.Resource.Entity.ExportTargetSystemDetail>
    {
        public DataEntity dbCon;
        //public bool Delete(TargetSystemDetail entity)
        //{
        //    throw new NotImplementedException();
        //}

        //public IList<TargetSystemDetail> GetAll()
        //{
        //    using (dbCon = new DataEntity())
        //    {
        //        List<TargetSystemDetail> TargetSystems = dbCon.TargetSystemDetails.ToList<TargetSystemDetail>();
        //        return TargetSystems;
        //    }
        //}

        //public IList<TargetSystemDetail> GetAll(TargetSystemDetail Entity)
        //{
        //    throw new NotImplementedException();
        //}

        //public IQueryable<TargetSystemDetail> GetAny()
        //{
        //    throw new NotImplementedException();
        //}

        //public TargetSystemDetail GetOne(TargetSystemDetail Entity)
        //{
        //    throw new NotImplementedException();
        //}

        //public TargetSystemDetail Insert(TargetSystemDetail entity)
        //{
        //    throw new NotImplementedException();
        //}

        //public IList<TargetSystemDetail> InsertBatch(IList<TargetSystemDetail> entities)
        //{
        //    throw new NotImplementedException();
        //}

        //public TargetSystemDetail Update(TargetSystemDetail entity)
        //{
        //    throw new NotImplementedException();
        //}

        //public IList<TargetSystemDetail> UpdateBatch(IList<TargetSystemDetail> entities)
        //{
        //    throw new NotImplementedException();
        //}
        public bool Delete(ExportTargetSystemDetail entity)
        {
            throw new NotImplementedException();
        }

        public IList<ExportTargetSystemDetail> GetAll()
        {
            using (dbCon = new DataEntity())
            {
                //dbCon.Set<ExportTargetSystemDetail>().OrderByDescending(item => item.DefaultType);
                List<ExportTargetSystemDetail> TargetSystems = dbCon.ExportTargetSystemDetails.ToList<ExportTargetSystemDetail>();
                return TargetSystems;
            }
        }

        public IList<ExportTargetSystemDetail> GetAll(ExportTargetSystemDetail Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<ExportTargetSystemDetail> GetAny()
        {
            throw new NotImplementedException();
        }

        public ExportTargetSystemDetail GetOne(ExportTargetSystemDetail Entity)
        {
            throw new NotImplementedException();
        }

        public ExportTargetSystemDetail Insert(ExportTargetSystemDetail entity)
        {
            throw new NotImplementedException();
        }

        public IList<ExportTargetSystemDetail> InsertBatch(IList<ExportTargetSystemDetail> entities)
        {
            throw new NotImplementedException();
        }

        public ExportTargetSystemDetail Update(ExportTargetSystemDetail entity)
        {
            throw new NotImplementedException();
        }

        public IList<ExportTargetSystemDetail> UpdateBatch(IList<ExportTargetSystemDetail> entities)
        {
            throw new NotImplementedException();
        }
    }
}

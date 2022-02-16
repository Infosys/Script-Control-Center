/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.WEM.Resource.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.WEM.Resource.DataAccess
{
    public class ReferenceDataDS : IDataAccess.IEntity<ReferenceData>
    {
        public DataEntity dbCon;
        public bool Delete(ReferenceData entity)
        {
            throw new NotImplementedException();
        }

        public IList<ReferenceData> GetAll()
        {
            using (dbCon = new DataEntity())
            {
                List<ReferenceData> referenceData =
                  (from ReferenceData in dbCon.ReferenceData
                   where ReferenceData.IsActive == true
                   select ReferenceData).ToList<ReferenceData>();
                return referenceData;
            }
        }

        public IList<ReferenceData> GetAll(ReferenceData Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<ReferenceData> GetAny()
        {
            throw new NotImplementedException();
        }

        public ReferenceData GetOne(ReferenceData Entity)
        {
            throw new NotImplementedException();
        }

        public ReferenceData Insert(ReferenceData entity)
        {
            using (dbCon = new DataEntity())
            {
                if(string.IsNullOrEmpty(entity.PartitionKey))
                    entity.PartitionKey = Convert.ToInt32(ConfigurationManager.AppSettings["Company"]).ToString("00000");
                if (string.IsNullOrEmpty(entity.RowKey))
                    entity.RowKey = Convert.ToInt32(ConfigurationManager.AppSettings["Company"]).ToString("00000");
                if (entity.CreatedOn == null || entity.CreatedOn == DateTime.MinValue)
                {
                    entity.CreatedOn = DateTime.UtcNow;
                    entity.Timestamp = DateTime.UtcNow;
                }
                dbCon.ReferenceData.Add(entity);
                dbCon.SaveChanges();
            }
            return entity;
        }

        public IList<ReferenceData> InsertBatch(IList<ReferenceData> entities)
        {
            throw new NotImplementedException();
        }

        public ReferenceData Update(ReferenceData entity)
        {
            throw new NotImplementedException();
        }

        public IList<ReferenceData> UpdateBatch(IList<ReferenceData> entities)
        {
            throw new NotImplementedException();
        }
    }

    public class ReferenceDataDSExt
    {
        public DataEntity dbCon;
        public bool IsDuplicateReferenceData(ReferenceData referenceData)
        {
            bool isDuplicate = false;
            using (dbCon = new DataEntity())
            {
                List<ReferenceData> presentReferences =
                    (from ReferenceData in dbCon.ReferenceData
                     where ReferenceData.ReferenceType == referenceData.ReferenceType && ReferenceData.ReferenceKey == referenceData.ReferenceKey
                     select ReferenceData).ToList();
                foreach (ReferenceData referencedata in presentReferences)
                {
                    if (referenceData.ReferenceKey.ToLower() == referencedata.ReferenceKey.ToLower()&& referenceData.ReferenceType.ToLower() == referencedata.ReferenceType.ToLower())
                    {
                        isDuplicate = true;
                        break;
                    }
                }
            }
            return isDuplicate;
        }

    }
}

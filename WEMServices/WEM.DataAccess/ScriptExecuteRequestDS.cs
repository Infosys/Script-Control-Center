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
namespace Infosys.WEM.Resource.DataAccess
{
    public class ScriptExecuteRequestDS : IDataAccess.IEntity<ScriptExecuteRequest>
    {
        public DataEntity dbCon;

        #region IEntity<ScriptExecuteRequest> Members
        
        public ScriptExecuteRequest Insert(ScriptExecuteRequest entity)
        {
            using (dbCon = new DataEntity())
            {
                if (entity.createddate == null || entity.createddate == DateTime.MinValue)
                {
                    entity.createddate = DateTime.UtcNow;
                }

                //entity.Priority to be used in future to set the order or execution

                dbCon.ScriptExecuteRequest.Add(entity);
                dbCon.SaveChanges();
            }

            return entity;
        }       

        public ScriptExecuteRequest Update(ScriptExecuteRequest entity)
        {
            ScriptExecuteRequest entityInDb = GetOne(entity);
            if (entityInDb != null)
            {                
                   using (dbCon = new DataEntity())
                    {
                        dbCon.ScriptExecuteRequest.Attach(entityInDb);
                        if (entity.modifieddate == null || entity.modifieddate == DateTime.MinValue)
                        {
                            entity.modifieddate = DateTime.UtcNow;
                        }

                        EntityExtension<ScriptExecuteRequest>.ApplyOnlyChanges(entityInDb, entity);
                        dbCon.SaveChanges();
                    }

                
                    ScriptExecuteRequest activity = new ScriptExecuteRequest()
                    {
                        transactionId = entity.transactionId,
                        scriptid = entity.scriptid,
                        categoryid = entity.categoryid,
                        scriptname = entity.scriptname,
                        inparams = entity.inparams,
                        username = entity.username,
                        remoteservernames = entity.remoteservernames,
                        remoteexecutionhost = entity.remoteexecutionhost,
                        executionmodetype = entity.executionmodetype,
                        scheduledpattern = entity.scheduledpattern,
                        schedulestartdatetime = entity.schedulestartdatetime,
                        scheduleenddatetime = entity.scheduleenddatetime,
                        scheduleoccurences = entity.scheduleoccurences,
                        schedulepriority = entity.schedulepriority,
                        scheduleonclusters = entity.scheduleonclusters,
                        iapnodetransport = entity.iapnodetransport,
                        domain = entity.domain,
                        referencekey = entity.referencekey,
                        path = entity.path,
                        createdby = entity.createdby,
                        createddate = entity.createddate,
                        modifiedby = entity.modifiedby,
                        modifieddate = entity.modifieddate
                    };                    

                    var actDe = new ScriptExecuteRequestDS().Insert(activity);
            }

            return entityInDb;
        }

        public IList<ScriptExecuteRequest> GetAll()
        {
            using (dbCon = new DataEntity())
                return dbCon.ScriptExecuteRequest.ToList();
        }       

        public ScriptExecuteRequest GetOne(ScriptExecuteRequest Entity)
        {  
            using (dbCon = new DataEntity())
            {
                return dbCon.ScriptExecuteRequest.FirstOrDefault(sc => sc.transactionId == Entity.transactionId);
            }
        }

        public IList<ScriptExecuteRequest> GetAll(ScriptExecuteRequest Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<ScriptExecuteRequest> GetAny()
        {
            throw new NotImplementedException();
        }
        public IList<ScriptExecuteRequest> InsertBatch(IList<ScriptExecuteRequest> entities)
        {
            throw new NotImplementedException();
        }

        public IList<ScriptExecuteRequest> UpdateBatch(IList<ScriptExecuteRequest> entities)
        {
            throw new NotImplementedException();
        }

        public bool Delete(ScriptExecuteRequest entity)
        {
            throw new NotImplementedException();
        }

        #endregion

    }

    public class ScriptExecuteRequestDSExt
    {
        public DataEntity dbCon;

        public bool IsDuplicate(string transactionId)
        {

            bool isDuplicate = false;
            using (dbCon = new DataEntity())
            {
                using (LogHandler.TraceOperations("Document.ScriptExecuteRequestDS:isDuplicate", LogHandler.Layer.Resource, System.Guid.Empty, null))
                {
                    List<ScriptExecuteRequest> scriptexecuterequests =
                  (from scriptexecuterequesttable in dbCon.ScriptExecuteRequest
                   where scriptexecuterequesttable.transactionId.ToString().ToLower() == transactionId.ToString().ToLower()
                   select scriptexecuterequesttable).ToList<ScriptExecuteRequest>();
                    if (scriptexecuterequests.Count != 0)
                    {
                        isDuplicate = true;
                    }

                    /*foreach (ScriptExecuteRequest request in scriptexecuterequests)
                     {
                         if (request.transactionId.ToString().ToLower() == transactionId.ToString().ToLower())
                         {
                             isDuplicate = true;
                             break;
                         }
                     }*/
                }
                
            }

            return isDuplicate;
        }

        public List<ScriptExecuteResponse> getTransactionStatus(string transactionId)
        {
            try
            {
                using (dbCon = new DataEntity())
                {
                    Guid transactionid = Guid.Parse(transactionId);
                    //return dbCon.ScriptExecuteResponse.Where(sc => sc.transactionId == transactionId).ToList();
                    return dbCon.ScriptExecuteResponse.Where(sc => sc.transactionId == Guid.Parse(transactionId)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<TransactionStatusResponse> getTransactionsByStatus(string status,string referenceKey)
        {
            try
            {
                using (dbCon = new DataEntity())
                {                    

                    //var query = dbCon.ReferenceData.Where(rd=>rd.ReferenceType==referenceKey)
                    //            .GroupBy(c => c.PartitionKey)
                    //            .Select(g => new {
                    //                PartitionKey = g.Key,
                    //                findByIdUrl = g.Where(c => c.ReferenceKey == "findByIdUrl").Max(c => c.ReferenceValue),
                    //                NiaServiceAccount = g.Where(c => c.ReferenceKey == "NiaServiceAccount").Max(c => c.ReferenceValue),
                    //                NiaServiceAccPassword = g.Where(c => c.ReferenceKey == "NiaServiceAccPassword").Max(c => c.ReferenceValue)
                    //            }).ToList();

                    var results_tmp = dbCon.ScriptExecuteResponse
                                .Join(
                                    dbCon.ScriptExecuteRequest,
                                    response => response.transactionId,
                                    request => request.transactionId,
                                    (response, request) => new { ScriptExecuteResponse = response, ScriptExecuteRequest = request })
                                .Join(
                                    dbCon.ReferenceData.Where(rd => rd.ReferenceType == referenceKey)
                                    .GroupBy(c => c.PartitionKey)
                                    .Select(g => new {
                                        PartitionKey = g.Key,                                        
                                        NiaServiceAccount = g.Where(c => c.ReferenceKey == "NiaServiceAccount").Max(c => c.ReferenceValue),
                                        NiaServiceAccPassword = g.Where(c => c.ReferenceKey == "NiaServiceAccPassword").Max(c => c.ReferenceValue),
                                        casServerUrl = g.Where(c => c.ReferenceKey == "casServerUrl").Max(c => c.ReferenceValue),
                                        casServiceUrl = g.Where(c => c.ReferenceKey == "casServiceUrl").Max(c => c.ReferenceValue),
                                        niaEcrScriptExecuteUrl = g.Where(c => c.ReferenceKey == "niaEcrScriptExecuteUrl").Max(c => c.ReferenceValue),
                                        niaEcrFindByActivityIdUrl = g.Where(c => c.ReferenceKey == "niaEcrFindByActivityIdUrl").Max(c => c.ReferenceValue),
                                        niaEcrFindAllNodesUrl = g.Where(c => c.ReferenceKey == "niaEcrFindAllNodesUrl").Max(c => c.ReferenceValue),
                                        serviceAreas = g.Where(c => c.ReferenceKey == "serviceAreas").Max(c => c.ReferenceValue),
                                        findbyIdUrl = g.Where(c => c.ReferenceKey == "findByIdUrl").Max(c => c.ReferenceValue)
                                    }),
                                    sre => sre.ScriptExecuteRequest.PartitionKey,
                                    reference => reference.PartitionKey,
                                    (sre, reference) => new { sre.ScriptExecuteResponse, sre.ScriptExecuteRequest,reference })
                                    .Where(se => se.ScriptExecuteRequest.referencekey == referenceKey && se.ScriptExecuteResponse.currentstate == status)
                                .Select(c => new TransactionStatusResponse()
                                {
                                   CompanyId = c.ScriptExecuteResponse.PartitionKey,
                                    //TransactionId = new Guid(c.ScriptExecuteResponse.transactionId),
                                   TransactionId = c.ScriptExecuteResponse.transactionId.Value,
                                   CurrentState = c.ScriptExecuteResponse.currentstate,
                                   ComputerName= c.ScriptExecuteResponse.computername,
                                   SourceTransactionId= c.ScriptExecuteResponse.SourceTransactionId,
                                   NiaServiceAccount= c.reference.NiaServiceAccount,
                                   NiaServiceAccPassword= c.reference.NiaServiceAccPassword,
                                   casServerUrl= c.reference.casServerUrl,
                                   casServiceUrl= c.reference.casServiceUrl,
                                   niaEcrScriptExecuteUrl= c.reference.niaEcrScriptExecuteUrl,
                                   niaEcrFindByActivityIdUrl= c.reference.niaEcrFindByActivityIdUrl,
                                   niaEcrFindAllNodesUrl= c.reference.niaEcrFindAllNodesUrl,
                                   serviceAreas= c.reference.serviceAreas,
                                   findbyIdUrl = c.reference.findbyIdUrl
                                }).ToList();

                    return results_tmp;

                    //return dbCon.ScriptExecuteResponse.Where(sc => sc.currentstate == status).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public class TransactionStatusResponse
    {         
        public string CompanyId { get; set; }
        public Guid TransactionId { get; set; }
        public string CurrentState { get; set; }
        public string ComputerName { get; set; }
        public string SourceTransactionId { get; set; }
        public string findbyIdUrl { get; set; }
        public string NiaServiceAccount { get; set; }
        public string NiaServiceAccPassword { get; set; }
        public string casServerUrl { get; set; }
        public string casServiceUrl { get; set; }
        public string niaEcrScriptExecuteUrl { get; set; }
        public string niaEcrFindByActivityIdUrl { get; set; }
        public string niaEcrFindAllNodesUrl { get; set; }
        public string serviceAreas { get; set; }
    }
}

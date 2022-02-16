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
using Infosys.WEM.Infrastructure.Common;
using Infosys.WEM.Node.Service.Contracts;
using Infosys.WEM.Node.Service.Contracts.Message;
using Infosys.WEM.Resource.DataAccess;
using Infosys.WEM.SecurityAccess.Contracts.Data;
using System.ServiceModel.Activation;
using System.Runtime.Caching;

namespace Infosys.WEM.Service.Implementation
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public abstract class Transaction_ServiceBase : ITransaction
    {
        public virtual LogTransactionResMsg LogTransaction(LogTransactionReqMsg value)
        {
            return null;
        }

        public virtual GetTransactionsResMsg GetTransactions(GetTransactionsReqMsg value)
        {
            return null;
        }
    }

    public partial class Transaction : Transaction_ServiceBase
    {
        string compCategoriesCacheKey = "CATEGORIES_IN_COMPANY";

        public override LogTransactionResMsg LogTransaction(LogTransactionReqMsg value)
        {
            //Block 1
            DateTime processStartedTime1 = DateTime.Now;
            LogTransactionResMsg response = new LogTransactionResMsg();
            if (value != null && value.Request != null && !string.IsNullOrEmpty(value.Request.ModuleId))
            {
                try
                {
                    TransactionDS tranDS = new TransactionDS();
                    var entity = Translators.TransactionSE_DE.TransactionDEToSE(tranDS.Insert(Translators.TransactionSE_DE.TransactionSEToDE(value.Request)));
                    if (entity != null)
                    {
                        response.IsSuccess = true;
                        response.InstanceId = entity.InstanceId;
                    }
                }
                catch (Exception wemScriptException)
                {
                    response.IsSuccess = false;
                    response.AdditonalInfo = wemScriptException.Message;
                    Exception ex = new Exception();
                    bool rethrow = ExceptionHandler.HandleException(wemScriptException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                    if (rethrow)
                    {
                        throw ex;
                    }
                }
            }
            else
            {
                response.IsSuccess = false;
                response.AdditonalInfo = "Invalid input.";
            }
            LogHandler.LogError(string.Format("Time taken by Transaction:Block 1 (LogTransaction) : {0}", DateTime.Now.Subtract(processStartedTime1).TotalSeconds), LogHandler.Layer.Business, null);

            return response;
        }

        public override GetTransactionsResMsg GetTransactions(GetTransactionsReqMsg value)
        {
            GetTransactionsResMsg response = new GetTransactionsResMsg();
            if (value != null && value.FilterCriteria != null)
            {
                try
                {
                    if (value.FilterCriteria.CategoryId != 0)
                    {
                        if (CheckRequestorAccess(value.FilterCriteria.CompanyId, value.FilterCriteria.CategoryId))
                        {
                            TransactionDS tranDS = new TransactionDS();
                            response.Transactions = Translators.TransactionSE_DE.TransactionListDEToSE(tranDS.GetAllMatching(new Resource.Entity.TransactionInstance() { CategoryId = value.FilterCriteria.CategoryId }, value.FilterCriteria.StartDate, value.FilterCriteria.EndDate));
                            response.IsSuccess = true;
                        }
                        else
                        {
                            response.AdditionalInfo = string.Format("Requestor doesnt have the role of Manager or Analyst for category with id -{0}", value.FilterCriteria.CategoryId);
                            response.IsSuccess = false;
                        }
                    }
                    else
                    {
                        //we need to get the details for all the categories to which the requester has access as manager or analyst
                        UserDS userDS = new UserDS();

                        IList<Infosys.WEM.Resource.Entity.User> users = null;
                        List<User> usersSe = null;
                        List<User> rightUsers = null;
                        string requestor = Infosys.WEM.Infrastructure.Common.Utility.GetLoggedInUser(true);                                            
                        var superadmin = new SecurityAccess().IsSuperAdmin(Infosys.WEM.Infrastructure.SecurityCore.SecureData.Secure(requestor, ApplicationConstants.SECURE_PASSCODE), value.FilterCriteria.CompanyId.ToString());
                        if (superadmin != null && superadmin.IsSuperAdmin)
                            users = userDS.GetAll(value.FilterCriteria.CompanyId);
                        else
                            users = userDS.GetAll(new Resource.Entity.User() { Alias = requestor });   
                        
                        usersSe = Translators.UserSE_DE.UserDEListtoSEList(users as List<Infosys.WEM.Resource.Entity.User>);
                        usersSe = usersSe.Where(u => u.IsActive == true).ToList();

                        if (superadmin != null && superadmin.IsSuperAdmin)
                            rightUsers = usersSe;
                        else
                            rightUsers = usersSe.Where(u => u.Role == 2 || u.Role == 3).ToList();                       
                            

                        if (rightUsers != null && rightUsers.Count > 0)
                        {
                            response.Transactions = new List<Node.Service.Contracts.Data.Transaction>();
                            TransactionDS tranDS = new TransactionDS();
                            rightUsers.Select(x => x.CategoryId).Distinct().ToList().ForEach(ru =>
                            {
                                List<Node.Service.Contracts.Data.Transaction> transactions = Translators.TransactionSE_DE.TransactionListDEToSE(tranDS.GetAllMatching(new Resource.Entity.TransactionInstance() { CategoryId = ru }, value.FilterCriteria.StartDate, value.FilterCriteria.EndDate));

                                if (transactions != null)
                                    response.Transactions.AddRange(transactions);
                            });
                            response.IsSuccess = true;
                        }
                    }

                    //first check if it is in the memory cache                    
                    ObjectCache memCache = MemoryCache.Default;
                    var categories = new List<Common.Contracts.Data.Category>();
                    if (memCache.Contains(compCategoriesCacheKey))
                    {
                        categories = memCache.Get(compCategoriesCacheKey) as List<Common.Contracts.Data.Category>;
                    }
                    else
                    {
                        if (response != null && response.Transactions != null && response.Transactions.Count > 0)
                        {
                            Scripts.Resource.DataAccess.CategoryDSExt catds = new Scripts.Resource.DataAccess.CategoryDSExt();
                            Dictionary<int, int> outValue = new Dictionary<int, int>();
                            categories = Translators.Common.CategorySE_DE.CategoryDEListtoSEList(catds.GetAllCategories(value.FilterCriteria.CompanyId.ToString(), "",out outValue).ToList());
                            //then add it to the memory cache
                            if (categories != null && categories.Count > 0)
                            {
                                CacheItemPolicy cachePolicy = new CacheItemPolicy();
                                cachePolicy.AbsoluteExpiration = DateTime.Now.AddMinutes(10.0);
                                memCache.Add(compCategoriesCacheKey, categories, cachePolicy);

                            }
                        }
                    }

                    if (categories != null && categories.Count > 0)
                    {
                        //group by category id
                        var groups = response.Transactions.GroupBy(t => t.CategoryId).ToList();
                        groups.ForEach(g =>
                        {
                            foreach (var category in categories)
                            {
                                if (category.CategoryId == g.Key)
                                {
                                    g.ToList().ForEach(t => t.CategoryName = category.Name);
                                    break;
                                }
                            }
                        });
                    }
                }
                catch (Exception wemScriptException)
                {
                    response.AdditionalInfo = wemScriptException.Message;
                    response.IsSuccess = false;
                    Exception ex = new Exception();
                    bool rethrow = ExceptionHandler.HandleException(wemScriptException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                    if (rethrow)
                    {
                        throw ex;
                    }
                }
            }
            return response;
        }

        /// <summary>
        /// To check if the requestor has access to the category
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        private bool CheckRequestorAccess(int companyId, int categoryId)
        {
            bool hasAccess = false;
            string requestor = Infosys.WEM.Infrastructure.Common.Utility.GetLoggedInUser();

            //first check if the user is super admin, then allow
            //else check if the user is analyst or manager
            var superadmin = new SecurityAccess().IsSuperAdmin(Infosys.WEM.Infrastructure.SecurityCore.SecureData.Secure(requestor, ApplicationConstants.SECURE_PASSCODE), companyId.ToString());
            if (superadmin != null && superadmin.IsSuperAdmin)
            {
                hasAccess = true;
            }
            else
            {
                //the below code taken from SecurityAccess -> GetAnyUser
                UserDS userDS = new UserDS();
                var users = userDS.GetAnyUser(new Resource.Entity.User
                {
                    CategoryId = categoryId,
                    CompanyId = companyId
                }).Select(u => new User
                {
                    Alias = u.Alias,
                    CompanyId = u.CompanyId,
                    //CreatedBy = u.CreatedBy,
                    DisplayName = u.DisplayName,
                    CategoryId = u.CategoryId.GetValueOrDefault(),
                    IsActive = u.IsActive.GetValueOrDefault(),
                    //LastModifiedBy = u.LastModifiedBy,
                    Role = u.Role,
                    UserId = u.Id,
                    IsDL = u.IsDL.GetValueOrDefault(),
                    GroupId = u.GroupId

                }).ToList();
                //till here code taken from SecurityAccess -> GetAnyUser

                if (users != null && users.Count > 0)
                {
                    for (int i = 0; i < users.Count; i++)
                    {
                        if (requestor.ToLower().Contains(users[i].Alias.ToLower()) && (users[i].Role == 2 || users[i].Role == 3)) //2,3- role of Manager or Analyst
                        {
                            hasAccess = true;
                            break;
                        }
                    }
                }
            }
            return hasAccess;
        }
    }
}

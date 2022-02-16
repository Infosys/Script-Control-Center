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
using System.ServiceModel.Activation;

using Infosys.WEM.Infrastructure.Common;
using Infosys.WEM.Resource.DataAccess;
using Infosys.WEM.Service.Common.Contracts;
using Infosys.WEM.Service.Common.Contracts.Message;
using SE = Infosys.WEM.Service.Common.Contracts.Message;
using DE = Infosys.WEM.Resource.Entity;
using Infosys.WEM.Scripts.Resource.DataAccess;
using System.Runtime.Caching;
using Infosys.WEM.Service.Common.Contracts.Data;

namespace Infosys.WEM.Service.Implementation
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public abstract class CommonRepository_ServiceBase : ICommonRepository
    {
        public virtual GetCompanyResMsg GetCompanyDetails(string companyId)
        {
            return null;
        }

        public virtual GetAllCategoriesResMsg GetAllCategoriesByCompany(string companyId, string module)
        {
            return null;
        }

        public virtual AddCategoryResMsg AddCategory(AddCategoryReqMsg value)
        {
            return null;
        }

        public virtual UpdateCategoryResMsg UpdateCategory(UpdateCategoryReqMsg value)
        {
            return null;
        }

        public virtual DeleteCategoryResMsg DeleteCategory(DeleteCategoryReqMsg value)
        {
            return null;
        }

        public virtual GetAllModulesResMsg GetAllModules()
        {
            return null;
        }

        public virtual GetAllClustersByCategoryResMsg GetAllClustersByCategory(string categoryId)
        {
            return null;
        }


        public virtual GetAllCategoriesResMsg GetAllCategoriesWithData(string companyId, string module)
        {
            return null;
        }

        public virtual GetAllCategoriesResMsg GetAllCategoriesByCompanyCategoryId(string companyId, string module, string categoryId)
        {
            return null;
        }

        public virtual AddReferenceDataResMsg AddReferenceData(AddReferenceDataReqMsg value)
        {
            return null;
        }

        public virtual GetReferenceDataResMsg GetReferenceData(GetReferenceDataReqMsg value)
        {
            return null;
        }
    }

    public partial class CommonRepository : CommonRepository_ServiceBase
    {
        string compCategoriesCacheKey = "CATEGORIES_IN_COMPANY";
        public override GetCompanyResMsg GetCompanyDetails(string companyId)
        {
            GetCompanyResMsg resp = null;

            try
            {
                CompaniesDS companyDS = new CompaniesDS();
                Infosys.WEM.Service.Common.Contracts.Data.Company company = new Infosys.WEM.Service.Common.Contracts.Data.Company();

                var resposne = companyDS.GetOne(new DE.Companies { CompanyId = Convert.ToInt32(companyId) });

                company.CompanyId = resposne.CompanyId;
                company.StorageBaseUrl = resposne.StorageBaseUrl;
                company.DeploymentBaseUrl = resposne.DeploymentBaseUrl;
                company.RemoteShareUrl = resposne.RemoteShareUrl;
                company.EnableSecureTransactions = resposne.EnableSecureTransactions;

                resp = new GetCompanyResMsg
                {
                    Company = company
                };
            }

            catch (Exception wemException)
            {
                Exception ex = new Exception();

                bool rethrow = ExceptionHandler.HandleException(wemException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }

            return resp;
        }

        public override GetAllCategoriesResMsg GetAllCategoriesByCompanyCategoryId(string companyId, string moduleId,string categoryId)
        {
            GetAllCategoriesResMsg response = new GetAllCategoriesResMsg();
            Dictionary<int, int> numberOfScripts = new Dictionary<int, int>();
            try
            {
                CategoryDSExt categoryDsExt = new CategoryDSExt();
                response.Categories = Translators.Common.CategorySE_DE.CategoryDEListtoSEList(categoryDsExt.GetAllCategories(companyId, moduleId, out numberOfScripts, categoryId).ToList());
                response.Categories.ForEach(cat =>
                {
                    cat.NumberOfScripts = (numberOfScripts.ContainsKey(cat.CategoryId)) ? numberOfScripts[cat.CategoryId] : 0;
                });
            }
            catch (Exception wemScriptException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemScriptException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        public override GetAllCategoriesResMsg GetAllCategoriesByCompany(string companyId, string moduleId)
        {
            GetAllCategoriesResMsg response = new GetAllCategoriesResMsg();
            Dictionary<int, int> numberOfScripts = new Dictionary<int, int>(); 
            try
            {
                CategoryDSExt categoryDsExt = new CategoryDSExt();
                response.Categories = Translators.Common.CategorySE_DE.CategoryDEListtoSEList(categoryDsExt.GetAllCategories(companyId, moduleId,out numberOfScripts).ToList());
                response.Categories.ForEach(cat=>
                {
                    cat.NumberOfScripts = (numberOfScripts.ContainsKey(cat.CategoryId)) ? numberOfScripts[cat.CategoryId]:0;
                });
            }
            catch (Exception wemScriptException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemScriptException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        List<DE.Script> scripts;
        List<DE.WorkflowMaster> wfs;
        List<Infosys.WEM.Service.Common.Contracts.Data.Category> categories;
        List<Infosys.WEM.Service.Common.Contracts.Data.Category> tempcategories;

        public override GetAllCategoriesResMsg GetAllCategoriesWithData(string companyId, string moduleId)
        {
            GetAllCategoriesResMsg response = new GetAllCategoriesResMsg();
            try
            {
                CategoryDSExt categoryDsExt = new CategoryDSExt();
                //response.Categories = Translators.Common.CategorySE_DE.CategoryDEListtoSEList(categoryDsExt.GetAllCategories(companyId, moduleId).ToList());
                Dictionary<int, int> outDict = new Dictionary<int, int>();
                categories = Translators.Common.CategorySE_DE.CategoryDEListtoSEList(categoryDsExt.GetAllCategories(companyId, moduleId, out outDict).ToList());
                tempcategories = Translators.Common.CategorySE_DE.CategoryDEListtoSEList(categoryDsExt.GetAllCategories(companyId, moduleId, out outDict).ToList());

                if (moduleId == "2")
                {
                    CleanScripts();
                }
                else if (moduleId =="3")
                {
                    CleanWF();
                }
               

                response.Categories = categories;

            }
            catch (Exception wemScriptException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemScriptException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        private void CleanWF()
        {
            WorkflowMasterDS wfMaster = new WorkflowMasterDS();
            wfs = wfMaster.GetAll().ToList();

            for (int i = 0; i < tempcategories.Count; i++)
            {
                var c = tempcategories[i];

                var child = tempcategories.FirstOrDefault(c1 => c.CategoryId == c1.ParentId);

                object hasWfs = null;

                if (child != null)
                {
                    hasWfs = wfs.FirstOrDefault(s => s.CategoryId == child.CategoryId);
                }
                else
                {
                    hasWfs = wfs.FirstOrDefault(s => s.CategoryId == c.CategoryId);
                }

                if (hasWfs == null)
                {
                    var t = categories.FirstOrDefault(c1 => c1.CategoryId == c.CategoryId);
                    categories.Remove(t);
                    RemoveWfParents(c);
                }
            }
        }

        private void CleanScripts()
        {
            ScriptDS scriptDs = new ScriptDS();
            scripts = scriptDs.GetAll().ToList();

            for (int i = 0; i < tempcategories.Count; i++)
            {
                var c = tempcategories[i];

                var child = tempcategories.FirstOrDefault(c1 => c.CategoryId == c1.ParentId);

                object hasScripts = null;

                if (child != null)
                {
                    hasScripts = scripts.FirstOrDefault(s => s.CategoryId == child.CategoryId);
                }
                else
                {
                    hasScripts = scripts.FirstOrDefault(s => s.CategoryId == c.CategoryId);
                }

                if (hasScripts == null)
                {
                    var t = categories.FirstOrDefault(c1 => c1.CategoryId == c.CategoryId);
                    categories.Remove(t);
                    RemoveScriptParents(c);
                }
            }
        }

        private void RemoveScriptParents(Common.Contracts.Data.Category c)
        {
            var parent = tempcategories.FirstOrDefault(p => c.ParentId == p.CategoryId);

            if (parent != null)
            {
                var parentHasScripts = scripts.FirstOrDefault(s => s.CategoryId == parent.CategoryId);
                var children = tempcategories.FirstOrDefault(c1 => c1.ParentId == parent.CategoryId);

                if (parentHasScripts == null && children == null)
                {
                    var t = categories.FirstOrDefault(c1 => c1.CategoryId == parent.CategoryId);
                    categories.Remove(t);
                    RemoveScriptParents(parent);
                }
            }

        }

        private void RemoveWfParents(Common.Contracts.Data.Category c)
        {
            var parent = tempcategories.FirstOrDefault(p => c.ParentId == p.CategoryId);

            if (parent != null)
            {
                var parentHasWfs = wfs.FirstOrDefault(s => s.CategoryId == parent.CategoryId);
                var children = tempcategories.FirstOrDefault(c1 => c1.ParentId == parent.CategoryId);

                if (parentHasWfs == null && children == null)
                {
                    var t = categories.FirstOrDefault(c1 => c1.CategoryId == parent.CategoryId);
                    categories.Remove(t);
                    RemoveWfParents(parent);
                }
            }

        }

        public override GetAllClustersByCategoryResMsg GetAllClustersByCategory(string categoryId)
        {
            GetAllClustersByCategoryResMsg response = new GetAllClustersByCategoryResMsg();

            if (String.IsNullOrEmpty(categoryId))
                categoryId = "0";

            try
            {
                if (Security.Access.Check(categoryId))
                {
                    SemanticCategoryDS scDS = new SemanticCategoryDS();

                    var result = scDS.GetAll(new DE.SemanticCategory { CategoryId = Convert.ToInt32(categoryId) }).Where(c => c.IsActive == true).ToList();

                    var semanticnodes = new SemanticNodeClusterDS().GetAll();

                    if (result != null || result.Count > 0)
                    {
                        List<SemanticNode> nodes = new List<SemanticNode>();
                        result.ToList().ForEach(n =>
                        {
                            SemanticNode node = new SemanticNode();
                            node.Id = n.SemanticClusterId;
                            node.Name = n.SemanticClusterName;
                            node.Nodes = semanticnodes.Where(c => c.PartitionKey == n.SemanticClusterId).Select(
                                sc => sc.IapNodeId).ToList();
                            nodes.Add(node);
                        });
                        response.Nodes = nodes;
                    }
                }
            }
            catch (Exception wemScriptException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemScriptException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        public override AddCategoryResMsg AddCategory(AddCategoryReqMsg value)
        {
            AddCategoryResMsg response = new AddCategoryResMsg();
            try
            {
                //first check if the category or subcategory namne is duplicate


                CategoryDSExt categoryDsExt = new CategoryDSExt();
                CategoryDS categoryDs = new CategoryDS();
                List<DE.Category> categories = Translators.Common.CategorySE_DE.CategorySEListtoDEList(value.Categories);

                bool isDuplicate = false;
                foreach (DE.Category category in categories)
                {
                    if (Checkccess(category))
                    {
                        if (category.PartitionKey == category.CompanyId + "_ROOT")
                        {
                            isDuplicate = categoryDsExt.IsDuplicateCategory(category);
                        }
                        else
                        {
                            isDuplicate = categoryDsExt.IsDuplicateSubCategory(category);
                        }
                        if (isDuplicate)
                            break;
                        categoryDs.Insert(category);
                    }
                    else
                        throw new Exception("You do not have access to this category");
                }
                if (isDuplicate)
                    throw new Exception("One or more provided category/sub category name is same as any existing category or subcategory under the intended category.");
                response.IsSuccess = true;

                //invalidate the memory cache having the category details
                //so that the next time the category details are asked e.g. in case of transactio, latest will be returned
                InvalidateCategoryIncache();
            }
            catch (Exception wemScriptException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemScriptException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        private bool Checkccess(DE.Category category)
        {
            if (category.ParentId == 0)
            {
                if (Security.Access.IsSuperAdmin())
                    return true;
            }
            else if (category.ParentId != 0)
            {
                if (Security.Access.Role(category.ParentId.GetValueOrDefault().ToString()))
                    return true;
            }
            return false;
        }

        public override UpdateCategoryResMsg UpdateCategory(UpdateCategoryReqMsg value)
        {
            UpdateCategoryResMsg response = new UpdateCategoryResMsg();
            try
            {
                CategoryDS categoryDs = new CategoryDS();
                CategoryDSExt categoryDsExt = new CategoryDSExt();

                bool isDuplicate = false;
                string name = "";

                foreach (var category in value.Categories)
                {
                    if (Security.Access.Role(category.CategoryId.ToString()))
                    {
                        var deCategory = Translators.Common.CategorySE_DE.CategorySEtoDE(category);

                        if (!String.IsNullOrEmpty(category.NewName) && category.Name != category.NewName)
                        {
                            if (deCategory.PartitionKey == deCategory.CompanyId + "_ROOT")
                            {
                                isDuplicate = categoryDsExt.IsDuplicateCategory(deCategory);
                            }
                            else
                            {
                                isDuplicate = categoryDsExt.IsDuplicateSubCategory(deCategory);
                            }
                        }

                        if (isDuplicate)
                            break;
                        else
                            categoryDs.Update(deCategory);


                        if (isDuplicate)
                        {
                            response.IsSuccess = false;
                            throw new Exception("Category name " + name + "  is already present.Try giving a different name");
                        }

                        List<DE.Category> categories = Translators.Common.CategorySE_DE.CategorySEListtoDEList(value.Categories);

                        categories.ForEach(c =>
                        {
                            categoryDs.Update(c);
                        });

                        response.IsSuccess = true;
                        //invalidate the memory cache having the category details
                        //so that the next time the category details are asked e.g. in case of transactio, latest will be returned
                        InvalidateCategoryIncache();
                    }
                    else
                        throw new Exception("You do not have access or in a role to update category");
                }

            }

            catch (Exception wemScriptException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemScriptException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        public override DeleteCategoryResMsg DeleteCategory(DeleteCategoryReqMsg value)
        {
            DeleteCategoryResMsg response = new DeleteCategoryResMsg();
            try
            {
                if (value != null)
                {
                    //for mark each category/subcategory for deletion if not
                    for (int i = 0; i < value.Categories.Count; i++)
                    {
                        if (Security.Access.Role(value.Categories[i].CategoryId.ToString()))
                        {
                            value.Categories[i].IsDeleted = true;
                        }
                        else
                            throw new Exception("You do not have access to this category " + value.Categories[i].CategoryId);
                    }
                    CategoryDS categoryDs = new CategoryDS();
                    categoryDs.UpdateBatch(Translators.Common.CategorySE_DE.CategorySEListtoDEList(value.Categories));
                    response.IsSuccess = true;

                    //invalidate the memory cache having the category details
                    //so that the next time the category details are asked e.g. in case of transactio, latest will be returned
                    InvalidateCategoryIncache();
                }
            }
            catch (Exception wemScriptException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemScriptException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override GetAllModulesResMsg GetAllModules()
        {
            GetAllModulesResMsg resp = new GetAllModulesResMsg();

            try
            {
                ModuleDSExt categoryDS = new ModuleDSExt();
                resp.Module = Translators.Common.ModuleDE_SE.ModuleDEListtoSEList(categoryDS.GetAllModules().ToList());

                //var resposne = companyDS.ge

                //company.CompanyId = resposne.CompanyId;
                //company.StorageBaseUrl = resposne.StorageBaseUrl;
                //company.DeploymentBaseUrl = resposne.DeploymentBaseUrl;

                //resp = new GetAllModulesResMsg
                //{
                //    Company = company
                //};
            }

            catch (Exception wemException)
            {
                Exception ex = new Exception();

                bool rethrow = ExceptionHandler.HandleException(wemException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }

            return resp;
        }

        private void InvalidateCategoryIncache()
        {
            ObjectCache memCache = MemoryCache.Default;
            if (memCache.Contains(compCategoriesCacheKey))
            {
                memCache.Remove(compCategoriesCacheKey);
            }
        }

        public override AddReferenceDataResMsg AddReferenceData(AddReferenceDataReqMsg value)
        {
            AddReferenceDataResMsg response = new AddReferenceDataResMsg();
            try
            {
                //first check if the category or subcategory namne is duplicate
                ReferenceDataDSExt referenceDsExt = new ReferenceDataDSExt();
                ReferenceDataDS referenceDs = new ReferenceDataDS();
                List<DE.ReferenceData> referenceData = Translators.Common.APIServiceSE_DE.ReferenceDataSEListtoDEList(value.referenceData);

                bool isDuplicate = false;
                foreach (DE.ReferenceData reference in referenceData)
                {   
                    if(!referenceDsExt.IsDuplicateReferenceData(reference))
                       referenceDs.Insert(reference);
                    if (isDuplicate)
                        break;
                }
                if (isDuplicate)
                    throw new Exception("One or more provided referencetype and referencekey values are under the intended ReferenceData.");
                response.IsSuccess = true;
                
            }
            catch (Exception wemScriptException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemScriptException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        public override GetReferenceDataResMsg GetReferenceData(GetReferenceDataReqMsg value)
        {
            GetReferenceDataResMsg response = new GetReferenceDataResMsg();
            try
            {
                ReferenceDataDS referenceDs = new ReferenceDataDS();
                List<ReferenceData> referenceDatas = new List<ReferenceData>();
                List<DE.ReferenceData> referenceDataDE = referenceDs.GetAll().Where(dre => dre.PartitionKey == value.PartitionKey).ToList();
                List<ReferenceData> references= Translators.Common.APIServiceSE_DE.ReferenceDataDEListtoSEList(referenceDataDE); 
                if(!string.IsNullOrEmpty(value.ReferenceType) && !string.IsNullOrEmpty(value.ReferenceKey))
                {
                    referenceDatas = references.Where(rd => rd.ReferenceType.ToLower() == value.ReferenceType.ToLower() && rd.ReferenceKey.ToLower() == value.ReferenceKey.ToLower()).ToList();
                }
                else if (string.IsNullOrEmpty(value.ReferenceKey) && !string.IsNullOrEmpty(value.ReferenceType))
                {
                    referenceDatas = references.Where(rd => rd.ReferenceType.ToLower() == value.ReferenceType.ToLower()).ToList();
                }
                
                response.referenceData = referenceDatas;
            }
            catch (Exception wemScriptException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemScriptException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

    }

    public class CategoryComparer : IEqualityComparer<Infosys.WEM.Service.Common.Contracts.Data.Category>
    {

        public bool Equals(Infosys.WEM.Service.Common.Contracts.Data.Category x, Infosys.WEM.Service.Common.Contracts.Data.Category y)
        {
            return x.CategoryId == y.CategoryId;
        }
        public int GetHashCode(Infosys.WEM.Service.Common.Contracts.Data.Category obj) { return obj.CategoryId; }
    }
}

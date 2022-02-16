/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using Infosys.WEM.Business.Component;
using BusinessTranslator = Infosys.WEM.Business.Component.Translators;
using Infosys.WEM.Service.Contracts;
using Infosys.WEM.Service.Contracts.Message;
using Infosys.WEM.Infrastructure.Common;
using Infosys.WEM.Resource.DataAccess;
using BE = Infosys.WEM.Business.Entity;
using SE = Infosys.WEM.Service.Contracts.Data;
using DE = Infosys.WEM.Resource.Entity;
using System.Configuration;



namespace Infosys.WEM.Service.Implementation
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public abstract class WorkflowAutomation_ServiceBase : IWorkflowAutomation
    {
        public virtual PublishResMsg Publish(PublishReqMsg value)
        {
            return null;
        }

        public virtual PublishResMsg UpdateWorkflow(PublishReqMsg value)
        {
            return null;
        }

        //public virtual PublishObjectModelResMsg PublishObjectModel(PublishObjectModelReqMsg value)
        //{
        //    return null;
        //}

        public virtual GetWorkflowDetailsResMsg GetWorkflowDetails(int categoryId, Guid workflowId, int workflowVer, string requestId, string requestorSourceIp)
        {
            return null;
        }

        public virtual InvokeWorkflowResMsg InvokeWorkflow(InvokeWorkflowReqMsg value)
        {
            return null;
        }

        //public virtual GetAllActiveWorkflowCategoriesResMsg GetAllActiveWorkflowCategories(string companyId)
        //{
        //    return null;
        //}

        public virtual GetAllActiveWorkflowsByCategoryResMsg GetAllActiveWorkflowsByCategory(string categoryId)//,string subCategoryId)
        {
            return null;
        }

        public virtual GetDocumentURIResMsg GetDocumentURI(int categoryId, string workflowId, int workflowVer, string requestId, string requestorSourceIp)
        {
            throw new NotImplementedException();
        }


        public virtual AddCategoryResMsg AddCategory(AddCategoryReqMsg value)
        {
            throw new NotImplementedException();
        }

        //public virtual GetWorkflowByCategoryResMsg GetWorkflowByCategory(string categoryId)
        //{
        //    return null;
        //}

        public virtual DeleteWorkflowResMsg DeleteWorkflow(DeleteWorkflowReqMsg value)
        {
            return null;
        }

    }

    public partial class WorkflowAutomation : WorkflowAutomation_ServiceBase
    {

        public override PublishResMsg Publish(PublishReqMsg value)
        {
            PublishResMsg responseMsgSE = null;
            WEMValidationException validateException = new WEMValidationException(ErrorMessages.Duplicate_Workflow_Name.ToString());
            List<ValidationError> validateErrs = new List<ValidationError>();

            if (!string.IsNullOrEmpty(value.WorkflowURI))
                value.StorageBaseURL = (new CommonRepository()).GetCompanyDetails(value.CompanyId).Company.StorageBaseUrl + value.WorkflowURI;

            try
            {
                ValidateUploadingFile(value.WorkflowURI);
                ValidateNameField(value);

                if (WorkflowMasterKeysExtension.IsDuplicate(Translators.AutomationWorkflowSEToWorkflowExecutionStoreDE.PublishReqSEToWorkflowMasterDE(value, true)))
                {
                    ValidationError validationErr = new ValidationError();
                    validationErr.Code = "1041";
                    validationErr.Description = ErrorMessages.Duplicate_Workflow_Name;
                    validateErrs.Add(validationErr);
                    validateException.Data.Add("ValidationErrors", validateErrs);
                    throw validateException;
                }


                ManageWorkflows workflowBL = new Business.Component.ManageWorkflows();

                if (Security.Access.Check(value.CategoryID.ToString()))
                {
                    Infosys.WEM.Resource.Entity.Document.Workflow response = Upload(value);

                    if ((response != null && response.StatusCode == 0) || !value.Modified)
                    {
                        responseMsgSE = Translators.AutomationWorkflowSEToManageWorkflowBE.WorkflowMasterBEToPublishReqSE(
                                                        workflowBL.PublishWorkflow(
                                                            Translators.AutomationWorkflowSEToManageWorkflowBE.PublishReqSEToWorkflowMasterBE(value)
                                                            ));

                        if (responseMsgSE != null && value.Parameters != null && value.Parameters.Count > 0)
                        {
                            WorkflowParamDS paramDs = new WorkflowParamDS();

                            List<SE.WorkflowParam> newPrams = value.Parameters.Where(p => p.IsNew).ToList();
                            paramDs.InsertBatch(Translators.WorkflowParamsSE_DE.WorkflowParamsListSEtoDE(newPrams));

                            List<SE.WorkflowParam> updatedParams = value.Parameters.Where(p => !p.IsNew).ToList();
                            paramDs.UpdateBatch(Translators.WorkflowParamsSE_DE.WorkflowParamsListSEtoDE(updatedParams));

                        }
                    }
                }
            }
            catch (Exception wemException)
            {
                Exception ex = new Exception();

                bool rethrow = ExceptionHandler.HandleException(wemException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    //ServiceFaults explictly have to be packed as a custom object because webhttp binding cannot handle FaultContracts.
                    responseMsgSE = new PublishResMsg();
                    responseMsgSE.ServiceFaults = ExceptionHandler.ExtractServiceFaults(ex);

                    //throw ex;

                }
            }

            return responseMsgSE;
        }

        private Infosys.WEM.Resource.Entity.Document.Workflow Upload(PublishReqMsg value)
        {
            Infosys.WEM.Resource.Entity.Document.Workflow response = null;

            if (value.WFContent != null && value.Modified)
            {
                Infosys.WEM.Resource.DataAccess.Document.WorkflowDS wf =
                    new Resource.DataAccess.Document.WorkflowDS();

                response = wf.Upload(new DE.Document.Workflow
                {

                    CompanyId = Convert.ToInt32(value.CompanyId),
                    File =  new System.IO.MemoryStream(value.WFContent),
                    FileName = value.FileName,
                    StorageBaseURL = value.StorageBaseURL,
                    UploadedBy = Utility.GetLoggedInUser(),
                    WorkflowId = value.WorkflowID.ToString()

                });
            }

            return response;
        }

        public override PublishResMsg UpdateWorkflow(PublishReqMsg value)
        {
            PublishResMsg responseMsgSE = null;

            if (!string.IsNullOrEmpty(value.WorkflowURI))
                value.StorageBaseURL = (new CommonRepository()).GetCompanyDetails(value.CompanyId).Company.StorageBaseUrl + value.WorkflowURI;

            try
            {
                ManageWorkflows workflowBL = new Business.Component.ManageWorkflows();

                Infosys.WEM.Resource.Entity.Document.Workflow response = Upload(value);

                if ((response != null && response.StatusCode == 0) || !value.Modified)
                {

                    responseMsgSE = Translators.AutomationWorkflowSEToManageWorkflowBE.WorkflowMasterBEToPublishReqSE(
                                                    workflowBL.Updateworkflow(
                                                        Translators.AutomationWorkflowSEToManageWorkflowBE.PublishReqSEToWorkflowMasterBE(value)
                                                        ));
                    if (responseMsgSE != null && value.Parameters != null && value.Parameters.Count > 0)
                    {
                        WorkflowParamDS paramDs = new WorkflowParamDS();

                        //group together the updated and new ones
                        List<SE.WorkflowParam> newPrams = value.Parameters.Where(p => p.IsNew).ToList();
                        paramDs.InsertBatch(Translators.WorkflowParamsSE_DE.WorkflowParamsListSEtoDE(newPrams));

                        List<SE.WorkflowParam> updatedParams = value.Parameters.Where(p => !p.IsNew).ToList();
                        paramDs.UpdateBatch(Translators.WorkflowParamsSE_DE.WorkflowParamsListSEtoDE(updatedParams));
                        //paramDs.UpdateBatch(Translators.WorkflowParamsSE_DE.WorkflowParamsListSEtoDE(value.Parameters));
                    }
                }
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

            return responseMsgSE;
        }

        //public override PublishObjectModelResMsg PublishObjectModel(PublishObjectModelReqMsg value)
        //{
        //    PublishObjectModelResMsg response = null;

        //    try
        //    { 

        //    }
        //    catch (Exception wemException)
        //    {
        //        Exception ex = new Exception();

        //        bool rethrow = ExceptionHandler.HandleException(wemException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

        //        if (rethrow)
        //        {
        //            throw ex;
        //        }
        //    }

        //    return response;
        //}

        public override DeleteWorkflowResMsg DeleteWorkflow(DeleteWorkflowReqMsg value)
        {
            try
            {
                ManageWorkflows wf = new ManageWorkflows();
                wf.Delete(new BE.WorkflowMaster
                {
                    WorkflowID = value.Workflow.WorkflowID,
                    WorkflowVersion = value.Workflow.WorkflowVersion,
                    CategoryID = value.Workflow.CategoryID
                });

                //then delete the workflow parameter, if any
                //check if there is any parameter associated  with the concerned work flow
                WorkflowParamDS paramDs = new WorkflowParamDS();
                List<SE.WorkflowParam> wfParams = Translators.WorkflowParamsSE_DE.WorkflowParamsListDEtoSE(
                            paramDs.GetAll(Translators.WorkflowParamsSE_DE.WorkflowParamsSEtoDE(
                            new SE.WorkflowParam() { WorkflowId = value.Workflow.WorkflowID.ToString() })).ToList());

                if (wfParams != null && wfParams.Count > 0)
                {
                    for (int i = 0; i < wfParams.Count; i++)
                    {
                        wfParams[i].IsDeleted = true;
                    }
                    paramDs.UpdateBatch(Translators.WorkflowParamsSE_DE.WorkflowParamsListSEtoDE(wfParams));
                }

                return new DeleteWorkflowResMsg { Status = true };
            }
            catch (Exception wemException)
            {
                Exception ex = new Exception();

                bool rethrow = ExceptionHandler.HandleException(wemException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw;
                }
            }
            return new DeleteWorkflowResMsg { Status = false };
        }

        public override GetWorkflowDetailsResMsg GetWorkflowDetails(int categoryId, Guid workflowId, int workflowVer, string requestId, string requestorSourceIp)
        {
            GetWorkflowDetailsResMsg responseMsgSE = null;
            try
            {
                GetWorkflowDetailsReqMsg value = new GetWorkflowDetailsReqMsg
                {
                    WorkflowIdentifier = new SE.WorkflowIdentifier
                    {
                        CategoryID = categoryId,
                        WorkflowID = workflowId,
                        WorkflowVer = workflowVer,
                        RequestorID = requestId,
                        RequestorSourceIP = requestorSourceIp
                    }
                };

                WorkflowMasterDS workflowMasterDS = new WorkflowMasterDS();

                ManageWorkflows workflowBL = new Business.Component.ManageWorkflows();

                SE.WorkflowIdentifier workflowIdSE = value.WorkflowIdentifier;
                //Fetch the latest active version

                responseMsgSE = Translators.AutomationWorkflowSEToManageWorkflowBE.WorkflowMasterBEToGetWorkflowDetailsResSE(
                workflowBL.GetLatestActiveVersion(workflowIdSE.CategoryID, workflowIdSE.WorkflowID, workflowIdSE.WorkflowVer));

                //now get the parameters details
                if (responseMsgSE != null && responseMsgSE.WorkflowDetails != null)
                {
                    WorkflowParamDS paramDs = new WorkflowParamDS();
                    responseMsgSE.WorkflowDetails.Parameters = Translators.WorkflowParamsSE_DE.WorkflowParamsListDEtoSE(paramDs.GetAll(Translators.WorkflowParamsSE_DE.WorkflowParamsSEtoDE(new SE.WorkflowParam() { WorkflowId = workflowId.ToString() })).ToList());
                }
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

            return responseMsgSE;

        }

        //public override GetAllActiveWorkflowCategoriesResMsg GetAllActiveWorkflowCategories(string companyId)
        //{
        //    GetAllActiveWorkflowCategoriesResMsg responseMsgSE = null;
        //    try
        //    {
        //        GetAllActiveWorkflowCategoriesReqMsg value = new GetAllActiveWorkflowCategoriesReqMsg { CompanyID = Convert.ToInt32(companyId) };

        //        WorkflowCategoryMasterDS workflowCategoryMasterDS = new WorkflowCategoryMasterDS();

        //        responseMsgSE = (
        //            Translators.AutomationWorkflowSEToWorkflowExecutionStoreDE.WorkflowCategoryMasterDEListToGetAllActiveWorkflowCategoriesResSE(
        //            workflowCategoryMasterDS.GetAll(
        //            Translators.AutomationWorkflowSEToWorkflowExecutionStoreDE.GetAllActiveWorkflowCategoriesReqSEToWorkflowCategoryMasterDE(value)
        //            )
        //            ));
        //        //Select only active categories
        //        //responseMsgSE.CategoryDetails = responseMsgSE.CategoryDetails.Where(res => res.IsActive == true).ToList<SE.WorkflowCategoryMaster>();
        //    }
        //    catch (Exception wemException)
        //    {
        //        Exception ex = new Exception();

        //        bool rethrow = ExceptionHandler.HandleException(wemException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

        //        if (rethrow)
        //        {
        //            throw ex;
        //        }
        //    }
        //    return responseMsgSE;

        //}

        public override GetAllActiveWorkflowsByCategoryResMsg GetAllActiveWorkflowsByCategory(string categoryId)//,string subCategoryId)
        {
            GetAllActiveWorkflowsByCategoryResMsg responseMsgSE = new GetAllActiveWorkflowsByCategoryResMsg();
            try
            {
                if (Security.Access.Check(categoryId))
                {
                    WorkflowMasterDS wfMaster = new WorkflowMasterDS();
                    //var masterWf = wfMaster.GetAllSubCategories(new DE.WorkflowMaster { CategoryId = Convert.ToInt32(categoryId),SubCategoryId = Convert.ToInt32(subCategoryId) }).ToList();

                    var masterWf = wfMaster.GetWorkflowsByCategoryId(new DE.WorkflowMaster { CategoryId = Convert.ToInt32(categoryId) }).ToList();

                    responseMsgSE = new GetAllActiveWorkflowsByCategoryResMsg
                    {
                        CategoryWorkflowMapping =
                            Translators.WorkflowMasterSE_DE.WorkflowMasterDEToWorkflowMasterSE(masterWf)
                    };

                    if (responseMsgSE != null && responseMsgSE.CategoryWorkflowMapping != null && responseMsgSE.CategoryWorkflowMapping.Count > 0)
                    {
                        //now get the parameters details
                        WorkflowParamDS paramDs = new WorkflowParamDS();
                        for (int i = 0; i < responseMsgSE.CategoryWorkflowMapping.Count; i++)
                        {
                            responseMsgSE.CategoryWorkflowMapping[i].Parameters = Translators.WorkflowParamsSE_DE.WorkflowParamsListDEtoSE(
                                paramDs.GetAll(Translators.WorkflowParamsSE_DE.WorkflowParamsSEtoDE(
                                new SE.WorkflowParam() { WorkflowId = responseMsgSE.CategoryWorkflowMapping[i].WorkflowID.ToString() })).ToList());
                        }
                    }
                }
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
            return responseMsgSE;
        }


        public override GetDocumentURIResMsg GetDocumentURI(int categoryId, string workflowId, int workflowVer, string requestId, string requestorSourceIp)
        {
            return null;
        }

        public override InvokeWorkflowResMsg InvokeWorkflow(InvokeWorkflowReqMsg value)
        {
            return null;
        }

        public override AddCategoryResMsg AddCategory(AddCategoryReqMsg value)
        {
            try
            {
                var categoryDE =
                    Translators.WorkflowCategoryMasterSE_DE.WorkflowCategoryMasterSEToWorkflowCategoryDE(value.Category);

                ManageCategory category = new ManageCategory();
                var response = category.Add(categoryDE);
                AddCategoryResMsg catResponse = new AddCategoryResMsg
                {
                    CategoryId = response.Id,
                    Success = true
                };
                return catResponse;
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
            return new AddCategoryResMsg
                {
                    Success = true
                }; ;
        }

        //public override GetWorkflowByCategoryResMsg GetWorkflowByCategory(string categoryId)
        //{
        //    GetWorkflowByCategoryResMsg responseMsgSE = null;
        //    try
        //    {
        //        GetWorkflowReqMsg value = new GetWorkflowReqMsg { CategoryId = Convert.ToInt32(categoryId) };

        //        //var workflowDE = Translators.WorkflowMasterSE_DE.WorkflowMasterSEToWorkflowMasterDE(value);

        //        WorkflowMasterDS workflowMaster = new WorkflowMasterDS();
        //        var workflows = workflowMaster.GetAll().ToList();//.Where(wf => wf.CategoryId == Convert.ToInt32(categoryId)).ToList();
        //        responseMsgSE = new GetWorkflowByCategoryResMsg { WorkflowDetails = Translators.WorkflowMasterSE_DE.WorkflowMasterDEToWorkflowMasterSE(workflows) };
        //       // return workflows;
        //    }
        //    catch (Exception wemException)
        //    {
        //        Exception ex = new Exception();

        //        bool rethrow = ExceptionHandler.HandleException(wemException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

        //        if (rethrow)
        //        {
        //            throw ex;
        //        }
        //    }
        //    return responseMsgSE;
        //   // return null;

        //}


        private void ValidateUploadingFile(string WorkflowURI) 
        {
            bool raiseError = false;

            if (!string.IsNullOrEmpty(WorkflowURI))
            {
                if (!(System.IO.Path.GetExtension(WorkflowURI).ToLower().Equals(".xaml") || System.IO.Path.GetExtension(WorkflowURI).ToLower().Equals(".iapw")))
                    raiseError = true;
            }
            else
                raiseError = true;

            if (raiseError)
            {
                WEMValidationException validateException = new WEMValidationException(ErrorMessages.FileUpload_Validation_Failed.ToString());
                List<ValidationError> validateErrs = new List<ValidationError>();

                ValidationError validationErr = new ValidationError();
                validationErr.Code = "1013";
                validationErr.Description = string.Format(ErrorMessages.FileUpload_Validation_Failed, "Uploaded file type is not allowed for workflow");
                validateErrs.Add(validationErr);
                validateException.Data.Add("ValidationErrors", validateErrs);
                throw validateException;
            }
        }


        private void ValidateNameField(PublishReqMsg workflowRequest)
        {
            if (workflowRequest != null)
            {
                //check for value invalidcharacters
                WEMValidationException validateException = new WEMValidationException(ErrorMessages.InvalidCharacter_Validation.ToString());
                List<ValidationError> validateErrs = new List<ValidationError>();

                if (ValidationUtility.InvalidCharacterValidator(workflowRequest.Name))
                {
                    ValidationError validationErr = new ValidationError();
                    validationErr.Code = Errors.ErrorCodes.InvalidCharacter_Validation.ToString();
                    validationErr.Description = string.Format(ErrorMessages.InvalidCharacter_Validation, workflowRequest.Name, "Workflow.Name");
                    validateErrs.Add(validationErr);
                }
                if (workflowRequest.Parameters != null)
                {
                    foreach (var param in workflowRequest.Parameters)
                    {
                        if (ValidationUtility.InvalidCharacterValidator(param.Name))
                        {
                            ValidationError validationErr = new ValidationError();
                            validationErr.Code = Errors.ErrorCodes.InvalidCharacter_Validation.ToString();
                            validationErr.Description = string.Format(ErrorMessages.InvalidCharacter_Validation, param.Name, "Worfklow.Parameters.Name");
                            validateErrs.Add(validationErr);
                        }
                    }
                }
                if (validateErrs.Count > 0)
                {
                    validateException.Data.Add("ValidationErrors", validateErrs);
                    throw validateException;
                }
            }
        }

    }

    public static class StringExtension
    {
        public static Guid ToGuid(this string s)
        {
            Guid t;

            Guid.TryParse(s, out t);

            return t;
        }
    }
}

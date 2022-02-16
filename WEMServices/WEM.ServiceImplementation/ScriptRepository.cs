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

using Infosys.WEM.Scripts.Service.Contracts;
using Infosys.WEM.Scripts.Service.Contracts.Message;
using Infosys.WEM.Scripts.Resource.DataAccess;
using Infosys.WEM.Scripts.Resource.DataAccess.Document;
using DE = Infosys.WEM.Resource.Entity;
using SE = Infosys.WEM.Scripts.Service.Contracts.Data;
using DocumentEntity = Infosys.WEM.Resource.Entity.Document;
using Infosys.WEM.Infrastructure.Common;
using System.Configuration;
using Newtonsoft.Json;

namespace Infosys.WEM.Service.Implementation
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public abstract class ScriptRepository_ServiceBase : IScriptRepository
    {

        #region IScriptRepository Members

        public virtual GetScriptDetailsResMsg GetScriptDetails(string scriptId, string categoryId, string version = "0")
        {
            return null;
        }

        public virtual GetAllScriptDetailsResMsg GetAllScriptDetails(string categoryId)
        {
            return null;
        }

        public virtual GetAllScriptDetailsResMsg GetAllScriptDetailsWithSubcategories(string categoryId, Boolean IncludeSubCategoryScripts)
        {
            return null;
        }

        public virtual AddScriptResMsg AddScript(AddScriptReqMsg value)
        {
            return null;
        }

        public virtual DeleteScriptResMsg DeleteScript(DeleteScriptReqMsg value)
        {
            return null;
        }

        public virtual UpdateScriptResMsg UpdateScript(UpdateScriptReqMsg value)
        {
            return null;
        }

        #endregion
    }

    public partial class ScriptRepository : ScriptRepository_ServiceBase
    {
        public override GetScriptDetailsResMsg GetScriptDetails(string scriptId, string categoryId, string version = "0")
        {
            GetScriptDetailsResMsg response = new GetScriptDetailsResMsg();
            using (LogHandler.TraceOperations("ScriptRepository:GetScriptDetails", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
            {
                if (!string.IsNullOrEmpty(scriptId) && !string.IsNullOrEmpty(categoryId))
                {
                    try
                    {
                        if (Security.Access.Check(categoryId))
                        {
                            ScriptDS scriptDs = new ScriptDS();
                            if (string.IsNullOrEmpty(version)) version = "0";
                            SE.Script scriptSE = new SE.Script() { ScriptId = int.Parse(scriptId), CategoryId = int.Parse(categoryId), ScriptFileVersion = int.Parse(version) };
                            //translater SE to DE
                            DE.Script scriptDE = Translators.Scripts.ScriptSE_DE.ScriptSEtoDE(scriptSE);
                            response.ScriptDetails = Translators.Scripts.ScriptSE_DE.ScriptDEtoSE(scriptDs.GetOne(scriptDE));
                            string data = JsonConvert.SerializeObject(response.ScriptDetails);
                            if (response.ScriptDetails != null)
                            {
                                response.ScriptDetails.Parameters = GetScriptParams(scriptId); ;
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
                }
            }
            return response;
        }

        public override GetAllScriptDetailsResMsg GetAllScriptDetailsWithSubcategories(string categoryId,Boolean IncludeSubCategoryScripts)
        {
            return GetScriptDetails(categoryId, IncludeSubCategoryScripts);
        }


        public override GetAllScriptDetailsResMsg GetAllScriptDetails(string categoryId)
        {
            return GetScriptDetails(categoryId);
        }

        private GetAllScriptDetailsResMsg GetScriptDetails(string categoryId, Boolean IncludeSubCategoryScripts=false)
        {
            GetAllScriptDetailsResMsg response = new GetAllScriptDetailsResMsg();
            try
            {
                if (Security.Access.Check(categoryId))
                //if (Security.Access.ManagerOrAnalyst(categoryId))
                {
                    ScriptDS scriptDs = new ScriptDS();
                    ScriptDSScriptExt scriptDSScriptExt = new ScriptDSScriptExt();
                    SE.Script scriptSE = new SE.Script() { CategoryId = int.Parse(categoryId) };
                    DE.Script scriptDE = Translators.Scripts.ScriptSE_DE.ScriptSEtoDE(scriptSE);
                    if (IncludeSubCategoryScripts)
                        response.Scripts = Translators.Scripts.ScriptSE_DE.ScriptDEListtoSEList(scriptDSScriptExt.GetAllScript(scriptDE, IncludeSubCategoryScripts).ToList());
                    else
                    response.Scripts = Translators.Scripts.ScriptSE_DE.ScriptDEListtoSEList(scriptDs.GetAll(scriptDE).ToList());

                    if (response.Scripts != null && response.Scripts.Count > 0)
                    {
                        response.Scripts.ToList().ForEach(s =>
                        {
                            s.Parameters = GetScriptParams(s.ScriptId.ToString());
                        });
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

        bool allowDuplicate = false;
        public override AddScriptResMsg AddScript(AddScriptReqMsg value)
        {
            AddScriptResMsg response = new AddScriptResMsg();
            DeleteScriptReqMsg deleteScript = new DeleteScriptReqMsg();
            value.Script.StorageBaseUrl = (new CommonRepository()).GetCompanyDetails(value.Script.BelongsToOrg).Company.StorageBaseUrl;

            try
            {
                ValidateUploadingFile(value.Script);
                ValidateNameField(value.Script);
                ScriptDS scriptDs = new ScriptDS();
                if (Security.Access.ManagerOrAnalyst(value.Script.CategoryId.ToString()))
                {
                    DE.Script tempDeScript = Translators.Scripts.ScriptSE_DE.ScriptSEtoDE(value.Script);
                    if (allowDuplicate || !ScriptDSExt.IsDuplicate(tempDeScript))
                    {
                        SE.Script tempScript = Translators.Scripts.ScriptSE_DE.ScriptDEtoSE(scriptDs.Insert(tempDeScript));
                        if (tempScript != null && tempScript.ScriptId > 0)
                        {
                            deleteScript.CategoryId = tempScript.CategoryId;
                            deleteScript.ScriptId = tempScript.ScriptId;
                            // deleteScript.ModifiedBy = tempScript.ModifiedBy;

                            value.Script.ScriptId = tempScript.ScriptId;
                            value.Script.ScriptFileVersion = tempScript.ScriptFileVersion;
                            if (value.Script.Parameters != null && value.Script.Parameters.Count > 0)
                            {
                                //first update the script id received
                                value.Script.Parameters.ForEach(p => { p.ScriptId = tempScript.ScriptId; });
                                ScriptParamDS scriptparamDs = new ScriptParamDS();
                                scriptparamDs.InsertBatch(Translators.Scripts.ScriptParamsSE_DE.ScriptParamsListSEtoDE(value.Script.Parameters));
                            }

                            if (value.Script.ScriptContent != null)
                            {
                                //upload the file content with following details:
                                //container name = S_<company id>_<script id>
                                //file name = <script name>_<version>.ext ; ext- as applicable e.g.- vbs, bat, etc
                                DocumentEntity.Script scriptDoc = Translators.Scripts.ScriptSE_DocumentEntity.ScriptSEtoDocumentEntity(value.Script);
                                ScriptRepositoryDS scriptrepoDs = new ScriptRepositoryDS();
                                scriptDoc = scriptrepoDs.Upload(scriptDoc);
                                if (scriptDoc.StatusCode == 0)
                                {
                                    value.Script.ScriptURL = scriptDoc.ScriptUrl;
                                    //value.Script.ModifiedBy = Utility.GetLoggedInUser();// value.Script.CreatedBy;
                                    //update url in the script table
                                    scriptDs.Update(Translators.Scripts.ScriptSE_DE.ScriptSEtoDE(value.Script));
                                }
                                else
                                {
                                    throw new Exception(scriptDoc.StatusMessage);
                                }
                            }
                            response.IsSuccess = true;
                        }
                    }
                    else
                    {
                        WEMValidationException validateException = new WEMValidationException(ErrorMessages.Duplicate_Script_Name);
                        List<ValidationError> validateErrs = new List<ValidationError>();
                        ValidationError validationErr = new ValidationError();
                        validationErr.Code = "1042";
                        validationErr.Description = ErrorMessages.Duplicate_Script_Name;
                        validateErrs.Add(validationErr);

                        if (validateErrs.Count > 0)
                        {
                            validateException.Data.Add("ValidationErrors", validateErrs);
                            throw validateException;
                        }

                    }
                }
                else
                {
                    throw new Exception("You do not have access to add script");
                }
            }
            catch (Exception wemScriptException)
            {
                try { DeleteScript(deleteScript); }
                catch { }
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemScriptException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    //ServiceFaults explictly have to be packed as a custom object because webhttp binding cannot handle FaultContracts.
                    response.ServiceFaults = ExceptionHandler.ExtractServiceFaults(ex);
                    //throw ex;
                }
            }
            allowDuplicate = false;
            return response;

        }

        public override UpdateScriptResMsg UpdateScript(UpdateScriptReqMsg value)
        {
            UpdateScriptResMsg response = new UpdateScriptResMsg();

            value.Script.StorageBaseUrl = (new CommonRepository()).GetCompanyDetails(value.Script.BelongsToOrg).Company.StorageBaseUrl;

            try
            {
                ValidateNameField(value.Script);

                if (Security.Access.ManagerOrAnalyst(value.Script.CategoryId.ToString()))
                {
                    //change the script version if-
                    //1. the script has any file and if the content is changed
                    //2. the parameter count is changed
                    //3. script name is changed
                    //4. parameter type and name changed
                    //5. category is changed
                    bool addNew = false;
                    ScriptDS scriptDs = new ScriptDS();
                    string createBy = Utility.GetLoggedInUser();// value.Script.ModifiedBy; //to be used if a new script to be added with different version
                    bool checkDuplicate = false;

                    //first get the current entry
                    SE.Script currentScript = GetScriptDetails(value.Script.ScriptId.ToString(), value.Script.CategoryId.ToString(), value.Script.ScriptFileVersion.ToString()).ScriptDetails;

                    if (currentScript.Name != value.Script.Name)
                    {
                        addNew = true;
                        checkDuplicate = true;
                    }
                    else if (value.Script.ScriptContent != null)
                    {
                        addNew = true;
                    }
                    else if (value.Script.CategoryId != currentScript.CategoryId)
                    {
                        addNew = true;
                        checkDuplicate = true;
                    }
                    else if (value.Script.Parameters == null)
                    {
                        if (currentScript.Parameters != null && currentScript.Parameters.Count > 0)
                            addNew = true;
                    }
                    else // if (!value.Script.Parameters.Equals(currentScript.Parameters))
                    {
                        if (value.Script.Parameters != null && value.Script.Parameters.Count > 0)
                        {
                            foreach (SE.ScriptParam parameter in value.Script.Parameters)
                            {
                                if (currentScript.Parameters != null)
                                {
                                    if (currentScript.Parameters.Where(p =>
                                        p.Name == parameter.Name && p.ParamType == parameter.ParamType
                                    ).FirstOrDefault() == null)
                                    {
                                        addNew = true;
                                        break;
                                    }
                                }
                            }
                        }
                        if (!addNew)
                        {
                            if (currentScript.Parameters != null && currentScript.Parameters.Count > 0)
                            {
                                foreach (SE.ScriptParam parameter in currentScript.Parameters)
                                {
                                    if (value.Script.Parameters != null)
                                    {
                                        if (value.Script.Parameters.Where(p => p.Name == parameter.Name && p.ParamType == parameter.ParamType).FirstOrDefault() == null)
                                        {
                                            addNew = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        //addNew = true;
                    }

                    if (addNew)
                    {
                        DE.Script tempDeScript = Translators.Scripts.ScriptSE_DE.ScriptSEtoDE(value.Script);
                        if (checkDuplicate)
                        {
                            if (ScriptDSExt.IsDuplicate(tempDeScript))
                            {
                                throw new Exception("The script name is same as an existing script.");
                            }
                        }

                        //add new and soft delete the old version
                        //assign the created by value for script and parameter(s)
                        //value.Script.CreatedBy = createBy;
                        if (value.Script.Parameters != null && value.Script.Parameters.Count > 0)
                        {
                            value.Script.Parameters.ForEach(p => { p.CreatedBy = createBy; });
                        }
                        AddScriptReqMsg req = new AddScriptReqMsg() { Script = value.Script };
                        DeleteScriptReqMsg delReq = new DeleteScriptReqMsg() { ScriptId = value.Script.ScriptId, CategoryId = currentScript.CategoryId };
                        //ModifiedBy = value.Script.ModifiedBy };

                        allowDuplicate = true;
                        //update corresponding existing param for deletion
                        List<SE.ScriptParam> parameters = GetScriptParams(value.Script.ScriptId.ToString());
                        if (parameters != null && parameters.Count > 0)
                        {
                            parameters.ForEach(p =>
                            {
                                p.IsDeleted = true;
                                p.ModifiedBy = createBy;
                            });

                            ScriptParamDS scriptparamDs = new ScriptParamDS();
                            scriptparamDs.UpdateBatch(Translators.Scripts.ScriptParamsSE_DE.ScriptParamsListSEtoDE(parameters));
                        }

                        response.IsSuccess = DeleteScript(delReq).IsSuccess && AddScript(req).IsSuccess;
                    }
                    else
                    {
                        //update the script metadata
                        DE.Script tempScriptDe = Translators.Scripts.ScriptSE_DE.ScriptSEtoDE(value.Script);
                        SE.Script tempScriptSe = Translators.Scripts.ScriptSE_DE.ScriptDEtoSE(scriptDs.Update(tempScriptDe));

                        //now update parameter
                        if (value.Script.Parameters != null && value.Script.Parameters.Count > 0)
                        {
                            //first update the script id received for double sure
                            //value.Script.Parameters.ForEach(p => { p.ScriptId = value.Script.ScriptId; });
                            ScriptParamDS scriptparamDs = new ScriptParamDS();
                            scriptparamDs.UpdateBatch(Translators.Scripts.ScriptParamsSE_DE.ScriptParamsListSEtoDE(value.Script.Parameters));
                        }
                        response.IsSuccess = true;
                    }
                }
                else
                    throw new Exception("You do not have access to update this script");
            }
            catch (Exception wemScriptException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemScriptException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    response.ServiceFaults = ExceptionHandler.ExtractServiceFaults(ex);
                    //throw ex;
                }
            }
            return response;
        }

        public override DeleteScriptResMsg DeleteScript(DeleteScriptReqMsg value)
        {
            DeleteScriptResMsg response = new DeleteScriptResMsg();
            try
            {
                ScriptDS scriptDs = new ScriptDS();
                if (Security.Access.ManagerOrAnalyst(value.CategoryId.ToString()))
                {
                    SE.Script tempScript = new SE.Script() { ScriptId = value.ScriptId, CategoryId = value.CategoryId };//, ModifiedBy = value.ModifiedBy };
                    //update the script for deletion
                    tempScript.IsDeleted = true;
                    scriptDs.Update(Translators.Scripts.ScriptSE_DE.ScriptSEtoDE(tempScript));
                    response.IsSuccess = true;
                }
                else
                {
                    throw new Exception("You do not have access to delete this script");
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

        private List<SE.ScriptParam> GetScriptParams(string scriptId)
        {
            List<SE.ScriptParam> paramList = new List<SE.ScriptParam>();
            ScriptParamDS paramDs = new ScriptParamDS();
            SE.ScriptParam paramSE = new SE.ScriptParam() { ScriptId = int.Parse(scriptId) };
            paramList = Translators.Scripts.ScriptParamsSE_DE.ScriptParamsListDEtoSE(paramDs.GetAll(Translators.Scripts.ScriptParamsSE_DE.ScriptParamsSEtoDE(paramSE)).ToList());
            return paramList;
        }

        private bool IsSameParameter(SE.ScriptParam oldParam, SE.ScriptParam newParam)
        {
            bool isSame = false;
            if (oldParam.Name == newParam.Name && oldParam.ParamType == oldParam.ParamType)
                isSame = true;
            return isSame;
        }

        private void ValidateNameField(Infosys.WEM.Scripts.Service.Contracts.Data.Script script)
        {
            if (script != null)
            {
                //check for value invalidcharacters
                WEMValidationException validateException = new WEMValidationException(ErrorMessages.InvalidCharacter_Validation.ToString());
                List<ValidationError> validateErrs = new List<ValidationError>();

                if (ValidationUtility.InvalidCharacterValidator(script.Name))
                {
                    ValidationError validationErr = new ValidationError();
                    validationErr.Code = Errors.ErrorCodes.InvalidCharacter_Validation.ToString();
                    validationErr.Description = string.Format(ErrorMessages.InvalidCharacter_Validation, script.Name, "Script.Name");
                    validateErrs.Add(validationErr);
                }

                if (script.Parameters != null)
                {
                    foreach (var param in script.Parameters)
                    {
                        if (ValidationUtility.InvalidCharacterValidator(param.Name))
                        {
                            ValidationError validationErr = new ValidationError();
                            validationErr.Code = Errors.ErrorCodes.InvalidCharacter_Validation.ToString();
                            validationErr.Description = string.Format(ErrorMessages.InvalidCharacter_Validation, param.Name, "Script.Parameters.Name");
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

        private void ValidateUploadingFile(Infosys.WEM.Scripts.Service.Contracts.Data.Script script)
        {
            string[] allowedScriptExtention= null;
            
            if(Convert.ToBoolean((new CommonRepository()).GetCompanyDetails(script.BelongsToOrg).Company.EnableSecureTransactions))
                allowedScriptExtention= new string[] {"sh","ps1"};
            else
                allowedScriptExtention = new string[] { "bat", "vbs", "iap", "py", "js", "ps1", "iapd", "sh" };

            if (!string.IsNullOrEmpty(script.ScriptType))
            {
                if (!allowedScriptExtention.ToList().Exists(x => x.ToLower().Equals(script.ScriptType.ToLower())))
                {
                    WEMValidationException validateException = new WEMValidationException(ErrorMessages.FileUpload_Validation_Failed.ToString());
                    List<ValidationError> validateErrs = new List<ValidationError>();

                    ValidationError validationErr = new ValidationError();
                    validationErr.Code = "1013";
                    validationErr.Description = string.Format(ErrorMessages.FileUpload_Validation_Failed, "Uploaded file type is not allowed for script");
                    validateErrs.Add(validationErr);
                    validateException.Data.Add("ValidationErrors", validateErrs);
                    throw validateException;
                }
            }
        }
    }
}

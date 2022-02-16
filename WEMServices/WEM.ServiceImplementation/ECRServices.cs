/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.WEM.Infrastructure.Common;
using Infosys.WEM.Nia.Service.Contracts;
using Infosys.WEM.Nia.Service.Contracts.Data;
using Infosys.WEM.Nia.Service.Contracts.Message;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.WEM.Service.Implementation
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]

    public abstract class ECRServices_ServiceBase : IECRServices
    {
        public virtual BrowseScriptCategoryResMsg BrowseScriptCategory(BrowseScriptCategoryReqMsg value)
        {
            throw new NotImplementedException();
        }

        public virtual AddECRCategoryResMsg AddScriptCategory(AddECRCategoryReqMsg value)
        {
            throw new NotImplementedException();
        }

        public virtual GetScriptByCategoryResMsg GetAllScriptsByCategoryId(GetScriptByCategoryReqMsg value)
        {
            throw new NotImplementedException();
        }
    }

    public partial class ECRServices : ECRServices_ServiceBase
    {
        public override BrowseScriptCategoryResMsg BrowseScriptCategory(BrowseScriptCategoryReqMsg value)
        {
            BrowseScriptCategoryResMsg response = new BrowseScriptCategoryResMsg();
            try
            {                
                Infosys.Nia.Services.RestClient client = Infosys.Nia.Services.RestClient.getInstance(value.Login.UserName,
                    value.Login.Password, string.Format(ECRServiceConstants.CasServerUriFormat, value.Login.CasServerAddr), string.Format(ECRServiceConstants.ECRServerUriFormat, value.Login.ECRServerAddr));
                client.getTicket();
                var resp = client.getResponseOfGET(string.Format(ECRServiceConstants.BrosweCategoryUriFormat, value.Login.ECRServerAddr));
                string rawdata = resp.Replace("\\", "");
                var cats = JsonConvert.DeserializeObject<CategoryTree>(rawdata);
                response.Categories = cats;
              
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

        public override AddECRCategoryResMsg AddScriptCategory(AddECRCategoryReqMsg value)
        {
            AddECRCategoryResMsg response = new AddECRCategoryResMsg();
            try
            {
                Infosys.Nia.Services.RestClient client = Infosys.Nia.Services.RestClient.getInstance(value.Login.UserName,
                        value.Login.Password, string.Format(ECRServiceConstants.CasServerUriFormat, value.Login.CasServerAddr), string.Format(ECRServiceConstants.ECRServerUriFormat, value.Login.ECRServerAddr));
                client.getTicket();
                string postBody = JsonConvert.SerializeObject(value.ECRCategory);
                var resp = client.getResponseOfPOST(string.Format(ECRServiceConstants.AddECRScriptCategory, value.Login.ECRServerAddr), postBody);
                response.IsSuccess = true;
                response.CategoryId = resp;
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

        public override GetScriptByCategoryResMsg GetAllScriptsByCategoryId(GetScriptByCategoryReqMsg value)
        {
            GetScriptByCategoryResMsg response = new GetScriptByCategoryResMsg();
            List<NIAScript> niaScriptsList = new List<NIAScript>();
            try
            {
                response.GetInstanceStartTime = System.DateTime.Now;
                Infosys.Nia.Services.RestClient client = Infosys.Nia.Services.RestClient.getInstance(value.Login.UserName,
                        value.Login.Password, string.Format(ECRServiceConstants.CasServerUriFormat, value.Login.CasServerAddr), string.Format(ECRServiceConstants.ECRServerUriFormat, value.Login.ECRServerAddr));
                response.GetInstanceEndTime = System.DateTime.Now;
                response.GetTicketStartTime = System.DateTime.Now;
                client.getTicket();
                response.GetTicketEndTime = System.DateTime.Now;
                //string postBody = JsonConvert.SerializeObject(value.Data);
                response.GetResponseOfPOSTStartTime = System.DateTime.Now;
                var resultsGet = client.getResponseOfPOST(string.Format(ECRServiceConstants.GetScriptByCategoryUrl, value.Login.ECRServerAddr), value.Data,value.RequestHeaderMap);
                response.GetResponseOfPOSTEndTime = System.DateTime.Now;
                dynamic results = JsonConvert.DeserializeObject(resultsGet);

                foreach (var result in results)
                {
                    NIAScript niaScript = new NIAScript();
                    niaScript.scriptId = result.id;
                    niaScript.scriptName = result.name;
                    niaScript.description = result.description;
                    niaScript.scriptType = result.scriptType;

                    // Setting parameter values for the scripts
                    List<NIAScriptParamVOList> niaScriptParamsList = new List<NIAScriptParamVOList>();
                    var parameters = result.scriptParamVOList;
                    foreach (var parameter in parameters)
                    {
                        NIAScriptParamVOList scriptParams = new NIAScriptParamVOList();
                        scriptParams.id = parameter.id;
                        scriptParams.paramName = parameter.paramName;
                        scriptParams.defaultValue = parameter.defParamValue;
                        scriptParams.id = niaScript.scriptId;
                        scriptParams.isMandatory = parameter.isMandatory;
                        niaScriptParamsList.Add(scriptParams);
                    }
                    niaScript.niaScriptParamList = niaScriptParamsList;
                    niaScriptsList.Add(niaScript);
                }
                response.NIAScripts = niaScriptsList;
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
}

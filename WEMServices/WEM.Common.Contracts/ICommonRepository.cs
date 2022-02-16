/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using System.Text;
using Infosys.WEM.Infrastructure.Common;
using Infosys.WEM.Infrastructure.Common.Validators;
using Infosys.WEM.Service.Common.Contracts.Message;

using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WCF;

namespace Infosys.WEM.Service.Common.Contracts
{
    [ServiceContract]
    [ValidationBehavior]    
    public interface ICommonRepository
    {
        [WebGet(UriTemplate = "/GetCompanyDetails/{companyId}")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]
        GetCompanyResMsg GetCompanyDetails(
            [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string companyId
            );

        [WebGet(UriTemplate = "/GetAllCategoriesByCompany?companyId={companyId}&module={module}")]
        //[OperationContract(Name = "GetAllCategoriesByCompany")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]        
        GetAllCategoriesResMsg GetAllCategoriesByCompany(
            [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string companyId,
            [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string module);

        [WebGet(UriTemplate = "/GetAllCategoriesWithData?companyId={companyId}&module={module}")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]        
        GetAllCategoriesResMsg GetAllCategoriesWithData(
            [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string companyId,
            [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string module);
        

        [WebGet(UriTemplate = "/GetAllClustersByCategory?categoryId={categoryId}")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]        
        GetAllClustersByCategoryResMsg GetAllClustersByCategory(
             [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string categoryId);

        [WebInvoke(Method = "POST")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        AddCategoryResMsg AddCategory(AddCategoryReqMsg value);

        [WebInvoke(Method = "PUT")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        UpdateCategoryResMsg UpdateCategory(UpdateCategoryReqMsg value);

        [WebInvoke(Method = "DELETE")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        DeleteCategoryResMsg DeleteCategory(DeleteCategoryReqMsg value);

        [WebInvoke(Method = "GET")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        GetAllModulesResMsg GetAllModules();

        [WebGet(UriTemplate = "/GetAllCategoriesByCompanyCategoryId?companyId={companyId}&module={module}&categoryId={categoryId}")]
        //[OperationContract(Name = "GetAllCategoriesByCompanyCategoryId")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]
        GetAllCategoriesResMsg GetAllCategoriesByCompanyCategoryId(
           [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string companyId,
           [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string module,
            [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
        string categoryId);

        [WebInvoke(Method = "POST")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        AddReferenceDataResMsg AddReferenceData(AddReferenceDataReqMsg value);

        [WebInvoke(Method = "POST")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        GetReferenceDataResMsg GetReferenceData(GetReferenceDataReqMsg value);
    }
}

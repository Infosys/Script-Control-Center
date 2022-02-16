/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;
using System.Reflection;
using Infosys.WEM.Infrastructure.SecurityCore;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WCF;

namespace Infosys.WEM.Infrastructure.Common.Validators
{    
    public sealed class ADValidatorAttribute : ValidatorAttribute
    {
        protected override Validator DoCreateValidator(Type targetType)
        {
            return new ADValidator();
        }
    }

    public sealed class  ADValidator : Validator
    {
        public ADValidator()
            : base("AD User Validation", "String")
        {
        }
        protected override string DefaultMessageTemplate
        {
            get { return "AD User Validation"; }
        }
        // This method does the actual validation
        public override void DoValidate(object objectToValidate, object currentTarget, string key, ValidationResults validationResults)
        {
            string alias = "";

            if (objectToValidate.GetType() == typeof(String))
            {
                alias = objectToValidate.ToString();                
            }
            else
            {
                PropertyInfo[] properties = objectToValidate.GetType().GetProperties();

                for (int i = 0; i < properties.Length; i++)
                {
                    PropertyInfo p = properties[i];
                    if (p.Name == "Alias")
                    {
                        alias = (String)p.GetValue(objectToValidate, null);
                        break;
                    }
                }
            }

            alias = SecureData.UnSecure(alias, ApplicationConstants.SECURE_PASSCODE);
            var found = false;
            if (alias.ToLower().Equals(ApplicationConstants.DOMAIN_USERS))
                found = true;
            else
                found = Utility.CheckValidUser(alias);

            if (!found)
            {
                LogValidationResult(
                  validationResults,
                  string.Format(this.MessageTemplate, new object[] { objectToValidate }),
                  currentTarget,
                  key);
            }
   
        }
    }
}



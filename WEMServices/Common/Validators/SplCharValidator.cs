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
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WCF;

namespace Infosys.WEM.Infrastructure.Common.Validators
{
    public sealed class SpecialCharacterValidatorAttribute : ValidatorAttribute
    {
        protected override Validator DoCreateValidator(Type targetType)
        {
            return new SpecialCharacterValidator();
        }
    }

    public sealed class  SpecialCharacterValidator : Validator
    {
        public SpecialCharacterValidator()
            : base("Special Character Validation", "String")
        {
        }
        protected override string DefaultMessageTemplate
        {
            get { return "Special Character Validation"; }
        }
        // This method does the actual validation
        public override void DoValidate(object objectToValidate, object currentTarget, string key, ValidationResults validationResults)
        {
            if (objectToValidate != null)
            {
                var splcharfound = ValidationUtility.InvalidCharacterValidator(objectToValidate.ToString());
                if (splcharfound)
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
}

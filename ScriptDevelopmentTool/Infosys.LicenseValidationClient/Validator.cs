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

using Infosys.MTC.Licensing.Framework;
using Infosys.MTC.Licensing.Framework.Interfaces;
using System.IO;

namespace Infosys.LicenseValidationClient
{
    public class Validator
    {

        public static ValidationResult Validate(string keyvalSubDir = @"\Infosys\IAP\")
        {
            ValidationResult valResult = new ValidationResult();
            valResult.FeaturesAllowed = new List<Feature>();
            try
            {
                valResult.IsSuccess = true;
                valResult.FeaturesAllowed.Add(Feature.ObjectModelExplorer);
                valResult.FeaturesAllowed.Add(Feature.ScriptRepository);
                valResult.FeaturesAllowed.Add(Feature.WorkflowDesigner);
                return valResult;
            }
            catch
            {
                valResult.IsSuccess = false;
                valResult.FeaturesAllowed = null;
            }

            return valResult;
        }

    }

    public class ValidationResult
    {
        public bool IsSuccess { get; set; }
        public List<Feature> FeaturesAllowed { get; set; }
    }

    public enum Feature
    {
        WorkflowDesigner,
        ScriptRepository,
        ObjectModelExplorer
    }
}

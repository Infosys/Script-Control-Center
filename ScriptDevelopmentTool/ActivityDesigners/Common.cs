/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infosys.WEM.AutomationActivity.Designers
{
    public struct ApplicationConstants
    {
        public const string SCRIPT_REPO_SERVICEINTERFACE = "/iapwemservices/WEMScriptService.svc";
        public const string COMMON_SERVICEINTERFACE = "/iapwemservices/WEMCommonService.svc";
        public const string SECURITY_SERVICEINTERFACE = "/iapwemservices/WEMSecurityAccessService.svc";
        public const string WORKFLOW_SERVICEINTERFACE = "/iapwemservices/WEMService.svc";
        public const string PARAMDIRECTION_IN = "(In)";
        public const string PARAMDIRECTION_OUT = "(Out)";
        public const string PARAMDIRECTION_INANDOUT = "(InAndOut)";
        public const string VARIABLENAMEWF_PREFIX = "vwf_";
        public const string VARIABLENAMESCRIPT_PREFIX = "vsc_";
        public const string VARIABLENAMEWF_FORMAT = VARIABLENAMEWF_PREFIX + "{0}_{1}";
        public const string VARIABLENAMESCRIPT_FORMAT = VARIABLENAMESCRIPT_PREFIX  + "{0}_{1}";
        public const string ACTIVITYPLACEMENTNOTINSEQUENCE = "This Activity is required to be placed within a Sequence Activity. Only one such type of Activty can be contained in the same Sequence Activity.";
        public const string ACTIVITYPLACEMENTNOTINSEQUENCE_TITLE ="Incorrect Placement of Activity";
        public const string ACTIVITYVARIABLENOTCREATED = "Variable cannot be created. Please fix the issue before proceeding.";
        public const string ITEMDUPLICATE = "A {0} with Name {1} is found to be duplicate. Hence it cannot be added to this List. Rename the item with a unique name to make it available in this list.";
        public const string ITEMDUPLICATE_TITLE = "{0} Name Duplicate";
        public const string DELETEVARIABLES = "{0} Category or names have been updated. We have found previously configured Invoke{0} activity variable(s) in the variable list, In-Scope of this Sequence Activity.\n\n" +
                                                "Do you wish to Remove the Invoke{0} Variables which have been configured previously?\n\n" +
                                                "Please note, Clicking on Yes would also Delete other Invoke{0} Activity variables being used by another instance of the same activity within this Sequence. Do Review your variable list."; 
        public const string DELETEVARIABLES_WF_TITLE = "Invoke Workflow - Remove Variables";
        public const string DELETEVARIABLES_SC_TITLE = "Invoke Script - Remove Variables";
        public const string RESUME_WORKFLOW = "Workflow is resumed";
    }

}

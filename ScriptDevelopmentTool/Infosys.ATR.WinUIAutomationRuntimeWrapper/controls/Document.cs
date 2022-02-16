/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Automation;
using Infosys.WEM.Infrastructure.Common;

namespace Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls
{
    public class Document : Controls.Base.TextBoxBase
    {
        private string className = "Document";

        public Document(IntPtr appWinHandle, IntPtr screenWinHandle, string automationId, string automationName, string applicationTreePath, string applicationType, string fullControlQualifier)
            : base(appWinHandle, screenWinHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier)
        {
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.DOCUMENT))
            {
                //in param
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "appWinHandle", Logging.Constants.PARAMDIRECTION_IN, appWinHandle.ToString());
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "screenWinHandle", Logging.Constants.PARAMDIRECTION_IN, screenWinHandle.ToString());
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "automationId", Logging.Constants.PARAMDIRECTION_IN, automationId);
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "automationName", Logging.Constants.PARAMDIRECTION_IN, automationName);
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "applicationTreePath", Logging.Constants.PARAMDIRECTION_IN, applicationTreePath);
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "applicationType", Logging.Constants.PARAMDIRECTION_IN, applicationType);

                switch (this.ApplicationType.ToLower())
                {
                    case "java":
                        //Control_Reference.JavaControlReference = FindControl(this.ControlPath, true);
                        break;
                    default:
                        base.ControlCondition = PrepareCondition();
                        //Control_Reference.WinControlReference = FindControl(ControlCondition);
                        InitializeControlCondition(ControlCondition);
                        break;
                }
            }
            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.DOCUMENT);
        }

        private Condition PrepareCondition()
        {
            PropertyCondition documentControl = new PropertyCondition(
            AutomationElement.ControlTypeProperty,
            ControlType.Document);
            PropertyCondition automationNameProp = new PropertyCondition(
            AutomationElement.NameProperty,
            this.AutomationName);
            PropertyCondition automationIdProp = new PropertyCondition(
             AutomationElement.AutomationIdProperty,
            this.AutomationId);
            AndCondition documentControlCondition = new AndCondition(documentControl, automationNameProp, automationIdProp);
            return documentControlCondition;
        }
    }
}

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
    public class RadioButton : Controls.Base.ButtonBase
    {
        private string className = "RadioButton";

        public RadioButton(IntPtr appWinHandle, IntPtr screenWinHandle, string automationId, string automationName, string applicationTreePath, string applicationType, string fullControlQualifier)
            : base(appWinHandle, screenWinHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier)
        {
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.RADIOBUTTON))
            {
                //in param
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "appWinHandle", Logging.Constants.PARAMDIRECTION_IN, appWinHandle.ToString());
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "screenWinHandle", Logging.Constants.PARAMDIRECTION_IN, screenWinHandle.ToString());
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "automationId", Logging.Constants.PARAMDIRECTION_IN, automationId);
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "automationName", Logging.Constants.PARAMDIRECTION_IN, automationName);
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "applicationTreePath", Logging.Constants.PARAMDIRECTION_IN, applicationTreePath);
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "applicationType", Logging.Constants.PARAMDIRECTION_IN, applicationType);

                //base.ControlCondition = PrepareCondition();
                //Control_Reference.WinControlReference = FindControl(ControlCondition);
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
            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.RADIOBUTTON);

        }

        private Condition PrepareCondition()
        {
            PropertyCondition radiobuttonControl = new PropertyCondition(
            AutomationElement.ControlTypeProperty,
            ControlType.RadioButton);
            PropertyCondition automationNameProp = new PropertyCondition(
            AutomationElement.NameProperty,
            this.AutomationName);
            PropertyCondition automationIdProp = new PropertyCondition(
             AutomationElement.AutomationIdProperty,
            this.AutomationId);
            AndCondition radiobuttonControlCondition = new AndCondition(radiobuttonControl, automationNameProp, automationIdProp);
            return radiobuttonControlCondition;
        }

        public bool IsSelected
        {
            get
            {
                bool isSelected = false;
                if (Control_Reference.WinControlReference != null)
                {
                    object pattern;
                    if (Control_Reference.WinControlReference.TryGetCurrentPattern(SelectionItemPattern.Pattern, out pattern))
                    {
                        SelectionItemPattern selectionItemPattern = pattern as SelectionItemPattern;
                        isSelected = selectionItemPattern.Current.IsSelected;
                    }
                }
                else if (Control_Reference.JavaControlReference != null)
                {
                    isSelected = Control_Reference.JavaControlReference.states.ToLower().Contains("checked") ? true : false;
                }
                else if (this.ImageReference != null && !string.IsNullOrEmpty(this.ImageReference.CurrentState))
                {
                    bool currentState = false;
                    if (bool.TryParse(this.ImageReference.CurrentState, out currentState))
                    {
                        isSelected = currentState;
                        //discuss with Sid, to handle the scenario when the state mentioned in the atr file is not
                        //same as those supported by bool- isSelected
                        //N.B. for state, the atr is not populated thru available dropdown options but thru user enterred text
                    }
                    else
                    {
                        //after discussion it is finalized that we will assume the state in atr is same as expected,
                        //and in case the different than the know ones, then raise exception
                        string message = string.Format(Logging.ErrorMessages.CONTROL_STATE_NOTFOUND, this.GetType().ToString(), this.ImageReference.CurrentState);
                        LogHandler.LogError(message, LogHandler.Layer.Infrastructure, null);
                        throw new Exception(message);
                    }
                }
                return isSelected;
            }
        }

        public void Select()
        {
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.SELECT))
            {
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, className, Logging.Constants.SELECT);

                if (Control_Reference.WinControlReference != null)
                {
                    object pattern;
                    if (Control_Reference.WinControlReference.TryGetCurrentPattern(SelectionItemPattern.Pattern, out pattern))
                    {
                        SelectionItemPattern selectionItemPattern = pattern as SelectionItemPattern;
                        selectionItemPattern.Select();
                    }
                }
                else if (Control_Reference.JavaControlReference != null)
                {
                    this.Click();
                }
                else if (ImageReference != null && !string.IsNullOrEmpty(ImageReference.CurrentState))
                {
                    this.Click();
                }
            }
            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.SELECT);
        }
    }
}

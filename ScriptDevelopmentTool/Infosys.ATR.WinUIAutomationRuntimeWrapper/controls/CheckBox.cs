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
    public class CheckBox : Controls.Base.ButtonBase
    {
        TogglePattern toggleInstance;

        private string className = "CheckBox";

        public CheckBox(IntPtr appWinHandle, IntPtr screenWinHandle, string automationId, string automationName, string applicationTreePath, string applicationType, string fullControlQualifier)
            : base(appWinHandle, screenWinHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier)
        {
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.CHECKBOX))
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
            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.CHECKBOX);

        }

        private Condition PrepareCondition()
        {
            PropertyCondition checkBoxControl = new PropertyCondition(
            AutomationElement.ControlTypeProperty,
            ControlType.CheckBox);
            PropertyCondition automationNameProp = new PropertyCondition(
            AutomationElement.NameProperty,
            this.AutomationName);
            PropertyCondition automationIdProp = new PropertyCondition(
             AutomationElement.AutomationIdProperty,
            this.AutomationId);
            AndCondition checkBoxControlCondition = new AndCondition(checkBoxControl, automationNameProp, automationIdProp);
            return checkBoxControlCondition;
        }

        public enum ToggleState
        {
            Indeterminate,
            Off,
            On,
            UnInitialized
        }

        private bool InitializeState()
        {
            bool initialized = false;
            if (Control_Reference.WinControlReference != null)
            {
                object togglePattern;
                //if (base.ControlReference.TryGetCurrentPattern(
                //            TogglePattern.Pattern, out togglePattern))
                if (base.Control_Reference.WinControlReference.TryGetCurrentPattern(
                            TogglePattern.Pattern, out togglePattern))
                {
                    toggleInstance = (TogglePattern)togglePattern;
                    initialized = true;
                }
            }
            else if (Control_Reference.JavaControlReference != null)
            {

            }
            return initialized;
        }

        public ToggleState State
        {
            get
            {
                ToggleState state = ToggleState.UnInitialized;
                if (Control_Reference.WinControlReference != null)
                {
                    if (null != toggleInstance)
                    {
                        state = (ToggleState)toggleInstance.Current.ToggleState;
                    }
                    else
                    {
                        if (InitializeState())
                        {
                            state = (ToggleState)toggleInstance.Current.ToggleState;
                        }
                        else
                        {
                            state = ToggleState.UnInitialized;
                        }
                    }
                }
                else if (Control_Reference.JavaControlReference != null)
                {
                    state = Control_Reference.JavaControlReference.states.ToLower().Contains("checked") ? ToggleState.On : ToggleState.Off;
                }
                else if (this.ImageReference != null && !string.IsNullOrEmpty(this.ImageReference.CurrentState))
                {
                    ToggleState currentState = ToggleState.UnInitialized;
                    if (Enum.TryParse<ToggleState>(this.ImageReference.CurrentState, true, out currentState))
                    {
                        state = currentState;
                        //discuss with Sid, to handle the scenario when the state mentioned in the atr file is not
                        //same as those supported by the enum- ToggleState
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
                return state;

            }

        }

    }
}

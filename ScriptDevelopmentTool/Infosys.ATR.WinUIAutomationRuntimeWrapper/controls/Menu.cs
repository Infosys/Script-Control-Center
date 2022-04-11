﻿/****************************************************************
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
    public class Menu : Controls.Base.ListBase
    {
        private string className = "Menu";

        public Menu(IntPtr appWinHandle, IntPtr screenWinHandle, string automationId, string automationName, string applicationTreePath, string applicationType, string fullControlQualifier)
            : base(appWinHandle, screenWinHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier)
        {
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.MENU))
            {
                //in param
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "appWinHandle", Logging.Constants.PARAMDIRECTION_IN, appWinHandle.ToString());
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "screenWinHandle", Logging.Constants.PARAMDIRECTION_IN, screenWinHandle.ToString());
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "automationId", Logging.Constants.PARAMDIRECTION_IN, automationId);
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "automationName", Logging.Constants.PARAMDIRECTION_IN, automationName);
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "applicationTreePath", Logging.Constants.PARAMDIRECTION_IN, applicationTreePath);
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "applicationType", Logging.Constants.PARAMDIRECTION_IN, applicationType);

                //base.ControlCondition = PrepareCondition();
                //ControlReference = FindControl(ControlCondition);
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
            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.MENU);

        }

        private Condition PrepareCondition()
        {
            PropertyCondition menuitemControl = new PropertyCondition(
            AutomationElement.ControlTypeProperty,
            ControlType.MenuItem);
            PropertyCondition automationNameProp = new PropertyCondition(
            AutomationElement.NameProperty,
            this.AutomationName);
            PropertyCondition automationIdProp = new PropertyCondition(
             AutomationElement.AutomationIdProperty,
            this.AutomationId);
            AndCondition menuitemControlCondition = new AndCondition(menuitemControl, automationNameProp, automationIdProp);
            return menuitemControlCondition;
        }

        Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Base.ExpandCollapseState _currentState;

        public Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Base.ExpandCollapseState State
        {
            get
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Base.ExpandCollapseState currentState = Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Base.ExpandCollapseState.Collapsed;
                if (Control_Reference.WinControlReference != null)
                {
                    object pattern;
                    if (base.Control_Reference.WinControlReference.TryGetCurrentPattern(ExpandCollapsePattern.Pattern, out pattern))
                    {
                        ExpandCollapsePattern expcolPattern = pattern as ExpandCollapsePattern;
                        currentState = (Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Base.ExpandCollapseState)expcolPattern.Current.ExpandCollapseState;
                    }
                }
                else if (Control_Reference.JavaControlReference != null)
                {
                    currentState = _currentState;
                }
                else if (this.ImageReference != null && !string.IsNullOrEmpty(this.ImageReference.CurrentState))
                {
                    if (Enum.TryParse<Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Base.ExpandCollapseState>(this.ImageReference.CurrentState, true, out currentState))
                    {
                        _currentState = currentState;
                        //discuss with Sid, to handle the scenario when the state mentioned in the atr file is not
                        //same as those supported by the enum- Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Base.ExpandCollapseState
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
                return currentState;
            }
        }

        public void Expand()
        {
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.EXPAND))
            {
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, className, Logging.Constants.EXPAND);

                if (Control_Reference.WinControlReference != null)
                {
                    object pattern;
                    if (base.Control_Reference.WinControlReference.TryGetCurrentPattern(ExpandCollapsePattern.Pattern, out pattern))
                    {
                        ExpandCollapsePattern expcolPattern = pattern as ExpandCollapsePattern;
                        expcolPattern.Expand();
                    }
                }
                else if (Control_Reference.JavaControlReference != null)
                {
                    this.Click();
                    _currentState = Base.ExpandCollapseState.Expanded;
                }
                else if (ImageReference != null && !string.IsNullOrEmpty(ImageReference.CurrentState))
                {
                    Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Base.ExpandCollapseState currentState = Base.ExpandCollapseState.UnInitialized;
                    if (Enum.TryParse<Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Base.ExpandCollapseState>(ImageReference.CurrentState, true, out currentState))
                    {
                        if (currentState == Base.ExpandCollapseState.Collapsed)
                        {
                            this.Click();
                            _currentState = Base.ExpandCollapseState.Expanded;
                        }
                    }
                }
            }
            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.EXPAND);
        }

        public void Collapse()
        {
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.COLLAPSE))
            {
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, className, Logging.Constants.COLLAPSE);

                if (Control_Reference.WinControlReference != null)
                {
                    object pattern;
                    if (base.Control_Reference.WinControlReference.TryGetCurrentPattern(ExpandCollapsePattern.Pattern, out pattern))
                    {
                        ExpandCollapsePattern expcolPattern = pattern as ExpandCollapsePattern;
                        expcolPattern.Collapse();
                    }
                }
                else if (Control_Reference.JavaControlReference != null)
                {
                    this.Click();
                    _currentState = Base.ExpandCollapseState.Collapsed;
                }
                else if (ImageReference != null && !string.IsNullOrEmpty(ImageReference.CurrentState))
                {
                    Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Base.ExpandCollapseState currentState = Base.ExpandCollapseState.UnInitialized;
                    if (Enum.TryParse<Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Base.ExpandCollapseState>(ImageReference.CurrentState, true, out currentState))
                    {
                        if (currentState == Base.ExpandCollapseState.Expanded)
                        {
                            this.Click();
                            _currentState = Base.ExpandCollapseState.Collapsed;
                        }
                    }
                }
            }
            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.COLLAPSE);
        }
    }
}
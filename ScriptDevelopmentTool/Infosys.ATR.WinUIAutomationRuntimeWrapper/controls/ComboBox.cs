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
    public class ComboBox : Controls.Base.ListBase
    {
        public StructureChangedEventHandler UIStructureChangeEventHandler;
        private string className = "ComboBox";

        public ComboBox(IntPtr appWinHandle, IntPtr screenWinHandle, string automationId, string automationName, string applicationTreePath, string applicationType, string fullControlQualifier)
            : base(appWinHandle, screenWinHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier)
        {
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.COMBOBOX))
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
            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.COMBOBOX);

        }

        private Condition PrepareCondition()
        {
            PropertyCondition comboBoxControl = new PropertyCondition(
            AutomationElement.ControlTypeProperty,
            ControlType.ComboBox);
            PropertyCondition automationNameProp = new PropertyCondition(
            AutomationElement.NameProperty,
            this.AutomationName);
            PropertyCondition automationIdProp = new PropertyCondition(
             AutomationElement.AutomationIdProperty,
            this.AutomationId);
            AndCondition comboBoxControlCondition = new AndCondition(comboBoxControl, automationNameProp, automationIdProp);
            return comboBoxControlCondition;
        }

        //public enum ExpandCollapseState
        //{
        //    Collapsed,
        //    Expanded,
        //    LeafNode,
        //    PartiallyExpanded,
        //    UnInitialized
        //}

        private Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Base.ExpandCollapseState _ExpandCollapseState;

        public Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Base.ExpandCollapseState State
        {
            get
            {
                if (Control_Reference == null || (Control_Reference.WinControlReference == null && Control_Reference.JavaControlReference == null))
                {
                    _ExpandCollapseState = Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Base.ExpandCollapseState.UnInitialized;
                }
                else if (this.ImageReference != null && !string.IsNullOrEmpty(this.ImageReference.CurrentState))
                {
                    Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Base.ExpandCollapseState currentState = Base.ExpandCollapseState.UnInitialized;
                    if (Enum.TryParse<Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Base.ExpandCollapseState>(this.ImageReference.CurrentState, true, out currentState))
                    {
                        _ExpandCollapseState = currentState;
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
                return _ExpandCollapseState;
            }
            set { _ExpandCollapseState = value; }
        }

        public void Expand()
        {
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.EXPAND))
            {
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, className, Logging.Constants.EXPAND);

                if (Control_Reference.WinControlReference != null)
                {
                    AutomationPattern[] ap = base.Control_Reference.WinControlReference.GetSupportedPatterns();
                    if (!ap.Contains(SelectionPattern.Pattern))
                    {
                        // In web apps the web combo/dropdown control do not contain the list control as sub-control/composite of the combobox, hence the check
                        UIStructureChangeEventHandler = new StructureChangedEventHandler(ComboBoxControlStructurehanged);
                        Automation.AddStructureChangedEventHandler(AutomationElement.RootElement, TreeScope.Descendants, UIStructureChangeEventHandler);
                    }

                    object expcolPattern;
                    if (base.Control_Reference.WinControlReference.TryGetCurrentPattern(ExpandCollapsePattern.Pattern, out expcolPattern))
                    {
                        //Rahul- the below sleep is needed as quite sometime by the time expand is called, the concerned element is yet not enabled.
                        //not noticed in debug mode but noticed in run mode
                        System.Threading.Thread.Sleep(200);
                        if (Control_Reference.WinControlReference.Current.IsEnabled)
                            ((ExpandCollapsePattern)expcolPattern).Expand();
                        _ExpandCollapseState = (Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Base.ExpandCollapseState)((ExpandCollapsePattern)expcolPattern).Current.ExpandCollapseState;
                    }
                    else
                    {
                        //for combobox in web application, the atr file should have the entity config details for the
                        //the button inside the combobox with name and controlname both same as "Open"
                        Controls.Button btnComboBox = this.FindControl("Open") as Controls.Button;
                        btnComboBox.Click();

                    }

                }
                else if (Control_Reference.JavaControlReference != null)
                {
                    this.Click();
                    _ExpandCollapseState = Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Base.ExpandCollapseState.Expanded;
                }
                else if (ImageReference != null && !string.IsNullOrEmpty(ImageReference.CurrentState))
                {
                    Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Base.ExpandCollapseState currentState = Base.ExpandCollapseState.UnInitialized;
                    if (Enum.TryParse<Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Base.ExpandCollapseState>(ImageReference.CurrentState, true, out currentState))
                    {
                        if (currentState == Base.ExpandCollapseState.Collapsed)
                        {
                            this.Click();
                            _ExpandCollapseState = Base.ExpandCollapseState.Expanded;
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
                    object expcolPattern;
                    if (base.Control_Reference.WinControlReference.TryGetCurrentPattern(ExpandCollapsePattern.Pattern, out expcolPattern))
                    {
                        ((ExpandCollapsePattern)expcolPattern).Collapse();
                        _ExpandCollapseState = (Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Base.ExpandCollapseState)((ExpandCollapsePattern)expcolPattern).Current.ExpandCollapseState;
                    }
                    else
                    {
                        Controls.Button btnComboBox = new Controls.Button(IntPtr.Zero, new IntPtr(base.Control_Reference.WinControlReference.Current.NativeWindowHandle), this.AutomationId,
                            this.AutomationName, this.ControlPath, this.ApplicationType, this.FullControlQualifier);
                        btnComboBox.Click();
                        this.WebListCtrlWinHandle = IntPtr.Zero; // The list window has been closed
                    }
                }
                else if (Control_Reference.JavaControlReference != null)
                {
                    this.Click();
                    _ExpandCollapseState = Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Base.ExpandCollapseState.Collapsed;
                }
                else if (ImageReference != null && !string.IsNullOrEmpty(ImageReference.CurrentState))
                {
                    Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Base.ExpandCollapseState currentState = Base.ExpandCollapseState.UnInitialized;
                    if (Enum.TryParse<Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Base.ExpandCollapseState>(ImageReference.CurrentState, true, out currentState))
                    {
                        if (currentState == Base.ExpandCollapseState.Expanded)
                        {
                            this.Click();
                            _ExpandCollapseState = Base.ExpandCollapseState.Collapsed;
                        }
                    }
                }
            }
            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.COLLAPSE);
        }

        private void ComboBoxControlStructurehanged(object sender, StructureChangedEventArgs e)
        {
            AutomationElement automationElement = ((AutomationElement)sender);
            if (automationElement.Current.ControlType == ControlType.List)
            {
                this.WebListCtrlWinHandle = new IntPtr(automationElement.Current.NativeWindowHandle);
            }
            //Unregister the event subscription of the UI structure change event
            UIStructureChangeEventHandler = null;
        }

    }
}

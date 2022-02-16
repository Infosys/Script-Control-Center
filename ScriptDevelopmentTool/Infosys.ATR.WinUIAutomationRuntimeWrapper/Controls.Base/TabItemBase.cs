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
using System.Windows.Automation;
using System.Windows.Forms;
using Infosys.WEM.Infrastructure.Common;

namespace Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Base
{
    public abstract class TabItemBase: Control
    {
        private Condition _condition;

        ControlReferenceBase _Control_Reference = new ControlReferenceBase();
        protected ControlReferenceBase Control_Reference
        {
            get
            {
                _Control_Reference.JavaControlReference = JavaControlElementFound;
                _Control_Reference.WinControlReference = WinControlElementFound;
                return _Control_Reference;
            }
            set { _Control_Reference = value; }
        }

        public TabItemBase(IntPtr appWinHandle, IntPtr screenWinHandle, string automationId, string automationName,
            string applicationTreePath, string applicationType, string fullControlQualifier)
            : base(appWinHandle, screenWinHandle)
        {
            this.AutomationId = automationId;
            this.AutomationName = automationName;
            this.ControlPath = applicationTreePath;
            this.ApplicationType = applicationType;
            this.FullControlQualifier= fullControlQualifier;
        }

        protected Condition ControlCondition
        {
            get { return _condition; }
            set { _condition = value; }
        }

        public string ReceiveText()
        {
            string fieldValue = "";
            if (null != Control_Reference.WinControlReference)
            {
                object textPattern;
                if (Control_Reference.WinControlReference.TryGetCurrentPattern(
                    TextPattern.Pattern, out textPattern))
                {
                    if (Control_Reference.WinControlReference.Current.IsKeyboardFocusable)
                        Control_Reference.WinControlReference.SetFocus();
                    System.Windows.Automation.Text.TextPatternRange rng = ((TextPattern)textPattern).DocumentRange;
                    fieldValue = rng.GetText(-1);
                }

                else
                {
                    fieldValue = Control_Reference.WinControlReference.Current.Name;
                }
            }
            else if (Control_Reference.JavaControlReference != null)
            {
                fieldValue = Control_Reference.JavaControlReference.textValue;
            }
            return fieldValue;
        }

        public bool IsSelected()
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
                //TBD for java app
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

        public void Select()
        {
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
    }
}

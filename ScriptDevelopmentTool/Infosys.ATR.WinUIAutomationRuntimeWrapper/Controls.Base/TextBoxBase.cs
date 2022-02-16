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
using System.Windows.Forms;

namespace Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Base
{
    public abstract class TextBoxBase : Control
    {
        private Condition _condition;

        //protected AutomationElement ControlReference;
        //protected ControlReferenceBase Control_Reference = new ControlReferenceBase();
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

        public TextBoxBase(IntPtr appWinHandle, IntPtr screenWinHandle, string automationId, string automationName,
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

        public bool SendText(string messageToSend)
        {
            bool result = true;
            if (null != Control_Reference.WinControlReference)
            {
                //Automation.AddAutomationPropertyChangedEventHandler(IT, TreeScope.Element, TxtBoxEventHandler, objParam);
                object valuePattern;
                //ValuePattern valPattern;
                // Control does not support the ValuePattern pattern  
                // so use keyboard input to insert content. 
                // 
                // NOTE: Elements that support TextPattern  
                //       do not support ValuePattern and TextPattern 
                //       does not support setting the text of  
                //       multi-line edit or document controls. 
                //       For this reason, text input must be simulated 
                //       using one of the following methods. 
                //        
                if (!Control_Reference.WinControlReference.TryGetCurrentPattern(
                    ValuePattern.Pattern, out valuePattern))
                {
                    //TODO TEst for web app
                    // Set focus for input functionality and begin.
                    try
                    {
                        if (Control_Reference.WinControlReference.Current.IsKeyboardFocusable)
                            Control_Reference.WinControlReference.SetFocus();

                        // Pause before sending keyboard input.
                        System.Threading.Thread.Sleep(100);

                        // Delete existing content in the control and insert new content.
                        SendKeys.SendWait("^{HOME}");   // Move to start of control
                        SendKeys.SendWait("^+{END}");   // Select everything
                        SendKeys.SendWait("{DEL}");     // Delete selection
                        SendKeys.SendWait(messageToSend);
                    }
                    catch
                    {
                        result = false;
                        //this approach of try-catch is needed as not all text area support setfocus
                    }
                }
                // Control supports the ValuePattern pattern so we can  
                // use the SetValue method to insert content. 
                else
                {

                    // Set focus for input functionality and begin.
                    if (Control_Reference.WinControlReference.Current.IsKeyboardFocusable)
                    {
                        Control_Reference.WinControlReference.SetFocus();
                        ((ValuePattern)valuePattern).SetValue(messageToSend);
                    }
                    else
                    {
                        this.Click();
                        // Delete existing content in the control and insert new content.
                        SendKeys.SendWait("^{HOME}");   // Move to start of control
                        SendKeys.SendWait("^+{END}");   // Select everything
                        SendKeys.SendWait("{DEL}");     // Delete selection
                        SendKeys.SendWait(messageToSend);
                    }

                    
                }
            }
            else if (Control_Reference.JavaControlReference != null || ImageReference != null)
            {
                this.Click();
                // Delete existing content in the control and insert new content.
                SendKeys.SendWait("^{HOME}");   // Move to start of control
                SendKeys.SendWait("^+{END}");   // Select everything
                SendKeys.SendWait("{DEL}");     // Delete selection
                SendKeys.SendWait(messageToSend);
            }

            //else if (ImageReference != null)
            //{ }

            return result;
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


    }
}

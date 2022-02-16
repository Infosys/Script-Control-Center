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

namespace Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Base
{
    public class CustomBase: Control
    {
        Condition _condition;
        //protected AutomationElement ControlReference;
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

        protected Condition ControlCondition
        {
            get { return _condition; }
            set { _condition = value; }
        }

        public CustomBase(IntPtr appWinHandle, IntPtr screenWinHandle, string automationId, string automationName,
            string applicationTreePath, string applicationType, string fullControlQualifier)
            : base(appWinHandle, screenWinHandle)
        {
            this.AutomationId = automationId;
            this.AutomationName = automationName;
            this.ControlPath = applicationTreePath;
            this.ApplicationType = applicationType;
            this.FullControlQualifier = fullControlQualifier;
        }
    }
}

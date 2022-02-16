/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IMSWorkBench.Infrastructure.Interface.Services
{    

    public class GenericException :Exception
    {
        private bool _showMessage = true;
        private bool _exitApplication = false;

        public bool ShowMessage
        {
            get { return _showMessage; }
            set { _showMessage = value; }
        }

        public bool ExitApplication
        {
            get { return _exitApplication; }
            set { _exitApplication = value; }
        }
        
        public GenericException() :base()
        { }

        public GenericException (string message):base(message)
        { }

        public GenericException (string message,bool showMessage, bool exitApplication) : this(message)
        {
            this._showMessage = showMessage;
            this._exitApplication = exitApplication;
        }
    }
}

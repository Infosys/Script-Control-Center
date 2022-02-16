/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
#region Using Directives

using System;
using System.Diagnostics;

#endregion Using Directives


namespace ScintillaNET.Internal
{
    //
    // EventArgs for the SCNotification event
    //
    internal sealed class SCNotificationEventArgs : EventArgs
    {
        #region Fields

        private NativeMethods.SCNotification _notification;

        #endregion Fields


        #region Propeties

        public NativeMethods.SCNotification Notification
        {
            get
            {
                return _notification;
            }
        }

        #endregion Properties


        #region Constructors

        public SCNotificationEventArgs(NativeMethods.SCNotification notification)
        {
            _notification = notification;
        }

        #endregion Constructors
    }
}

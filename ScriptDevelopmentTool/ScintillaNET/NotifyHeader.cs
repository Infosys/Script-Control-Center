/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
#region Using Directives

using System;
using System.Runtime.InteropServices;

#endregion Using Directives


namespace ScintillaNET
{
    /// <summary>
    ///     This matches the Win32 NMHDR structure
    /// </summary>
    [Obsolete("This type will not be public in future versions.")]
    [StructLayout(LayoutKind.Sequential)]
    public struct NotifyHeader
    {
        public IntPtr hwndFrom; // environment specific window handle/pointer
        public IntPtr idFrom;   // CtrlID of the window issuing the notification
        public uint code;       // The SCN_* notification code
    }
}

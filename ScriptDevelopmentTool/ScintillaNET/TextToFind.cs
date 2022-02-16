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
    // TODO Make internal
    [StructLayout(LayoutKind.Sequential)]
    public struct TextToFind
    {
        public CharacterRange chrg;         // range to search
        public IntPtr lpstrText;            // the search pattern (zero terminated)
        public CharacterRange chrgText;     // returned as position of matching text
    }
}

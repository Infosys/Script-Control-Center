/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
#region Using Directives

using System;

#endregion Using Directives


namespace ScintillaNET
{
    public enum CaretPolicy
    {
        Slop = 0x01,
        Strict = 0x04,
        Jumps = 0x10,
        Even = 0x08,
    }
}

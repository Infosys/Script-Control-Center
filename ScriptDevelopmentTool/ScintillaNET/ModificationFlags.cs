﻿/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
#region Using Directives

using System;

#endregion Using Directives


namespace ScintillaNET
{
    public enum ModificationFlags
    {
        InsertText = 0x1,
        DeleteText = 0x2,
        ChangeStyle = 0x4,
        ChangeFold = 0x8,
        User = 0x10,
        Undo = 0x20,
        Redo = 0x40,
        StepInUndoRedo = 0x100,
        ChangeMarker = 0x200,
        BeforeInsert = 0x400,
        BeforeDelete = 0x800,
    }
}

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
    /// <summary>
    ///     Document's EndOfLine Mode
    /// </summary>
    public enum EndOfLineMode
    {
        /// <summary>
        ///     Carriage Return + Line Feed (Windows Style)
        /// </summary>
        Crlf = 0,

        /// <summary>
        ///     Carriage Return Only (Mac Style)
        /// </summary>
        CR = 1,

        /// <summary>
        ///     Line Feed Only (Unix Style)
        /// </summary>
        LF = 2,
    }
}

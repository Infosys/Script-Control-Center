﻿/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
#region Using Directives

using System;
using ScintillaNET.Internal;

#endregion Using Directives


namespace ScintillaNET
{
    /// <summary>
    ///     Specifies how wrapped lines are indented when line wrapping is enabled in a <see cref="Scintilla" /> control.
    /// </summary>
    public enum LineWrappingIndentMode
    {
        /// <summary>
        ///     Wrapped lines are aligned on the left and indented by the amount
        ///     spcified in the <see cref="LineWrapping.Indent" /> property.
        /// </summary>
        Fixed = NativeMethods.SC_WRAPINDENT_FIXED,

        /// <summary>
        ///     Wrapped lines are aligned to the first subline indent.
        /// </summary>
        Same = NativeMethods.SC_WRAPINDENT_SAME,

        /// <summary>
        ///     Wrapped lines are aligned to the first subline indent plus
        ///     one more level of indentation.
        /// </summary>
        Indent = NativeMethods.SC_WRAPINDENT_INDENT
    }
}

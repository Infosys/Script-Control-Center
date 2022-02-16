/****************************************************************
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
    ///     Specifies the locations of line wrapping visual glyphs in a <see cref="Scintilla" /> control.
    /// </summary>
    [Flags]
    public enum LineWrappingVisualFlagsLocations
    {
        /// <summary>
        ///     Line wrapping glyphs are drawn near the control border.
        /// </summary>
        Default = NativeMethods.SC_WRAPVISUALFLAGLOC_DEFAULT,

        /// <summary>
        ///     Line wrapping glyphs are drawn at the end of wrapped lines near the text.
        /// </summary>
        EndByText = NativeMethods.SC_WRAPVISUALFLAGLOC_END_BY_TEXT,

        /// <summary>
        ///     Line wrapping glyphs are drawn at the start of wrapped lines near the text.
        /// </summary>
        StartByText = NativeMethods.SC_WRAPVISUALFLAGLOC_START_BY_TEXT,
    }
}

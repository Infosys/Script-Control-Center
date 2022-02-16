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
    ///     Specifies how line wrapping visual glyphs are displayed in a <see cref="Scintilla" /> control.
    /// </summary>
    [Flags]
    public enum LineWrappingVisualFlags
    {
        /// <summary>
        ///     No line wrapping glyphs are displayed.
        /// </summary>
        None = NativeMethods.SC_WRAPVISUALFLAG_NONE,

        /// <summary>
        ///     Line wrapping glyphs are displayed at the end of wrapped lines.
        /// </summary>
        End = NativeMethods.SC_WRAPVISUALFLAG_END,

        /// <summary>
        ///     Line wrapping glyphs are displayed at the start of wrapped lines. This also has
        ///     the effect of indenting the line by one additional unit to accommodate the glyph.
        /// </summary>
        Start = NativeMethods.SC_WRAPVISUALFLAG_START,
    }
}

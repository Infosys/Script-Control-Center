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
    ///     Specifies the line layout caching strategy used by a <see cref="Scintilla" /> control.
    /// </summary>
    public enum LayoutCacheMode
    {
        /// <summary>
        ///     No line layout data is cached.
        /// </summary>
        None = NativeMethods.SC_CACHE_NONE,

        /// <summary>
        ///     Line layout data of the current caret line is cached.
        /// </summary>
        Caret = NativeMethods.SC_CACHE_CARET,

        /// <summary>
        ///     Line layout data for all visible lines and the current caret line are cached.
        /// </summary>
        Page = NativeMethods.SC_CACHE_PAGE,

        /// <summary>
        ///     Line layout data for the entire document is cached.
        /// </summary>
        Document = NativeMethods.SC_CACHE_DOCUMENT,
    }
}

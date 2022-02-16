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
    ///     Style of smart indent
    /// </summary>
    public enum SmartIndent
    {
        /// <summary>
        ///     No smart indent
        /// </summary>
        None = 0,

        /// <summary>
        ///     C++ style indenting
        /// </summary>
        CPP = 1,

        /// <summary>
        ///     Alternate C++ style indenting
        /// </summary>
        CPP2 = 4,

        /// <summary>
        ///     Block indenting, the last indentation is retained in new lines
        /// </summary>
        Simple = 2
    }
}

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
    ///     Defines character classifications in a <see cref="Scintilla" /> control.
    /// </summary>
    public enum CharClassification
    {
        /// <summary>
        ///     Characters with this classification are interpreted as whitespace characters.
        /// </summary>
        Whitespace = 0,

        /// <summary>
        ///     Characters with this classification are interpreted as word characters.
        /// </summary>
        Word = 2,

        /// <summary>
        ///     Characters with this classification are interpreted as punctuation characters.
        /// </summary>
        Punctuation = 3
    }
}

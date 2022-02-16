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
    ///     Specifies how indicators are drawn on text.
    /// </summary>
    public enum IndicatorDrawMode
    {
        /// <summary>
        ///     Indicators are drawn under text.
        /// </summary>
        Underlay = 0,

        /// <summary>
        ///     Indicators are drawn over text.
        /// </summary>
        Overlay = 1
    }
}

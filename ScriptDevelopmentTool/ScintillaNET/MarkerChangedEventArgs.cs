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
    ///     Provides data for the MarkerChanged event
    /// </summary>
    public class MarkerChangedEventArgs : ModifiedEventArgs
    {
        #region Fields

        private int _line;

        #endregion Fields


        #region Properties

        /// <summary>
        ///     Returns the line number where the marker change occured
        /// </summary>
        public int Line
        {
            get
            {
                return _line;
            }
            set
            {
                _line = value;
            }
        }

        #endregion Properties


        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the LinesNeedShownEventArgs class.
        /// </summary>
        /// <param name="line">Line number where the marker change occured</param>
        /// <param name="modificationType">What type of Scintilla modification occured</param>
        public MarkerChangedEventArgs(int line, int modificationType) : base(modificationType)
        {
            _line = line;
        }

        #endregion Constructors
    }
}

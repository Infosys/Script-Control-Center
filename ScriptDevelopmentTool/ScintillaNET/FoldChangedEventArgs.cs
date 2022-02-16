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
    ///     Provides data for the FoldChanged event
    /// </summary>
    public class FoldChangedEventArgs : ModifiedEventArgs
    {
        #region Fields

        private int _line;
        private int _newFoldLevel;
        private int _previousFoldLevel;

        #endregion Fields


        #region Properties

        /// <summary>
        ///     Gets/Sets the Line # that the fold change occured on
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


        /// <summary>
        ///     Gets the new Fold Level of the line
        /// </summary>
        public int NewFoldLevel
        {
            get
            {
                return _newFoldLevel;
            }
        }


        /// <summary>
        ///     Gets the previous Fold Level of the line
        /// </summary>
        public int PreviousFoldLevel
        {
            get
            {
                return _previousFoldLevel;
            }

        }

        #endregion Properties


        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the FoldChangedEventArgs class.
        /// </summary>
        /// <param name="line">Line # that the fold change occured on</param>
        /// <param name="newFoldLevel">new Fold Level of the line</param>
        /// <param name="previousFoldLevel">previous Fold Level of the line</param>
        /// <param name="modificationType">What kind of fold modification occured</param>
        public FoldChangedEventArgs(int line, int newFoldLevel, int previousFoldLevel, int modificationType) : base(modificationType)
        {
            _line = line;
            _newFoldLevel = newFoldLevel;
            _previousFoldLevel = previousFoldLevel;
        }

        #endregion Constructors
    }
}

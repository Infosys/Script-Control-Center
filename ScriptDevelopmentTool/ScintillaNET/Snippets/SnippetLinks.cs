/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
#region Using Directives

using System;
using System.Collections.Generic;

#endregion Using Directives


namespace ScintillaNET
{
    public class SnippetLink
    {
        #region Fields

        private string _key;
        private List<SnippetLinkRange> _ranges = new List<SnippetLinkRange>();

        #endregion Fields


        #region Properties

        public string Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
            }
        }


        public List<SnippetLinkRange> Ranges
        {
            get
            {
                return _ranges;
            }
            set
            {
                _ranges = value;
            }
        }

        #endregion Properties


        #region Constructors

        public SnippetLink(string key)
        {
            _key = key;
        }

        #endregion Constructors
    }
}

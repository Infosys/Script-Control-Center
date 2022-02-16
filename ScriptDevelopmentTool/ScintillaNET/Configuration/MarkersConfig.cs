/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
#region Using Directives

using System;
using System.Drawing;

#endregion Using Directives

namespace ScintillaNET.Configuration
{
    public class MarkersConfig
    {
        #region Fields

        private int? _alpha;
        private Color _backColor;
        private Color _foreColor;
        private bool? _inherit;
        private string _name;
        private int? _number;
        private MarkerSymbol? _symbol;

        #endregion Fields


        #region Properties

        public int? Alpha
        {
            get
            {
                return _alpha;
            }
            set
            {
                _alpha = value;
            }
        }


        public Color BackColor
        {
            get
            {
                return _backColor;
            }
            set
            {
                _backColor = value;
            }
        }


        public Color ForeColor
        {
            get
            {
                return _foreColor;
            }
            set
            {
                _foreColor = value;
            }
        }


        public bool? Inherit
        {
            get
            {
                return _inherit;
            }
            set
            {
                _inherit = value;
            }
        }


        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }


        public int? Number
        {
            get
            {
                return _number;
            }
            set
            {
                _number = value;
            }
        }


        public MarkerSymbol? Symbol
        {
            get
            {
                return _symbol;
            }
            set
            {
                _symbol = value;
            }
        }

        #endregion Properties
    }
}

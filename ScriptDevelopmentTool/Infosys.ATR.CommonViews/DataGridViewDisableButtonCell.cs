/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Infosys.ATR.CommonViews
{
    /// <summary>
    /// This class derives from DataGridViewButtonColumn and sets the cell template to DataGridViewDisableButtonCell class.
    /// </summary>
    public class DataGridViewDisableButtonColumn : DataGridViewButtonColumn
    {
        public DataGridViewDisableButtonColumn()
        {
            this.CellTemplate = new DataGridViewDisableButtonCell();
        }
    }

    /// <summary>
    /// This class derives from DataGridViewButtonCell class and contains properties and methods to override enabled
    /// property of cell.
    /// </summary>
    public class DataGridViewDisableButtonCell : DataGridViewButtonCell
    {
        private bool enabled;
        /// <summary>
        /// This property is used to enable/disable button cell.
        /// </summary>
        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                enabled = value;
            }
        }

        /// <summary>
        /// This method overrides the Clone method so that the Enabled property is copied. 
        /// </summary>
        /// <returns>Object of DataGridViewDisableButtonCell containing cloned property</returns>
        public override object Clone()
        {
            DataGridViewDisableButtonCell btnCell = (DataGridViewDisableButtonCell)base.Clone();
            btnCell.Enabled = this.Enabled;
            return btnCell;
        }

        /// <summary>
        /// The constructor sets the default button cell enabled value to true.
        /// </summary>
        public DataGridViewDisableButtonCell()
        {
            this.enabled = true;
        }

        /// <summary>
        /// This method contains the necessary logic to enable the button cell.
        /// </summary>
        /// <param name="graphics">Graphics object </param>
        /// <param name="clipBounds">Rectangle object containing clipbounds</param>
        /// <param name="cellBounds">Rectangle object containing cell bounds</param>
        /// <param name="rowIndex">Selected row index</param>
        /// <param name="elementState">DataGridViewElementStates object</param>
        /// <param name="value">Object value containing enabled/disabled</param>
        /// <param name="formattedValue">formatted value</param>
        /// <param name="errorText">Error text (if any)</param>
        /// <param name="cellStyle">DataGridViewCellStyle object</param>
        /// <param name="advBorderStyle">border style</param>
        /// <param name="paintParts">paint the cell</param>
        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex,
            DataGridViewElementStates elementState, object value, object formattedValue, string errorText,
            DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advBorderStyle, DataGridViewPaintParts paintParts)
        {
            if (!this.enabled)
            {
                // If specified, draw the cell background
                if ((paintParts & DataGridViewPaintParts.Background) == DataGridViewPaintParts.Background)
                {
                    SolidBrush cellBackground = new SolidBrush(cellStyle.BackColor);
                    graphics.FillRectangle(cellBackground, cellBounds);
                    cellBackground.Dispose();
                }

                // If specified, draw the cell borders
                if ((paintParts & DataGridViewPaintParts.Border) == DataGridViewPaintParts.Border)
                {
                    PaintBorder(graphics, clipBounds, cellBounds, cellStyle, advBorderStyle);
                }

                // Calculate the area in which to draw the button.
                Rectangle btnArea = cellBounds;
                Rectangle btnAdjustment = this.BorderWidths(advBorderStyle);
                btnArea.X += btnAdjustment.X;
                btnArea.Y += btnAdjustment.Y;
                btnArea.Height -= btnAdjustment.Height;
                btnArea.Width -= btnAdjustment.Width;

                // This draws the disabled button                
                ButtonRenderer.DrawButton(graphics, btnArea, PushButtonState.Disabled);

                // This draws the disabled button text 
                if (this.FormattedValue is String)
                {
                    TextRenderer.DrawText(graphics, (string)this.FormattedValue, this.DataGridView.Font,
                        btnArea, SystemColors.GrayText);
                }
            }
            else
            {
                // Base class to handle the paint event for dispaying enabled button 
                base.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText,
                    cellStyle, advBorderStyle, paintParts);
            }
        }
    }
}
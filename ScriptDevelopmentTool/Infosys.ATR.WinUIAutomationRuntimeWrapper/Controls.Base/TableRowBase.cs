/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Base
{
    public class TableRowBase
    {
        List<TableCell> cells = new List<TableCell>();

        public List<TableCell> Cells
        {
            get { return cells; }
            set { cells = value; }
        }
        
        public List<TableCell> GetAllCells()
        {
            return cells;
        }

        public TableCell GetCellAt(int index)
        {
            return cells[index];
        }

        public List<int> GetCellIndexesWithtext(string celltext)
        {
            List<int> indexes = null;
            if(cells != null && cells.Count>0)
            {
                indexes = new List<int>();
                for(int i=0; i< cells.Count; i++)
                {
                    if (cells[i].ControlElementFound.Current.Name == celltext)
                        indexes.Add(i);
                }
            }

            return indexes;
        }
    }
}

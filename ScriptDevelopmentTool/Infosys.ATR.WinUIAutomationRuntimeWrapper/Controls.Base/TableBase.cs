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
using System.Windows.Automation;

namespace Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Base
{
    public class TableBase: Control
    {
        Condition _condition;
        ControlReferenceBase _Control_Reference = new ControlReferenceBase();
        int cellsPerRow = 1;
        List<TableRow> rows = null;

        public int CellsPerRow
        {
            get { return cellsPerRow; }
            set { cellsPerRow = value; }
        }
        protected ControlReferenceBase Control_Reference
        {
            get
            {
                _Control_Reference.JavaControlReference = JavaControlElementFound;
                _Control_Reference.WinControlReference = WinControlElementFound;
                return _Control_Reference;
            }
            set { _Control_Reference = value; }
        }

        protected Condition ControlCondition
        {
            get { return _condition; }
            set { _condition = value; }
        }

        public TableBase(IntPtr appWinHandle, IntPtr screenWinHandle, string automationId, string automationName,
            string applicationTreePath, string applicationType, string fullControlQualifier)
            : base(appWinHandle, screenWinHandle)
        {
            this.AutomationId = automationId;
            this.AutomationName = automationName;
            this.ControlPath = applicationTreePath;
            this.ApplicationType = applicationType;
            this.FullControlQualifier = fullControlQualifier;
        }

        public List<TableRow> GetAllRows()
        {            
            if (this.Control_Reference.WinControlReference != null)
            {
                PropertyCondition cellControl = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Custom);
                AutomationElementCollection ctls = this.Control_Reference.WinControlReference.FindAll(TreeScope.Subtree, cellControl);
                if (ctls != null && ctls.Count > 0)
                {
                    rows = new List<TableRow>();
                    if (ctls.Count % cellsPerRow != 0)
                        throw new Exception("TableBase- Incorrect count provided for Cells Per Row");

                    for (int i = 0; i < ctls.Count; i++)
                    {
                        TableRow row = new TableRow();
                        row.Cells = new List<TableCell>();
                        for (int j = 0; j < cellsPerRow; j++)
                        {
                            TableCell cell = new TableCell((IntPtr)this.AppWindowHandle, (IntPtr)this.ScreenWindowHandle, ctls[i + j].Current.AutomationId, ctls[i + j].Current.Name, "", this.ApplicationType, "");
                            cell.ControlElementFound = ctls[i + j];
                            row.Cells.Add(cell);
                        }
                        i = i + cellsPerRow -1;
                        rows.Add(row);
                    }
                }
            }
            return rows;
        }

        public TableRow GetRowAt(int index)
        {
            TableRow row = null;
            if (rows == null || rows.Count == 0)
                rows = GetAllRows();
            if (rows != null && rows.Count > 0)
            {
                if (index < rows.Count && index >=0)
                    row = rows[index];
                else
                    throw new Exception("TableBase- Index provided is outside the Rows available");
            }
            return row;
        }

        public List<TableRow> GetRowsWithCellText(string celltext)
        {
            List<TableRow> rows = null;
            if (rows == null || rows.Count == 0)
                rows = GetAllRows();
            if (rows != null && rows.Count>0)
            {
                foreach (TableRow row in rows)
                {
                    foreach (TableCell cell in row.Cells)
                    {
                        if (cell.ControlElementFound.Current.Name == celltext)
                        {
                            rows.Add(row);
                            break;
                        }
                    }
                }
            }
            return rows;
        }


    }
}

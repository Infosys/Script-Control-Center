using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infosys.WEM.ScriptExecutionLibrary;
using Infosys.ATR.CommonViews;

namespace Infosys.ATR.ScriptsRepository.Views.Scripts
{
    public partial class ScriptOutputDisplay : Form
    {
        internal static string serverName = "";
        public ScriptOutputDisplay()
        {
            InitializeComponent();
        }

        public ScriptOutputDisplay(List<ExecutionResult> consolidatedOutput)
        {
            InitializeComponent();
            List<ExecutionResultView> executionResultView = new List<ExecutionResultView>();
            ExecutionResultView view = null;
            string[] arrServerList = serverName.Split(',');
            int rowIndex = 0;
            foreach (ExecutionResult result in consolidatedOutput)
            {
                view = new ExecutionResultView();
                view.SuccessMessage = result.SuccessMessage;
                view.ErrorMessage = result.ErrorMessage;
                view.IsSuccess = result.IsSuccess;
                view.ServerName = arrServerList[rowIndex];
                executionResultView.Add(view);
                rowIndex = rowIndex + 1;
            }

            OutputView ctrlOutputView = new OutputView();
            ctrlOutputView.Display(executionResultView);
            ctrlOutputView.Dock = DockStyle.Fill;
            this.panel1.Controls.Add(ctrlOutputView);
        }

        //public void GenerateOutput(List<ExecutionResult> consolidatedOutput)
        //{

        //    DataTable table = new DataTable();
        //    table.Columns.Add("Server");
        //    table.Columns.Add("Result");
        //    table.Columns.Add("Output");

        //    string[] arrServerList = serverName.Split(',');
        //    int count = 0;
        //    foreach (ExecutionResult output in consolidatedOutput)
        //    {

        //        DataRow dr = table.NewRow();
        //        dr["Server"] = arrServerList[count];

        //        if (output.IsSuccess)
        //        {
        //            dr["Result"] = "Sucess";
        //            dr["Output"] = output.SuccessMessage;
        //        }
        //        else
        //        {
        //            dr["Result"] = "Fail";
        //            dr["Output"] = output.ErrorMessage;
        //        }
        //        table.Rows.Add(dr);
        //        count = count + 1;
        //    }
        //    dataGridView1.DataSource = table;

        //    dataGridView1.Columns[0].Width = 100;
        //    dataGridView1.Columns[1].Width = 50;
        //    dataGridView1.Columns[2].Width = 550;

        //    dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        //    dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

        //    DataGridViewCell cell = new DataGridViewTextBoxCell();

        //    foreach (DataGridViewColumn col in dataGridView1.Columns)
        //    {
        //        col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        //        col.CellTemplate = cell;
        //    }


        //    int grdHeight = 65;
        //    int rowHeight = dataGridView1.Rows[0].Height;

        //    if (dataGridView1.Rows.Count > 5)
        //    {
        //        grdHeight = (rowHeight * 5) + 5;
        //        this.Height = grdHeight + 40;
        //        dataGridView1.ScrollBars = ScrollBars.Vertical;
        //    }
        //    else
        //    {
        //        grdHeight = grdHeight + (rowHeight * dataGridView1.Rows.Count);
        //        this.Height = grdHeight;
        //    }

        //   dataGridView1.ClientSize = new System.Drawing.Size(dataGridView1.Width, grdHeight);

        //    dataGridView1.DefaultCellStyle.SelectionBackColor = dataGridView1.DefaultCellStyle.BackColor;
        //    dataGridView1.DefaultCellStyle.SelectionForeColor = dataGridView1.DefaultCellStyle.ForeColor;
        //    this.Controls.Add(dataGridView1);
            
        //    // Form properties
        //    this.Padding = new Padding(0, 0, 0, 0);
        //    this.MinimizeBox = false;
        //    this.MaximizeBox = false;          
        //    this.StartPosition = FormStartPosition.CenterScreen;
        //    this.FormBorderStyle = FormBorderStyle.FixedDialog;
        //}

        //private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        //{
        //    //for (int i = 0; i < dataGridView1.Rows.Count; i++)
        //    {
        //        dataGridView1.Rows[0].Frozen = true;
        //    }

        //}

    }
}

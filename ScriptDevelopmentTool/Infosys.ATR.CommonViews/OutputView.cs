using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Threading;
using System.ComponentModel;
using System.Text;

namespace Infosys.ATR.CommonViews
{
    public partial class OutputView : UserControl
    {
        private const int ServerColumn = 0;
        private const int ResultColumn = 1;
        private const int StatusLinkColumn = 2;
        private const int DataLinkColumn = 3;

        private const int ProgressColumn = 4;
        private const int StatusColumn = 5;
        private const int DataColumn = 6;
        private const int CopyStatusBtn = 7;
        private const int CopyDataBtn = 8;

        // private DataGridView dataGridView1 = null;
        private static int grdHeight = 65;
        private static int grdWidth = 800;
        public static List<ExecutionResultView> consolidatedOutput = null;

        public delegate void CloseOutputViewHandler();
        public event CloseOutputViewHandler CloseOutputView;

        //Maps the output grid row to scriptID, this should help to have a single row with all output consolidated against it
        private static Dictionary<string, int> outputScriptMap = new Dictionary<string, int>();
        private MemoryStream mem = null;
        private StreamWriter writerMem = null;
        
        bool bgWorker = false;
        System.ComponentModel.BackgroundWorker worker = new System.ComponentModel.BackgroundWorker();

        public OutputView()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;

            mem = new MemoryStream(1000);
            writerMem = new StreamWriter(mem);
            writerMem.AutoFlush = true;
            Console.SetOut(writerMem);            
            txtConsoleOutput.Dock = DockStyle.Fill;

            worker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);
            worker.ProgressChanged += new ProgressChangedEventHandler(bgWorker_ProgressChanged);
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerAsync();      
        }
        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker sendingWorker = (BackgroundWorker)sender;//Capture the BackgroundWorker that fired the event

            while (!bgWorker)
            {
                if (!sendingWorker.CancellationPending)//At each iteration of the loop,check if there is a cancellation request pending
                {
                    sendingWorker.ReportProgress(1, System.Text.Encoding.Default.GetString(mem.ToArray()));//Report our progress to the main thread
                    try { Thread.Sleep(10); }
                    catch
                    {
                        //added blank catch for thread continuation 
                    }
                }
                else
                {
                    bgWorker = true;
                    e.Cancel = true;//If a cancellation request is pending, assign this flag a value of true
                    break;// If a cancellation request is pending, break to exit the loop                    
                }
            }
            //e.Result = sb.ToString();// Send our result to the main thread!           
        }

        private void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            txtConsoleOutput.Text = (string)e.UserState;            
        }

        private void bgWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            txtConsoleOutput.Text = "";
            writerMem.Close();
            mem.Close();
            StreamWriter standardOutput = new StreamWriter(Console.OpenStandardOutput());
            standardOutput.AutoFlush = true;
            Console.SetOut(standardOutput);
            Console.WriteLine("Test");
        }


        public void PopulateGrid(Guid scriptID, ExecutionResultView output, int progress)
        {
            Application.DoEvents();

            if (progress == 0 & !output.IsSuccess) { progress = 100; } //Temp Fix for exception cases where progress values are not getting sent.
            string outputViewKey = output.ServerName + scriptID;

            if (outputScriptMap.ContainsKey(outputViewKey))
            {
                int rowIndex = outputScriptMap[outputViewKey];

                DataGridViewRow row = dataGridView1.Rows[rowIndex];

                row.Cells[ProgressColumn].Value = progress;
                dataGridView1.Rows[rowIndex].Selected = true;
                if (progress != 100)
                {
                    row.Cells[ResultColumn].Value = output.IsSuccess ? "In Progress" : "In Progress";
                    Console.WriteLine(output.IsSuccess ? output.SuccessMessage : output.ErrorMessage);
                }
                else
                {
                    row.Cells[ResultColumn].Value = output.IsSuccess ? "Success" : "Fail";
                }

                
                row.Cells[StatusLinkColumn].Value += output.IsSuccess ? output.SuccessMessage : output.ErrorMessage;
                row.Cells[StatusLinkColumn].Value += Environment.NewLine;
                if (progress == 100)
                {
                    Console.WriteLine(new string('*', 300));
                    Console.WriteLine(new string('*', 300));
                    
                    //If workflow then display parameters
                    if (!output.IsScript && output.IsSuccess)
                    {
                        if (output.Output != null)
                        {
                            string paramsvalue = string.Empty;
                            foreach (var outvalue in output.Output)
                            {
                                paramsvalue += outvalue.ParameterName + ":" + outvalue.ParameterValue;
                            }
                            row.Cells[DataLinkColumn].Value += output.IsSuccess ? paramsvalue : paramsvalue;
                        }
                    }
                    else
                    {
                        row.Cells[DataLinkColumn].Value += output.IsSuccess ? output.data : "";
                        row.Cells[DataLinkColumn].Value += Environment.NewLine;
                    }
                }
            }
            else
            {
                if (progress == 100)
                {
                    output.Progress = 100;
                }
                output.Progress = progress;
                Display(outputViewKey, output);
            }
        }

        private void Display(string displayID, ExecutionResultView output)
        {
            Console.WriteLine(Environment.NewLine);
            string[] row = null;
            //if (dataGridView1.RowCount == 0)
            initializeDataGrid();
            if ((output.IsSuccess) && (output.Progress != 100))
            {
                row = new string[] { output.ServerName, "In Progress", output.SuccessMessage, "" };
                Console.WriteLine(output.SuccessMessage);
            }
            else if ((output.IsSuccess) && (output.Progress == 100))
            {
                row = new string[] { output.ServerName, "Success", output.SuccessMessage, output.data };//changed
                Console.WriteLine(output.SuccessMessage);
            }
            else
            {
                row = new string[] { output.ServerName, "Fail", output.ErrorMessage, "" };
                Console.WriteLine(output.ErrorMessage);
            }

            

            int result = dataGridView1.Rows.Add(row);
            outputScriptMap.Add(displayID, dataGridView1.RowCount - 1);
            addGridButtons();
            DataGridViewRow newRow = dataGridView1.Rows[result];
            newRow.Cells[4].Value = output.Progress;

            AddOutput(output, newRow);

            if (output.Progress == 100)
            {
                DataGridViewRow rowAdded = dataGridView1.Rows[result];
                rowAdded.Cells[4].Value = 100;
                if (output.IsSuccess == true)
                {
                    if (!output.IsScript)
                    {
                        if (output.Output != null)
                        {
                            if (output.Output.Count != 0)
                            {
                                string paramsvalue = string.Empty;
                                foreach (var outvalue in output.Output)
                                {
                                    paramsvalue += outvalue.ParameterName + ":" + outvalue.ParameterValue;
                                }
                                row = new string[] { output.ServerName, "Success", output.SuccessMessage, paramsvalue };
                            }
                            //else
                            //{
                            //    rowAdded.Cells[StatusColumn].Value = output.SuccessMessage;
                            //}
                        }
                        else
                            row = new string[] { output.ServerName, "Fail", output.SuccessMessage, "" };
                    }
                    else
                    {
                        row = new string[] { output.ServerName, "Success", output.SuccessMessage, output.data };
                    }
                }
                else
                {
                    row = new string[] { output.ServerName, "Fail", output.ErrorMessage, "" };
                }

                //if (output.IsSuccess)
                //    row = new string[] { output.ServerName, "Success", output.SuccessMessage, "" };
                //else
                //    row = new string[] { output.ServerName, "Fail", output.ErrorMessage, "" };

            }
            formatDataGrid();
        }

        private void AddOutput(ExecutionResultView output, DataGridViewRow rowAdded)
        {
            if (output.IsSuccess == true)
            {
                if (!output.IsScript)
                {
                    if (output.Output != null)
                    {
                        if (output.Output.Count != 0)
                        {
                            string paramsvalue = string.Empty;
                            foreach (var outvalue in output.Output)
                            {
                                paramsvalue += outvalue.ParameterName + ":" + outvalue.ParameterValue;
                            }
                            rowAdded.Cells[DataColumn].Value = paramsvalue;
                        }
                    }
                }
            }
        }
        

        /// <summary>
        /// This method is used to process inout and contruct GridView for displaying the output.
        /// </summary>
        /// <param name="consolidatedOutput">List of ExecutionResultView containing consolidated input</param>
        public void Display(List<ExecutionResultView> consolidatedOutput)
        {
            bool IsExist = false;
            if (!IsExist)
            {
                foreach (var result in consolidatedOutput)
                {
                    PopulateGrid(result.Identifier, result, 100);
                }
                IsExist = true;
                
            }
            else
            {
            // dataGridView1 = new DataGridView();
            if (OutputView.consolidatedOutput == null)
                OutputView.consolidatedOutput = consolidatedOutput;
            else
            {
                foreach (ExecutionResultView result in consolidatedOutput)
                {
                    OutputView.consolidatedOutput.Add(result);
                }
            }
            int rowIndex = 0;
            string[] row = null;

            initializeDataGrid();
            addGridButtons();
            string value = "";
            foreach (ExecutionResultView output in OutputView.consolidatedOutput)
            {
                if (output.Output != null && output.Output.Count > 0)
                {
                    value = GetXMLFromParamObject(output.Output);
                }
                else
                    value = "";
                if (output.IsSuccess)
                {
                        if (!output.IsScript && output.Output != null)
                    {

                            string paramsvalue = string.Empty;
                        foreach (var outvalue in output.Output)
                            {
                                paramsvalue += outvalue.ParameterName + ":" + outvalue.ParameterValue;
                            }
                            row = new string[] { output.ServerName, "Success", output.SuccessMessage, paramsvalue };


                    }
                    else
                    {
                        row = new string[] { output.ServerName, "Success", output.SuccessMessage, output.SuccessMessage };
                    }
                }

                if (!output.IsSuccess)
                    {
                    row = new string[] { output.ServerName, "Fail", output.ErrorMessage, value };
                    }

                
                rowIndex = dataGridView1.Rows.Add(row);
                DataGridViewRow rowAdded = dataGridView1.Rows[rowIndex];
                rowAdded.Cells[ProgressColumn].Value = 100;
            }

            //Add Grid Buttons
            //addGridButtons();
            formatDataGrid();
                }
        }


        private void addGridButtons()
        {
            DataGridViewProgressColumn column = new DataGridViewProgressColumn();
            //// dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            column.HeaderText = "Progress";
            dataGridView1.Columns.Insert(ProgressColumn, column);

            DataGridViewLinkColumn lnkShowMore = new DataGridViewLinkColumn();
            lnkShowMore.LinkBehavior = LinkBehavior.NeverUnderline;
            lnkShowMore.LinkColor = System.Drawing.Color.Black;
            lnkShowMore.HeaderText = "Status Message";
            lnkShowMore.ToolTipText = "Click on Show More button to display complete status Message";
            lnkShowMore.SortMode = DataGridViewColumnSortMode.Automatic;
            dataGridView1.Columns.Insert(5, lnkShowMore);

            //Link button for displaying parameter data
            DataGridViewLinkColumn lnkShowMoreParamOutput = new DataGridViewLinkColumn();
            lnkShowMoreParamOutput.LinkBehavior = LinkBehavior.NeverUnderline;
            lnkShowMoreParamOutput.LinkColor = System.Drawing.Color.Black;
            lnkShowMoreParamOutput.HeaderText = "Data";
            //lnkShowMoreParamOutput.ToolTipText = "Click on Show More button to display complete data";
            lnkShowMoreParamOutput.SortMode = DataGridViewColumnSortMode.Automatic;
            dataGridView1.Columns.Insert(6, lnkShowMoreParamOutput);

            // Button to copy status message
            DataGridViewButtonColumn btnCopy = new DataGridViewButtonColumn();
            btnCopy.Name = "Copy Status Message";
            btnCopy.Text = "Copy";
            btnCopy.HeaderText = "Copy Status Message";
            btnCopy.UseColumnTextForButtonValue = true;
            btnCopy.ToolTipText = "Click to copy Status Message to clipboard";
            dataGridView1.Columns.Insert(7, btnCopy);

            DataGridViewDisableButtonColumn btnCopyData = new DataGridViewDisableButtonColumn();
            //Button to copy parameter data
            // DataGridViewButtonColumn btnCopyData = new DataGridViewButtonColumn();
            btnCopyData.Name = "Copy Data";
            btnCopyData.Text = "Copy";
            btnCopyData.HeaderText = "Copy Data";
            btnCopyData.UseColumnTextForButtonValue = true;
            //btnCopyData.ToolTipText = "Click to copy Data to clipboard";
            dataGridView1.Columns.Insert(8, btnCopyData);



            dataGridView1.CellFormatting += dataGridView1_CellFormatting;

            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            // Change column style
            DataGridViewCell cell = new DataGridViewTextBoxCell();
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.HeaderCell.Style.Font = new Font("Arial", 11F, FontStyle.Bold, GraphicsUnit.Pixel);
                if (col.Index == 7)
                {
                    col.DefaultCellStyle.BackColor = System.Drawing.Color.Gray;
                }
            }

            // Adjust grid height based on number of rows and display scrollbar if number of rows is greater than 5
            if (dataGridView1.Rows.Count > 0)
            {
                int rowHeight = dataGridView1.Rows[0].Height;
                if (dataGridView1.Rows.Count > 5)
                {
                    grdHeight = (rowHeight * 5) + 30;
                    dataGridView1.ScrollBars = ScrollBars.Vertical;
                }
                else
                {
                    grdHeight = grdHeight + (rowHeight * dataGridView1.Rows.Count);
                }
            }
            dataGridView1.Focus();
            dataGridView1.Dock = DockStyle.Fill;

        }

        private void initializeDataGrid()
        {

            dataGridView1.CellMouseClick -= dataGridView1_CellMouseClick;
            dataGridView1.CellMouseClick += dataGridView1_CellMouseClick;

            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.ColumnCount = 4;
            dataGridView1.Columns[ServerColumn].Name = "Server";
            dataGridView1.Columns[ResultColumn].Name = "Result";
            //This column will be hidden as the value from this column will be used for link column
            dataGridView1.Columns[StatusLinkColumn].Name = "Status Message";
            //Column to display out parameters (if any)
            dataGridView1.Columns[DataLinkColumn].Name = "Data";
           // dataGridView1.Columns[ProgressColumn].Name = "Progress";
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
        }

        private void formatDataGrid()
        {
           // this.Invalidate();
            dataGridView1.Columns[ServerColumn].Width = 150;
            dataGridView1.Columns[ResultColumn].Width = 100;
            dataGridView1.Columns[DataLinkColumn].Width = 0;
            dataGridView1.Columns[DataLinkColumn].Visible = false;
            dataGridView1.Columns[StatusLinkColumn].Width = 0;
            dataGridView1.Columns[StatusLinkColumn].Visible = false;
            dataGridView1.Columns[StatusColumn].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[DataColumn].Width = 400;
            dataGridView1.Columns[CopyStatusBtn].Width = 150;
            dataGridView1.Columns[CopyDataBtn].Width = 100;

            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            // Change column style
            DataGridViewCell cell = new DataGridViewTextBoxCell();
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.HeaderCell.Style.Font = new Font("Arial", 11F, FontStyle.Bold, GraphicsUnit.Pixel);
                if (col.Index == CopyStatusBtn)
                {
                    col.DefaultCellStyle.BackColor = System.Drawing.Color.Gray;
                }
            }

            // Adjust grid height based on number of rows and display scrollbar if number of rows is greater than 5
            int rowHeight = dataGridView1.Rows[0].Height;
            if (dataGridView1.Rows.Count > 5)
            {
                grdHeight = (rowHeight * 5) + 30;
                dataGridView1.ScrollBars = ScrollBars.Vertical;
            }
            else
            {
                grdHeight = grdHeight + (rowHeight * dataGridView1.Rows.Count);
            }

            dataGridView1.Focus();
            dataGridView1.Dock = DockStyle.Fill;
        }

        /// <summary>
        /// This event is used to format the cells of the grid
        /// </summary>
        /// <param name="sender">source of event</param>
        /// <param name="e">object DataGridViewCellFormattingEventArgs contaning grid details</param>
        void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Server Name column
            if (e.ColumnIndex == 0)
            {
                e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Bold);
            }
   
                   // Result column
            else if (e.ColumnIndex == ResultColumn)
            {
                e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Bold);
                if (this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().ToLower().Equals("success"))
                    e.CellStyle.ForeColor = Color.Green;
                else if (this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().ToLower().Equals("in progress"))
                    e.CellStyle.ForeColor = Color.Blue;
                else
                    e.CellStyle.ForeColor = Color.Red;
            }

            // Link column for displying output
            else if (e.ColumnIndex == StatusColumn)
            {
                DataGridViewCell cell = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                cell.ToolTipText = "Click to display complete output";
                this.dataGridView1.Rows[e.RowIndex].Cells[StatusColumn].Value = this.dataGridView1.Rows[e.RowIndex].Cells[StatusLinkColumn].Value;
            }

                 // Link column for displaying parameter output
            else if (e.ColumnIndex == DataColumn)
            {
                DataGridViewCell cell = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (this.dataGridView1.Rows[e.RowIndex].Cells[DataLinkColumn].Value == null)
                {
                    cell.ToolTipText = "";
                    //this.dataGridView1.Rows[e.RowIndex].Cells[7].ReadOnly = true;
                }
                else
                {
                    cell.ToolTipText = "Click to display complete parameter output";
                    this.dataGridView1.Rows[e.RowIndex].Cells[DataColumn].Value = this.dataGridView1.Rows[e.RowIndex].Cells[DataLinkColumn].Value;
                }
            }
            // Button to copy output to clipboard
            else if (e.ColumnIndex == CopyStatusBtn)
            {
                DataGridViewCell cell = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                cell.ToolTipText = "Click to copy status message to clipboard";
            }

            else if (e.ColumnIndex == CopyDataBtn)
            {
                if (this.dataGridView1.Rows[e.RowIndex].Cells[3].Value == null || string.IsNullOrEmpty(this.dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString()))
                {
                    ((DataGridViewDisableButtonCell)this.dataGridView1.Rows[e.RowIndex].Cells[8]).Enabled = false;

                }
                else
                {
                    ((DataGridViewDisableButtonCell)this.dataGridView1.Rows[e.RowIndex].Cells[8]).Enabled = true;
                    this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "Click to copy data to clipboard";
                }
            }

            dataGridView1.Rows[0].Frozen = true;
        }


        /// <summary>
        /// This event is fired when cell of grid is clicked.
        /// </summary>
        /// <param name="sender">ource of event</param>
        /// <param name="e">object DataGridViewCellMouseEventArgs containing cell details</param>
        void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
            dataGridView1.Rows[e.RowIndex].Selected = true;
            
            // Link column for displying status message
            if (e.ColumnIndex == 5)
            {
                ShowOutput(Convert.ToString(row.Cells[2].Value), "Status Message");
            }
            // Link column for displying parameter data
            else if (e.ColumnIndex == 6)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(row.Cells[3].Value)))
                {
                    ShowOutput(Convert.ToString(row.Cells[3].Value), "Data");
                }
            }
            // Button to copy output to clipboard
            else if (e.ColumnIndex == 7)
            {
                System.Windows.Forms.Clipboard.SetText(Convert.ToString(row.Cells[2].Value));
            }
            else if (e.ColumnIndex == 8)
            {
                if (row.Cells[3].Value != null && !string.IsNullOrEmpty(Convert.ToString(row.Cells[3].Value)))
                    System.Windows.Forms.Clipboard.SetText(Convert.ToString(row.Cells[3].Value));
            }
        }

        /// <summary>
        /// This method is used to create Popup window and display the output in the same.
        /// </summary>
        /// <param name="output">output to be displayed</param>
        private static void ShowOutput(string output, string formText)
        {
            using (Form form = new Form())
            {
                TextBox txtOutput = new TextBox();
                txtOutput.Multiline = true;
                txtOutput.ReadOnly = true;
                txtOutput.ScrollBars = ScrollBars.Vertical;
                txtOutput.Text = output;
                txtOutput.Select(0, 0);
                txtOutput.Dock = DockStyle.Fill;

                form.StartPosition = FormStartPosition.CenterParent;
                form.Height = grdHeight + 100;
                form.Text = formText;
                form.Width = grdWidth;
                form.Controls.Add(txtOutput);
                form.ShowDialog();
            }
        }

        private void toolStripStatusLabel2_Click(object sender, EventArgs e)
        {
            if (consolidatedOutput != null)
            {
                OutputView.consolidatedOutput.Clear();
                outputScriptMap.Clear();
            }

            if (CloseOutputView != null)
            {
                CloseOutputView();
                outputScriptMap.Clear();
                worker.CancelAsync();
            }
        }

        /// <summary>
        /// This method is used to convert parameter object to XML.
        /// </summary>
        /// <param name="outParam">Parameter object</param>
        /// <returns>XML representation of Parameter object</returns>
        public static string GetXMLFromParamObject(object outParam)
        {
            StringWriter stringWriter = null;
            XmlTextWriter textWriter = null;
            string returnValue = "";
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(outParam.GetType());
                stringWriter = new StringWriter();
                textWriter = new XmlTextWriter(stringWriter);
                xmlSerializer.Serialize(textWriter, outParam);
                returnValue = stringWriter.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                stringWriter.Close();
                textWriter.Close();
            }
            return returnValue;
        }

        private void tbOuputView_SelectedIndexChanged(object sender, EventArgs e)
        {
            Application.DoEvents();             
        }
    }

    /// <summary>
    /// Class to contain execution results. This will be used as input to Display method.
    /// </summary>
    public class ExecutionResultView
    {
        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsSuccess { get; set; }
        public string ServerName { get; set; }
        public List<OutParameter> Output { get; set; }
        public int Progress { get; set; }
        public bool IsScript { get; set; }
        public Guid Identifier { get; set; }
        public string data { get; set; }
    }

    public class ExecutionStatusView : ExecutionResultView
    {
        public Guid Identifier { get; set; }
        public int progress { get; set; }
    }

    public class OutParameter
    {
        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }
    }


}

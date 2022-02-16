using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Infosys.ATR.AutomationClient;
using System.Text.RegularExpressions;
using WEMClient = Infosys.WEM.Client;
using CommonContracts = Infosys.WEM.Service.Common.Contracts.Message;

namespace Infosys.ATR.ScheduleRequestUI
{
    public partial class ScheduleRequestUI : UserControl
    {
        bool recurrancePanel = false;
        bool runOnNode = false;
        bool runOnCluster = false;
        private const int margin = 10;
        int grpScheduleHeight = 0;
        int pnlScheduleHeight = 0;
        ucIAPNodes iapPaneSchedule = null;
        int executionTypeValue;
        public ScheduleRequestUI(int executionType)
        {
            InitializeComponent();
            executionTypeValue = executionType;
            rbNow.Checked = true;
            rbRunOnNode.Checked = true;
            pnlScheduleProperties.Location = new Point(pnlScheduleProperties.Location.X, pnlScheduledFor.Bottom);
            grpSchduleIAPNode.Location = new Point(pnlScheduleProperties.Location.X, pnlScheduleProperties.Bottom - 40);

        }

        private void rbNow_CheckedChanged(object sender, EventArgs e)
        {
            if (rbNow.Checked)
            {
                if (recurrancePanel == true && runOnNode == true)
                {
                    recurrancePanel = false;
                    grpSetExecute.Visible = false;
                    pnlScheduleProperties.Location = new Point(pnlScheduleProperties.Location.X, pnlScheduledFor.Bottom);
                    DisplayIAPNode();
                }
                else if (recurrancePanel)
                {
                    //  int height = 0;
                    recurrancePanel = false;
                    //  int height = grpSetExecute.Height;
                    grpSetExecute.Visible = false;
                    // if (runOnCluster)
                    // height = 50;
                    pnlSchedule.Height = pnlSchedule.Height - grpSetExecute.Height;// +height; ;
                    grpSchedule.Height = pnlSchedule.Height + margin;

                }
                else if (runOnNode)
                {
                    DisplayIAPNode();
                }

                //if (rbNow.Checked)
                //{
                //    //int height = grpSetExecute.Height;
                //    grpSetExecute.Visible = false;
                //    pnlScheduleProperties.Location = new Point(pnlScheduleProperties.Location.X, pnlScheduledFor.Bottom);
                //    if (runOnNode)
                //    {
                //        grpSchduleIAPNode.Location = new Point(grpSchduleIAPNode.Location.X, pnlScheduleProperties.Bottom - 40);
                //    }
                //    else
                //    {
                //        pnlSchedule.Height = pnlScheduleHeight - 150;
                //        grpSchedule.Height = grpScheduleHeight;
                //    }

                //    if (recurrancePanel)
                //    {
                //        pnlSchedule.Height = pnlScheduleHeight;
                //        grpSchedule.Height = grpScheduleHeight;
                //        recurrancePanel = false;
                //    }
                //}
                //if (recurrancePanel == true && runOnNode == true)
                //{
                //    recurrancePanel = false;
                //    grpSetExecute.Visible = false;
                //    pnlScheduleProperties.Location = new Point(pnlScheduleProperties.Location.X, pnlScheduledFor.Bottom);
                //    DisplayIAPNode();
                //}
                //else if (recurrancePanel)
                //{
                //    recurrancePanel = false;
                //    int height = grpSetExecute.Height;
                //    grpSetExecute.Visible = false;
                //    pnlSchedule.Height = pnlSchedule.Height - height;
                //    grpSchedule.Height = pnlSchedule.Height + (margin*2);
                //}
                //else if (runOnNode)
                //{
                //    runOnNode = true;
                //    DisplayIAPNode();

                //}

            }
        }

        private void DisplayIAPNode()
        {
            grpSchduleIAPNode.Width = iapPaneSchedule.Width;
            grpSchduleIAPNode.Height = iapPaneSchedule.Height + margin;
            if (runOnNode)
            {
                if (rbNow.Checked)
                    pnlSchedule.Height = pnlScheduledFor.Height + pnlScheduleProperties.Height + 150;
                else
                    pnlSchedule.Height = pnlScheduledFor.Height + grpSetExecute.Height + pnlScheduleProperties.Height + 150;
            }
            else if (runOnCluster)
            {
                if (rbNow.Checked)
                    pnlSchedule.Height = pnlScheduledFor.Height + pnlScheduleProperties.Height;
                else
                    pnlSchedule.Height = pnlScheduledFor.Height + grpSetExecute.Height + pnlScheduleProperties.Height;
            }

            grpSchedule.Height = pnlSchedule.Height + (margin * 2);
            grpSchduleIAPNode.Location = new Point(pnlScheduleProperties.Location.X, pnlScheduleProperties.Bottom - 40);
            grpScheduleHeight = grpSchedule.Height;
            pnlScheduleHeight = pnlSchedule.Height;
        }

        private void rbSetExecute_CheckedChanged(object sender, EventArgs e)
        {
            if (rbSetExecute.Checked == true)
            {
                grpSetExecute.Visible = true;
                recurrancePanel = true;
                int height = grpSetExecute.Height;
                pnlScheduleProperties.Location = new Point(pnlScheduleProperties.Location.X, grpSetExecute.Bottom);
                if (runOnNode)
                    grpSchduleIAPNode.Location = new Point(grpSchduleIAPNode.Location.X, pnlScheduleProperties.Bottom - 40);
                pnlSchedule.Height = pnlSchedule.Height + height;
                grpSchedule.Height = grpSchedule.Height + height;
                this.Height = this.Height + height;
                dtPickerStartDate.Value = DateTime.Now.Date;
                txtHours.Text = DateTime.Now.Hour.ToString("00");
                txtMinutes.Text = DateTime.Now.AddMinutes(1).Minute.ToString("00");
                dtPickerEndBy.Value = DateTime.Today.AddMonths(1);
                rbNoEndDate.Checked = true;
                //System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
                //ToolTip1.SetToolTip(this.dtPickerEndBy, dateValidationMsg);
            }
            else
            {
                grpSetExecute.Visible = false;
                pnlScheduleProperties.Location = new Point(pnlScheduleProperties.Location.X, pnlScheduledFor.Height + margin);
                grpSchedule.Height = pnlScheduledFor.Height + pnlScheduleProperties.Height + 20;
            }

            //if (rbSetExecute.Checked == true)
            //{
            //    grpSetExecute.Visible = true;
            //    recurrancePanel = true;
            //    dtPickerEndBy.Value = DateTime.Today.AddMonths(1);
            //    rbNoEndDate.Checked = true;

            //    pnlScheduleProperties.Location = new Point(pnlScheduleProperties.Location.X, grpSetExecute.Bottom);
            //    if (runOnNode)
            //    {
            //        grpSchduleIAPNode.Location = new Point(grpSchduleIAPNode.Location.X, pnlScheduleProperties.Bottom - 40);
            //        pnlSchedule.Height = pnlScheduledFor.Height + pnlScheduleProperties.Height + 150;
            //        grpSchedule.Height = pnlSchedule.Height ;
            //    }
            //    else if (runOnCluster)
            //    {
            //        pnlSchedule.Height = pnlSchedule.Height + grpSetExecute.Height;// pnlScheduledFor.Height + pnlScheduleProperties.Height + grpSetExecute.Height + (margin * 2);
            //        grpSchedule.Height = pnlSchedule.Height + (margin*2);
            //    }

            //}
            //else
            //{
            //    grpSetExecute.Visible = false;
            //pnlScheduleProperties.Location = new Point(pnlScheduleProperties.Location.X, pnlScheduledFor.Height + margin);
            //grpSchedule.Height = pnlScheduledFor.Height + pnlScheduleProperties.Height + margin;
            //this.Height = this.Height - grpSetExecute.Height;
            //}
        }

        //private void dtPickerStartDate_ValueChanged(object sender, EventArgs e)
        //{
        //    DateTime startdate = Convert.ToDateTime(dtPickerStartDate.Value);
        //    if (startdate != null)
        //    {
        //        int compareResult = startdate.CompareTo(DateTime.Now);
        //        if (compareResult < 0)
        //        {
        //            MessageBox.Show("Start Date must be greater than or equal to current date", "Validate Input");
        //            dtPickerStartDate.Value = DateTime.Now;
        //        }
        //        else if (startdate > dtPickerEndBy.Value)
        //        {
        //            MessageBox.Show("Start Date must be less than or equal to end date", "Validate Input");
        //            dtPickerStartDate.Value = DateTime.Now;
        //        }
        //    }
        //}

        //private void dtPickerEndBy_ValueChanged(object sender, EventArgs e)
        //{

        //}

        private void rbSelCluster_CheckedChanged(object sender, EventArgs e)
        {
            if (rbSelCluster.Checked == true)
            {
                runOnCluster = true;
                lblSelectCluster.Visible = true;
                cmbSemantic.Visible = true;
                FillClusters();
                int height = 150;// iapPaneSchedule.Height + margin;
                grpSchduleIAPNode.Visible = false;
                grpSchduleIAPNode.Height = 0;
                pnlSchedule.AutoScroll = false;
                pnlSchedule.Height = pnlSchedule.Height - height;
                grpSchedule.Height = grpSchedule.Height - height + margin;
            }
            else
            {
                cmbSemantic.Hide();
                lblSelectCluster.Visible = false;
                runOnCluster = false;
                //pnlSchedule.Height = pnlSchedule.Height - 50;
                //grpSchedule.Height = grpSchedule.Height - 50 + margin;
            }

            //int height = 150;
            //if (rbSelCluster.Checked == true)
            //{
            //    runOnCluster = true;
            //    runOnNode = false;
            //    lblSelectCluster.Visible = true;
            //    cmbSemantic.Visible = true;
            //    FillClusters();

            //    if (recurrancePanel)
            //        height = height - grpSetExecute.Height;
            //    grpSchduleIAPNode.Visible = false;
            //    grpSchduleIAPNode.Height = 0;
            //    pnlSchedule.AutoScroll = false;
            //    pnlSchedule.Height = pnlSchedule.Height - height;
            //    grpSchedule.Height = grpSchedule.Height - height + margin;               
            //}
            //else
            //{
            //    cmbSemantic.Hide();
            //    lblSelectCluster.Visible = false;
            //    runOnCluster = false;
            //}
        }

        private void rbRunOnNode_CheckedChanged(object sender, EventArgs e)
        {
            if (rbRunOnNode.Checked == true)
            {
                DisplayRunOnNodeUI();
            }
            else
            {
                if (grpSchduleIAPNode.Controls.Contains(iapPaneSchedule))
                    grpSchduleIAPNode.Controls.Remove(iapPaneSchedule);
                grpSchduleIAPNode.Height = 0;
                grpSchduleIAPNode.Visible = false;
                runOnNode = false;
            }
        }

        private void DisplayRunOnNodeUI()
        {
            if (iapPaneSchedule == null)
            {
                if (executionTypeValue == 1)
                    iapPaneSchedule = new ucIAPNodes(ExecutionType.Workflow, "Schedule");
                else if (executionTypeValue == 2)
                    iapPaneSchedule = new ucIAPNodes(ExecutionType.Script, "Schedule");
                else
                    iapPaneSchedule = new ucIAPNodes(ExecutionType.None, "Schedule");
            }
            runOnNode = true;
            pnlSchedule.AutoScroll = true;
            pnlSchedule.HorizontalScroll.Visible = false;
            cmbSemantic.Visible = false;
            lblSelectCluster.Visible = false;
            grpSchduleIAPNode.Visible = true;
            iapPaneSchedule.mode = "Schedule";

            iapPaneSchedule.Dock = DockStyle.Top;
            iapPaneSchedule.HideExecute = true;

            if (rbRunOnNode.Checked && !grpSchduleIAPNode.Controls.Contains(iapPaneSchedule))
                grpSchduleIAPNode.Controls.Add(iapPaneSchedule);
            DisplayIAPNode();
        }

        private void txtPriority_Enter(object sender, EventArgs e)
        {
            TextBox txtBox = (TextBox)sender;
            int VisibleTime = 1000;  //in milliseconds

            ToolTip tt = new ToolTip();
            tt.Show("lower the value higher the priority", txtBox, 0, 0, VisibleTime);
        }

        private void txtRatio_Leave(object sender, EventArgs e)
        {
            rbEndAfter.Checked = true;
            if (!IsValidInteger(txtRatio.Text.Trim()))
            {
                MessageBox.Show("Please enter positive or negative integer value only", "Validate Input");
            }
        }

        /// <summary>
        /// This method is used to validate if entered value is positive or negative number.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private bool IsValidInteger(string input)
        {
            string expression = @"^-?[0-9]\d*(\d+)?$";
            if (Regex.IsMatch(input, expression))
                return true;
            else
                return false;
        }

        public string Priority
        {
            get
            {
                return txtPriority.Text;
            }
        }

        public DateTime StartDate
        {
            get
            {
                return dtPickerStartDate.Value.AddHours(int.Parse(txtHours.Text)).AddMinutes(int.Parse(txtMinutes.Text));
            }
        }

        public DateTime EndDate
        {
            get
            {
                return dtPickerEndBy.Value.Date;
            }
        }

        public bool ScheduledForNow
        {
            get
            {
                return rbNow.Checked;
            }
        }
        public bool ScheduledForLater
        {
            get
            {
                return rbSetExecute.Checked;
            }
        }

        public bool NoEndDate
        {
            get
            {
                return rbNoEndDate.Checked;
            }
        }

        public bool EndBy
        {
            get
            {
                return rbEndBy.Checked;
            }
        }

        public bool RunOnCluster
        {
            get
            {
                return rbSelCluster.Checked;
            }
        }

        public string ClusterName
        {
            get
            {
                return cmbSemantic.Text;
            }
        }

        public string ClusterValue
        {
            get
            {
                return cmbSemantic.SelectedValue.ToString();
            }
        }

        public bool RunOnNode
        {
            get
            {
                return rbRunOnNode.Checked;
            }
        }

        public string Iterations
        {
            get
            {
                return txtRatio.Text;
            }
        }

        public List<string> SelectedNodes
        {
            get
            {
                var list = new List<string>();
                foreach (var item in iapPaneSchedule.SelectedNodes)
                {
                    list.Add(item);
                }
                return list;

            }
        }
        public int CategoryId { get; set; }
        public int NodeType { set { iapPaneSchedule.NodeType = value; } }

        private static Infosys.WEM.Service.Common.Contracts.Message.GetAllClustersByCategoryResMsg GetAllClustersByCategory(string categoryId)
        {
            Infosys.WEM.Service.Common.Contracts.Message.GetAllClustersByCategoryResMsg response = null;
            WEMClient.CommonRepository commonRepoClient = new WEMClient.CommonRepository();
            response = commonRepoClient.ServiceChannel.GetAllClustersByCategory(categoryId.ToString());
            return response;
        }

        private void FillClusters()
        {
            var response = GetAllClustersByCategory(CategoryId.ToString());
            if (response.Nodes != null && response.Nodes.Count > 0)
            {
                this.cmbSemantic.DataSource = response.Nodes.ToList();
                this.cmbSemantic.DisplayMember = "Name";
                this.cmbSemantic.ValueMember = "Id";
                //cmbSemantic.Items.Add("Select Cluster");
                cmbSemantic.SelectedIndex = 0;
            }
        }

        private void txtPriority_Leave(object sender, EventArgs e)
        {
            // rbEndAfter.Checked = true;
            if (!IsValidInteger(txtPriority.Text.Trim()))
            {
                MessageBox.Show("Please enter positive or negative integer value only", "Validate Input");
            }
        }

        private void dtPickerEndBy_ValueChanged(object sender, EventArgs e)
        {
            rbEndBy.Checked = true;
        }

        private void dtPickerStartDate_ValueChanged(object sender, EventArgs e)
        {
            
        }

        private void txtHours_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtHours.Text))
            {
                int hours;
                bool result = int.TryParse(txtHours.Text, out hours);

                if (result == false)
                {
                    MessageBox.Show("Invalid Hours");
                    txtHours.Text = "00";
                }
                else if (result == true)
                {
                    if (hours < 0 || hours > 23)
                    {
                        MessageBox.Show("Invalid Hours");
                        txtHours.Text = "00";
                    }
                }
            }
        }

        private void txtMinutes_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtMinutes.Text))
            {
                int minutes;
                bool result = int.TryParse(txtMinutes.Text, out minutes);

                if (result == false)
                {
                    MessageBox.Show("Invalid Minutes");
                    txtMinutes.Text = "00";
                }
                else if (result == true)
                {
                    if (minutes < 0 || minutes > 59)
                    {
                        MessageBox.Show("Invalid Minutes");
                        txtMinutes.Text = "00";
                    }
                }
            }
        }
        //private void dtPickerEndBy_ValueChanged(object sender, EventArgs e)
        //{
        //    if (this.dtPickerEndBy.Value < this.dtPickerStartDate.Value)
        //    {
        //        MessageBox.Show("End date must be greater than start date.","Validate End Date");
        //        this.dtPickerEndBy.Value = this.dtPickerStartDate.Value;
        //        rbEndBy.Checked = false;
        //        return;
        //    }
        //    rbEndBy.Checked = true;

        //}
       

    }
}

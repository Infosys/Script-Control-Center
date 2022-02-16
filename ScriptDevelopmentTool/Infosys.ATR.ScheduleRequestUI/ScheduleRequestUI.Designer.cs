namespace Infosys.ATR.ScheduleRequestUI
{
    partial class ScheduleRequestUI
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.grpSchedule = new System.Windows.Forms.GroupBox();
            this.pnlSchedule = new System.Windows.Forms.Panel();
            this.grpSchduleIAPNode = new System.Windows.Forms.GroupBox();
            this.pnlScheduleProperties = new System.Windows.Forms.Panel();
            this.lblSelectCluster = new System.Windows.Forms.Label();
            this.rbRunOnNode = new System.Windows.Forms.RadioButton();
            this.lblPriority = new System.Windows.Forms.Label();
            this.rbSelCluster = new System.Windows.Forms.RadioButton();
            this.cmbSemantic = new System.Windows.Forms.ComboBox();
            this.txtPriority = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grpSetExecute = new System.Windows.Forms.GroupBox();
            this.txtMinutes = new System.Windows.Forms.TextBox();
            this.txtHours = new System.Windows.Forms.TextBox();
            this.rbEndBy = new System.Windows.Forms.RadioButton();
            this.rbEndAfter = new System.Windows.Forms.RadioButton();
            this.rbNoEndDate = new System.Windows.Forms.RadioButton();
            this.lblOccurances = new System.Windows.Forms.Label();
            this.txtRatio = new System.Windows.Forms.TextBox();
            this.dtPickerEndBy = new System.Windows.Forms.DateTimePicker();
            this.lblStartDate = new System.Windows.Forms.Label();
            this.dtPickerStartDate = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.pnlScheduledFor = new System.Windows.Forms.Panel();
            this.rbNow = new System.Windows.Forms.RadioButton();
            this.rbSetExecute = new System.Windows.Forms.RadioButton();
            this.lblExecutedOn = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.grpSchedule.SuspendLayout();
            this.pnlSchedule.SuspendLayout();
            this.pnlScheduleProperties.SuspendLayout();
            this.grpSetExecute.SuspendLayout();
            this.pnlScheduledFor.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpSchedule
            // 
            this.grpSchedule.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpSchedule.Controls.Add(this.pnlSchedule);
            this.grpSchedule.Location = new System.Drawing.Point(2, 4);
            this.grpSchedule.Name = "grpSchedule";
            this.grpSchedule.Size = new System.Drawing.Size(604, 296);
            this.grpSchedule.TabIndex = 4;
            this.grpSchedule.TabStop = false;
            this.grpSchedule.Text = "Schedule Properties";
            // 
            // pnlSchedule
            // 
            this.pnlSchedule.Controls.Add(this.grpSchduleIAPNode);
            this.pnlSchedule.Controls.Add(this.pnlScheduleProperties);
            this.pnlSchedule.Controls.Add(this.grpSetExecute);
            this.pnlSchedule.Controls.Add(this.pnlScheduledFor);
            this.pnlSchedule.Controls.Add(this.lblExecutedOn);
            this.pnlSchedule.Location = new System.Drawing.Point(6, 19);
            this.pnlSchedule.Name = "pnlSchedule";
            this.pnlSchedule.Size = new System.Drawing.Size(596, 271);
            this.pnlSchedule.TabIndex = 0;
            // 
            // grpSchduleIAPNode
            // 
            this.grpSchduleIAPNode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpSchduleIAPNode.Location = new System.Drawing.Point(6, 224);
            this.grpSchduleIAPNode.Name = "grpSchduleIAPNode";
            this.grpSchduleIAPNode.Size = new System.Drawing.Size(573, 32);
            this.grpSchduleIAPNode.TabIndex = 18;
            this.grpSchduleIAPNode.TabStop = false;
            this.grpSchduleIAPNode.Text = "IAP Node(s)";
            // 
            // pnlScheduleProperties
            // 
            this.pnlScheduleProperties.Controls.Add(this.lblSelectCluster);
            this.pnlScheduleProperties.Controls.Add(this.rbRunOnNode);
            this.pnlScheduleProperties.Controls.Add(this.lblPriority);
            this.pnlScheduleProperties.Controls.Add(this.rbSelCluster);
            this.pnlScheduleProperties.Controls.Add(this.cmbSemantic);
            this.pnlScheduleProperties.Controls.Add(this.txtPriority);
            this.pnlScheduleProperties.Controls.Add(this.label1);
            this.pnlScheduleProperties.Location = new System.Drawing.Point(6, 119);
            this.pnlScheduleProperties.Name = "pnlScheduleProperties";
            this.pnlScheduleProperties.Size = new System.Drawing.Size(586, 103);
            this.pnlScheduleProperties.TabIndex = 17;
            // 
            // lblSelectCluster
            // 
            this.lblSelectCluster.AutoSize = true;
            this.lblSelectCluster.Location = new System.Drawing.Point(3, 68);
            this.lblSelectCluster.Name = "lblSelectCluster";
            this.lblSelectCluster.Size = new System.Drawing.Size(120, 13);
            this.lblSelectCluster.TabIndex = 23;
            this.lblSelectCluster.Text = "Select the Cluster to run";
            // 
            // rbRunOnNode
            // 
            this.rbRunOnNode.AutoSize = true;
            this.rbRunOnNode.Location = new System.Drawing.Point(268, 39);
            this.rbRunOnNode.Name = "rbRunOnNode";
            this.rbRunOnNode.Size = new System.Drawing.Size(89, 17);
            this.rbRunOnNode.TabIndex = 22;
            this.rbRunOnNode.TabStop = true;
            this.rbRunOnNode.Text = "Run on Node";
            this.rbRunOnNode.UseVisualStyleBackColor = true;
            this.rbRunOnNode.CheckedChanged += new System.EventHandler(this.rbRunOnNode_CheckedChanged);
            // 
            // lblPriority
            // 
            this.lblPriority.AutoSize = true;
            this.lblPriority.Location = new System.Drawing.Point(5, 6);
            this.lblPriority.Name = "lblPriority";
            this.lblPriority.Size = new System.Drawing.Size(38, 13);
            this.lblPriority.TabIndex = 19;
            this.lblPriority.Text = "Priority";
            // 
            // rbSelCluster
            // 
            this.rbSelCluster.AutoSize = true;
            this.rbSelCluster.Location = new System.Drawing.Point(154, 39);
            this.rbSelCluster.Name = "rbSelCluster";
            this.rbSelCluster.Size = new System.Drawing.Size(95, 17);
            this.rbSelCluster.TabIndex = 21;
            this.rbSelCluster.TabStop = true;
            this.rbSelCluster.Text = "Run on Cluster";
            this.rbSelCluster.UseVisualStyleBackColor = true;
            this.rbSelCluster.CheckedChanged += new System.EventHandler(this.rbSelCluster_CheckedChanged);
            // 
            // cmbSemantic
            // 
            this.cmbSemantic.AutoCompleteCustomSource.AddRange(new string[] {
            "localhost",
            "localhost",
            "localhost"});
            this.cmbSemantic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSemantic.FormattingEnabled = true;
            this.cmbSemantic.Location = new System.Drawing.Point(154, 67);
            this.cmbSemantic.Name = "cmbSemantic";
            this.cmbSemantic.Size = new System.Drawing.Size(143, 21);
            this.cmbSemantic.TabIndex = 18;
            this.cmbSemantic.Visible = false;
            // 
            // txtPriority
            // 
            this.txtPriority.Location = new System.Drawing.Point(154, 6);
            this.txtPriority.Name = "txtPriority";
            this.txtPriority.Size = new System.Drawing.Size(48, 20);
            this.txtPriority.TabIndex = 17;
            this.txtPriority.Text = "1000";
            this.txtPriority.Enter += new System.EventHandler(this.txtPriority_Enter);
            this.txtPriority.Leave += new System.EventHandler(this.txtPriority_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(141, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "Select Cluster to Run Option";
            // 
            // grpSetExecute
            // 
            this.grpSetExecute.Controls.Add(this.txtMinutes);
            this.grpSetExecute.Controls.Add(this.txtHours);
            this.grpSetExecute.Controls.Add(this.rbEndBy);
            this.grpSetExecute.Controls.Add(this.rbEndAfter);
            this.grpSetExecute.Controls.Add(this.rbNoEndDate);
            this.grpSetExecute.Controls.Add(this.lblOccurances);
            this.grpSetExecute.Controls.Add(this.txtRatio);
            this.grpSetExecute.Controls.Add(this.dtPickerEndBy);
            this.grpSetExecute.Controls.Add(this.lblStartDate);
            this.grpSetExecute.Controls.Add(this.dtPickerStartDate);
            this.grpSetExecute.Controls.Add(this.label2);
            this.grpSetExecute.Location = new System.Drawing.Point(6, 28);
            this.grpSetExecute.Name = "grpSetExecute";
            this.grpSetExecute.Size = new System.Drawing.Size(587, 91);
            this.grpSetExecute.TabIndex = 11;
            this.grpSetExecute.TabStop = false;
            this.grpSetExecute.Text = "Range of Recurrence";
            this.grpSetExecute.Visible = false;
            // 
            // txtMinutes
            // 
            this.txtMinutes.Location = new System.Drawing.Point(189, 19);
            this.txtMinutes.MaxLength = 2;
            this.txtMinutes.Name = "txtMinutes";
            this.txtMinutes.Size = new System.Drawing.Size(21, 20);
            this.txtMinutes.TabIndex = 13;
            this.txtMinutes.Text = "00";
            this.txtMinutes.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.txtMinutes, "Enter Minutes");
            this.txtMinutes.TextChanged += new System.EventHandler(this.txtMinutes_TextChanged);
            // 
            // txtHours
            // 
            this.txtHours.Location = new System.Drawing.Point(161, 19);
            this.txtHours.MaxLength = 2;
            this.txtHours.Name = "txtHours";
            this.txtHours.Size = new System.Drawing.Size(21, 20);
            this.txtHours.TabIndex = 12;
            this.txtHours.Text = "00";
            this.txtHours.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.txtHours, "Enter Hours in 24-Hour Format");
            this.txtHours.TextChanged += new System.EventHandler(this.txtHours_TextChanged);
            // 
            // rbEndBy
            // 
            this.rbEndBy.AutoSize = true;
            this.rbEndBy.Checked = true;
            this.rbEndBy.Location = new System.Drawing.Point(267, 61);
            this.rbEndBy.Name = "rbEndBy";
            this.rbEndBy.Size = new System.Drawing.Size(61, 17);
            this.rbEndBy.TabIndex = 25;
            this.rbEndBy.TabStop = true;
            this.rbEndBy.Text = "End by:";
            this.rbEndBy.UseVisualStyleBackColor = true;
            // 
            // rbEndAfter
            // 
            this.rbEndAfter.AutoSize = true;
            this.rbEndAfter.Location = new System.Drawing.Point(267, 40);
            this.rbEndAfter.Name = "rbEndAfter";
            this.rbEndAfter.Size = new System.Drawing.Size(71, 17);
            this.rbEndAfter.TabIndex = 24;
            this.rbEndAfter.TabStop = true;
            this.rbEndAfter.Text = "End after:";
            this.rbEndAfter.UseVisualStyleBackColor = true;
            // 
            // rbNoEndDate
            // 
            this.rbNoEndDate.AutoSize = true;
            this.rbNoEndDate.Location = new System.Drawing.Point(267, 17);
            this.rbNoEndDate.Name = "rbNoEndDate";
            this.rbNoEndDate.Size = new System.Drawing.Size(84, 17);
            this.rbNoEndDate.TabIndex = 23;
            this.rbNoEndDate.TabStop = true;
            this.rbNoEndDate.Text = "No end date";
            this.rbNoEndDate.UseVisualStyleBackColor = true;
            // 
            // lblOccurances
            // 
            this.lblOccurances.AutoSize = true;
            this.lblOccurances.Location = new System.Drawing.Point(382, 42);
            this.lblOccurances.Name = "lblOccurances";
            this.lblOccurances.Size = new System.Drawing.Size(66, 13);
            this.lblOccurances.TabIndex = 16;
            this.lblOccurances.Text = "occurrences";
            // 
            // txtRatio
            // 
            this.txtRatio.Location = new System.Drawing.Point(341, 39);
            this.txtRatio.Name = "txtRatio";
            this.txtRatio.Size = new System.Drawing.Size(35, 20);
            this.txtRatio.TabIndex = 15;
            this.txtRatio.Text = "1";
            this.txtRatio.Leave += new System.EventHandler(this.txtRatio_Leave);
            // 
            // dtPickerEndBy
            // 
            this.dtPickerEndBy.CustomFormat = "MM/dd/yyyy";
            this.dtPickerEndBy.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtPickerEndBy.Location = new System.Drawing.Point(341, 65);
            this.dtPickerEndBy.Name = "dtPickerEndBy";
            this.dtPickerEndBy.Size = new System.Drawing.Size(95, 20);
            this.dtPickerEndBy.TabIndex = 14;
            this.dtPickerEndBy.ValueChanged += new System.EventHandler(this.dtPickerEndBy_ValueChanged);
            // 
            // lblStartDate
            // 
            this.lblStartDate.AutoSize = true;
            this.lblStartDate.Location = new System.Drawing.Point(7, 19);
            this.lblStartDate.Name = "lblStartDate";
            this.lblStartDate.Size = new System.Drawing.Size(55, 13);
            this.lblStartDate.TabIndex = 12;
            this.lblStartDate.Text = "Start Date";
            // 
            // dtPickerStartDate
            // 
            this.dtPickerStartDate.CustomFormat = "MM/dd/yyyy";
            this.dtPickerStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtPickerStartDate.Location = new System.Drawing.Point(63, 19);
            this.dtPickerStartDate.Name = "dtPickerStartDate";
            this.dtPickerStartDate.Size = new System.Drawing.Size(95, 20);
            this.dtPickerStartDate.TabIndex = 11;
            this.toolTip1.SetToolTip(this.dtPickerStartDate, "Select Start Date");
            this.dtPickerStartDate.ValueChanged += new System.EventHandler(this.dtPickerStartDate_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(181, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(10, 13);
            this.label2.TabIndex = 29;
            this.label2.Text = ":";
            // 
            // pnlScheduledFor
            // 
            this.pnlScheduledFor.Controls.Add(this.rbNow);
            this.pnlScheduledFor.Controls.Add(this.rbSetExecute);
            this.pnlScheduledFor.Location = new System.Drawing.Point(160, 3);
            this.pnlScheduledFor.Name = "pnlScheduledFor";
            this.pnlScheduledFor.Size = new System.Drawing.Size(377, 26);
            this.pnlScheduledFor.TabIndex = 15;
            // 
            // rbNow
            // 
            this.rbNow.AutoSize = true;
            this.rbNow.Location = new System.Drawing.Point(0, 3);
            this.rbNow.Name = "rbNow";
            this.rbNow.Size = new System.Drawing.Size(47, 17);
            this.rbNow.TabIndex = 9;
            this.rbNow.TabStop = true;
            this.rbNow.Text = "Now";
            this.rbNow.UseVisualStyleBackColor = true;
            this.rbNow.CheckedChanged += new System.EventHandler(this.rbNow_CheckedChanged);
            // 
            // rbSetExecute
            // 
            this.rbSetExecute.AutoSize = true;
            this.rbSetExecute.Location = new System.Drawing.Point(114, 3);
            this.rbSetExecute.Name = "rbSetExecute";
            this.rbSetExecute.Size = new System.Drawing.Size(94, 17);
            this.rbSetExecute.TabIndex = 10;
            this.rbSetExecute.TabStop = true;
            this.rbSetExecute.Text = "Set Date/time ";
            this.rbSetExecute.UseVisualStyleBackColor = true;
            this.rbSetExecute.CheckedChanged += new System.EventHandler(this.rbSetExecute_CheckedChanged);
            // 
            // lblExecutedOn
            // 
            this.lblExecutedOn.AutoSize = true;
            this.lblExecutedOn.Location = new System.Drawing.Point(11, 6);
            this.lblExecutedOn.Name = "lblExecutedOn";
            this.lblExecutedOn.Size = new System.Drawing.Size(76, 13);
            this.lblExecutedOn.TabIndex = 3;
            this.lblExecutedOn.Text = "Scheduled for ";
            // 
            // ScheduleRequestUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpSchedule);
            this.Name = "ScheduleRequestUI";
            this.Size = new System.Drawing.Size(636, 303);
            this.grpSchedule.ResumeLayout(false);
            this.pnlSchedule.ResumeLayout(false);
            this.pnlSchedule.PerformLayout();
            this.pnlScheduleProperties.ResumeLayout(false);
            this.pnlScheduleProperties.PerformLayout();
            this.grpSetExecute.ResumeLayout(false);
            this.grpSetExecute.PerformLayout();
            this.pnlScheduledFor.ResumeLayout(false);
            this.pnlScheduledFor.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpSchedule;
        private System.Windows.Forms.Panel pnlSchedule;
        private System.Windows.Forms.GroupBox grpSchduleIAPNode;
        private System.Windows.Forms.Panel pnlScheduleProperties;
        private System.Windows.Forms.Label lblSelectCluster;
        private System.Windows.Forms.RadioButton rbRunOnNode;
        private System.Windows.Forms.Label lblPriority;
        private System.Windows.Forms.RadioButton rbSelCluster;
        private System.Windows.Forms.ComboBox cmbSemantic;
        private System.Windows.Forms.TextBox txtPriority;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox grpSetExecute;
        private System.Windows.Forms.RadioButton rbEndBy;
        private System.Windows.Forms.RadioButton rbEndAfter;
        private System.Windows.Forms.RadioButton rbNoEndDate;
        private System.Windows.Forms.Label lblOccurances;
        private System.Windows.Forms.TextBox txtRatio;
        private System.Windows.Forms.DateTimePicker dtPickerEndBy;
        private System.Windows.Forms.Label lblStartDate;
        private System.Windows.Forms.DateTimePicker dtPickerStartDate;
        private System.Windows.Forms.Panel pnlScheduledFor;
        private System.Windows.Forms.RadioButton rbNow;
        private System.Windows.Forms.RadioButton rbSetExecute;
        private System.Windows.Forms.Label lblExecutedOn;
        private System.Windows.Forms.TextBox txtMinutes;
        private System.Windows.Forms.TextBox txtHours;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

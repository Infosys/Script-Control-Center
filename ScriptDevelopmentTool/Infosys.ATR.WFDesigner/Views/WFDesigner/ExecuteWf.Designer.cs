namespace Infosys.ATR.WFDesigner.Views
{
    partial class ExecuteWf
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grpBoxRunScriptOptions = new System.Windows.Forms.GroupBox();
            this.rdSchedule = new System.Windows.Forms.RadioButton();
            this.rdIapNodes = new System.Windows.Forms.RadioButton();
            this.rdLocally = new System.Windows.Forms.RadioButton();
            this.grpBoxScriptParams = new System.Windows.Forms.GroupBox();
            this.btnReset = new System.Windows.Forms.Button();
            this.pnlTextBoxes = new System.Windows.Forms.Panel();
            this.grpIAPNode = new System.Windows.Forms.GroupBox();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pnlScheduledRequest = new System.Windows.Forms.Panel();
            this.chkTraceWFDetails = new System.Windows.Forms.CheckBox();
            this.grpBoxRunScriptOptions.SuspendLayout();
            this.grpBoxScriptParams.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpBoxRunScriptOptions
            // 
            this.grpBoxRunScriptOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpBoxRunScriptOptions.Controls.Add(this.rdSchedule);
            this.grpBoxRunScriptOptions.Controls.Add(this.rdIapNodes);
            this.grpBoxRunScriptOptions.Controls.Add(this.rdLocally);
            this.grpBoxRunScriptOptions.Location = new System.Drawing.Point(12, 12);
            this.grpBoxRunScriptOptions.Name = "grpBoxRunScriptOptions";
            this.grpBoxRunScriptOptions.Size = new System.Drawing.Size(602, 47);
            this.grpBoxRunScriptOptions.TabIndex = 0;
            this.grpBoxRunScriptOptions.TabStop = false;
            this.grpBoxRunScriptOptions.Text = "Run Workflow Options";
            // 
            // rdSchedule
            // 
            this.rdSchedule.AutoSize = true;
            this.rdSchedule.Location = new System.Drawing.Point(449, 20);
            this.rdSchedule.Name = "rdSchedule";
            this.rdSchedule.Size = new System.Drawing.Size(118, 17);
            this.rdSchedule.TabIndex = 2;
            this.rdSchedule.Text = "Schedule Workflow";
            this.rdSchedule.UseVisualStyleBackColor = true;
            this.rdSchedule.CheckedChanged += new System.EventHandler(this.rdSchedule_CheckedChanged);
            // 
            // rdIapNodes
            // 
            this.rdIapNodes.AutoSize = true;
            this.rdIapNodes.Location = new System.Drawing.Point(218, 19);
            this.rdIapNodes.Name = "rdIapNodes";
            this.rdIapNodes.Size = new System.Drawing.Size(122, 17);
            this.rdIapNodes.TabIndex = 1;
            this.rdIapNodes.Text = "Run On IAP Node(s)";
            this.rdIapNodes.UseVisualStyleBackColor = true;
            this.rdIapNodes.CheckedChanged += new System.EventHandler(this.rdIapNodes_CheckedChanged);
            // 
            // rdLocally
            // 
            this.rdLocally.AutoSize = true;
            this.rdLocally.Location = new System.Drawing.Point(23, 20);
            this.rdLocally.Name = "rdLocally";
            this.rdLocally.Size = new System.Drawing.Size(81, 17);
            this.rdLocally.TabIndex = 0;
            this.rdLocally.Text = "Run Locally";
            this.rdLocally.UseVisualStyleBackColor = true;
            this.rdLocally.CheckedChanged += new System.EventHandler(this.rdLocally_CheckedChanged);
            // 
            // grpBoxScriptParams
            // 
            this.grpBoxScriptParams.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpBoxScriptParams.Controls.Add(this.btnReset);
            this.grpBoxScriptParams.Controls.Add(this.pnlTextBoxes);
            this.grpBoxScriptParams.Location = new System.Drawing.Point(13, 65);
            this.grpBoxScriptParams.Name = "grpBoxScriptParams";
            this.grpBoxScriptParams.Size = new System.Drawing.Size(601, 85);
            this.grpBoxScriptParams.TabIndex = 3;
            this.grpBoxScriptParams.TabStop = false;
            this.grpBoxScriptParams.Text = "Workflow Parameters";
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(458, 56);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 10;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click_1);
            // 
            // pnlTextBoxes
            // 
            this.pnlTextBoxes.Location = new System.Drawing.Point(6, 19);
            this.pnlTextBoxes.Name = "pnlTextBoxes";
            this.pnlTextBoxes.Size = new System.Drawing.Size(592, 34);
            this.pnlTextBoxes.TabIndex = 0;
            // 
            // grpIAPNode
            // 
            this.grpIAPNode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpIAPNode.Location = new System.Drawing.Point(13, 235);
            this.grpIAPNode.Name = "grpIAPNode";
            this.grpIAPNode.Size = new System.Drawing.Size(601, 29);
            this.grpIAPNode.TabIndex = 4;
            this.grpIAPNode.TabStop = false;
            this.grpIAPNode.Text = "IAP Node(s)";
            // 
            // pnlButtons
            // 
            this.pnlButtons.Controls.Add(this.chkTraceWFDetails);
            this.pnlButtons.Controls.Add(this.btnRun);
            this.pnlButtons.Controls.Add(this.btnCancel);
            this.pnlButtons.Location = new System.Drawing.Point(12, 299);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(601, 29);
            this.pnlButtons.TabIndex = 5;
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(428, 3);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(91, 23);
            this.btnRun.TabIndex = 7;
            this.btnRun.Text = "Run Workflow";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click_1);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(524, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click_1);
            // 
            // pnlScheduledRequest
            // 
            this.pnlScheduledRequest.Location = new System.Drawing.Point(13, 158);
            this.pnlScheduledRequest.Name = "pnlScheduledRequest";
            this.pnlScheduledRequest.Size = new System.Drawing.Size(601, 50);
            this.pnlScheduledRequest.TabIndex = 6;
            // 
            // chkTraceWFDetails
            // 
            this.chkTraceWFDetails.AutoSize = true;
            this.chkTraceWFDetails.Location = new System.Drawing.Point(5, 6);
            this.chkTraceWFDetails.Name = "chkTraceWFDetails";
            this.chkTraceWFDetails.Size = new System.Drawing.Size(137, 17);
            this.chkTraceWFDetails.TabIndex = 8;
            this.chkTraceWFDetails.Text = "Trace Workflow Details";
            this.chkTraceWFDetails.UseVisualStyleBackColor = true;
            this.chkTraceWFDetails.CheckedChanged += new System.EventHandler(this.chkTraceWFDetails_CheckedChanged);
            // 
            // ExecuteWf
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 342);
            this.Controls.Add(this.pnlScheduledRequest);
            this.Controls.Add(this.pnlButtons);
            this.Controls.Add(this.grpIAPNode);
            this.Controls.Add(this.grpBoxScriptParams);
            this.Controls.Add(this.grpBoxRunScriptOptions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExecuteWf";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Execute Workflow";
            this.grpBoxRunScriptOptions.ResumeLayout(false);
            this.grpBoxRunScriptOptions.PerformLayout();
            this.grpBoxScriptParams.ResumeLayout(false);
            this.pnlButtons.ResumeLayout(false);
            this.pnlButtons.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpBoxRunScriptOptions;
        private System.Windows.Forms.RadioButton rdLocally;
        private System.Windows.Forms.RadioButton rdIapNodes;
        private System.Windows.Forms.GroupBox grpBoxScriptParams;
        private System.Windows.Forms.GroupBox grpIAPNode;
        private System.Windows.Forms.Panel pnlTextBoxes;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.RadioButton rdSchedule;
        private System.Windows.Forms.Panel pnlScheduledRequest;
        private System.Windows.Forms.CheckBox chkTraceWFDetails;
    }
}
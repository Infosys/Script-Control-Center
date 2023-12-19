namespace Infosys.ATR.ScriptsRepository.Views.Scripts
{
    partial class ScriptParameters
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
            this.components = new System.ComponentModel.Container();
            this.btnAddToList = new System.Windows.Forms.Button();
            this.lblServerName = new System.Windows.Forms.Label();
            this.txtServerName = new System.Windows.Forms.TextBox();
            this.lblServerList = new System.Windows.Forms.Label();
            this.btnRemove = new System.Windows.Forms.Button();
            this.lstServerName = new System.Windows.Forms.ListBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnRunScript = new System.Windows.Forms.Button();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.grpBoxRunScriptOptions = new System.Windows.Forms.GroupBox();
            this.rdSchedule = new System.Windows.Forms.RadioButton();
            this.rdIAPNode = new System.Windows.Forms.RadioButton();
            this.pnlRemoteControls = new System.Windows.Forms.Panel();
            this.pnlLocalControls = new System.Windows.Forms.Panel();
            this.cmbShellServerList = new System.Windows.Forms.ComboBox();
            this.lblProcessing = new System.Windows.Forms.Label();
            this.lblDisplayServerName = new System.Windows.Forms.Label();
            this.rdbRemote = new System.Windows.Forms.RadioButton();
            this.rdbLocal = new System.Windows.Forms.RadioButton();
            this.grpBoxScriptParams = new System.Windows.Forms.GroupBox();
            this.pnlTextBoxes = new System.Windows.Forms.Panel();
            this.grpIAPNode = new System.Windows.Forms.GroupBox();
            this.pnlScheduledRequest = new System.Windows.Forms.Panel();
            this.pnlUserCredentials = new System.Windows.Forms.Panel();
            this.rbPassAuth = new System.Windows.Forms.RadioButton();
            this.lblPasswordValidation = new System.Windows.Forms.Label();
            this.rbKeyAuth = new System.Windows.Forms.RadioButton();
            this.lblUserNameValidation = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.lblUserName = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.grpBoxRemoteParameters = new System.Windows.Forms.GroupBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pnlButtons.SuspendLayout();
            this.grpBoxRunScriptOptions.SuspendLayout();
            this.pnlRemoteControls.SuspendLayout();
            this.pnlLocalControls.SuspendLayout();
            this.grpBoxScriptParams.SuspendLayout();
            this.pnlUserCredentials.SuspendLayout();
            this.grpBoxRemoteParameters.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAddToList
            // 
            this.btnAddToList.Location = new System.Drawing.Point(735, 3);
            this.btnAddToList.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnAddToList.Name = "btnAddToList";
            this.btnAddToList.Size = new System.Drawing.Size(112, 35);
            this.btnAddToList.TabIndex = 4;
            this.btnAddToList.Text = "Add To List";
            this.btnAddToList.UseVisualStyleBackColor = true;
            this.btnAddToList.Click += new System.EventHandler(this.btnAddToList_Click);
            // 
            // lblServerName
            // 
            this.lblServerName.AutoSize = true;
            this.lblServerName.Location = new System.Drawing.Point(2, 8);
            this.lblServerName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblServerName.Name = "lblServerName";
            this.lblServerName.Size = new System.Drawing.Size(115, 20);
            this.lblServerName.TabIndex = 3;
            this.lblServerName.Text = "Machine Name";
            // 
            // txtServerName
            // 
            this.txtServerName.Location = new System.Drawing.Point(256, 6);
            this.txtServerName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtServerName.Name = "txtServerName";
            this.txtServerName.Size = new System.Drawing.Size(457, 26);
            this.txtServerName.TabIndex = 2;
            // 
            // lblServerList
            // 
            this.lblServerList.AutoSize = true;
            this.lblServerList.Location = new System.Drawing.Point(2, 6);
            this.lblServerList.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblServerList.Name = "lblServerList";
            this.lblServerList.Size = new System.Drawing.Size(84, 20);
            this.lblServerList.TabIndex = 10;
            this.lblServerList.Text = "Server List";
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(732, 12);
            this.btnRemove.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(112, 40);
            this.btnRemove.TabIndex = 6;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // lstServerName
            // 
            this.lstServerName.FormattingEnabled = true;
            this.lstServerName.ItemHeight = 20;
            this.lstServerName.Location = new System.Drawing.Point(255, 6);
            this.lstServerName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lstServerName.Name = "lstServerName";
            this.lstServerName.Size = new System.Drawing.Size(458, 124);
            this.lstServerName.TabIndex = 5;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(298, 5);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(112, 35);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(166, 5);
            this.btnReset.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(112, 35);
            this.btnReset.TabIndex = 8;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnRunScript
            // 
            this.btnRunScript.Enabled = false;
            this.btnRunScript.Location = new System.Drawing.Point(14, 5);
            this.btnRunScript.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnRunScript.Name = "btnRunScript";
            this.btnRunScript.Size = new System.Drawing.Size(128, 35);
            this.btnRunScript.TabIndex = 7;
            this.btnRunScript.Text = "Run Script";
            this.btnRunScript.UseVisualStyleBackColor = true;
            this.btnRunScript.Click += new System.EventHandler(this.btnRunScript_Click);
            // 
            // pnlButtons
            // 
            this.pnlButtons.Controls.Add(this.btnRunScript);
            this.pnlButtons.Controls.Add(this.btnCancel);
            this.pnlButtons.Controls.Add(this.btnReset);
            this.pnlButtons.Location = new System.Drawing.Point(514, 598);
            this.pnlButtons.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(416, 48);
            this.pnlButtons.TabIndex = 2;
            // 
            // grpBoxRunScriptOptions
            // 
            this.grpBoxRunScriptOptions.Controls.Add(this.rdSchedule);
            this.grpBoxRunScriptOptions.Controls.Add(this.rdIAPNode);
            this.grpBoxRunScriptOptions.Controls.Add(this.pnlRemoteControls);
            this.grpBoxRunScriptOptions.Controls.Add(this.pnlLocalControls);
            this.grpBoxRunScriptOptions.Controls.Add(this.rdbRemote);
            this.grpBoxRunScriptOptions.Controls.Add(this.rdbLocal);
            this.grpBoxRunScriptOptions.Location = new System.Drawing.Point(18, 18);
            this.grpBoxRunScriptOptions.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpBoxRunScriptOptions.Name = "grpBoxRunScriptOptions";
            this.grpBoxRunScriptOptions.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpBoxRunScriptOptions.Size = new System.Drawing.Size(912, 258);
            this.grpBoxRunScriptOptions.TabIndex = 12;
            this.grpBoxRunScriptOptions.TabStop = false;
            this.grpBoxRunScriptOptions.Text = "Run Script Options";
            // 
            // rdSchedule
            // 
            this.rdSchedule.AutoSize = true;
            this.rdSchedule.Location = new System.Drawing.Point(702, 31);
            this.rdSchedule.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rdSchedule.Name = "rdSchedule";
            this.rdSchedule.Size = new System.Drawing.Size(146, 24);
            this.rdSchedule.TabIndex = 7;
            this.rdSchedule.Text = "Schedule Script";
            this.rdSchedule.UseVisualStyleBackColor = true;
            this.rdSchedule.CheckedChanged += new System.EventHandler(this.rdSchedule_CheckedChanged);
            // 
            // rdIAPNode
            // 
            this.rdIAPNode.AutoSize = true;
            this.rdIAPNode.Location = new System.Drawing.Point(402, 29);
            this.rdIAPNode.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rdIAPNode.Name = "rdIAPNode";
            this.rdIAPNode.Size = new System.Drawing.Size(176, 24);
            this.rdIAPNode.TabIndex = 6;
            this.rdIAPNode.TabStop = true;
            this.rdIAPNode.Text = "Run on IAP Node(s)";
            this.rdIAPNode.UseVisualStyleBackColor = true;
            this.rdIAPNode.CheckedChanged += new System.EventHandler(this.rdIAPNode_CheckedChanged);
            // 
            // pnlRemoteControls
            // 
            this.pnlRemoteControls.Controls.Add(this.btnRemove);
            this.pnlRemoteControls.Controls.Add(this.lblServerList);
            this.pnlRemoteControls.Controls.Add(this.lstServerName);
            this.pnlRemoteControls.Location = new System.Drawing.Point(10, 115);
            this.pnlRemoteControls.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlRemoteControls.Name = "pnlRemoteControls";
            this.pnlRemoteControls.Size = new System.Drawing.Size(849, 135);
            this.pnlRemoteControls.TabIndex = 5;
            // 
            // pnlLocalControls
            // 
            this.pnlLocalControls.Controls.Add(this.cmbShellServerList);
            this.pnlLocalControls.Controls.Add(this.lblProcessing);
            this.pnlLocalControls.Controls.Add(this.lblDisplayServerName);
            this.pnlLocalControls.Controls.Add(this.btnAddToList);
            this.pnlLocalControls.Controls.Add(this.lblServerName);
            this.pnlLocalControls.Controls.Add(this.txtServerName);
            this.pnlLocalControls.Location = new System.Drawing.Point(10, 66);
            this.pnlLocalControls.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlLocalControls.Name = "pnlLocalControls";
            this.pnlLocalControls.Size = new System.Drawing.Size(849, 48);
            this.pnlLocalControls.TabIndex = 4;
            // 
            // cmbShellServerList
            // 
            this.cmbShellServerList.FormattingEnabled = true;
            this.cmbShellServerList.Location = new System.Drawing.Point(258, 3);
            this.cmbShellServerList.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbShellServerList.Name = "cmbShellServerList";
            this.cmbShellServerList.Size = new System.Drawing.Size(456, 28);
            this.cmbShellServerList.TabIndex = 7;
            // 
            // lblProcessing
            // 
            this.lblProcessing.AutoSize = true;
            this.lblProcessing.ForeColor = System.Drawing.Color.Red;
            this.lblProcessing.Location = new System.Drawing.Point(256, 31);
            this.lblProcessing.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblProcessing.Name = "lblProcessing";
            this.lblProcessing.Size = new System.Drawing.Size(99, 20);
            this.lblProcessing.TabIndex = 6;
            this.lblProcessing.Text = "Processing...";
            // 
            // lblDisplayServerName
            // 
            this.lblDisplayServerName.AutoSize = true;
            this.lblDisplayServerName.Location = new System.Drawing.Point(252, 11);
            this.lblDisplayServerName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDisplayServerName.Name = "lblDisplayServerName";
            this.lblDisplayServerName.Size = new System.Drawing.Size(198, 20);
            this.lblDisplayServerName.TabIndex = 5;
            this.lblDisplayServerName.Text = "Display Local Server Name";
            // 
            // rdbRemote
            // 
            this.rdbRemote.AutoSize = true;
            this.rdbRemote.Location = new System.Drawing.Point(176, 31);
            this.rdbRemote.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rdbRemote.Name = "rdbRemote";
            this.rdbRemote.Size = new System.Drawing.Size(135, 24);
            this.rdbRemote.TabIndex = 3;
            this.rdbRemote.TabStop = true;
            this.rdbRemote.Text = "Run Remotely";
            this.rdbRemote.UseVisualStyleBackColor = true;
            this.rdbRemote.CheckedChanged += new System.EventHandler(this.rdbRemote_CheckedChanged);
            // 
            // rdbLocal
            // 
            this.rdbLocal.AutoSize = true;
            this.rdbLocal.Location = new System.Drawing.Point(9, 29);
            this.rdbLocal.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rdbLocal.Name = "rdbLocal";
            this.rdbLocal.Size = new System.Drawing.Size(116, 24);
            this.rdbLocal.TabIndex = 2;
            this.rdbLocal.TabStop = true;
            this.rdbLocal.Text = "Run Locally";
            this.rdbLocal.UseVisualStyleBackColor = true;
            this.rdbLocal.CheckedChanged += new System.EventHandler(this.rdbLocal_CheckedChanged);
            // 
            // grpBoxScriptParams
            // 
            this.grpBoxScriptParams.Controls.Add(this.pnlTextBoxes);
            this.grpBoxScriptParams.Location = new System.Drawing.Point(16, 280);
            this.grpBoxScriptParams.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpBoxScriptParams.Name = "grpBoxScriptParams";
            this.grpBoxScriptParams.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpBoxScriptParams.Size = new System.Drawing.Size(914, 91);
            this.grpBoxScriptParams.TabIndex = 13;
            this.grpBoxScriptParams.TabStop = false;
            this.grpBoxScriptParams.Text = "Script Parameters";
            // 
            // pnlTextBoxes
            // 
            this.pnlTextBoxes.Location = new System.Drawing.Point(9, 23);
            this.pnlTextBoxes.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlTextBoxes.Name = "pnlTextBoxes";
            this.pnlTextBoxes.Size = new System.Drawing.Size(850, 58);
            this.pnlTextBoxes.TabIndex = 0;
            // 
            // grpIAPNode
            // 
            this.grpIAPNode.Location = new System.Drawing.Point(18, 431);
            this.grpIAPNode.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpIAPNode.Name = "grpIAPNode";
            this.grpIAPNode.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpIAPNode.Size = new System.Drawing.Size(912, 15);
            this.grpIAPNode.TabIndex = 16;
            this.grpIAPNode.TabStop = false;
            this.grpIAPNode.Text = "IAP Node(s)";
            // 
            // pnlScheduledRequest
            // 
            this.pnlScheduledRequest.Location = new System.Drawing.Point(18, 372);
            this.pnlScheduledRequest.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlScheduledRequest.Name = "pnlScheduledRequest";
            this.pnlScheduledRequest.Size = new System.Drawing.Size(912, 38);
            this.pnlScheduledRequest.TabIndex = 17;
            // 
            // pnlUserCredentials
            // 
            this.pnlUserCredentials.Controls.Add(this.rbPassAuth);
            this.pnlUserCredentials.Controls.Add(this.lblPasswordValidation);
            this.pnlUserCredentials.Controls.Add(this.rbKeyAuth);
            this.pnlUserCredentials.Controls.Add(this.lblUserNameValidation);
            this.pnlUserCredentials.Controls.Add(this.txtPassword);
            this.pnlUserCredentials.Controls.Add(this.txtUserName);
            this.pnlUserCredentials.Controls.Add(this.lblUserName);
            this.pnlUserCredentials.Controls.Add(this.lblPassword);
            this.pnlUserCredentials.Location = new System.Drawing.Point(16, 25);
            this.pnlUserCredentials.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlUserCredentials.Name = "pnlUserCredentials";
            this.pnlUserCredentials.Size = new System.Drawing.Size(849, 125);
            this.pnlUserCredentials.TabIndex = 19;
            // 
            // rbPassAuth
            // 
            this.rbPassAuth.AutoSize = true;
            this.rbPassAuth.Checked = true;
            this.rbPassAuth.Location = new System.Drawing.Point(546, 5);
            this.rbPassAuth.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rbPassAuth.Name = "rbPassAuth";
            this.rbPassAuth.Size = new System.Drawing.Size(210, 24);
            this.rbPassAuth.TabIndex = 21;
            this.rbPassAuth.TabStop = true;
            this.rbPassAuth.Text = "Password Authentication";
            this.rbPassAuth.UseVisualStyleBackColor = true;
            // 
            // lblPasswordValidation
            // 
            this.lblPasswordValidation.AutoSize = true;
            this.lblPasswordValidation.ForeColor = System.Drawing.Color.Red;
            this.lblPasswordValidation.Location = new System.Drawing.Point(122, 91);
            this.lblPasswordValidation.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPasswordValidation.Name = "lblPasswordValidation";
            this.lblPasswordValidation.Size = new System.Drawing.Size(15, 20);
            this.lblPasswordValidation.TabIndex = 23;
            this.lblPasswordValidation.Text = "*";
            // 
            // rbKeyAuth
            // 
            this.rbKeyAuth.AutoSize = true;
            this.rbKeyAuth.Location = new System.Drawing.Point(255, 5);
            this.rbKeyAuth.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rbKeyAuth.Name = "rbKeyAuth";
            this.rbKeyAuth.Size = new System.Drawing.Size(167, 24);
            this.rbKeyAuth.TabIndex = 20;
            this.rbKeyAuth.Text = "Key Authentication";
            this.rbKeyAuth.UseVisualStyleBackColor = true;
            this.rbKeyAuth.CheckedChanged += new System.EventHandler(this.rbKeyAuth_CheckedChanged);
            // 
            // lblUserNameValidation
            // 
            this.lblUserNameValidation.AutoSize = true;
            this.lblUserNameValidation.ForeColor = System.Drawing.Color.Red;
            this.lblUserNameValidation.Location = new System.Drawing.Point(122, 48);
            this.lblUserNameValidation.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUserNameValidation.Name = "lblUserNameValidation";
            this.lblUserNameValidation.Size = new System.Drawing.Size(15, 20);
            this.lblUserNameValidation.TabIndex = 18;
            this.lblUserNameValidation.Text = "*";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(248, 80);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(590, 26);
            this.txtPassword.TabIndex = 22;
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(248, 40);
            this.txtUserName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(592, 26);
            this.txtUserName.TabIndex = 20;
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(3, 48);
            this.lblUserName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(81, 20);
            this.lblUserName.TabIndex = 19;
            this.lblUserName.Text = "User Alias";
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(4, 91);
            this.lblPassword.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(78, 20);
            this.lblPassword.TabIndex = 21;
            this.lblPassword.Text = "Password";
            // 
            // grpBoxRemoteParameters
            // 
            this.grpBoxRemoteParameters.Controls.Add(this.pnlUserCredentials);
            this.grpBoxRemoteParameters.Location = new System.Drawing.Point(18, 431);
            this.grpBoxRemoteParameters.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpBoxRemoteParameters.Name = "grpBoxRemoteParameters";
            this.grpBoxRemoteParameters.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpBoxRemoteParameters.Size = new System.Drawing.Size(912, 158);
            this.grpBoxRemoteParameters.TabIndex = 15;
            this.grpBoxRemoteParameters.TabStop = false;
            this.grpBoxRemoteParameters.Text = "Remote Credentials";
            // 
            // ScriptParameters
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(948, 657);
            this.Controls.Add(this.pnlScheduledRequest);
            this.Controls.Add(this.grpIAPNode);
            this.Controls.Add(this.grpBoxRemoteParameters);
            this.Controls.Add(this.grpBoxScriptParams);
            this.Controls.Add(this.grpBoxRunScriptOptions);
            this.Controls.Add(this.pnlButtons);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ScriptParameters";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Script Parameters";
            this.pnlButtons.ResumeLayout(false);
            this.grpBoxRunScriptOptions.ResumeLayout(false);
            this.grpBoxRunScriptOptions.PerformLayout();
            this.pnlRemoteControls.ResumeLayout(false);
            this.pnlRemoteControls.PerformLayout();
            this.pnlLocalControls.ResumeLayout(false);
            this.pnlLocalControls.PerformLayout();
            this.grpBoxScriptParams.ResumeLayout(false);
            this.pnlUserCredentials.ResumeLayout(false);
            this.pnlUserCredentials.PerformLayout();
            this.grpBoxRemoteParameters.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblServerName;
        private System.Windows.Forms.TextBox txtServerName;
        private System.Windows.Forms.Button btnAddToList;
        private System.Windows.Forms.ListBox lstServerName;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnRunScript;
        private System.Windows.Forms.Label lblServerList;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.GroupBox grpBoxRunScriptOptions;
        private System.Windows.Forms.RadioButton rdbRemote;
        private System.Windows.Forms.RadioButton rdbLocal;
        private System.Windows.Forms.Panel pnlLocalControls;
        private System.Windows.Forms.Panel pnlRemoteControls;
        private System.Windows.Forms.GroupBox grpBoxScriptParams;
        private System.Windows.Forms.Panel pnlTextBoxes;
        private System.Windows.Forms.Label lblDisplayServerName;
        private System.Windows.Forms.Label lblProcessing;
        private System.Windows.Forms.RadioButton rdIAPNode;
        private System.Windows.Forms.GroupBox grpIAPNode;
        private System.Windows.Forms.RadioButton rdSchedule;
        private System.Windows.Forms.Panel pnlScheduledRequest;
        private System.Windows.Forms.Panel pnlUserCredentials;
        private System.Windows.Forms.Label lblPasswordValidation;
        private System.Windows.Forms.Label lblUserNameValidation;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.GroupBox grpBoxRemoteParameters;
        private System.Windows.Forms.RadioButton rbPassAuth;
        private System.Windows.Forms.RadioButton rbKeyAuth;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ComboBox cmbShellServerList;
    }
}
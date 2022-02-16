namespace Infosys.ATR.ExportUtility
{
    partial class ExportUtility
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabConfigExport = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.tvSource = new System.Windows.Forms.TreeView();
            this.btnAdd = new System.Windows.Forms.Button();
            this.tvDestination = new System.Windows.Forms.TreeView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbServerInstance = new System.Windows.Forms.ComboBox();
            this.chkRetrieveScripts = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtCasServer = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cmbServerType = new System.Windows.Forms.ComboBox();
            this.btnGo = new System.Windows.Forms.Button();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUserId = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTargetServer = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPastExports = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.cmbPastExportStatus = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cmbExportList = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.cmbFilterCriteria = new System.Windows.Forms.ComboBox();
            this.cmbUserList = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCancelExport = new System.Windows.Forms.Button();
            this.btnOverwrite = new System.Windows.Forms.Button();
            this.grdPastExport = new System.Windows.Forms.DataGridView();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.tabControl1.SuspendLayout();
            this.tabConfigExport.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPastExports.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdPastExport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tabControl1.Controls.Add(this.tabConfigExport);
            this.tabControl1.Controls.Add(this.tabPastExports);
            this.tabControl1.Location = new System.Drawing.Point(1, 12);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(10, 3, 3, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(768, 483);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabConfigExport
            // 
            this.tabConfigExport.BackColor = System.Drawing.SystemColors.Control;
            this.tabConfigExport.Controls.Add(this.groupBox2);
            this.tabConfigExport.Controls.Add(this.groupBox1);
            this.tabConfigExport.Location = new System.Drawing.Point(4, 25);
            this.tabConfigExport.Name = "tabConfigExport";
            this.tabConfigExport.Padding = new System.Windows.Forms.Padding(3);
            this.tabConfigExport.Size = new System.Drawing.Size(760, 454);
            this.tabConfigExport.TabIndex = 0;
            this.tabConfigExport.Text = "Configure Export";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnCancel);
            this.groupBox2.Controls.Add(this.btnExport);
            this.groupBox2.Controls.Add(this.tvSource);
            this.groupBox2.Controls.Add(this.btnAdd);
            this.groupBox2.Controls.Add(this.tvDestination);
            this.groupBox2.Location = new System.Drawing.Point(0, 169);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(755, 279);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Categories/Script to Export";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(539, 249);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 25);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(649, 249);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(100, 25);
            this.btnExport.TabIndex = 4;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // tvSource
            // 
            this.tvSource.Location = new System.Drawing.Point(10, 19);
            this.tvSource.Name = "tvSource";
            this.tvSource.Size = new System.Drawing.Size(329, 219);
            this.tvSource.TabIndex = 0;
            this.tvSource.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvSource_AfterSelect);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(355, 103);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(57, 41);
            this.btnAdd.TabIndex = 1;
            this.btnAdd.Text = ">>";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // tvDestination
            // 
            this.tvDestination.Location = new System.Drawing.Point(427, 19);
            this.tvDestination.Name = "tvDestination";
            this.tvDestination.Size = new System.Drawing.Size(322, 219);
            this.tvDestination.TabIndex = 2;
            this.tvDestination.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvDestination_AfterSelect);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.cmbServerInstance);
            this.groupBox1.Controls.Add(this.chkRetrieveScripts);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtCasServer);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.cmbServerType);
            this.groupBox1.Controls.Add(this.btnGo);
            this.groupBox1.Controls.Add(this.txtPassword);
            this.groupBox1.Controls.Add(this.txtUserId);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtTargetServer);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(0, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(755, 157);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Target Server Login";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 60);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(82, 13);
            this.label5.TabIndex = 23;
            this.label5.Text = "Server Instance";
            // 
            // cmbServerInstance
            // 
            this.cmbServerInstance.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbServerInstance.FormattingEnabled = true;
            this.cmbServerInstance.Location = new System.Drawing.Point(91, 57);
            this.cmbServerInstance.Name = "cmbServerInstance";
            this.cmbServerInstance.Size = new System.Drawing.Size(264, 21);
            this.cmbServerInstance.TabIndex = 22;
            this.cmbServerInstance.SelectedIndexChanged += new System.EventHandler(this.cmbServerInstance_SelectedIndexChanged);
            this.cmbServerInstance.SelectionChangeCommitted += new System.EventHandler(this.cmbServerInstance_SelectionChangeCommitted);
            // 
            // chkRetrieveScripts
            // 
            this.chkRetrieveScripts.AutoSize = true;
            this.chkRetrieveScripts.Location = new System.Drawing.Point(486, 95);
            this.chkRetrieveScripts.Name = "chkRetrieveScripts";
            this.chkRetrieveScripts.Size = new System.Drawing.Size(101, 17);
            this.chkRetrieveScripts.TabIndex = 21;
            this.chkRetrieveScripts.Text = "Retrieve Scripts";
            this.chkRetrieveScripts.UseVisualStyleBackColor = true;
            this.chkRetrieveScripts.CheckedChanged += new System.EventHandler(this.chkRetrieveScripts_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 134);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 20;
            this.label4.Text = "Cas Server";
            // 
            // txtCasServer
            // 
            this.txtCasServer.Location = new System.Drawing.Point(91, 131);
            this.txtCasServer.Name = "txtCasServer";
            this.txtCasServer.Size = new System.Drawing.Size(264, 20);
            this.txtCasServer.TabIndex = 2;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 25);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 13);
            this.label8.TabIndex = 18;
            this.label8.Text = "Server Type";
            // 
            // cmbServerType
            // 
            this.cmbServerType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbServerType.FormattingEnabled = true;
            this.cmbServerType.Location = new System.Drawing.Point(91, 22);
            this.cmbServerType.Name = "cmbServerType";
            this.cmbServerType.Size = new System.Drawing.Size(264, 21);
            this.cmbServerType.TabIndex = 0;
            this.cmbServerType.SelectedIndexChanged += new System.EventHandler(this.cmbServerType_SelectedIndexChanged);
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(650, 113);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(100, 25);
            this.btnGo.TabIndex = 5;
            this.btnGo.Text = "Go";
            this.btnGo.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(486, 58);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(264, 20);
            this.txtPassword.TabIndex = 4;
            // 
            // txtUserId
            // 
            this.txtUserId.Location = new System.Drawing.Point(486, 25);
            this.txtUserId.Name = "txtUserId";
            this.txtUserId.Size = new System.Drawing.Size(264, 20);
            this.txtUserId.TabIndex = 3;
            this.txtUserId.Tag = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(415, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Password";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(427, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "User Id";
            // 
            // txtTargetServer
            // 
            this.txtTargetServer.Location = new System.Drawing.Point(91, 96);
            this.txtTargetServer.Name = "txtTargetServer";
            this.txtTargetServer.Size = new System.Drawing.Size(264, 20);
            this.txtTargetServer.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 99);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "DNS Server";
            // 
            // tabPastExports
            // 
            this.tabPastExports.Controls.Add(this.groupBox3);
            this.tabPastExports.Location = new System.Drawing.Point(4, 25);
            this.tabPastExports.Name = "tabPastExports";
            this.tabPastExports.Padding = new System.Windows.Forms.Padding(3);
            this.tabPastExports.Size = new System.Drawing.Size(760, 454);
            this.tabPastExports.TabIndex = 1;
            this.tabPastExports.Text = "Past Exports";
            this.tabPastExports.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.groupBox5);
            this.groupBox3.Controls.Add(this.groupBox4);
            this.groupBox3.Controls.Add(this.panel1);
            this.groupBox3.Controls.Add(this.grdPastExport);
            this.groupBox3.Location = new System.Drawing.Point(7, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(747, 442);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Script Export Details";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label10);
            this.groupBox5.Controls.Add(this.cmbPastExportStatus);
            this.groupBox5.Location = new System.Drawing.Point(6, 82);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(735, 60);
            this.groupBox5.TabIndex = 30;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Export Details";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 22);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(70, 13);
            this.label10.TabIndex = 28;
            this.label10.Text = "Export Status";
            // 
            // cmbPastExportStatus
            // 
            this.cmbPastExportStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPastExportStatus.FormattingEnabled = true;
            this.cmbPastExportStatus.Location = new System.Drawing.Point(78, 18);
            this.cmbPastExportStatus.Name = "cmbPastExportStatus";
            this.cmbPastExportStatus.Size = new System.Drawing.Size(151, 21);
            this.cmbPastExportStatus.TabIndex = 29;
            this.cmbPastExportStatus.SelectedIndexChanged += new System.EventHandler(this.cmbPastExportStatus_SelectedIndexChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cmbExportList);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.cmbFilterCriteria);
            this.groupBox4.Controls.Add(this.cmbUserList);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Location = new System.Drawing.Point(6, 19);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(735, 62);
            this.groupBox4.TabIndex = 26;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Export Configuration Selection Criteria";
            // 
            // cmbExportList
            // 
            this.cmbExportList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbExportList.FormattingEnabled = true;
            this.cmbExportList.Location = new System.Drawing.Point(572, 32);
            this.cmbExportList.Name = "cmbExportList";
            this.cmbExportList.Size = new System.Drawing.Size(160, 21);
            this.cmbExportList.TabIndex = 15;
            this.cmbExportList.SelectedIndexChanged += new System.EventHandler(this.cmbExportList_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(1, 32);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(70, 13);
            this.label9.TabIndex = 25;
            this.label9.Text = "Export Period";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(476, 35);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(96, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Select Export Date";
            // 
            // cmbFilterCriteria
            // 
            this.cmbFilterCriteria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFilterCriteria.FormattingEnabled = true;
            this.cmbFilterCriteria.Location = new System.Drawing.Point(76, 27);
            this.cmbFilterCriteria.Name = "cmbFilterCriteria";
            this.cmbFilterCriteria.Size = new System.Drawing.Size(153, 21);
            this.cmbFilterCriteria.TabIndex = 24;
            this.cmbFilterCriteria.SelectedIndexChanged += new System.EventHandler(this.cmbFilterCriteria_SelectedIndexChanged);
            // 
            // cmbUserList
            // 
            this.cmbUserList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUserList.FormattingEnabled = true;
            this.cmbUserList.Location = new System.Drawing.Point(308, 29);
            this.cmbUserList.Name = "cmbUserList";
            this.cmbUserList.Size = new System.Drawing.Size(159, 21);
            this.cmbUserList.TabIndex = 22;
            this.cmbUserList.SelectedIndexChanged += new System.EventHandler(this.cmbUserList_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(244, 32);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 13);
            this.label6.TabIndex = 23;
            this.label6.Text = "Select User";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnCancelExport);
            this.panel1.Controls.Add(this.btnOverwrite);
            this.panel1.Location = new System.Drawing.Point(520, 406);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(221, 33);
            this.panel1.TabIndex = 21;
            // 
            // btnCancelExport
            // 
            this.btnCancelExport.Location = new System.Drawing.Point(4, 5);
            this.btnCancelExport.Name = "btnCancelExport";
            this.btnCancelExport.Size = new System.Drawing.Size(100, 25);
            this.btnCancelExport.TabIndex = 22;
            this.btnCancelExport.Text = "Cancel";
            this.btnCancelExport.UseVisualStyleBackColor = true;
            this.btnCancelExport.Click += new System.EventHandler(this.btnCancelExport_Click);
            // 
            // btnOverwrite
            // 
            this.btnOverwrite.Location = new System.Drawing.Point(115, 5);
            this.btnOverwrite.Name = "btnOverwrite";
            this.btnOverwrite.Size = new System.Drawing.Size(100, 25);
            this.btnOverwrite.TabIndex = 21;
            this.btnOverwrite.Text = "Submit";
            this.btnOverwrite.UseVisualStyleBackColor = true;
            this.btnOverwrite.Click += new System.EventHandler(this.btnOverwrite_Click);
            // 
            // grdPastExport
            // 
            this.grdPastExport.BackgroundColor = System.Drawing.SystemColors.Control;
            this.grdPastExport.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdPastExport.Location = new System.Drawing.Point(6, 148);
            this.grdPastExport.Name = "grdPastExport";
            this.grdPastExport.Size = new System.Drawing.Size(735, 255);
            this.grdPastExport.TabIndex = 17;
            this.grdPastExport.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.grdPastExport_CellFormatting);
            this.grdPastExport.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.grdPastExport_DataError);
            this.grdPastExport.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.grdPastExport_EditingControlShowing);
            this.grdPastExport.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.grdPastExport_RowPostPaint);
            this.grdPastExport.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.grdPastExport_RowsAdded);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(486, 113);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(153, 23);
            this.progressBar1.TabIndex = 24;
            this.progressBar1.Visible = false;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // ExportUtility
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(769, 489);
            this.Controls.Add(this.tabControl1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExportUtility";
            this.Text = "Export Utility";
            this.Load += new System.EventHandler(this.ExportUtility_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabConfigExport.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPastExports.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdPastExport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabConfigExport;
        private System.Windows.Forms.TabPage tabPastExports;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cmbServerType;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUserId;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtTargetServer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TreeView tvSource;
        private System.Windows.Forms.TreeView tvDestination;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.DataGridView grdPastExport;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmbExportList;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnOverwrite;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtCasServer;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox chkRetrieveScripts;
        private System.Windows.Forms.ComboBox cmbServerInstance;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbUserList;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cmbFilterCriteria;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox cmbPastExportStatus;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button btnCancelExport;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ErrorProvider errorProvider1;
    }
}
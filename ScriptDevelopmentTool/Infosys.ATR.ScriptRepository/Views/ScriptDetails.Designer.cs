namespace Infosys.ATR.ScriptRepository.Views
{
    partial class ScriptDetails
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScriptDetails));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label2 = new System.Windows.Forms.Label();
            this.txtScriptName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtScriptFile = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtDesc = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.dgParams = new System.Windows.Forms.DataGridView();
            this.Delete = new System.Windows.Forms.DataGridViewImageColumn();
            this.btnAddParam = new System.Windows.Forms.Button();
            this.pnlParameters = new System.Windows.Forms.Panel();
            this.pnlParam = new System.Windows.Forms.Panel();
            this.cmbIsPaired = new System.Windows.Forms.ComboBox();
            this.label22 = new System.Windows.Forms.Label();
            this.cmbIsSecret = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.cmbIOTypes = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.cmbBool = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtAllowedValues = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtDefaultValue = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtParamName = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cmbSubCategory = new System.Windows.Forms.ComboBox();
            this.pnlParamsMain = new System.Windows.Forms.Panel();
            this.pnlSaveButton = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.label15 = new System.Windows.Forms.Label();
            this.txtCommand = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.txtArgs = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.txtWorkingDir = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblTaskType = new System.Windows.Forms.Label();
            this.cmbCategory = new System.Windows.Forms.ComboBox();
            this.label21 = new System.Windows.Forms.Label();
            this.cmbTaskType = new System.Windows.Forms.ComboBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnDownload = new System.Windows.Forms.Button();
            this.btnRunScript = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgParams)).BeginInit();
            this.pnlParameters.SuspendLayout();
            this.pnlParam.SuspendLayout();
            this.pnlParamsMain.SuspendLayout();
            this.pnlSaveButton.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Script";
            // 
            // txtScriptName
            // 
            this.txtScriptName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtScriptName.Location = new System.Drawing.Point(101, 77);
            this.txtScriptName.Name = "txtScriptName";
            this.txtScriptName.Size = new System.Drawing.Size(304, 20);
            this.txtScriptName.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Name";
            // 
            // txtScriptFile
            // 
            this.txtScriptFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtScriptFile.Location = new System.Drawing.Point(101, 215);
            this.txtScriptFile.Name = "txtScriptFile";
            this.txtScriptFile.ReadOnly = true;
            this.txtScriptFile.Size = new System.Drawing.Size(283, 20);
            this.txtScriptFile.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 218);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Upload Script";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.FlatAppearance.BorderSize = 0;
            this.btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowse.Image = ((System.Drawing.Image)(resources.GetObject("btnBrowse.Image")));
            this.btnBrowse.Location = new System.Drawing.Point(386, 213);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(21, 23);
            this.btnBrowse.TabIndex = 12;
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtDesc
            // 
            this.txtDesc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDesc.Location = new System.Drawing.Point(101, 103);
            this.txtDesc.Multiline = true;
            this.txtDesc.Name = "txtDesc";
            this.txtDesc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDesc.Size = new System.Drawing.Size(304, 56);
            this.txtDesc.TabIndex = 14;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 106);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Description";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 55);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "Sub Category";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(6, 346);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(78, 13);
            this.label8.TabIndex = 21;
            this.label8.Text = "Parameter(s)";
            // 
            // dgParams
            // 
            this.dgParams.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgParams.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgParams.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgParams.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgParams.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Delete});
            this.dgParams.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgParams.Location = new System.Drawing.Point(0, 0);
            this.dgParams.MultiSelect = false;
            this.dgParams.Name = "dgParams";
            this.dgParams.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgParams.RowHeadersVisible = false;
            this.dgParams.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgParams.Size = new System.Drawing.Size(405, 120);
            this.dgParams.TabIndex = 22;
            this.dgParams.TabStop = false;
            this.dgParams.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgParams_CellMouseClick);
            this.dgParams.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dgParams_RowPostPaint);
            // 
            // Delete
            // 
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.Color.Red;
            dataGridViewCellStyle9.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle9.NullValue")));
            this.Delete.DefaultCellStyle = dataGridViewCellStyle9;
            this.Delete.FillWeight = 20F;
            this.Delete.HeaderText = "";
            this.Delete.Image = ((System.Drawing.Image)(resources.GetObject("Delete.Image")));
            this.Delete.Name = "Delete";
            this.Delete.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // btnAddParam
            // 
            this.btnAddParam.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddParam.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddParam.ForeColor = System.Drawing.Color.White;
            this.btnAddParam.Image = ((System.Drawing.Image)(resources.GetObject("btnAddParam.Image")));
            this.btnAddParam.Location = new System.Drawing.Point(87, 341);
            this.btnAddParam.Margin = new System.Windows.Forms.Padding(0);
            this.btnAddParam.Name = "btnAddParam";
            this.btnAddParam.Size = new System.Drawing.Size(25, 23);
            this.btnAddParam.TabIndex = 23;
            this.btnAddParam.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnAddParam.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnAddParam.UseVisualStyleBackColor = true;
            this.btnAddParam.Click += new System.EventHandler(this.btnAddParam_Click);
            // 
            // pnlParameters
            // 
            this.pnlParameters.Controls.Add(this.dgParams);
            this.pnlParameters.Controls.Add(this.pnlParam);
            this.pnlParameters.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlParameters.Location = new System.Drawing.Point(0, 0);
            this.pnlParameters.Name = "pnlParameters";
            this.pnlParameters.Size = new System.Drawing.Size(405, 263);
            this.pnlParameters.TabIndex = 25;
            // 
            // pnlParam
            // 
            this.pnlParam.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlParam.Controls.Add(this.cmbIsPaired);
            this.pnlParam.Controls.Add(this.label22);
            this.pnlParam.Controls.Add(this.cmbIsSecret);
            this.pnlParam.Controls.Add(this.label5);
            this.pnlParam.Controls.Add(this.btnCancel);
            this.pnlParam.Controls.Add(this.label14);
            this.pnlParam.Controls.Add(this.btnAdd);
            this.pnlParam.Controls.Add(this.cmbIOTypes);
            this.pnlParam.Controls.Add(this.label11);
            this.pnlParam.Controls.Add(this.cmbBool);
            this.pnlParam.Controls.Add(this.label12);
            this.pnlParam.Controls.Add(this.txtAllowedValues);
            this.pnlParam.Controls.Add(this.label13);
            this.pnlParam.Controls.Add(this.txtDefaultValue);
            this.pnlParam.Controls.Add(this.label10);
            this.pnlParam.Controls.Add(this.txtParamName);
            this.pnlParam.Controls.Add(this.label9);
            this.pnlParam.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlParam.Location = new System.Drawing.Point(0, 120);
            this.pnlParam.Name = "pnlParam";
            this.pnlParam.Size = new System.Drawing.Size(405, 143);
            this.pnlParam.TabIndex = 24;
            this.pnlParam.Visible = false;
            // 
            // cmbIsPaired
            // 
            this.cmbIsPaired.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIsPaired.FormattingEnabled = true;
            this.cmbIsPaired.Items.AddRange(new object[] {
            "True",
            "False"});
            this.cmbIsPaired.Location = new System.Drawing.Point(76, 89);
            this.cmbIsPaired.Name = "cmbIsPaired";
            this.cmbIsPaired.Size = new System.Drawing.Size(122, 21);
            this.cmbIsPaired.TabIndex = 35;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(-1, 94);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(79, 13);
            this.label22.TabIndex = 34;
            this.label22.Text = "Is Name Paired";
            // 
            // cmbIsSecret
            // 
            this.cmbIsSecret.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbIsSecret.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIsSecret.FormattingEnabled = true;
            this.cmbIsSecret.Items.AddRange(new object[] {
            "False",
            "True"});
            this.cmbIsSecret.Location = new System.Drawing.Point(276, 62);
            this.cmbIsSecret.Name = "cmbIsSecret";
            this.cmbIsSecret.Size = new System.Drawing.Size(122, 21);
            this.cmbIsSecret.TabIndex = 33;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(211, 67);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 13);
            this.label5.TabIndex = 32;
            this.label5.Text = "Is Secret";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(241, 113);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 31;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(210, 16);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(51, 13);
            this.label14.TabIndex = 30;
            this.label14.Text = "Values (,)";
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(322, 113);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 25;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // cmbIOTypes
            // 
            this.cmbIOTypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbIOTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIOTypes.FormattingEnabled = true;
            this.cmbIOTypes.Items.AddRange(new object[] {
            "In",
            "Out"});
            this.cmbIOTypes.Location = new System.Drawing.Point(275, 36);
            this.cmbIOTypes.Name = "cmbIOTypes";
            this.cmbIOTypes.Size = new System.Drawing.Size(122, 21);
            this.cmbIOTypes.TabIndex = 29;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(210, 40);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(49, 13);
            this.label11.TabIndex = 28;
            this.label11.Text = "Direction";
            // 
            // cmbBool
            // 
            this.cmbBool.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBool.FormattingEnabled = true;
            this.cmbBool.Items.AddRange(new object[] {
            "True",
            "False"});
            this.cmbBool.Location = new System.Drawing.Point(76, 63);
            this.cmbBool.Name = "cmbBool";
            this.cmbBool.Size = new System.Drawing.Size(122, 21);
            this.cmbBool.TabIndex = 27;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(1, 68);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(68, 13);
            this.label12.TabIndex = 26;
            this.label12.Text = "Is Mandatory";
            // 
            // txtAllowedValues
            // 
            this.txtAllowedValues.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAllowedValues.Location = new System.Drawing.Point(275, 5);
            this.txtAllowedValues.Name = "txtAllowedValues";
            this.txtAllowedValues.Size = new System.Drawing.Size(122, 20);
            this.txtAllowedValues.TabIndex = 25;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(209, 3);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(44, 13);
            this.label13.TabIndex = 24;
            this.label13.Text = "Allowed";
            // 
            // txtDefaultValue
            // 
            this.txtDefaultValue.Location = new System.Drawing.Point(78, 36);
            this.txtDefaultValue.Name = "txtDefaultValue";
            this.txtDefaultValue.Size = new System.Drawing.Size(120, 20);
            this.txtDefaultValue.TabIndex = 9;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 39);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(71, 13);
            this.label10.TabIndex = 8;
            this.label10.Text = "Default Value";
            // 
            // txtParamName
            // 
            this.txtParamName.Location = new System.Drawing.Point(78, 7);
            this.txtParamName.Name = "txtParamName";
            this.txtParamName.Size = new System.Drawing.Size(120, 20);
            this.txtParamName.TabIndex = 7;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 9);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(35, 13);
            this.label9.TabIndex = 6;
            this.label9.Text = "Name";
            // 
            // cmbSubCategory
            // 
            this.cmbSubCategory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSubCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSubCategory.FormattingEnabled = true;
            this.cmbSubCategory.Location = new System.Drawing.Point(101, 52);
            this.cmbSubCategory.Name = "cmbSubCategory";
            this.cmbSubCategory.Size = new System.Drawing.Size(303, 21);
            this.cmbSubCategory.TabIndex = 27;
            // 
            // pnlParamsMain
            // 
            this.pnlParamsMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlParamsMain.Controls.Add(this.pnlSaveButton);
            this.pnlParamsMain.Controls.Add(this.pnlParameters);
            this.pnlParamsMain.Location = new System.Drawing.Point(7, 364);
            this.pnlParamsMain.Name = "pnlParamsMain";
            this.pnlParamsMain.Size = new System.Drawing.Size(405, 307);
            this.pnlParamsMain.TabIndex = 28;
            // 
            // pnlSaveButton
            // 
            this.pnlSaveButton.Controls.Add(this.btnRunScript);
            this.pnlSaveButton.Controls.Add(this.btnSave);
            this.pnlSaveButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSaveButton.Location = new System.Drawing.Point(0, 263);
            this.pnlSaveButton.Name = "pnlSaveButton";
            this.pnlSaveButton.Size = new System.Drawing.Size(405, 41);
            this.pnlSaveButton.TabIndex = 26;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(323, 11);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(14, 167);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(58, 13);
            this.label15.TabIndex = 29;
            this.label15.Text = "Task Type";
            // 
            // txtCommand
            // 
            this.txtCommand.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCommand.Location = new System.Drawing.Point(101, 240);
            this.txtCommand.Name = "txtCommand";
            this.txtCommand.Size = new System.Drawing.Size(305, 20);
            this.txtCommand.TabIndex = 32;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(16, 242);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(54, 13);
            this.label16.TabIndex = 31;
            this.label16.Text = "Command";
            // 
            // txtArgs
            // 
            this.txtArgs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtArgs.Location = new System.Drawing.Point(101, 270);
            this.txtArgs.Name = "txtArgs";
            this.txtArgs.Size = new System.Drawing.Size(305, 20);
            this.txtArgs.TabIndex = 34;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(16, 266);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(54, 13);
            this.label17.TabIndex = 33;
            this.label17.Text = "Command";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(15, 279);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(82, 13);
            this.label18.TabIndex = 35;
            this.label18.Text = "Argument String";
            // 
            // txtWorkingDir
            // 
            this.txtWorkingDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWorkingDir.Location = new System.Drawing.Point(101, 300);
            this.txtWorkingDir.Name = "txtWorkingDir";
            this.txtWorkingDir.Size = new System.Drawing.Size(305, 20);
            this.txtWorkingDir.TabIndex = 37;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(15, 296);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(47, 13);
            this.label19.TabIndex = 36;
            this.label19.Text = "Working";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(16, 309);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(49, 13);
            this.label20.TabIndex = 38;
            this.label20.Text = "Directory";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 193);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(85, 13);
            this.label7.TabIndex = 39;
            this.label7.Text = "Download Script";
            // 
            // lblTaskType
            // 
            this.lblTaskType.AutoSize = true;
            this.lblTaskType.Location = new System.Drawing.Point(100, 167);
            this.lblTaskType.Name = "lblTaskType";
            this.lblTaskType.Size = new System.Drawing.Size(16, 13);
            this.lblTaskType.TabIndex = 42;
            this.lblTaskType.Text = "---";
            // 
            // cmbCategory
            // 
            this.cmbCategory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategory.FormattingEnabled = true;
            this.cmbCategory.Location = new System.Drawing.Point(101, 27);
            this.cmbCategory.Name = "cmbCategory";
            this.cmbCategory.Size = new System.Drawing.Size(303, 21);
            this.cmbCategory.TabIndex = 44;
            this.cmbCategory.SelectedIndexChanged += new System.EventHandler(this.cmbCategory_SelectedIndexChanged);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(14, 30);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(49, 13);
            this.label21.TabIndex = 43;
            this.label21.Text = "Category";
            // 
            // cmbTaskType
            // 
            this.cmbTaskType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbTaskType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTaskType.FormattingEnabled = true;
            this.cmbTaskType.Items.AddRange(new object[] {
            "File",
            "Command"});
            this.cmbTaskType.Location = new System.Drawing.Point(101, 164);
            this.cmbTaskType.Name = "cmbTaskType";
            this.cmbTaskType.Size = new System.Drawing.Size(105, 21);
            this.cmbTaskType.TabIndex = 45;
            this.cmbTaskType.SelectedIndexChanged += new System.EventHandler(this.cmbTaskType_SelectedIndexChanged);
            // 
            // btnDownload
            // 
            this.btnDownload.Enabled = false;
            this.btnDownload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDownload.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDownload.ForeColor = System.Drawing.Color.White;
            this.btnDownload.Image = ((System.Drawing.Image)(resources.GetObject("btnDownload.Image")));
            this.btnDownload.Location = new System.Drawing.Point(98, 188);
            this.btnDownload.Margin = new System.Windows.Forms.Padding(0);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(25, 23);
            this.btnDownload.TabIndex = 46;
            this.btnDownload.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnDownload.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // btnRunScript
            // 
            this.btnRunScript.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRunScript.Location = new System.Drawing.Point(242, 11);
            this.btnRunScript.Name = "btnRunScript";
            this.btnRunScript.Size = new System.Drawing.Size(75, 23);
            this.btnRunScript.TabIndex = 7;
            this.btnRunScript.Text = "Run Script";
            this.btnRunScript.UseVisualStyleBackColor = true;
            this.btnRunScript.Click += new System.EventHandler(this.btnRunScript_Click);
            // 
            // ScriptDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.btnDownload);
            this.Controls.Add(this.cmbTaskType);
            this.Controls.Add(this.cmbCategory);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.lblTaskType);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.txtWorkingDir);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.txtArgs);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.txtCommand);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.pnlParamsMain);
            this.Controls.Add(this.cmbSubCategory);
            this.Controls.Add(this.btnAddParam);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtDesc);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtScriptFile);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtScriptName);
            this.Controls.Add(this.label1);
            this.Name = "ScriptDetails";
            this.Size = new System.Drawing.Size(419, 684);
            ((System.ComponentModel.ISupportInitialize)(this.dgParams)).EndInit();
            this.pnlParameters.ResumeLayout(false);
            this.pnlParam.ResumeLayout(false);
            this.pnlParam.PerformLayout();
            this.pnlParamsMain.ResumeLayout(false);
            this.pnlSaveButton.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtScriptName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtScriptFile;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtDesc;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DataGridView dgParams;
        private System.Windows.Forms.Button btnAddParam;
        private System.Windows.Forms.Panel pnlParameters;
        private System.Windows.Forms.Panel pnlParam;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ComboBox cmbIOTypes;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cmbBool;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtAllowedValues;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtDefaultValue;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtParamName;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.DataGridViewImageColumn Delete;
        private System.Windows.Forms.ComboBox cmbSubCategory;
        private System.Windows.Forms.Panel pnlParamsMain;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtCommand;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txtArgs;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox txtWorkingDir;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblTaskType;
        private System.Windows.Forms.ComboBox cmbCategory;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Panel pnlSaveButton;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ComboBox cmbTaskType;
        private System.Windows.Forms.ComboBox cmbIsSecret;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.ComboBox cmbIsPaired;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Button btnRunScript;
    }
}

namespace Infosys.ATR.WFDesigner.Views
{
    partial class WFDetails
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WFDetails));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbCategory = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbSubCategory = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnDownload = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRunWF = new System.Windows.Forms.Button();
            this.btnInfo = new System.Windows.Forms.Button();
            this.btnAddParam = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtTags = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.pnlLower = new System.Windows.Forms.Panel();
            this.pnlParameters = new System.Windows.Forms.Panel();
            this.pnlNewParam = new System.Windows.Forms.Panel();
            this.cmbIsSecret = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
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
            this.pnlAddedParam = new System.Windows.Forms.Panel();
            this.dgParams = new System.Windows.Forms.DataGridView();
            this.Delete = new System.Windows.Forms.DataGridViewImageColumn();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.pnlSave = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label15 = new System.Windows.Forms.Label();
            this.chkIsPersistWF = new System.Windows.Forms.CheckBox();
            this.txtIdleTimeOut = new System.Windows.Forms.TextBox();
            this.pnlPersistWF = new System.Windows.Forms.Panel();
            this.lblTags = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.cmbIsReference = new System.Windows.Forms.ComboBox();
            this.pnlLower.SuspendLayout();
            this.pnlParameters.SuspendLayout();
            this.pnlNewParam.SuspendLayout();
            this.pnlAddedParam.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgParams)).BeginInit();
            this.pnlSave.SuspendLayout();
            this.panel1.SuspendLayout();
            this.pnlPersistWF.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Workflow Details";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Category";
            // 
            // cmbCategory
            // 
            this.cmbCategory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategory.FormattingEnabled = true;
            this.cmbCategory.Location = new System.Drawing.Point(137, 48);
            this.cmbCategory.Name = "cmbCategory";
            this.cmbCategory.Size = new System.Drawing.Size(172, 21);
            this.cmbCategory.TabIndex = 2;
            this.cmbCategory.SelectedIndexChanged += new System.EventHandler(this.cmbCategory_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Sub Categories";
            // 
            // cmbSubCategory
            // 
            this.cmbSubCategory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSubCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSubCategory.FormattingEnabled = true;
            this.cmbSubCategory.Location = new System.Drawing.Point(137, 73);
            this.cmbSubCategory.Name = "cmbSubCategory";
            this.cmbSubCategory.Size = new System.Drawing.Size(172, 21);
            this.cmbSubCategory.TabIndex = 4;
            this.cmbSubCategory.SelectedIndexChanged += new System.EventHandler(this.cmbSubCategory_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 97);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Name";
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Location = new System.Drawing.Point(137, 98);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(172, 20);
            this.txtName.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 129);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Description";
            // 
            // txtDescription
            // 
            this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescription.Location = new System.Drawing.Point(137, 121);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDescription.Size = new System.Drawing.Size(172, 51);
            this.txtDescription.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(20, 250);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(103, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Download Workflow";
            // 
            // btnDownload
            // 
            this.btnDownload.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDownload.Enabled = false;
            this.btnDownload.FlatAppearance.BorderSize = 0;
            this.btnDownload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDownload.Image = ((System.Drawing.Image)(resources.GetObject("btnDownload.Image")));
            this.btnDownload.Location = new System.Drawing.Point(131, 246);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(32, 23);
            this.btnDownload.TabIndex = 20;
            this.btnDownload.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolTip1.SetToolTip(this.btnDownload, "Click here to download the workflow package");
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(222, 122);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 39;
            this.btnAdd.Text = "Add";
            this.toolTip1.SetToolTip(this.btnAdd, "Click here to add new Parameter");
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRunWF
            // 
            this.btnRunWF.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRunWF.Location = new System.Drawing.Point(150, 17);
            this.btnRunWF.Name = "btnRunWF";
            this.btnRunWF.Size = new System.Drawing.Size(75, 23);
            this.btnRunWF.TabIndex = 12;
            this.btnRunWF.Text = "Run WF";
            this.toolTip1.SetToolTip(this.btnRunWF, "Click here to run workflow locally");
            this.btnRunWF.UseVisualStyleBackColor = true;
            this.btnRunWF.Click += new System.EventHandler(this.btnRunWF_Click);
            // 
            // btnInfo
            // 
            this.btnInfo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnInfo.FlatAppearance.BorderSize = 0;
            this.btnInfo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInfo.Image = ((System.Drawing.Image)(resources.GetObject("btnInfo.Image")));
            this.btnInfo.Location = new System.Drawing.Point(109, 0);
            this.btnInfo.Name = "btnInfo";
            this.btnInfo.Size = new System.Drawing.Size(21, 23);
            this.btnInfo.TabIndex = 13;
            this.toolTip1.SetToolTip(this.btnInfo, "Click here for Workflow Info");
            this.btnInfo.UseVisualStyleBackColor = true;
            this.btnInfo.Click += new System.EventHandler(this.btnInfo_Click);
            // 
            // btnAddParam
            // 
            this.btnAddParam.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddParam.FlatAppearance.BorderSize = 0;
            this.btnAddParam.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddParam.Image = ((System.Drawing.Image)(resources.GetObject("btnAddParam.Image")));
            this.btnAddParam.Location = new System.Drawing.Point(82, 266);
            this.btnAddParam.Name = "btnAddParam";
            this.btnAddParam.Size = new System.Drawing.Size(36, 23);
            this.btnAddParam.TabIndex = 22;
            this.toolTip1.SetToolTip(this.btnAddParam, "Click here to add new Parameter");
            this.btnAddParam.UseVisualStyleBackColor = true;
            this.btnAddParam.Click += new System.EventHandler(this.btnAddParam_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(131, 122);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 46;
            this.btnCancel.Text = "Cancel";
            this.toolTip1.SetToolTip(this.btnCancel, "Click here to cancel parameter");
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtTags
            // 
            this.txtTags.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTags.Location = new System.Drawing.Point(137, 176);
            this.txtTags.Name = "txtTags";
            this.txtTags.Size = new System.Drawing.Size(172, 20);
            this.txtTags.TabIndex = 67;
            this.toolTip1.SetToolTip(this.txtTags, "Enter Tags");
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(3, 270);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(78, 13);
            this.label7.TabIndex = 21;
            this.label7.Text = "Parameter(s)";
            // 
            // pnlLower
            // 
            this.pnlLower.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlLower.Controls.Add(this.pnlParameters);
            this.pnlLower.Location = new System.Drawing.Point(6, 298);
            this.pnlLower.Name = "pnlLower";
            this.pnlLower.Size = new System.Drawing.Size(303, 291);
            this.pnlLower.TabIndex = 23;
            // 
            // pnlParameters
            // 
            this.pnlParameters.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlParameters.Controls.Add(this.pnlNewParam);
            this.pnlParameters.Controls.Add(this.pnlAddedParam);
            this.pnlParameters.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlParameters.Location = new System.Drawing.Point(0, 0);
            this.pnlParameters.Name = "pnlParameters";
            this.pnlParameters.Size = new System.Drawing.Size(303, 290);
            this.pnlParameters.TabIndex = 1;
            this.pnlParameters.Visible = false;
            // 
            // pnlNewParam
            // 
            this.pnlNewParam.Controls.Add(this.cmbIsReference);
            this.pnlNewParam.Controls.Add(this.label17);
            this.pnlNewParam.Controls.Add(this.label19);
            this.pnlNewParam.Controls.Add(this.cmbIsSecret);
            this.pnlNewParam.Controls.Add(this.label8);
            this.pnlNewParam.Controls.Add(this.btnCancel);
            this.pnlNewParam.Controls.Add(this.label14);
            this.pnlNewParam.Controls.Add(this.btnAdd);
            this.pnlNewParam.Controls.Add(this.cmbIOTypes);
            this.pnlNewParam.Controls.Add(this.label11);
            this.pnlNewParam.Controls.Add(this.cmbBool);
            this.pnlNewParam.Controls.Add(this.label12);
            this.pnlNewParam.Controls.Add(this.txtAllowedValues);
            this.pnlNewParam.Controls.Add(this.label13);
            this.pnlNewParam.Controls.Add(this.txtDefaultValue);
            this.pnlNewParam.Controls.Add(this.label10);
            this.pnlNewParam.Controls.Add(this.txtParamName);
            this.pnlNewParam.Controls.Add(this.label9);
            this.pnlNewParam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlNewParam.Location = new System.Drawing.Point(0, 132);
            this.pnlNewParam.Name = "pnlNewParam";
            this.pnlNewParam.Size = new System.Drawing.Size(301, 156);
            this.pnlNewParam.TabIndex = 1;
            this.pnlNewParam.Visible = false;
            // 
            // cmbIsSecret
            // 
            this.cmbIsSecret.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbIsSecret.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIsSecret.FormattingEnabled = true;
            this.cmbIsSecret.Items.AddRange(new object[] {
            "False",
            "True"});
            this.cmbIsSecret.Location = new System.Drawing.Point(222, 64);
            this.cmbIsSecret.Name = "cmbIsSecret";
            this.cmbIsSecret.Size = new System.Drawing.Size(76, 21);
            this.cmbIsSecret.TabIndex = 48;
            this.cmbIsSecret.SelectedIndexChanged += new System.EventHandler(this.cmbIsSecret_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(166, 69);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(49, 13);
            this.label8.TabIndex = 47;
            this.label8.Text = "Is Secret";
            // 
            // label14
            // 
            this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(165, 18);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(51, 13);
            this.label14.TabIndex = 45;
            this.label14.Text = "Values (,)";
            // 
            // cmbIOTypes
            // 
            this.cmbIOTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbIOTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIOTypes.FormattingEnabled = true;
            this.cmbIOTypes.Items.AddRange(new object[] {
            "In",
            "Out"});
            this.cmbIOTypes.Location = new System.Drawing.Point(222, 38);
            this.cmbIOTypes.Name = "cmbIOTypes";
            this.cmbIOTypes.Size = new System.Drawing.Size(75, 21);
            this.cmbIOTypes.TabIndex = 44;
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(165, 42);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(49, 13);
            this.label11.TabIndex = 43;
            this.label11.Text = "Direction";
            // 
            // cmbBool
            // 
            this.cmbBool.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbBool.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBool.FormattingEnabled = true;
            this.cmbBool.Items.AddRange(new object[] {
            "True",
            "False"});
            this.cmbBool.Location = new System.Drawing.Point(74, 65);
            this.cmbBool.Name = "cmbBool";
            this.cmbBool.Size = new System.Drawing.Size(84, 21);
            this.cmbBool.TabIndex = 42;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(1, 70);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(74, 13);
            this.label12.TabIndex = 41;
            this.label12.Text = "Is Mandatory?";
            // 
            // txtAllowedValues
            // 
            this.txtAllowedValues.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAllowedValues.Location = new System.Drawing.Point(222, 7);
            this.txtAllowedValues.Name = "txtAllowedValues";
            this.txtAllowedValues.Size = new System.Drawing.Size(75, 20);
            this.txtAllowedValues.TabIndex = 40;
            // 
            // label13
            // 
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(164, 5);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(44, 13);
            this.label13.TabIndex = 38;
            this.label13.Text = "Allowed";
            // 
            // txtDefaultValue
            // 
            this.txtDefaultValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDefaultValue.Location = new System.Drawing.Point(76, 38);
            this.txtDefaultValue.Name = "txtDefaultValue";
            this.txtDefaultValue.Size = new System.Drawing.Size(82, 20);
            this.txtDefaultValue.TabIndex = 37;
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(1, 41);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(71, 13);
            this.label10.TabIndex = 36;
            this.label10.Text = "Default Value";
            // 
            // txtParamName
            // 
            this.txtParamName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtParamName.Location = new System.Drawing.Point(76, 9);
            this.txtParamName.Name = "txtParamName";
            this.txtParamName.Size = new System.Drawing.Size(82, 20);
            this.txtParamName.TabIndex = 35;
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(1, 11);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(35, 13);
            this.label9.TabIndex = 34;
            this.label9.Text = "Name";
            // 
            // pnlAddedParam
            // 
            this.pnlAddedParam.Controls.Add(this.dgParams);
            this.pnlAddedParam.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlAddedParam.Location = new System.Drawing.Point(0, 0);
            this.pnlAddedParam.Name = "pnlAddedParam";
            this.pnlAddedParam.Size = new System.Drawing.Size(301, 132);
            this.pnlAddedParam.TabIndex = 0;
            this.pnlAddedParam.Visible = false;
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
            this.dgParams.Name = "dgParams";
            this.dgParams.ReadOnly = true;
            this.dgParams.RowHeadersVisible = false;
            this.dgParams.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgParams.Size = new System.Drawing.Size(301, 132);
            this.dgParams.TabIndex = 0;
            this.dgParams.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgParams_CellMouseClick);
            // 
            // Delete
            // 
            this.Delete.FillWeight = 20F;
            this.Delete.HeaderText = "";
            this.Delete.Image = ((System.Drawing.Image)(resources.GetObject("Delete.Image")));
            this.Delete.Name = "Delete";
            this.Delete.ReadOnly = true;
            this.Delete.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // checkBox1
            // 
            this.checkBox1.Location = new System.Drawing.Point(23, 229);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(281, 16);
            this.checkBox1.TabIndex = 24;
            this.checkBox1.Text = "Uses UI Automation";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // pnlSave
            // 
            this.pnlSave.Controls.Add(this.btnSave);
            this.pnlSave.Controls.Add(this.panel1);
            this.pnlSave.Controls.Add(this.btnRunWF);
            this.pnlSave.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSave.Location = new System.Drawing.Point(0, 0);
            this.pnlSave.Name = "pnlSave";
            this.pnlSave.Size = new System.Drawing.Size(320, 43);
            this.pnlSave.TabIndex = 0;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSave.Location = new System.Drawing.Point(240, 17);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(69, 23);
            this.btnSave.TabIndex = 15;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click_1);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnInfo);
            this.panel1.Location = new System.Drawing.Point(2, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(133, 24);
            this.panel1.TabIndex = 14;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(20, 3);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(70, 13);
            this.label15.TabIndex = 25;
            this.label15.Text = "Idle Time Out";
            // 
            // chkIsPersistWF
            // 
            this.chkIsPersistWF.Location = new System.Drawing.Point(226, 1);
            this.chkIsPersistWF.Name = "chkIsPersistWF";
            this.chkIsPersistWF.Size = new System.Drawing.Size(80, 24);
            this.chkIsPersistWF.TabIndex = 26;
            this.chkIsPersistWF.Text = "Persist WF";
            this.chkIsPersistWF.UseVisualStyleBackColor = true;
            // 
            // txtIdleTimeOut
            // 
            this.txtIdleTimeOut.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtIdleTimeOut.Location = new System.Drawing.Point(137, 3);
            this.txtIdleTimeOut.Name = "txtIdleTimeOut";
            this.txtIdleTimeOut.Size = new System.Drawing.Size(52, 20);
            this.txtIdleTimeOut.TabIndex = 27;
            this.txtIdleTimeOut.TextChanged += new System.EventHandler(this.txtIdleTimeOut_TextChanged);
            // 
            // pnlPersistWF
            // 
            this.pnlPersistWF.Controls.Add(this.txtIdleTimeOut);
            this.pnlPersistWF.Controls.Add(this.label15);
            this.pnlPersistWF.Controls.Add(this.chkIsPersistWF);
            this.pnlPersistWF.Location = new System.Drawing.Point(0, 199);
            this.pnlPersistWF.Name = "pnlPersistWF";
            this.pnlPersistWF.Size = new System.Drawing.Size(320, 26);
            this.pnlPersistWF.TabIndex = 28;
            // 
            // lblTags
            // 
            this.lblTags.AutoSize = true;
            this.lblTags.Location = new System.Drawing.Point(20, 180);
            this.lblTags.Name = "lblTags";
            this.lblTags.Size = new System.Drawing.Size(70, 19);
            this.lblTags.TabIndex = 68;
            this.lblTags.Text = "Tags";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(2, 104);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(33, 13);
            this.label17.TabIndex = 51;
            this.label17.Text = "key? ";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(1, 92);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(68, 13);
            this.label19.TabIndex = 49;
            this.label19.Text = "Is Reference";
            // 
            // cmbIsReference
            // 
            this.cmbIsReference.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbIsReference.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIsReference.FormattingEnabled = true;
            this.cmbIsReference.Items.AddRange(new object[] {
            "True",
            "False"});
            this.cmbIsReference.Location = new System.Drawing.Point(74, 92);
            this.cmbIsReference.Name = "cmbIsReference";
            this.cmbIsReference.Size = new System.Drawing.Size(84, 21);
            this.cmbIsReference.TabIndex = 52;
            // 
            // WFDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.lblTags);
            this.Controls.Add(this.pnlPersistWF);
            this.Controls.Add(this.txtTags);
            this.Controls.Add(this.pnlSave);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.pnlLower);
            this.Controls.Add(this.btnAddParam);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnDownload);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbSubCategory);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbCategory);
            this.Controls.Add(this.label2);
            this.Name = "WFDetails";
            this.Size = new System.Drawing.Size(320, 591);
            this.pnlLower.ResumeLayout(false);
            this.pnlParameters.ResumeLayout(false);
            this.pnlNewParam.ResumeLayout(false);
            this.pnlNewParam.PerformLayout();
            this.pnlAddedParam.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgParams)).EndInit();
            this.pnlSave.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.pnlPersistWF.ResumeLayout(false);
            this.pnlPersistWF.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbCategory;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbSubCategory;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnAddParam;
        private System.Windows.Forms.Panel pnlLower;
        private System.Windows.Forms.Panel pnlParameters;
        private System.Windows.Forms.Panel pnlNewParam;
        private System.Windows.Forms.Panel pnlAddedParam;
        private System.Windows.Forms.DataGridView dgParams;
        private System.Windows.Forms.ComboBox cmbIsSecret;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnCancel;
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
        private System.Windows.Forms.DataGridViewImageColumn Delete;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Panel pnlSave;
        private System.Windows.Forms.Button btnRunWF;
        private System.Windows.Forms.Button btnInfo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.CheckBox chkIsPersistWF;
        private System.Windows.Forms.TextBox txtIdleTimeOut;
        private System.Windows.Forms.Panel pnlPersistWF;
        private System.Windows.Forms.Label lblTags;
        private System.Windows.Forms.TextBox txtTags;
        private System.Windows.Forms.ComboBox cmbIsReference;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label19;
    }
}

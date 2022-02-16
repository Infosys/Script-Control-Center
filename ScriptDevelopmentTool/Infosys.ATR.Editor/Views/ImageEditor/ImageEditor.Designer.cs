namespace Infosys.ATR.Editor.Views.ImageEditor
{
    partial class ImageEditor
    {
        /// <summary>
        /// The presenter used by this view.
        /// </summary>
        private Infosys.ATR.Editor.Views.ImageEditorPresenter _presenter = null;

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
            this.pnlApplication = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnAddApp = new System.Windows.Forms.Button();
            this.txtVersion = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbBrowser = new System.Windows.Forms.ComboBox();
            this.cmbAppType = new System.Windows.Forms.ComboBox();
            this.btnAppPathBrowser = new System.Windows.Forms.Button();
            this.txtAppPath = new System.Windows.Forms.TextBox();
            this.lblApplicationPath = new System.Windows.Forms.Label();
            this.txtAppName = new System.Windows.Forms.TextBox();
            this.lblAppName = new System.Windows.Forms.Label();
            this.pnlImage = new System.Windows.Forms.Panel();
            this.panelAreaMasterM = new System.Windows.Forms.Panel();
            this.pnlAreaMaster = new System.Windows.Forms.Panel();
            this.pnlArea = new System.Windows.Forms.Panel();
            this.grpArea = new System.Windows.Forms.GroupBox();
            this.btnValidate = new System.Windows.Forms.Button();
            this.txtValidate = new System.Windows.Forms.TextBox();
            this.lblValidate = new System.Windows.Forms.Label();
            this.btnBottom = new System.Windows.Forms.Button();
            this.txtBottom = new System.Windows.Forms.TextBox();
            this.lblBottom = new System.Windows.Forms.Label();
            this.btnTop = new System.Windows.Forms.Button();
            this.txtTop = new System.Windows.Forms.TextBox();
            this.lblTop = new System.Windows.Forms.Label();
            this.btnLeft = new System.Windows.Forms.Button();
            this.txtLeft = new System.Windows.Forms.TextBox();
            this.lblLeft = new System.Windows.Forms.Label();
            this.btnRight = new System.Windows.Forms.Button();
            this.txtRight = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnDefault = new System.Windows.Forms.Button();
            this.txtDefault = new System.Windows.Forms.TextBox();
            this.lblDefault = new System.Windows.Forms.Label();
            this.pnlStates = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.pnlState = new System.Windows.Forms.Panel();
            this.lblState = new System.Windows.Forms.Label();
            this.cmbState = new System.Windows.Forms.ComboBox();
            this.btnState = new System.Windows.Forms.Button();
            this.pnlAppName = new System.Windows.Forms.Panel();
            this.btnAddStateName = new System.Windows.Forms.Button();
            this.txtstateName = new System.Windows.Forms.TextBox();
            this.lblStateName = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.pnlApplication.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.pnlImage.SuspendLayout();
            this.panelAreaMasterM.SuspendLayout();
            this.pnlAreaMaster.SuspendLayout();
            this.pnlArea.SuspendLayout();
            this.grpArea.SuspendLayout();
            this.pnlStates.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.pnlState.SuspendLayout();
            this.pnlAppName.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlApplication
            // 
            this.pnlApplication.Controls.Add(this.groupBox1);
            this.pnlApplication.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlApplication.Location = new System.Drawing.Point(0, 0);
            this.pnlApplication.Name = "pnlApplication";
            this.pnlApplication.Size = new System.Drawing.Size(315, 195);
            this.pnlApplication.TabIndex = 0;
            this.pnlApplication.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnClear);
            this.groupBox1.Controls.Add(this.btnAddApp);
            this.groupBox1.Controls.Add(this.txtVersion);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.cmbBrowser);
            this.groupBox1.Controls.Add(this.cmbAppType);
            this.groupBox1.Controls.Add(this.btnAppPathBrowser);
            this.groupBox1.Controls.Add(this.txtAppPath);
            this.groupBox1.Controls.Add(this.lblApplicationPath);
            this.groupBox1.Controls.Add(this.txtAppName);
            this.groupBox1.Controls.Add(this.lblAppName);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(315, 195);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Application";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(163, 164);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 73;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            // 
            // btnAddApp
            // 
            this.btnAddApp.Location = new System.Drawing.Point(68, 164);
            this.btnAddApp.Name = "btnAddApp";
            this.btnAddApp.Size = new System.Drawing.Size(75, 23);
            this.btnAddApp.TabIndex = 72;
            this.btnAddApp.Text = "Add";
            this.btnAddApp.UseVisualStyleBackColor = true;
            this.btnAddApp.Click += new System.EventHandler(this.btnAddApp_Click);
            // 
            // txtVersion
            // 
            this.txtVersion.Location = new System.Drawing.Point(68, 136);
            this.txtVersion.Name = "txtVersion";
            this.txtVersion.Size = new System.Drawing.Size(186, 20);
            this.txtVersion.TabIndex = 71;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 139);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(42, 13);
            this.label7.TabIndex = 70;
            this.label7.Text = "Version";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 111);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(45, 13);
            this.label6.TabIndex = 69;
            this.label6.Text = "Browser";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 51);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 13);
            this.label5.TabIndex = 68;
            this.label5.Text = "Type";
            // 
            // cmbBrowser
            // 
            this.cmbBrowser.FormattingEnabled = true;
            this.cmbBrowser.Items.AddRange(new object[] {
            "Internet Explorer",
            "Firefox",
            "Chrome"});
            this.cmbBrowser.Location = new System.Drawing.Point(68, 106);
            this.cmbBrowser.Name = "cmbBrowser";
            this.cmbBrowser.Size = new System.Drawing.Size(186, 21);
            this.cmbBrowser.TabIndex = 67;
            // 
            // cmbAppType
            // 
            this.cmbAppType.FormattingEnabled = true;
            this.cmbAppType.Items.AddRange(new object[] {
            "Web",
            "WinDesktop",
            "Java"});
            this.cmbAppType.Location = new System.Drawing.Point(68, 48);
            this.cmbAppType.Name = "cmbAppType";
            this.cmbAppType.Size = new System.Drawing.Size(186, 21);
            this.cmbAppType.TabIndex = 66;
            this.cmbAppType.SelectedIndexChanged += new System.EventHandler(this.cmbAppType_SelectedIndexChanged);
            // 
            // btnAppPathBrowser
            // 
            this.btnAppPathBrowser.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAppPathBrowser.Location = new System.Drawing.Point(262, 78);
            this.btnAppPathBrowser.Name = "btnAppPathBrowser";
            this.btnAppPathBrowser.Size = new System.Drawing.Size(31, 20);
            this.btnAppPathBrowser.TabIndex = 57;
            this.btnAppPathBrowser.Text = "...";
            this.btnAppPathBrowser.UseVisualStyleBackColor = true;
            this.btnAppPathBrowser.Click += new System.EventHandler(this.btnAppPathBrowser_Click);
            // 
            // txtAppPath
            // 
            this.txtAppPath.Enabled = false;
            this.txtAppPath.Location = new System.Drawing.Point(68, 78);
            this.txtAppPath.Name = "txtAppPath";
            this.txtAppPath.Size = new System.Drawing.Size(186, 20);
            this.txtAppPath.TabIndex = 12;
            // 
            // lblApplicationPath
            // 
            this.lblApplicationPath.AutoSize = true;
            this.lblApplicationPath.Location = new System.Drawing.Point(12, 82);
            this.lblApplicationPath.Name = "lblApplicationPath";
            this.lblApplicationPath.Size = new System.Drawing.Size(29, 13);
            this.lblApplicationPath.TabIndex = 9;
            this.lblApplicationPath.Text = "Path";
            // 
            // txtAppName
            // 
            this.txtAppName.Location = new System.Drawing.Point(68, 22);
            this.txtAppName.Name = "txtAppName";
            this.txtAppName.Size = new System.Drawing.Size(187, 20);
            this.txtAppName.TabIndex = 11;
            // 
            // lblAppName
            // 
            this.lblAppName.AutoSize = true;
            this.lblAppName.Location = new System.Drawing.Point(12, 28);
            this.lblAppName.Name = "lblAppName";
            this.lblAppName.Size = new System.Drawing.Size(35, 13);
            this.lblAppName.TabIndex = 10;
            this.lblAppName.Text = "Name";
            // 
            // pnlImage
            // 
            this.pnlImage.Controls.Add(this.panelAreaMasterM);
            this.pnlImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlImage.Location = new System.Drawing.Point(0, 195);
            this.pnlImage.Name = "pnlImage";
            this.pnlImage.Size = new System.Drawing.Size(315, 605);
            this.pnlImage.TabIndex = 1;
            // 
            // panelAreaMasterM
            // 
            this.panelAreaMasterM.Controls.Add(this.pnlAreaMaster);
            this.panelAreaMasterM.Controls.Add(this.panel2);
            this.panelAreaMasterM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelAreaMasterM.Location = new System.Drawing.Point(0, 0);
            this.panelAreaMasterM.Name = "panelAreaMasterM";
            this.panelAreaMasterM.Size = new System.Drawing.Size(315, 605);
            this.panelAreaMasterM.TabIndex = 2;
            // 
            // pnlAreaMaster
            // 
            this.pnlAreaMaster.Controls.Add(this.pnlArea);
            this.pnlAreaMaster.Controls.Add(this.pnlStates);
            this.pnlAreaMaster.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAreaMaster.Location = new System.Drawing.Point(0, 0);
            this.pnlAreaMaster.Name = "pnlAreaMaster";
            this.pnlAreaMaster.Size = new System.Drawing.Size(315, 465);
            this.pnlAreaMaster.TabIndex = 1;
            // 
            // pnlArea
            // 
            this.pnlArea.Controls.Add(this.grpArea);
            this.pnlArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlArea.Location = new System.Drawing.Point(0, 90);
            this.pnlArea.Name = "pnlArea";
            this.pnlArea.Size = new System.Drawing.Size(315, 375);
            this.pnlArea.TabIndex = 1;
            // 
            // grpArea
            // 
            this.grpArea.Controls.Add(this.btnValidate);
            this.grpArea.Controls.Add(this.txtValidate);
            this.grpArea.Controls.Add(this.lblValidate);
            this.grpArea.Controls.Add(this.btnBottom);
            this.grpArea.Controls.Add(this.txtBottom);
            this.grpArea.Controls.Add(this.lblBottom);
            this.grpArea.Controls.Add(this.btnTop);
            this.grpArea.Controls.Add(this.txtTop);
            this.grpArea.Controls.Add(this.lblTop);
            this.grpArea.Controls.Add(this.btnLeft);
            this.grpArea.Controls.Add(this.txtLeft);
            this.grpArea.Controls.Add(this.lblLeft);
            this.grpArea.Controls.Add(this.btnRight);
            this.grpArea.Controls.Add(this.txtRight);
            this.grpArea.Controls.Add(this.label1);
            this.grpArea.Controls.Add(this.btnDefault);
            this.grpArea.Controls.Add(this.txtDefault);
            this.grpArea.Controls.Add(this.lblDefault);
            this.grpArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpArea.Location = new System.Drawing.Point(0, 0);
            this.grpArea.Name = "grpArea";
            this.grpArea.Size = new System.Drawing.Size(315, 375);
            this.grpArea.TabIndex = 1;
            this.grpArea.TabStop = false;
            this.grpArea.Text = "Area";
            // 
            // btnValidate
            // 
            this.btnValidate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnValidate.Image = global::Infosys.ATR.Editor.Properties.Resources.camera;
            this.btnValidate.Location = new System.Drawing.Point(272, 149);
            this.btnValidate.Name = "btnValidate";
            this.btnValidate.Size = new System.Drawing.Size(31, 23);
            this.btnValidate.TabIndex = 71;
            this.btnValidate.UseVisualStyleBackColor = true;
            this.btnValidate.Click += new System.EventHandler(this.btnValidate_Click);
            // 
            // txtValidate
            // 
            this.txtValidate.Location = new System.Drawing.Point(71, 154);
            this.txtValidate.Name = "txtValidate";
            this.txtValidate.ReadOnly = true;
            this.txtValidate.Size = new System.Drawing.Size(184, 20);
            this.txtValidate.TabIndex = 70;
            this.txtValidate.Tag = "Validate";
            this.txtValidate.Click += new System.EventHandler(this.txtValidate_Click);
            this.txtValidate.TextChanged += new System.EventHandler(this.txtValidate_TextChanged);
            // 
            // lblValidate
            // 
            this.lblValidate.Location = new System.Drawing.Point(12, 153);
            this.lblValidate.Name = "lblValidate";
            this.lblValidate.Size = new System.Drawing.Size(53, 23);
            this.lblValidate.TabIndex = 69;
            this.lblValidate.Text = "Validate";
            // 
            // btnBottom
            // 
            this.btnBottom.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBottom.Image = global::Infosys.ATR.Editor.Properties.Resources.camera;
            this.btnBottom.Location = new System.Drawing.Point(272, 123);
            this.btnBottom.Name = "btnBottom";
            this.btnBottom.Size = new System.Drawing.Size(31, 23);
            this.btnBottom.TabIndex = 68;
            this.btnBottom.UseVisualStyleBackColor = true;
            this.btnBottom.Click += new System.EventHandler(this.btnBottom_Click);
            // 
            // txtBottom
            // 
            this.txtBottom.Location = new System.Drawing.Point(71, 125);
            this.txtBottom.Name = "txtBottom";
            this.txtBottom.ReadOnly = true;
            this.txtBottom.Size = new System.Drawing.Size(184, 20);
            this.txtBottom.TabIndex = 67;
            this.txtBottom.Tag = "Below";
            this.txtBottom.Click += new System.EventHandler(this.txtBottom_Click);
            this.txtBottom.TextChanged += new System.EventHandler(this.txtBottom_TextChanged);
            // 
            // lblBottom
            // 
            this.lblBottom.Location = new System.Drawing.Point(12, 127);
            this.lblBottom.Name = "lblBottom";
            this.lblBottom.Size = new System.Drawing.Size(42, 23);
            this.lblBottom.TabIndex = 66;
            this.lblBottom.Text = "Below";
            // 
            // btnTop
            // 
            this.btnTop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTop.Image = global::Infosys.ATR.Editor.Properties.Resources.camera;
            this.btnTop.Location = new System.Drawing.Point(272, 97);
            this.btnTop.Name = "btnTop";
            this.btnTop.Size = new System.Drawing.Size(31, 23);
            this.btnTop.TabIndex = 65;
            this.btnTop.UseVisualStyleBackColor = true;
            this.btnTop.Click += new System.EventHandler(this.btnTop_Click);
            // 
            // txtTop
            // 
            this.txtTop.Location = new System.Drawing.Point(71, 99);
            this.txtTop.Name = "txtTop";
            this.txtTop.ReadOnly = true;
            this.txtTop.Size = new System.Drawing.Size(184, 20);
            this.txtTop.TabIndex = 64;
            this.txtTop.Tag = "Above";
            this.txtTop.Click += new System.EventHandler(this.txtTop_Click);
            this.txtTop.TextChanged += new System.EventHandler(this.txtTop_TextChanged);
            // 
            // lblTop
            // 
            this.lblTop.Location = new System.Drawing.Point(12, 101);
            this.lblTop.Name = "lblTop";
            this.lblTop.Size = new System.Drawing.Size(42, 23);
            this.lblTop.TabIndex = 63;
            this.lblTop.Text = "Above";
            // 
            // btnLeft
            // 
            this.btnLeft.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLeft.Image = global::Infosys.ATR.Editor.Properties.Resources.camera;
            this.btnLeft.Location = new System.Drawing.Point(272, 71);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(31, 23);
            this.btnLeft.TabIndex = 62;
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // txtLeft
            // 
            this.txtLeft.Location = new System.Drawing.Point(71, 73);
            this.txtLeft.Name = "txtLeft";
            this.txtLeft.ReadOnly = true;
            this.txtLeft.Size = new System.Drawing.Size(184, 20);
            this.txtLeft.TabIndex = 61;
            this.txtLeft.Tag = "Left";
            this.txtLeft.Click += new System.EventHandler(this.txtLeft_Click);
            this.txtLeft.TextChanged += new System.EventHandler(this.txtLeft_TextChanged);
            // 
            // lblLeft
            // 
            this.lblLeft.Location = new System.Drawing.Point(12, 75);
            this.lblLeft.Name = "lblLeft";
            this.lblLeft.Size = new System.Drawing.Size(42, 23);
            this.lblLeft.TabIndex = 60;
            this.lblLeft.Text = "Left";
            // 
            // btnRight
            // 
            this.btnRight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRight.Image = global::Infosys.ATR.Editor.Properties.Resources.camera;
            this.btnRight.Location = new System.Drawing.Point(272, 45);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(31, 23);
            this.btnRight.TabIndex = 59;
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // txtRight
            // 
            this.txtRight.Location = new System.Drawing.Point(71, 47);
            this.txtRight.Name = "txtRight";
            this.txtRight.ReadOnly = true;
            this.txtRight.Size = new System.Drawing.Size(184, 20);
            this.txtRight.TabIndex = 58;
            this.txtRight.Tag = "Right";
            this.txtRight.Click += new System.EventHandler(this.txtRight_Click);
            this.txtRight.TextChanged += new System.EventHandler(this.txtRight_TextChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 23);
            this.label1.TabIndex = 57;
            this.label1.Text = "Right";
            // 
            // btnDefault
            // 
            this.btnDefault.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDefault.Image = global::Infosys.ATR.Editor.Properties.Resources.camera;
            this.btnDefault.Location = new System.Drawing.Point(272, 19);
            this.btnDefault.Name = "btnDefault";
            this.btnDefault.Size = new System.Drawing.Size(31, 23);
            this.btnDefault.TabIndex = 56;
            this.btnDefault.UseVisualStyleBackColor = true;
            this.btnDefault.Click += new System.EventHandler(this.btnDefault_Click);
            // 
            // txtDefault
            // 
            this.txtDefault.Location = new System.Drawing.Point(71, 21);
            this.txtDefault.Name = "txtDefault";
            this.txtDefault.ReadOnly = true;
            this.txtDefault.Size = new System.Drawing.Size(184, 20);
            this.txtDefault.TabIndex = 55;
            this.txtDefault.Tag = "Center";
            this.txtDefault.Click += new System.EventHandler(this.txtDefault_Click);
            this.txtDefault.TextChanged += new System.EventHandler(this.txtDefault_TextChanged);
            // 
            // lblDefault
            // 
            this.lblDefault.Location = new System.Drawing.Point(12, 23);
            this.lblDefault.Name = "lblDefault";
            this.lblDefault.Size = new System.Drawing.Size(42, 23);
            this.lblDefault.TabIndex = 54;
            this.lblDefault.Text = "Center";
            // 
            // pnlStates
            // 
            this.pnlStates.Controls.Add(this.groupBox3);
            this.pnlStates.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlStates.Location = new System.Drawing.Point(0, 0);
            this.pnlStates.Name = "pnlStates";
            this.pnlStates.Size = new System.Drawing.Size(315, 90);
            this.pnlStates.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.pnlState);
            this.groupBox3.Controls.Add(this.pnlAppName);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(315, 90);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "State";
            // 
            // pnlState
            // 
            this.pnlState.Controls.Add(this.lblState);
            this.pnlState.Controls.Add(this.cmbState);
            this.pnlState.Controls.Add(this.btnState);
            this.pnlState.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlState.Location = new System.Drawing.Point(3, 49);
            this.pnlState.Name = "pnlState";
            this.pnlState.Size = new System.Drawing.Size(309, 38);
            this.pnlState.TabIndex = 6;
            // 
            // lblState
            // 
            this.lblState.AutoSize = true;
            this.lblState.Location = new System.Drawing.Point(9, 7);
            this.lblState.Name = "lblState";
            this.lblState.Size = new System.Drawing.Size(32, 13);
            this.lblState.TabIndex = 7;
            this.lblState.Text = "State";
            // 
            // cmbState
            // 
            this.cmbState.FormattingEnabled = true;
            this.cmbState.Location = new System.Drawing.Point(65, 5);
            this.cmbState.Name = "cmbState";
            this.cmbState.Size = new System.Drawing.Size(153, 21);
            this.cmbState.TabIndex = 6;
            this.cmbState.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // btnState
            // 
            this.btnState.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnState.Location = new System.Drawing.Point(224, 3);
            this.btnState.Name = "btnState";
            this.btnState.Size = new System.Drawing.Size(75, 23);
            this.btnState.TabIndex = 5;
            this.btnState.Text = "Add";
            this.btnState.UseVisualStyleBackColor = true;
            this.btnState.Click += new System.EventHandler(this.button1_Click);
            // 
            // pnlAppName
            // 
            this.pnlAppName.Controls.Add(this.btnAddStateName);
            this.pnlAppName.Controls.Add(this.txtstateName);
            this.pnlAppName.Controls.Add(this.lblStateName);
            this.pnlAppName.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlAppName.Location = new System.Drawing.Point(3, 16);
            this.pnlAppName.Name = "pnlAppName";
            this.pnlAppName.Size = new System.Drawing.Size(309, 33);
            this.pnlAppName.TabIndex = 5;
            // 
            // btnAddStateName
            // 
            this.btnAddStateName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddStateName.Location = new System.Drawing.Point(224, 5);
            this.btnAddStateName.Name = "btnAddStateName";
            this.btnAddStateName.Size = new System.Drawing.Size(75, 23);
            this.btnAddStateName.TabIndex = 10;
            this.btnAddStateName.Text = "Add";
            this.btnAddStateName.UseVisualStyleBackColor = true;
            this.btnAddStateName.Click += new System.EventHandler(this.button2_Click);
            // 
            // txtstateName
            // 
            this.txtstateName.Location = new System.Drawing.Point(65, 7);
            this.txtstateName.Name = "txtstateName";
            this.txtstateName.Size = new System.Drawing.Size(153, 20);
            this.txtstateName.TabIndex = 9;
            // 
            // lblStateName
            // 
            this.lblStateName.AutoSize = true;
            this.lblStateName.Location = new System.Drawing.Point(9, 13);
            this.lblStateName.Name = "lblStateName";
            this.lblStateName.Size = new System.Drawing.Size(35, 13);
            this.lblStateName.TabIndex = 8;
            this.lblStateName.Text = "Name";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel5);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 465);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(315, 140);
            this.panel2.TabIndex = 0;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.pictureBox1);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 27);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(315, 113);
            this.panel5.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(315, 113);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.label2);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(315, 27);
            this.panel4.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 13);
            this.label2.TabIndex = 0;
            // 
            // ImageEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.pnlImage);
            this.Controls.Add(this.pnlApplication);
            this.Name = "ImageEditor";
            this.Size = new System.Drawing.Size(315, 800);
            this.pnlApplication.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.pnlImage.ResumeLayout(false);
            this.panelAreaMasterM.ResumeLayout(false);
            this.pnlAreaMaster.ResumeLayout(false);
            this.pnlArea.ResumeLayout(false);
            this.grpArea.ResumeLayout(false);
            this.grpArea.PerformLayout();
            this.pnlStates.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.pnlState.ResumeLayout(false);
            this.pnlState.PerformLayout();
            this.pnlAppName.ResumeLayout(false);
            this.pnlAppName.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlApplication;
        private System.Windows.Forms.Panel pnlImage;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtAppPath;
        private System.Windows.Forms.Label lblApplicationPath;
        private System.Windows.Forms.TextBox txtAppName;
        private System.Windows.Forms.Label lblAppName;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button btnAppPathBrowser;
        private System.Windows.Forms.Panel panelAreaMasterM;
        private System.Windows.Forms.Panel pnlAreaMaster;
        private System.Windows.Forms.Panel pnlArea;
        private System.Windows.Forms.GroupBox grpArea;
        private System.Windows.Forms.Button btnValidate;
        private System.Windows.Forms.TextBox txtValidate;
        private System.Windows.Forms.Label lblValidate;
        private System.Windows.Forms.Button btnBottom;
        private System.Windows.Forms.TextBox txtBottom;
        private System.Windows.Forms.Label lblBottom;
        private System.Windows.Forms.Button btnTop;
        private System.Windows.Forms.TextBox txtTop;
        private System.Windows.Forms.Label lblTop;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.TextBox txtLeft;
        private System.Windows.Forms.Label lblLeft;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.TextBox txtRight;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnDefault;
        private System.Windows.Forms.TextBox txtDefault;
        private System.Windows.Forms.Label lblDefault;
        private System.Windows.Forms.Panel pnlStates;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Panel pnlState;
        private System.Windows.Forms.Label lblState;
        private System.Windows.Forms.ComboBox cmbState;
        private System.Windows.Forms.Button btnState;
        private System.Windows.Forms.Panel pnlAppName;
        private System.Windows.Forms.Button btnAddStateName;
        private System.Windows.Forms.TextBox txtstateName;
        private System.Windows.Forms.Label lblStateName;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnAddApp;
        private System.Windows.Forms.TextBox txtVersion;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbBrowser;
        private System.Windows.Forms.ComboBox cmbAppType;



    }
}

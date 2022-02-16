namespace Infosys.ATR.WFDesigner.Views
{
    partial class TransactionView
    {

        /// <summary>
        /// The presenter used by this view.
        /// </summary>
        private Infosys.ATR.WFDesigner.Views.TransactionPresenter _presenter = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TransactionView));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.grbTransactionFilters = new System.Windows.Forms.GroupBox();
            this.grblstNodes = new System.Windows.Forms.GroupBox();
            this.pnllstNodes = new System.Windows.Forms.Panel();
            this.lstNodes = new System.Windows.Forms.CheckedListBox();
            this.grblstWFScript = new System.Windows.Forms.GroupBox();
            this.pnllstWFScript = new System.Windows.Forms.Panel();
            this.lstWFScripts = new System.Windows.Forms.CheckedListBox();
            this.grpbUserName = new System.Windows.Forms.GroupBox();
            this.pnllstUsers = new System.Windows.Forms.Panel();
            this.lstUsers = new System.Windows.Forms.CheckedListBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.pnlGrid = new System.Windows.Forms.Panel();
            this.flwlpSummary = new System.Windows.Forms.FlowLayoutPanel();
            this.dgTransaction = new System.Windows.Forms.DataGridView();
            this.pnlFilter = new System.Windows.Forms.Panel();
            this.btnFilter = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.cmbCategory = new System.Windows.Forms.ComboBox();
            this.btnHide = new System.Windows.Forms.Button();
            this.cmbPeriod = new System.Windows.Forms.ComboBox();
            this.cmbState = new System.Windows.Forms.ComboBox();
            this.cmbModule = new System.Windows.Forms.ComboBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnResume = new System.Windows.Forms.Button();
            this.propGrdTransaction = new System.Windows.Forms.PropertyGrid();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.grbTransactionFilters.SuspendLayout();
            this.grblstNodes.SuspendLayout();
            this.pnllstNodes.SuspendLayout();
            this.grblstWFScript.SuspendLayout();
            this.pnllstWFScript.SuspendLayout();
            this.grpbUserName.SuspendLayout();
            this.pnllstUsers.SuspendLayout();
            this.panel2.SuspendLayout();
            this.pnlGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgTransaction)).BeginInit();
            this.pnlFilter.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.SystemColors.Window;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.AutoScroll = true;
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(1269, 535);
            this.splitContainer1.SplitterDistance = 1030;
            this.splitContainer1.TabIndex = 1;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.AutoScroll = true;
            this.splitContainer2.Panel1.Controls.Add(this.grbTransactionFilters);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.AutoScroll = true;
            this.splitContainer2.Panel2.Controls.Add(this.pnlGrid);
            this.splitContainer2.Panel2.Controls.Add(this.pnlFilter);
            this.splitContainer2.Size = new System.Drawing.Size(1030, 535);
            this.splitContainer2.SplitterDistance = 210;
            this.splitContainer2.TabIndex = 0;
            // 
            // grbTransactionFilters
            // 
            this.grbTransactionFilters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grbTransactionFilters.Controls.Add(this.grblstNodes);
            this.grbTransactionFilters.Controls.Add(this.grblstWFScript);
            this.grbTransactionFilters.Controls.Add(this.grpbUserName);
            this.grbTransactionFilters.Controls.Add(this.panel2);
            this.grbTransactionFilters.Location = new System.Drawing.Point(0, 0);
            this.grbTransactionFilters.Name = "grbTransactionFilters";
            this.grbTransactionFilters.Size = new System.Drawing.Size(210, 535);
            this.grbTransactionFilters.TabIndex = 0;
            this.grbTransactionFilters.TabStop = false;
            this.grbTransactionFilters.Text = "Filter Panel";
            // 
            // grblstNodes
            // 
            this.grblstNodes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grblstNodes.Controls.Add(this.pnllstNodes);
            this.grblstNodes.Location = new System.Drawing.Point(2, 379);
            this.grblstNodes.Name = "grblstNodes";
            this.grblstNodes.Size = new System.Drawing.Size(206, 150);
            this.grblstNodes.TabIndex = 1;
            this.grblstNodes.TabStop = false;
            this.grblstNodes.Text = "Machine/Node";
            // 
            // pnllstNodes
            // 
            this.pnllstNodes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnllstNodes.AutoScroll = true;
            this.pnllstNodes.Controls.Add(this.lstNodes);
            this.pnllstNodes.Location = new System.Drawing.Point(4, 19);
            this.pnllstNodes.Name = "pnllstNodes";
            this.pnllstNodes.Size = new System.Drawing.Size(198, 126);
            this.pnllstNodes.TabIndex = 1;
            // 
            // lstNodes
            // 
            this.lstNodes.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstNodes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstNodes.FormattingEnabled = true;
            this.lstNodes.Location = new System.Drawing.Point(0, 0);
            this.lstNodes.Name = "lstNodes";
            this.lstNodes.Size = new System.Drawing.Size(198, 126);
            this.lstNodes.TabIndex = 13;
            this.toolTip1.SetToolTip(this.lstNodes, "Select Machine/Node");
            // 
            // grblstWFScript
            // 
            this.grblstWFScript.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grblstWFScript.Controls.Add(this.pnllstWFScript);
            this.grblstWFScript.Location = new System.Drawing.Point(2, 219);
            this.grblstWFScript.Name = "grblstWFScript";
            this.grblstWFScript.Size = new System.Drawing.Size(206, 150);
            this.grblstWFScript.TabIndex = 0;
            this.grblstWFScript.TabStop = false;
            this.grblstWFScript.Text = "Workflow/Script";
            // 
            // pnllstWFScript
            // 
            this.pnllstWFScript.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnllstWFScript.AutoScroll = true;
            this.pnllstWFScript.Controls.Add(this.lstWFScripts);
            this.pnllstWFScript.Location = new System.Drawing.Point(4, 19);
            this.pnllstWFScript.Name = "pnllstWFScript";
            this.pnllstWFScript.Size = new System.Drawing.Size(198, 126);
            this.pnllstWFScript.TabIndex = 1;
            // 
            // lstWFScripts
            // 
            this.lstWFScripts.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstWFScripts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstWFScripts.FormattingEnabled = true;
            this.lstWFScripts.Location = new System.Drawing.Point(0, 0);
            this.lstWFScripts.Name = "lstWFScripts";
            this.lstWFScripts.Size = new System.Drawing.Size(198, 126);
            this.lstWFScripts.TabIndex = 12;
            this.toolTip1.SetToolTip(this.lstWFScripts, "Select Workflow/Script");
            // 
            // grpbUserName
            // 
            this.grpbUserName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpbUserName.Controls.Add(this.pnllstUsers);
            this.grpbUserName.Location = new System.Drawing.Point(2, 61);
            this.grpbUserName.Name = "grpbUserName";
            this.grpbUserName.Size = new System.Drawing.Size(206, 150);
            this.grpbUserName.TabIndex = 0;
            this.grpbUserName.TabStop = false;
            this.grpbUserName.Text = "User Name";
            // 
            // pnllstUsers
            // 
            this.pnllstUsers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnllstUsers.AutoScroll = true;
            this.pnllstUsers.Controls.Add(this.lstUsers);
            this.pnllstUsers.Location = new System.Drawing.Point(4, 20);
            this.pnllstUsers.Name = "pnllstUsers";
            this.pnllstUsers.Size = new System.Drawing.Size(200, 126);
            this.pnllstUsers.TabIndex = 0;
            // 
            // lstUsers
            // 
            this.lstUsers.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstUsers.FormattingEnabled = true;
            this.lstUsers.Location = new System.Drawing.Point(0, 0);
            this.lstUsers.Name = "lstUsers";
            this.lstUsers.Size = new System.Drawing.Size(200, 126);
            this.lstUsers.TabIndex = 11;
            this.toolTip1.SetToolTip(this.lstUsers, "Select Users");
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.btnSearch);
            this.panel2.Controls.Add(this.txtSearch);
            this.panel2.Location = new System.Drawing.Point(6, 18);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(187, 27);
            this.panel2.TabIndex = 24;
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSearch.FlatAppearance.BorderSize = 0;
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
            this.btnSearch.Location = new System.Drawing.Point(158, 2);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(21, 23);
            this.btnSearch.TabIndex = 10;
            this.toolTip1.SetToolTip(this.btnSearch, "Click here to Search");
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.Location = new System.Drawing.Point(3, 3);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(149, 20);
            this.txtSearch.TabIndex = 9;
            this.toolTip1.SetToolTip(this.txtSearch, "Enter Text to Search");
            // 
            // pnlGrid
            // 
            this.pnlGrid.Controls.Add(this.flwlpSummary);
            this.pnlGrid.Controls.Add(this.dgTransaction);
            this.pnlGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGrid.Location = new System.Drawing.Point(0, 40);
            this.pnlGrid.Name = "pnlGrid";
            this.pnlGrid.Size = new System.Drawing.Size(816, 495);
            this.pnlGrid.TabIndex = 2;
            // 
            // flwlpSummary
            // 
            this.flwlpSummary.AutoScroll = true;
            this.flwlpSummary.AutoSize = true;
            this.flwlpSummary.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.flwlpSummary.Cursor = System.Windows.Forms.Cursors.Default;
            this.flwlpSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flwlpSummary.Location = new System.Drawing.Point(0, 0);
            this.flwlpSummary.Margin = new System.Windows.Forms.Padding(10, 3, 3, 3);
            this.flwlpSummary.Name = "flwlpSummary";
            this.flwlpSummary.Padding = new System.Windows.Forms.Padding(10, 30, 0, 0);
            this.flwlpSummary.Size = new System.Drawing.Size(816, 495);
            this.flwlpSummary.TabIndex = 8;
            // 
            // dgTransaction
            // 
            this.dgTransaction.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgTransaction.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgTransaction.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgTransaction.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgTransaction.Location = new System.Drawing.Point(0, 0);
            this.dgTransaction.MultiSelect = false;
            this.dgTransaction.Name = "dgTransaction";
            this.dgTransaction.ReadOnly = true;
            this.dgTransaction.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgTransaction.Size = new System.Drawing.Size(816, 495);
            this.dgTransaction.TabIndex = 0;
            this.dgTransaction.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgTransaction_ColumnHeaderMouseClick);
            this.dgTransaction.SelectionChanged += new System.EventHandler(this.dgTransaction_SelectionChanged);
            // 
            // pnlFilter
            // 
            this.pnlFilter.Controls.Add(this.btnFilter);
            this.pnlFilter.Controls.Add(this.btnReset);
            this.pnlFilter.Controls.Add(this.cmbCategory);
            this.pnlFilter.Controls.Add(this.btnHide);
            this.pnlFilter.Controls.Add(this.cmbPeriod);
            this.pnlFilter.Controls.Add(this.cmbState);
            this.pnlFilter.Controls.Add(this.cmbModule);
            this.pnlFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFilter.Location = new System.Drawing.Point(0, 0);
            this.pnlFilter.Name = "pnlFilter";
            this.pnlFilter.Size = new System.Drawing.Size(816, 40);
            this.pnlFilter.TabIndex = 0;
            // 
            // btnFilter
            // 
            this.btnFilter.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFilter.FlatAppearance.BorderSize = 0;
            this.btnFilter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFilter.Image = ((System.Drawing.Image)(resources.GetObject("btnFilter.Image")));
            this.btnFilter.Location = new System.Drawing.Point(650, 8);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(21, 23);
            this.btnFilter.TabIndex = 5;
            this.toolTip1.SetToolTip(this.btnFilter, "Click hear to Search");
            this.btnFilter.UseVisualStyleBackColor = true;
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // btnReset
            // 
            this.btnReset.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReset.FlatAppearance.BorderSize = 0;
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReset.Image = ((System.Drawing.Image)(resources.GetObject("btnReset.Image")));
            this.btnReset.Location = new System.Drawing.Point(681, 8);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(27, 23);
            this.btnReset.TabIndex = 6;
            this.toolTip1.SetToolTip(this.btnReset, "Click here to refresh the current result");
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // cmbCategory
            // 
            this.cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategory.Location = new System.Drawing.Point(162, 9);
            this.cmbCategory.Name = "cmbCategory";
            this.cmbCategory.Size = new System.Drawing.Size(153, 21);
            this.cmbCategory.TabIndex = 1;
            this.toolTip1.SetToolTip(this.cmbCategory, "Select Category");
            // 
            // btnHide
            // 
            this.btnHide.AutoSize = true;
            this.btnHide.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnHide.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnHide.Location = new System.Drawing.Point(718, 8);
            this.btnHide.Name = "btnHide";
            this.btnHide.Size = new System.Drawing.Size(86, 23);
            this.btnHide.TabIndex = 7;
            this.btnHide.Text = "View Details";
            this.toolTip1.SetToolTip(this.btnHide, "Click here to View Detail Transactions");
            this.btnHide.UseVisualStyleBackColor = true;
            this.btnHide.Click += new System.EventHandler(this.btnHide_Click);
            // 
            // cmbPeriod
            // 
            this.cmbPeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPeriod.Location = new System.Drawing.Point(321, 9);
            this.cmbPeriod.Name = "cmbPeriod";
            this.cmbPeriod.Size = new System.Drawing.Size(153, 21);
            this.cmbPeriod.TabIndex = 3;
            this.toolTip1.SetToolTip(this.cmbPeriod, "Select Duration ");
            // 
            // cmbState
            // 
            this.cmbState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbState.FormattingEnabled = true;
            this.cmbState.Location = new System.Drawing.Point(480, 9);
            this.cmbState.Name = "cmbState";
            this.cmbState.Size = new System.Drawing.Size(153, 21);
            this.cmbState.TabIndex = 4;
            this.toolTip1.SetToolTip(this.cmbState, "Select Workflow Execution State");
            // 
            // cmbModule
            // 
            this.cmbModule.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbModule.Location = new System.Drawing.Point(3, 9);
            this.cmbModule.Name = "cmbModule";
            this.cmbModule.Size = new System.Drawing.Size(153, 21);
            this.cmbModule.TabIndex = 0;
            this.toolTip1.SetToolTip(this.cmbModule, "Select Module");
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(235, 535);
            this.tabControl1.TabIndex = 6;
            this.toolTip1.SetToolTip(this.tabControl1, "Transaction Property");
            // 
            // tabPage1
            // 
            this.tabPage1.AutoScroll = true;
            this.tabPage1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Controls.Add(this.propGrdTransaction);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(0);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(227, 509);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Property Inspector";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnResume);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(3, 446);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(221, 60);
            this.panel1.TabIndex = 6;
            // 
            // btnResume
            // 
            this.btnResume.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnResume.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnResume.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResume.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnResume.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.btnResume.Location = new System.Drawing.Point(0, 0);
            this.btnResume.Name = "btnResume";
            this.btnResume.Size = new System.Drawing.Size(221, 60);
            this.btnResume.TabIndex = 5;
            this.btnResume.Text = "Resume";
            this.toolTip1.SetToolTip(this.btnResume, "Click here to Resume Workflow");
            this.btnResume.UseVisualStyleBackColor = true;
            this.btnResume.Click += new System.EventHandler(this.btnResume_Click);
            // 
            // propGrdTransaction
            // 
            this.propGrdTransaction.Cursor = System.Windows.Forms.Cursors.Default;
            this.propGrdTransaction.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propGrdTransaction.Location = new System.Drawing.Point(3, 3);
            this.propGrdTransaction.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.propGrdTransaction.Name = "propGrdTransaction";
            this.propGrdTransaction.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.propGrdTransaction.Size = new System.Drawing.Size(221, 503);
            this.propGrdTransaction.TabIndex = 0;
            this.toolTip1.SetToolTip(this.propGrdTransaction, "Transaction Property");
            // 
            // TransactionView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.splitContainer1);
            this.Name = "TransactionView";
            this.Size = new System.Drawing.Size(1269, 535);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.grbTransactionFilters.ResumeLayout(false);
            this.grblstNodes.ResumeLayout(false);
            this.pnllstNodes.ResumeLayout(false);
            this.grblstWFScript.ResumeLayout(false);
            this.pnllstWFScript.ResumeLayout(false);
            this.grpbUserName.ResumeLayout(false);
            this.pnllstUsers.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.pnlGrid.ResumeLayout(false);
            this.pnlGrid.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgTransaction)).EndInit();
            this.pnlFilter.ResumeLayout(false);
            this.pnlFilter.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.GroupBox grbTransactionFilters;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.CheckedListBox lstNodes;
        private System.Windows.Forms.CheckedListBox lstWFScripts;
        private System.Windows.Forms.CheckedListBox lstUsers;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Panel pnlGrid;
        private System.Windows.Forms.FlowLayoutPanel flwlpSummary;
        private System.Windows.Forms.DataGridView dgTransaction;
        private System.Windows.Forms.Panel pnlFilter;
        private System.Windows.Forms.Button btnHide;
        private System.Windows.Forms.ComboBox cmbPeriod;
        private System.Windows.Forms.ComboBox cmbCategory;
        private System.Windows.Forms.ComboBox cmbState;
        private System.Windows.Forms.ComboBox cmbModule;
        private System.Windows.Forms.Button btnResume;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox grpbUserName;
        private System.Windows.Forms.Panel pnllstUsers;
        private System.Windows.Forms.GroupBox grblstNodes;
        private System.Windows.Forms.Panel pnllstNodes;
        private System.Windows.Forms.GroupBox grblstWFScript;
        private System.Windows.Forms.Panel pnllstWFScript;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.PropertyGrid propGrdTransaction;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolTip toolTip1;

    }
}

namespace Infosys.ATR.WFDesigner.Views
{
    partial class Transaction
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
        private void InitializeComponent()
        {
            this.pnlRight = new System.Windows.Forms.Panel();
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.pnlCenter = new System.Windows.Forms.Panel();
            this.pnlGrid = new System.Windows.Forms.Panel();
            this.dgTransaction = new System.Windows.Forms.DataGridView();
            this.pnlFilter = new System.Windows.Forms.Panel();
            this.cmbPeriod = new System.Windows.Forms.ComboBox();
            this.cmbCategory = new System.Windows.Forms.ComboBox();
            this.comboState = new System.Windows.Forms.ComboBox();
            this.cmbModule = new System.Windows.Forms.ComboBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lnkSearch = new System.Windows.Forms.LinkLabel();
            this.lnkReset = new System.Windows.Forms.LinkLabel();
            this.btnSearchAny = new System.Windows.Forms.Button();
            this.lsWfScript = new System.Windows.Forms.ListBox();
            this.lblWfScript = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.lstUserName = new System.Windows.Forms.ListBox();
            this.lstNodes = new System.Windows.Forms.ListBox();
            this.lblNodes = new System.Windows.Forms.Label();
            this.pnlRight.SuspendLayout();
            this.pnlCenter.SuspendLayout();
            this.pnlGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgTransaction)).BeginInit();
            this.pnlFilter.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlRight
            // 
            this.pnlRight.AutoScroll = true;
            this.pnlRight.Controls.Add(this.lstNodes);
            this.pnlRight.Controls.Add(this.lblNodes);
            this.pnlRight.Controls.Add(this.lsWfScript);
            this.pnlRight.Controls.Add(this.lblWfScript);
            this.pnlRight.Controls.Add(this.lblName);
            this.pnlRight.Controls.Add(this.lstUserName);
            this.pnlRight.Controls.Add(this.btnSearchAny);
            this.pnlRight.Controls.Add(this.txtSearch);
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlRight.Location = new System.Drawing.Point(0, 0);
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Size = new System.Drawing.Size(148, 492);
            this.pnlRight.TabIndex = 0;
            // 
            // pnlLeft
            // 
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlLeft.Location = new System.Drawing.Point(664, 0);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Size = new System.Drawing.Size(169, 492);
            this.pnlLeft.TabIndex = 1;
            // 
            // pnlCenter
            // 
            this.pnlCenter.Controls.Add(this.pnlGrid);
            this.pnlCenter.Controls.Add(this.pnlFilter);
            this.pnlCenter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCenter.Location = new System.Drawing.Point(148, 0);
            this.pnlCenter.Name = "pnlCenter";
            this.pnlCenter.Size = new System.Drawing.Size(516, 492);
            this.pnlCenter.TabIndex = 2;
            // 
            // pnlGrid
            // 
            this.pnlGrid.Controls.Add(this.dgTransaction);
            this.pnlGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGrid.Location = new System.Drawing.Point(0, 44);
            this.pnlGrid.Name = "pnlGrid";
            this.pnlGrid.Size = new System.Drawing.Size(516, 448);
            this.pnlGrid.TabIndex = 1;
            // 
            // dgTransaction
            // 
            this.dgTransaction.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgTransaction.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgTransaction.Location = new System.Drawing.Point(0, 0);
            this.dgTransaction.Name = "dgTransaction";
            this.dgTransaction.Size = new System.Drawing.Size(516, 448);
            this.dgTransaction.TabIndex = 0;
            // 
            // pnlFilter
            // 
            this.pnlFilter.Controls.Add(this.lnkReset);
            this.pnlFilter.Controls.Add(this.lnkSearch);
            this.pnlFilter.Controls.Add(this.cmbPeriod);
            this.pnlFilter.Controls.Add(this.cmbCategory);
            this.pnlFilter.Controls.Add(this.comboState);
            this.pnlFilter.Controls.Add(this.cmbModule);
            this.pnlFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFilter.Location = new System.Drawing.Point(0, 0);
            this.pnlFilter.Name = "pnlFilter";
            this.pnlFilter.Size = new System.Drawing.Size(516, 44);
            this.pnlFilter.TabIndex = 0;
            // 
            // cmbPeriod
            // 
            this.cmbPeriod.FormattingEnabled = true;
            this.cmbPeriod.Location = new System.Drawing.Point(325, 10);
            this.cmbPeriod.Name = "cmbPeriod";
            this.cmbPeriod.Size = new System.Drawing.Size(98, 21);
            this.cmbPeriod.TabIndex = 3;
            // 
            // cmbCategory
            // 
            this.cmbCategory.FormattingEnabled = true;
            this.cmbCategory.Location = new System.Drawing.Point(223, 10);
            this.cmbCategory.Name = "cmbCategory";
            this.cmbCategory.Size = new System.Drawing.Size(98, 21);
            this.cmbCategory.TabIndex = 2;
            // 
            // comboState
            // 
            this.comboState.FormattingEnabled = true;
            this.comboState.Location = new System.Drawing.Point(121, 10);
            this.comboState.Name = "comboState";
            this.comboState.Size = new System.Drawing.Size(98, 21);
            this.comboState.TabIndex = 1;
            this.comboState.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // cmbModule
            // 
            this.cmbModule.FormattingEnabled = true;
            this.cmbModule.Location = new System.Drawing.Point(19, 10);
            this.cmbModule.Name = "cmbModule";
            this.cmbModule.Size = new System.Drawing.Size(98, 21);
            this.cmbModule.TabIndex = 0;
            this.cmbModule.SelectedIndexChanged += new System.EventHandler(this.cmbState_SelectedIndexChanged);
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(5, 11);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(137, 20);
            this.txtSearch.TabIndex = 0;
            // 
            // lnkSearch
            // 
            this.lnkSearch.AutoSize = true;
            this.lnkSearch.Location = new System.Drawing.Point(427, 18);
            this.lnkSearch.Name = "lnkSearch";
            this.lnkSearch.Size = new System.Drawing.Size(41, 13);
            this.lnkSearch.TabIndex = 4;
            this.lnkSearch.TabStop = true;
            this.lnkSearch.Text = "Search";
            // 
            // lnkReset
            // 
            this.lnkReset.AutoSize = true;
            this.lnkReset.Location = new System.Drawing.Point(472, 18);
            this.lnkReset.Name = "lnkReset";
            this.lnkReset.Size = new System.Drawing.Size(35, 13);
            this.lnkReset.TabIndex = 5;
            this.lnkReset.TabStop = true;
            this.lnkReset.Text = "Reset";
            // 
            // btnSearchAny
            // 
            this.btnSearchAny.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearchAny.Location = new System.Drawing.Point(67, 44);
            this.btnSearchAny.Name = "btnSearchAny";
            this.btnSearchAny.Size = new System.Drawing.Size(75, 23);
            this.btnSearchAny.TabIndex = 3;
            this.btnSearchAny.Text = "Search";
            this.btnSearchAny.UseVisualStyleBackColor = true;
            // 
            // lsWfScript
            // 
            this.lsWfScript.FormattingEnabled = true;
            this.lsWfScript.Location = new System.Drawing.Point(0, 235);
            this.lsWfScript.Name = "lsWfScript";
            this.lsWfScript.Size = new System.Drawing.Size(140, 121);
            this.lsWfScript.TabIndex = 7;
            // 
            // lblWfScript
            // 
            this.lblWfScript.AutoSize = true;
            this.lblWfScript.Location = new System.Drawing.Point(2, 216);
            this.lblWfScript.Name = "lblWfScript";
            this.lblWfScript.Size = new System.Drawing.Size(56, 13);
            this.lblWfScript.TabIndex = 6;
            this.lblWfScript.Text = "WF/Script";
            // 
            // lblName
            // 
            this.lblName.Location = new System.Drawing.Point(0, 71);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(142, 13);
            this.lblName.TabIndex = 5;
            this.lblName.Text = "User Name";
            // 
            // lstUserName
            // 
            this.lstUserName.FormattingEnabled = true;
            this.lstUserName.Location = new System.Drawing.Point(3, 87);
            this.lstUserName.Name = "lstUserName";
            this.lstUserName.Size = new System.Drawing.Size(140, 121);
            this.lstUserName.TabIndex = 4;
            // 
            // lstNodes
            // 
            this.lstNodes.FormattingEnabled = true;
            this.lstNodes.Location = new System.Drawing.Point(0, 382);
            this.lstNodes.Name = "lstNodes";
            this.lstNodes.Size = new System.Drawing.Size(140, 121);
            this.lstNodes.TabIndex = 9;
            // 
            // lblNodes
            // 
            this.lblNodes.AutoSize = true;
            this.lblNodes.Location = new System.Drawing.Point(2, 363);
            this.lblNodes.Name = "lblNodes";
            this.lblNodes.Size = new System.Drawing.Size(79, 13);
            this.lblNodes.TabIndex = 8;
            this.lblNodes.Text = "Machine/Node";
            // 
            // Transaction
            // 
            this.Controls.Add(this.pnlCenter);
            this.Controls.Add(this.pnlLeft);
            this.Controls.Add(this.pnlRight);
            this.Name = "Transaction";
            this.Size = new System.Drawing.Size(833, 492);
            this.pnlRight.ResumeLayout(false);
            this.pnlRight.PerformLayout();
            this.pnlCenter.ResumeLayout(false);
            this.pnlGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgTransaction)).EndInit();
            this.pnlFilter.ResumeLayout(false);
            this.pnlFilter.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.Panel pnlRight;
        private System.Windows.Forms.Panel pnlLeft;
        private System.Windows.Forms.Panel pnlCenter;
        private System.Windows.Forms.Panel pnlGrid;
        private System.Windows.Forms.DataGridView dgTransaction;
        private System.Windows.Forms.Panel pnlFilter;
        private System.Windows.Forms.ComboBox cmbModule;
        private System.Windows.Forms.ComboBox cmbPeriod;
        private System.Windows.Forms.ComboBox cmbCategory;
        private System.Windows.Forms.ComboBox comboState;
        private System.Windows.Forms.ListBox lstNodes;
        private System.Windows.Forms.Label lblNodes;
        private System.Windows.Forms.ListBox lsWfScript;
        private System.Windows.Forms.Label lblWfScript;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.ListBox lstUserName;
        private System.Windows.Forms.Button btnSearchAny;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.LinkLabel lnkReset;
        private System.Windows.Forms.LinkLabel lnkSearch;

    }
}

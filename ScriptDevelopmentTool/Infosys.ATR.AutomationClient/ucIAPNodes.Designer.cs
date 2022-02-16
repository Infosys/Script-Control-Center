namespace Infosys.ATR.AutomationClient
{
    partial class ucIAPNodes
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
            this.lblDomain = new System.Windows.Forms.Label();
            this.btnFetchDomain = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lbxRegisteredNodes = new System.Windows.Forms.ListBox();
            this.lbxSelectedNodes = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnExecute = new System.Windows.Forms.Button();
            this.lblExe = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbDomains = new System.Windows.Forms.ComboBox();
            this.btnUnSelect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblDomain
            // 
            this.lblDomain.AutoSize = true;
            this.lblDomain.Location = new System.Drawing.Point(17, 16);
            this.lblDomain.Name = "lblDomain";
            this.lblDomain.Size = new System.Drawing.Size(46, 13);
            this.lblDomain.TabIndex = 0;
            this.lblDomain.Text = "Domain:";
            // 
            // btnFetchDomain
            // 
            this.btnFetchDomain.Location = new System.Drawing.Point(310, 10);
            this.btnFetchDomain.Name = "btnFetchDomain";
            this.btnFetchDomain.Size = new System.Drawing.Size(75, 23);
            this.btnFetchDomain.TabIndex = 2;
            this.btnFetchDomain.Text = "Fetch";
            this.btnFetchDomain.UseVisualStyleBackColor = true;
            this.btnFetchDomain.Click += new System.EventHandler(this.btnFetchDomain_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(17, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Registered Nodes:";
            // 
            // lbxRegisteredNodes
            // 
            this.lbxRegisteredNodes.FormattingEnabled = true;
            this.lbxRegisteredNodes.Location = new System.Drawing.Point(20, 72);
            this.lbxRegisteredNodes.Name = "lbxRegisteredNodes";
            this.lbxRegisteredNodes.ScrollAlwaysVisible = true;
            this.lbxRegisteredNodes.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbxRegisteredNodes.Size = new System.Drawing.Size(240, 212);
            this.lbxRegisteredNodes.Sorted = true;
            this.lbxRegisteredNodes.TabIndex = 4;
            // 
            // lbxSelectedNodes
            // 
            this.lbxSelectedNodes.FormattingEnabled = true;
            this.lbxSelectedNodes.Location = new System.Drawing.Point(301, 72);
            this.lbxSelectedNodes.Name = "lbxSelectedNodes";
            this.lbxSelectedNodes.ScrollAlwaysVisible = true;
            this.lbxSelectedNodes.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbxSelectedNodes.Size = new System.Drawing.Size(240, 212);
            this.lbxSelectedNodes.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(298, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Selected Nodes:";
            // 
            // btnSelect
            // 
            this.btnSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelect.Location = new System.Drawing.Point(265, 165);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(29, 23);
            this.btnSelect.TabIndex = 7;
            this.btnSelect.Text = ">>";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnExecute
            // 
            this.btnExecute.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExecute.Location = new System.Drawing.Point(302, 313);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(29, 23);
            this.btnExecute.TabIndex = 8;
            this.btnExecute.Text = ">";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // lblExe
            // 
            this.lblExe.AutoSize = true;
            this.lblExe.Location = new System.Drawing.Point(330, 318);
            this.lblExe.Name = "lblExe";
            this.lblExe.Size = new System.Drawing.Size(145, 13);
            this.lblExe.TabIndex = 9;
            this.lblExe.Text = "Execute on intended Node(s)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.label4.Location = new System.Drawing.Point(300, 288);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(239, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "(Select the Node(s), use CTRL for multiple select)";
            this.label4.Visible = false;
            // 
            // cmbDomains
            // 
            this.cmbDomains.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDomains.FormattingEnabled = true;
            this.cmbDomains.Location = new System.Drawing.Point(83, 12);
            this.cmbDomains.Name = "cmbDomains";
            this.cmbDomains.Size = new System.Drawing.Size(221, 21);
            this.cmbDomains.TabIndex = 11;
            // 
            // btnUnSelect
            // 
            this.btnUnSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUnSelect.Location = new System.Drawing.Point(265, 190);
            this.btnUnSelect.Name = "btnUnSelect";
            this.btnUnSelect.Size = new System.Drawing.Size(29, 23);
            this.btnUnSelect.TabIndex = 12;
            this.btnUnSelect.Text = "<<";
            this.btnUnSelect.UseVisualStyleBackColor = true;
            this.btnUnSelect.Click += new System.EventHandler(this.btnUnSelect_Click);
            // 
            // ucIAPNodes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnUnSelect);
            this.Controls.Add(this.cmbDomains);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblExe);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.lbxSelectedNodes);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbxRegisteredNodes);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnFetchDomain);
            this.Controls.Add(this.lblDomain);
            this.Name = "ucIAPNodes";
            this.Size = new System.Drawing.Size(560, 350);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDomain;
        private System.Windows.Forms.Button btnFetchDomain;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lbxRegisteredNodes;
        private System.Windows.Forms.ListBox lbxSelectedNodes;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Label lblExe;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbDomains;
        private System.Windows.Forms.Button btnUnSelect;
    }
}

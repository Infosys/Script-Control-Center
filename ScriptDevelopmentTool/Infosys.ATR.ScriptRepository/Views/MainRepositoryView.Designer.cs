namespace Infosys.ATR.ScriptRepository.Views
{
    partial class MainRepositoryView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainRepositoryView));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.scriptsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newScriptMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runScriptLocallyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.categorySettingsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newCategoryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newSubCategoryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlRepository = new System.Windows.Forms.Panel();
            this.splitContainerCatSubcatTree = new System.Windows.Forms.SplitContainer();
            this.tvCatSubcat = new System.Windows.Forms.TreeView();
            this.splitContainerDetails = new System.Windows.Forms.SplitContainer();
            this.dgList = new System.Windows.Forms.DataGridView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolbarNewScript = new System.Windows.Forms.ToolStripButton();
            this.toolbarRunScript = new System.Windows.Forms.ToolStripButton();
            this.Run = new System.Windows.Forms.DataGridViewImageColumn();
            this.Delete = new System.Windows.Forms.DataGridViewImageColumn();
            this.toolbarCategorySettings = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolbarNewCategory = new System.Windows.Forms.ToolStripMenuItem();
            this.toolbarNewSubCategory = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlButtons.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.pnlRepository.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerCatSubcatTree)).BeginInit();
            this.splitContainerCatSubcatTree.Panel1.SuspendLayout();
            this.splitContainerCatSubcatTree.Panel2.SuspendLayout();
            this.splitContainerCatSubcatTree.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerDetails)).BeginInit();
            this.splitContainerDetails.Panel1.SuspendLayout();
            this.splitContainerDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgList)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlButtons
            // 
            this.pnlButtons.Controls.Add(this.toolStrip1);
            this.pnlButtons.Controls.Add(this.menuStrip1);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlButtons.Location = new System.Drawing.Point(0, 0);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(873, 70);
            this.pnlButtons.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.scriptsMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(873, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // scriptsMenuItem
            // 
            this.scriptsMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newScriptMenuItem,
            this.runScriptLocallyMenuItem,
            this.toolStripSeparator1,
            this.categorySettingsMenuItem});
            this.scriptsMenuItem.Name = "scriptsMenuItem";
            this.scriptsMenuItem.ShortcutKeyDisplayString = "";
            this.scriptsMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.S)));
            this.scriptsMenuItem.Size = new System.Drawing.Size(54, 20);
            this.scriptsMenuItem.Text = "&Scripts";
            // 
            // newScriptMenuItem
            // 
            this.newScriptMenuItem.Name = "newScriptMenuItem";
            this.newScriptMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.N)));
            this.newScriptMenuItem.Size = new System.Drawing.Size(205, 22);
            this.newScriptMenuItem.Text = "&New Script";
            this.newScriptMenuItem.Click += new System.EventHandler(this.newScriptMenuItem_Click);
            // 
            // runScriptLocallyMenuItem
            // 
            this.runScriptLocallyMenuItem.Enabled = false;
            this.runScriptLocallyMenuItem.Name = "runScriptLocallyMenuItem";
            this.runScriptLocallyMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.R)));
            this.runScriptLocallyMenuItem.Size = new System.Drawing.Size(205, 22);
            this.runScriptLocallyMenuItem.Text = "&Run Script Locally";
            this.runScriptLocallyMenuItem.Click += new System.EventHandler(this.runScriptLocallyMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(202, 6);
            // 
            // categorySettingsMenuItem
            // 
            this.categorySettingsMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newCategoryMenuItem,
            this.newSubCategoryMenuItem});
            this.categorySettingsMenuItem.Name = "categorySettingsMenuItem";
            this.categorySettingsMenuItem.Size = new System.Drawing.Size(205, 22);
            this.categorySettingsMenuItem.Text = "Category Settings";
            // 
            // newCategoryMenuItem
            // 
            this.newCategoryMenuItem.Name = "newCategoryMenuItem";
            this.newCategoryMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.C)));
            this.newCategoryMenuItem.Size = new System.Drawing.Size(210, 22);
            this.newCategoryMenuItem.Text = "New &Category";
            this.newCategoryMenuItem.Click += new System.EventHandler(this.newCategoryMenuItem_Click);
            // 
            // newSubCategoryMenuItem
            // 
            this.newSubCategoryMenuItem.Name = "newSubCategoryMenuItem";
            this.newSubCategoryMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.U)));
            this.newSubCategoryMenuItem.Size = new System.Drawing.Size(210, 22);
            this.newSubCategoryMenuItem.Text = "New S&ub Category";
            this.newSubCategoryMenuItem.Click += new System.EventHandler(this.newSubCategoryMenuItem_Click);
            // 
            // pnlRepository
            // 
            this.pnlRepository.Controls.Add(this.splitContainerCatSubcatTree);
            this.pnlRepository.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRepository.Location = new System.Drawing.Point(0, 70);
            this.pnlRepository.Name = "pnlRepository";
            this.pnlRepository.Size = new System.Drawing.Size(873, 473);
            this.pnlRepository.TabIndex = 1;
            // 
            // splitContainerCatSubcatTree
            // 
            this.splitContainerCatSubcatTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerCatSubcatTree.Location = new System.Drawing.Point(0, 0);
            this.splitContainerCatSubcatTree.Name = "splitContainerCatSubcatTree";
            // 
            // splitContainerCatSubcatTree.Panel1
            // 
            this.splitContainerCatSubcatTree.Panel1.Controls.Add(this.tvCatSubcat);
            // 
            // splitContainerCatSubcatTree.Panel2
            // 
            this.splitContainerCatSubcatTree.Panel2.Controls.Add(this.splitContainerDetails);
            this.splitContainerCatSubcatTree.Size = new System.Drawing.Size(873, 473);
            this.splitContainerCatSubcatTree.SplitterDistance = 170;
            this.splitContainerCatSubcatTree.TabIndex = 0;
            // 
            // tvCatSubcat
            // 
            this.tvCatSubcat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvCatSubcat.Location = new System.Drawing.Point(0, 0);
            this.tvCatSubcat.Name = "tvCatSubcat";
            this.tvCatSubcat.Size = new System.Drawing.Size(170, 473);
            this.tvCatSubcat.TabIndex = 0;
            this.tvCatSubcat.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.tvCatSubcat_AfterExpand);
            this.tvCatSubcat.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvCatSubcat_AfterSelect);
            this.tvCatSubcat.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tvCatSubcat_MouseClick);
            // 
            // splitContainerDetails
            // 
            this.splitContainerDetails.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainerDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerDetails.Location = new System.Drawing.Point(0, 0);
            this.splitContainerDetails.Name = "splitContainerDetails";
            // 
            // splitContainerDetails.Panel1
            // 
            this.splitContainerDetails.Panel1.Controls.Add(this.dgList);
            // 
            // splitContainerDetails.Panel2
            // 
            this.splitContainerDetails.Panel2.AutoScroll = true;
            this.splitContainerDetails.Panel2.BackColor = System.Drawing.SystemColors.Window;
            this.splitContainerDetails.Size = new System.Drawing.Size(699, 473);
            this.splitContainerDetails.SplitterDistance = 365;
            this.splitContainerDetails.TabIndex = 0;
            // 
            // dgList
            // 
            this.dgList.AllowUserToOrderColumns = true;
            this.dgList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgList.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgList.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Run,
            this.Delete});
            this.dgList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgList.Location = new System.Drawing.Point(0, 0);
            this.dgList.MultiSelect = false;
            this.dgList.Name = "dgList";
            this.dgList.ReadOnly = true;
            this.dgList.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgList.RowHeadersVisible = false;
            this.dgList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgList.Size = new System.Drawing.Size(363, 471);
            this.dgList.TabIndex = 0;
            this.dgList.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgList_CellMouseClick);
            this.dgList.CellToolTipTextNeeded += new System.Windows.Forms.DataGridViewCellToolTipTextNeededEventHandler(this.dgList__CellToolTipTextNeeded);
            this.dgList.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgList_ColumnHeaderMouseClick);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolbarNewScript,
            this.toolbarRunScript,
            this.toolbarCategorySettings});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(873, 25);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolbarNewScript
            // 
            this.toolbarNewScript.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolbarNewScript.Image = ((System.Drawing.Image)(resources.GetObject("toolbarNewScript.Image")));
            this.toolbarNewScript.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolbarNewScript.Name = "toolbarNewScript";
            this.toolbarNewScript.Size = new System.Drawing.Size(23, 22);
            this.toolbarNewScript.Text = "New Script";
            this.toolbarNewScript.Click += new System.EventHandler(this.toolbarNewScript_Click);
            // 
            // toolbarRunScript
            // 
            this.toolbarRunScript.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolbarRunScript.Image = ((System.Drawing.Image)(resources.GetObject("toolbarRunScript.Image")));
            this.toolbarRunScript.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolbarRunScript.Name = "toolbarRunScript";
            this.toolbarRunScript.Size = new System.Drawing.Size(23, 22);
            this.toolbarRunScript.Text = "Run Script Locally";
            this.toolbarRunScript.Click += new System.EventHandler(this.toolbarRunScript_Click);
            // 
            // Run
            // 
            this.Run.FillWeight = 15F;
            this.Run.HeaderText = "";
            this.Run.Image = ((System.Drawing.Image)(resources.GetObject("Run.Image")));
            this.Run.Name = "Run";
            this.Run.ReadOnly = true;
            this.Run.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Run.ToolTipText = "Run Script Locally";
            // 
            // Delete
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Red;
            dataGridViewCellStyle3.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle3.NullValue")));
            this.Delete.DefaultCellStyle = dataGridViewCellStyle3;
            this.Delete.FillWeight = 15F;
            this.Delete.HeaderText = "";
            this.Delete.Image = ((System.Drawing.Image)(resources.GetObject("Delete.Image")));
            this.Delete.Name = "Delete";
            this.Delete.ReadOnly = true;
            this.Delete.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Delete.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Delete.ToolTipText = "Delete";
            // 
            // toolbarCategorySettings
            // 
            this.toolbarCategorySettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolbarCategorySettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolbarNewCategory,
            this.toolbarNewSubCategory});
            this.toolbarCategorySettings.Image = ((System.Drawing.Image)(resources.GetObject("toolbarCategorySettings.Image")));
            this.toolbarCategorySettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolbarCategorySettings.Name = "toolbarCategorySettings";
            this.toolbarCategorySettings.Size = new System.Drawing.Size(29, 22);
            this.toolbarCategorySettings.ToolTipText = "Category Settings";
            // 
            // toolbarNewCategory
            // 
            this.toolbarNewCategory.Name = "toolbarNewCategory";
            this.toolbarNewCategory.Size = new System.Drawing.Size(172, 22);
            this.toolbarNewCategory.Text = "New Category";
            this.toolbarNewCategory.ToolTipText = "New Category";
            this.toolbarNewCategory.Click += new System.EventHandler(this.toolbarNewCategory_Click_1);
            // 
            // toolbarNewSubCategory
            // 
            this.toolbarNewSubCategory.Name = "toolbarNewSubCategory";
            this.toolbarNewSubCategory.Size = new System.Drawing.Size(172, 22);
            this.toolbarNewSubCategory.Text = "New Sub Category";
            this.toolbarNewSubCategory.ToolTipText = "New Sub Category";
            this.toolbarNewSubCategory.Click += new System.EventHandler(this.toolbarNewSubCategory_Click_1);
            // 
            // MainRepositoryView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlRepository);
            this.Controls.Add(this.pnlButtons);
            this.Name = "MainRepositoryView";
            this.Size = new System.Drawing.Size(873, 543);
            this.pnlButtons.ResumeLayout(false);
            this.pnlButtons.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.pnlRepository.ResumeLayout(false);
            this.splitContainerCatSubcatTree.Panel1.ResumeLayout(false);
            this.splitContainerCatSubcatTree.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerCatSubcatTree)).EndInit();
            this.splitContainerCatSubcatTree.ResumeLayout(false);
            this.splitContainerDetails.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerDetails)).EndInit();
            this.splitContainerDetails.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgList)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Panel pnlRepository;
        private System.Windows.Forms.SplitContainer splitContainerCatSubcatTree;
        private System.Windows.Forms.SplitContainer splitContainerDetails;
        private System.Windows.Forms.TreeView tvCatSubcat;
        private System.Windows.Forms.DataGridView dgList;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem scriptsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newScriptMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runScriptLocallyMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem categorySettingsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newCategoryMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newSubCategoryMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolbarNewScript;
        private System.Windows.Forms.ToolStripButton toolbarRunScript;
        private System.Windows.Forms.DataGridViewImageColumn Run;
        private System.Windows.Forms.DataGridViewImageColumn Delete;
        private System.Windows.Forms.ToolStripDropDownButton toolbarCategorySettings;
        private System.Windows.Forms.ToolStripMenuItem toolbarNewCategory;
        private System.Windows.Forms.ToolStripMenuItem toolbarNewSubCategory;
    }
}

namespace Infosys.ATR.Admin.Views
{
    partial class Explorer
    {
        ExplorerPresenter _presenter = null;
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
            this.outerContainer = new System.Windows.Forms.SplitContainer();
            this.innerContainer = new System.Windows.Forms.SplitContainer();
            this.pnlTreeView = new System.Windows.Forms.Panel();
            this.trGroups = new System.Windows.Forms.TreeView();
            this.groupMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addGroup = new System.Windows.Forms.ToolStripMenuItem();
            this.delGroup = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.usersGrid = new System.Windows.Forms.DataGridView();
            this._centerWorkspace = new Microsoft.Practices.CompositeUI.WPF.DeckWorkspace();
            this._rightWorkspace = new Microsoft.Practices.CompositeUI.WPF.DeckWorkspace();
            ((System.ComponentModel.ISupportInitialize)(this.outerContainer)).BeginInit();
            this.outerContainer.Panel1.SuspendLayout();
            this.outerContainer.Panel2.SuspendLayout();
            this.outerContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.innerContainer)).BeginInit();
            this.innerContainer.Panel1.SuspendLayout();
            this.innerContainer.Panel2.SuspendLayout();
            this.innerContainer.SuspendLayout();
            this.pnlTreeView.SuspendLayout();
            this.groupMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.usersGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // outerContainer
            // 
            this.outerContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outerContainer.Location = new System.Drawing.Point(0, 0);
            this.outerContainer.Name = "outerContainer";
            // 
            // outerContainer.Panel1
            // 
            this.outerContainer.Panel1.Controls.Add(this.innerContainer);
            // 
            // outerContainer.Panel2
            // 
            this.outerContainer.Panel2.Controls.Add(this._rightWorkspace);
            this.outerContainer.Size = new System.Drawing.Size(1214, 582);
            this.outerContainer.SplitterDistance = 851;
            this.outerContainer.TabIndex = 0;
            // 
            // innerContainer
            // 
            this.innerContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.innerContainer.Location = new System.Drawing.Point(0, 0);
            this.innerContainer.Name = "innerContainer";
            // 
            // innerContainer.Panel1
            // 
            this.innerContainer.Panel1.Controls.Add(this.pnlTreeView);
            // 
            // innerContainer.Panel2
            // 
            this.innerContainer.Panel2.Controls.Add(this.usersGrid);
            this.innerContainer.Panel2.Controls.Add(this._centerWorkspace);
            this.innerContainer.Size = new System.Drawing.Size(851, 582);
            this.innerContainer.SplitterDistance = 148;
            this.innerContainer.TabIndex = 0;
            // 
            // pnlTreeView
            // 
            this.pnlTreeView.Controls.Add(this.trGroups);
            this.pnlTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTreeView.Location = new System.Drawing.Point(0, 0);
            this.pnlTreeView.Name = "pnlTreeView";
            this.pnlTreeView.Size = new System.Drawing.Size(148, 582);
            this.pnlTreeView.TabIndex = 0;
            // 
            // trGroups
            // 
            this.trGroups.ContextMenuStrip = this.groupMenu;
            this.trGroups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trGroups.HideSelection = false;
            this.trGroups.Location = new System.Drawing.Point(0, 0);
            this.trGroups.Name = "trGroups";
            this.trGroups.Size = new System.Drawing.Size(148, 582);
            this.trGroups.TabIndex = 0;
            this.trGroups.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trGroups_AfterSelect);
            // 
            // groupMenu
            // 
            this.groupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addGroup,
            this.delGroup,
            this.refreshToolStripMenuItem});
            this.groupMenu.Name = "groupMenu";
            this.groupMenu.Size = new System.Drawing.Size(153, 92);
            this.groupMenu.Opening += new System.ComponentModel.CancelEventHandler(this.groupMenu_Opening);
            this.groupMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.groupMenu_ItemClicked);
            // 
            // addGroup
            // 
            this.addGroup.Name = "addGroup";
            this.addGroup.Size = new System.Drawing.Size(152, 22);
            this.addGroup.Text = "Add";
            this.addGroup.Click += new System.EventHandler(this.addGroup_Click);
            // 
            // delGroup
            // 
            this.delGroup.Name = "delGroup";
            this.delGroup.Size = new System.Drawing.Size(152, 22);
            this.delGroup.Text = "Delete";
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            // 
            // usersGrid
            // 
            this.usersGrid.AllowUserToAddRows = false;
            this.usersGrid.AllowUserToDeleteRows = false;
            this.usersGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.usersGrid.BackgroundColor = System.Drawing.Color.White;
            this.usersGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.usersGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.usersGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.usersGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.usersGrid.Location = new System.Drawing.Point(0, 0);
            this.usersGrid.MultiSelect = false;
            this.usersGrid.Name = "usersGrid";
            this.usersGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.usersGrid.RowHeadersVisible = false;
            this.usersGrid.Size = new System.Drawing.Size(699, 582);
            this.usersGrid.TabIndex = 1;
            this.usersGrid.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.usersGrid_CellClick);
            this.usersGrid.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.usersGrid_CellPainting);
            this.usersGrid.ColumnAdded += new System.Windows.Forms.DataGridViewColumnEventHandler(this.usersGrid_ColumnAdded);
            this.usersGrid.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.usersGrid_EditingControlShowing);
            // 
            // _centerWorkspace
            // 
            this._centerWorkspace.BackColor = System.Drawing.Color.White;
            this._centerWorkspace.Dock = System.Windows.Forms.DockStyle.Fill;
            this._centerWorkspace.Location = new System.Drawing.Point(0, 0);
            this._centerWorkspace.Name = "_centerWorkspace";
            this._centerWorkspace.Size = new System.Drawing.Size(699, 582);
            this._centerWorkspace.TabIndex = 0;
            this._centerWorkspace.Text = "deckWorkspace1";
            // 
            // _rightWorkspace
            // 
            this._rightWorkspace.BackColor = System.Drawing.Color.White;
            this._rightWorkspace.Dock = System.Windows.Forms.DockStyle.Fill;
            this._rightWorkspace.Location = new System.Drawing.Point(0, 0);
            this._rightWorkspace.Name = "_rightWorkspace";
            this._rightWorkspace.Size = new System.Drawing.Size(359, 582);
            this._rightWorkspace.TabIndex = 0;
            this._rightWorkspace.Text = "deckWorkspace2";
            // 
            // Explorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.outerContainer);
            this.Name = "Explorer";
            this.Size = new System.Drawing.Size(1214, 582);
            this.outerContainer.Panel1.ResumeLayout(false);
            this.outerContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.outerContainer)).EndInit();
            this.outerContainer.ResumeLayout(false);
            this.innerContainer.Panel1.ResumeLayout(false);
            this.innerContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.innerContainer)).EndInit();
            this.innerContainer.ResumeLayout(false);
            this.pnlTreeView.ResumeLayout(false);
            this.groupMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.usersGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer outerContainer;
        private System.Windows.Forms.SplitContainer innerContainer;
        private Microsoft.Practices.CompositeUI.WPF.DeckWorkspace _centerWorkspace;
        private Microsoft.Practices.CompositeUI.WPF.DeckWorkspace _rightWorkspace;
        private System.Windows.Forms.Panel pnlTreeView;
        private System.Windows.Forms.TreeView trGroups;
        private System.Windows.Forms.DataGridView usersGrid;
        private System.Windows.Forms.ContextMenuStrip groupMenu;
        private System.Windows.Forms.ToolStripMenuItem addGroup;
        private System.Windows.Forms.ToolStripMenuItem delGroup;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
    }
}

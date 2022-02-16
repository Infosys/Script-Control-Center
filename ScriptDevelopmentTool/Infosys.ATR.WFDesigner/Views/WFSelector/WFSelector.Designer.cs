namespace Infosys.ATR.WFDesigner.Views
{
    partial class WFSelector
    {
        /// <summary>
        /// The presenter used by this view.
        /// </summary>
        private Infosys.ATR.WFDesigner.Views.WFSelectorPresenter _presenter = null;

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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pnlWFTree = new System.Windows.Forms.Panel();
            this.trWF = new System.Windows.Forms.TreeView();
            this.ctxtMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.pnlWFList = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.pnlCatList = new System.Windows.Forms.Panel();
            this.categoryWorkspace = new Microsoft.Practices.CompositeUI.WPF.DeckWorkspace();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.pnlWFTree.SuspendLayout();
            this.ctxtMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.pnlWFList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.pnlCatList.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.pnlWFTree);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(975, 531);
            this.splitContainer1.SplitterDistance = 141;
            this.splitContainer1.TabIndex = 0;
            // 
            // pnlWFTree
            // 
            this.pnlWFTree.Controls.Add(this.trWF);
            this.pnlWFTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlWFTree.Location = new System.Drawing.Point(0, 0);
            this.pnlWFTree.Name = "pnlWFTree";
            this.pnlWFTree.Size = new System.Drawing.Size(141, 531);
            this.pnlWFTree.TabIndex = 0;
            // 
            // trWF
            // 
            this.trWF.ContextMenuStrip = this.ctxtMenu;
            this.trWF.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trWF.Location = new System.Drawing.Point(0, 0);
            this.trWF.Name = "trWF";
            this.trWF.Size = new System.Drawing.Size(141, 531);
            this.trWF.TabIndex = 0;
            this.trWF.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trWF_AfterSelect);
            this.trWF.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.trWF_NodeMouseClick);
            this.trWF.MouseClick += new System.Windows.Forms.MouseEventHandler(this.trWF_MouseClick);
            // 
            // ctxtMenu
            // 
            this.ctxtMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshToolStripMenuItem});
            this.ctxtMenu.Name = "ctxtMenu";
            this.ctxtMenu.Size = new System.Drawing.Size(113, 26);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.pnlWFList);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.pnlCatList);
            this.splitContainer2.Size = new System.Drawing.Size(830, 531);
            this.splitContainer2.SplitterDistance = 505;
            this.splitContainer2.TabIndex = 0;
            // 
            // pnlWFList
            // 
            this.pnlWFList.Controls.Add(this.dataGridView1);
            this.pnlWFList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlWFList.Location = new System.Drawing.Point(0, 0);
            this.pnlWFList.Name = "pnlWFList";
            this.pnlWFList.Size = new System.Drawing.Size(505, 531);
            this.pnlWFList.TabIndex = 1;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.Size = new System.Drawing.Size(505, 531);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseDoubleClick_1);
            this.dataGridView1.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dataGridView1_CellPainting);
            this.dataGridView1.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_ColumnHeaderMouseClick);
            this.dataGridView1.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_RowEnter);
            // 
            // pnlCatList
            // 
            this.pnlCatList.BackColor = System.Drawing.Color.White;
            this.pnlCatList.Controls.Add(this.categoryWorkspace);
            this.pnlCatList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCatList.Location = new System.Drawing.Point(0, 0);
            this.pnlCatList.Name = "pnlCatList";
            this.pnlCatList.Size = new System.Drawing.Size(321, 531);
            this.pnlCatList.TabIndex = 0;
            // 
            // categoryWorkspace
            // 
            this.categoryWorkspace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.categoryWorkspace.Location = new System.Drawing.Point(0, 0);
            this.categoryWorkspace.Name = "categoryWorkspace";
            this.categoryWorkspace.Size = new System.Drawing.Size(321, 531);
            this.categoryWorkspace.TabIndex = 0;
            this.categoryWorkspace.Text = "deckWorkspace1";
            // 
            // WFSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "WFSelector";
            this.Size = new System.Drawing.Size(975, 531);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.pnlWFTree.ResumeLayout(false);
            this.ctxtMenu.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.pnlWFList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.pnlCatList.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel pnlWFTree;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Panel pnlWFList;
        private System.Windows.Forms.Panel pnlCatList;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TreeView trWF;
        private Microsoft.Practices.CompositeUI.WPF.DeckWorkspace categoryWorkspace;
        private System.Windows.Forms.ContextMenuStrip ctxtMenu;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
    }
}

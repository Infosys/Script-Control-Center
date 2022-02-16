namespace Infosys.ATR.DevelopmentStudio
{
    partial class uc_ControlEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uc_ControlEditor));
            this.trAppControl = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pnlControlTree = new System.Windows.Forms.Panel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.pnlControlTree.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // trAppControl
            // 
            this.trAppControl.CheckBoxes = true;
            this.trAppControl.ContextMenuStrip = this.contextMenuStrip1;
            this.trAppControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trAppControl.ImageIndex = 0;
            this.trAppControl.ImageList = this.imageList1;
            this.trAppControl.ItemHeight = 24;
            this.trAppControl.Location = new System.Drawing.Point(0, 0);
            this.trAppControl.Name = "trAppControl";
            this.trAppControl.SelectedImageIndex = 0;
            this.trAppControl.Size = new System.Drawing.Size(684, 489);
            this.trAppControl.StateImageList = this.imageList1;
            this.trAppControl.TabIndex = 0;
            this.trAppControl.TabStop = false;
            this.trAppControl.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.trAppControl_AfterCheck);
            this.trAppControl.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.trAppControl_AfterExpand);
            this.trAppControl.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trAppControl_AfterSelect);
            this.trAppControl.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.trAppControl_NodeMouseClick);
            this.trAppControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.trAppControl_MouseDown);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "node.jpg");
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.trAppControl);
            this.splitContainer1.Size = new System.Drawing.Size(1148, 489);
            this.splitContainer1.SplitterDistance = 684;
            this.splitContainer1.TabIndex = 1;
            // 
            // pnlControlTree
            // 
            this.pnlControlTree.Controls.Add(this.splitContainer1);
            this.pnlControlTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlControlTree.Location = new System.Drawing.Point(0, 0);
            this.pnlControlTree.Name = "pnlControlTree";
            this.pnlControlTree.Size = new System.Drawing.Size(1148, 489);
            this.pnlControlTree.TabIndex = 4;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(108, 48);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.addToolStripMenuItem.Text = "Add";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // uc_ControlEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlControlTree);
            this.Name = "uc_ControlEditor";
            this.Size = new System.Drawing.Size(1148, 489);
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.pnlControlTree.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView trAppControl;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel pnlControlTree;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;

    }
}

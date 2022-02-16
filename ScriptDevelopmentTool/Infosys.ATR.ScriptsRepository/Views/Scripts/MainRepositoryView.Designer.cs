namespace Infosys.ATR.ScriptRepository.Views
{
    partial class MainRepositoryView
    {
        public ScriptsPresenter _presenter = null;
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlRepository = new System.Windows.Forms.Panel();
            this.splitContainerCatSubcatTree = new System.Windows.Forms.SplitContainer();
            this.tvCatSubcat = new System.Windows.Forms.TreeView();
            this.ctxtmenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainerDetails = new System.Windows.Forms.SplitContainer();
            this.dgList = new System.Windows.Forms.DataGridView();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.pnlRepository.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerCatSubcatTree)).BeginInit();
            this.splitContainerCatSubcatTree.Panel1.SuspendLayout();
            this.splitContainerCatSubcatTree.Panel2.SuspendLayout();
            this.splitContainerCatSubcatTree.SuspendLayout();
            this.ctxtmenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerDetails)).BeginInit();
            this.splitContainerDetails.Panel1.SuspendLayout();
            this.splitContainerDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgList)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlRepository
            // 
            this.pnlRepository.Controls.Add(this.splitContainerCatSubcatTree);
            this.pnlRepository.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRepository.Location = new System.Drawing.Point(0, 0);
            this.pnlRepository.Name = "pnlRepository";
            this.pnlRepository.Size = new System.Drawing.Size(873, 543);
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
            this.splitContainerCatSubcatTree.Size = new System.Drawing.Size(873, 543);
            this.splitContainerCatSubcatTree.SplitterDistance = 170;
            this.splitContainerCatSubcatTree.TabIndex = 0;
            // 
            // tvCatSubcat
            // 
            this.tvCatSubcat.ContextMenuStrip = this.ctxtmenu;
            this.tvCatSubcat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvCatSubcat.HideSelection = false;
            this.tvCatSubcat.Location = new System.Drawing.Point(0, 0);
            this.tvCatSubcat.Name = "tvCatSubcat";
            this.tvCatSubcat.Size = new System.Drawing.Size(170, 543);
            this.tvCatSubcat.TabIndex = 0;
            this.tvCatSubcat.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.tvCatSubcat_AfterExpand);
            this.tvCatSubcat.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvCatSubcat_AfterSelect);
            this.tvCatSubcat.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvCatSubcat_NodeMouseClick);
            this.tvCatSubcat.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tvCatSubcat_MouseClick);
            // 
            // ctxtmenu
            // 
            this.ctxtmenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshToolStripMenuItem});
            this.ctxtmenu.Name = "ctxtmenu";
            this.ctxtmenu.Size = new System.Drawing.Size(113, 26);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
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
            this.splitContainerDetails.Size = new System.Drawing.Size(699, 543);
            this.splitContainerDetails.SplitterDistance = 365;
            this.splitContainerDetails.TabIndex = 0;
            // 
            // dgList
            // 
            this.dgList.AllowUserToOrderColumns = true;
            this.dgList.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgList.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgList.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgList.Location = new System.Drawing.Point(0, 0);
            this.dgList.MultiSelect = false;
            this.dgList.Name = "dgList";
            this.dgList.ReadOnly = true;
            this.dgList.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgList.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgList.RowHeadersVisible = false;
            this.dgList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgList.Size = new System.Drawing.Size(363, 541);
            this.dgList.TabIndex = 0;
            this.dgList.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgList_CellFormatting);
            this.dgList.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgList_CellMouseClick);
            this.dgList.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgList_CellMouseDoubleClick);
            this.dgList.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgList_CellMouseEnter);
            this.dgList.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgList_CellMouseLeave);
            this.dgList.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dgList_CellPainting);
            this.dgList.CellToolTipTextNeeded += new System.Windows.Forms.DataGridViewCellToolTipTextNeededEventHandler(this.dgList__CellToolTipTextNeeded);
            this.dgList.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgList_ColumnHeaderMouseClick);
            this.dgList.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgList_RowEnter);
            this.dgList.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dgList_KeyPress);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            // 
            // MainRepositoryView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlRepository);
            this.Name = "MainRepositoryView";
            this.Size = new System.Drawing.Size(873, 543);
            this.pnlRepository.ResumeLayout(false);
            this.splitContainerCatSubcatTree.Panel1.ResumeLayout(false);
            this.splitContainerCatSubcatTree.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerCatSubcatTree)).EndInit();
            this.splitContainerCatSubcatTree.ResumeLayout(false);
            this.ctxtmenu.ResumeLayout(false);
            this.splitContainerDetails.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerDetails)).EndInit();
            this.splitContainerDetails.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlRepository;
        private System.Windows.Forms.SplitContainer splitContainerCatSubcatTree;
        private System.Windows.Forms.SplitContainer splitContainerDetails;
        private System.Windows.Forms.TreeView tvCatSubcat;
        private System.Windows.Forms.DataGridView dgList;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip ctxtmenu;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
    }
}

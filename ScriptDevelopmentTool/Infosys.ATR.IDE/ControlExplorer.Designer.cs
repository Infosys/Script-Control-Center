namespace Infosys.ATR.DevelopmentStudio
{
    partial class ControlExplorer
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlExplorer));
            this.pnlControls = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.pnlControlTree = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.trAppControl = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripbtnOK = new System.Windows.Forms.ToolStripButton();
            this.toolStripbtnRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripbtnCtlCaptureOnMove = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_SaveOM = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonEditOM = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonTakeSnap = new System.Windows.Forms.ToolStripButton();
            this.pnlControls.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.pnlControlTree.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlControls
            // 
            this.pnlControls.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlControls.Controls.Add(this.toolStrip1);
            this.pnlControls.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlControls.Location = new System.Drawing.Point(0, 0);
            this.pnlControls.Name = "pnlControls";
            this.pnlControls.Size = new System.Drawing.Size(924, 30);
            this.pnlControls.TabIndex = 1;
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(30, 28);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripbtnOK,
            this.toolStripbtnRefresh,
            this.toolStripbtnCtlCaptureOnMove,
            this.toolStripButton_SaveOM,
            this.toolStripButtonEditOM,
            this.toolStripButtonTakeSnap,
            this.toolStripComboBox1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(922, 28);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip1";
            this.toolStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStrip1_ItemClicked);
            // 
            // pnlControlTree
            // 
            this.pnlControlTree.Controls.Add(this.splitContainer1);
            this.pnlControlTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlControlTree.Location = new System.Drawing.Point(0, 30);
            this.pnlControlTree.Name = "pnlControlTree";
            this.pnlControlTree.Size = new System.Drawing.Size(924, 486);
            this.pnlControlTree.TabIndex = 2;
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
            this.splitContainer1.Size = new System.Drawing.Size(924, 486);
            this.splitContainer1.SplitterDistance = 551;
            this.splitContainer1.TabIndex = 1;
            // 
            // trAppControl
            // 
            this.trAppControl.CheckBoxes = true;
            this.trAppControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trAppControl.ImageIndex = 0;
            this.trAppControl.ImageList = this.imageList1;
            this.trAppControl.ItemHeight = 24;
            this.trAppControl.Location = new System.Drawing.Point(0, 0);
            this.trAppControl.Name = "trAppControl";
            this.trAppControl.SelectedImageIndex = 0;
            this.trAppControl.Size = new System.Drawing.Size(551, 486);
            this.trAppControl.TabIndex = 0;
            this.trAppControl.TabStop = false;
            this.trAppControl.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.trAppControl_AfterCheck);
            this.trAppControl.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.trAppControl_AfterExpand);
            this.trAppControl.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trAppControl_AfterSelect);
            this.trAppControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.trAppControl_MouseDown);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "node.jpg");
            // 
            // imageList2
            // 
            this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
            this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList2.Images.SetKeyName(0, "control-capture.jpg");
            this.imageList2.Images.SetKeyName(1, "stop-control-capture.jpg");
            this.imageList2.Images.SetKeyName(2, "control-capture - move.jpg");
            this.imageList2.Images.SetKeyName(3, "stop-control-capture -move.jpg");
            this.imageList2.Images.SetKeyName(4, "camera.png");
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // toolStripComboBox1
            // 
            this.toolStripComboBox1.DropDownWidth = 121;
            this.toolStripComboBox1.Items.AddRange(new object[] {
            "Normal",
            "Web"});
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            this.toolStripComboBox1.Size = new System.Drawing.Size(121, 28);
            this.toolStripComboBox1.Text = "---Select Mode---";
            // 
            // toolStripbtnOK
            // 
            this.toolStripbtnOK.AutoSize = false;
            this.toolStripbtnOK.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripbtnOK.Image = ((System.Drawing.Image)(resources.GetObject("toolStripbtnOK.Image")));
            this.toolStripbtnOK.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripbtnOK.Name = "toolStripbtnOK";
            this.toolStripbtnOK.Size = new System.Drawing.Size(25, 25);
            this.toolStripbtnOK.Text = "Use the control highlighted";
            this.toolStripbtnOK.Click += new System.EventHandler(this.toolStripbtnOK_Click);
            // 
            // toolStripbtnRefresh
            // 
            this.toolStripbtnRefresh.AutoSize = false;
            this.toolStripbtnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripbtnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("toolStripbtnRefresh.Image")));
            this.toolStripbtnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripbtnRefresh.Name = "toolStripbtnRefresh";
            this.toolStripbtnRefresh.Size = new System.Drawing.Size(28, 25);
            this.toolStripbtnRefresh.Text = "Refresh the control explorer";
            this.toolStripbtnRefresh.Click += new System.EventHandler(this.toolStripbtnRefresh_Click);
            // 
            // toolStripbtnCtlCaptureOnMove
            // 
            this.toolStripbtnCtlCaptureOnMove.AutoSize = false;
            this.toolStripbtnCtlCaptureOnMove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripbtnCtlCaptureOnMove.Image = ((System.Drawing.Image)(resources.GetObject("toolStripbtnCtlCaptureOnMove.Image")));
            this.toolStripbtnCtlCaptureOnMove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripbtnCtlCaptureOnMove.Name = "toolStripbtnCtlCaptureOnMove";
            this.toolStripbtnCtlCaptureOnMove.Size = new System.Drawing.Size(34, 30);
            this.toolStripbtnCtlCaptureOnMove.Text = "Start capturing the control details from application. Press \'SHIFT + move Mouse\' " +
                "on the control";
            this.toolStripbtnCtlCaptureOnMove.Click += new System.EventHandler(this.toolStripbtnCtlCaptureOnMove_Click);
            // 
            // toolStripButton_SaveOM
            // 
            this.toolStripButton_SaveOM.AutoSize = false;
            this.toolStripButton_SaveOM.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_SaveOM.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_SaveOM.Image")));
            this.toolStripButton_SaveOM.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_SaveOM.Name = "toolStripButton_SaveOM";
            this.toolStripButton_SaveOM.Size = new System.Drawing.Size(25, 25);
            this.toolStripButton_SaveOM.Text = "Save Application Object Model(s) for the selected node(s)";
            this.toolStripButton_SaveOM.Click += new System.EventHandler(this.toolStripButton_SaveOM_Click);
            // 
            // toolStripButtonEditOM
            // 
            this.toolStripButtonEditOM.AutoSize = false;
            this.toolStripButtonEditOM.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonEditOM.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonEditOM.Image")));
            this.toolStripButtonEditOM.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonEditOM.Name = "toolStripButtonEditOM";
            this.toolStripButtonEditOM.Size = new System.Drawing.Size(25, 25);
            this.toolStripButtonEditOM.Text = "Edit an Object Model";
            this.toolStripButtonEditOM.Click += new System.EventHandler(this.toolStripButtonEditOM_Click);
            // 
            // toolStripButtonTakeSnap
            // 
            this.toolStripButtonTakeSnap.AutoSize = false;
            this.toolStripButtonTakeSnap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonTakeSnap.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonTakeSnap.Image")));
            this.toolStripButtonTakeSnap.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonTakeSnap.Name = "toolStripButtonTakeSnap";
            this.toolStripButtonTakeSnap.Size = new System.Drawing.Size(25, 25);
            this.toolStripButtonTakeSnap.Text = "Take Snap";
            this.toolStripButtonTakeSnap.Click += new System.EventHandler(this.toolStripButtonTakeSnap_Click);
            // 
            // ControlExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(924, 516);
            this.Controls.Add(this.pnlControlTree);
            this.Controls.Add(this.pnlControls);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ControlExplorer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Control Explorer";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ControlExplorer_FormClosed);
            this.Load += new System.EventHandler(this.ControlExplorer_Load);
            this.pnlControls.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.pnlControlTree.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlControls;
        private System.Windows.Forms.Panel pnlControlTree;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TreeView trAppControl;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripbtnOK;
        private System.Windows.Forms.ToolStripButton toolStripbtnRefresh;
        private System.Windows.Forms.ImageList imageList2;
        private System.Windows.Forms.ToolStripButton toolStripbtnCtlCaptureOnMove;
        private System.Windows.Forms.ToolStripButton toolStripButton_SaveOM;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ToolStripButton toolStripButtonEditOM;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripButton toolStripButtonTakeSnap;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
    }
}
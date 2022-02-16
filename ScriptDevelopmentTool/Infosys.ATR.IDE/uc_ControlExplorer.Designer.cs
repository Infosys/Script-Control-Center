namespace Infosys.ATR.DevelopmentStudio
{
    partial class uc_ControlExplorer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uc_ControlExplorer));
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.pnlControlTree = new System.Windows.Forms.Panel();
            this.trAppControl = new System.Windows.Forms.TreeView();
            this.pnlControlTree.SuspendLayout();
            this.SuspendLayout();
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
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "node.jpg");
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // pnlControlTree
            // 
            this.pnlControlTree.Controls.Add(this.trAppControl);
            this.pnlControlTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlControlTree.Location = new System.Drawing.Point(0, 0);
            this.pnlControlTree.Name = "pnlControlTree";
            this.pnlControlTree.Size = new System.Drawing.Size(736, 463);
            this.pnlControlTree.TabIndex = 4;
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
            this.trAppControl.Size = new System.Drawing.Size(736, 463);
            this.trAppControl.TabIndex = 1;
            this.trAppControl.TabStop = false;
            this.trAppControl.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.trAppControl_AfterCheck);
            this.trAppControl.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.trAppControl_AfterExpand);
            this.trAppControl.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trAppControl_AfterSelect);
            // 
            // uc_ControlExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlControlTree);
            this.Name = "uc_ControlExplorer";
            this.Size = new System.Drawing.Size(736, 463);
            this.Load += new System.EventHandler(this.ControlExplorer_Load);
            this.pnlControlTree.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList imageList2;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Panel pnlControlTree;
        private System.Windows.Forms.TreeView trAppControl;
    }
}

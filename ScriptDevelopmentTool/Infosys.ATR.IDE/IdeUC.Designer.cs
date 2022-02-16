namespace Infosys.ATR.DevelopmentStudio
{
    partial class IdeUC
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
            this.pnlControls = new System.Windows.Forms.Panel();
            this.pnlBody = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lblFuns = new System.Windows.Forms.Label();
            this.rtxtEditor = new System.Windows.Forms.RichTextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pnlBody.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlControls
            // 
            this.pnlControls.Location = new System.Drawing.Point(0, 0);
            this.pnlControls.Name = "pnlControls";
            this.pnlControls.Size = new System.Drawing.Size(200, 100);
            this.pnlControls.TabIndex = 2;
            // 
            // pnlBody
            // 
            this.pnlBody.Controls.Add(this.splitContainer1);
            this.pnlBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBody.Location = new System.Drawing.Point(0, 0);
            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Size = new System.Drawing.Size(693, 545);
            this.pnlBody.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.Window;
            this.splitContainer1.Panel1.Controls.Add(this.lblFuns);
            this.splitContainer1.Panel1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.splitContainer1_Panel1_MouseDoubleClick);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.rtxtEditor);
            this.splitContainer1.Size = new System.Drawing.Size(693, 545);
            this.splitContainer1.SplitterDistance = 168;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            this.splitContainer1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.splitContainer1_MouseDoubleClick);
            // 
            // lblFuns
            // 
            this.lblFuns.AutoSize = true;
            this.lblFuns.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFuns.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblFuns.Location = new System.Drawing.Point(3, 4);
            this.lblFuns.Name = "lblFuns";
            this.lblFuns.Size = new System.Drawing.Size(74, 16);
            this.lblFuns.TabIndex = 0;
            this.lblFuns.Text = "Functions";
            // 
            // rtxtEditor
            // 
            this.rtxtEditor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtxtEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtxtEditor.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtxtEditor.Location = new System.Drawing.Point(0, 0);
            this.rtxtEditor.Name = "rtxtEditor";
            this.rtxtEditor.Size = new System.Drawing.Size(520, 545);
            this.rtxtEditor.TabIndex = 0;
            this.rtxtEditor.Text = "";
            // 
            // IdeUC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.pnlControls);
            this.Name = "IdeUC";
            this.Size = new System.Drawing.Size(693, 545);
            this.Load += new System.EventHandler(this.IDE_Load);
            this.pnlBody.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlControls;
        private System.Windows.Forms.Panel pnlBody;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label lblFuns;
        private System.Windows.Forms.RichTextBox rtxtEditor;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripbtnGetCtl;
        private System.Windows.Forms.ToolStripButton toolStripbtnRun;
        private System.Windows.Forms.ToolStripButton toolStripbtnSave;
    }
}

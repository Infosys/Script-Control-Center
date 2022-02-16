namespace Infosys.ATR.Editor.Views
{
    partial class frmReplay
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
            this.pnlNavigation = new System.Windows.Forms.Panel();
            this.lnkPrevious = new System.Windows.Forms.LinkLabel();
            this.lnkNext = new System.Windows.Forms.LinkLabel();
            this.pnlTitle = new System.Windows.Forms.Panel();
            this.lblState = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pnlNavigation.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlNavigation
            // 
            this.pnlNavigation.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pnlNavigation.Controls.Add(this.lnkPrevious);
            this.pnlNavigation.Controls.Add(this.lnkNext);
            this.pnlNavigation.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlNavigation.Location = new System.Drawing.Point(0, 437);
            this.pnlNavigation.Name = "pnlNavigation";
            this.pnlNavigation.Size = new System.Drawing.Size(752, 26);
            this.pnlNavigation.TabIndex = 0;
            // 
            // lnkPrevious
            // 
            this.lnkPrevious.AutoSize = true;
            this.lnkPrevious.Location = new System.Drawing.Point(639, 5);
            this.lnkPrevious.Name = "lnkPrevious";
            this.lnkPrevious.Size = new System.Drawing.Size(48, 13);
            this.lnkPrevious.TabIndex = 1;
            this.lnkPrevious.TabStop = true;
            this.lnkPrevious.Text = "Previous";
            this.lnkPrevious.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkPrevious_LinkClicked);
            // 
            // lnkNext
            // 
            this.lnkNext.AutoSize = true;
            this.lnkNext.Location = new System.Drawing.Point(691, 5);
            this.lnkNext.Name = "lnkNext";
            this.lnkNext.Size = new System.Drawing.Size(29, 13);
            this.lnkNext.TabIndex = 0;
            this.lnkNext.TabStop = true;
            this.lnkNext.Text = "Next";
            this.lnkNext.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkNext_LinkClicked);
            // 
            // pnlTitle
            // 
            this.pnlTitle.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pnlTitle.Controls.Add(this.lblState);
            this.pnlTitle.Controls.Add(this.lblTitle);
            this.pnlTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTitle.Location = new System.Drawing.Point(0, 0);
            this.pnlTitle.Name = "pnlTitle";
            this.pnlTitle.Size = new System.Drawing.Size(752, 27);
            this.pnlTitle.TabIndex = 1;
            // 
            // lblState
            // 
            this.lblState.Location = new System.Drawing.Point(505, 0);
            this.lblState.Name = "lblState";
            this.lblState.Size = new System.Drawing.Size(215, 18);
            this.lblState.TabIndex = 1;
            this.lblState.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTitle
            // 
            this.lblTitle.Location = new System.Drawing.Point(4, 5);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(215, 18);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 27);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(752, 410);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // frmReplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(752, 463);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.pnlTitle);
            this.Controls.Add(this.pnlNavigation);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmReplay";
            this.Text = "Replay";
            this.pnlNavigation.ResumeLayout(false);
            this.pnlNavigation.PerformLayout();
            this.pnlTitle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlNavigation;
        private System.Windows.Forms.Panel pnlTitle;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.LinkLabel lnkPrevious;
        private System.Windows.Forms.LinkLabel lnkNext;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblState;
    }
}
namespace Infosys.ATR.UIAutomation.Recorder
{
    partial class TaskViewItem
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TaskViewItem));
            this.lblDesc = new System.Windows.Forms.Label();
            this.pbCapture = new System.Windows.Forms.PictureBox();
            this.pbDelete = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbCapture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbDelete)).BeginInit();
            this.SuspendLayout();
            // 
            // lblDesc
            // 
            this.lblDesc.AutoSize = true;
            this.lblDesc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDesc.Location = new System.Drawing.Point(20, 0);
            this.lblDesc.Name = "lblDesc";
            this.lblDesc.Size = new System.Drawing.Size(35, 13);
            this.lblDesc.TabIndex = 1;
            this.lblDesc.Text = "label1";
            this.lblDesc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblDesc.Click += new System.EventHandler(this.lblDesc_Click);
            // 
            // pbCapture
            // 
            this.pbCapture.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pbCapture.BackgroundImage")));
            this.pbCapture.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pbCapture.Dock = System.Windows.Forms.DockStyle.Right;
            this.pbCapture.Location = new System.Drawing.Point(319, 0);
            this.pbCapture.Name = "pbCapture";
            this.pbCapture.Size = new System.Drawing.Size(20, 19);
            this.pbCapture.TabIndex = 2;
            this.pbCapture.TabStop = false;
            this.pbCapture.Click += new System.EventHandler(this.pbCapture_Click);
            // 
            // pbDelete
            // 
            this.pbDelete.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pbDelete.BackgroundImage")));
            this.pbDelete.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pbDelete.Dock = System.Windows.Forms.DockStyle.Left;
            this.pbDelete.Location = new System.Drawing.Point(0, 0);
            this.pbDelete.Name = "pbDelete";
            this.pbDelete.Size = new System.Drawing.Size(20, 19);
            this.pbDelete.TabIndex = 0;
            this.pbDelete.TabStop = false;
            this.pbDelete.Click += new System.EventHandler(this.pbDelete_Click);
            // 
            // TaskViewItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.pbCapture);
            this.Controls.Add(this.lblDesc);
            this.Controls.Add(this.pbDelete);
            this.Name = "TaskViewItem";
            this.Size = new System.Drawing.Size(339, 19);
            ((System.ComponentModel.ISupportInitialize)(this.pbCapture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbDelete)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbDelete;
        private System.Windows.Forms.Label lblDesc;
        private System.Windows.Forms.PictureBox pbCapture;
    }
}

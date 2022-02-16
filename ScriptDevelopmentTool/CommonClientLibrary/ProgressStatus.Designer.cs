namespace Infosys.IAP.CommonClientLibrary
{
    partial class ProgressStatus
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
            this.uclProgressStatus1 = new Infosys.IAP.CommonClientLibrary.uclProgressStatus();
            this.SuspendLayout();
            // 
            // uclProgressStatus1
            // 
            this.uclProgressStatus1.BackColor = System.Drawing.SystemColors.Window;
            this.uclProgressStatus1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.uclProgressStatus1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uclProgressStatus1.ForeColor = System.Drawing.SystemColors.Highlight;
            this.uclProgressStatus1.Location = new System.Drawing.Point(0, 0);
            this.uclProgressStatus1.Name = "uclProgressStatus1";
            this.uclProgressStatus1.Size = new System.Drawing.Size(485, 26);
            this.uclProgressStatus1.TabIndex = 0;
            this.uclProgressStatus1.Value = 0F;
            // 
            // ProgressStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(485, 26);
            this.ControlBox = false;
            this.Controls.Add(this.uclProgressStatus1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressStatus";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Generating Playback Script.......";
            this.ResumeLayout(false);

        }

        #endregion

        private uclProgressStatus uclProgressStatus1;
    }
}
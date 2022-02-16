namespace ScriptRepository
{
    partial class Form1
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
            this.mainRepositoryView1 = new Infosys.ATR.ScriptRepository.Views.MainRepositoryView();
            this.SuspendLayout();
            // 
            // mainRepositoryView1
            // 
            this.mainRepositoryView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainRepositoryView1.Location = new System.Drawing.Point(0, 0);
            this.mainRepositoryView1.Name = "mainRepositoryView1";
            this.mainRepositoryView1.Size = new System.Drawing.Size(735, 506);
            this.mainRepositoryView1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(735, 506);
            this.Controls.Add(this.mainRepositoryView1);
            this.Name = "Form1";
            this.Text = "IAP Script Repository";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Infosys.ATR.ScriptRepository.Views.MainRepositoryView mainRepositoryView1;

    }
}


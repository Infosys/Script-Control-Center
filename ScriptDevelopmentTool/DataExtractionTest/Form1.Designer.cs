namespace DataExtractionTest
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
            this.btnApp = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAppEv = new System.Windows.Forms.Button();
            this.btnAppRl = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnApp
            // 
            this.btnApp.Location = new System.Drawing.Point(35, 42);
            this.btnApp.Name = "btnApp";
            this.btnApp.Size = new System.Drawing.Size(75, 23);
            this.btnApp.TabIndex = 0;
            this.btnApp.Text = "Applications";
            this.btnApp.UseVisualStyleBackColor = true;
            this.btnApp.Click += new System.EventHandler(this.btnApp_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Data Extraction Test";
            // 
            // btnAppEv
            // 
            this.btnAppEv.Location = new System.Drawing.Point(35, 80);
            this.btnAppEv.Name = "btnAppEv";
            this.btnAppEv.Size = new System.Drawing.Size(113, 23);
            this.btnAppEv.TabIndex = 2;
            this.btnAppEv.Text = "Application Events";
            this.btnAppEv.UseVisualStyleBackColor = true;
            this.btnAppEv.Click += new System.EventHandler(this.btnAppEv_Click);
            // 
            // btnAppRl
            // 
            this.btnAppRl.Location = new System.Drawing.Point(35, 119);
            this.btnAppRl.Name = "btnAppRl";
            this.btnAppRl.Size = new System.Drawing.Size(128, 23);
            this.btnAppRl.TabIndex = 3;
            this.btnAppRl.Text = "Application Relations";
            this.btnAppRl.UseVisualStyleBackColor = true;
            this.btnAppRl.Click += new System.EventHandler(this.btnAppRl_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 173);
            this.Controls.Add(this.btnAppRl);
            this.Controls.Add(this.btnAppEv);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnApp);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnApp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnAppEv;
        private System.Windows.Forms.Button btnAppRl;
    }
}


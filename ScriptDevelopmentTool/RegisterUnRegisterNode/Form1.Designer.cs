namespace RegisterUnRegisterNode
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
            this.btnRegister = new System.Windows.Forms.Button();
            this.btnUnRegister = new System.Windows.Forms.Button();
            this.btnGetSysInfo = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnRegister
            // 
            this.btnRegister.Location = new System.Drawing.Point(106, 74);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(75, 23);
            this.btnRegister.TabIndex = 0;
            this.btnRegister.Text = "Register";
            this.btnRegister.UseVisualStyleBackColor = true;
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);
            // 
            // btnUnRegister
            // 
            this.btnUnRegister.Location = new System.Drawing.Point(106, 131);
            this.btnUnRegister.Name = "btnUnRegister";
            this.btnUnRegister.Size = new System.Drawing.Size(75, 23);
            this.btnUnRegister.TabIndex = 1;
            this.btnUnRegister.Text = "Un Register";
            this.btnUnRegister.UseVisualStyleBackColor = true;
            this.btnUnRegister.Click += new System.EventHandler(this.btnUnRegister_Click);
            // 
            // btnGetSysInfo
            // 
            this.btnGetSysInfo.Location = new System.Drawing.Point(106, 182);
            this.btnGetSysInfo.Name = "btnGetSysInfo";
            this.btnGetSysInfo.Size = new System.Drawing.Size(75, 23);
            this.btnGetSysInfo.TabIndex = 2;
            this.btnGetSysInfo.Text = "Get Sys Info";
            this.btnGetSysInfo.UseVisualStyleBackColor = true;
            this.btnGetSysInfo.Click += new System.EventHandler(this.btnGetSysInfo_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.btnGetSysInfo);
            this.Controls.Add(this.btnUnRegister);
            this.Controls.Add(this.btnRegister);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnRegister;
        private System.Windows.Forms.Button btnUnRegister;
        private System.Windows.Forms.Button btnGetSysInfo;
    }
}


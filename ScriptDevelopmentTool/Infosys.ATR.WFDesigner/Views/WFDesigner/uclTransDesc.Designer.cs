namespace Infosys.ATR.WFDesigner.Views
{
    partial class uclTransDesc
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
            this.grpTransDesc = new System.Windows.Forms.GroupBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.grpTransDesc.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpTransDesc
            // 
            this.grpTransDesc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpTransDesc.Controls.Add(this.txtDescription);
            this.grpTransDesc.Location = new System.Drawing.Point(9, 9);
            this.grpTransDesc.Name = "grpTransDesc";
            this.grpTransDesc.Size = new System.Drawing.Size(406, 74);
            this.grpTransDesc.TabIndex = 0;
            this.grpTransDesc.TabStop = false;
            this.grpTransDesc.Text = "Transaction Description";
            // 
            // txtDescription
            // 
            this.txtDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDescription.Location = new System.Drawing.Point(3, 16);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDescription.Size = new System.Drawing.Size(400, 55);
            this.txtDescription.TabIndex = 9;
            // 
            // uclTransDesc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpTransDesc);
            this.Name = "uclTransDesc";
            this.Size = new System.Drawing.Size(429, 88);
            this.grpTransDesc.ResumeLayout(false);
            this.grpTransDesc.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpTransDesc;
        private System.Windows.Forms.TextBox txtDescription;
    }
}

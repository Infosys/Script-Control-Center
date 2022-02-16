namespace Infosys.ATR.WFDesigner.Views.Transaction
{
    partial class uclTransStatus
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
            this.pnlTransStatus = new System.Windows.Forms.Panel();
            this.btnTansNumber = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblTransstatus = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pnlTransStatus.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTransStatus
            // 
            this.pnlTransStatus.AutoSize = true;
            this.pnlTransStatus.Controls.Add(this.btnTansNumber);
            this.pnlTransStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTransStatus.Location = new System.Drawing.Point(0, 0);
            this.pnlTransStatus.Name = "pnlTransStatus";
            this.pnlTransStatus.Size = new System.Drawing.Size(276, 236);
            this.pnlTransStatus.TabIndex = 0;
            // 
            // btnTansNumber
            // 
            this.btnTansNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTansNumber.AutoSize = true;
            this.btnTansNumber.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnTansNumber.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTansNumber.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnTansNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 72F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTansNumber.Location = new System.Drawing.Point(0, 0);
            this.btnTansNumber.Name = "btnTansNumber";
            this.btnTansNumber.Size = new System.Drawing.Size(276, 184);
            this.btnTansNumber.TabIndex = 0;
            this.btnTansNumber.Text = "20";
            this.btnTansNumber.UseVisualStyleBackColor = true;
            this.btnTansNumber.Click += new System.EventHandler(this.btnTansNumber_Click);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.AutoSize = true;
            this.panel2.Controls.Add(this.lblTransstatus);
            this.panel2.Location = new System.Drawing.Point(0, 195);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(276, 41);
            this.panel2.TabIndex = 1;
            // 
            // lblTransstatus
            // 
            this.lblTransstatus.AutoSize = true;
            this.lblTransstatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTransstatus.Location = new System.Drawing.Point(8, 5);
            this.lblTransstatus.Name = "lblTransstatus";
            this.lblTransstatus.Size = new System.Drawing.Size(79, 25);
            this.lblTransstatus.TabIndex = 0;
            this.lblTransstatus.Text = "Status";
            this.lblTransstatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uclTransStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.pnlTransStatus);
            this.Name = "uclTransStatus";
            this.Size = new System.Drawing.Size(276, 236);
            this.pnlTransStatus.ResumeLayout(false);
            this.pnlTransStatus.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlTransStatus;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblTransstatus;
        private System.Windows.Forms.Button btnTansNumber;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

namespace Infosys.ATR.AutomationClient
{
    partial class ucResultItem
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
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblNode = new System.Windows.Forms.Label();
            this.lblMessage = new System.Windows.Forms.Label();
            this.txtCompleteMsg = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(118, 8);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(37, 13);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "Status";
            // 
            // lblNode
            // 
            this.lblNode.AutoSize = true;
            this.lblNode.Location = new System.Drawing.Point(3, 7);
            this.lblNode.Name = "lblNode";
            this.lblNode.Size = new System.Drawing.Size(33, 13);
            this.lblNode.TabIndex = 1;
            this.lblNode.Text = "Node";
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(214, 8);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(50, 13);
            this.lblMessage.TabIndex = 2;
            this.lblMessage.Text = "Message";
            this.toolTip1.SetToolTip(this.lblMessage, "Double click to expand/collapse full message");
            this.lblMessage.DoubleClick += new System.EventHandler(this.lblMessage_DoubleClick);
            // 
            // txtCompleteMsg
            // 
            this.txtCompleteMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCompleteMsg.Location = new System.Drawing.Point(0, 38);
            this.txtCompleteMsg.Multiline = true;
            this.txtCompleteMsg.Name = "txtCompleteMsg";
            this.txtCompleteMsg.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtCompleteMsg.Size = new System.Drawing.Size(607, 0);
            this.txtCompleteMsg.TabIndex = 3;
            // 
            // ucResultItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.txtCompleteMsg);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.lblNode);
            this.Controls.Add(this.lblStatus);
            this.Name = "ucResultItem";
            this.Size = new System.Drawing.Size(607, 40);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblNode;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.TextBox txtCompleteMsg;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

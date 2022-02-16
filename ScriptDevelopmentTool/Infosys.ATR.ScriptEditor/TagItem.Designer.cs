namespace Infosys.ATR.ScriptEditor
{
    partial class TagItem
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
            this.txtAddTag = new System.Windows.Forms.TextBox();
            this.pnlTags = new System.Windows.Forms.Panel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // txtAddTag
            // 
            this.txtAddTag.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAddTag.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtAddTag.Location = new System.Drawing.Point(0, 0);
            this.txtAddTag.Name = "txtAddTag";
            this.txtAddTag.Size = new System.Drawing.Size(289, 20);
            this.txtAddTag.TabIndex = 0;
            this.txtAddTag.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtNewTag_KeyPress);
            // 
            // pnlTags
            // 
            this.pnlTags.AutoScroll = true;
            this.pnlTags.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTags.Location = new System.Drawing.Point(0, 20);
            this.pnlTags.Name = "pnlTags";
            this.pnlTags.Size = new System.Drawing.Size(289, 55);
            this.pnlTags.TabIndex = 1;
            // 
            // TagItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.pnlTags);
            this.Controls.Add(this.txtAddTag);
            this.Name = "TagItem";
            this.Size = new System.Drawing.Size(289, 75);
            this.Load += new System.EventHandler(this.TagItem_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtAddTag;
        private System.Windows.Forms.Panel pnlTags;
        private System.Windows.Forms.ToolTip toolTip1;


    }
}

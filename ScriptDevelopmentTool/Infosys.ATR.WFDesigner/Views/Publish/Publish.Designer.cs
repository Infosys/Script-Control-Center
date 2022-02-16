namespace Infosys.ATR.WFDesigner.Views
{
    partial class Publish
    {

        public PublishPresenter _presenter = null;

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
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.btnPublsh = new System.Windows.Forms.Button();
            this.lblCategory = new System.Windows.Forms.Label();
            this.trCat = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(56, 243);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(35, 13);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Name";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(153, 243);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(312, 20);
            this.txtName.TabIndex = 1;
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(56, 290);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(60, 13);
            this.lblDescription.TabIndex = 2;
            this.lblDescription.Text = "Description";
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(153, 287);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDescription.Size = new System.Drawing.Size(312, 79);
            this.txtDescription.TabIndex = 3;
            // 
            // btnPublsh
            // 
            this.btnPublsh.Location = new System.Drawing.Point(390, 388);
            this.btnPublsh.Name = "btnPublsh";
            this.btnPublsh.Size = new System.Drawing.Size(75, 23);
            this.btnPublsh.TabIndex = 12;
            this.btnPublsh.Text = "Publish";
            this.btnPublsh.UseVisualStyleBackColor = true;
            this.btnPublsh.Click += new System.EventHandler(this.btnPublsh_Click);
            // 
            // lblCategory
            // 
            this.lblCategory.AutoSize = true;
            this.lblCategory.Location = new System.Drawing.Point(56, 33);
            this.lblCategory.Name = "lblCategory";
            this.lblCategory.Size = new System.Drawing.Size(82, 13);
            this.lblCategory.TabIndex = 13;
            this.lblCategory.Text = "Select Category";
            // 
            // trCat
            // 
            this.trCat.Location = new System.Drawing.Point(153, 33);
            this.trCat.Name = "trCat";
            this.trCat.Size = new System.Drawing.Size(312, 186);
            this.trCat.TabIndex = 14;
            this.trCat.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trCat_AfterSelect);
            // 
            // Publish
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.trCat);
            this.Controls.Add(this.lblCategory);
            this.Controls.Add(this.btnPublsh);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblName);
            this.Name = "Publish";
            this.Size = new System.Drawing.Size(586, 451);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Button btnPublsh;
        private System.Windows.Forms.Label lblCategory;
        private System.Windows.Forms.TreeView trCat;
    }
}

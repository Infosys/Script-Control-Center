﻿namespace Infosys.ATR.ScriptEditor
{
    partial class UseCaseView
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtDesc = new System.Windows.Forms.TextBox();
            this.txtCreatedBy = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCreatedOn = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtMachineName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtOS = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtDomain = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtMachineType = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lblEnd = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.pnlTags = new System.Windows.Forms.Panel();
            this.Tag = new System.Windows.Forms.Label();
            this.lblTags = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 75);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Description";
            // 
            // txtDesc
            // 
            this.txtDesc.Location = new System.Drawing.Point(132, 72);
            this.txtDesc.Multiline = true;
            this.txtDesc.Name = "txtDesc";
            this.txtDesc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDesc.Size = new System.Drawing.Size(163, 50);
            this.txtDesc.TabIndex = 1;
            this.txtDesc.TextChanged += new System.EventHandler(this.txtDesc_TextChanged);
            // 
            // txtCreatedBy
            // 
            this.txtCreatedBy.Location = new System.Drawing.Point(132, 127);
            this.txtCreatedBy.Name = "txtCreatedBy";
            this.txtCreatedBy.Size = new System.Drawing.Size(163, 20);
            this.txtCreatedBy.TabIndex = 3;
            this.txtCreatedBy.TextChanged += new System.EventHandler(this.txtCreatedBy_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 130);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Created By ";
            // 
            // txtCreatedOn
            // 
            this.txtCreatedOn.Location = new System.Drawing.Point(132, 153);
            this.txtCreatedOn.Name = "txtCreatedOn";
            this.txtCreatedOn.ReadOnly = true;
            this.txtCreatedOn.Size = new System.Drawing.Size(163, 20);
            this.txtCreatedOn.TabIndex = 5;
            this.txtCreatedOn.TextChanged += new System.EventHandler(this.txtCreatedOn_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 156);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Created On";
            // 
            // txtMachineName
            // 
            this.txtMachineName.Location = new System.Drawing.Point(480, 45);
            this.txtMachineName.Name = "txtMachineName";
            this.txtMachineName.Size = new System.Drawing.Size(163, 20);
            this.txtMachineName.TabIndex = 7;
            this.txtMachineName.TextChanged += new System.EventHandler(this.txtMachineName_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(351, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Machine Name";
            // 
            // txtIP
            // 
            this.txtIP.Location = new System.Drawing.Point(480, 71);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(163, 20);
            this.txtIP.TabIndex = 9;
            this.txtIP.TextChanged += new System.EventHandler(this.txtIP_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(351, 74);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Machine IP";
            // 
            // txtOS
            // 
            this.txtOS.Location = new System.Drawing.Point(480, 97);
            this.txtOS.Name = "txtOS";
            this.txtOS.Size = new System.Drawing.Size(163, 20);
            this.txtOS.TabIndex = 11;
            this.txtOS.TextChanged += new System.EventHandler(this.txtOS_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(351, 100);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(66, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Machine OS";
            // 
            // txtDomain
            // 
            this.txtDomain.Location = new System.Drawing.Point(480, 123);
            this.txtDomain.Name = "txtDomain";
            this.txtDomain.Size = new System.Drawing.Size(163, 20);
            this.txtDomain.TabIndex = 13;
            this.txtDomain.TextChanged += new System.EventHandler(this.txtDomain_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(351, 126);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(87, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Machine Domain";
            // 
            // txtMachineType
            // 
            this.txtMachineType.Location = new System.Drawing.Point(480, 149);
            this.txtMachineType.Name = "txtMachineType";
            this.txtMachineType.Size = new System.Drawing.Size(163, 20);
            this.txtMachineType.TabIndex = 15;
            this.txtMachineType.TextChanged += new System.EventHandler(this.txtMachineType_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(351, 152);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(75, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "Machine Type";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(568, 296);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 16;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(132, 46);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(163, 20);
            this.txtName.TabIndex = 18;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 49);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(35, 13);
            this.label9.TabIndex = 17;
            this.label9.Text = "Name";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label10.Location = new System.Drawing.Point(3, 12);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(131, 16);
            this.label10.TabIndex = 19;
            this.label10.Text = "Use Case details:";
            // 
            // lblEnd
            // 
            this.lblEnd.AutoSize = true;
            this.lblEnd.Location = new System.Drawing.Point(1, 298);
            this.lblEnd.Name = "lblEnd";
            this.lblEnd.Size = new System.Drawing.Size(10, 13);
            this.lblEnd.TabIndex = 20;
            this.lblEnd.Text = ".";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.Maroon;
            this.label11.Location = new System.Drawing.Point(188, 299);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(374, 13);
            this.label11.TabIndex = 21;
            this.label11.Text = "Click \"Save\" to persist the changes and the order of the activities (if changed)";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(-1, 252);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(136, 13);
            this.label12.TabIndex = 71;
            this.label12.Text = "text box and press ENTER)";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(1, 239);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(126, 13);
            this.label13.TabIndex = 70;
            this.label13.Text = "(Provide new Tag in the  ";
            // 
            // pnlTags
            // 
            this.pnlTags.AutoScroll = true;
            this.pnlTags.BackColor = System.Drawing.SystemColors.Window;
            this.pnlTags.Location = new System.Drawing.Point(135, 214);
            this.pnlTags.Name = "pnlTags";
            this.pnlTags.Size = new System.Drawing.Size(508, 55);
            this.pnlTags.TabIndex = 69;
            // 
            // Tag
            // 
            this.Tag.AutoSize = true;
            this.Tag.Location = new System.Drawing.Point(3, 225);
            this.Tag.Name = "Tag";
            this.Tag.Size = new System.Drawing.Size(34, 13);
            this.Tag.TabIndex = 68;
            this.Tag.Text = "Tags ";
            // 
            // lblTags
            // 
            this.lblTags.AutoSize = true;
            this.lblTags.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTags.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblTags.Location = new System.Drawing.Point(3, 204);
            this.lblTags.Name = "lblTags";
            this.lblTags.Size = new System.Drawing.Size(120, 16);
            this.lblTags.TabIndex = 67;
            this.lblTags.Text = "Use Case Tags:";
            // 
            // UseCaseView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.pnlTags);
            this.Controls.Add(this.Tag);
            this.Controls.Add(this.lblTags);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.lblEnd);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtMachineType);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtDomain);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtOS);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtIP);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtMachineName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtCreatedOn);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtCreatedBy);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtDesc);
            this.Controls.Add(this.label1);
            this.Name = "UseCaseView";
            this.Size = new System.Drawing.Size(650, 326);
            this.Load += new System.EventHandler(this.UseCaseView_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtDesc;
        private System.Windows.Forms.TextBox txtCreatedBy;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtCreatedOn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtMachineName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtOS;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtDomain;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtMachineType;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblEnd;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Panel pnlTags;
        private System.Windows.Forms.Label Tag;
        private System.Windows.Forms.Label lblTags;
    }
}
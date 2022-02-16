namespace Infosys.ATR.ScriptEditor
{
    partial class ActivityView
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ActivityView));
            this.label10 = new System.Windows.Forms.Label();
            this.txtActId = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtParentId = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtDesc = new System.Windows.Forms.TextBox();
            this.lblDesc = new System.Windows.Forms.Label();
            this.txtAppName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtAppExe = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblTasks = new System.Windows.Forms.Label();
            this.dgTasks = new System.Windows.Forms.DataGridView();
            this.Delete = new System.Windows.Forms.DataGridViewButtonColumn();
            this.txtControlId = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtTaskName = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.lblSelectedTask = new System.Windows.Forms.Label();
            this.btnAddTask = new System.Windows.Forms.Button();
            this.txtTaskDesc = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtWinTitle = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtTaskGroupId = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtXPath = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.pic = new System.Windows.Forms.PictureBox();
            this.btnCrop = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.txtTaskText = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.lblEnd = new System.Windows.Forms.Label();
            this.lblTags = new System.Windows.Forms.Label();
            this.Tag = new System.Windows.Forms.Label();
            this.pnlTags = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgTasks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label10.Location = new System.Drawing.Point(3, 12);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(113, 16);
            this.label10.TabIndex = 20;
            this.label10.Text = "Activity details:";
            // 
            // txtActId
            // 
            this.txtActId.Location = new System.Drawing.Point(132, 47);
            this.txtActId.Name = "txtActId";
            this.txtActId.ReadOnly = true;
            this.txtActId.Size = new System.Drawing.Size(163, 20);
            this.txtActId.TabIndex = 22;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 50);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 13);
            this.label9.TabIndex = 21;
            this.label9.Text = "Activity";
            // 
            // txtParentId
            // 
            this.txtParentId.Location = new System.Drawing.Point(132, 73);
            this.txtParentId.Name = "txtParentId";
            this.txtParentId.ReadOnly = true;
            this.txtParentId.Size = new System.Drawing.Size(163, 20);
            this.txtParentId.TabIndex = 24;
            this.txtParentId.TextChanged += new System.EventHandler(this.txtParentId_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 76);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "Parent Activity Id";
            // 
            // txtDesc
            // 
            this.txtDesc.Location = new System.Drawing.Point(132, 99);
            this.txtDesc.Multiline = true;
            this.txtDesc.Name = "txtDesc";
            this.txtDesc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDesc.Size = new System.Drawing.Size(163, 50);
            this.txtDesc.TabIndex = 26;
            this.txtDesc.TextChanged += new System.EventHandler(this.txtDesc_TextChanged);
            // 
            // lblDesc
            // 
            this.lblDesc.AutoSize = true;
            this.lblDesc.Location = new System.Drawing.Point(3, 102);
            this.lblDesc.Name = "lblDesc";
            this.lblDesc.Size = new System.Drawing.Size(60, 13);
            this.lblDesc.TabIndex = 25;
            this.lblDesc.Text = "Description";
            // 
            // txtAppName
            // 
            this.txtAppName.Location = new System.Drawing.Point(480, 47);
            this.txtAppName.Name = "txtAppName";
            this.txtAppName.ReadOnly = true;
            this.txtAppName.Size = new System.Drawing.Size(163, 20);
            this.txtAppName.TabIndex = 28;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(339, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 13);
            this.label3.TabIndex = 27;
            this.label3.Text = "Aplication Name";
            // 
            // txtAppExe
            // 
            this.txtAppExe.Location = new System.Drawing.Point(480, 73);
            this.txtAppExe.Multiline = true;
            this.txtAppExe.Name = "txtAppExe";
            this.txtAppExe.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAppExe.Size = new System.Drawing.Size(163, 58);
            this.txtAppExe.TabIndex = 30;
            this.txtAppExe.TextChanged += new System.EventHandler(this.txtAppExe_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(339, 76);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(142, 13);
            this.label4.TabIndex = 29;
            this.label4.Text = "Application Executable/URL";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(568, 770);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 31;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblTasks
            // 
            this.lblTasks.AutoSize = true;
            this.lblTasks.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTasks.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblTasks.Location = new System.Drawing.Point(3, 239);
            this.lblTasks.Name = "lblTasks";
            this.lblTasks.Size = new System.Drawing.Size(55, 16);
            this.lblTasks.TabIndex = 32;
            this.lblTasks.Text = "Tasks:";
            // 
            // dgTasks
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dgTasks.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgTasks.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dgTasks.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgTasks.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgTasks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgTasks.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Delete});
            this.dgTasks.GridColor = System.Drawing.Color.LightGray;
            this.dgTasks.Location = new System.Drawing.Point(6, 268);
            this.dgTasks.MultiSelect = false;
            this.dgTasks.Name = "dgTasks";
            this.dgTasks.ReadOnly = true;
            this.dgTasks.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgTasks.RowHeadersWidth = 20;
            this.dgTasks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgTasks.Size = new System.Drawing.Size(637, 179);
            this.dgTasks.TabIndex = 33;
            this.dgTasks.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgTasks_CellMouseClick);
            // 
            // Delete
            // 
            this.Delete.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.Delete.HeaderText = "";
            this.Delete.Name = "Delete";
            this.Delete.ReadOnly = true;
            this.Delete.Text = "Delete";
            this.Delete.ToolTipText = "Click to delete the task";
            this.Delete.UseColumnTextForButtonValue = true;
            this.Delete.Width = 21;
            // 
            // txtControlId
            // 
            this.txtControlId.Location = new System.Drawing.Point(132, 523);
            this.txtControlId.Name = "txtControlId";
            this.txtControlId.ReadOnly = true;
            this.txtControlId.Size = new System.Drawing.Size(163, 20);
            this.txtControlId.TabIndex = 35;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 526);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 13);
            this.label6.TabIndex = 34;
            this.label6.Text = "Control Id";
            // 
            // txtTaskName
            // 
            this.txtTaskName.Location = new System.Drawing.Point(132, 549);
            this.txtTaskName.Name = "txtTaskName";
            this.txtTaskName.ReadOnly = true;
            this.txtTaskName.Size = new System.Drawing.Size(163, 20);
            this.txtTaskName.TabIndex = 37;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 552);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 13);
            this.label7.TabIndex = 36;
            this.label7.Text = "Name";
            // 
            // lblSelectedTask
            // 
            this.lblSelectedTask.AutoSize = true;
            this.lblSelectedTask.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSelectedTask.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblSelectedTask.Location = new System.Drawing.Point(3, 490);
            this.lblSelectedTask.Name = "lblSelectedTask";
            this.lblSelectedTask.Size = new System.Drawing.Size(164, 16);
            this.lblSelectedTask.TabIndex = 38;
            this.lblSelectedTask.Text = "Selected Task details:";
            // 
            // btnAddTask
            // 
            this.btnAddTask.Location = new System.Drawing.Point(568, 452);
            this.btnAddTask.Name = "btnAddTask";
            this.btnAddTask.Size = new System.Drawing.Size(75, 23);
            this.btnAddTask.TabIndex = 39;
            this.btnAddTask.Text = "Add Task";
            this.btnAddTask.UseVisualStyleBackColor = true;
            this.btnAddTask.Click += new System.EventHandler(this.btnAddTask_Click);
            // 
            // txtTaskDesc
            // 
            this.txtTaskDesc.Location = new System.Drawing.Point(480, 572);
            this.txtTaskDesc.Multiline = true;
            this.txtTaskDesc.Name = "txtTaskDesc";
            this.txtTaskDesc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtTaskDesc.Size = new System.Drawing.Size(163, 56);
            this.txtTaskDesc.TabIndex = 41;
            this.txtTaskDesc.TextChanged += new System.EventHandler(this.txtTaskDesc_TextChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(347, 575);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(60, 13);
            this.label11.TabIndex = 40;
            this.label11.Text = "Description";
            // 
            // txtWinTitle
            // 
            this.txtWinTitle.Location = new System.Drawing.Point(480, 519);
            this.txtWinTitle.Name = "txtWinTitle";
            this.txtWinTitle.Size = new System.Drawing.Size(163, 20);
            this.txtWinTitle.TabIndex = 43;
            this.txtWinTitle.TextChanged += new System.EventHandler(this.txtWinTitle_TextChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(347, 522);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(69, 13);
            this.label12.TabIndex = 42;
            this.label12.Text = "Window Title";
            // 
            // txtTaskGroupId
            // 
            this.txtTaskGroupId.Location = new System.Drawing.Point(480, 545);
            this.txtTaskGroupId.Name = "txtTaskGroupId";
            this.txtTaskGroupId.Size = new System.Drawing.Size(163, 20);
            this.txtTaskGroupId.TabIndex = 45;
            this.txtTaskGroupId.TextChanged += new System.EventHandler(this.txtTaskGroupId_TextChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(347, 548);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(48, 13);
            this.label13.TabIndex = 44;
            this.label13.Text = "Group Id";
            // 
            // txtXPath
            // 
            this.txtXPath.Location = new System.Drawing.Point(480, 632);
            this.txtXPath.Multiline = true;
            this.txtXPath.Name = "txtXPath";
            this.txtXPath.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtXPath.Size = new System.Drawing.Size(163, 59);
            this.txtXPath.TabIndex = 47;
            this.txtXPath.TextChanged += new System.EventHandler(this.txtXPath_TextChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(347, 635);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(93, 13);
            this.label14.TabIndex = 46;
            this.label14.Text = "XPath (For HTML)";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(3, 576);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(70, 13);
            this.label15.TabIndex = 48;
            this.label15.Text = "Image (if any)";
            // 
            // pic
            // 
            this.pic.BackColor = System.Drawing.SystemColors.Window;
            this.pic.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pic.InitialImage = ((System.Drawing.Image)(resources.GetObject("pic.InitialImage")));
            this.pic.Location = new System.Drawing.Point(132, 576);
            this.pic.Name = "pic";
            this.pic.Size = new System.Drawing.Size(163, 140);
            this.pic.TabIndex = 49;
            this.pic.TabStop = false;
            // 
            // btnCrop
            // 
            this.btnCrop.Location = new System.Drawing.Point(208, 722);
            this.btnCrop.Name = "btnCrop";
            this.btnCrop.Size = new System.Drawing.Size(75, 23);
            this.btnCrop.TabIndex = 50;
            this.btnCrop.Text = "Crop";
            this.btnCrop.UseVisualStyleBackColor = true;
            this.btnCrop.Click += new System.EventHandler(this.btnCrop_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(284, 721);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(18, 24);
            this.pictureBox1.TabIndex = 51;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(127, 722);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 52;
            this.btnLoad.Text = "Upload";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // txtTaskText
            // 
            this.txtTaskText.Location = new System.Drawing.Point(480, 697);
            this.txtTaskText.Multiline = true;
            this.txtTaskText.Name = "txtTaskText";
            this.txtTaskText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtTaskText.Size = new System.Drawing.Size(163, 60);
            this.txtTaskText.TabIndex = 54;
            this.txtTaskText.TextChanged += new System.EventHandler(this.txtTaskText_TextChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(347, 712);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(99, 13);
            this.label16.TabIndex = 53;
            this.label16.Text = "(Text Based Key(s))";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(347, 700);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(58, 13);
            this.label17.TabIndex = 55;
            this.label17.Text = "Task Text ";
            // 
            // lblEnd
            // 
            this.lblEnd.AutoSize = true;
            this.lblEnd.Location = new System.Drawing.Point(3, 767);
            this.lblEnd.Name = "lblEnd";
            this.lblEnd.Size = new System.Drawing.Size(10, 13);
            this.lblEnd.TabIndex = 56;
            this.lblEnd.Text = ".";
            // 
            // lblTags
            // 
            this.lblTags.AutoSize = true;
            this.lblTags.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTags.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblTags.Location = new System.Drawing.Point(3, 162);
            this.lblTags.Name = "lblTags";
            this.lblTags.Size = new System.Drawing.Size(102, 16);
            this.lblTags.TabIndex = 58;
            this.lblTags.Text = "Activity Tags:";
            // 
            // Tag
            // 
            this.Tag.AutoSize = true;
            this.Tag.Location = new System.Drawing.Point(3, 183);
            this.Tag.Name = "Tag";
            this.Tag.Size = new System.Drawing.Size(34, 13);
            this.Tag.TabIndex = 61;
            this.Tag.Text = "Tags ";
            // 
            // pnlTags
            // 
            this.pnlTags.AutoScroll = true;
            this.pnlTags.BackColor = System.Drawing.SystemColors.Window;
            this.pnlTags.Location = new System.Drawing.Point(135, 172);
            this.pnlTags.Name = "pnlTags";
            this.pnlTags.Size = new System.Drawing.Size(508, 55);
            this.pnlTags.TabIndex = 64;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1, 197);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(126, 13);
            this.label2.TabIndex = 65;
            this.label2.Text = "(Provide new Tag in the  ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(-1, 210);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(136, 13);
            this.label5.TabIndex = 66;
            this.label5.Text = "text box and press ENTER)";
            // 
            // ActivityView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pnlTags);
            this.Controls.Add(this.Tag);
            this.Controls.Add(this.lblTags);
            this.Controls.Add(this.lblEnd);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.txtTaskText);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnCrop);
            this.Controls.Add(this.pic);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.txtXPath);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.txtTaskGroupId);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.txtWinTitle);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.txtTaskDesc);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.btnAddTask);
            this.Controls.Add(this.lblSelectedTask);
            this.Controls.Add(this.txtTaskName);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtControlId);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.dgTasks);
            this.Controls.Add(this.lblTasks);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtAppExe);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtAppName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtDesc);
            this.Controls.Add(this.lblDesc);
            this.Controls.Add(this.txtParentId);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtActId);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label10);
            this.Name = "ActivityView";
            this.Size = new System.Drawing.Size(650, 820);
            this.Load += new System.EventHandler(this.ActivityView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgTasks)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtActId;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtParentId;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtDesc;
        private System.Windows.Forms.Label lblDesc;
        private System.Windows.Forms.TextBox txtAppName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtAppExe;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblTasks;
        private System.Windows.Forms.DataGridView dgTasks;
        private System.Windows.Forms.DataGridViewButtonColumn Delete;
        private System.Windows.Forms.TextBox txtControlId;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtTaskName;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblSelectedTask;
        private System.Windows.Forms.Button btnAddTask;
        private System.Windows.Forms.TextBox txtTaskDesc;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtWinTitle;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtTaskGroupId;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtXPath;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.PictureBox pic;
        private System.Windows.Forms.Button btnCrop;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.TextBox txtTaskText;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label lblEnd;
        private System.Windows.Forms.Label lblTags;
        private System.Windows.Forms.Label Tag;
        private System.Windows.Forms.Panel pnlTags;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
    }
}

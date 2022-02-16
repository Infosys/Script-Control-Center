namespace Infosys.ATR.DevelopmentStudio
{
    partial class EditObjectModel
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.txtAppTreePath = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtType = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.txtID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.txtElement = new System.Windows.Forms.RichTextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.btnValidate = new System.Windows.Forms.Button();
            this.cmbValidate = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnCtr = new System.Windows.Forms.Button();
            this.cmbDown = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cmbUp = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cmbLeft = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbRight = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbCtr = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnAdd = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.Color.White;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pictureBox1);
            this.splitContainer1.Size = new System.Drawing.Size(401, 540);
            this.splitContainer1.SplitterDistance = 340;
            this.splitContainer1.TabIndex = 1;
            this.splitContainer1.DoubleClick += new System.EventHandler(this.pictureBox1_DoubleClick);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.ShowToolTips = true;
            this.tabControl1.Size = new System.Drawing.Size(401, 340);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            this.tabControl1.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControl1_Selected);
            this.tabControl1.TabIndexChanged += new System.EventHandler(this.tabControl1_TabIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnAdd);
            this.tabPage1.Controls.Add(this.txtAppTreePath);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.txtType);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.txtName);
            this.tabPage1.Controls.Add(this.lblName);
            this.tabPage1.Controls.Add(this.txtID);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(393, 314);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Win32";
            this.tabPage1.ToolTipText = "Click for basic object identifier(s)";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // txtAppTreePath
            // 
            this.txtAppTreePath.Location = new System.Drawing.Point(153, 104);
            this.txtAppTreePath.Multiline = true;
            this.txtAppTreePath.Name = "txtAppTreePath";
            this.txtAppTreePath.Size = new System.Drawing.Size(173, 48);
            this.txtAppTreePath.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 106);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(109, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Application Tree Path";
            // 
            // txtType
            // 
            this.txtType.Location = new System.Drawing.Point(153, 78);
            this.txtType.Name = "txtType";
            this.txtType.Size = new System.Drawing.Size(173, 20);
            this.txtType.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Type/ Role";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(153, 52);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(173, 20);
            this.txtName.TabIndex = 3;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(28, 54);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(35, 13);
            this.lblName.TabIndex = 2;
            this.lblName.Text = "Name";
            // 
            // txtID
            // 
            this.txtID.Location = new System.Drawing.Point(153, 26);
            this.txtID.Name = "txtID";
            this.txtID.Size = new System.Drawing.Size(173, 20);
            this.txtID.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "ID/ Automation Id";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.txtElement);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(393, 314);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "More...";
            this.tabPage2.ToolTipText = "Click for the complete object details";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // txtElement
            // 
            this.txtElement.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtElement.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtElement.Location = new System.Drawing.Point(3, 3);
            this.txtElement.Name = "txtElement";
            this.txtElement.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtElement.Size = new System.Drawing.Size(387, 308);
            this.txtElement.TabIndex = 0;
            this.txtElement.Text = "";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.btnValidate);
            this.tabPage3.Controls.Add(this.cmbValidate);
            this.tabPage3.Controls.Add(this.label9);
            this.tabPage3.Controls.Add(this.btnDown);
            this.tabPage3.Controls.Add(this.btnUp);
            this.tabPage3.Controls.Add(this.btnLeft);
            this.tabPage3.Controls.Add(this.btnRight);
            this.tabPage3.Controls.Add(this.btnCtr);
            this.tabPage3.Controls.Add(this.cmbDown);
            this.tabPage3.Controls.Add(this.label8);
            this.tabPage3.Controls.Add(this.cmbUp);
            this.tabPage3.Controls.Add(this.label7);
            this.tabPage3.Controls.Add(this.cmbLeft);
            this.tabPage3.Controls.Add(this.label6);
            this.tabPage3.Controls.Add(this.cmbRight);
            this.tabPage3.Controls.Add(this.label5);
            this.tabPage3.Controls.Add(this.cmbCtr);
            this.tabPage3.Controls.Add(this.label4);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(393, 314);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Images";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // btnValidate
            // 
            this.btnValidate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnValidate.Image = global::Infosys.ATR.DevelopmentStudio.Properties.Resources.camera;
            this.btnValidate.Location = new System.Drawing.Point(274, 194);
            this.btnValidate.Name = "btnValidate";
            this.btnValidate.Size = new System.Drawing.Size(28, 23);
            this.btnValidate.TabIndex = 17;
            this.btnValidate.UseVisualStyleBackColor = true;
            this.btnValidate.Click += new System.EventHandler(this.btnValidate_Click);
            // 
            // cmbValidate
            // 
            this.cmbValidate.FormattingEnabled = true;
            this.cmbValidate.Items.AddRange(new object[] {
            ""});
            this.cmbValidate.Location = new System.Drawing.Point(135, 199);
            this.cmbValidate.Name = "cmbValidate";
            this.cmbValidate.Size = new System.Drawing.Size(121, 21);
            this.cmbValidate.TabIndex = 16;
            this.cmbValidate.SelectedIndexChanged += new System.EventHandler(this.cmbValidate_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(45, 202);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(45, 13);
            this.label9.TabIndex = 15;
            this.label9.Text = "Validate";
            // 
            // btnDown
            // 
            this.btnDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDown.Image = global::Infosys.ATR.DevelopmentStudio.Properties.Resources.camera;
            this.btnDown.Location = new System.Drawing.Point(274, 162);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(28, 23);
            this.btnDown.TabIndex = 14;
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnUp
            // 
            this.btnUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUp.Image = global::Infosys.ATR.DevelopmentStudio.Properties.Resources.camera;
            this.btnUp.Location = new System.Drawing.Point(274, 130);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(28, 23);
            this.btnUp.TabIndex = 13;
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnLeft
            // 
            this.btnLeft.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLeft.Image = global::Infosys.ATR.DevelopmentStudio.Properties.Resources.camera;
            this.btnLeft.Location = new System.Drawing.Point(274, 98);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(28, 23);
            this.btnLeft.TabIndex = 12;
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // btnRight
            // 
            this.btnRight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRight.Image = global::Infosys.ATR.DevelopmentStudio.Properties.Resources.camera;
            this.btnRight.Location = new System.Drawing.Point(274, 66);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(28, 23);
            this.btnRight.TabIndex = 11;
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // btnCtr
            // 
            this.btnCtr.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCtr.Image = global::Infosys.ATR.DevelopmentStudio.Properties.Resources.camera;
            this.btnCtr.Location = new System.Drawing.Point(274, 34);
            this.btnCtr.Name = "btnCtr";
            this.btnCtr.Size = new System.Drawing.Size(28, 23);
            this.btnCtr.TabIndex = 10;
            this.btnCtr.UseVisualStyleBackColor = true;
            this.btnCtr.Click += new System.EventHandler(this.btnCtr_Click);
            // 
            // cmbDown
            // 
            this.cmbDown.FormattingEnabled = true;
            this.cmbDown.Items.AddRange(new object[] {
            ""});
            this.cmbDown.Location = new System.Drawing.Point(132, 166);
            this.cmbDown.Name = "cmbDown";
            this.cmbDown.Size = new System.Drawing.Size(121, 21);
            this.cmbDown.TabIndex = 9;
            this.cmbDown.SelectedIndexChanged += new System.EventHandler(this.cmbDown_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(45, 169);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(35, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "Down";
            // 
            // cmbUp
            // 
            this.cmbUp.FormattingEnabled = true;
            this.cmbUp.Items.AddRange(new object[] {
            ""});
            this.cmbUp.Location = new System.Drawing.Point(132, 133);
            this.cmbUp.Name = "cmbUp";
            this.cmbUp.Size = new System.Drawing.Size(121, 21);
            this.cmbUp.TabIndex = 7;
            this.cmbUp.SelectedIndexChanged += new System.EventHandler(this.cmbUp_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(45, 136);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(21, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Up";
            // 
            // cmbLeft
            // 
            this.cmbLeft.FormattingEnabled = true;
            this.cmbLeft.Items.AddRange(new object[] {
            ""});
            this.cmbLeft.Location = new System.Drawing.Point(132, 100);
            this.cmbLeft.Name = "cmbLeft";
            this.cmbLeft.Size = new System.Drawing.Size(121, 21);
            this.cmbLeft.TabIndex = 5;
            this.cmbLeft.SelectedIndexChanged += new System.EventHandler(this.cmbLeft_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(45, 103);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(25, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "Left";
            // 
            // cmbRight
            // 
            this.cmbRight.FormattingEnabled = true;
            this.cmbRight.Items.AddRange(new object[] {
            ""});
            this.cmbRight.Location = new System.Drawing.Point(132, 67);
            this.cmbRight.Name = "cmbRight";
            this.cmbRight.Size = new System.Drawing.Size(121, 21);
            this.cmbRight.TabIndex = 3;
            this.cmbRight.SelectedIndexChanged += new System.EventHandler(this.cmbRight_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(45, 70);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Right";
            // 
            // cmbCtr
            // 
            this.cmbCtr.FormattingEnabled = true;
            this.cmbCtr.Location = new System.Drawing.Point(132, 34);
            this.cmbCtr.Name = "cmbCtr";
            this.cmbCtr.Size = new System.Drawing.Size(121, 21);
            this.cmbCtr.TabIndex = 1;
            this.cmbCtr.SelectedIndexChanged += new System.EventHandler(this.cmbCtr_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(45, 37);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Center";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.Window;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(401, 196);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            this.pictureBox1.DoubleClick += new System.EventHandler(this.pictureBox1_DoubleClick);
            // 
            // btnAdd
            // 
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Location = new System.Drawing.Point(31, 185);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 8;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Visible = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // EditObjectModel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "EditObjectModel";
            this.Size = new System.Drawing.Size(401, 540);
            this.Load += new System.EventHandler(this.EditObjectModel_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.RichTextBox txtElement;
        private System.Windows.Forms.TextBox txtID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtAppTreePath;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.ComboBox cmbDown;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cmbUp;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmbLeft;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbRight;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbCtr;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnCtr;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnValidate;
        private System.Windows.Forms.ComboBox cmbValidate;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnAdd;
    }
}

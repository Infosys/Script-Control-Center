namespace Infosys.ATR.DevelopmentStudio
{
    partial class Shell
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Shell));
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripbtnOK = new System.Windows.Forms.ToolStripButton();
            this.toolStripbtnRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripbtnCtlCaptureOnMove = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_SaveOM = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonEditOM = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonTakeSnap = new System.Windows.Forms.ToolStripButton();
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.toolStrip2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip2
            // 
            this.toolStrip2.AutoSize = false;
            this.toolStrip2.ImageScalingSize = new System.Drawing.Size(30, 28);
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripbtnOK,
            this.toolStripbtnRefresh,
            this.toolStripbtnCtlCaptureOnMove,
            this.toolStripButton_SaveOM,
            this.toolStripButtonEditOM,
            this.toolStripButtonTakeSnap,
            this.toolStripComboBox1});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(914, 33);
            this.toolStrip2.TabIndex = 5;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripbtnOK
            // 
            this.toolStripbtnOK.AutoSize = false;
            this.toolStripbtnOK.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripbtnOK.Image = ((System.Drawing.Image)(resources.GetObject("toolStripbtnOK.Image")));
            this.toolStripbtnOK.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripbtnOK.Name = "toolStripbtnOK";
            this.toolStripbtnOK.Size = new System.Drawing.Size(25, 25);
            this.toolStripbtnOK.Text = "Use the control highlighted";
            this.toolStripbtnOK.Click += new System.EventHandler(this.toolStripbtnOK_Click);
            // 
            // toolStripbtnRefresh
            // 
            this.toolStripbtnRefresh.AutoSize = false;
            this.toolStripbtnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripbtnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("toolStripbtnRefresh.Image")));
            this.toolStripbtnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripbtnRefresh.Name = "toolStripbtnRefresh";
            this.toolStripbtnRefresh.Size = new System.Drawing.Size(28, 25);
            this.toolStripbtnRefresh.Text = "Refresh the control explorer";
            this.toolStripbtnRefresh.Click += new System.EventHandler(this.toolStripbtnRefresh_Click);
            // 
            // toolStripbtnCtlCaptureOnMove
            // 
            this.toolStripbtnCtlCaptureOnMove.AutoSize = false;
            this.toolStripbtnCtlCaptureOnMove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripbtnCtlCaptureOnMove.Image = ((System.Drawing.Image)(resources.GetObject("toolStripbtnCtlCaptureOnMove.Image")));
            this.toolStripbtnCtlCaptureOnMove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripbtnCtlCaptureOnMove.Name = "toolStripbtnCtlCaptureOnMove";
            this.toolStripbtnCtlCaptureOnMove.Size = new System.Drawing.Size(34, 30);
            this.toolStripbtnCtlCaptureOnMove.Text = "Start capturing the control details from application. Press \'SHIFT + move Mouse\' " +
                "on the control";
            this.toolStripbtnCtlCaptureOnMove.Click += new System.EventHandler(this.toolStripbtnCtlCaptureOnMove_Click);
            // 
            // toolStripButton_SaveOM
            // 
            this.toolStripButton_SaveOM.AutoSize = false;
            this.toolStripButton_SaveOM.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_SaveOM.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_SaveOM.Image")));
            this.toolStripButton_SaveOM.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_SaveOM.Name = "toolStripButton_SaveOM";
            this.toolStripButton_SaveOM.Size = new System.Drawing.Size(25, 25);
            this.toolStripButton_SaveOM.Text = "Save Application Object Model(s) for the selected node(s)";
            this.toolStripButton_SaveOM.Click += new System.EventHandler(this.toolStripButton_SaveOM_Click);
            // 
            // toolStripButtonEditOM
            // 
            this.toolStripButtonEditOM.AutoSize = false;
            this.toolStripButtonEditOM.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonEditOM.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonEditOM.Image")));
            this.toolStripButtonEditOM.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonEditOM.Name = "toolStripButtonEditOM";
            this.toolStripButtonEditOM.Size = new System.Drawing.Size(25, 25);
            this.toolStripButtonEditOM.Text = "Edit an Object Model";
            this.toolStripButtonEditOM.Click += new System.EventHandler(this.toolStripButtonEditOM_Click);
            // 
            // toolStripButtonTakeSnap
            // 
            this.toolStripButtonTakeSnap.AutoSize = false;
            this.toolStripButtonTakeSnap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonTakeSnap.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonTakeSnap.Image")));
            this.toolStripButtonTakeSnap.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonTakeSnap.Name = "toolStripButtonTakeSnap";
            this.toolStripButtonTakeSnap.Size = new System.Drawing.Size(25, 25);
            this.toolStripButtonTakeSnap.Text = "Take Snap";
            this.toolStripButtonTakeSnap.Click += new System.EventHandler(this.toolStripButtonTakeSnap_Click);
            // 
            // toolStripComboBox1
            // 
            this.toolStripComboBox1.Items.AddRange(new object[] {
            "Normal",
            "Web"});
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            this.toolStripComboBox1.Size = new System.Drawing.Size(121, 33);
            this.toolStripComboBox1.Text = "---Select Mode---";
            this.toolStripComboBox1.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox1_SelectedIndexChanged);
            this.toolStripComboBox1.Click += new System.EventHandler(this.toolStripComboBox1_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tabControl1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 33);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(914, 469);
            this.panel1.TabIndex = 6;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(914, 469);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(906, 443);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Control Explorer";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // Shell
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(914, 502);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Shell";
            this.Text = "iFEA";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Shell_FormClosed);
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton toolStripbtnOK;
        private System.Windows.Forms.ToolStripButton toolStripbtnRefresh;
        private System.Windows.Forms.ToolStripButton toolStripbtnCtlCaptureOnMove;
        private System.Windows.Forms.ToolStripButton toolStripButton_SaveOM;
        private System.Windows.Forms.ToolStripButton toolStripButtonEditOM;
        private System.Windows.Forms.ToolStripButton toolStripButtonTakeSnap;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
    }
}
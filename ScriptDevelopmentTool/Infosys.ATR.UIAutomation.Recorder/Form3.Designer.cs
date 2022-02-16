namespace Infosys.ATR.UIAutomation.Recorder
{
    
    partial class Form3
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
        [System.STAThread]
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form3));
            this.txt_Url = new System.Windows.Forms.TextBox();
            this.btn_Go = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.webBrowserWindow = new System.Windows.Forms.WebBrowser();
            this.tbcBrowsers = new System.Windows.Forms.TabControl();
            this.btnAddTab = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // txt_Url
            // 
            this.txt_Url.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_Url.Location = new System.Drawing.Point(122, 9);
            this.txt_Url.Name = "txt_Url";
            this.txt_Url.Size = new System.Drawing.Size(703, 20);
            this.txt_Url.TabIndex = 0;
            this.txt_Url.Text = "http://";
            // 
            // btn_Go
            // 
            this.btn_Go.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Go.BackColor = System.Drawing.SystemColors.Window;
            this.btn_Go.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_Go.BackgroundImage")));
            this.btn_Go.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btn_Go.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btn_Go.Location = new System.Drawing.Point(827, 8);
            this.btn_Go.Name = "btn_Go";
            this.btn_Go.Size = new System.Drawing.Size(23, 23);
            this.btn_Go.TabIndex = 1;
            this.btn_Go.UseVisualStyleBackColor = false;
            this.btn_Go.Click += new System.EventHandler(this.btn_Go_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label1.Location = new System.Drawing.Point(3, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Enter website URL\r\n";
            // 
            // webBrowserWindow
            // 
            this.webBrowserWindow.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.webBrowserWindow.Location = new System.Drawing.Point(0, 509);
            this.webBrowserWindow.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserWindow.Name = "webBrowserWindow";
            this.webBrowserWindow.ScriptErrorsSuppressed = true;
            this.webBrowserWindow.Size = new System.Drawing.Size(880, 20);
            this.webBrowserWindow.TabIndex = 5;
            this.webBrowserWindow.Visible = false;
            this.webBrowserWindow.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser_DocumentCompleted);
            // 
            // tbcBrowsers
            // 
            this.tbcBrowsers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbcBrowsers.Location = new System.Drawing.Point(0, 33);
            this.tbcBrowsers.Name = "tbcBrowsers";
            this.tbcBrowsers.SelectedIndex = 0;
            this.tbcBrowsers.Size = new System.Drawing.Size(880, 496);
            this.tbcBrowsers.TabIndex = 6;
            this.tbcBrowsers.SelectedIndexChanged += new System.EventHandler(this.tbcBrowsers_SelectedIndexChanged);
            // 
            // btnAddTab
            // 
            this.btnAddTab.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddTab.BackColor = System.Drawing.SystemColors.Window;
            this.btnAddTab.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnAddTab.BackgroundImage")));
            this.btnAddTab.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnAddTab.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnAddTab.Location = new System.Drawing.Point(854, 8);
            this.btnAddTab.Name = "btnAddTab";
            this.btnAddTab.Size = new System.Drawing.Size(23, 23);
            this.btnAddTab.TabIndex = 7;
            this.btnAddTab.UseVisualStyleBackColor = false;
            this.btnAddTab.Click += new System.EventHandler(this.btnAddTab_Click);
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ClientSize = new System.Drawing.Size(880, 529);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_Go);
            this.Controls.Add(this.txt_Url);
            this.Controls.Add(this.btnAddTab);
            this.Controls.Add(this.tbcBrowsers);
            this.Controls.Add(this.webBrowserWindow);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form3";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ATR Web Browser";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Form3_Load);
            this.Resize += new System.EventHandler(this.Form3_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txt_Url;
        private System.Windows.Forms.Button btn_Go;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.WebBrowser webBrowserWindow;
        private System.Windows.Forms.TabControl tbcBrowsers;
        private System.Windows.Forms.Button btnAddTab;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
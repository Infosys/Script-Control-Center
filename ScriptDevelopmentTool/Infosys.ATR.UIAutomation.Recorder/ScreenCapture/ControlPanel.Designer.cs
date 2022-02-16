namespace Infosys.ATR.UIAutomation.Recorder.ScreenCapture
{
    partial class ControlPanel
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
            this.bttCaptureScreen = new System.Windows.Forms.Button();
            this.bttCaptureArea = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.bttTips = new System.Windows.Forms.Button();
            this.txtTips = new System.Windows.Forms.RichTextBox();
            this.saveToClipboard = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // bttCaptureScreen
            // 
            this.bttCaptureScreen.BackColor = System.Drawing.Color.WhiteSmoke;
            this.bttCaptureScreen.ForeColor = System.Drawing.SystemColors.ControlText;
            this.bttCaptureScreen.Location = new System.Drawing.Point(2, 15);
            this.bttCaptureScreen.Name = "bttCaptureScreen";
            this.bttCaptureScreen.Size = new System.Drawing.Size(90, 32);
            this.bttCaptureScreen.TabIndex = 0;
            this.bttCaptureScreen.TabStop = false;
            this.bttCaptureScreen.Text = "Capture Screen";
            this.bttCaptureScreen.UseVisualStyleBackColor = false;
            this.bttCaptureScreen.Click += new System.EventHandler(this.bttCaptureScreen_Click);
            this.bttCaptureScreen.KeyUp += new System.Windows.Forms.KeyEventHandler(this.bttCaptureScreen_KeyUp);
            // 
            // bttCaptureArea
            // 
            this.bttCaptureArea.BackColor = System.Drawing.Color.WhiteSmoke;
            this.bttCaptureArea.ForeColor = System.Drawing.SystemColors.ControlText;
            this.bttCaptureArea.Location = new System.Drawing.Point(2, 57);
            this.bttCaptureArea.Name = "bttCaptureArea";
            this.bttCaptureArea.Size = new System.Drawing.Size(90, 32);
            this.bttCaptureArea.TabIndex = 2;
            this.bttCaptureArea.TabStop = false;
            this.bttCaptureArea.Text = "Capture Area";
            this.bttCaptureArea.UseVisualStyleBackColor = false;
            this.bttCaptureArea.Click += new System.EventHandler(this.bttCaptureArea_Click);
            this.bttCaptureArea.KeyUp += new System.Windows.Forms.KeyEventHandler(this.bttCaptureArea_KeyUp);
            // 
            // bttTips
            // 
            this.bttTips.BackColor = System.Drawing.Color.WhiteSmoke;
            this.bttTips.ForeColor = System.Drawing.SystemColors.ControlText;
            this.bttTips.Location = new System.Drawing.Point(92, 57);
            this.bttTips.Name = "bttTips";
            this.bttTips.Size = new System.Drawing.Size(32, 32);
            this.bttTips.TabIndex = 4;
            this.bttTips.TabStop = false;
            this.bttTips.Text = "tips";
            this.bttTips.UseVisualStyleBackColor = false;
            this.bttTips.Click += new System.EventHandler(this.bttTips_Click);
            this.bttTips.KeyUp += new System.Windows.Forms.KeyEventHandler(this.bttTips_KeyUp);
            // 
            // txtTips
            // 
            this.txtTips.BackColor = System.Drawing.SystemColors.Window;
            this.txtTips.Location = new System.Drawing.Point(125, 7);
            this.txtTips.Name = "txtTips";
            this.txtTips.ReadOnly = true;
            this.txtTips.Size = new System.Drawing.Size(247, 105);
            this.txtTips.TabIndex = 5;
            this.txtTips.Text = "";
            this.txtTips.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtTips_KeyUp);
            // 
            // saveToClipboard
            // 
            this.saveToClipboard.AutoSize = true;
            this.saveToClipboard.Location = new System.Drawing.Point(9, 95);
            this.saveToClipboard.Name = "saveToClipboard";
            this.saveToClipboard.Size = new System.Drawing.Size(110, 17);
            this.saveToClipboard.TabIndex = 6;
            this.saveToClipboard.TabStop = false;
            this.saveToClipboard.Text = "Save to Clipboard";
            this.saveToClipboard.UseVisualStyleBackColor = true;
            this.saveToClipboard.CheckedChanged += new System.EventHandler(this.saveToClipboard_CheckedChanged);
            this.saveToClipboard.KeyUp += new System.Windows.Forms.KeyEventHandler(this.saveToClipboard_KeyUp);
            // 
            // ControlPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LemonChiffon;
            this.ClientSize = new System.Drawing.Size(375, 122);
            this.Controls.Add(this.saveToClipboard);
            this.Controls.Add(this.txtTips);
            this.Controls.Add(this.bttCaptureScreen);
            this.Controls.Add(this.bttTips);
            this.Controls.Add(this.bttCaptureArea);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ControlPanel";
            this.ShowIcon = false;
            this.Text = "ScreenCapture";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ControlPanel_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bttCaptureScreen;
        private System.Windows.Forms.Button bttCaptureArea;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button bttTips;
        private System.Windows.Forms.RichTextBox txtTips;
        private System.Windows.Forms.CheckBox saveToClipboard;
    }
}
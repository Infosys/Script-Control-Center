namespace TrackActivities
{
    partial class Form1
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
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnkeyboard = new System.Windows.Forms.Button();
            this.btnMouse = new System.Windows.Forms.Button();
            this.btnRecord = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.txtApp = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtProcId = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtUc = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtHandle = new System.Windows.Forms.TextBox();
            this.btnMachineDetails = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(0, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Serialize";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(0, 58);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(282, 141);
            this.textBox1.TabIndex = 1;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // btnkeyboard
            // 
            this.btnkeyboard.Location = new System.Drawing.Point(81, 0);
            this.btnkeyboard.Name = "btnkeyboard";
            this.btnkeyboard.Size = new System.Drawing.Size(75, 23);
            this.btnkeyboard.TabIndex = 2;
            this.btnkeyboard.Text = "keyboard";
            this.btnkeyboard.UseVisualStyleBackColor = true;
            this.btnkeyboard.Click += new System.EventHandler(this.btnkeyboard_Click);
            // 
            // btnMouse
            // 
            this.btnMouse.Location = new System.Drawing.Point(162, 0);
            this.btnMouse.Name = "btnMouse";
            this.btnMouse.Size = new System.Drawing.Size(75, 23);
            this.btnMouse.TabIndex = 3;
            this.btnMouse.Text = "mouse";
            this.btnMouse.UseVisualStyleBackColor = true;
            this.btnMouse.Click += new System.EventHandler(this.btnMouse_Click);
            // 
            // btnRecord
            // 
            this.btnRecord.Location = new System.Drawing.Point(243, 0);
            this.btnRecord.Name = "btnRecord";
            this.btnRecord.Size = new System.Drawing.Size(75, 23);
            this.btnRecord.TabIndex = 4;
            this.btnRecord.Text = "record";
            this.btnRecord.UseVisualStyleBackColor = true;
            this.btnRecord.Click += new System.EventHandler(this.btnRecord_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(243, 29);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 6;
            this.btnStop.Text = "stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(226, 316);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "play";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // txtApp
            // 
            this.txtApp.Location = new System.Drawing.Point(86, 237);
            this.txtApp.Name = "txtApp";
            this.txtApp.Size = new System.Drawing.Size(215, 20);
            this.txtApp.TabIndex = 8;
            this.txtApp.Text = "C:\\Windows\\system32\\calc.exe";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 237);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "App path";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 263);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Process id";
            // 
            // txtProcId
            // 
            this.txtProcId.Location = new System.Drawing.Point(86, 263);
            this.txtProcId.Name = "txtProcId";
            this.txtProcId.Size = new System.Drawing.Size(215, 20);
            this.txtProcId.TabIndex = 10;
            this.txtProcId.Text = "7628";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 211);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Use case path";
            // 
            // txtUc
            // 
            this.txtUc.Location = new System.Drawing.Point(86, 211);
            this.txtUc.Name = "txtUc";
            this.txtUc.Size = new System.Drawing.Size(215, 20);
            this.txtUc.TabIndex = 12;
            this.txtUc.Text = "C:\\Users\\rahul_bandopadhyaya\\Desktop\\usecase1.xml";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 289);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Handle";
            // 
            // txtHandle
            // 
            this.txtHandle.Location = new System.Drawing.Point(86, 289);
            this.txtHandle.Name = "txtHandle";
            this.txtHandle.Size = new System.Drawing.Size(215, 20);
            this.txtHandle.TabIndex = 14;
            // 
            // btnMachineDetails
            // 
            this.btnMachineDetails.Location = new System.Drawing.Point(111, 315);
            this.btnMachineDetails.Name = "btnMachineDetails";
            this.btnMachineDetails.Size = new System.Drawing.Size(100, 23);
            this.btnMachineDetails.TabIndex = 16;
            this.btnMachineDetails.Text = "Machine Details";
            this.btnMachineDetails.UseVisualStyleBackColor = true;
            this.btnMachineDetails.Click += new System.EventHandler(this.btnMachineDetails_Click);
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(30, 315);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(75, 23);
            this.btnTest.TabIndex = 17;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(288, 103);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(80, 17);
            this.checkBox1.TabIndex = 18;
            this.checkBox1.Text = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(289, 127);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(85, 17);
            this.radioButton1.TabIndex = 19;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "radioButton1";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(336, 348);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnMachineDetails);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtHandle);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtUc);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtProcId);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtApp);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnRecord);
            this.Controls.Add(this.btnMouse);
            this.Controls.Add(this.btnkeyboard);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnkeyboard;
        private System.Windows.Forms.Button btnMouse;
        private System.Windows.Forms.Button btnRecord;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox txtApp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtProcId;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtUc;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtHandle;
        private System.Windows.Forms.Button btnMachineDetails;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.RadioButton radioButton1;
    }
}


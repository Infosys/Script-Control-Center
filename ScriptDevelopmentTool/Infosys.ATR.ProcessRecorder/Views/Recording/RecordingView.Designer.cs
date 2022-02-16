namespace Infosys.ATR.ProcessRecorder.Views
{
    partial class RecordingView
    {
        /// <summary>
        /// The presenter used by this view.
        /// </summary>
        private Infosys.ATR.ProcessRecorder.Views.RecordingPresenter _presenter = null;
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
            this.pnlGrid = new System.Windows.Forms.Panel();
            this.dgUseCases = new System.Windows.Forms.DataGridView();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.propGrdUsecase = new System.Windows.Forms.PropertyGrid();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnGenPlaybackScript = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.pnlGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgUseCases)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.SystemColors.Window;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.AutoScroll = true;
            this.splitContainer1.Panel1.Controls.Add(this.pnlGrid);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(1275, 500);
            this.splitContainer1.SplitterDistance = 1034;
            this.splitContainer1.TabIndex = 2;
            // 
            // pnlGrid
            // 
            this.pnlGrid.Controls.Add(this.dgUseCases);
            this.pnlGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGrid.Location = new System.Drawing.Point(0, 0);
            this.pnlGrid.Name = "pnlGrid";
            this.pnlGrid.Size = new System.Drawing.Size(1034, 500);
            this.pnlGrid.TabIndex = 3;
            // 
            // dgUseCases
            // 
            this.dgUseCases.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgUseCases.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgUseCases.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgUseCases.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgUseCases.Location = new System.Drawing.Point(0, 0);
            this.dgUseCases.MultiSelect = false;
            this.dgUseCases.Name = "dgUseCases";
            this.dgUseCases.ReadOnly = true;
            this.dgUseCases.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgUseCases.Size = new System.Drawing.Size(1034, 500);
            this.dgUseCases.TabIndex = 0;
            this.dgUseCases.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgUseCases_ColumnHeaderMouseClick);
            this.dgUseCases.SelectionChanged += new System.EventHandler(this.dgUseCases_SelectionChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(237, 500);
            this.tabControl1.TabIndex = 6;
            // 
            // tabPage1
            // 
            this.tabPage1.AutoScroll = true;
            this.tabPage1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Controls.Add(this.propGrdUsecase);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(0);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(229, 474);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Property Inspector";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // propGrdUsecase
            // 
            this.propGrdUsecase.Cursor = System.Windows.Forms.Cursors.Default;
            this.propGrdUsecase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propGrdUsecase.Location = new System.Drawing.Point(3, 3);
            this.propGrdUsecase.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.propGrdUsecase.Name = "propGrdUsecase";
            this.propGrdUsecase.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.propGrdUsecase.Size = new System.Drawing.Size(223, 468);
            this.propGrdUsecase.TabIndex = 0;
            this.toolTip1.SetToolTip(this.propGrdUsecase, "UseCase Property");
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnGenPlaybackScript);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(3, 411);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(223, 60);
            this.panel1.TabIndex = 6;
            // 
            // btnGenPlaybackScript
            // 
            this.btnGenPlaybackScript.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGenPlaybackScript.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnGenPlaybackScript.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenPlaybackScript.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGenPlaybackScript.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.btnGenPlaybackScript.Location = new System.Drawing.Point(0, 0);
            this.btnGenPlaybackScript.Name = "btnGenPlaybackScript";
            this.btnGenPlaybackScript.Size = new System.Drawing.Size(223, 60);
            this.btnGenPlaybackScript.TabIndex = 5;
            this.btnGenPlaybackScript.Text = "Generate Playback Script";
            this.toolTip1.SetToolTip(this.btnGenPlaybackScript, "Click here to Generate Playback Script");
            this.btnGenPlaybackScript.UseVisualStyleBackColor = true;
            this.btnGenPlaybackScript.Click += new System.EventHandler(this.btnGenPlaybackScript_Click);
            // 
            // RecordingView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "RecordingView";
            this.Size = new System.Drawing.Size(1275, 500);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.pnlGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgUseCases)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.PropertyGrid propGrdUsecase;
        private System.Windows.Forms.Panel pnlGrid;
        private System.Windows.Forms.DataGridView dgUseCases;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnGenPlaybackScript;

    }
}

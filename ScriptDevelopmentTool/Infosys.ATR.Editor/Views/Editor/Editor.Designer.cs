namespace Infosys.ATR.Editor.Views
{
    partial class Editor
    {
        /// <summary>
        /// The presenter used by this view.
        /// </summary>
        private Infosys.ATR.Editor.Views.EditorPresenter _presenter = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Editor));
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnlTreeview = new System.Windows.Forms.Panel();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panel3 = new System.Windows.Forms.Panel();
            this._objectModelDeck = new Microsoft.Practices.CompositeUI.WinForms.DeckWorkspace();
            this.panel1.SuspendLayout();
            this.pnlTreeview.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.pnlTreeview);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(807, 469);
            this.panel1.TabIndex = 0;
            // 
            // pnlTreeview
            // 
            this.pnlTreeview.Controls.Add(this.treeView1);
            this.pnlTreeview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTreeview.Location = new System.Drawing.Point(0, 0);
            this.pnlTreeview.Name = "pnlTreeview";
            this.pnlTreeview.Size = new System.Drawing.Size(487, 469);
            this.pnlTreeview.TabIndex = 2;
            // 
            // treeView1
            // 
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView1.ContextMenuStrip = this.contextMenuStrip1;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeView1.HideSelection = false;
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(487, 469);
            this.treeView1.TabIndex = 1;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "application");
            this.imageList1.Images.SetKeyName(1, "screen");
            this.imageList1.Images.SetKeyName(2, "control");
            this.imageList1.Images.SetKeyName(3, "desktop");
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this._objectModelDeck);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(487, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(320, 469);
            this.panel3.TabIndex = 1;
            this.panel3.Visible = false;
            // 
            // _objectModelDeck
            // 
            this._objectModelDeck.Dock = System.Windows.Forms.DockStyle.Fill;
            this._objectModelDeck.Location = new System.Drawing.Point(0, 0);
            this._objectModelDeck.Name = "_objectModelDeck";
            this._objectModelDeck.Size = new System.Drawing.Size(320, 469);
            this._objectModelDeck.TabIndex = 0;
            this._objectModelDeck.Text = "deckWorkspace1";
            // 
            // Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "Editor";
            this.Size = new System.Drawing.Size(807, 469);
            this.panel1.ResumeLayout(false);
            this.pnlTreeview.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel pnlTreeview;
        private System.Windows.Forms.TreeView treeView1;
        private Microsoft.Practices.CompositeUI.WinForms.DeckWorkspace _objectModelDeck;

    }
}

namespace Infosys.ATR.Editor.Views
{
    partial class PropertyEditor
    {
        /// <summary>
        /// The presenter used by this view.
        /// </summary>
        private Infosys.ATR.Editor.Views.PropertyEditorPresenter _presenter = null;

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
            this._imgWorkspace = new Microsoft.Practices.CompositeUI.WinForms.TabWorkspace();
            this.SuspendLayout();
            // 
            // _imgWorkspace
            // 
            this._imgWorkspace.Dock = System.Windows.Forms.DockStyle.Fill;
            this._imgWorkspace.Location = new System.Drawing.Point(0, 0);
            this._imgWorkspace.Name = "_imgWorkspace";
            this._imgWorkspace.SelectedIndex = 0;
            this._imgWorkspace.Size = new System.Drawing.Size(303, 469);
            this._imgWorkspace.TabIndex = 0;
            // 
            // PropertyEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this._imgWorkspace);
            this.Name = "PropertyEditor";
            this.Size = new System.Drawing.Size(303, 469);
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Practices.CompositeUI.WinForms.TabWorkspace _imgWorkspace;



    }
}

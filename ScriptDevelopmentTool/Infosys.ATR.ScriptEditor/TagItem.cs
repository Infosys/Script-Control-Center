using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

//using Infosys.Collaboration.CtxtMgmt.Core;

namespace Infosys.ATR.ScriptEditor
{
    public partial class TagItem : UserControl
    {
        //private variables
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TagItem));
        List<string> passedTags = new List<string>();

        //public variables
        public class TagsChangedArgs : EventArgs
        {
            public string ChangedTags { get; set; }
        }
        public delegate void TagsChangedEventHandler(TagsChangedArgs e);
        public event TagsChangedEventHandler TagsChanged;

        public TagItem(string[] tags)
        {
            InitializeComponent();
            passedTags = tags.ToList();
            for (int i = 0; i < tags.Length; i++)
            {
                PopulateTags(tags[i]);
            }
        }

        private void PopulateTags(string tag)
        {
            string id = System.Guid.NewGuid().ToString();
            Label lblLink = new Label();
            lblLink.Name = "lbl" + id;
            lblLink.Text = tag;
            lblLink.ForeColor = Color.Navy;
            lblLink.TextAlign = ContentAlignment.MiddleLeft;
            lblLink.Dock = DockStyle.Left;
            toolTip1.SetToolTip(lblLink, tag);
            lblLink.Click += new EventHandler(lblLink_Click);
            pnlTags.Controls.Add(lblLink);

            Button btnTag = new Button();
            btnTag.Name = id;
            btnTag.Tag = tag;
            btnTag.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("del")));
            btnTag.BackgroundImageLayout = ImageLayout.Center;
            btnTag.Height = 20;
            btnTag.Width = 20;
            btnTag.FlatStyle = FlatStyle.Flat;
            btnTag.FlatAppearance.BorderSize = 0;
            btnTag.BackColor = Color.White;
            btnTag.Dock = DockStyle.Left;
            btnTag.Click += new EventHandler(btnTag_Click);
            pnlTags.Controls.Add(btnTag);
        }

        void btnTag_Click(object sender, EventArgs e)
        {
            Button btnTag = sender as Button;
            string id = btnTag.Name;
            pnlTags.Controls.RemoveByKey("lbl" + id);
            passedTags.Remove(btnTag.Tag as string);
            pnlTags.Controls.RemoveByKey(id);
            RaiseChangedTags();
        }

        void txtNewTag_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r') //i.e. "enter" is pressed
            {
                PopulateTags(txtAddTag.Text);                
                passedTags.Add(txtAddTag.Text);
                txtAddTag.Text = "";
                RaiseChangedTags();
            }
        }

        void lblLink_Click(object sender, EventArgs e)
        {
            string tag = (sender as Label).Text;
            MessageBox.Show(tag);
            //to pass the tag text to the ICT (infosys collaboration tool- buzz)
            //ContextManagerHelper ictHelper = new ContextManagerHelper();
            //ictHelper.RaiseScreenChanged(BuildContext(tag));
        }

        //private PresentationEntities.Context BuildContext(string name)
        //{
        //    int iDocIndex = name.IndexOf('.');
        //    if (iDocIndex > 0)
        //        name = name.Remove(iDocIndex);
        //    PresentationEntities.Context context = new PresentationEntities.Context();
        //    context.BusinessContext = new System.Collections.Generic.Dictionary<string, string>();
        //    context.BusinessContext.Add(Constants.DictionaryKeyForTextSearch, name);
        //    return context;
        //}

        void RaiseChangedTags()
        {
            if (TagsChanged != null)
            {
                TagsChangedArgs args = new TagsChangedArgs();
                passedTags.ForEach(t => {
                    args.ChangedTags += t + ";";
                });
                TagsChanged(args);
            }
        }

        private void TagItem_Load(object sender, EventArgs e)
        {
            //Pen pen = new System.Drawing.Pen(Color.Red);
            //Graphics graphics = txtAddTag.CreateGraphics();
            //graphics.DrawRectangle(pen, 1, 1, txtAddTag.Width, txtAddTag.Height);
        }
    }
}

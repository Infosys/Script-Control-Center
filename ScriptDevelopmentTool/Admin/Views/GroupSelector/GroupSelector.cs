using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using Infosys.ATR.Entities;
using Infosys.ATR.Admin.Entities;

namespace Infosys.ATR.Admin.Views
{
    public partial class GroupSelector : UserControl, IGroupSelector,IClose
    {
        TreeNode _root;
        List<int> groups = null;

        public GroupSelector()
        {
            InitializeComponent();
            this.Size = MaximumSize = MinimumSize;
            
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x0084;

            switch (m.Msg)
            {
                case WM_NCHITTEST:
                    return;
            }

            base.WndProc(ref m);
        }

        public void Initialize()
        {
            if(this.trGroups.Nodes != null)
                this.trGroups.Nodes.Clear();

            this._presenter.GetAllGroups();

            _root = new TreeNode("Groups");
            this.trGroups.Nodes.Add(_root);

            Groups.Where(g => g.ParentId == 0).ToList().
                ForEach(sg =>
                {
                    AddNode(sg, null);
                });

        }

        public void Close()
        {
            this._presenter.OnCloseView();           
        }

        private void AddNode(Groups g, TreeNode parent)
        {
            if (parent == null)
            {
                parent = new TreeNode();
                parent.Text = g.Name;
                parent.Tag = g;
                _root.Nodes.Add(parent);
            }
            else
            {
                TreeNode child = new TreeNode();
                child.Text = g.Name;
                child.Tag = g;
                parent.Nodes.Add(child);
                parent = child;
            }

            Groups.Where(sg => sg.ParentId == g.CategoryId).ToList().ForEach(child =>
            {
                AddNode(child, parent);
            });
        }        

        public List<Groups> Groups { get; set; }

        private void btnOk_Click(object sender, EventArgs e)
        {
            var node = this.trGroups.SelectedNode;
            if (node != null && node.Text != "Groups")
            {
                if (groups != null)
                {
                    groups.Clear();
                }
                else
                {
                    groups = new List<int>();
                }
                var g = node.Tag as Groups;
                groups.Add(g.CategoryId);
                GetChildren(node);
            }

            this._presenter.AddGroupUsers_Handler(groups);
            this._presenter.OnCloseView();
        }

        private void GetChildren(TreeNode n)
        {
            for (int i = 0; i < n.Nodes.Count; i++)
            {
                var g = n.Nodes[i].Tag as Groups;
                groups.Add(g.CategoryId);
                GetChildren(n.Nodes[i]);
            }
        }
    }
}

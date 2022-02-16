using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Infosys.ATR.Admin.Constants;
using Infosys.ATR.Admin.Entities;

namespace Infosys.ATR.Admin.Views
{
    public partial class GroupsExplorer : UserControl, IGroupsExplorer,IClose
    {
        TreeNode _root;
        List<int> children = new List<int>();
        List<int> parent = new List<int>();
        public List<Groups> Groups { get; set; }
        public BindingList<Users> Users { get; set; }

        public GroupsExplorer()
        {
            InitializeComponent();
            this.Size = MaximumSize = MinimumSize;
            this._centerWorkspace.Name = Constants.WorkspaceNames.GrCenterWorkspace;
            this._rightWorkspace.Name = Constants.WorkspaceNames.GrRightWorkspace;            
        }      

        internal void Initialize()
        {
            this._presenter.GetAllGroups();

            //this._presenter.GetCategories();            

            this._presenter.GetAllUsers();

            _root = new TreeNode("Groups");
            this.trGroups.Nodes.Add(_root);

            Groups.Where(g => g.ParentId == 0).ToList().
                ForEach(sg =>
                {
                    AddNode(sg, null);
                });

            if (_root.Nodes.Count > 0)
            {
                trGroups.SelectedNode = _root.Nodes[0];
            }
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

        private void GetChildren(TreeNode n)
        {
            for (int i = 0; i < n.Nodes.Count; i++)
            {
                var g = n.Nodes[i].Tag as Groups;
                children.Add(g.CategoryId);
                GetChildren(n.Nodes[i]);
            }
        }

        private void trGroups_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var group = e.Node.Tag as Groups;
            GetUsers(group);
        }

        void GetUsers(Groups group)
        {
           // var group = e.Node.Tag as Groups;

            if (group != null)
            {
                this.usersGrid.DataSource = null;

                this._presenter.GetAllUsersInaGroup(group);
                this.usersGrid.DataSource = Users;
                this.usersGrid.Columns[0].HeaderText = "";
                this.usersGrid.Columns[7].HeaderText = "";
                //this.usersGrid.Columns[0].Width = 15;
                //this.usersGrid.Columns[7].Width = 15;
                //this.usersGrid.Columns[1].Width = 180;
                this.usersGrid.Columns[2].Visible = this.usersGrid.Columns[3].Visible =
                    this.usersGrid.Columns[4].Visible = this.usersGrid.Columns[5].Visible = this.usersGrid.Columns[6].Visible = 
                    this.usersGrid.Columns[8].Visible = this.usersGrid.Columns[9].Visible = false;

                this.usersGrid.Columns[7].DisplayIndex = 1;

                this.usersGrid.Columns[1].DefaultCellStyle = new DataGridViewCellStyle();               
                
                DataGridViewComboBoxColumn roles = new DataGridViewComboBoxColumn();
                roles.HeaderText = "Role";

                roles.DataSource = this._presenter.GetAllRoles();
                roles.DisplayMember = "Name";
                roles.ValueMember = "Id";
                roles.DataPropertyName = "RoleId";

                usersGrid.Columns.Add(roles);
                usersGrid.Refresh();
                group.Action = Entities.Action.View;
                this.children.Clear();
                this.parent.Clear();
                GetParentNodes(this.trGroups.SelectedNode);
                group.Parents = this.parent;
                this.children.Add(group.CategoryId);
                GetChildren(this.trGroups.SelectedNode);
                group.Children = this.children;
                this._presenter.ShowGroupDetails_Handler(group);
            }
            else
            {
                this.usersGrid.DataSource = null; ;
                this.usersGrid.Refresh();
                this._presenter.ShowGroupDetails_Handler(group);
            }
        }

        private void usersGrid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            usersGrid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;


            if (e.RowIndex != -1)
            {
                //this.usersGrid.Columns[2].DefaultCellStyle.Padding = new Padding(10, 0, -10, 0);
                //this.usersGrid.Columns[3].DefaultCellStyle.Padding = new Padding(-10, 0, 0, 0);
                if (e.ColumnIndex == 0)
                {
                    this.usersGrid.Columns[0].Width = 25;
                }
                else if (e.ColumnIndex == 1)
                {
                    e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.Border);
                    ControlPaint.DrawBorder(e.Graphics, e.CellBounds,
                        usersGrid.GridColor, 0, ButtonBorderStyle.None,
                        usersGrid.GridColor, 1, ButtonBorderStyle.None,
                        usersGrid.GridColor, 1, ButtonBorderStyle.Solid,
                        usersGrid.GridColor, 1, ButtonBorderStyle.Solid);
                    e.Handled = true;

                    usersGrid.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    this.usersGrid.Columns[1].Width = 280;

                }
                else if (e.ColumnIndex == 7)
                {
                    e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.Border);
                    ControlPaint.DrawBorder(e.Graphics, e.CellBounds,
                        usersGrid.GridColor, 1, ButtonBorderStyle.None,
                        usersGrid.GridColor, 1, ButtonBorderStyle.None,
                        usersGrid.GridColor, 0, ButtonBorderStyle.None,
                        usersGrid.GridColor, 1, ButtonBorderStyle.Solid);
                    e.Handled = true;
                    usersGrid.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    this.usersGrid.Columns[7].Width = 25;
                }
                else
                {
                    e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.Border);
                    ControlPaint.DrawBorder(e.Graphics, e.CellBounds,
                        usersGrid.GridColor, 0, ButtonBorderStyle.None,
                        usersGrid.GridColor, 1, ButtonBorderStyle.None,
                        usersGrid.GridColor, 1, ButtonBorderStyle.Solid,
                        usersGrid.GridColor, 1, ButtonBorderStyle.Solid);
                    e.Handled = true;
                }

            }
        }

        private void GetParentNodes(TreeNode n)
        {
            if (n.Parent.Text != "Groups")
            {
                var p = n.Parent.Tag as Groups;
                parent.Add(p.CategoryId);
                GetParentNodes(n.Parent);
            }
        }

        private void usersGrid_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            var cmbRole = e.Control as ComboBox;
            if (cmbRole != null)
            {
                cmbRole.SelectedIndexChanged -= new EventHandler(cmbRole_SelectedIndexChanged);
                cmbRole.SelectedIndexChanged += new EventHandler(cmbRole_SelectedIndexChanged);
            }
        }

        void cmbRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            Users user = null;
            try
            {
                ComboBox cmbRole = sender as ComboBox;
                if (cmbRole != null)
                {
                    if (cmbRole.SelectedValue != null)
                    {
                        var value = (int)cmbRole.SelectedValue;
                        user = this.usersGrid.CurrentRow.DataBoundItem as Users;
                        if (user.RoleId != value)
                        {
                            user.RoleId = value;
                            bool updated = this._presenter.UpdateUser(user);

                            if (updated) MessageBox.Show("User details updated", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch
            {
                //this.usersGrid.CurrentCell.Value = temp;
            }
        }

        private void usersGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                Users user = this.usersGrid.CurrentRow.DataBoundItem as Users;
                bool isDeleted = this._presenter.DeleteUser(user);
                if (isDeleted)
                {
                    var group = this.trGroups.SelectedNode.Tag as Groups;
                    GetUsers(group);
                }                
            }
        }    

       

        private void groupMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var text = e.ClickedItem.Text;

            CheckAccess();

            if (text == "Add Group")
            {
                Groups g = this.trGroups.SelectedNode.Tag as Groups;
                if (g == null)
                {
                    g = new Entities.Groups();
                    g.ParentId = 0;                    
                }
                g.Action = Infosys.ATR.Admin.Entities.Action.Add;
                this._presenter.ShowGroupDetails_Handler(g);                
            }
            else if (text == "Refresh")
            {
                this.trGroups.Nodes.Clear();
                Initialize();
            }
            else if (text == "Delete")
            {
                Groups g = this.trGroups.SelectedNode.Tag as Groups;
                DeleteGroup(this.trGroups.SelectedNode.Nodes);
                var isDeleted = this._presenter.DeleteGroup(g);
                if (isDeleted) MessageBox.Show("Group Deleted", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.trGroups.Nodes.Clear();
                Initialize();
            }
        }

        private void DeleteGroup(TreeNodeCollection n)
        {
            foreach (TreeNode child in n)
            {
                Groups g = child.Tag as Groups;
                this._presenter.DeleteGroup(g);
                DeleteGroup(child.Nodes);
            }          
        }

        public void CheckAccess()
        {
            var selectednode = this.trGroups.SelectedNode;

            if (selectednode != null)
            {
                Groups g = selectednode.Tag as Groups;

                if (g != null)
                {

                    bool issuperAdmin = (bool)
                        this._presenter.WorkItem.RootWorkItem.Items["IsSuperAdmin"];

                    if (g.CompanyId == 0 && !issuperAdmin)
                    {
                        this.groupMenu.Items[0].Enabled =
                            this.groupMenu.Items[1].Enabled = false;
                    }
                    else
                    {
                        this.groupMenu.Items[0].Enabled =
                           this.groupMenu.Items[1].Enabled = true;
                    }
                }
            }
        }

        public void RefershCategory(string node)
        {
            this.trGroups.Nodes.Clear();
            Initialize();
            this.trGroups.ExpandAll();
            this.trGroups.Refresh();

            if (!string.IsNullOrEmpty(node))
            {
                TreeNode selected = null;
                foreach (TreeNode trnode in trGroups.Nodes)
                {
                    selected = GetNode(trnode, node);
                    if (selected != null) break;
                }

                if (selected != null)
                    trGroups.SelectedNode = selected;
            }
        }

        private TreeNode GetNode(TreeNode n, string text)
        {
            foreach (TreeNode node in n.Nodes)
            {
                if (node.Text.Equals(text)) return node;
                TreeNode next = GetNode(node, text);
                if (next != null) return next;
            }
            return null;
        }

        private void groupMenu_Opening(object sender, CancelEventArgs e)
        {
            CheckAccess();
        }

        private void addGroup_Click(object sender, EventArgs e)
        {

        }

        public void Close()
        {
            this._presenter.OnCloseView();
        }


        public void RefreshGroupUsers()
        {
            if (this.trGroups.SelectedNode != null)
            {
                var e = this.trGroups.SelectedNode;
                var group = e.Tag as Groups;
                GetUsers(group);
            }
        }

        private void usersGrid_Paint(object sender, PaintEventArgs e)
        {
    
        }

  
    }
}

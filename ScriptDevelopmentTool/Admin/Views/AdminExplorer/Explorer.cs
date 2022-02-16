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
    public partial class Explorer : UserControl, IExplorer
    {
        TreeNode _root;
        TreeNode _semanticRoot;
        List<int> children = new List<int>();
        List<int> parent = new List<int>();
        public List<Groups> Groups { get; set; }
        public BindingList<Users> Users { get; set; }
        public TreeNode SelectedNode { get; set; }
        public List<SemanticGroup> Clusters { get; set; }

        private bool AddCategory = false;
        internal bool ShowExplorer { get; set; }

        public Explorer()
        {
            InitializeComponent();
            this.ShowExplorer = true;
            this._centerWorkspace.Name = Constants.WorkspaceNames.CenterWorkspace;
            this._rightWorkspace.Name = Constants.WorkspaceNames.RightWorkspace;
        }

        internal void Initialize()
        {
            //this._presenter.GetAllGroups();            
            this._presenter.GetCategories();            
            this._presenter.GetAllUsers();
            this.ShowExplorer = this._presenter.ShowExplorer;

            _root = new TreeNode("Categories");
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

            CreateSemanticTree();
        }

        private void CreateSemanticTree()
        {
            this._presenter.GetClusters();           
            this._presenter.WorkItem.State["SemanticCluster"] = Clusters;
            _semanticRoot = new TreeNode("Semantic Groups");
            this.trGroups.Nodes.Add(_semanticRoot);
            if (Clusters != null)
            {
                Clusters.ForEach(c =>
                {
                    TreeNode cluster = new TreeNode(c.Name);
                    cluster.Tag = c;
                    _semanticRoot.Nodes.Add(cluster);
                });
            }
            _semanticRoot.ExpandAll();
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
            if (e.Node != null)
            {
                SelectedNode = e.Node;
                if (SelectedNode.Text == "Semantic Groups")
                {
                    CloseWorkspace();
                    this._presenter.ShowSemanticCluster_Handler(new SemanticGroup { Action = Entities.Action.Add });
                    this.usersGrid.DataSource = null;
                }
                else if (SelectedNode.Tag != null && SelectedNode.Tag.GetType() == typeof(SemanticGroup))
                {
                    CloseWorkspace();
                    var sg = SelectedNode.Tag as SemanticGroup;
                    sg.Action = Entities.Action.View;
                    this._presenter.ShowSemanticCluster_Handler(sg);
                    this.usersGrid.DataSource = null;
                }
                else
                {
                    var group = e.Node.Tag as Groups;
                    GetUsers(group);
                }
            }
        }

        void GetUsers(Groups group)
        {
            // var group = e.Node.Tag as Groups;

            if (group != null)
            {
                this.usersGrid.DataSource = null;

                this._presenter.GetAnyUsers(group);
                this.usersGrid.DataSource = Users;
                this.usersGrid.Columns[0].HeaderText = "";
                this.usersGrid.Columns[7].HeaderText = "";
                //this.usersGrid.Columns[0].Width = 10;
                //this.usersGrid.Columns[1].Width = 250;
                //this.usersGrid.Columns[7].Width = 10;
                this.usersGrid.Columns[2].Visible = this.usersGrid.Columns[3].Visible =
                    this.usersGrid.Columns[4].Visible = this.usersGrid.Columns[5].Visible = this.usersGrid.Columns[6].Visible =
                    this.usersGrid.Columns[8].Visible = this.usersGrid.Columns[9].Visible = false;

                this.usersGrid.Columns[7].DisplayIndex = 1;

                DataGridViewComboBoxColumn roles = new DataGridViewComboBoxColumn();
                roles.HeaderText = "Role";
                roles.DataSource = this._presenter.GetAllRoles();
                roles.DisplayMember = "Name";
                roles.ValueMember = "Id";
                roles.DataPropertyName = "RoleId";
                usersGrid.Columns.Add(roles);
                //roles.Width = 40;
                usersGrid.Refresh();

                group.Action = Entities.Action.View;
                this.children.Clear();
                this.parent.Clear();
                GetParentNodes(this.trGroups.SelectedNode);
                group.Parents = this.parent;
                this.children.Add(group.CategoryId);
                GetChildren(this.trGroups.SelectedNode);
                group.Children = this.children;
                //this._presenter.ShowGroupDetails_Handler(group);
            }
            else
            {
                this.usersGrid.DataSource = null;
                this.usersGrid.Refresh();
                // this._presenter.ShowGroupDetails_Handler(group);
            }
            CloseWorkspace();
            this._presenter.ShowGroupDetails_Handler(group);
        }

        void CloseWorkspace()
        {

            var activesmartpart = this._rightWorkspace.ActiveSmartPart;

            if (activesmartpart != null)
                this._rightWorkspace.Close(activesmartpart);


        }

        private void usersGrid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            usersGrid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;


            if (e.RowIndex != -1)
            {
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
                    this.usersGrid.Columns[1].Width = 550;

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

        public void GetAllNodes(Groups group)
        {
            GetParentNodes(this.trGroups.SelectedNode);
            group.Parents = this.parent;
            this.children.Add(group.CategoryId);
            GetChildren(this.trGroups.SelectedNode);
            group.Children = this.children;
        }

        private void GetParentNodes(TreeNode n)
        {
            if (n.Parent.Text != "Categories")
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

                var dialog = MessageBox.Show("Do you want to delete the selected User?", "IAP", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

                if (dialog == DialogResult.OK)
                {
                    bool isDeleted = this._presenter.DeleteUser(user);
                    if (isDeleted)
                    {
                        var group = this.trGroups.SelectedNode.Tag as Groups;
                        GetUsers(group);
                    }
                }
            }
        }

        internal void NewCategory()
        {
            if (this.trGroups.SelectedNode != null)
            {
                var selectedNode = this.trGroups.SelectedNode;

                //if ((selectedNode.Text == "Semantic Groups"
                //    && (this.trGroups.SelectedNode.Tag == null) ||
                //    this.trGroups.SelectedNode.Tag.GetType() == typeof(SemanticGroup)))

                if(selectedNode.Text == "Semantic Groups" &&
                    selectedNode.Tag == null)
                {
                    //SemanticGroup sc = this.trGroups.SelectedNode.Tag as SemanticGroup;

                    //if (sc == null)
                    ////{
                        SemanticGroup sc = new SemanticGroup();
                   // }
                    sc.Action = Entities.Action.Add;
                    this._presenter.ShowSemanticCluster_Handler(sc);
                }
                else if (selectedNode.Tag !=null &&
                    selectedNode.Tag.GetType() == typeof(SemanticGroup))
                {
                    SemanticGroup sc = this.trGroups.SelectedNode.Tag as SemanticGroup;
                    sc.Action = Entities.Action.Add;
                    this._presenter.ShowSemanticCluster_Handler(sc);
                }
                else
                {
                    Groups g = this.trGroups.SelectedNode.Tag as Groups;

                    if (g == null)
                    {
                        g = new Entities.Groups();
                        g.ParentId = 0;
                    }

                    g.AddCategory = AddCategory;

                    g.Action = Infosys.ATR.Admin.Entities.Action.Add;
                    this._presenter.ShowGroupDetails_Handler(g);
                }

            }
            else
            {
                MessageBox.Show("Select a node to add category", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        internal void Refresh()
        {
            if (this.trGroups.Nodes != null)
            {
                this.trGroups.Nodes.Clear();
                Initialize();
            }
            this.trGroups.ExpandAll();
        }

        internal void DeleteCategory()
        {
            var selectedNode = this.trGroups.SelectedNode;
            if (selectedNode != null)
            {
                if (selectedNode.Tag != null && selectedNode.Tag.GetType() == typeof(SemanticGroup))
                {
                    SemanticGroup sc = selectedNode.Tag as SemanticGroup;
                    sc.Action = Entities.Action.Delete;
                    this._presenter.ShowSemanticCluster_Handler(sc);
                }
                else
                {

                    Groups g = selectedNode.Tag as Groups;

                    var dialog = MessageBox.Show("Deleting category will also delete all the semantic groups associated with it. Do you want to continue?", "IAP", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

                    if (dialog == DialogResult.OK)
                    {
                        DeleteGroup(this.trGroups.SelectedNode.Nodes);
                        var isDeleted = this._presenter.DeleteGroup(g);
                        if (isDeleted)
                        {
                            this._presenter.CategoryDeleted_Handler(g.CategoryId);
                            MessageBox.Show("Category Deleted", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        this.trGroups.Nodes.Clear();
                        Initialize();
                        this.trGroups.ExpandAll();
                        this._presenter.CategoryDeleted_Handler(g.CategoryId);
                    }
                }
            }
            else
            {
                MessageBox.Show("Select a node to delete", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void groupMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var text = e.ClickedItem.Text;

            CheckAccess();
            AddCategory = false;
            if (text == "Add")
            {
                AddCategory = true;
                NewCategory();
            }
            else if (text == "Refresh")
            {
                Refresh();
            }
            else if (text == "Delete")
            {
                DeleteCategory();
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

                bool issuperAdmin = (bool)
                        this._presenter.WorkItem.RootWorkItem.Items["IsSuperAdmin"];

                if (g != null)
                {
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
                else
                {
                    if (!issuperAdmin)
                        this.groupMenu.Items[0].Enabled =
                            this.groupMenu.Items[1].Enabled = false;
                }
            }
        }



        public void RefreshCategory(string node)
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

        private void groupMenu_Opening(object sender, CancelEventArgs e)
        {
            CheckAccess();
        }

        private void addGroup_Click(object sender, EventArgs e)
        {

        }


        public void DisplayMessage(string p)
        {
            MessageBox.Show(p, "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void RefreshUsers()
        {
            if (this.trGroups.SelectedNode != null)
            {
                var e = this.trGroups.SelectedNode;
                var group = e.Tag as Groups;
                GetUsers(group);
            }
        }

        public void RefreshSemanticTree()
        {
            this.trGroups.Nodes.RemoveAt(1);
            CreateSemanticTree();
        }

        private void usersGrid_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            //switch (e.Column.Index)
            //{
            //    case 0:
            //    case 7:
            //        e.Column.Width = 10;
            //        break;
            //    case 1:
            //        e.Column.Width = 250;
            //        break;
            //}
        }


    }
}

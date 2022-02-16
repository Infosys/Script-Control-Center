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
using System.Collections.ObjectModel;

namespace Infosys.ATR.Admin.Views
{
    public partial class SemanticCluster : UserControl, ISemanticCluster, IClose
    {
        public SemanticGroup Cluster { get; set; }
        public List<String> Domains { get; set; }
        public BindingList<Nodes> Nodes { get; set; }
        public BindingList<Nodes> Selected { get; set; }
        public BindingList<Nodes> DomainNodes { get; set; }

        public SemanticCluster()
        {
            InitializeComponent();
        }


        public void Close()
        {
            this._presenter.OnCloseView();
        }

        public new void Show()
        {
            if (Selected != null)
            {
                Selected.Clear();
                Selected = null;
            }

            if (DomainNodes != null)
            {
                DomainNodes.Clear();
                DomainNodes = null;
            }

            this.listBox1.DataSource = null;
            this.listBox2.DataSource = null;

            this._presenter.GetDomains();
            this.cmbDomains.DataSource = Domains;

            Selected = new BindingList<Nodes>();
            DomainNodes = new BindingList<Nodes>();

            if (Cluster != null)
            {
                if (Cluster.Action == Entities.Action.View)
                {
                    View();
                }
                else if (Cluster.Action == Entities.Action.Add)
                {
                    Add();
                }
                else if (Cluster.Action == Entities.Action.Delete)
                {
                    Delete();
                }

                var semanticCluster = this._presenter.WorkItem.SmartParts.Get("SemanticCluster");

                if (semanticCluster == null)
                {
                    semanticCluster = this._presenter.WorkItem.SmartParts.AddNew<Admin.Views.GroupDetails>("SemanticCluster");
                }

                this._presenter.WorkItem.Workspaces[Constants.WorkspaceNames.RightWorkspace].Show(semanticCluster);
            }

            CheckAccess();

            Filter();

            PopulateSelectedNodes(DomainNodes);
        }

        /// <summary>
        /// show only nodes in cluster associated with a domain and
        /// do not show nodes in active nodes if the nodes are already associated with the cluster
        /// </summary>
        private void Filter()
        {
            if (Nodes != null && Selected != null)
            {
                FilterOnDomain();

                DomainNodes.ToList().ForEach(s =>
                {
                    var node1 = Nodes.FirstOrDefault(n => n.Name == s.Name);
                    if (node1 != null) Nodes.Remove(node1);

                });
            }
        }

        private void CheckAccess()
        {
            bool issuperAdmin = (bool)
                       this._presenter.WorkItem.RootWorkItem.Items["IsSuperAdmin"];

            if (!issuperAdmin)
            {
                foreach (Control c in this.Controls)
                    c.Enabled = false;
            }
        }

        void View()
        {
            this.txtName.Text = Cluster.Name;
            this.txtDescription.Text = Cluster.Description;
            GetActiveNodes();

            button1.Enabled = false;
            button2.Enabled = true;
        }

        void Delete()
        {
            this.txtName.Text = Cluster.Name;
            this.txtDescription.Text = Cluster.Description;
            GetActiveNodes();
            button1.Enabled = false;
            button2.Enabled = true;

            var result = MessageBox.Show("Deleting a Semantic Group will remove it from all the associated Categories.Do you want to continue?",
                "IAP", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
            {
                this._presenter.UpdateCluster(Cluster, false);

                this._presenter.UpdateSemanticTree_Handler(null);
            }
        }

        void GetActiveNodes()
        {
            if (Cluster.Action != Entities.Action.Add
                && Cluster.ActiveNodes != null)
            {
                Cluster.ActiveNodes.ForEach(n => Selected.Add(n));
            }
            PopulateSelectedNodes(DomainNodes);
        }

        void Add()
        {
            this.txtName.Text = "";
            this.txtDescription.Text = "";
            GetActiveNodes();
            this.button1.Enabled = true;
            this.button2.Enabled = false;
        }

        void PopulateActiveNodes()
        {
            this.listBox1.DataSource = Nodes;
            this.listBox1.DisplayMember = "Name";
            this.listBox1.ValueMember = "Domain";
            this.listBox1.Tag = typeof(Nodes);
            this.listBox1.Refresh();
        }

        void PopulateSelectedNodes(BindingList<Nodes> BindNodes)
        {            
            var ds = BindNodes.Where(n => n.State == "Active").ToList();
            this.listBox2.DataSource = ds;
            this.listBox2.DisplayMember = "Name";
            this.listBox2.ValueMember = "Domain";
            this.listBox2.Tag = typeof(Nodes);
            this.listBox2.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SemanticGroup sg = null;

            if (DomainNodes.Count > 0)
            {
                if (!string.IsNullOrEmpty(txtName.Text))
                {
                    if (txtName.Text.Length <= 20)
                    {
                        int len = txtName.Text.Length > 10 ? 10 : txtName.Text.Length;
                        sg = new SemanticGroup
                        {
                            Name = txtName.Text,
                            Description = txtDescription.Text,
                            CreatedBy = System.Threading.Thread.CurrentPrincipal.Identity.Name
                        };

                        var response = this._presenter.AddCluster(sg);

                        if (response != null)
                        {
                            this._presenter.AddClusterNode(response, DomainNodes);
                            MessageBox.Show("Semantic Cluster Created", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Cluster cannot be created. Try giving a unique name", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        this._presenter.UpdateSemanticTree_Handler(sg);
                    }
                }
                else
                    MessageBox.Show("Cluster cannot be created without a name", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("Cluster cannot be created without any nodes", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmbDomains_SelectedIndexChanged(object sender, EventArgs e)
        {
            var value = this.cmbDomains.SelectedValue.ToString();
            if (!string.IsNullOrEmpty(value))
            {
                this._presenter.GetActiveNodes(value);
                PopulateActiveNodes();
                Filter();
            }
        }

        private void contextMenuStrip1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            //var n = this.listBox1.SelectedItem as Nodes;
            if (!chkShowAll.Checked)
            {

                var items = this.listBox1.SelectedItems;
                if (items != null)
                {
                    List<Nodes> nodesToRemove = new List<Nodes>();
                    for (int i = 0; i < items.Count; i++)
                    {
                        var n = items[i] as Nodes;
                        if (n != null)
                        {
                            var one = DomainNodes.FirstOrDefault(s => s.Name == n.Name && s.Domain == n.Domain);
                            if (one == null)
                            {
                                n.State = "Active";                               
                                DomainNodes.Add(n);
                            }
                            else if (one.State == "InActive")
                            {
                                one.State = "Active";
                            }
                        }
                        nodesToRemove.Add(n);
                    }

                    nodesToRemove.ForEach(nr => Nodes.Remove(nr));

                    PopulateActiveNodes();
                    PopulateSelectedNodes(DomainNodes);

                }
                
            }
            else
            {
                MessageBox.Show("Nodes cannot be moved when Show All is checked", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!chkShowAll.Checked)
            {
                var items = this.listBox2.SelectedItems;

                if (items != null)
                {
                    List<Nodes> nodesToRemove = new List<Nodes>();

                    for (int i = 0; i < items.Count; i++)
                    {
                        var n = items[i] as Nodes;

                        var one = Nodes.FirstOrDefault(s => s.Name == n.Name && s.Domain == n.Domain);
                        if (one == null)
                        {
                            Nodes.Add(n);
                        }

                        var r = DomainNodes.FirstOrDefault(n1 => n1.Name == n.Name && n1.Domain == n.Domain);
                        r.State = "InActive";                        
                        nodesToRemove.Add(n);
                    }

                    PopulateActiveNodes();
                    PopulateSelectedNodes(DomainNodes);
                }
            }
            else
            {
                MessageBox.Show("Nodes cannot be moved when Show All is checked", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (DomainNodes.Count > 0)
            {
                if (!string.IsNullOrEmpty(txtName.Text))
                {
                    if (txtName.Text.Length <= 20)
                    {
                        int len = txtName.Text.Length > 10 ? 10 : txtName.Text.Length;

                        Cluster.NewName = txtName.Text;
                        Cluster.Description = txtDescription.Text;
                        Cluster.ActiveNodes.Clear();

                        for (int i = 0; i < DomainNodes.Count; i++)
                        {
                            var t = DomainNodes[i];
                            Cluster.ActiveNodes.Add(new Entities.Nodes
                            {
                                CompanyId = t.CompanyId,
                                Domain = t.Domain,
                                DotNetVersion = t.DotNetVersion,
                                ExecutionEngineSupported = t.ExecutionEngineSupported,
                                HttpPort = t.HttpPort,
                                Is64Bit = t.Is64Bit,
                                Name = t.Name,
                                OSVersion = t.OSVersion,
                                State = t.State,
                                TcpPort = t.TcpPort,
                                WorkflowServiceVersion = t.WorkflowServiceVersion

                            });
                        }

                        var response = this._presenter.UpdateCluster(Cluster, true);

                        if (response)
                            this._presenter.UpdateClusterNode(Cluster.Id, DomainNodes, Cluster.Id);
                        else
                        {
                            MessageBox.Show("Semantic Cluster not Updated. Try giving an unique name.", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }

                    MessageBox.Show("Semantic Cluster Updated", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this._presenter.UpdateSemanticTree_Handler(Cluster);
                }
                else
                    MessageBox.Show("Cluster cannot be updated without a name", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("Cluster cannot be updated without any nodes", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void chkShowAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShowAll.Checked)
            {
                //hack
                for (int i = 0; i < Selected.Count; i++)
                    Selected[i].State = "Active";
                PopulateSelectedNodes(Selected);
            }
            else
            {
                FilterOnDomain();
                Filter();
                
            }
        }

        private void FilterOnDomain()
        {
            if (!chkShowAll.Checked)
            {
                if (DomainNodes != null)
                {
                    DomainNodes.Clear();

                    for (int i = 0; i < Selected.Count; i++)
                    {
                        var t = Selected[i];

                        if (t.Domain == this.cmbDomains.SelectedValue.ToString())
                        {
                            //hack
                            t.State = "Active";
                            DomainNodes.Add(t);
                        }
                    }

                }
                PopulateSelectedNodes(DomainNodes);
            }
        }

    }
}

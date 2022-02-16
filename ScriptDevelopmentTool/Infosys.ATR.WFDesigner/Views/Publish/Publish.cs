using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Configuration;

using Infosys.ATR.WFDesigner.Entities;

namespace Infosys.ATR.WFDesigner.Views
{
    public partial class Publish : UserControl, IPublish, IClose
    {
        internal WFDesigner wfDe = null;
        TreeNode _root;
        public List<Category> Categories { get; set; }

        public Publish()
        {
            InitializeComponent();
        }

        public bool Close()
        {
            this._presenter.OnCloseView();
            return true;
        }

        internal void Initialize()
        {
            TreeNode selectedNode = null;
            trCat.Nodes.Clear();
            this._presenter.GetCategories();

            _root = new TreeNode("Categories");

            this.trCat.Nodes.Add(_root);
            _root.Expand();

            if (Categories != null)
            {

                Categories.Where(c => c.ParentId == 0).ToList().
                    ForEach(sc =>
                    {
                        AddNode(sc, null);
                    });
            }

            if (wfDe.data != null)
            {
                txtName.Text = wfDe.data.Name;
                txtDescription.Text = wfDe.data.Description;
                int catId = wfDe.data.CategoryID;
                //if (wfDe.data.SubCategoryID > 0)
                //    catId = wfDe.data.SubCategoryID;

                // Find cat/sub to be selected in case of editing of workflow
                foreach (TreeNode node in trCat.Nodes)
                {
                    selectedNode = FindSelectedNode(catId, node);
                    if (selectedNode != null) break;
                }
                if (selectedNode != null)
                {
                    this.trCat.SelectedNode = selectedNode;
                    selectedNode.Expand();
                    this.trCat.HideSelection = false;
                    this.trCat.Focus();
                }
            }
        }



        /// <summary>
        /// This adds category/subcategory node.
        /// </summary>
        /// <param name="c">WorkflowCategoryMaster object</param>
        /// <param name="parent">Parent Node</param>
        private void AddNode(Category c, TreeNode parent)
        {
            if (parent == null)
            {
                parent = new TreeNode();
                parent.Text = c.Name;
                parent.Tag = c;
                _root.Nodes.Add(parent);
            }
            else
            {
                TreeNode child = new TreeNode();
                child.Text = c.Name;
                child.Tag = c;
                parent.Nodes.Add(child);
                parent = child;
            }

            Categories.Where(sc => sc.ParentId == c.CategoryId).ToList().ForEach(child =>
            {
                AddNode(child, parent);
            });

        }
        public TreeNode FindSelectedNode(int catId, TreeNode rootNode)
        {
            foreach (TreeNode node in rootNode.Nodes)
            {
                Category category = node.Tag as Category;
                if (category != null)
                {
                    if (category.CategoryId.Equals(catId)) return node;
                    TreeNode next = FindSelectedNode(catId, node);
                    if (next != null) return next;
                }
            }
            return null;
        }

        private void btnPublsh_Click(object sender, EventArgs e)
        {
            WorkflowPE pe = null;
            int version = 0;
            if (!ValidationPassed())
            {
                MessageBox.Show("Please provide the details for all these Workflow fields- Name, Cateory.", "Data Missing...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                var iapworkflowstore = ConfigurationManager.AppSettings["IapWorkflowStore"];
                var cat = trCat.SelectedNode.Tag as Category;
                TreeNode node = trCat.SelectedNode;

                if (wfDe.data == null)
                {
                    Guid guid = Guid.NewGuid();
                    WorkflowPE data = new WorkflowPE();

                    data.CategoryID = cat.CategoryId;

                    data.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    data.Description = txtDescription.Text;
                    data.Name = txtName.Text;
                    data.WorkflowID = guid;
                    version = 1;
                    data.FileName = "1.xaml";
                    data.CompanyId = Convert.ToInt32(ConfigurationManager.AppSettings["Company"]);
                    data.WorkflowURI = String.Format(iapworkflowstore + "/{0}/{1}", guid, data.FileName);
                    data.IncrementVersion = true;
                    var repsonse = wfDe.Upload(data);
                    pe = this._presenter.Publish(wfDe.data);
                    MessageBox.Show("Workflow Published Successfully", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    wfDe.data.WorkflowVersion = version;
                }
                // if editing and changes done in workflow
                else if (wfDe.data != null && wfDe.isDirty)
                {
                    wfDe.data.CategoryID = cat.CategoryId;
                    wfDe.data.Description = txtDescription.Text;
                    wfDe.data.Name = txtName.Text;
                    var fileName = Convert.ToInt16(wfDe.data.FileName.Split('.')[0]);
                    version = fileName + 1;
                    wfDe.data.FileName = (fileName + 1).ToString() + ".xaml";
                    wfDe.data.WorkflowURI = String.Format(iapworkflowstore + "/{0}/{1}", wfDe.data.WorkflowID, wfDe.data.FileName);
                    wfDe.data.IncrementVersion = true;
                    var response = wfDe.Upload(wfDe.data);
                    pe = this._presenter.Update(wfDe.data);
                    MessageBox.Show("Workflow Updated Successfully", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    wfDe.data.WorkflowVersion = version;
                }
                // if  no changes have been done in workflow
                else if (wfDe.data != null && !wfDe.isDirty)
                {
                    int catId;
                    wfDe.data.Description = txtDescription.Text;
                    wfDe.data.Name = txtName.Text;

                    catId = cat.CategoryId;

                    wfDe.data.FileName = wfDe.data.FileName + "." + "xaml";
                    wfDe.data.WorkflowURI = String.Format(iapworkflowstore + "/{0}/{1}", wfDe.data.WorkflowID, wfDe.data.FileName);

                    // if only metadata changes e.g. name and/or desc has been updated
                    if (wfDe.data.CategoryID == catId)//&& wfDe.data.SubCategoryID == subCatId)
                    {
                        pe = this._presenter.Publish(wfDe.data);
                    }
                    else
                    {
                      //  var fileName = Convert.ToInt16(wfDe.data.FileName.Split('.')[0]);
                      //  wfDe.data.FileName = (fileName + 1).ToString() + ".xaml";
                        wfDe.data.CategoryID = catId;
                        wfDe.data.IncrementVersion = false;
                        pe = this._presenter.Update(wfDe.data);
                    }

                    MessageBox.Show("Workflow Updated Successfully", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                }

               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "IAP", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                wfDe.isDirty = false;
                this._presenter.OnCloseView();                
               // wfDe.AddPropertyTab(wfDe.data);
            }
        }

        private bool ValidationPassed()
        {
            bool isPass = true;
            if (String.IsNullOrEmpty(txtName.Text))
            {
                isPass = false;
                return isPass;
            }

            if (trCat.SelectedNode == null || trCat.SelectedNode.Tag == null)
            {
                isPass = false;
                return isPass;
            }
            return isPass;
        }

        private void trCat_AfterSelect(object sender, TreeViewEventArgs e)
        {
            trCat.HideSelection = false;
        }
    }
}

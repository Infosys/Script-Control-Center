using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Linq.Expressions;

using WEMClient = Infosys.WEM.Client;
using Infosys.WEM.Scripts.Service.Contracts.Message;
using System.Diagnostics;
using Infosys.WEM.ScriptExecutionLibrary;

namespace Infosys.ATR.ScriptRepository.Views
{
    public partial class MainRepositoryView : UserControl
    {
        TreeNodeContext currentNode;
        bool asc = false;
        WEMClient.ScriptRepository scriptClient = new WEMClient.ScriptRepository();
        string _categorySelected = "", _subcategorySelected = "";
        private const string SCRIPTRESULT = "Script Execution Result";
        //List<Models.Category> _categories;

        private int selectedScriptIndex = 0;
        public MainRepositoryView()
        {
            InitializeComponent();
            splitContainerDetails.Panel2.VerticalScroll.Visible = true;
        }

        public void LoadCategory(List<Models.Category> categories = null)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                if (categories == null)
                {
                    //fetch the categories and assign before population
                    //ScriptRepo scriptClient = new ScriptRepo("http://localhost:61335/WEMScriptService.svc");
                    //WEMClient.ScriptRepository scriptClient = new WEMClient.ScriptRepository();
                    GetAllCategoryAndSubcategoryResMsg response = scriptClient.ServiceChannel.GetAllCategoryAndSubcategory();
                    categories = Translators.CategoryTreePE_SE.CategoryListSEtoPE(response.Categories);
                }
                PopulateCategories(categories);

            }
            catch (Exception ex)
            {
                string err = ex.Message;
                if (ex.InnerException != null)
                    err = err + ". \nInner Exception- " + ex.InnerException.Message;
                MessageBox.Show(err, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }

        private void PopulateCategories(List<Models.Category> categories)
        {
            //first clear the tree view
            tvCatSubcat.Nodes.Clear();
            if (categories != null)
            {
                List<TreeNode> categoryNodes = new List<TreeNode>();
                categories.ForEach(category =>
                {
                    TreeNode categoryNode = new TreeNode(category.Name);
                    categoryNode.Tag = category;
                    categoryNodes.Add(categoryNode);
                    PopulateSubCategories(categoryNode, category.SubCategories);
                });
                //create the root node
                TreeNode root = new TreeNode("Categories", categoryNodes.ToArray());
                root.Tag = categories;
                root.Expand();
                tvCatSubcat.Nodes.Add(root);
                tvCatSubcat.SelectedNode = root;
            }
            dgList.Columns[0].Visible = false;
        }

        private void PopulateSubCategories(TreeNode categoryNode, List<Models.SubCategory> subCategories)
        {
            if (subCategories != null)
            {
                List<TreeNode> subCategoryNodes = new List<TreeNode>();
                subCategories.ForEach(subCat =>
                {
                    TreeNode subcategoryNode = new TreeNode(subCat.Name);
                    subcategoryNode.Tag = subCat;
                    subCategoryNodes.Add(subcategoryNode);
                    PopulateScripts(subcategoryNode, subCat.Scripts);
                });
                categoryNode.Nodes.AddRange(subCategoryNodes.ToArray());
            }
            dgList.Columns[0].Visible = false;
        }

        private void PopulateScripts(TreeNode subCategoryNode, List<Models.Script> scripts)
        {
            subCategoryNode.Nodes.Clear();
            if (scripts != null && scripts.Count > 0)
            {
                List<TreeNode> scriptNodes = new List<TreeNode>();
                scripts.ForEach(script =>
                {
                    TreeNode scriptNode = new TreeNode(script.Name);
                    scriptNode.Tag = script;
                    scriptNodes.Add(scriptNode);
                });
                subCategoryNode.Nodes.AddRange(scriptNodes.ToArray());
                dgList.Columns[1].Visible = true;
            }
            else
            {
                //add a dummy node
                TreeNode dummyNode = new TreeNode("Please wait...");
                Models.SubCategory tempsubcat = (Models.SubCategory)subCategoryNode.Tag;
                if (tempsubcat != null)
                {
                    dummyNode.Tag = new DummyNodeObject() { Id = tempsubcat.Id, Name = tempsubcat.Name };
                    subCategoryNode.Nodes.Add(dummyNode);
                }
            }

        }

        private void tvCatSubcat_MouseClick(object sender, MouseEventArgs e)
        {
            //select the concerned node
            //Point loc = ((TreeView)sender).PointToClient(new Point(e.X, e.Y));
            //TreeNode node = ((TreeView)sender).GetNodeAt(loc);
            runScriptLocallyMenuItem.Enabled = false;
            toolbarRunScript.Enabled = false;
        }

        private void tvCatSubcat_AfterExpand(object sender, TreeViewEventArgs e)
        {
            runScriptLocallyMenuItem.Enabled = false;
            toolbarRunScript.Enabled = false;
            tvCatSubcat.SelectedNode = e.Node;
            if (e.Node.Tag.GetType() == typeof(Models.SubCategory))
            {
                FetchScripts(e.Node);
            }
        }

        private void dgList_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dgList.DataSource == null)
                return;
            if (e.RowIndex < 0)
                return;
            this.Cursor = Cursors.WaitCursor;
            if (e.ColumnIndex == 1)
            {
                //i.e. Delete is called
                if (MessageBox.Show("Are you sure you want to delete?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    //delete the row selected
                    try
                    {
                        switch (currentNode)
                        {
                            case TreeNodeContext.Root:
                                DeleteCategory(e.RowIndex);
                                break;
                            case TreeNodeContext.Category:
                                DeleteSubCategory(e.RowIndex);
                                break;
                            case TreeNodeContext.SubCategory:
                            case TreeNodeContext.Script:
                                DeleteScript(e.RowIndex);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        string err = ex.Message;
                        if (ex.InnerException != null)
                            err = err + ". \nInner Exception- " + ex.InnerException.Message;
                        MessageBox.Show(err, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            // Logic to run the script
            else if (e.ColumnIndex == 0)
            {
                RunScriptLocally(e.RowIndex);
            }
            else
            {
                splitContainerDetails.Panel2.Controls.Clear();
                switch (currentNode)
                {
                    case TreeNodeContext.Root:
                        //i.e. category is listed in the grid view
                        ShowCategoryDetails(e.RowIndex);
                        break;
                    case TreeNodeContext.Category:
                        //i.e. sub category is listed in the grid view
                        ShowSubCategoryDetails(e.RowIndex);
                        break;
                    case TreeNodeContext.SubCategory:
                        //i.e. script is listed in the grid view
                        ShowScriptDetails(e.RowIndex);
                        break;
                    case TreeNodeContext.Script:
                        ShowScriptDetails(e.RowIndex);
                        break;
                }
            }
            this.Cursor = Cursors.Default;
        }

        private void RunScriptLocally(int selectedRowIndex)
        {
            ScriptIndentifier scriptIden = new ScriptIndentifier();
            Models.SubCategory subcategory = null;

            if ((tvCatSubcat.SelectedNode.Tag.GetType() == typeof(Models.SubCategory)))
            {
                _categorySelected = (tvCatSubcat.SelectedNode.Parent.Tag as Models.Category).Id;
                subcategory = tvCatSubcat.SelectedNode.Tag as Models.SubCategory;
            }
            else if ((tvCatSubcat.SelectedNode.Tag.GetType() == typeof(Models.Script)))
            {
                subcategory = tvCatSubcat.SelectedNode.Parent.Tag as Models.SubCategory;
            }
            if (subcategory != null)
            {
                Models.Script selectedscript = subcategory.Scripts[selectedRowIndex];

                int executionScriptId = 0;
                int subCatID = 0;
                if (Int32.TryParse(selectedscript.Id, out executionScriptId))
                {
                    scriptIden.ScriptId = executionScriptId;
                }
                if (Int32.TryParse(subcategory.Id, out subCatID))
                {
                    scriptIden.SubCategoryId = subCatID;
                }

                if (executionScriptId > 0 && subCatID > 0)
                {
                    ExecutionResult resutl = ScriptExecutionManager.Execute(scriptIden);

                    if (resutl.IsSuccess)
                        MessageBox.Show("Script executed successfully. Please refer to below details for output.\n\n" + resutl.SuccessMessage, SCRIPTRESULT);
                    else
                        MessageBox.Show("Error in script execution. Refer to following details.\n\n" + resutl.ErrorMessage, SCRIPTRESULT);
                }
                else
                {
                    MessageBox.Show("Invalid Script", SCRIPTRESULT);
                }
            }
        }

        private void tvCatSubcat_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            splitContainerDetails.Panel2.Controls.Clear();
            if (e.Node.Tag != null)
            {
                if (e.Node.Tag.GetType() == typeof(Models.Category))
                {
                    currentNode = TreeNodeContext.Category;
                    Models.Category cat = e.Node.Tag as Models.Category;
                    _categorySelected = cat.Id;
                    if (cat.SubCategories != null)
                    {
                        dgList.DataSource = GetSubCategorySubset(cat.SubCategories);
                        //select the first row if any
                        if (dgList.Rows.Count > 0)
                        {
                            dgList.Rows[0].Selected = true;
                            dgList_CellMouseClick(new object(), new DataGridViewCellMouseEventArgs(2, 0, 0, 0, new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, 0, 0, 0)));
                        }
                    }
                    else
                    {
                        //clean the grid
                        dgList.DataSource = null;
                        dgList.Columns[0].Visible = false;
                        dgList.Columns[1].Visible = false;
                    }

                }
                else if (e.Node.Tag.GetType() == typeof(Models.SubCategory))
                {
                    currentNode = TreeNodeContext.SubCategory;
                    Models.SubCategory subcat = e.Node.Tag as Models.SubCategory;
                    _subcategorySelected = subcat.Id;
                    if (subcat.Scripts != null)
                    {
                        //dgList.DataSource = subcat.Scripts;
                        dgList.DataSource = GetScriptSubset(subcat.Scripts);
                        //select the first row if any
                        if (dgList.Rows.Count > 0)
                        {
                            dgList.Rows[0].Selected = true;
                            dgList_CellMouseClick(new object(), new DataGridViewCellMouseEventArgs(2, 0, 0, 0, new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, 0, 0, 0)));

                        }
                    }
                    else
                    {
                        FetchScripts(e.Node);
                    }
                }
                else if (e.Node.Tag.GetType() == typeof(Models.Script))
                {
                    currentNode = TreeNodeContext.Script;
                    Models.SubCategory subcat = e.Node.Parent.Tag as Models.SubCategory;
                    if (subcat.Scripts != null)
                    {
                        //dgList.DataSource = subcat.Scripts;
                        dgList.DataSource = GetScriptSubset(subcat.Scripts);
                        //highlight the script in the grid view
                        if (dgList.Rows.Count > e.Node.Index)
                        {
                            dgList.Rows[e.Node.Index].Selected = true;
                            dgList_CellMouseClick(new object(), new DataGridViewCellMouseEventArgs(2, e.Node.Index, 0, 0, new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, 0, 0, 0)));
                        }
                    }
                    else
                    {
                        //clean the grid
                        dgList.DataSource = null;
                    }
                }
                else if (e.Node.Tag.GetType() == typeof(List<Models.Category>)) //i.e. when the root is selected
                {
                    currentNode = TreeNodeContext.Root;
                    List<Models.Category> cats = e.Node.Tag as List<Models.Category>;
                    if (cats != null)
                    {
                        dgList.DataSource = GetCategorySubset(cats);
                        //select the first row if any
                        if (dgList.Rows.Count > 0)
                        {
                            dgList.Rows[0].Selected = true;
                            dgList_CellMouseClick(new object(), new DataGridViewCellMouseEventArgs(2, 0, 0, 0, new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, 0, 0, 0)));
                        }
                    }
                    else
                    {
                        //clean the grid
                        dgList.DataSource = null;
                    }
                }
            }
            this.Cursor = Cursors.Default;
        }

        private List<Models.CategorySubCategorySubset> GetCategorySubset(List<Models.Category> categories)
        {
            List<Models.CategorySubCategorySubset> objs = new List<Models.CategorySubCategorySubset>();
            categories.ForEach(c =>
            {
                objs.Add(new Models.CategorySubCategorySubset() { Name = c.Name, Description = c.Description });
            });
            return objs;
        }

        private List<Models.CategorySubCategorySubset> GetSubCategorySubset(List<Models.SubCategory> subCategories)
        {
            List<Models.CategorySubCategorySubset> objs = new List<Models.CategorySubCategorySubset>();
            subCategories.ForEach(c =>
            {
                objs.Add(new Models.CategorySubCategorySubset() { Name = c.Name, Description = c.Description });
            });
            return objs;
        }

        private List<Models.ScriptSubSet> GetScriptSubset(List<Models.Script> scripts)
        {
            List<Models.ScriptSubSet> objs = new List<Models.ScriptSubSet>();
            scripts.ForEach(c =>
            {
                objs.Add(new Models.ScriptSubSet() { Name = c.Name, Description = c.Description, ScriptType = c.ScriptType });
            });
            return objs;
        }

        private void ShowCategoryDetails(int index)
        {
            List<Models.Category> categories = tvCatSubcat.SelectedNode.Tag as List<Models.Category>;
            if (categories != null)
            {
                Models.Category catSelected = categories[index];
                Views.CategoryDetails catdetails = new CategoryDetails(catSelected);
                catdetails.CategoryProcessed += new CategoryDetails.CategoryProcessedEventHandler(cat_CategoryProcessed);
                catdetails.Dock = DockStyle.Top;
                splitContainerDetails.Panel2.Controls.Add(catdetails);
                dgList.Columns[1].Visible = true;
            }
        }

        private void ShowSubCategoryDetails(int index)
        {
            Models.Category category = tvCatSubcat.SelectedNode.Tag as Models.Category;
            if (category != null)
            {
                if (category.SubCategories != null && category.SubCategories.Count > index)
                {
                    Models.SubCategory subcatSelected = category.SubCategories[index];
                    Views.SubCategoryDetails subcatdetails = new SubCategoryDetails(subcatSelected, false, category.Id);
                    subcatdetails.SubCategoryProcessed += new SubCategoryDetails.SubCategoryProcessedEventHandler(subcat_SubCategoryProcessed);
                    subcatdetails.Dock = DockStyle.Top;
                    splitContainerDetails.Panel2.Controls.Add(subcatdetails);
                    dgList.Columns[1].Visible = true;
                }
            }
        }

        private void ShowScriptDetails(int index)
        {
            selectedScriptIndex = index;
            runScriptLocallyMenuItem.Enabled = true;
            toolbarRunScript.Enabled = true;
            if (tvCatSubcat.SelectedNode.Tag.GetType() == typeof(Models.SubCategory))
            {
                //set the category selected
                _categorySelected = (tvCatSubcat.SelectedNode.Parent.Tag as Models.Category).Id;
                Models.SubCategory subcategory = tvCatSubcat.SelectedNode.Tag as Models.SubCategory;
                if (subcategory != null)
                {
                    Models.Script selectedscript = subcategory.Scripts[index];
                    Views.ScriptDetails scriptdetails = new ScriptDetails(selectedscript, _categorySelected);
                    scriptdetails.ScriptProcessed += new ScriptDetails.ScriptProcessedEventHandler(script_ScriptProcessed);
                    dgList.Columns[0].Visible = true;
                    scriptdetails.Dock = DockStyle.Top;
                    splitContainerDetails.Panel2.Controls.Add(scriptdetails);
                }
            }
            else if (tvCatSubcat.SelectedNode.Tag.GetType() == typeof(Models.Script))
            {
                //set the category selected
                _categorySelected = (tvCatSubcat.SelectedNode.Parent.Parent.Tag as Models.Category).Id;
                Models.SubCategory subcategory = tvCatSubcat.SelectedNode.Parent.Tag as Models.SubCategory;
                if (subcategory != null)
                {
                    Models.Script selectedscript = subcategory.Scripts[index];
                    Views.ScriptDetails scriptdetails = new ScriptDetails(selectedscript, _categorySelected);
                    scriptdetails.ScriptProcessed += new ScriptDetails.ScriptProcessedEventHandler(script_ScriptProcessed);
                    dgList.Columns[0].Visible = true;
                    scriptdetails.Dock = DockStyle.Top;
                    splitContainerDetails.Panel2.Controls.Add(scriptdetails);
                }
            }
        }

        private void dgList_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //return if the top let header is clicked
            if (e.ColumnIndex == 0 && e.RowIndex == -1)
                return;
            DataGridViewColumn columnClicked = dgList.Columns[e.ColumnIndex];
            if (dgList.Rows.Count > 0 && dgList.DataSource != null)
            {
                Type t = null;
                if (dgList.DataSource.GetType() == typeof(List<Models.CategorySubCategorySubset>))
                {
                    t = typeof(Models.CategorySubCategorySubset);
                    var param = Expression.Parameter(t, "data");
                    var mySortExpression = Expression.Lambda<Func<Models.CategorySubCategorySubset, object>>(Expression.Property(param, columnClicked.Name), param);
                    if (!asc)
                    {
                        asc = true;
                        dgList.DataSource = ((List<Models.CategorySubCategorySubset>)dgList.DataSource).AsQueryable().OrderBy(mySortExpression).ToList();
                    }
                    else
                    {
                        asc = false;
                        dgList.DataSource = ((List<Models.CategorySubCategorySubset>)dgList.DataSource).AsQueryable().OrderByDescending(mySortExpression).ToList();
                    }
                }
                else if (dgList.DataSource.GetType() == typeof(List<Models.ScriptSubSet>))
                {
                    t = typeof(Models.ScriptSubSet);
                    var param = Expression.Parameter(t, "data");
                    var mySortExpression = Expression.Lambda<Func<Models.ScriptSubSet, object>>(Expression.Property(param, columnClicked.Name), param);
                    if (!asc)
                    {
                        asc = true;
                        dgList.DataSource = ((List<Models.ScriptSubSet>)dgList.DataSource).AsQueryable().OrderBy(mySortExpression).ToList();
                    }
                    else
                    {
                        asc = false;
                        dgList.DataSource = ((List<Models.ScriptSubSet>)dgList.DataSource).AsQueryable().OrderByDescending(mySortExpression).ToList();
                    }
                }
            }
        }

        void cat_CategoryProcessed(CategoryDetails.CategoryProcessedArgs e)
        {
            LoadCategory();
        }

        void subcat_SubCategoryProcessed(SubCategoryDetails.SubCategoryProcessedArgs e)
        {
            LoadCategory();
            //then expand the parent category
            ExpandCategory(e.ParentCategoryId);
        }

        void script_ScriptProcessed(ScriptDetails.ScriptProcessedArgs e)
        {
            LoadCategory();
            ExpandSubCategory(e.SubCategoryId, e.CategoryId, e.ScriptId);
        }

        private void DeleteCategory(int index)
        {
            List<Models.Category> categories = tvCatSubcat.SelectedNode.Tag as List<Models.Category>;
            if (categories != null)
            {
                Models.Category catSelected = categories[index];
                catSelected.ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                DeleteCategoryReqMsg req = new DeleteCategoryReqMsg();
                req.Categories = new List<WEM.Scripts.Service.Contracts.Data.Category>();
                req.Categories.Add(Translators.CategoryPE_SE.CategoryPEtoSE(catSelected));
                scriptClient.ServiceChannel.DeleteCategory(req);
            }
            LoadCategory();
        }

        private void DeleteSubCategory(int index)
        {
            Models.Category category = tvCatSubcat.SelectedNode.Tag as Models.Category;
            if (category != null)
            {
                Models.SubCategory subcatSelected = category.SubCategories[index];
                subcatSelected.ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                DeleteCategoryReqMsg req = new DeleteCategoryReqMsg();
                req.Categories = new List<WEM.Scripts.Service.Contracts.Data.Category>();
                req.Categories.Add(Translators.CategoryPE_SE.SubCategoryPEtoSE(subcatSelected, category.Id));
                scriptClient.ServiceChannel.DeleteCategory(req);
            }
            LoadCategory();
            ExpandCategory(category.Id);
        }

        private void DeleteScript(int index)
        {
            int tempScriptId = 0, tempsubcatId = 0;
            Models.SubCategory subcat = null;
            if (tvCatSubcat.SelectedNode.Tag.GetType() == typeof(Models.Script))
            {
                subcat = tvCatSubcat.SelectedNode.Parent.Tag as Models.SubCategory;
            }
            else if (tvCatSubcat.SelectedNode.Tag.GetType() == typeof(Models.SubCategory))
            {
                subcat = tvCatSubcat.SelectedNode.Tag as Models.SubCategory;
            }
            if (subcat != null)
            {
                if (subcat.Scripts.Count > index)
                {
                    Models.Script script = subcat.Scripts[index];
                    if (int.TryParse(script.Id, out tempScriptId) && int.TryParse(script.SubCategory, out tempsubcatId))
                    {
                        DeleteScriptReqMsg req = new DeleteScriptReqMsg() { ScriptId = tempScriptId, SubCategoryId = tempsubcatId, ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name };
                        scriptClient.ServiceChannel.DeleteScript(req);
                    }
                }
            }
            LoadCategory();
            ExpandSubCategory(tempsubcatId.ToString(), _categorySelected);
        }

        private void SelectCategory(string name)
        {
            if (dgList.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgList.Rows)
                {
                    if ((row.DataBoundItem as Models.CategorySubCategorySubset).Name == name)
                    {
                        row.Selected = true;
                        dgList_CellMouseClick(new object(), new DataGridViewCellMouseEventArgs(2, row.Index, 0, 0, new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, 0, 0, 0)));
                        break;
                    }
                }
            }
        }

        private void ExpandCategory(string id)
        {
            tvCatSubcat.TopNode.Expand();
            foreach (TreeNode node in tvCatSubcat.TopNode.Nodes)
            {
                if ((node.Tag as Models.Category).Id == id)
                {
                    node.Expand();
                    break;
                }
            }
            dgList.Columns[0].Visible = false;
        }

        private void ExpandSubCategory(string subcategoryId, string categoryId, string scriptId = "")
        {
            TreeNode cat = null, subcat = null;
            foreach (TreeNode node in tvCatSubcat.TopNode.Nodes)
            {
                if ((node.Tag as Models.Category).Id == categoryId)
                {
                    node.Expand();
                    cat = node;
                    break;
                }
            }
            if (cat != null)
            {
                foreach (TreeNode node in cat.Nodes)
                {
                    if ((node.Tag as Models.SubCategory).Id == subcategoryId)
                    {
                        node.Nodes.Clear();
                        //add a dummy node
                        TreeNode dummyNode = new TreeNode("Please wait...");
                        dummyNode.Tag = new DummyNodeObject() { Id = subcategoryId, Name = (node.Tag as Models.SubCategory).Name };
                        node.Nodes.Add(dummyNode);
                        System.Threading.Thread.Sleep(200);
                        //tvCatSubcat.SelectedNode = node;
                        node.Expand();
                        subcat = node;
                        break;
                    }
                }
            }
            if (subcat != null && !string.IsNullOrEmpty(scriptId))
            {
                //select the script from the tree
                foreach (TreeNode node in subcat.Nodes)
                {
                    Models.Script scriptNode = node.Tag as Models.Script;
                    if (scriptNode != null && scriptNode.Id == scriptId)
                    {
                        tvCatSubcat.SelectedNode = node;
                        break;
                    }
                }
            }
        }

        private void FetchScripts(TreeNode subcategory)
        {
            //set the category selected
            //_categorySelected = (subcategory.Parent.Tag as Models.Category).Id;
            if (subcategory.Nodes.Count == 1 && subcategory.Nodes[0].Tag.GetType() == typeof(DummyNodeObject))
            {
                //i.e. blank subcategory, then try to fetch scripts if any thru the service call
                //and then assign it to the e.Node.Tag
                DummyNodeObject dummyObj = subcategory.Nodes[0].Tag as DummyNodeObject;
                GetAllScriptDetailsResMsg response = scriptClient.ServiceChannel.GetAllScriptDetails(dummyObj.Id);
                List<Models.Script> scripts = Translators.ScriptPE_SE.ScriptListSEtoPE(response.Scripts.ToList());
                //assign the scripts fetched to the node tag having the sub-category details
                if (scripts != null && scripts.Count > 0)
                {
                    (subcategory.Tag as Models.SubCategory).Scripts = scripts;
                    PopulateScripts(subcategory, scripts);

                    //then populate grid
                    dgList.DataSource = GetScriptSubset(scripts);
                    //select the first row if any
                    if (dgList.Rows.Count > 0)
                    {
                        dgList.Rows[0].Selected = true;
                        dgList_CellMouseClick(new object(), new DataGridViewCellMouseEventArgs(2, 0, 0, 0, new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, 0, 0, 0)));
                        dgList.Columns[1].Visible = true;
                    }
                    else
                    {
                        dgList.DataSource = null;
                        dgList.Columns[0].Visible = false;
                        dgList.Columns[1].Visible = false;
                    }
                }
                else
                {
                    subcategory.Nodes.Clear();
                    dgList.DataSource = null;
                    dgList.RowHeadersVisible = false;
                }
            }
            else if (subcategory.Nodes.Count == 0)
            {
                dgList.DataSource = null;
                dgList.Columns[0].Visible = false;
                dgList.Columns[1].Visible = false;
            }
            else if (subcategory.Nodes[0].Tag.GetType() == typeof(Models.Script))
            {
                //do nothing;
            }
            else
                dgList.DataSource = null;
        }

        private void newCategoryMenuItem_Click(object sender, EventArgs e)
        {
            CreateNewCategory();
        }

        private void CreateNewCategory()
        {
            this.Cursor = Cursors.WaitCursor;
            splitContainerDetails.Panel2.Controls.Clear();
            Views.CategoryDetails cat = new CategoryDetails(null, true);
            cat.CategoryProcessed += new CategoryDetails.CategoryProcessedEventHandler(cat_CategoryProcessed);
            cat.Dock = DockStyle.Top;
            splitContainerDetails.Panel2.Controls.Add(cat);
            this.Cursor = Cursors.Default;
        }

        private void newSubCategoryMenuItem_Click(object sender, EventArgs e)
        {
            CreateNewSubCategory();
        }

        private void CreateNewSubCategory()
        {
            this.Cursor = Cursors.WaitCursor;
            splitContainerDetails.Panel2.Controls.Clear();
            Views.SubCategoryDetails subcat = new SubCategoryDetails(null, true);
            subcat.SubCategoryProcessed += new SubCategoryDetails.SubCategoryProcessedEventHandler(subcat_SubCategoryProcessed);
            subcat.Dock = DockStyle.Top;
            splitContainerDetails.Panel2.Controls.Add(subcat);
            this.Cursor = Cursors.Default;
        }

        private void newScriptMenuItem_Click(object sender, EventArgs e)
        {
            CreateNewScript();
        }

        private void CreateNewScript()
        {
            this.Cursor = Cursors.WaitCursor;
            splitContainerDetails.Panel2.Controls.Clear();
            Views.ScriptDetails script = new ScriptDetails(null, _categorySelected, true, _subcategorySelected);
            script.ScriptProcessed += new ScriptDetails.ScriptProcessedEventHandler(script_ScriptProcessed);
            script.Dock = DockStyle.Top;
            splitContainerDetails.Panel2.Controls.Add(script);
            this.Cursor = Cursors.Default;
        }

        private void runScriptLocallyMenuItem_Click(object sender, EventArgs e)
        {
            RunScriptLocally(selectedScriptIndex);
        }

        private void toolbarNewScript_Click(object sender, EventArgs e)
        {
            CreateNewScript();
        }

        private void toolbarRunScript_Click(object sender, EventArgs e)
        {
            RunScriptLocally(selectedScriptIndex);
        }

        private void toolbarNewCategory_Click(object sender, EventArgs e)
        {
            CreateNewCategory();
        }

        private void toolbarNewSubCategory_Click(object sender, EventArgs e)
        {
            CreateNewSubCategory();
        }

        private void dgList__CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                e.ToolTipText = "Run Script Locally";
            }
            else if (e.ColumnIndex == 1)
            {
                e.ToolTipText = "Delete";
            }
        }

        private void toolbarNewCategory_Click_1(object sender, EventArgs e)
        {
            CreateNewCategory();
        }

        private void toolbarNewSubCategory_Click_1(object sender, EventArgs e)
        {
            CreateNewSubCategory();
        }


    }

    public enum TreeNodeContext
    {
        Category, SubCategory, Script, Root
    }

    public class DummyNodeObject
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}

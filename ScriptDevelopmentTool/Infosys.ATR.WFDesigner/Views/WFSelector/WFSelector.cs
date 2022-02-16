using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Infosys.WEM.Service.Contracts.Data;
using Infosys.ATR.WFDesigner.Constants;
using Infosys.ATR.WFDesigner.Entities;
using Microsoft.Practices.CompositeUI.EventBroker;
using IMSWorkBench.Infrastructure.Interface;
using Infosys.ATR.Entities;

namespace Infosys.ATR.WFDesigner.Views
{
    public partial class WFSelector : UserControl, IWFSelector, IClose
    {
        WFDetails _details;
        Settings _settings = null;

        BindingList<Tuple<Image, Image, Image, string, string, int, Tuple<string, Guid, int, string, string, string, string>, Tuple<bool, bool, int, string, string, string>>> _wfcat;
        TreeNode _root;
        private const string ImageKey = "CatImage";

        public List<Category> Categories { get; set; }
        public List<WorkflowPE> Workflows { get; set; }
        ImageList treeList = null;
        bool workflow = false;
        int selectedRowIndex = 0;
        int companyId;
        internal static List<Infosys.ATR.Entities.Users> Users = null;
        internal bool enableControls = true;
        internal static Users CurentUser = null;
        internal bool enableRun = false;


        [EventPublication(Constants.EventTopicNames.ShowRun, PublicationScope.Global)]
        public event EventHandler<EventArgs<bool>> ShowRun;        

        private bool ascending = true;

        public WFSelector()
        {
            InitializeComponent();
            this.categoryWorkspace.Name = WorkspaceNames.catDeckWorkspace;
            this.dataGridView1.Dock = DockStyle.Fill;
            _details = new WFDetails(false);
            _details.Dock = DockStyle.Fill;

            this.dataGridView1.CellMouseClick += new DataGridViewCellMouseEventHandler(dataGridView1_CellMouseClick);
            this.dataGridView1.SelectionChanged += new EventHandler(dataGridView1_SelectionChanged);
        }

        internal void AddSmartParts()
        {
            this._presenter.WorkItem.SmartParts.Add(_details, "WFDetails");
            _settings = this._presenter.WorkItem.SmartParts.Get<Settings>("Settings");
            if (_settings == null)
                _settings = this._presenter.WorkItem.SmartParts.AddNew<Settings>("Settings");
            this._presenter.WorkItem.Workspaces.Add(categoryWorkspace, "CatDeckWorkspace");
        }

        internal void Initialize(bool refresh)
        {
            var security = this._presenter.WorkItem.RootWorkItem.State["Security"];
            companyId = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Company"]);
            this._presenter.GetCategories(companyId, refresh);

            var isSuperAdmin = (bool)this._presenter.WorkItem.RootWorkItem.Items["IsSuperAdmin"];

            Users = this._presenter.WorkItem.RootWorkItem.Items.Get("CurrentUser") as List<Infosys.ATR.Entities.Users>;

            if (Categories != null)
            {

                if (!isSuperAdmin)
                {
                    if (security.Equals("AllowAuthorised"))
                    {
                        var userCategory = this._presenter.WorkItem.RootWorkItem.Items.Get("CurrentUserCategories") as List<int>;
                        if (userCategory != null)
                        {
                            //Users = this._presenter.WorkItem.RootWorkItem.Items.Get("CurrentUser") as List<Infosys.ATR.Entities.Users>;
                            Categories = this.Categories.Where(c => userCategory.Contains(c.CategoryId)).
                            Select(c => new Entities.Category
                            {
                                CreatedBy = c.CreatedBy,
                                Description = c.Description,
                                CategoryId = c.CategoryId,
                                ModifiedBy = c.ModifiedBy,
                                Name = c.Name,
                                ParentId = c.ParentId,
                                CompanyId = c.CompanyId
                            }).ToList();

                        }
                    }
                }

                for (int i = 0; i < Categories.Count; i++)
                {
                    var c = Categories[i];
                    if (c.CompanyId == 0 && c.ParentId == 0)
                    {

                        var subCat = Categories.Where(sc => sc.CompanyId == 0 && sc.ParentId == Convert.ToInt32(c.CategoryId));
                        if (subCat == null || subCat.Count() == 0)
                        {
                            Categories.Remove(c);
                        }
                    }
                }

               // RemoveEmptyCategories();

                if (Categories != null)
                {
                    if (this.trWF.Nodes != null)
                        this.trWF.Nodes.Clear();

                    _root = new TreeNode("Categories");
                    this.trWF.Nodes.Add(_root);
                    _root.Expand();
                    treeList = new ImageList();
                    Image img = Image.FromFile(@"Images\Folder.png");
                    treeList.Images.Add(ImageKey, img);
                    trWF.ImageList = treeList;

                    Categories.Where(c => c.ParentId == 0).ToList().
                        ForEach(sc =>
                        {
                            AddNode(sc, null);
                        });

                    DisplayCategories();
                }
            }
            else
                MessageBox.Show("No Category exists", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void RemoveEmptyCategories()
        {
            for (int i = 0; i < this.Categories.Count; i++)
            {              
                var c = Categories[i];

                if (!HasWF(c))
                {
                    Categories.Remove(c);
                   // DeleteParentCategories(c);
                    
                }

            }

        }

        private bool HasWF(Category c)
        {
            
            var childCategories = Categories.Where(sc => sc.ParentId == Convert.ToInt32(c.CategoryId)).ToList();

            if (childCategories == null || childCategories.Count == 0)
            {
                this._presenter.GetWorkflowByCategory(c.CategoryId);

                if (Workflows == null || Workflows.Count == 0)
                    return false;
            }

            return true;
        }

        private void DeleteParentCategories(Category c)
        {
            var p = Categories.FirstOrDefault(cat => cat.CategoryId == c.ParentId);

            if (p != null)
            {
                if (!HasWF(p))
                {                    
                    Categories.Remove(p);
                    DeleteParentCategories(p);
                }
                else
                {
                    DeleteParentCategories(p);
                }
            }
        }

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

        private void DisplayCategories()
        {
            if (Categories.Count > 0)
            {
                _wfcat = new BindingList<Tuple<Image, Image, Image, string, string, int, Tuple<string, Guid, int, string, string, string, string>, Tuple<bool, bool, int, string, string, string>>>();
                //new BindingList<Tuple<Image, Image, Image, string, string, int, Tuple<string, Guid, int, string, string, string, string>>>();
                Categories.Where(c => c.ParentId == 0).ToList().ForEach(wf =>
                {
                    Tuple<Image, Image, Image, string, string, int, Tuple<string, Guid, int, string, string, string, string>, Tuple<bool, bool, int, string, string, string>> tupleCat =
                        new Tuple<Image, Image, Image, string, string, int, Tuple<string, Guid, int, string, string, string, string>, Tuple<bool, bool, int, string, string, string>>(
                         null, null, Image.FromFile(@"Images\folder.png"), wf.Name, wf.Description, wf.CategoryId,
                        new Tuple<string, Guid, int, string, string, string, string>(string.Empty, new System.Guid(), 0, string.Empty, string.Empty, string.Empty, string.Empty), new Tuple<bool, bool, int, string, string, string>(false, false, 0, string.Empty, string.Empty, string.Empty));
                    _wfcat.Add(tupleCat);
                });
                DisplayData();
            }
        }

        private void trWF_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Display();
        }

        private void Display()
        {

            var e = this.trWF.SelectedNode;
            if (!workflow)
            {

                workflow = false;
                //if (e.Node.Tag != null)
                if (e.Tag != null)
                {
                    if (this.trWF.SelectedNode.Tag.GetType() == typeof(Category))
                    {
                        var cat = this.trWF.SelectedNode.Tag as Category;

                        if (cat != null)
                        {
                            var isGuest = IsGuest(cat);

                            if (isGuest)
                            {
                                //dataGridView1.Enabled = false;                                
                                this.enableControls = false;
                            }
                            else
                                this.enableControls = true;

                            LoadSubCatAndWF(cat);


                            if (CurentUser != null)
                            {
                                if (CurentUser.Role == Infosys.ATR.Entities.Roles.Guest.ToString() || CurentUser.Role == Infosys.ATR.Entities.Roles.Agent.ToString())
                                {
                                    _details.DisableControls(false);

                                    if (CurentUser.Role == Roles.Agent.ToString())
                                        ShowRun(this, new EventArgs<bool>(!this.enableControls));
                                    else
                                        ShowRun(this, new EventArgs<bool>(this.enableControls));
                                }
                                else
                                    _details.DisableControls(true);
                            }
                        }
                    }
                    else
                    {
                        this.enableControls = false;
                        DisplayCategories();
                    }
                }
                // if root node is selected
                else
                {
                    this.enableControls = false;
                    DisplayCategories();
                }
            }

            else
            {
                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();
            }

            //ShowRun(this, new EventArgs<bool>(this.enableControls));
        }

        private bool IsGuest(Category cat)
        {
            if (!Infosys.ATR.Entities.CommonObjects.IsSuperAdmin)
            {
                CurentUser = Users.FirstOrDefault(u => u.CategoryId == Convert.ToInt32(cat.CategoryId));

                if (Infosys.ATR.Entities.CommonObjects.IsSuperAdmin)
                    CurentUser = Users.FirstOrDefault();
                else
                {
                    if (CurentUser != null)
                        if (CurentUser.Role == Infosys.ATR.Entities.Roles.Guest.ToString() || CurentUser.Role == Infosys.ATR.Entities.Roles.Agent.ToString())
                            return true;
                }
            }
            return false;
        }

        /// <summary>
        /// This method load subcategories and workflows based on the category/subcategory selected.
        /// </summary>
        /// <param name="subCatId">category id</param>
        /// <param name="parentId">parent id</param>
        private void LoadSubCatAndWF(Category cat)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.DataSource = null;
            _wfcat.Clear();
            _wfcat = null;
            //   _wfcat = new BindingList<Tuple<Image, Image, Image, string, string, int, int, Tuple<string, Guid, int, string, string, string, string>>>();

            _wfcat = new BindingList<Tuple<Image, Image, Image, string, string, int, Tuple<string, Guid, int, string, string, string, string>, Tuple<bool, bool, int, string, string, string>>>();

            if (Categories.Count > 0)
            {
                // Get all subcategories based where selected cat id is equal to parent id
                Categories.Where(c => c.ParentId == cat.CategoryId).ToList().ForEach(wf =>
                    {
                        Tuple<Image, Image, Image, string, string, int, Tuple<string, Guid, int, string, string, string, string>, Tuple<bool, bool, int, string, string, string>> tupleCat =
                           new Tuple<Image, Image, Image, string, string, int, Tuple<string, Guid, int, string, string, string, string>, Tuple<bool, bool, int, string, string, string>>(
                          null, null, Image.FromFile(@"Images\folder.png"), wf.Name, wf.Description, wf.CategoryId,
                           new Tuple<string, Guid, int, string, string, string, string>(string.Empty, new System.Guid(), 0, string.Empty, string.Empty, string.Empty, string.Empty), new Tuple<bool, bool, int, string, string, string>(false, false, 0, string.Empty, string.Empty, string.Empty));
                        _wfcat.Add(tupleCat);

                    });
            }

            LoadWorkflows(cat.CategoryId);

            DisplayData();
        }
        /// <summary>
        /// This method loads workflows based on cat/subcat.
        /// </summary>
        /// <param name="catId">category id</param>
        /// <param name="subCatID">sub category id</param>
        private void LoadWorkflows(int catId)//, int subCatID)
        {
            //this._presenter.GetWorkflowByCategory(catId, subCatID);

            this._presenter.GetWorkflowByCategory(catId);

            if (Workflows != null && Workflows.Count > 0)
            {
                Workflows.ForEach(wf =>
                {

                    Tuple<Image, Image, Image, string, string, int, Tuple<string, Guid, int, string, string, string, string>, Tuple<bool, bool, int, string, string, string>> tupleWF =
                                   new Tuple<Image, Image, Image, string, string, int, Tuple<string, Guid, int, string, string, string, string>, Tuple<bool, bool, int, string, string, string>>(
                                   Image.FromFile(@"Images\remove.png"), Image.FromFile(@"Images\edit.png"), Image.FromFile(@"Images\workflow.png"), wf.Name, wf.Description, wf.CategoryID,
                                   new Tuple<string, Guid, int, string, string, string, string>(wf.WorkflowURI, wf.WorkflowID, wf.WorkflowVersion, wf.CreatedBy,
                                       wf.LastModifiedBy, wf.LastModifiedOn, wf.PublishedOn.ToString()), new Tuple<bool, bool, int, string, string, string>(wf.UsesUIAutomation, wf.IslongRunningWorkflow, wf.IdleStateTimeout, wf.Tags, wf.LicenseType, wf.SourceUrl));
                    _wfcat.Add(tupleWF);
                });                
            }            
        }
        /// <summary>
        /// This method populates gridview with the data.
        /// </summary>
        private void DisplayData()
        {
            if (_wfcat != null && _wfcat.Count > 0)
            {
                this.dataGridView1.DataSource = _wfcat;
                //this.dataGridView1.Columns[0].Width = 25;
                //this.dataGridView1.Columns[1].Width = 25;
                this.dataGridView1.Columns[2].Width = 30;
                this.dataGridView1.Columns[3].Width = 250;
                this.dataGridView1.Columns[4].Width = 375;
                this.dataGridView1.Columns[0].HeaderText = "";
                this.dataGridView1.Columns[1].HeaderText = "";
                this.dataGridView1.Columns[2].HeaderText = "";
                this.dataGridView1.Columns[3].HeaderText = "Name";
                this.dataGridView1.Columns[4].HeaderText = "Description";
                this.dataGridView1.Columns[5].Visible = this.dataGridView1.Columns[6].Visible = this.dataGridView1.Columns[7].Visible = false;
                dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                this.dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                this.dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                if (this.dataGridView1.Rows.Count > 0 &&
                    this.dataGridView1.Rows.Count > selectedRowIndex)
                {
                    ShowCategoryWFDetails(selectedRowIndex);
                    this.dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    this.dataGridView1.Rows[0].Selected = false;
                    this.dataGridView1.Rows[selectedRowIndex].Selected = true;
                }

                this.dataGridView1.Columns[0].Visible = this.dataGridView1.Columns[1].Visible = false;
            }
            else
            {
                this.dataGridView1.DataSource = null;
                this.dataGridView1.Rows.Clear();
                selectedRowIndex = -1;
                ShowCategoryWFDetails(selectedRowIndex);
            }

            
        }
        /// <summary>
        /// This method is used to display delete and open buttons in the grid.
        /// </summary>
        //private void DisplayImages()
        //{
        //    foreach (DataGridViewRow row in this.dataGridView1.Rows)
        //    {
        //        Tuple<string, Guid, int, string, string, string, string> tupleWFDetails = (Tuple<string, Guid, int, string, string, string, string>)row.Cells[6].Value;
        //        if (!String.IsNullOrEmpty(tupleWFDetails.Item1))
        //        {
        //            this.dataGridView1.Columns[0].Visible = true;
        //           // this.dataGridView1.Columns[1].Visible = true;
        //        }

        //        if (CurentUser != null)
        //        {
        //            if (CurentUser.Role == Infosys.ATR.Entities.Roles.Guest.ToString() || CurentUser.Role == Infosys.ATR.Entities.Roles.Agent.ToString())
        //            {
        //                this.dataGridView1.Columns[0].Visible = false;
        //               // this.dataGridView1.Columns[1].Visible = false;
        //            }
        //        }
        //    }
        //}

        public bool Close()
        {
            this._presenter.OnCloseView();
            return true;
        }



        void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            //if(this.dataGridView1.CurrentCell.ColumnIndex != 0)
            //{
            //   ShowDetails(this.dataGridView1.CurrentRow.Index);
            //}
        }

        void EnableDisableDeleteMenu( DataGridViewRow selectedRow )
        {          
            Tuple<string, Guid, int, string, string, string, string> tupleWFDetails = 
                (Tuple<string, Guid, int, string, string, string, string>)selectedRow.Cells[6].Value;

            if (!String.IsNullOrEmpty(tupleWFDetails.Item1))
            {
                this._presenter.EnableDeleteMenu_Handler(true);
            }
            else
            {
                this._presenter.EnableDeleteMenu_Handler(false);
            }
        }

        private void dataGridView1_CellMouseClick(Object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                dataGridView1.Rows[e.RowIndex].Selected = true;
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];

                EnableDisableDeleteMenu(selectedRow);

                ShowCategoryWFDetails(e.RowIndex);

                //if (e.ColumnIndex == 0)
                //{
                //    // If workflow URI is not null e.g. WF item has been clicked not category item
                //    if (!String.IsNullOrEmpty(tupleWFDetails.Item1))
                //    {
                //        var dialog =
                //            MessageBox.Show("Do you want to delete the selected Workflow?", "IAP", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

                //        if (dialog == DialogResult.OK)
                //        {
                //            WorkflowPE pe = GetDataBoundObject(e.RowIndex);
                //            this._presenter.Delete(pe);

                //            var t = (Tuple<Image, Image, Image, string, string, int, Tuple<string, Guid, int, string, string, string, string>, Tuple<bool, bool, int, string, string, string>>)
                //         this.dataGridView1.Rows[e.RowIndex].DataBoundItem;

                //            _wfcat.Remove(t);
                //            //Workflows.RemoveAll(wf => t.Item7.Item2 == wf.WorkflowID);
                //            Workflows.RemoveAll(wf => t.Item7.Item2 == wf.WorkflowID);
                //            MessageBox.Show("Workflow deleted successfully");
                //            this.dataGridView1.Refresh();

                //            Display();
                //        }
                //    }
                //}
                //else if (e.ColumnIndex == 1)
                //{
                //    // If workflow URI is not null e.g. WF item has been clicked not category item
                //    if (!String.IsNullOrEmpty(tupleWFDetails.Item1))
                //    {
                //        this._presenter.OpenWFFromRepository_handler(GetDataBoundObject(e.RowIndex));
                //        this._presenter.ActivateMenuHandler();

                //    }
                //}
                //else
                {
                    //ShowCategoryWFDetails(e.RowIndex);
                    //DisplayImages();
                }

                //if (!this.enableControls)
                //    _details.DisableControls(false);
            }
        }

        private void ShowCategoryWFDetails(int rowIndex)
        {
            if (rowIndex == -1)
            {
                this._presenter.WorkItem.Workspaces[Constants.WorkspaceNames.catDeckWorkspace].Show(_settings);
                this._presenter.ShowCatDetails_Handler(null);
                this._presenter.ShowWFDetails(null);
                ShowRun(this, new EventArgs<bool>(false));
                selectedRowIndex = 0;
                MessageBox.Show("No category/workflow exists");
            }
            else
            {
                if (dataGridView1.Rows.Count > rowIndex)
                {
                    DataGridViewRow selectedRow = dataGridView1.Rows[rowIndex];
                    Tuple<string, Guid, int, string, string, string, string> tupleWFDetails = (Tuple<string, Guid, int, string, string, string, string>)selectedRow.Cells[6].Value;

                    // If workflow URI is null
                    if (String.IsNullOrEmpty(tupleWFDetails.Item1))
                    {
                        this.dataGridView1.Columns[0].Visible = false;
                        this.dataGridView1.Columns[1].Visible = false;
                        this.dataGridView1.Columns[0].DefaultCellStyle.NullValue = null;
                        this.dataGridView1.Columns[1].DefaultCellStyle.NullValue = null;
                        this._presenter.WorkItem.Workspaces[Constants.WorkspaceNames.catDeckWorkspace].Show(_settings);
                        Category cat = GetCatDataBoundObject(rowIndex);
                        Tuple<TreeNode, Category> objCat = new Tuple<TreeNode, Category>(null, cat);
                        this._presenter.ShowCatDetails_Handler(objCat);
                        ShowRun(this, new EventArgs<bool>(!this.enableControls));
                    }
                    else
                    {
                        //if (CurentUser != null)
                        //{
                        //    if (CurentUser.Role == Infosys.ATR.Entities.Roles.Agent.ToString())
                        //        ShowRun(this, new EventArgs<bool>(true));
                        //}

                        ////if (CurentUser != null)
                        ////{
                        ////    if (CurentUser.Role == Infosys.ATR.Entities.Roles.Guest.ToString() || CurentUser.Role == Infosys.ATR.Entities.Roles.Agent.ToString())
                        ////        _details.DisableControls(false);
                        ////    else
                        ////        _details.DisableControls(true);
                        ////}

                        this._presenter.WorkItem.Workspaces[Constants.WorkspaceNames.catDeckWorkspace].Show(_details);
                        this._presenter.ShowWFDetails(GetDataBoundObject(rowIndex));
                        ShowRun(this, new EventArgs<bool>(this.enableControls));
                    }
                }
            }
        }

        private WorkflowPE GetDataBoundObject(int rowId)
        {

            Tuple<Image, Image, Image, String, String, int, Tuple<string, Guid, int, string, string, string, string>, Tuple<bool, bool, int, string, string, string>> t =
             (Tuple<Image, Image, Image, String, String, int, Tuple<string, Guid, int, string, string, string, string>, Tuple<bool, bool, int, string, string, string>>)
          this.dataGridView1.Rows[rowId].DataBoundItem;

            var wfParam = Workflows.FirstOrDefault(wf => wf.WorkflowID == t.Item7.Item2);

            return new WorkflowPE
            {
                Name = t.Item4,
                Description = t.Item5,
                CategoryID = t.Item6,
                // SubCategoryID = t.Item7,
                WorkflowURI = t.Item7.Item1,
                WorkflowID = t.Item7.Item2,
                WorkflowVersion = t.Item7.Item3,
                CreatedBy = t.Item7.Item4,
                LastModifiedBy = t.Item7.Item5,
                LastModifiedOn = t.Item7.Item6,
                PublishedOn = Convert.ToDateTime(t.Item7.Item7),
                FileName = t.Item7.Item3.ToString(),
                UsesUIAutomation = t.Rest.Item1,
                IslongRunningWorkflow = t.Rest.Item2,
                IdleStateTimeout = t.Rest.Item3,
                Tags = t.Rest.Item4,
                LicenseType = t.Rest.Item5,
                SourceUrl = t.Rest.Item6,

                Parameters = wfParam.Parameters
            };            
        }

        private Category GetCatDataBoundObject(int rowId)
        {
            Tuple<Image, Image, Image, String, String, int, Tuple<string, Guid, int, string, string, string, string>, Tuple<bool, bool, int, string, string, string>> t =
                (Tuple<Image, Image, Image, String, String, int, Tuple<string, Guid, int, string, string, string, string>, Tuple<bool, bool, int, string, string, string>>)
             this.dataGridView1.Rows[rowId].DataBoundItem;
            return new Category
            {
                Name = t.Item4,
                Description = t.Item5,
                CategoryId = t.Item6,
                //ParentId = t.Item7
            };
        }

        private void trWF_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {

        }

        private void trWF_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

            if (e.RowIndex != -1)
            {
                this.dataGridView1.Columns[2].DefaultCellStyle.Padding = new Padding(10, 0, -10, 0);
                this.dataGridView1.Columns[3].DefaultCellStyle.Padding = new Padding(-10, 0, 0, 0);

                if (e.ColumnIndex == 0 || e.ColumnIndex == 1)
                {
                    e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.Border);
                    ControlPaint.DrawBorder(e.Graphics, e.CellBounds,
                        dataGridView1.GridColor, 0, ButtonBorderStyle.None,
                        dataGridView1.GridColor, 1, ButtonBorderStyle.None,
                        dataGridView1.GridColor, 1, ButtonBorderStyle.Solid,
                        dataGridView1.GridColor, 1, ButtonBorderStyle.Solid);
                    e.Handled = true;
                }
                if (e.ColumnIndex == 2)
                {
                    e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.Border);
                    ControlPaint.DrawBorder(e.Graphics, e.CellBounds,
                        dataGridView1.GridColor, 1, ButtonBorderStyle.None,
                        dataGridView1.GridColor, 1, ButtonBorderStyle.None,
                        dataGridView1.GridColor, 0, ButtonBorderStyle.None,
                        dataGridView1.GridColor, 1, ButtonBorderStyle.Solid);
                    e.Handled = true;
                }
                if (e.ColumnIndex == 3 || e.ColumnIndex == 4)
                {
                    e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.Border);
                    ControlPaint.DrawBorder(e.Graphics, e.CellBounds,
                        dataGridView1.GridColor, 0, ButtonBorderStyle.None,
                        dataGridView1.GridColor, 1, ButtonBorderStyle.None,
                        dataGridView1.GridColor, 1, ButtonBorderStyle.Solid,
                        dataGridView1.GridColor, 1, ButtonBorderStyle.Solid);
                    e.Handled = true;
                }
            }

        }

        public TreeNode FindSelectedNode(Category itemId, TreeNode rootNode, Boolean workflow)
        {
            foreach (TreeNode node in rootNode.Nodes)
            {
                Category category = node.Tag as Category;
                if (category != null)
                {
                    if (category.CategoryId.Equals(itemId.CategoryId)) return node;
                    TreeNode next = FindSelectedNode(itemId, node, workflow);
                    if (next != null) return next;
                }
            }
            return null;
        }


        private void dataGridView1_CellMouseDoubleClick_1(object sender, DataGridViewCellMouseEventArgs e)
        {
            //not sure what's happening here. making it to open the WF in editor

            if (e.RowIndex != -1)
            {
                dataGridView1.Rows[e.RowIndex].Selected = true;
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];
                Tuple<string, Guid, int, string, string, string, string> tupleWFDetails = (Tuple<string, Guid, int, string, string, string, string>)selectedRow.Cells[6].Value;


                if (!String.IsNullOrEmpty(tupleWFDetails.Item1))
                {
                    this._presenter.OpenWFFromRepository_handler(GetDataBoundObject(e.RowIndex));
                    this._presenter.ActivateMenuHandler();

                }
            }

            //TreeNode selectedNode = null;
            //workflow = false;
            //Category cat = (Category)null;
            //if (e.RowIndex != -1)
            //{
            //    int rowIndex = e.RowIndex;
            //    DataGridViewRow selectedRow = dataGridView1.Rows[rowIndex];

            //    Tuple<string, Guid, int, string, string, string, string> tupleWFDetails =
            //        (Tuple<string, Guid, int, string, string, string, string>)selectedRow.Cells[6].Value;

            //    cat = GetCatDataBoundObject(rowIndex);

            //    var isGuest = IsGuest(cat);

            //    if (isGuest)
            //    {
            //        //dataGridView1.Enabled = false;
            //        this.enableControls = false;
            //    }
            //    else
            //    {
            //        dataGridView1.Enabled = true;
            //        this.enableControls = true;

            //    }

            //    selectedRowIndex = rowIndex;
            //    LoadSubCatAndWF(cat);

            //    foreach (TreeNode node in trWF.Nodes)
            //    {
            //        selectedNode = FindSelectedNode(cat, node, workflow);
            //        if (selectedNode != null) break;
            //    }

            //    this.trWF.CollapseAll();
            //    if (workflow)
            //        selectedNode = selectedNode.Parent;
            //    if (selectedNode != null)
            //    {
            //        this.trWF.SelectedNode = selectedNode;
            //        selectedNode.Expand();
            //        this.trWF.Focus();
            //    }
            //    selectedRowIndex = 0;
            //}

            //if (!this.enableControls)
            //    _details.DisableControls(false);
            //else
            //    _details.DisableControls(true);
        }

        [EventSubscription(Constants.EventTopicNames.DisplayWFDetails, ThreadOption.UserInterface)]
        public void DisplayWFDetailsEventHandler(object sender, EventArgs<Category> e)
        {
            if (e.Data != null)
            {
                Category cat = new Category();
                cat.CategoryId = e.Data.CategoryId;
                cat.ParentId = e.Data.ParentId;
                LoadSubCatAndWF(cat);
            }
        }

        [EventSubscription(Constants.EventTopicNames.RefreshCategories, ThreadOption.UserInterface)]
        public void RefreshCategories(object sender, EventArgs<String> e)
        {
            Initialize(true);
        }

        [EventSubscription(Constants.EventTopicNames.CategoryDeleted, ThreadOption.UserInterface)]
        public void CategoryDeleted(object sender, EventArgs e)
        {
            Initialize(true);
        }

        [EventSubscription(Constants.EventTopicNames.RefreshExplore, ThreadOption.UserInterface)]
        public void RefreshExplore(object sender, EventArgs e)
        {
            // Initialize();
        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

            if (e.ColumnIndex != null && e.ColumnIndex == 3)
            {
                var dataSource = dataGridView1.DataSource as BindingList<Tuple<Image, Image, Image, string, string, int, Tuple<string, Guid, int, string, string, string, string>, Tuple<bool, bool, int, string, string, string>>>;

                BindingList<Tuple<Image, Image, Image, string, string, int, Tuple<string, Guid, int, string, string, string, string>, Tuple<bool, bool, int, string, string, string>>> _t =
                    new BindingList<Tuple<Image, Image, Image, string, string, int, Tuple<string, Guid, int, string, string, string, string>, Tuple<bool, bool, int, string, string, string>>>();

                if (_t.Count > 0) _t.Clear();

                if (ascending)
                {
                    var ascendingOrder = dataSource.OrderBy(t => t.Item4).ToList();

                    ascendingOrder.ForEach(wf =>
                    {
                        _t.Add(ConvertToTuple(wf));
                    });

                    dataGridView1.DataSource = _t;
                    ascending = false;
                }
                else
                {
                    var descendingOrder = dataSource.OrderByDescending(t => t.Item4).ToList();

                    descendingOrder.ForEach(wf =>
                    {
                        _t.Add(ConvertToTuple(wf));
                    });

                    dataGridView1.DataSource = _t;
                    ascending = true;
                }
                ShowCategoryWFDetails(0);
            }
        }

        private Tuple<Image, Image, Image, string, string, int, Tuple<string, Guid, int, string, string, string, string>, Tuple<bool, bool, int, string, string, string>> ConvertToTuple(
            Tuple<Image, Image, Image, string, string, int, Tuple<string, Guid, int, string, string, string, string>, Tuple<bool, bool, int, string, string, string>> wf)
        {
            Tuple<Image, Image, Image, string, string, int, Tuple<string, Guid, int, string, string, string, string>, Tuple<bool, bool, int, string, string, string>> tupleWF =
                                 new Tuple<Image, Image, Image, string, string, int, Tuple<string, Guid, int, string, string, string, string>, Tuple<bool, bool, int, string, string, string>>(
                                 wf.Item1, wf.Item2, wf.Item3, wf.Item4, wf.Item5, wf.Item6, new Tuple<string, Guid, int, string, string, string, string>
                                     (wf.Item7.Item1, wf.Item7.Item2, wf.Item7.Item3, wf.Item7.Item4, wf.Item7.Item5, wf.Item7.Item6, wf.Item7.Item7),
                                     new Tuple<bool, bool, int, string, string, string>(wf.Rest.Item1, wf.Rest.Item2, wf.Rest.Item3, wf.Rest.Item4, wf.Rest.Item5, wf.Rest.Item6));

            return tupleWF;
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Initialize(true);
        }

        public void RunWFSelector() 
        {
            this._details.RunSelectedWF();
        }
        public void DeleteWF()
        {
            if (dataGridView1.SelectedRows != null)
            {
                //var selectedRow = dataGridView1.SelectedRows[0];
                //dataGridView1.Rows[e.RowIndex].Selected = true;

                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                Tuple<string, Guid, int, string, string, string, string> tupleWFDetails = (Tuple<string, Guid, int, string, string, string, string>)selectedRow.Cells[6].Value;

                var index = selectedRow.Index;

                if (!String.IsNullOrEmpty(tupleWFDetails.Item1))
                {
                    var dialog =
                        MessageBox.Show("Do you want to delete the selected Workflow?", "IAP", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

                    if (dialog == DialogResult.OK)
                    {
                        WorkflowPE pe = GetDataBoundObject(selectedRow.Index);
                        this._presenter.Delete(pe);

                        var t = (Tuple<Image, Image, Image, string, string, int, Tuple<string, Guid, int, string, string, string, string>, Tuple<bool, bool, int, string, string, string>>)
                     this.dataGridView1.Rows[index].DataBoundItem;

                        _wfcat.Remove(t);
                        //Workflows.RemoveAll(wf => t.Item7.Item2 == wf.WorkflowID);
                        Workflows.RemoveAll(wf => t.Item7.Item2 == wf.WorkflowID);
                        MessageBox.Show("Workflow deleted successfully");
                        this.dataGridView1.Refresh();

                        Display();
                    }
                }
            }
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                dataGridView1.Rows[e.RowIndex].Selected = true;
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];

                EnableDisableDeleteMenu(selectedRow);
            }
        }

        
    }
}

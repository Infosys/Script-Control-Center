using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using WEMClient = Infosys.WEM.Client;
using Infosys.WEM.Service.Common.Contracts.Message;
using Infosys.WEM.Service.Common.Contracts.Data;
//using Infosys.WEM.Scripts.Service.Contracts.Message;
//using Infosys.WEM.Scripts.Service.Contracts.Data;

namespace Infosys.ATR.ScriptRepository.Views
{
    public partial class SubCategoryDetails : UserControl
    {
        bool _newItem;
        string _parentCategoryId, _subcategoryId;
        WEMClient.CommonRepository commonRepositoryClient = new WEMClient.CommonRepository();

        public class SubCategoryProcessedArgs : EventArgs
        {
            public Models.EntityProcessingTypes ProcessingType { get; set; }
            public string Id { get; set; }
            public string ParentCategoryId { get; set; }
            public string Name { get; set; }
        }
        public delegate void SubCategoryProcessedEventHandler(SubCategoryProcessedArgs e);
        public event SubCategoryProcessedEventHandler SubCategoryProcessed;

        public SubCategoryDetails(Models.SubCategory subcategory, bool newItem = false, string parentCategoryId = "")
        {
            this.Cursor = Cursors.WaitCursor;
            InitializeComponent();
            _newItem = newItem;
            _parentCategoryId = parentCategoryId;
            if (subcategory != null)
            {
                txtSubCategoryName.Text = subcategory.Name;
                txtDesc.Text = subcategory.Description;
                _subcategoryId = subcategory.Id;
            }
            else
            {
                //fetch the available categories
                //and bind it to the combobox
                //i.e. in case of new sub-category
                pnlCategory.Visible = true;
               string companyId = System.Configuration.ConfigurationManager.AppSettings["Company"];
                Infosys.WEM.Service.Common.Contracts.Message.GetAllCategoriesResMsg response = commonRepositoryClient.ServiceChannel.GetAllCategoriesByCompany(companyId,Constants.Application.ModuleID);

                //GetAllCategoriesResMsg res = client.ServiceChannel.GetAllCategories();
                BindCategories(response.Categories);
            }

            if (newItem)
            {
                //btnSave.Text = "Add";
                label2.Text = "New Sub Category";
            }
            this.Cursor = Cursors.Default;
        }

        private void BindCategories(List<Infosys.WEM.Service.Common.Contracts.Data.Category> categories)
        {
            if (categories != null)
            {
                cmbCategories.DataSource = categories;
                cmbCategories.DisplayMember = "Name";
                cmbCategories.ValueMember = "CategoryId";
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (_newItem)
                {
                    if ((int)cmbCategories.SelectedValue >= 0)
                    {
                        AddCategoryReqMsg req = new AddCategoryReqMsg();
                        req.Categories = new List<Category>();
                        Category cat = new Category();
                       // cat.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                        cat.Description = txtDesc.Text;
                        cat.Name = txtSubCategoryName.Text;
                        cat.ParentCategoryId = (int)cmbCategories.SelectedValue;
                        req.Categories.Add(cat);
                        if (commonRepositoryClient.ServiceChannel.AddCategory(req).IsSuccess)
                        {
                            if (SubCategoryProcessed != null)
                            {
                                SubCategoryProcessedArgs eventArgs = new SubCategoryProcessedArgs();
                                eventArgs.ProcessingType = Models.EntityProcessingTypes.Add;
                                eventArgs.ParentCategoryId = cat.ParentCategoryId.ToString();
                                SubCategoryProcessed(eventArgs);
                            }
                        }
                        else
                        {
                            string err = "There is an error adding the sub category. \nPlease verify if the details provided are correct and name is not same as any existing Category or Sub Category.";
                            MessageBox.Show(err, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    UpdateCategoryReqMsg req = new UpdateCategoryReqMsg();
                    req.Categories = new List<Category>();
                    Category subcat = new Category();
               //     subcat.ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    subcat.Name = txtSubCategoryName.Text;
                    subcat.Description = txtDesc.Text;
                    subcat.CategoryId = int.Parse(_subcategoryId);
                    subcat.ParentCategoryId = int.Parse(_parentCategoryId);
                    req.Categories.Add(subcat);
                    if (commonRepositoryClient.ServiceChannel.UpdateCategory(req).IsSuccess)
                    {
                        if (SubCategoryProcessed !=null)
                        {
                            SubCategoryProcessedArgs eventArgs = new SubCategoryProcessedArgs();
                            eventArgs.ProcessingType = Models.EntityProcessingTypes.Update;
                            eventArgs.ParentCategoryId = _parentCategoryId;
                            eventArgs.Id = _subcategoryId;
                            eventArgs.Name = subcat.Name;
                            SubCategoryProcessed(eventArgs);
                        }
                    }
                }
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

    }
}

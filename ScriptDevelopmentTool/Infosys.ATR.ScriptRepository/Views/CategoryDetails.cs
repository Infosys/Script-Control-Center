using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using WEMClient = Infosys.WEM.Client;
using Infosys.WEM.Scripts.Service.Contracts.Message;
using Infosys.WEM.Scripts.Service.Contracts.Data;

namespace Infosys.ATR.ScriptRepository.Views
{
    public partial class CategoryDetails : UserControl
    {
        bool _newItem; string categoryId;
        WEMClient.ScriptRepository client = new WEMClient.ScriptRepository();

        public class CategoryProcessedArgs : EventArgs
        {
            public Models.EntityProcessingTypes ProcessingType { get; set; }
            public string Id { get; set; }
            public string Name { get; set; }
        }
        public delegate void CategoryProcessedEventHandler(CategoryProcessedArgs e);
        public event CategoryProcessedEventHandler CategoryProcessed;

        public CategoryDetails(Models.Category category, bool newItem = false)
        {
            InitializeComponent();
            _newItem = newItem;            
            if (category != null)
            {
                txtCategoryName.Text = category.Name;
                txtDesc.Text = category.Description;
                categoryId = category.Id;
            }

            if (_newItem)
            {
                btnSave.Text = "Add";
                label2.Text = "New Category";
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (_newItem)
                {
                    AddCategoryReqMsg req = new AddCategoryReqMsg();
                    req.Categories = new List<Category>();
                    Category cat = new Category();
                    cat.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    cat.Description = txtDesc.Text;
                    cat.Name = txtCategoryName.Text;
                    req.Categories.Add(cat);
                    if (client.ServiceChannel.AddCategory(req).IsSuccess)
                    {
                        if (CategoryProcessed != null)
                        {
                            CategoryProcessedArgs eventArgs = new CategoryProcessedArgs();
                            eventArgs.ProcessingType = Models.EntityProcessingTypes.Add;
                            //no id for add
                            CategoryProcessed(eventArgs);
                        }
                        MessageBox.Show(string.Format("The new category- {0} is added.",cat.Name) , "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        string err = "There is an error adding the category. \nPlease verify if the details provided are correct and name is not same as any existing Category.";
                        MessageBox.Show(err, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    //i.e. for update
                    UpdateCategoryReqMsg req = new UpdateCategoryReqMsg();
                    req.Categories = new List<Category>();
                    Category cat = new Category();
                    cat.ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    cat.Name = txtCategoryName.Text;
                    cat.Description = txtDesc.Text;
                    cat.CategoryId = int.Parse(categoryId);
                    req.Categories.Add(cat);
                    if (client.ServiceChannel.UpdateCategory(req).IsSuccess)
                    {
                        if (CategoryProcessed != null)
                        {
                            CategoryProcessedArgs eventArgs = new CategoryProcessedArgs();
                            eventArgs.ProcessingType = Models.EntityProcessingTypes.Update;
                            eventArgs.Id = categoryId;
                            eventArgs.Name = cat.Name;
                            CategoryProcessed(eventArgs);
                        }
                        MessageBox.Show(string.Format("The category- {0} is updated.", cat.Name), "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        string err = "There is an error updating the category. \nPlease verify if the details provided are correct and name is not same as any existing Category.";
                        MessageBox.Show(err, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                string err = "There is an error adding/updating the category. \nPlease verify if the details provided are correct and name is not same as any existing Category.";
                err = err + "\nMore Infomation- " + ex.Message;
                if (ex.InnerException != null)
                    err = err + ". \nInner Exception- " + ex.InnerException.Message;
                MessageBox.Show(err, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }
    }
}

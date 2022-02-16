using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceModel.Web;

using Infosys.ATR.ScriptRepository.Models;
//using ScriptRepository.ScriptServiceRef;

namespace ScriptRepository
{ 
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            List<Category> cats = new List<Category>();

            Category cat1 = new Category();
           // cat1.SubCategories = new List<SubCategory>();
            cat1.Name = "Desktop";

            SubCategory subcat1 = new SubCategory();
            subcat1.Scripts = new List<Script>();
            subcat1.Name = "Desktop File Operations";

            Script script1 = new Script();
            script1.Name = "Dir";
            Script script2 = new Script();
            script2.Name = "CD";

            subcat1.Scripts.Add(script1);
            subcat1.Scripts.Add(script2);
            //cat1.SubCategories.Add(subcat1);
            //cat1.SubCategories.Add(new SubCategory() { Name = "Blank" });
            cats.Add(cat1);

            Category cat2 = new Category();
           // cat2.SubCategories = new List<SubCategory>();
            cat2.Name = "Active Directory";

            SubCategory subcat2 = new SubCategory();
            subcat2.Scripts = new List<Script>();
            subcat2.Name = "User Operations";

            Script script3 = new Script();
            script3.Name = "Add User";
            script3.Parameters = new List<ScriptParameter>();
            ScriptParameter param1 = new ScriptParameter();
            param1.Name = "UserName";
            param1.AllowedValues = "Rahul";
            param1.IOType = ParameterIOTypes.Out;
            //param1.Type = "string";
            param1.IsMandatory = true;
            script3.Parameters.Add(param1);

            Script script4 = new Script();
            script4.Name = "Remove User";

            subcat2.Scripts.Add(script3);
            subcat2.Scripts.Add(script4);
          //  cat2.SubCategories.Add(subcat2);
            cats.Add(cat2);

            //mainRepositoryView1.LoadCategory(cats);
            mainRepositoryView1.LoadCategory();


            //ScriptRepository.ScriptServiceRef.ScriptRepositoryClient proxy = new ScriptServiceRef.ScriptRepositoryClient();
            //object obj = proxy.GetAllCategoryAndSubcategory();
        }
    }
}

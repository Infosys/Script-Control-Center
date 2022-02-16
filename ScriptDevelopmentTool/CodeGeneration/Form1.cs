using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Infosys.ATR.CodeGeneration;

namespace CodeGeneration
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CSharpCodeGenerator genertor = new CSharpCodeGenerator();
            List<PropertyDef> properties = new List<PropertyDef>();
            PropertyDef property = new PropertyDef();
            property.PropertyName = "EmployeeName";
            property.PropertyType = typeof(string);
            property.Comments = "Property to hold name";
            property.IsCollection = false;

            properties.Add(property);

            PropertyDef property2 = new PropertyDef();
            property2.PropertyName = "EmpployeeNumber";
            property2.PropertyType = typeof(int);
            property2.Comments = "Property to hold number";
            property2.IsCollection = false;

            properties.Add(property2);

            textBox1.Text = CSharpCodeGenerator.Generate("Infosys.ATR", "ATRApp", properties);
        }
    }
}

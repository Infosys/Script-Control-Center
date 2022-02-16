using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.IO;

using System.Data.OleDb;


namespace Infosys.ATR.UIAutomation.LookupReader
{
    public partial class ExcelReader : Form
    {
        public ExcelReader()
        {
            InitializeComponent();
           
            panel1.Visible = false;
            System.Windows.Forms.ToolTip ToolTipForCopy = new System.Windows.Forms.ToolTip();
            ToolTipForCopy.SetToolTip(this.btnAppName, "Copy to Clipboard");
            ToolTipForCopy.SetToolTip(this.btnFileExtension, "Copy to Clipboard");
            ToolTipForCopy.SetToolTip(this.btnFilePath, "Copy to Clipboard");
            ToolTipForCopy.SetToolTip(this.btnNode, "Copy to Clipboard");
            this.Height = 110;
            this.Width = 420;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string filepath = ConfigurationManager.AppSettings["FileLocation"];
            panel1.Visible = false;
            lblNoResults.Visible = false;
            string searchText=txtbxSearch.Text;
            bool IsDataFound = false;
            System.Data.DataTable ds = ReadExcelToTable(filepath);
          for(int i=0;i<ds.Rows.Count;i++)
          {
             // if (ds.Rows[i][0].ToString().ToLower().Replace(" ", string.Empty).Contains(searchText.ToLower().Replace(" ", string.Empty)))
              if (searchText.ToLower().Replace(" ", string.Empty).Contains(ds.Rows[i][0].ToString().ToLower().Replace(" ", string.Empty)))
              {
                  txtApplicationName.Text = ds.Rows[i][0].ToString();
                  txtFileExtension.Text = ds.Rows[i][1].ToString();
                  txtFilePath.Text = ds.Rows[i][2].ToString();
                  txtNode.Text = ds.Rows[i][3].ToString();
                  IsDataFound = true;
                  break;
              }
             
          }
          if (IsDataFound)
          {
              panel1.Visible = true;
              this.Height = 330;
              this.Width = 630;
          }
          else
          {
              lblNoResults.Visible = true;
              this.Height = 110;
              this.Width = 400;
              
          }

        }
      

        private System.Data.DataTable ReadExcelToTable(string path)
        {
            //Connection String

            string connstring = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties='Excel 8.0;HDR=YES;IMEX=1';";
           
            using (OleDbConnection conn = new OleDbConnection(connstring))
            {
                conn.Open();
                //Get All Sheets Name
           System.Data.DataTable sheetsName = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" });

                //Get the First Sheet Name
                string firstSheetName = sheetsName.Rows[0][2].ToString();

                //Query String 
                string sql = string.Format("SELECT * FROM [{0}]", firstSheetName);
                OleDbDataAdapter ada = new OleDbDataAdapter(sql, connstring);
                DataSet set = new DataSet();
                ada.Fill(set);
                return set.Tables[0];
            }
        }

       
        private void btnFileExtension_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(this.txtFileExtension.Text);
        }

        private void btnAppName_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(this.txtApplicationName.Text);
        }

        private void btnFilePath_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(this.txtFilePath.Text);
        }

        private void btnNode_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(this.txtNode.Text);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtbxSearch.Text = "";
            txtApplicationName.Text = "";
            txtFileExtension.Text = "";
            txtFilePath.Text = "";
            txtNode.Text = "";
            panel1.Visible = false;
            this.Height = 110;
            this.Width = 420;
        }

       
       

       
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Configuration;


namespace IMSWorkBench.Scripts.Views.ScriptsLayout
{
    public delegate bool PreRemoveTab(int indx);
    public class TabControlEx : TabControl
    {
        public TabControlEx()
            : base()
        {
            PreRemoveTabPage = null;
            this.DrawMode = TabDrawMode.OwnerDrawFixed;
        }

        public PreRemoveTab PreRemoveTabPage;

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            RectangleF tabTextArea = RectangleF.Empty;
            for (int nIndex = 0; nIndex < this.TabCount; nIndex++)
            {
                if (nIndex != this.SelectedIndex)
                {
                    /*if not active draw ,inactive close button*/
                    tabTextArea = (RectangleF)this.GetTabRect(nIndex);
                    //using (Bitmap bmp = new Bitmap(@"C:\Users\nimna_sandeep\Documents\Visual Studio 2010\Projects\TabwithClose\TabwithClose\Inactive.png"))
                    //{
                    //    e.Graphics.DrawImage(bmp,
                    //        tabTextArea.X+tabTextArea.Width -16, 5, 13, 13);
                    //}
                }
                else
                {
                   
                    tabTextArea = (RectangleF)this.GetTabRect(nIndex);
                    LinearGradientBrush br = new LinearGradientBrush(tabTextArea,
                        SystemColors.ControlLightLight, SystemColors.Control,
                        LinearGradientMode.Vertical);
                    e.Graphics.FillRectangle(br, tabTextArea);
                     string path = Directory.GetCurrentDirectory();
                    /*if active draw ,inactive close button*/
                     using (Bitmap bmp = new Bitmap(path + @"\Images\Close2.png"))
                    {
                        e.Graphics.DrawImage(bmp,
                            tabTextArea.X + tabTextArea.Width - 16, 5, 13, 13);
                           // tabTextArea.X + tabTextArea.Width - 16, 14, 13, 13);
                    }
                    br.Dispose();
                }
                string str = this.TabPages[nIndex].Text;
                StringFormat stringFormat = new StringFormat();

                stringFormat.Alignment = StringAlignment.Center ;
                using (SolidBrush brush = new SolidBrush(
                    this.TabPages[nIndex].ForeColor))
                {
                    //Draw the tab header text
                    e.Graphics.DrawString(str, this.Font, brush,
                    tabTextArea, stringFormat);
                }
            }

            //Rectangle r = e.Bounds;
            //r = GetTabRect(e.Index);
            //r.Offset(2, 2);
            //r.Width = 5;
            //r.Height = 5;
            //Brush b = new SolidBrush(Color.Black);
            //Pen p = new Pen(b);
            //e.Graphics.DrawLine(p, r.X, r.Y, r.X + r.Width, r.Y + r.Height);
            //e.Graphics.DrawLine(p, r.X + r.Width, r.Y, r.X, r.Y + r.Height);

            //string titel = this.TabPages[e.Index].Text;
            //Font f = this.Font;
            //e.Graphics.DrawString(titel, f, b, new PointF(r.X + 5, r.Y));
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            //Point p = e.Location;
            //for (int i = 0; i < TabCount; i++)
            //{
            //    Rectangle r = GetTabRect(i);
            //    r.Offset(2, 2);
            //    r.Width = 5;
            //    r.Height = 5;
            //    if (r.Contains(p))
            //    {
            //        if (MessageBox.Show("Would you like to Close this Tab?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            //        {
            //            CloseTab(i);
            //            break;
            //        }


            //    }
            //}
            //for (int i = 0; i < TabCount; i++)
            //{
            RectangleF tabTextArea = (RectangleF)this.GetTabRect(SelectedIndex);
            tabTextArea =
                new RectangleF(tabTextArea.X + tabTextArea.Width - 16, 5, 13, 13);
            Point pt = new Point(e.X, e.Y);
            if (tabTextArea.Contains(pt))
            {
                string tabTitle = this.TabPages[SelectedIndex].Text;
                if(tabTitle.Contains("Untitled-"))
                {
                    var confirmResult = MessageBox.Show("You have not saved the Script.Do you still want to Close??", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (confirmResult==DialogResult.Yes)
                    {
                        CloseTab(SelectedIndex);
                        DeleteFiles(tabTitle);
                        
                    }
                }
                else
                //if (MessageBox.Show("Would you like to Close this Tab?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var confirmResult= MessageBox.Show("Would you like to Close this Tab?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (confirmResult == DialogResult.Yes)
                    {
                        CloseTab(SelectedIndex);
                    }
                    // break;
                }

                // }
            }


        }
        public void DeleteFiles(string filename)
        {
            string UsecaseLocation = ConfigurationManager.AppSettings["UsecaseLocation"];

            if (Infosys.WEM.Infrastructure.Common.ValidationUtility.InvalidCharValidatorForFile(Path.GetFileNameWithoutExtension(filename)))
            {
                MessageBox.Show("Please provide the file name without Special Characters", "Special Characters...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (File.Exists(UsecaseLocation + "\\" + filename + ".txt"))
                    File.Delete(UsecaseLocation + "\\" + filename + ".txt");
        }

        private void CloseTab(int i)
        {
            if (PreRemoveTabPage != null)
            {
                bool closeIt = PreRemoveTabPage(i);
                if (!closeIt)
                    return;
            }
            TabPages.Remove(TabPages[i]);
        }
    }

}

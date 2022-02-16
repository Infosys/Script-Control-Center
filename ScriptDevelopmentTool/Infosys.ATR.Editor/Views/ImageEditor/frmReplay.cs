using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Infosys.ATR.UIAutomation.Entities;

namespace Infosys.ATR.Editor.Views
{
    public partial class frmReplay : Form
    {
        internal AutomationConfig AutoConfig { get; set; }
        internal string BaseDir { get; set; }
        int current = 0;
        int length;
        List<ReplayImage> images = new List<ReplayImage>();

        public frmReplay()
        {
            InitializeComponent();
        }

        internal void ShowForm()
        {
            AutoConfig.AppConfigs.ForEach(a =>
            {
                a.ScreenConfigs.ForEach(s =>
                {
                    s.ScreenImageConfig.StateImageConfig.ForEach(i =>
                    {
                        if (i.CenterImageName != null)
                        {
                            images.Add(new ReplayImage
                            {
                                Path = Path.Combine(BaseDir, i.CenterImageName.ImageName),
                                State = i.State,
                                Title = s.ScreenName
                            });
                        }
                    });
                });
            });
            length = images.Count - 1;
            ShowImage();
            this.Show();
        }

        private void lnkNext_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            current++;

            if (current <= length)
            {
                ShowImage();
            }
            else if (current > length)
            {
                current = length;               
            }
        }

        private void ShowImage()
        {
            var c = images[current];

            if (File.Exists(c.Path))
            {
                using (var t = new Bitmap(c.Path))
                {
                    lblTitle.Text = c.Title;
                    pictureBox1.Image = new Bitmap(t);
                }
            }            
        }

        private void lnkPrevious_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            current--;

            if (current >= 0)
            {
                ShowImage();                
            }
            else if (current < 0)
            {
                current = 0;
            }

        }
    }

    public class ReplayImage
    {
        public string Path { get; set; }
        public string Title { get; set; }
        public string State { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Windows.Threading;

namespace Infosys.ATR.UIAutomation.Recorder
{
    public delegate void PassBrowser(WebBrowser browser);
    public partial class Form3 : Form
    {
        public event EventHandler formClosed;
        public event PassBrowser passBrowserEvent;
        //private bool documentLoaded = false;
        protected virtual void OnFormClosed(EventArgs e)
        {
            if (formClosed != null)
            {
                formClosed(this, e);
            }
        }

        protected virtual void OnDocumentLoadCompleted(WebBrowser browser)
        {
            if (passBrowserEvent != null)
            {
                passBrowserEvent(browser);
            }
        }

        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            this.BringToFront();
            //webBrowserWindow.Size = new Size(this.Width - 50, this.Height - 50);   
            toolTip1.SetToolTip(btn_Go, "Browse");
            toolTip1.SetToolTip(btnAddTab, "Add a new tab");
        }
        private void Form3_Resize(object sender, EventArgs e)
        {
            //webBrowserWindow.Size = new Size(this.Width - 50, this.Height - 50);
            this.TopMost = false;
        }

        private void btn_Go_Click(object sender, EventArgs e)
        {
            try
            {
                //if (this.webBrowserWindow.InvokeRequired)
                //{
                //    webBrowserWindow.BeginInvoke(new Action(LaunchWebBrowser));
                //}
                //else
                //{
                //    LaunchWebBrowser();
                //}

                //for the tab control
                if (this.tbcBrowsers.InvokeRequired)
                    this.tbcBrowsers.BeginInvoke(new Action(StartBrowserAsNewTab));
                else
                    StartBrowserAsNewTab();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occured, please try again. Reason- " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StartBrowserAsNewTab()
        {
            string url = txt_Url.Text;
            string checkforduplicate = url.Replace("http://", "");
            WebBrowser browser = new WebBrowser();
            browser.ScriptErrorsSuppressed = true;
            browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser_DocumentCompleted);
            txt_Url.Text = "http://" + checkforduplicate;
            if (tbcBrowsers.TabPages.Count == 0)
            {
                tbcBrowsers.TabPages.Add("...");
                browser.Url = new Uri("http://" + checkforduplicate);
                browser.Dock = DockStyle.Fill;
                tbcBrowsers.TabPages[0].Controls.Add(browser);
                //tbcBrowsers.TabPages[0].Text = browser.DocumentTitle + "...";
            }
            else
            {
                //get the current web browser instance
                foreach (Control ctl in tbcBrowsers.SelectedTab.Controls)
                {
                    if (ctl.GetType() == typeof(WebBrowser))
                    {
                        browser = ctl as WebBrowser;
                        browser.Url = new Uri("http://" + checkforduplicate);
                        //tbcBrowsers.SelectedTab.Text = browser.DocumentTitle + "...";
                        break;
                    }
                }
            }

        }

        void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //documentLoaded = true;
            tbcBrowsers.SelectedTab.Text = (sender as WebBrowser).DocumentTitle;
            OnDocumentLoadCompleted(sender as WebBrowser);
        }

        public void LaunchWebBrowser()
        {
            string url = txt_Url.Text;
            string checkforduplicate = url.Replace("http://", "");
            this.webBrowserWindow.Url = new Uri("http://" + checkforduplicate);
            //this.webBrowserWindow.Url = new Uri(txt_Url.Text);
            txt_Url.Text = "http://" + checkforduplicate;

        }

        /// <summary>
        /// raise event on form closed to set flag in form1
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            OnFormClosed(e);
        }

        /// <summary>
        /// when document load completes in browser window, raise event to 
        /// pass reference of browser control to form1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            OnDocumentLoadCompleted(sender as WebBrowser);
        }

        private void btnAddTab_Click(object sender, EventArgs e)
        {
            //documentLoaded = false;
            tbcBrowsers.TabPages.Add("...");
            tbcBrowsers.TabPages[tbcBrowsers.TabPages.Count - 1].Select();
            WebBrowser browser = new WebBrowser();
            browser.ScriptErrorsSuppressed = true;
            browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser_DocumentCompleted);
            browser.Dock = DockStyle.Fill;
            tbcBrowsers.TabPages[tbcBrowsers.TabPages.Count - 1].Controls.Add(browser);
        }

        private void tbcBrowsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            WebBrowser browser = null;
            foreach (Control ctl in tbcBrowsers.SelectedTab.Controls)
            {
                if (ctl.GetType() == typeof(WebBrowser))
                {
                    browser = ctl as WebBrowser;
                    break;
                }
            }
            OnDocumentLoadCompleted(browser);
        }
    }
}

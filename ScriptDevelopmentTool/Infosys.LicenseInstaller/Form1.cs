using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Infosys.LicensingFramework;

namespace Infosys.LicenseInstaller
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnInstall_Click(object sender, EventArgs e)
        {
            if (VerifyData())
            {
                this.Cursor = Cursors.WaitCursor;
                LicenseInstalltionResult result = Installation.Start(txtLicenseKey.Text.Trim());
                if (result != null)
                {
                    if (result.IsSuccess)
                        MessageBox.Show(result.AnyInformation, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show(result.AnyInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                this.Cursor = Cursors.Default;
            }
            else
                MessageBox.Show("Please provide data in all the mandatory fields.", "Data missing", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool VerifyData()
        {
            bool verified = false;
            if (!string.IsNullOrEmpty(txtLicenseKey.Text.Trim()))// && !string.IsNullOrEmpty(txtProductInstallPath.Text.Trim()))
                verified = true;
            return verified;
        }
    }
}

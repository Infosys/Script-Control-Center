using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.IO;

namespace Infosys.ATR.SecureModuleAccessCount
{
    public partial class SecureDataForm : Form
    {
        public SecureDataForm()
        {
            InitializeComponent();
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtKey.Text))
                {
                    MessageBox.Show("Please provide a Key to start encryption (minimum 8 characters), e.g. Infosys1", "Key Missing", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (string.IsNullOrEmpty(txtPlainData.Text))
                {
                    MessageBox.Show("Please provide data to be encrypted", "Data Missing", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    TripleDES desEncryption = CreateDESCrypto(txtKey.Text.Trim());
                    //desEncryption.Key = ASCIIEncoding.ASCII.GetBytes(txtKey.Text.Trim());
                    //desEncryption.IV = ASCIIEncoding.ASCII.GetBytes(txtKey.Text.Trim());
                    ICryptoTransform desEncryptor = desEncryption.CreateEncryptor();

                    MemoryStream stream = new MemoryStream();
                    CryptoStream cryptedStream = new CryptoStream(stream, desEncryptor, CryptoStreamMode.Write);
                    StreamWriter writer = new StreamWriter(cryptedStream);
                    writer.Write(txtPlainData.Text);
                    writer.Flush();
                    cryptedStream.FlushFinalBlock();
                    writer.Flush();
                    txtEncryptedData.Text = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Encryption Failed- " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnValidate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtKey.Text))
            {
                MessageBox.Show("Please provide a Key to start encryption (minimum 8 characters), e.g. Infosys1", "Key Missing", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (string.IsNullOrEmpty(txtEncryptedData.Text))
            {
                MessageBox.Show("Encrypted data to be validated is missing", "Data Missing", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                TripleDES desEncryption = CreateDESCrypto(txtKey.Text.Trim());
                ICryptoTransform desDecryptor = desEncryption.CreateDecryptor();

                MemoryStream stream = new MemoryStream(Convert.FromBase64String(txtEncryptedData.Text));
                CryptoStream cryptedStream = new CryptoStream(stream, desDecryptor, CryptoStreamMode.Read);
                StreamReader reader = new StreamReader(cryptedStream);
                string plainText = reader.ReadToEnd();
                if(plainText == txtPlainData.Text)
                    MessageBox.Show("Encryption of the data is verifed to be correct", "Correct Encryption", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("Encryption of the data is verifed to be incorrect", "Incorrect Encryption", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private TripleDES CreateDESCrypto(string key)
        {
            MD5 md5provider = new MD5CryptoServiceProvider();
            TripleDES tripledes = new TripleDESCryptoServiceProvider();
            tripledes.Key = md5provider.ComputeHash(Encoding.Unicode.GetBytes(key));
            tripledes.IV = new byte[tripledes.BlockSize / 8];
            return tripledes;
        }


    }
}

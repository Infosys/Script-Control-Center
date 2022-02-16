/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.LicenseValidationClient
{
    public class EncryptDecrypt
    {
        const string SKVAL = "SW5mb3N5czE=";
        public static string Encrypt(string plainText)
        {
            string encryoptedText = "";
            string key = BackFromBase64(SKVAL);

            if (!string.IsNullOrEmpty(plainText))
            {
                //DESCryptoServiceProvider desEncryption = new DESCryptoServiceProvider();
                TripleDES desEncryption = CreateDESCrypto(key);
                //desEncryption.Key = ASCIIEncoding.ASCII.GetBytes(key);
                //desEncryption.IV = ASCIIEncoding.ASCII.GetBytes(key);
                ICryptoTransform desEncryptor = desEncryption.CreateEncryptor();

                MemoryStream stream = new MemoryStream();
                CryptoStream cryptedStream = new CryptoStream(stream, desEncryptor, CryptoStreamMode.Write);
                StreamWriter writer = new StreamWriter(cryptedStream);
                writer.Write(plainText);
                writer.Flush();
                cryptedStream.FlushFinalBlock();
                writer.Flush();
                encryoptedText = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length);
            }
            return encryoptedText;
        }

        public static string Decrypt(string encryptedText)
        {
            string plainText = "";
            string key = BackFromBase64(SKVAL);

            if (!string.IsNullOrEmpty(encryptedText))
            {
                //DESCryptoServiceProvider desEncryption = new DESCryptoServiceProvider();
                TripleDES desEncryption = CreateDESCrypto(key);
                //desEncryption.Key = ASCIIEncoding.ASCII.GetBytes(key);
                //desEncryption.IV = ASCIIEncoding.ASCII.GetBytes(key);
                ICryptoTransform desDecryptor = desEncryption.CreateDecryptor();

                MemoryStream stream = new MemoryStream(Convert.FromBase64String(encryptedText));
                CryptoStream cryptedStream = new CryptoStream(stream, desDecryptor, CryptoStreamMode.Read);
                StreamReader reader = new StreamReader(cryptedStream);
                plainText = reader.ReadToEnd();               
            }
            return plainText;
        }

        private static TripleDES CreateDESCrypto(string key)
        {
            MD5 md5provider = new MD5CryptoServiceProvider();
            TripleDES tripledes = new TripleDESCryptoServiceProvider();
            tripledes.Key = md5provider.ComputeHash(Encoding.Unicode.GetBytes(key));
            tripledes.IV = new byte[tripledes.BlockSize / 8];
            return tripledes;
        }              

        private static string BackFromBase64(string base64EncodedText)
        {
            var base64EncodedDataBytes = System.Convert.FromBase64String(base64EncodedText);
            return System.Text.Encoding.UTF8.GetString(base64EncodedDataBytes);
        }
    }
}

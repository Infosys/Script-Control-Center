/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infosys.WEM.Infrastructure.SecurityCore;
using System.Security.Cryptography;
using System.IO;

namespace Infosys.WEM.SecureHandler
{
    public class SecurePayload
    {
        public const string PASS = "IAP2GO_SEC!URE";
        static string secureK = DateTime.Now.Ticks.ToString();        
        public  const string keyText = "aWFw";
        public static string Secure(string textToSecure, string passCode)
        {
            if(string.IsNullOrEmpty(textToSecure.Trim()))
                return "";
            int indexK = 0;
            string cipherTxt = "";

            if (passCode.Equals(PASS))
            {
                cipherTxt = SecureData.EncryptData(textToSecure, secureK, out indexK);
                cipherTxt = cipherTxt + keyText + Base64Encode(indexK.ToString() + indexK.ToString().Length.ToString());
            }
            return cipherTxt;
        }

        public static string UnSecure(string secureText, string passCode)
        {

            if (string.IsNullOrEmpty(secureText.Trim()))
                return "";
            string plainText = "";

            if (passCode.Equals(PASS))
            {
                if (!secureText.EndsWith("162"))
                {
                    string[] secureTextSplit = secureText.Trim().Split(new string[] { keyText }, StringSplitOptions.RemoveEmptyEntries);
                    secureText = secureTextSplit[0] + Base64Decode(secureTextSplit[1]);
                }

                int indexK = int.Parse(secureText[secureText.Length - 1].ToString());
                int cipherLength = secureText.Length - indexK-1;
                int keyLength = indexK;

                if (cipherLength > 1)
                {
                    plainText = SecureData.DecryptData(secureText.Substring(0, cipherLength), Convert.ToInt16(secureText.Substring(cipherLength, keyLength)));
                }
                else
                    plainText = secureText;
            }
            return plainText;
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        private static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }      

        public static byte[] SecureBytes(byte[] plainBytes)
        {
            return SecureData.EncryptByteData(plainBytes);
        }

        public static byte[] UnSecureBytes(byte[] encryptedBytes) 
        {
            return SecureData.DecryptByteData(encryptedBytes);
        }
    }
}

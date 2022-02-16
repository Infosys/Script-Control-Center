/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.CompilerServices;
using DataAccess = Infosys.WEM.Infrastructure.SecurityCore.Data;
using DataEntity = Infosys.WEM.Infrastructure.SecurityCore.Data.Entity;
using BusinessEntity = Infosys.WEM.Infrastructure.SecurityCore.Business.Entity;
using Translator = Infosys.WEM.Infrastructure.SecurityCore.Translators;
using System.Net;

namespace Infosys.WEM.Infrastructure.SecurityCore
{
    public class SecureData
    {
        const string pass = "IAP2GO_SEC!URE";
        const string keyText = "aWFw";
        const string sharedSecretPrivate = "aWFwU2lnbmF0dXJl"; //iapSignature
        static string secureK = DateTime.Now.Ticks.ToString();

        public static string Secure(string textToSecure, string passCode)
        {
            if (string.IsNullOrEmpty(textToSecure.Trim()))
                return "";
            int indexK = 0;
            string cipherTxt = "";

            if (passCode.Equals(pass))
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

            if (passCode.Equals(pass))
            {
                string[] secureTextSplit = secureText.Trim().Split(new string[] { keyText }, StringSplitOptions.RemoveEmptyEntries);
                secureText = secureTextSplit[0] + Base64Decode(secureTextSplit[1]);

                int indexK = int.Parse(secureText[secureText.Length - 1].ToString());
                int cipherLength = secureText.Length - indexK - 1;
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

        private static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string DecryptData(string cipherText, int keyIndex)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentNullException("cipherText");

            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

            byte[] sharedSecretBytes = new byte[cipherTextBytes.Length - keyIndex];
            byte[] actualCipherText = new byte[keyIndex];

            for (int i = keyIndex, j = 0; i < cipherTextBytes.Length; i++, j++)
                sharedSecretBytes[j] = cipherTextBytes[i];
            for (int i = 0; i < keyIndex; i++)
                actualCipherText[i] = cipherTextBytes[i];

            string sharedSecret = Encoding.Unicode.GetString(sharedSecretBytes);
            cipherText = Convert.ToBase64String(actualCipherText);

            // Declare the AesManaged object 
            // used to decrypt the data. 
            AesManaged aesAlg = null;


            // Declare the string used to hold 
            // the decrypted text. 
            string plaintext = null;

            try
            {
                // generate the key from the shared secret and the salt 
                PasswordDeriveBytes key = new PasswordDeriveBytes(sharedSecret,
                    null, "SHA256", 1000);

                // Create a AesManaged object 
                // with the specified key and IV. 
                aesAlg = new AesManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

                // Create a decrytor to perform the stream transform. 
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                // Create the streams used for decryption.                 
                byte[] bytes = Convert.FromBase64String(cipherText);
                using (MemoryStream msDecrypt = new MemoryStream(bytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))

                            // Read the decrypted bytes from the decrypting stream 
                            // and place them in a string. 
                            plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
            finally
            {
                // Clear the AesManaged object. 
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            return plaintext;
        }

        public static string EncryptData(string plainText, string sharedSecret, out int keyIndex)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException("plainText");
            if (string.IsNullOrEmpty(sharedSecret))
                throw new ArgumentNullException("sharedSecret");

            string outStr = null;                       // Encrypted string to return 
            AesManaged aesAlg = null;              // AesManaged object used to encrypt the data. 

            try
            {
                // generate the key from the shared secret and the salt 
                PasswordDeriveBytes key = new PasswordDeriveBytes(sharedSecret
                    , null, "SHA256", 1000);

                // Create a AesManaged object 
                // with the specified key and IV. 
                aesAlg = new AesManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

                // Create a decrytor to perform the stream transform. 
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption. 
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                    }

                    byte[] cryptedText = msEncrypt.ToArray();
                    byte[] sharedSecretBytes = Encoding.Unicode.GetBytes(sharedSecret);
                    byte[] cipherText = new byte[cryptedText.Length + sharedSecretBytes.Length];
                    keyIndex = cryptedText.Length;

                    for (int i = 0; i < cryptedText.Length; i++)
                        cipherText[i] = cryptedText[i];
                    for (int j = cryptedText.Length, k = 0; j < (cryptedText.Length + sharedSecretBytes.Length);
                        j++, k++)
                        cipherText[j] = sharedSecretBytes[k];

                    outStr = Convert.ToBase64String(cipherText);
                }
            }
            finally
            {
                // Clear the AesManaged object. 
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            // Return the encrypted bytes from the memory stream. 
            return outStr;
        }

        internal static void ProtectData(string plainText, string sharedSecret, Guid companyId, string domain, string serviceId, string description)
        {
            int keyIndex = 0;
            string encryptedData = EncryptData(plainText, sharedSecret, out keyIndex);

            DataAccess.AppServiceAccountsDS dataAccess = new DataAccess.AppServiceAccountsDS();

            dataAccess.Insert
                (
                    DataAccess.AppServiceAccountsExtensions.GeneratePartitionKeyAndRowKey
                    (
                        Translator.AppServiceAccountsAndServiceAccount.TranslateServiceAccountBEToAppServiceAccountsDE
                        (
                            new BusinessEntity.ServiceAccount
                            {
                                CipherKeyIndex = keyIndex,
                                CompanyId = companyId,
                                CreatedOn = System.DateTime.UtcNow,
                                Description = description,
                                Domain = domain,
                                IsActive = true,
                                LastModifiedOn = null,
                                ServiceId = serviceId,
                                ServicePassword = encryptedData


                            }
                         )
                     )
                  );

        }

        public static byte[] DecryptByteData(byte[] cipherBytesToBeDecrypted)
        {
            int keyIndex = 0;

            if (cipherBytesToBeDecrypted == null)
                throw new ArgumentNullException("cipherText");

            List<byte> lstByte = new List<byte>();
            byte[] bytes = null;
            for (int i = cipherBytesToBeDecrypted.Length - 1; i >= 0; i--)
            {
                lstByte.Add(cipherBytesToBeDecrypted[i]);
                if (lstByte.Count > 0)
                {
                    bytes = new byte[lstByte.ToArray().Length];
                    int index = lstByte.ToArray().Length - 1;
                    foreach (byte item in lstByte.ToArray())
                    {
                        bytes[index] = item;
                        index--;
                    }

                    if (Encoding.Unicode.GetString(bytes).ToString().StartsWith(keyText))
                    {
                        string cipherText = Encoding.Unicode.GetString(bytes).ToString();
                        cipherText = cipherText.Substring(keyText.Length, cipherText.Length - keyText.Length);
                        int indexK = int.Parse(cipherText[cipherText.Length - 1].ToString());
                        keyIndex = Convert.ToInt16(cipherText.Substring(0, indexK));
                        break;
                    }
                }
            }

            byte[] cipherTextBytes = new byte[cipherBytesToBeDecrypted.Length - bytes.Length];
            Array.Copy(cipherBytesToBeDecrypted, cipherTextBytes, cipherTextBytes.Length);

            byte[] sharedSecretBytes = new byte[cipherTextBytes.Length - keyIndex];
            byte[] actualCipherText = new byte[keyIndex];

            for (int i = keyIndex, j = 0; i < cipherTextBytes.Length; i++, j++)
                sharedSecretBytes[j] = cipherTextBytes[i];
            for (int i = 0; i < keyIndex; i++)
                actualCipherText[i] = cipherTextBytes[i];

            string sharedSecret = Encoding.Unicode.GetString(DecryptSignature(sharedSecretBytes));
            cipherBytesToBeDecrypted = actualCipherText;

            // Declare the AesManaged object 
            // used to decrypt the data. 
            AesManaged aesAlg = null;


            // Declare the byte used to hold 
            // the decrypted byte. 
            byte[] decryptedBytes = null;

            try
            {
                // generate the key from the shared secret and the salt 
                PasswordDeriveBytes key = new PasswordDeriveBytes(sharedSecret, null, "SHA256", 1000);

                // Create a AesManaged object 
                // with the specified key and IV. 
                aesAlg = new AesManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.Mode = CipherMode.CBC;
                // Create a decrytor to perform the stream transform. 
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                // Create the streams used for decryption.      

                using (MemoryStream msDecrypt = new MemoryStream())
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
                    {
                        csDecrypt.Write(cipherBytesToBeDecrypted, 0, cipherBytesToBeDecrypted.Length);
                        csDecrypt.Close();
                    }

                    decryptedBytes = msDecrypt.ToArray();
                }
            }
            finally
            {
                // Clear the AesManaged object. 
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            return decryptedBytes;
        }
        public static byte[] EncryptByteData(byte[] plainBytesToBeEncrypted)
        {
            string sharedSecret = string.Format("{0}#{1}#{2}#{3}", "iapSecretKey", System.Security.Principal.WindowsIdentity.GetCurrent().Name,
                Dns.GetHostAddresses(Dns.GetHostName())[0].ToString(), DateTime.Now.Ticks.ToString());

            if (plainBytesToBeEncrypted == null)
                throw new ArgumentNullException("plainText");
            if (string.IsNullOrEmpty(sharedSecret))
                throw new ArgumentNullException("sharedSecret");

            byte[] encryptedBytes = null;          // Encrypted bytes to return 
            AesManaged aesAlg = null;              // AesManaged object used to encrypt the data. 

            try
            {
                // generate the key from the shared secret and the salt 
                PasswordDeriveBytes key = new PasswordDeriveBytes(sharedSecret, null, "SHA256", 1000);

                // Create a AesManaged object 
                // with the specified key and IV. 
                aesAlg = new AesManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

                // Create a decrytor to perform the stream transform. 
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption. 
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(plainBytesToBeEncrypted, 0, plainBytesToBeEncrypted.Length);
                        csEncrypt.Close();
                    }

                    byte[] cryptedText = msEncrypt.ToArray();
                    int keyIndex = cryptedText.Length;
                    byte[] sharedSecretBytes = EncryptSignature(Encoding.Unicode.GetBytes(sharedSecret));
                    byte[] identifier = Encoding.Unicode.GetBytes(keyText + keyIndex.ToString() + keyIndex.ToString().Length.ToString());

                    encryptedBytes = new byte[cryptedText.Length + sharedSecretBytes.Length + identifier.Length];

                    for (int i = 0; i < cryptedText.Length; i++)
                        encryptedBytes[i] = cryptedText[i];
                    for (int j = cryptedText.Length, k = 0; j < (cryptedText.Length + sharedSecretBytes.Length);
                        j++, k++)
                        encryptedBytes[j] = sharedSecretBytes[k];
                    for (int j = (cryptedText.Length + sharedSecretBytes.Length), k = 0; j < ((cryptedText.Length + sharedSecretBytes.Length) + identifier.Length);
                        j++, k++)
                        encryptedBytes[j] = identifier[k];
                }
            }
            finally
            {
                // Clear the AesManaged object. 
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            // Return the encrypted bytes from the memory stream. 
            return encryptedBytes;
        }

        private static byte[] EncryptSignature(byte[] signatureToBeEncrypted)
        {
            if (signatureToBeEncrypted == null)
                throw new ArgumentNullException("byteSignatureToBeEncrypted");

            byte[] encryptSignature = null;
            AesManaged aesAlg = null;  // AesManaged object used to encrypt the data. 


            try
            {
                // generate the key from the shared secret and the salt 
                PasswordDeriveBytes key = new PasswordDeriveBytes(sharedSecretPrivate, null, "SHA256", 1000);

                // Create a AesManaged object 
                // with the specified key and IV. 
                aesAlg = new AesManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

                // Create a decrytor to perform the stream transform. 
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption. 
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(signatureToBeEncrypted, 0, signatureToBeEncrypted.Length);
                        csEncrypt.Close();
                    }

                    byte[] cryptedText = msEncrypt.ToArray();
                    byte[] sharedSecretBytes = Encoding.Unicode.GetBytes(sharedSecretPrivate);
                    byte[] cipherText = new byte[cryptedText.Length + sharedSecretBytes.Length];

                    for (int i = 0; i < cryptedText.Length; i++)
                        cipherText[i] = cryptedText[i];
                    for (int j = cryptedText.Length, k = 0; j < (cryptedText.Length + sharedSecretBytes.Length);
                        j++, k++)
                        cipherText[j] = sharedSecretBytes[k];

                    encryptSignature = cipherText;
                }
            }
            finally
            {
                // Clear the AesManaged object. 
                if (aesAlg != null)
                    aesAlg.Clear();
            }
            // Return the encrypted bytes from the memory stream. 
            return encryptSignature;
        }
        private static byte[] DecryptSignature(byte[] cipherBytesToBeDecrypted)
        {
            byte[] decryptedSignature = null;

            if (cipherBytesToBeDecrypted == null)
                throw new ArgumentNullException("cipherBytesToBeDecrypted");

            byte[] cipherTextBytes = cipherBytesToBeDecrypted;
            byte[] sharedSecretBytes = Encoding.Unicode.GetBytes(sharedSecretPrivate);

            int keyIndex = cipherTextBytes.Length - sharedSecretBytes.Length;
            byte[] actualCipherBytes = new byte[keyIndex];

            for (int i = 0; i < keyIndex; i++)
                actualCipherBytes[i] = cipherTextBytes[i];

            // Declare the AesManaged object 
            // used to decrypt the data. 
            AesManaged aesAlg = null;

            try
            {
                // generate the key from the shared secret and the salt 
                PasswordDeriveBytes key = new PasswordDeriveBytes(sharedSecretPrivate,
                    null, "SHA256", 1000);

                // Create a AesManaged object 
                // with the specified key and IV. 
                aesAlg = new AesManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

                // Create a decrytor to perform the stream transform. 
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                // Create the streams used for decryption.                 

                using (MemoryStream msDecrypt = new MemoryStream())
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
                    {
                        csDecrypt.Write(actualCipherBytes, 0, actualCipherBytes.Length);
                        csDecrypt.Close();
                    }
                    decryptedSignature = msDecrypt.ToArray();
                }
            }
            finally
            {
                // Clear the AesManaged object. 
                if (aesAlg != null)
                    aesAlg.Clear();
            }
            return decryptedSignature;
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}

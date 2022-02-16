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
using System.IO;

namespace TestSecureHandler
{
    class Program
    {
        static void Main(string[] args)
        {
            //int indexK = 0;
            //string secureK = DateTime.Now.Ticks.ToString();
            byte[] filecontent = System.IO.File.ReadAllBytes("D:\\Ping Website.ps1");

            //string filecontentText = Encoding.UTF8.GetString(filecontent);

            //string encryptedBytesText = SecureData.EncryptData(filecontentText, secureK, out indexK);

            //string DecryptedBytesText = SecureData.DecryptData(encryptedBytesText, indexK);

            //byte[] DecryptedBytes = Encoding.Unicode.GetBytes(DecryptedBytesText);
            //DecryptedBytes=Encoding.Unicode.GetBytes(Encoding.Unicode.GetString(DecryptedBytes));

            byte[] encryptedBytes = SecureData.EncryptByteData(filecontent);

            //string encryptedBytesText = Encoding.Unicode.GetString(encryptedBytes);
            //string keyText = "aWFw";

            //encryptedBytesText = encryptedBytesText + keyText + indexK.ToString() + indexK.ToString().Length.ToString();
            //encryptedBytes = Encoding.Unicode.GetBytes(encryptedBytesText);


            File.WriteAllBytes("D:\\Ping WebsiteServer.ps1", encryptedBytes);

            encryptedBytes = System.IO.File.ReadAllBytes("D:\\Ping WebsiteServer.ps1");
            //encryptedBytesText = Encoding.Unicode.GetString(encryptedBytes);

            //string[] secureTextSplit = encryptedBytesText.Trim().Split(new string[] { keyText }, StringSplitOptions.None);

            //string actualText = secureTextSplit[0]; 

            ////encryptedBytesText = secureTextSplit[0] + secureTextSplit[1];


            //indexK = int.Parse(secureTextSplit[1][secureTextSplit[1].Length - 1].ToString());
            //int cipherLength = secureTextSplit[0].Length - indexK - 1;
            //int keyLength = indexK;

            ////string actualText = encryptedBytesText.Substring(0, cipherLength);
            //indexK = Convert.ToInt16(secureTextSplit[1].Substring(0, keyLength));

            //encryptedBytes = Encoding.Unicode.GetBytes(actualText);


            byte[] DecryptedBytes = SecureData.DecryptByteData(encryptedBytes);


            Stream ScriptContent = new System.IO.MemoryStream(DecryptedBytes);
            string content = (new StreamReader(ScriptContent)).ReadToEnd();

            Console.Write(content);

            File.WriteAllBytes("D:\\Ping WebsiteDecrypt.ps1", DecryptedBytes);





           

            Console.Read();
             
        }
    }
}

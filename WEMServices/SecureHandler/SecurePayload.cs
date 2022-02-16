using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infosys.WEM.Infrastructure.SecurityCore;

namespace Infosys.WEM.Infrastructure.SecureHandler
{
    public class SecurePayload
    {
        const string secureK = "IAP_Sec12!";
        const string pass = "IAP2GO_SEC!URE";
        public static string Secure(string textToSecure, string passCode)
        {
            int indexK = 0;
            string cipherTxt = "";

            if (passCode.Equals(pass))
            {
                cipherTxt = SecureData.EncryptData(textToSecure, secureK, out indexK);
                cipherTxt = cipherTxt + indexK.ToString() + indexK.ToString().Length.ToString();
            }
            return cipherTxt;
        }

        public static string UnSecure(string secureText, string passCode)
        {
            string plainText = "";

            if (passCode.Equals(pass))
            {
                int indexK = int.Parse(secureText[secureText.Length - 1].ToString());
                int cipherLength = secureText.Length - indexK-1;
                int keyLength = indexK;

                plainText = SecureData.DecryptData(secureText.Substring(0, cipherLength), Convert.ToInt16(secureText.Substring(cipherLength, keyLength)));
            }
            return plainText;
        }
    }
}

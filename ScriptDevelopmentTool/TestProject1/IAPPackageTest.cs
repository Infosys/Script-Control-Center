/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Infosys.IAP.CommonClientLibrary;
using Infosys.IAP.CommonClientLibrary.Models;
using System.Windows.Forms;

namespace TestProject1  
{
    [TestClass]
    public class IAPPackageTest   
    {
        //[TestMethod()] 
        //public void TestMethod1()
        //{
        //    byte[] filecontent = File.ReadAllBytes("d:\\HostName.bat");
        //    ContentMeta content = new ContentMeta() { Name = "HostName", ContentType = "bat", ModuleType = ModuleType.Script };
        //    IAPPackage.Export(content, filecontent);
        //}
         [TestMethod()]
        public void TestMethod1()
        {
            //byte[] filecontent = File.ReadAllBytes("d:\\HostName.bat");
            ContentMeta content = new ContentMeta() ;

            Stream strm = null;
            //IAPPackage.Import(@"C:\Users\ajit_ekghare\Desktop\urlps\UrlPing.iappkg", out content, out strm);
            MessageBox.Show(new StreamReader(strm).ReadToEnd(), "Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Console.WriteLine ();
        }
    }
}

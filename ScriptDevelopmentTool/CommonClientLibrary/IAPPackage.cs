/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.IAP.CommonClientLibrary.Models;
using Infosys.WEM.SecureHandler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Infosys.IAP.CommonClientLibrary
{
    public class PackageMeta
    {
        public  string PackageExtractLoc { set; get; }
        public  ContentMeta Content { set; get; }
        public  Stream FileStream { set; get; }
        public string PackagePath { set; get; }
    }
    public class IAPPackage
    {
        public static void Export(ContentMeta content, byte[] Filecontent)
        {
            if (MessageBox.Show(string.Format("Please confirm if the {0} file opened in the editor to be used for IAP package.", content.ContentType), "Export IAP Package", MessageBoxButtons.OKCancel, MessageBoxIcon.Question).ToString().ToLower() != "ok")
                return;

            using (FolderBrowserDialog fldrDiag = new FolderBrowserDialog())
            {
                fldrDiag.Description = string.Format("Select the location for the IAP package. The location should also have the depended files and folders referred by the .{0} file in the iap package to be created.", content.ContentType);
                var diagres = fldrDiag.ShowDialog();

                if (diagres.ToString().ToLower() == "ok")
                {
                    string path = fldrDiag.SelectedPath;
                    string metaPath = Path.Combine(path, content.Name + ".meta");
                    path = Path.Combine(path, content.Name+"."+content.ContentType);
                    File.WriteAllBytes(path, Filecontent);
                    WriteToXmlFile<ContentMeta>(metaPath, content);
                    var result = Infosys.ATR.Packaging.Operations.Package(path, "iapl");

                    if (result.IsSuccess)
                    {
                        var filecontent = System.IO.File.ReadAllBytes(result.PackagePath); //to read the entire package
                        //filecontent = SecurePayload.SecureBytes(filecontent);
                        File.WriteAllBytes(result.PackagePath, filecontent);
                        MessageBox.Show(string.Format("IAP Package is created at location {0}  successfully.", result.PackagePath), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        if (File.Exists(metaPath)) File.Delete(metaPath);
                        if (File.Exists(path)) File.Delete(path);
                    }
                    else
                        MessageBox.Show(result.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        public static void Import(string strPackageFilePath, out ContentMeta metaData, out Stream fileContent, out string extractionLoc)
        {
            extractionLoc = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),"iappackage"); //Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            if (!Directory.Exists(extractionLoc))
                Directory.CreateDirectory(extractionLoc);

            string fileName = Path.GetFileName(strPackageFilePath);
            string downloadLoc = Path.Combine(extractionLoc, fileName);
            var sContent = System.IO.File.ReadAllBytes(strPackageFilePath);

            if (Encoding.Unicode.GetString(sContent.ToArray()).Contains(SecurePayload.keyText))
                File.WriteAllBytes(downloadLoc, SecurePayload.UnSecureBytes(sContent.ToArray()));
            else
                File.WriteAllBytes(downloadLoc, sContent.ToArray());            

            //now unpackage and read the content of the default file
            var result = Infosys.ATR.Packaging.Operations.Unpackage(downloadLoc, extractionLoc);

            if (result.IsSuccess)
            {
                string metaPath = Path.Combine(result.PackagePath, Path.GetFileNameWithoutExtension(fileName)+ ".meta");
                if (!File.Exists(metaPath))
                    throw new Exception("Expected Metadata file not found in the package provided.");

                metaData = Deserialize<ContentMeta>(File.ReadAllText(metaPath));

                if (!File.Exists(Path.Combine(result.PackagePath, Path.GetFileNameWithoutExtension(fileName) + "." + metaData.ContentType)))
                    throw new Exception("Expected Module file not found in the package provided.");

                fileContent = new MemoryStream(File.ReadAllBytes(Path.Combine(result.PackagePath, Path.GetFileNameWithoutExtension(fileName)+"." + metaData.ContentType)));
                
                if (File.Exists(downloadLoc))
                    File.Delete(downloadLoc);
            }
            else
                throw new Exception(result.Message);
        }
        private static void WriteToXmlFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {   
            try
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                using (TextWriter writer = new StreamWriter(filePath, append))
                {
                    serializer.Serialize(writer, objectToWrite);
                }                
            }finally{}
        }
        public static T Deserialize<T>(string s)
        {
            T t = default(T);
            XmlSerializer xml = new XmlSerializer(typeof(T));
            t = (T)xml.Deserialize(new StringReader(s));
            return t;
        }
        private static MemoryStream SerializeToStream(object o)
        {
            MemoryStream stream = new MemoryStream();
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, o);
            return stream;
        }
        private static object DeserializeFromStream(MemoryStream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            stream.Seek(0, SeekOrigin.Begin);
            object o = formatter.Deserialize(stream);
            return o;
        }
    }
}

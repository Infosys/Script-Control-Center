/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.IAP.Infrastructure.Common.HttpModule.DataAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.IAP.Infrastructure.Common.HttpModule.Entity
{
    public class Translator
    {
        public static ContentFile ContentFileSEtoDE(ContentDetails content)
        {
            ContentFile _content = new ContentFile();
            if (content != null)
            {
                //string[] filePathParts =content.FilePath.Split(new char[]{'/'},StringSplitOptions.RemoveEmptyEntries); 
                _content.CompanyId = content.CompanyId;
                _content.FileContent = content.FileContent;                
                _content.FileExtn = Path.GetExtension(content.FilePath);
                _content.FileName = Path.GetFileName(content.FilePath);
                _content.FilePath = string.Format("{0}/{1}", content.documentsVDFromRoot, content.containerName);
                _content.FileVersion = content.Version;
                _content.CreatedBy = content.CreatedBy;
                _content.RowKey = Path.GetFileName(content.FilePath);

                ///http://<server>/iapworkflowstore/3e0d217f-cac9-4274-8cc9-970c31b6e0ef/1.xaml
                ///http://<server>/iapscriptstore/S_00002_3/S00003_1.bat
                ///
                _content.PartitionKey = string.Format("{0}/{1}", content.documentsVDFromRoot, content.containerName);
                _content.FileId = string.Format("{0}", content.containerName);
                if (content.documentsVDFromRoot.ToLower().Contains("iapworkflowstore"))
                    _content.FileType = "Workflow";
                else
                    _content.FileType = "Script";
            }
            return _content;
        }

        public static ContentDetails ContentFileDEtoSE(ContentFile content)
        {
            ContentDetails _content = new ContentDetails();
            if (content != null)
            {
                _content.CompanyId = content.CompanyId;
                _content.FileContent = content.FileContent;
                _content.Version = Convert.ToInt32(content.FileVersion);
                _content.FilePath = content.FilePath;
                _content.CreatedBy = content.CreatedBy;
            }
            return _content;
        }
    }


    public class ContentDetails 
    {
        public string FilePath { get; set; }         
        public int CompanyId { get; set; }
        public byte[] FileContent { get; set; }
        public int Version { get; set; }  
        public string CreatedBy { get; set; }
        public string documentsVDFromRoot { get; set; }
        public string containerName { get; set; } 
    }
}

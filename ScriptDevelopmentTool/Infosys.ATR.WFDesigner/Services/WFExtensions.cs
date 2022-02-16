/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;

namespace Infosys.ATR.WFDesigner.Services
{
    public class WFExtensions
    {
        public static void Rename()
        {
            string dirPath = ConfigurationManager.AppSettings["DownloadedFilesDir"];

            try
            {
                if (Directory.Exists(dirPath))
                {
                    string[] files = Directory.GetFiles(dirPath, "*.csv");

                    if (files.Length == 1)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(files[0]);
                        File.Copy(files[0], Path.Combine(dirPath,fileName+".xls"),true);
                        File.Delete(files[0]);
                    }
                }
            }
            catch
            {
                throw;
            }
        }

    }
}

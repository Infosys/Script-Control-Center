/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.IAP.Infrastructure.Common.HttpModule.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.IAP.Infrastructure.Common.HttpModule.Entity
{
    public class FileRepository
    {
        public static ContentDetails GetFileDetails(ContentDetails entity)
        {
            ContentDetails result = null;

            var entityItem = Translator.ContentFileSEtoDE(entity);
            var _entity = FileDS.GetOne(entityItem);

            if (_entity != null)
                result = Translator.ContentFileDEtoSE(_entity);

            return result;
        }
        public static bool UploadFileDetails(ContentDetails entity)
        {
            bool result = false;
            var entityItem = Translator.ContentFileSEtoDE(entity);
            var _entity = FileDS.GetOne(entityItem);
            var _contentFile = new ContentFile(); 

            if (_entity != null)
                _contentFile = FileDS.Update(entityItem);
            else
                _contentFile = FileDS.Insert(entityItem);

            if (_contentFile != null)
                    result = true;

            return result;
        }
    }
}

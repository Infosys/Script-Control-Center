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
using Infosys.WEM.Infrastructure.Common;
using SE = Infosys.WEM.Service.Common.Contracts.Data;
using DE = Infosys.WEM.Resource.Entity;
using System.Configuration;

namespace Infosys.WEM.Service.Implementation.Translators.Common
{
    public class APIServiceSE_DE
    {
        public static DE.ReferenceData ReferenceDataSEtoDE(SE.ReferenceData referenceDataSE)
        {
            DE.ReferenceData referenceDataDE = null;
            if (referenceDataSE != null)
            {
                referenceDataDE = new DE.ReferenceData();             
                
                referenceDataDE.Description = referenceDataSE.Description;
                referenceDataDE.ReferenceType = referenceDataSE.ReferenceType;
                referenceDataDE.ReferenceKey = referenceDataSE.ReferenceKey;
                referenceDataDE.ReferenceValue = referenceDataSE.ReferenceValue;
                referenceDataDE.IsDefault =Convert.ToBoolean(referenceDataSE.IsDefault);
                referenceDataDE.IsActive = Convert.ToBoolean(referenceDataSE.IsActive);
                referenceDataDE.PartitionKey = Convert.ToInt32(ConfigurationManager.AppSettings["Company"]).ToString("00000");
                referenceDataDE.RowKey = Convert.ToInt32(ConfigurationManager.AppSettings["Company"]).ToString("00000"); //e.g. to convert 1 to 00001, 10 to 00010

            }
            return referenceDataDE;
        }
        public static List<DE.ReferenceData> ReferenceDataSEListtoDEList(List<SE.ReferenceData> referenceDataSEList)
        {
            List<DE.ReferenceData> referenceDataDEList = null;
            if (referenceDataSEList != null)
            {
                referenceDataDEList = new List<DE.ReferenceData>();
                referenceDataSEList.ForEach(se =>
                {
                    referenceDataDEList.Add(ReferenceDataSEtoDE(se));
                });
            }
            return referenceDataDEList;
        }

        public static SE.ReferenceData ReferenceDataDEtoSE(DE.ReferenceData referenceDataDE)
        {
            SE.ReferenceData referenceDataSE = null;
            if (referenceDataDE != null)
            {
                referenceDataSE = new SE.ReferenceData();
                if (referenceDataDE.PartitionKey.Contains("_"))
                {
                    int index = referenceDataDE.PartitionKey.IndexOf("_");
                    if (index > 0)
                        referenceDataDE.PartitionKey = referenceDataDE.PartitionKey.Substring(index + 1);
                }
                referenceDataSE.Description = referenceDataDE.Description;
                referenceDataSE.ReferenceType = referenceDataDE.ReferenceType;
                referenceDataSE.ReferenceKey = referenceDataDE.ReferenceKey;
                referenceDataSE.ReferenceValue = referenceDataDE.ReferenceValue;
                referenceDataSE.IsDefault = Convert.ToString(referenceDataDE.IsDefault);
                referenceDataSE.IsActive = Convert.ToString(referenceDataDE.IsActive);               
                
            }

            return referenceDataSE;
        }

        public static List<SE.ReferenceData> ReferenceDataDEListtoSEList(List<DE.ReferenceData> referenceDataDEList)
        {
            List<SE.ReferenceData> referenceDataSEList = null;
            if (referenceDataDEList != null)
            {
                referenceDataSEList = new List<SE.ReferenceData>();
                referenceDataDEList.ForEach(de =>
                {
                    referenceDataSEList.Add(ReferenceDataDEtoSE(de));
                });
            }
            return referenceDataSEList;
        }
    }
}

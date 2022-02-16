/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWorkBench.Infrastructure.Library.Services
{
    public static class NativeDataTypes
    {

        public static Hashtable RetrieveDataTypes(string scriptType)
        {
            Hashtable hsDataTypes = new Hashtable();
            switch (scriptType)
            {
                case "ps1":
                    hsDataTypes.Add(scriptType + "01", "string");
                    hsDataTypes.Add(scriptType + "02", "char");
                    //hsDataTypes.Add(scriptType + "03", "byte");
                    hsDataTypes.Add(scriptType + "04", "int");
                    hsDataTypes.Add(scriptType + "05", "long");
                    hsDataTypes.Add(scriptType + "06", "bool");
                    hsDataTypes.Add(scriptType + "07", "decimal");
                    hsDataTypes.Add(scriptType + "08", "single");
                    hsDataTypes.Add(scriptType + "09", "short");
                    hsDataTypes.Add(scriptType + "10", "double");
                    hsDataTypes.Add(scriptType + "11", "datetime");
                    break;
                case "iapd":
                case "py":
                    hsDataTypes.Add("py" + "01", "string");
                    hsDataTypes.Add("py" + "02", "bool");
                    hsDataTypes.Add("py" + "03", "int");
                    hsDataTypes.Add("py" + "04", "long");
                    hsDataTypes.Add("py" + "05", "float");
                    // hsDataTypes.Add("py" + "06", "complex");
                    // hsDataTypes.Add("py" + "07", "unicode");
                    hsDataTypes.Add("py" + "08", "list");
                    hsDataTypes.Add("py" + "09", "tuple");
                    //hsDataTypes.Add("py" + "10", "bytearray");
                    // hsDataTypes.Add("py" + "11", "buffer");
                    //hsDataTypes.Add("py" + "12", "xrange");
                    hsDataTypes.Add("py" + "13", "set");
                    // hsDataTypes.Add("py" + "14", "frozenset");
                    hsDataTypes.Add("py" + "15", "dict");
                    //hsDataTypes.Add("py" + "16", "file");
                    //hsDataTypes.Add("py" + "17", "memoryview");

                    break;
                default:
                    break;
            }

            return hsDataTypes;
        }
    }
}

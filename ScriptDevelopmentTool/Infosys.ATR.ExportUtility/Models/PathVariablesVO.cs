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

namespace Infosys.ATR.ExportUtility.Models
{
    public class PathVariablesVO
    {
        public Dictionary<string, string> pathVariableMap = new Dictionary<string, string>();
        public List<String> serviceAreas;

        public Dictionary<string, string> getPathVariableMap()
        {
            return pathVariableMap;
        }

        public void setPathVariableMap(Dictionary<string, string> pathVariableMap)
        {
            this.pathVariableMap = pathVariableMap;
        }

        public List<String> getServiceAreas()
        {
            return serviceAreas;
        }

        public void setServiceAreas(List<String> serviceAreas)
        {
            this.serviceAreas = serviceAreas;
        }
    }
}

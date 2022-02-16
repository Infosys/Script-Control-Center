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

namespace CodeGenerationEngine.Model
{
    public class LoggingIni : BaseIni
    {
        public string loggingInfo { get; set; }

        public LoggingIni()
        {
            Ini = new List<ConfigInfo>()
            {
                new ConfigInfo(){Attribute="loggers", Parameters=new List<ParamInfo>(){new ParamInfo(){ Name="keys", Value="root,IAP"}}},
                new ConfigInfo(){Attribute="handlers", Parameters=new List<ParamInfo>(){new ParamInfo(){ Name="keys", Value="consoleHandler"}}},
                new ConfigInfo(){Attribute="formatters", Parameters=new List<ParamInfo>(){new ParamInfo(){ Name="keys", Value="simpleFormatter"}}},
                new ConfigInfo(){Attribute="logger_root", Parameters=new List<ParamInfo>(){new ParamInfo(){ Name="level", Value="DEBUG"},new ParamInfo(){ Name="handlers", Value="consoleHandler"}}},
                new ConfigInfo(){Attribute="logger_IAP", Parameters=new List<ParamInfo>(){new ParamInfo(){ Comment="#Change level to INFO in production environment, Set it to DEBUG in development environment", Name="level", Value="DEBUG"},new ParamInfo(){ Name="handlers", Value="consoleHandler"},new ParamInfo(){ Name="qualname", Value="IAP"},new ParamInfo(){ Name="propagate", Value="0"}}},
                new ConfigInfo(){Attribute="handler_consoleHandler", Parameters=new List<ParamInfo>(){new ParamInfo(){ Name="class", Value="StreamHandler"},new ParamInfo(){ Name="level", Value="DEBUG"},new ParamInfo(){ Name="formatter", Value="simpleFormatter"},new ParamInfo(){ Name="args", Value="(sys.stdout,)"}}},
                new ConfigInfo(){Attribute="formatter_simpleFormatter", Parameters=new List<ParamInfo>(){new ParamInfo(){ Name="format", Value="%(asctime)s - %(name)s - %(levelname)s - %(message)s"},new ParamInfo(){ Name="datefmt", Value=""}}}
            };

            loggingInfo = AppIniFiller();
        }

        private string AppIniFiller()
        {
            StringBuilder strBuilder = new StringBuilder();
            foreach (var conInfo in Ini)
            {
                strBuilder.Append(string.Format("\n\n[{0}]", conInfo.Attribute));

                foreach (var param in conInfo.Parameters)
                {
                    if (!string.IsNullOrEmpty(param.Comment))
                        strBuilder.Append(string.Format("\n{0}", param.Comment));

                    strBuilder.Append(string.Format("\n{0} = {1}", param.Name, param.Value));
                }
            }
            return strBuilder.ToString();
        }
    }
}

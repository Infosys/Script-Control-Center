/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerationEngine.Model
{
    public class iap_constants
    {
        public IList<AutomationFacade_Constants> AutoFac_Const { get; set; }
        public IList<AutomationFacade_Logging_Constants> AutoFac_LogConst { get; set; }
        public IList<AutomationFacade_Config_FilePath> ConfigPath { get; set; }

        public iap_constants()
        {
            Initialize();
        }

        public void Initialize()
        { 
                AutoFac_Const =new List<AutomationFacade_Constants>()
                {
                    new AutomationFacade_Constants(){ Name="ATR_PATH", Value=String.Format("Path.Combine(directory, \"App.atr\")")},
                    new AutomationFacade_Constants(){ Name="WAIT_FOREVER", Value="False"},
                    new AutomationFacade_Constants(){ Name="HIGHLIGHT_CONTROL", Value="True"},
                    new AutomationFacade_Constants(){ Name="MULTIPLE_SCALE_TEMPLATE_MATCHING", Value="False"},
                    new AutomationFacade_Constants(){ Name="FIRST_APPLICATION_TO_START", Value=string.Format("\"{0}\"",CodeGenerator.atrModel.AppConfigs.Select(x=>x.AppName).FirstOrDefault().ToString())},
                    new AutomationFacade_Constants(){ Name="SHOW_APP_STARTING_WAIT_BOX", Value="False"},
                    new AutomationFacade_Constants(){ Name="LAUNCH_APP", Value="False"}
                };
                AutoFac_LogConst = new List<AutomationFacade_Logging_Constants>(){

                    new AutomationFacade_Logging_Constants(){ Name="NOTIFICATIONS", Value="Notifications"},
                    new AutomationFacade_Logging_Constants(){ Name="RECIPIENTS", Value="Recipients"},
                    new AutomationFacade_Logging_Constants(){ Name="SMTP_SERVER", Value="SMTP_Server"},
                    new AutomationFacade_Logging_Constants(){ Name="SMTP_PORT", Value="SMTP_Port"},
                    new AutomationFacade_Logging_Constants(){ Name="SMTP_USERNAME", Value="SMTP_UserName"},
                    new AutomationFacade_Logging_Constants(){ Name="SMTP_PASSWORD", Value="SMTP_Pwd"},
                    new AutomationFacade_Logging_Constants(){ Name="RESTAPI", Value="RESTAPI"},
                    new AutomationFacade_Logging_Constants(){ Name="SERVER", Value="Server"},


                    new AutomationFacade_Logging_Constants(){ Name="LOGGER", Value="LOGGER"},
                    new AutomationFacade_Logging_Constants(){ Name="LOGGER_SOURCE", Value="LogRepositoryTarget"},                    
                    new AutomationFacade_Logging_Constants(){ Name="LOGGING_LISTENER", Value="LoggingListener"},
                    
               };
               ConfigPath = new List<AutomationFacade_Config_FilePath>(){ 
                   new AutomationFacade_Config_FilePath(){Name = "CONSOLE_LOG_CONFIG", Value = string.Format("Path.Combine(directory, \"logging.ini\")")},
                   new AutomationFacade_Config_FilePath(){Name = "IRONPYTHON_PATH", Value = string.Format("r\"{0}\"",ConfigurationManager.AppSettings["IronPython"])},
                   new AutomationFacade_Config_FilePath(){Name = "APP_INI_PATH", Value = string.Format("Path.Combine(directory, \"App.ini\")")},
                   new AutomationFacade_Config_FilePath(){ Name="LOG_PATH", Value=string.Format("Path.Combine(Directory.GetParent(directory).FullName, \"Logs\")")}
               };
        }
    }


    public class BaseConst
    {
        public string Name{get;set;}
        public string Value{get;set;} 
    }
    public class AutomationFacade_Constants: BaseConst
    {
    }

     public class  AutomationFacade_Logging_Constants:BaseConst 
    {
        
    }

     public class AutomationFacade_Config_FilePath : BaseConst
     {

     }
}

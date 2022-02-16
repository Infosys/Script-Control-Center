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
    public class AppIni : BaseIni  
    {

        public string AppInfo { get; set;}
        
         public AppIni()
        {
            Ini = new List<ConfigInfo>()
            {  
                new ConfigInfo(){ Attribute="LOGGER", Parameters=new List<ParamInfo>(){ new ParamInfo(){ Comment="###Set 1 for Database logging and 2 for Text File logging", Name ="LogRepositoryTarget", Value="2"},new ParamInfo(){ Comment="###Set LoggingListener to a log file path only if using Text File logging, ignore otherwise", Name="LoggingListener", Value=@"D:\IAP\IAP_log.log"}}},
                new ConfigInfo(){ Attribute="RESTAPI", Parameters=new List<ParamInfo>(){ new ParamInfo(){ Comment="###This is used only when LoggerSource is Dabatbase, ignore otherwise\n###Set this to the server name on which REST services have been deployed", Name ="server", Value="localhost"}}},
                new ConfigInfo(){ Attribute="Notifications", Parameters=new List<ParamInfo>(){ 
                     new ParamInfo(){ Comment="###This is used while sending email notifications\n###Overrite these values with actual values of your SMTP server", Name ="SMTP_Server", Value="127.0.0.1"},new ParamInfo(){ Name="SMTP_Port", Value="25"},
                     new ParamInfo(){ Comment="###OverWrite SMTPUserName with valid value of Email ID account which will be used to send emails", Name="SMTP_UserName", Value="testuser@infy.com"},
                     new ParamInfo(){ Comment="###OverWrite SMTPPwd with password of Email ID account which will be used to send emails", Name ="SMTP_Pwd", Value="AGHFHGFGFGGF"},
                     new ParamInfo(){ Comment="###Overwrite Recipients with valid recipients from the Admin team", Name ="Recipients", Value="testuser1@infy.com,testuser2@infy.com"}}}
            };

            AppInfo = AppIniFiller();
         }

         private string  AppIniFiller()
         {
             StringBuilder strBuilder = new StringBuilder();
             foreach (var conInfo in Ini)
             {
                 strBuilder.Append(string.Format("\n\n[{0}]",conInfo.Attribute));

                 foreach (var param in conInfo.Parameters) 
                 {
                     if (!string.IsNullOrEmpty(param.Comment))
                     strBuilder.Append(string.Format("\n{0}", param.Comment));

                     strBuilder.Append(string.Format("\n{0} : {1}", param.Name,param.Value));
                 }
             }
             return strBuilder.ToString();
         }
    }
}

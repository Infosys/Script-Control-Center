/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.ATR.UIAutomation.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Configuration;


namespace Infosys.ATR.UIAutomation.ATRMapper
{
    public class clsATRmapper
    {
        string _baseImageDir = ConfigurationManager.AppSettings["baseImageDir"];
        string _ProjectMode = "Win32";

        #region Public Methods...
        /// <summary>
        /// Method transforms the list of atrwb source documents string to single atr target document string,
        /// updates the existing atr target document string from the list of atrwb source documents string,
        /// defines name of application node based on input parameter “appClustIdentifier” value,
        /// capable to override the default application project mode.
        /// </summary>
        /// <param name="atrwbSourceDocuments">Represents the list of source atrwb document string.</param>
        /// <param name="atrTargetDocument">(Optional) Represents the target atr document string which is to be updated.</param>
        /// <param name="overrideAppProjectMode">(Optional) Represents the list of “Activity Id"s from atrwb source documents for which project mode is to be overridden.</param>
        /// <param name="appMode">(Optional) Represents the enum which has “Win32” as default value, it can be overridden to “ImageCapture” mode if default project mode of overridden document is “Win32”. Overriding “ImageCapture” to “Win32” mode is not allowed.</param>
        /// <param name="appClustIdentifier">(Optional) Represents the identifier to define the application name from URL in case of web application only, not applicable to WinDesktop application</param>
        /// <returns>returns the .atr document string</returns>
        public string ConvertToATR(List<string> atrwbSourceDocuments, string atrTargetDocument = null, List<string> overrideAppProjectMode = null, ApplicationMode appMode = ApplicationMode.Win32, int appClustIdentifier = 1)
        {
            Validate_atrWbstring(atrwbSourceDocuments);

            //define the local variables 
            string atrFileString = string.Empty;
            UseCase useCase = null;
            AutomationConfig autoConfig = null;            
            List<AppConfig> lstAppConfig = new List<AppConfig>();

            //Get the single atrwb document object model by merging list of source atrwb documents string.
            useCase = MergeUseCases(atrwbSourceDocuments);

            if (useCase != null)
            {
                //Iterate for each activity in object to map the values to respective properties of atr object model.
                foreach (var activity in useCase.Activities)
                {
                    //define the local variables
                    #region local variables                    
                    string _ApplicationLocationPath = string.Empty,
                           _WebBrowser = string.Empty,
                           _WebBrowserVersion = string.Empty,
                           _StringToCampare = string.Empty,
                           _ApplicationName = string.Empty,
                           _ScreenName = "Default_Screen";
                    #endregion

                    //get the application path

                    var application = activity.TargetApplication.TargetApplicationAttributes.Where(t => t.Name == "ApplicationName" && !string.IsNullOrEmpty(t.Value)).SingleOrDefault();
                    if(application!=null)
                        _ApplicationLocationPath = application.Value;

                    // defines the name of Application, Screen, Web browser for the .atr object model.
                    #region Manupulation to define the name of Aplication,Screen,etc.
                    if ((activity.TargetApplication.ApplicationType !=null && activity.TargetApplication.ApplicationType.Equals("WebApplication")) || _ApplicationLocationPath.ToLower().StartsWith("http"))
                    {

                        //set the application name based on value of "appClustIdentifier" input variable
                        if (_ApplicationLocationPath.Split(new char[] { '/' }).Length > (appClustIdentifier + 2))
                        {
                            int index = _ApplicationLocationPath.IndexOf(_ApplicationLocationPath.Split(new char[] { '/' })[appClustIdentifier + 1]);
                            _ApplicationName = _ApplicationLocationPath.Substring(0, index).Replace("://", "_")
                                                                                           .Replace("/", "_")
                                                                                           .Replace(".", "_")
                                                + "_" + _ApplicationLocationPath.Split(new char[] { '/' })[appClustIdentifier + 1];
                        }
                        else
                            _ApplicationName = _ApplicationLocationPath.Replace("://", "_")
                                                                       .Replace("/", "_")
                                                                       .Replace(".", "_");

                        //define the value of _StringToCampare variable to compare and consolidate the application based on value of "appClustIdentifier" input variable. 
                        if (_ApplicationLocationPath.Split(new char[] { '/' }).Length > (appClustIdentifier + 2))
                        {
                            if (appClustIdentifier.Equals(1))
                            {
                                int index = _ApplicationLocationPath.IndexOf(_ApplicationLocationPath.Split(new char[] { '/' })[appClustIdentifier + 1]);
                                _StringToCampare = _ApplicationLocationPath.Substring(0, index) + _ApplicationLocationPath.Split(new char[] { '/' })[appClustIdentifier + 1];
                            }
                            else
                            {
                                int index = _ApplicationLocationPath.IndexOf(_ApplicationLocationPath.Split(new char[] { '/' })[appClustIdentifier + 2]);
                                if (index >= 0)
                                    _StringToCampare = _ApplicationLocationPath.Substring(0, index);
                            }
                        }

                        //if the value of _StringToCampare variable is null of empty then set the application path as value
                        if (string.IsNullOrEmpty(_StringToCampare))
                            _StringToCampare = _ApplicationLocationPath;

                        //define the screen name for respective application based on value of "appClustIdentifier" input variable

                        if (_StringToCampare.Equals(_ApplicationLocationPath))
                            _ScreenName = _ApplicationName;
                        else
                            _ScreenName = _ApplicationLocationPath.Split(new char[] { '/' }).Length > appClustIdentifier + 2 ? _ApplicationLocationPath.Substring(_StringToCampare.Length, _ApplicationLocationPath.Length - _StringToCampare.Length).Replace('/', '_') : "";

                        _ScreenName = _ScreenName.Equals("_") ? _ApplicationName : _ScreenName;

                        int i = appClustIdentifier + 2;
                        while (string.IsNullOrEmpty(_ScreenName))
                        {
                            i--;
                            _ScreenName = _ApplicationLocationPath.Split(new char[] { '/' }).Length > i ? _ApplicationLocationPath.Split(new char[] { '/' })[i] : "";
                        }


                        _ScreenName = Convert.ToString(_ScreenName.Split(new char[] { '?' }).GetValue(0));

                        //if still application name variable is empty OR null then assign the name of screen to it. 

                        if (string.IsNullOrEmpty(_ApplicationName))
                            _ApplicationName = _ScreenName;

                        //get the name of web broweser
                        _WebBrowser = activity.TargetApplication.TargetApplicationAttributes.SingleOrDefault(a => a.Name.Equals("ModuleName")).Value.ToString();

                        switch (_WebBrowser.ToLower())
                        {
                            case "iexplore.exe":
                                _WebBrowser = "Internet Explorer";
                                break;
                            case "chrome.exe":
                                _WebBrowser = "Chrome";
                                break;
                            case "firefox.exe":
                                _WebBrowser = "Firefox";
                                break;
                            default:
                                _WebBrowser = "Internet Explorer";
                                break;
                        }
                    }
                    else if (activity.TargetApplication.ApplicationType !=null && activity.TargetApplication.ApplicationType.Equals("Win32"))
                    {
                        //get the name of Application, Screen and  value of _StringToCampare variable for Windesktop application
                        _ApplicationName = _ApplicationLocationPath.Replace(".exe", "");
                        _ScreenName = string.Format("{0}_Screen", _ApplicationName);
                        _StringToCampare = _ApplicationName;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(_ApplicationName))
                            _ApplicationName = _ApplicationLocationPath;
                        if (!string.IsNullOrEmpty(_ApplicationName))
                        {
                            _ScreenName = string.Format("{0}_Screen", _ApplicationName);
                            _StringToCampare = _ApplicationName;
                        }
                    }
                    #endregion

                    //Initialize the ScreenConfig and EntityConfig objects to binds the controls of interest under the respective Screen of application.
                    #region Adding Controls, Screens, Application...
                    foreach (var task in activity.Tasks)
                    {
                        //iIgnore the keyboard key press events
                        //if (!task.Event.ToString().ToLower().Equals("keyboardkeypress") && !string.IsNullOrEmpty(task.ControlName))
                        if (!task.Event.ToString().ToLower().Equals("keyboardkeypress"))
                        {
                            //if application type is Windesktop application then consider the value of “WindowTitle” attribute of Task as Screen Name
                            if (!activity.TargetApplication.ApplicationType.Equals("WebApplication",StringComparison.InvariantCultureIgnoreCase) && task.WindowTitle != "")
                                _ScreenName = task.WindowTitle;

                            if (overrideAppProjectMode == null)
                                overrideAppProjectMode = new List<string>();

                            string entity_name = string.Empty;                           

                            if(!string.IsNullOrEmpty(task.ControlType) && !string.IsNullOrEmpty(task.ControlName))
                                entity_name = System.Text.RegularExpressions.Regex.Replace(
                                    string.Format("{0}{1}{2}", task.ControlType.Replace("ControlType.", ""), task.ControlName, task.Order), @"[^0-9a-zA-Z]+", "_");
                            else if (!string.IsNullOrEmpty(task.ControlType) && !string.IsNullOrEmpty(task.ControlId))
                                entity_name = System.Text.RegularExpressions.Regex.Replace(
                                    string.Format("{0}{1}{2}", task.ControlType.Replace("ControlType.", ""), task.ControlId, task.Order), @"[^0-9a-zA-Z]+", "_");
                            else if (!string.IsNullOrEmpty(task.ControlType))
                                entity_name = System.Text.RegularExpressions.Regex.Replace(
                                    string.Format("{0}{1}", task.ControlType.Replace("ControlType.", ""), task.Order), @"[^0-9a-zA-Z]+", "_");
                            else
                                entity_name = System.Text.RegularExpressions.Regex.Replace(
                                    string.Format("{0}{1}", "Control_", task.Order), @"[^0-9a-zA-Z]+", "_");

                            //initialize the EntityConfig object to set the properties of control
                            //apply the rule: if overrideAppProjectMode input variable contains and matches to Activity Id in loop and input application mode is ImageCapture then skip the Win32 based control properties and keep Image based control properties, otherwise ignore.

                            EntityConfig entity = new EntityConfig()
                            {
                                EntityName = entity_name,//task.ControlName,
                                Parent = "Screen",
                                EntityControlConfig = (overrideAppProjectMode.Contains(activity.Id) && appMode.Equals(ApplicationMode.ImageCapture)) ?
                                new ControlConfig()
                                {
                                    ControlName = string.Empty,
                                    ControlClass = task.ControlType.Replace("ControlType.", ""),
                                    ControlPath = string.Empty
                                } : new ControlConfig()
                                {
                                    ControlName = task.ControlName,
                                    AutomationId = task.ControlId,
                                    ControlClass = task.ControlType.Replace("ControlType.", ""),
                                    ControlPath = task.ApplictionTreePath
                                },
                                EntityImageConfig = new ImageConfig()
                                {
                                    StateImageConfig = new List<StateImageConfig>() 
                                    { 
                                        new StateImageConfig()      
                                        {   
                                            State="",
                                            CenterImageName= new CenterImageSearchConfig(){ ImageName=string.Format("{0}.jpg",task.Id)}
                                        }
                                    }
                                }
                            };

                            _ScreenName = _ScreenName.Replace("__", "_").Replace("https_", "").Replace("http_", "").Replace(".", "_");
                            _ScreenName=_ScreenName.StartsWith("_")?_ScreenName.Substring(1,_ScreenName.Length-1):_ScreenName;
                            _ScreenName = _ScreenName.EndsWith("_") ? _ScreenName.Substring(0, _ScreenName.Length - 1) : _ScreenName;
                            _ScreenName = System.Text.RegularExpressions.Regex.Replace(_ScreenName, @"[^0-9a-zA-Z]+", "_");

                            //apply the rule: Check whether the defined application is exists in list of applications, if Yes, then append the defined Screen entity under that existing application otherwise add new application to list.
                            //apply the rule: Check whether the defined screen is exists in list of Screens correspond to respective application, if Yes, then append the control  entity under that existing Screen otherwise add new Screen.

                            AppConfig AppInPool = lstAppConfig.Where(x => x.AppControlConfig.ApplicationLocationPath.Replace("https", "http").StartsWith(_StringToCampare.Replace("https", "http"), StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                            if (AppInPool != null)
                            {
                                

                                ScreenConfig screen = AppInPool.ScreenConfigs.FindAll(x => x.ScreenName.ToUpper().Equals(_ScreenName.ToUpper())).FirstOrDefault();
                                if (screen != null)
                                {
                                    var _entity = screen.EntityConfigs.FindAll(x => x.EntityName.ToUpper().Equals(entity.EntityName.ToUpper())).FirstOrDefault();
                                    if (_entity==null)
                                        screen.EntityConfigs.Add(entity);
                                }
                                else
                                {
                                    AppInPool.ScreenConfigs.Add(new ScreenConfig()
                                    {
                                        ScreenName = _ScreenName,
                                        EntityConfigs = new List<EntityConfig>() { entity },
                                        ScreenImageConfig = new ImageConfig()
                                        {
                                            StateImageConfig = new List<StateImageConfig> { 

                                                new StateImageConfig()      
                                                {   
                                                    State="",
                                                    CenterImageName= new CenterImageSearchConfig(){ ImageName=string.Format("{0}.jpg",task.ScreenId)}
                                                }

                                            }
                                        },
                                        ScreenControlConfig = new ControlConfig() { ControlName = "" }
                                    });
                                }
                            }
                            else
                            {
                                _ApplicationName = System.Text.RegularExpressions.Regex.Replace(_ApplicationName, @"[^0-9a-zA-Z]+", "_");

                                lstAppConfig.Add(new AppConfig()
                                {   
                                    AppName = _ApplicationName.Replace("__", "_").Replace("https_", "").Replace("http_", ""),
                                    BaseImageDir = _baseImageDir,
                                    AppControlConfig = new ControlConfig()
                                    {
                                        ApplicationType = activity.TargetApplication.ApplicationType.Equals("WebApplication") ? "Web" : "WinDesktop",
                                        ApplicationLocationPath = _ApplicationLocationPath,
                                        WebBrowser = _WebBrowser,
                                        WebBrowserVersion = _WebBrowserVersion
                                    },
                                    ScreenConfigs = new List<ScreenConfig>(){new ScreenConfig()
                                    {
                                        ScreenName = _ScreenName,
                                        EntityConfigs = new List<EntityConfig>() { entity },
                                        ScreenImageConfig = new ImageConfig()
                                        {
                                            StateImageConfig = new List<StateImageConfig> { 

                                                new StateImageConfig()      
                                                {   
                                                    State="",
                                                    CenterImageName= new CenterImageSearchConfig(){ ImageName=string.Format("{0}.jpg",task.ScreenId)}
                                                }

                                            }
                                        },
                                        ScreenControlConfig = new ControlConfig() { ControlName = "" }
                                    }},
                                    AppImageConfig = new ImageConfig() { }
                                });
                            }
                        }
                    }
                    #endregion
                }
                // generate the atr document object model. 
                autoConfig = new AutomationConfig() { AppConfigs = lstAppConfig, ProjectMode = _ProjectMode };
            }

            //apply rule: if atrTargetDocument input variable is not null and empty then validate and update the atr targeted document string.
            if (!string.IsNullOrEmpty(atrTargetDocument))
                autoConfig = UpdateAtr(autoConfig, atrTargetDocument);

            //Serialize the atr document object model to string
            if (autoConfig != null)
                atrFileString = Utilities.Serialize<AutomationConfig>(autoConfig);

            //returns the atr document string
            return atrFileString;
        }
        /// <summary>
        /// Method transforms the list of source atrwb document string to the list of applist object.   
        /// </summary>
        /// <param name="atrwbSourceDocuments">Represents the list of source .atrwb document string.</param>
        /// <returns>returns the list of Applist object</returns>
        public List<Applist> GetAppList(List<string> atrwbSourceDocuments)
        {
            List<Applist> applist = new List<Applist>();
            List<UseCase> lstUseCase = GetUseCaseList(atrwbSourceDocuments);
            if (lstUseCase != null)
            {
                foreach (UseCase useCase in lstUseCase)
                {
                    foreach (Activity activity in useCase.Activities)
                    {
                        applist.Add(new Applist()
                        {
                            ActivityGUID = activity.Id,
                            AppName = activity.TargetApplication.TargetApplicationAttributes.SingleOrDefault(x => x.Name.Equals("ApplicationName")).Value.ToString(),
                            ApplicationExe = activity.TargetApplication.ApplicationExe.ToString()
                        });
                    }
                }
            }
            return applist;
        }
        #endregion
        #region Private Methods...
        /// <summary>
        /// Method transforms the list of atrwb document string to atrwb object model.
        /// </summary>
        /// <param name="atrwbSourceDocuments">Represents the list of source .atrwb document string.</param>
        /// <returns>returns the atrwb object model i.e UseCase object</returns>
        private UseCase MergeUseCases(List<string> atrwbSourceDocuments)
        {
            //Get the list of UseCase i.e. atrwb object model   
            List<UseCase> u1List = GetUseCaseList(atrwbSourceDocuments);
            UseCase finalobj = new UseCase();

            //Iterate for each Usecase object in list to merge .
            foreach (UseCase useCase in u1List)
            {
                //apply the rule: Check whether Activity exists in local UseCase object i.e. finalobj. If NOT, then assign first UseCase object in list to finalobj.
                if (finalobj.Activities != null)
                {
                    foreach (Activity activity in useCase.Activities)
                    {
                        //apply the rule: Check whether the Activity in the iterated object is exists in finalobj, if NOT, then add Activity node to finalobj object.
                        Activity actInstance = finalobj.Activities.SingleOrDefault(x => x.TargetApplication.TargetApplicationAttributes.SingleOrDefault(a => a.Name.Equals("ApplicationName")).Value.ToString().Equals(activity.TargetApplication.TargetApplicationAttributes.SingleOrDefault(a => a.Name.Equals("ApplicationName")).Value));
                        if (actInstance != null)
                        {
                            //apply the rule: if Yes, then check whether tasks under iterated Activity are matched with Task under existing Activity.If NOT, then append the tasks under the existing Activity, otherwise ignore it.
                            foreach (Task task in activity.Tasks)
                            {
                                Task taskInstance = actInstance.Tasks.SingleOrDefault(x => (
                                    x.ControlName.Equals(task.ControlName)
                                    && x.ControlId.Equals(task.ControlId)
                                    && x.ApplictionTreePath.Equals(task.ApplictionTreePath)
                                    && x.ControlType.Equals(task.ControlType)
                                    && x.WindowTitle.Equals(task.WindowTitle)
                                    ));

                                if (taskInstance == null)
                                    actInstance.Tasks.Add(task);
                            }
                        }
                        else
                            finalobj.Activities.Add(activity);
                    }
                }
                else
                    finalobj = useCase;
            }
            return finalobj;
        }
        /// <summary>
        /// Method transforms the list of atrwb document string to list of atrwb object model.
        /// </summary>
        /// <param name="atrwbSourceDocuments">Represents the list of source .atrwb document string.</param>
        /// <returns>returns the list of .atrwb object models i.e. list of UseCase object</returns>
        private List<UseCase> GetUseCaseList(List<string> atrwbSourceDocuments)
        {
            List<UseCase> lstUseCase = new List<UseCase>();
            Validate_atrWbstring(atrwbSourceDocuments);
            foreach (string atrwb in atrwbSourceDocuments)
                if (!string.IsNullOrEmpty(atrwb))
                {
                    lstUseCase.Add(Utilities.Deserialize<UseCase>(atrwb));
                }
            return lstUseCase;
        }
        /// <summary>
        /// Method updates the atr target document string from generated atr object model.
        /// </summary>
        /// <param name="autoConfig">Represents the generated atr object model i.e. AutomationConfig object</param>
        /// <param name="atrTargetDocument">Represents the atr targeted document string which is to be updated</param>
        /// <returns>returns the updated atr object model i.e. AutomationConfig object</returns>
        private AutomationConfig UpdateAtr(AutomationConfig autoConfig, string atrTargetDocument)
        {
            //deserialize atr targeted document string to atr object model i.e. AutomationConfig object
            AutomationConfig autoConfigToUpdate = Utilities.Deserialize<AutomationConfig>(atrTargetDocument);

            foreach (AppConfig app in autoConfig.AppConfigs)
            {
                //apply the rule: Check whether the iterated application exists in list of application, if NOT, then append the application object to updating atr object model.
                AppConfig appPool = autoConfigToUpdate.AppConfigs.Where(x => x.AppName.Equals(app.AppName)).FirstOrDefault();
                if (appPool != null)
                {
                    foreach (ScreenConfig screen in app.ScreenConfigs)
                    {
                        //apply the rule: Check whether the iterated Screen exists in list of Screen, if NOT, then append the screen to updating atr object model under existing application.
                        ScreenConfig screenPool = appPool.ScreenConfigs.Where(x => x.ScreenName.Equals(screen.ScreenName)).FirstOrDefault();
                        if (screenPool != null)
                        {
                            foreach (EntityConfig entity in screen.EntityConfigs)
                            {
                                //apply the rule: Check whether the iterated control entity exists in list of control, if NOT, then append the control to updating atr object model under existing screen.
                                EntityConfig entityPool = screenPool.EntityConfigs.Where(x => (
                                    x.EntityControlConfig.ControlName.Equals(entity.EntityControlConfig.ControlName) &&
                                    x.EntityControlConfig.AutomationId.Equals(entity.EntityControlConfig.AutomationId) &&
                                    x.EntityControlConfig.ControlClass.Equals(entity.EntityControlConfig.ControlClass) &&
                                    x.EntityControlConfig.ControlPath.Equals(entity.EntityControlConfig.ControlPath)
                                    )).FirstOrDefault();

                                if (entityPool == null)
                                    screenPool.EntityConfigs.Add(entity);
                            }
                        }
                        else
                            appPool.ScreenConfigs.Add(screen);
                    }
                }
                else
                    autoConfigToUpdate.AppConfigs.Add(app);
            }
            return autoConfigToUpdate;
        }
        #endregion

        private void Validate_atrWbstring(List<string> atrwbList)
        {
            if (atrwbList == null)
                throw new Exception("Input atrwb document string is null. It should not be empty or null....!!!");
            else
            {
                List<UseCase> useCase = null;
                int i = 0;
                foreach (string strInput in atrwbList)
                {
                    try
                    {
                        useCase = new List<UseCase>() {Utilities.Deserialize<UseCase>(strInput)};
                        i++;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Input atrwb document string (Item Number:{0}) is not in proper format...!!!", i++));
                    }
                }
            }
        }
    }


    /// <summary>
    /// Class to store infomartion regarding application 
    /// </summary>
    public class Applist
    {
        public string AppName { get; set; }
        public string ActivityGUID { get; set; }
        public string ApplicationExe { get; set; }
    }

    /// <summary>
    /// enum for Project Mode
    /// </summary>
    public enum ApplicationMode
    {
        Win32,
        ImageCapture
    }
}

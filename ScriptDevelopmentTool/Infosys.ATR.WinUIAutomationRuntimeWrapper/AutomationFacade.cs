/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Infosys.ATR.UIAutomation.Entities;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Diagnostics;
using Infosys.WEM.Infrastructure.Common;
using System.Windows.Automation;
using System.Reflection;
using System.Runtime.InteropServices;
using Infosys.ATR.OCRWrapper;
using System.Net;
using System.Windows.Forms;
using System.Threading;
using System.Xml.Linq;
using Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions;
using System.Configuration;

namespace Infosys.ATR.WinUIAutomationRuntimeWrapper
{
    public class AutomationFacade
    {
        //below two variables are to be used to do the re-try operation in case the intended control is not available
        //this will be primarily useful when the control takes some considerable time to get visible/accessible
        //e.g. when application takes a long time to initialize
        int numOfTrials = 3;
        int timeGapBetweenTrials = 300; //in ms

        bool highlightElement = false; //to be used to highlight the matched region in case of image based recognition
        int timeoutInSec = 10;//the time for the image matching to be tried for control identification
        bool templateMachingInOriginalScale = false; //to be used to tell in case of image based identification, if the maching to be done in original scale only
        bool waitForeverForTemplate = false; //to be used to tell if to wait idefinitely for the match to be found
        bool showWaitBox = true; //to be used to dictate if the 'application starting wait box' is to be shown or not
        bool useTrueColorTemplateMatching = false; //to be used in image based approach to dictate if the True color or gray scale image matching is to be followed
        string firstAppToStart = "";
        bool getAllMatchingControls = false;
        private string className = "AutomationFacade";
        Control parentControl = null;//to be used in case of restriction based search for image template
        string iapPackageTypes = ".iapw,.iapd"; //comma separated without any spaces
        private int InteractiveCheckExists = -1;
        bool findControlInMultipleControlStates = false; //used to find the controls across all the different states defined within a control. This option will not break after the first control state has been identified
        int imageMatchConfidenceThreshold = 80; //used to set the image recognition template match confidence score, any value below which will not be accepted as a match. The threshold should be in range 1-100
        Stream pSourceImage = null; //Source Image Control object to match if image recognition is to be done in background mode
        //events for client developers
        #region Event- FocusHasChanged
        public class FocusHasChangedArgs : EventArgs
        {
            public Control Control { get; set; }
        }
        public delegate void FocusHasChangedEventHandler(FocusHasChangedArgs e);
        private event FocusHasChangedEventHandler FocusHasChanged;
        #endregion


        /// <summary>
        /// The location where the ATR data file resides. If  it has value, to be used as the relative path for the template images.
        /// </summary>
        public string ATRFileDirectory { get; set; }

        //TODO:Rahul Read XML, check if app to be launched, launch, load App object
        public AutomationFacade(System.Xml.XmlDocument automationConfigXML, bool LaunchApps, bool showAppStartingWaitBox = true, string firstApplicationToStart = "", bool highLightControl = false, bool multipleScaleTemplateMatching = true, bool waitForeverForImageTemplate = false)
        {
            Core.Utilities.SetDLLsPath();
            highlightElement = highLightControl;
            templateMachingInOriginalScale = !multipleScaleTemplateMatching;
            waitForeverForTemplate = waitForeverForImageTemplate;
            showWaitBox = showAppStartingWaitBox;
            firstAppToStart = firstApplicationToStart;

            //var path = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            //path = System.IO.Path.Combine(path, IntPtr.Size == 8 ? "x64" : "x86");
            //bool ok = SetDllDirectory(path);
            //if (!ok) throw new System.ComponentModel.Win32Exception();

            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.AUTOMATIONFACADE))
            {

                //in param
                Core.Utilities.ClearCache();
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "automationConfigXML", Logging.Constants.PARAMDIRECTION_IN, automationConfigXML.Name);

                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "LaunchApps", Logging.Constants.PARAMDIRECTION_IN, LaunchApps.ToString());

                PopulateRetrySettings();
                if (automationConfigXML != null)
                {
                    string xmlString = Core.Translation.XMLDocAsString(automationConfigXML);
                    if (!string.IsNullOrEmpty(xmlString))
                        TranslateAutomationConfig(xmlString, LaunchApps);
                }
            }
            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.AUTOMATIONFACADE);
        }
        //TODO:Rahul Read XML, check if app to be launched, launch,  load App object
        public AutomationFacade(string xMLPath, bool LaunchApps, bool showAppStartingWaitBox = true, string firstApplicationToStart = "", bool highLightControl = false, bool multipleScaleTemplateMatching = true, bool waitForeverForImageTemplate = false)
        {
            Core.Utilities.SetDLLsPath();
            highlightElement = highLightControl;
            templateMachingInOriginalScale = !multipleScaleTemplateMatching;
            waitForeverForTemplate = waitForeverForImageTemplate;
            showWaitBox = showAppStartingWaitBox;
            firstAppToStart = firstApplicationToStart;
            string typeOfIAPPackage = "";
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.AUTOMATIONFACADE))
            {
                //in param
                Core.Utilities.ClearCache();
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "xMLPath", Logging.Constants.PARAMDIRECTION_IN, xMLPath);

                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "LaunchApps", Logging.Constants.PARAMDIRECTION_IN, LaunchApps.ToString());

                PopulateRetrySettings();

                bool xmlpathFound = false;
                string xmlString = "";
                if (IsValidUrlFormat(xMLPath.Replace("$", "dollar")))//i.e. iapw fo the workflow http store. this replace is needed as $ is not a valid charcter in a url
                {
                    //get all the iap package types:
                    string[] iappackages = iapPackageTypes.Split(',');
                    //check url is to be fetched from TLS
                    if (xMLPath.ToLower().Contains("http://$") || xMLPath.ToLower().Contains("https://$"))
                    {
                        LocalDataStoreSlot localData = System.Threading.Thread.GetNamedDataSlot("iappackageurl");
                        string iappackageurl = System.Threading.Thread.GetData(localData).ToString();
                        xMLPath = xMLPath.Replace("http://$", iappackageurl).Replace("https://$", iappackageurl);
                    }

                    xmlpathFound = true;
                    //get the .iap package part
                    string iappackageLoc = "";
                    foreach (string iappackage in iappackages)
                    {
                        if (xMLPath.Contains(iappackage))
                        {
                            iappackageLoc = xMLPath.Substring(0, xMLPath.IndexOf(iappackage) + iappackage.Length);
                            break;
                        }
                    }
                    string atrLoc = xMLPath.Replace(iappackageLoc, "").Replace(@"/", "\\");

                    //load the .iapw and extract the atr file from the iapw stream
                    WebClient storeclient = new WebClient();
                    CredentialCache credential = new CredentialCache();
                    credential.Add(new Uri(iappackageLoc), "NTLM", CredentialCache.DefaultNetworkCredentials);
                    storeclient.Credentials = credential;
                    Stream iapwcontent = storeclient.OpenRead(iappackageLoc);

                    //since this  iapwcontent is a stream over http, we wont be able to read the files from within the stream
                    //hence we need to first converft it to memory stream 
                    //also decrypt the stream to be used ahead
                    Stream iapmemoryStream = new Infosys.ATR.RepositoryAccess.FileRepository.WorkflowRepositoryDS().GetUnSecureStream(iapwcontent);

                    //the below is not needed as the above GetUnSecureStream method converts the stream to memeory stream
                    //using (MemoryStream memoryStream = new MemoryStream())
                    //{
                    //    int blkcount = 0;
                    //    do
                    //    {
                    //        byte[] buffer = new byte[1024];
                    //        blkcount = iapwcontent.Read(buffer, 0, 1024);
                    //        memoryStream.Write(buffer, 0, blkcount);
                    //    } while (iapwcontent.CanRead && blkcount > 0);
                    //    iapmemoryStream = memoryStream;
                    //}

                    //assign it to the Utilities to be used in the fuinctions there
                    Core.Utilities.IapwPackage = iapmemoryStream;

                    ATRFileDirectory = System.IO.Path.GetDirectoryName(atrLoc).Substring(1);//to exclude the starting slash
                    if (iapmemoryStream != null && iapmemoryStream.Length > 0)
                    {
                        Stream atr = Infosys.ATR.Packaging.Operations.ExtractFile(iapmemoryStream, atrLoc);
                        xmlString = StreamToString(atr);
                        Infosys.ATR.Packaging.Operations.ClosePackage();
                    }
                }
                else if (!string.IsNullOrEmpty(xMLPath) && !string.IsNullOrEmpty(typeOfIAPPackage = GetValidPackage(xMLPath)))// xMLPath.Contains(".iapw")) //i.e. local iapw
                {
                    xmlpathFound = true;
                    //get the package part e.g. .iapw
                    string iapPackageLoc = xMLPath.Substring(0, xMLPath.IndexOf(typeOfIAPPackage) + typeOfIAPPackage.Length);
                    string atrLoc = xMLPath.Replace(xMLPath, iapPackageLoc).Replace(@"/", "\\");

                    //load the local iapw package
                    using (FileStream fileStream = new FileStream(iapPackageLoc, FileMode.Open, FileAccess.Read))
                    {
                        //assign it to the Utilities to be used in the fuinctions there
                        Core.Utilities.IapwPackage = fileStream;

                        ATRFileDirectory = System.IO.Path.GetDirectoryName(atrLoc).Substring(1);//to exclude the starting slash;
                        if (fileStream != null && fileStream.Length > 0)
                        {
                            Stream atr = Infosys.ATR.Packaging.Operations.ExtractFile(fileStream, atrLoc);
                            xmlString = StreamToString(atr);
                            Infosys.ATR.Packaging.Operations.ClosePackage();
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(xMLPath) && System.IO.File.Exists(xMLPath))
                {
                    xmlpathFound = true;
                }
                else if (!string.IsNullOrEmpty(xMLPath))
                {
                    //try for the relative path
                    xMLPath = GetProbableAbsolutePath(xMLPath);
                    if (System.IO.File.Exists(xMLPath))
                        xmlpathFound = true;
                }

                if (xmlpathFound && string.IsNullOrEmpty(xmlString))
                {
                    //read the folder containing the atr file
                    ATRFileDirectory = System.IO.Path.GetDirectoryName(xMLPath);
                    xmlString = System.IO.File.ReadAllText(xMLPath);
                }

                if (!string.IsNullOrEmpty(xmlString))
                {
                    Core.Utilities.WriteLog("automation initialization started at " + DateTime.Now.ToString());
                    TranslateAutomationConfig(xmlString, LaunchApps);
                    Core.Utilities.WriteLog("automation initialization completed at " + DateTime.Now.ToString());
                }
                else
                {
                    throw new System.Exception("Path provided for Automation Config file does not exists");
                }
            }
            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.AUTOMATIONFACADE);
        }
        /// <summary>
        /// The constructor to be used if control identification is not to be done using canonical path.
        /// </summary>
        public AutomationFacade()
        {
            Core.Utilities.SetDLLsPath();
        }

        private Dictionary<string, Application> applications;

        public Dictionary<string, Application> Applications
        {
            get { return applications; }
            set { applications = value; }
        }

        /// <summary>
        /// This method is used to find control based on CanonicalPath, automationId & automationName.
        /// </summary>
        /// <param name="canonicalPath">CanonicalPath</param>
        /// <param name="automationId">automationId</param>
        /// <param name="automationName">automationName</param>
        /// <param name="sourceImage">Optional - Source Image in which the element is to be located. 
        /// To be specified if the control recognition is to be handled in the background processing mode only.Applicable only in ImageMode.</param>
        /// <returns>Control</returns>
        public Control FindControl(string canonicalPath, string automationId, string automationName, Stream sourceImage = null)
        {
            getAllMatchingControls = false;
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.FINDCONTROL))
            {
                //in param
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "canonicalPath", Logging.Constants.PARAMDIRECTION_IN, canonicalPath);
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "automationId", Logging.Constants.PARAMDIRECTION_IN, automationId);
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "automationName", Logging.Constants.PARAMDIRECTION_IN, automationName);

                Control ctl = null;
                Application app = null;
                Screen screen = null;
                if (Applications != null)
                {
                    string[] canonicalPathParts = canonicalPath.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    if (canonicalPathParts != null && canonicalPathParts.Length >= 2) //atleast the application part and the concerned control. as per new requirement, there may not be any screen part mentioned
                    {
                        LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_VARIABLE_VALUE, LogHandler.Layer.Business, "canonicalPathParts", canonicalPathParts.Length.ToString());
                        bool stopTraversing = false;
                        for (int i = 0; i < canonicalPathParts.Length; i++)
                        {
                            if (stopTraversing)
                                break;
                            switch (i)
                            {
                                case 0: //application
                                    app = Applications[canonicalPathParts[i]];
                                    break;
                                case 1: //screen
                                    if (app == null)
                                        stopTraversing = true;
                                    else if (app.Screens != null && app.Screens.ContainsKey(canonicalPathParts[i]))
                                    {
                                        screen = app.Screens[canonicalPathParts[i]];
                                        if (!string.IsNullOrEmpty(screen.AutomationName) && screen.AutomationName.Trim() != "")
                                            screen.RefreshScreenHandle();
                                    }
                                    else //i.e. no screen name provided, then the control details wud be directly under the application
                                    {
                                        if (ctl == null) //i.e. first level of control under the screen
                                        {
                                            ctl = app.Controls[canonicalPathParts[i]];
                                            ctl.GetAllMatchingControls = false;
                                            if (WaitForever)
                                            {
                                                ctl.DoEntityCaching = false;
                                                ctl.DoTabTracking = false;
                                            }
                                            if (i == canonicalPathParts.Length - 1)
                                            {
                                                ctl.AutomationId = automationId;
                                                ctl.AutomationName = automationName;
                                                ctl.UpdateCondition(automationId, automationName);
                                            }
                                            ctl.RefreshControlHandle(app.WindowsHandle, IntPtr.Zero);
                                        }
                                        else //i.e. for controls under control
                                        {
                                            ctl = ctl.Controls[canonicalPathParts[i]];
                                            ctl.GetAllMatchingControls = false;
                                            if (WaitForever)
                                            {
                                                ctl.DoEntityCaching = false;
                                                ctl.DoTabTracking = false;
                                            }
                                            if (i == canonicalPathParts.Length - 1)
                                            {
                                                ctl.AutomationId = automationId;
                                                ctl.AutomationName = automationName;
                                                ctl.UpdateCondition(automationId, automationName);
                                            }
                                            //if (!ctl.IsControlAvailable())
                                            //{
                                            //    ctl.RefreshControlHandle(app.WindowsHandle, IntPtr.Zero);
                                            //}
                                            ctl.RefreshControlHandle(app.WindowsHandle, IntPtr.Zero);
                                        }
                                    }
                                    break;
                                default: //control and also to consider any level under control
                                    //if (screen == null)      //need not to be check because as per new requirement, screen name may not be provided
                                    //    stopTraversing = true;
                                    //else
                                    if (ctl == null) //i.e. first level of control under the screen
                                    {
                                        ctl = screen.Controls[canonicalPathParts[i]];
                                        ctl.GetAllMatchingControls = false;
                                        if (WaitForever)
                                        {
                                            ctl.DoEntityCaching = false;
                                            ctl.DoTabTracking = false;
                                        }
                                        if (i == canonicalPathParts.Length - 1)
                                        {
                                            ctl.AutomationId = automationId;
                                            ctl.AutomationName = automationName;
                                            ctl.UpdateCondition(automationId, automationName);
                                        }
                                        ctl.RefreshControlHandle(app.WindowsHandle, screen.WindowsHandle);
                                    }
                                    else //i.e. for controls under control
                                    {
                                        ctl = ctl.Controls[canonicalPathParts[i]];
                                        ctl.GetAllMatchingControls = false;
                                        if (WaitForever)
                                        {
                                            ctl.DoEntityCaching = false;
                                            ctl.DoTabTracking = false;
                                        }
                                        if (i == canonicalPathParts.Length - 1)
                                        {
                                            ctl.AutomationId = automationId;
                                            ctl.AutomationName = automationName;
                                            ctl.UpdateCondition(automationId, automationName);
                                        }
                                        //if (!ctl.IsControlAvailable())
                                        //{
                                        //    ctl.RefreshControlHandle(app.WindowsHandle, screen.WindowsHandle);
                                        //}
                                        ctl.RefreshControlHandle(app.WindowsHandle, screen.WindowsHandle);
                                    }
                                    break;
                            }
                        }
                    }
                }
                //set source image stream so that the refreshcontrol can refer to the stream object

                //TODO:Rahul Return Control Instance from the Automation object
                //Check if the control being accessed is active (check property Control.isControlAvailable), if not than refresh the sceen. if appwinhandle is not available than refresh applevel object
                if (ctl != null && !ctl.IsControlAvailable() && app != null && screen != null)
                {
                    try
                    {
                        pSourceImage = sourceImage;
                        ctl = RefreshControl(app, screen, ctl);
                    }
                    finally
                    {
                        //clean the internal source image stream
                        pSourceImage = null;
                    }
                }

                //highlight the UI of the automation element, if any
                if (highlightElement && ctl != null)
                {
                    ctl.Highlight();
                }

                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.FINDCONTROL);

                return ctl;
            }
        }

        /// <summary>
        ///  Applicable for win32 based approach. Interface to find a control baes on the passed coordinates. If no coordinates passed then the current 
        /// mouse position as the intended coordinates.
        /// N.B. if passed, both the coordinates i.e. x and y are needed to be passed. If only one is passed then it will be ignored.
        /// </summary>
        /// <param name="x">the optional x coordinate</param>
        /// <param name="y">the optional y coordinate</param>
        /// <returns>identified control at the intended coordinates</returns>
        public Control FindControl(int? x, int? y)
        {
            Control ctl = null;
            if (x == null || y == null)
            {
                //get the x and y from the current mouse position
                x = Cursor.Position.X;
                y = Cursor.Position.Y;
            }
            if (x != null && y != null)
            {
                //then find control from the position
                AutomationElement autoEle = AutomationElement.FromPoint(new System.Windows.Point(x.Value, y.Value));
                if (autoEle != null)
                {
                    //instantiate control based on the type
                    ctl = new Control(IntPtr.Zero, IntPtr.Zero);
                    //ctl.ControlTypeName = autoEle.Current.ControlType.ProgrammaticName; not needed as it is handled in the get part of the property ControlTypeName. Read from ControlElementFound property
                    ctl.ControlElementFound = autoEle;
                }
                else
                    throw new Exception("No control could be found at the intended position");
            }
            else
                throw new Exception("Either x or y coordinate is not identifed or not valid");
            return ctl;
        }

        /// <summary>
        /// This method is used to find a matching control based on CanonicalPath. Legacy method to support older deployments.
        /// </summary>
        ///<param name="canonicalPath">canonicalPath</param>
        /// <param name="sourceImage">Optional - Source Image in which the element is to be located. 
        /// To be specified if the control recognition is to be handled in the background processing mode only.Applicable only in ImageMode.</param>
        ///<returns>Matched control</returns>
        public Control FindControl(string canonicalPath, Stream sourceImage = null)
        {
            return FindControl(canonicalPath, true, false, sourceImage);
        }
        /// <summary>
        /// This method is used to find a single control based on CanonicalPath. Additional configuration settings provided to do forcerefresh
        /// or use OR condition checks.
        /// </summary>
        /// <param name="canonicalPath">canonicalPath</param>
        /// <param name="allControlIdentifiersMustMatch">If true,an AND condition check on all control identifier properties is done.
        /// In AND Condition check then values of automationid, control name and control type should all match when trying to finding the control. 
        /// If false, aa OR condition check on all control identifier properties is done. In OR condition check, a match in value of either automationid, control name and control type 
        /// will result in the element being considered as a match and returned in the control collection </param>
        /// <param name="forceControlRefresh">Force refresh of the control element even if it exists in element cache</param>
        /// <param name="sourceImage">Optional - Source Image in which the element is to be located. 
        /// To be specified if the control recognition is to be handled in the background processing mode only.Applicable only in ImageMode.</param>
        /// <returns>Control</returns>
        public Control FindControl(string canonicalPath, bool allControlIdentifiersMustMatch, bool forceControlRefresh, Stream sourceImage = null)
        {
            getAllMatchingControls = false;
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.FINDCONTROL))
            {
                //in param
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "canonicalPath", Logging.Constants.PARAMDIRECTION_IN, canonicalPath);

                Control ctl = null;
                Application app = null;
                Screen screen = null;
                if (Applications != null)
                {
                    string[] canonicalPathParts = canonicalPath.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    if (canonicalPathParts != null && canonicalPathParts.Length >= 2) //atleast the application part and the concerned control. as per new requirement, there may not be any screen part mentioned
                    {
                        LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_VARIABLE_VALUE, LogHandler.Layer.Business, "canonicalPathParts", canonicalPathParts.Length.ToString());
                        bool stopTraversing = false;
                        try
                        {
                            for (int i = 0; i < canonicalPathParts.Length; i++)
                            {
                                if (stopTraversing)
                                    break;
                                switch (i)
                                {
                                    case 0: //application
                                        app = Applications[canonicalPathParts[i]];
                                        break;
                                    case 1: //screen
                                        if (app == null)
                                            stopTraversing = true;
                                        else if (app.Screens != null && app.Screens.ContainsKey(canonicalPathParts[i]))
                                        {
                                            screen = app.Screens[canonicalPathParts[i]];
                                            if (!string.IsNullOrEmpty(screen.AutomationName) && screen.AutomationName.Trim() != "")
                                                screen.RefreshScreenHandle();
                                        }
                                        else //i.e. no screen name provided, then the control details wud be directly under the application
                                        {
                                            if (ctl == null) //i.e. first level of control under the screen
                                            {
                                                ctl = app.Controls[canonicalPathParts[i]];
                                                ctl.GetAllMatchingControls = false;
                                                if (WaitForever)
                                                {
                                                    ctl.DoEntityCaching = false;
                                                    ctl.DoTabTracking = false;
                                                }
                                                if ((!allControlIdentifiersMustMatch) && (i == canonicalPathParts.Length - 1))
                                                {
                                                    ctl.UpdateCondition(ctl.AutomationId, ctl.AutomationName, false);
                                                }
                                                ctl.RefreshControlHandle(app.WindowsHandle, IntPtr.Zero);
                                            }
                                            else //i.e. for controls under control
                                            {
                                                parentControl = ctl;
                                                ctl = ctl.Controls[canonicalPathParts[i]];
                                                ctl.GetAllMatchingControls = false;
                                                if (WaitForever)
                                                {
                                                    ctl.DoEntityCaching = false;
                                                    ctl.DoTabTracking = false;
                                                }
                                                if ((!allControlIdentifiersMustMatch) && (i == canonicalPathParts.Length - 1))
                                                {
                                                    ctl.UpdateCondition(ctl.AutomationId, ctl.AutomationName, false);
                                                }
                                                if (!ctl.IsControlAvailable() || forceControlRefresh)
                                                {
                                                    ctl.RefreshControlHandle(app.WindowsHandle, IntPtr.Zero);
                                                }
                                            }
                                        }
                                        break;
                                    default: //control and also to consider any level under control
                                        //if (screen == null)      //need not to be check because as per new requirement, screen name may not be provided
                                        //    stopTraversing = true;
                                        //else
                                        if (ctl == null) //i.e. first level of control under the screen
                                        {
                                            ctl = screen.Controls[canonicalPathParts[i]];
                                            ctl.GetAllMatchingControls = false;
                                            if (WaitForever)
                                            {
                                                ctl.DoEntityCaching = false;
                                                ctl.DoTabTracking = false;
                                            }
                                            if ((!allControlIdentifiersMustMatch) && (i == canonicalPathParts.Length - 1))
                                            {
                                                ctl.UpdateCondition(ctl.AutomationId, ctl.AutomationName, false);
                                            }
                                            ctl.RefreshControlHandle(app.WindowsHandle, screen.WindowsHandle);
                                        }
                                        else //i.e. for controls under control
                                        {
                                            parentControl = ctl;
                                            ctl = ctl.Controls[canonicalPathParts[i]];
                                            ctl.GetAllMatchingControls = false;
                                            if (WaitForever)
                                            {
                                                ctl.DoEntityCaching = false;
                                                ctl.DoTabTracking = false;
                                            }
                                            if ((!allControlIdentifiersMustMatch) && (i == canonicalPathParts.Length - 1))
                                            {
                                                ctl.UpdateCondition(ctl.AutomationId, ctl.AutomationName, false);
                                            }
                                            if (!ctl.IsControlAvailable() || forceControlRefresh)
                                            {
                                                ctl.RefreshControlHandle(app.WindowsHandle, screen.WindowsHandle);
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                        catch (System.Collections.Generic.KeyNotFoundException ex)
                        {
                            throw new Exception("Incorrect name provided either for Application or Screen or Control in the canonical path.");
                        }
                    }
                }

                //TODO:Rahul Return Control Instance from the Automation object
                //Check if the control being accessed is active (check property Control.isControlAvailable), if not than refresh the sceen. if appwinhandle is not available than refresh applevel object
                if (ctl != null && !ctl.IsControlAvailable() && app != null && screen != null)
                {
                    try
                    {
                        pSourceImage = sourceImage;
                        ctl = RefreshControl(app, screen, ctl);
                    }
                    finally
                    {
                        //clean the internal source image stream
                        pSourceImage = null;
                    }

                }

                //highlight the UI of the automation element, if any
                if (highlightElement && ctl != null)
                {
                    ctl.Highlight();
                }

                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.FINDCONTROL);

                return ctl;
            }
        }
        /// <summary>
        /// This method is used to find all the matching controls based on CanonicalPath. Legacy method to support older deployments.
        /// </summary>
        ///<param name="canonicalPath">canonicalPath</param>
        /// <param name="sourceImage">Optional - Source Image in which the element is to be located. 
        /// To be specified if the control recognition is to be handled in the background processing mode only. Applicable only in ImageMode.</param>
        ///<returns>List of matching controls</returns>
        public List<Control> FindControls(string canonicalPath, Stream sourceImage = null)
        {
            return FindControls(canonicalPath, true, false, sourceImage);
        }

        /// <summary>
        /// This method is used to find all the matching controls based on CanonicalPath. Additional configuration settings provided to do forcerefresh
        /// or use OR condition checks.
        /// </summary>
        /// <param name="canonicalPath">canonicalPath</param>
        /// <param name="allControlIdentifiersMustMatch">If true,an AND condition check on all control identifier properties is done.
        /// In AND Condition check then values of automationid, control name and control type should all match when trying to finding the control. 
        /// If false, aa OR condition check on all control identifier properties is done. In OR condition check, a match in value of either automationid, control name and control type 
        /// will result in the element being considered as a match and returned in the control collection </param>
        /// <param name="forceControlRefresh">Force refresh of the control element even if it exists in element cache</param>
        /// <param name="sourceImage">Optional - Source Image in which the element is to be located. 
        /// To be specified if the control recognition is to be handled in the background processing mode only. Applicable only in ImageMode.</param>
        /// <returns>List of matching controls</returns>
        public List<Control> FindControls(string canonicalPath, bool allControlIdentifiersMustMatch, bool forceControlRefresh, Stream sourceImage = null)
        {
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.FINDCONTROLS))
            {
                getAllMatchingControls = true;
                //in param
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "canonicalPath", Logging.Constants.PARAMDIRECTION_IN, canonicalPath);
                List<Control> ctrls = new List<Control>();
                Control ctl = null;
                Application app = null;
                Screen screen = null;
                if (Applications != null)
                {
                    string[] canonicalPathParts = canonicalPath.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    if (canonicalPathParts != null && canonicalPathParts.Length >= 2) //atleast the application part and the concerned control. as per new requirement, there may not be any screen part mentioned
                    {
                        LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_VARIABLE_VALUE, LogHandler.Layer.Business, "canonicalPathParts", canonicalPathParts.Length.ToString());
                        bool stopTraversing = false;
                        for (int i = 0; i < canonicalPathParts.Length; i++)
                        {
                            if (stopTraversing)
                                break;
                            switch (i)
                            {
                                case 0: //application
                                    app = Applications[canonicalPathParts[i]];
                                    break;
                                case 1: //screen
                                    if (app == null)
                                        stopTraversing = true;
                                    else if (app.Screens != null && app.Screens.ContainsKey(canonicalPathParts[i]))
                                    {
                                        screen = app.Screens[canonicalPathParts[i]];
                                        if (!string.IsNullOrEmpty(screen.AutomationName) && screen.AutomationName.Trim() != "")
                                            screen.RefreshScreenHandle();
                                    }
                                    else //i.e. no screen name provided, then the control details wud be directly under the application
                                    {
                                        if (ctl == null) //i.e. first level of control under the screen
                                        {
                                            ctl = app.Controls[canonicalPathParts[i]];

                                            //set GetAllMatchingControls to tru only if this is last in the caconical parts
                                            if (i + 1 == canonicalPathParts.Length)
                                                ctl.GetAllMatchingControls = true;
                                            if (WaitForever)
                                            {
                                                ctl.DoEntityCaching = false;
                                                ctl.DoTabTracking = false;
                                            }

                                            if ((!allControlIdentifiersMustMatch) && (i == canonicalPathParts.Length - 1))
                                            {
                                                ctl.UpdateCondition(ctl.AutomationId, ctl.AutomationName, false);
                                            }
                                            ctl.RefreshControlHandle(app.WindowsHandle, IntPtr.Zero);
                                        }
                                        else //i.e. for controls under control
                                        {
                                            ctl = ctl.Controls[canonicalPathParts[i]];
                                            if (i + 1 == canonicalPathParts.Length)
                                                ctl.GetAllMatchingControls = true;
                                            if (WaitForever)
                                            {
                                                ctl.DoEntityCaching = false;
                                                ctl.DoTabTracking = false;
                                            }
                                            if ((!allControlIdentifiersMustMatch) && (i == canonicalPathParts.Length - 1))
                                            {
                                                ctl.UpdateCondition(ctl.AutomationId, ctl.AutomationName, false);
                                            }
                                            if (!ctl.IsControlAvailable() || forceControlRefresh)
                                            {
                                                ctl.RefreshControlHandle(app.WindowsHandle, IntPtr.Zero);
                                            }
                                        }
                                    }
                                    break;
                                default: //control and also to consider any level under control
                                    //if (screen == null)      //need not to be check because as per new requirement, screen name may not be provided
                                    //    stopTraversing = true;
                                    //else
                                    if (ctl == null) //i.e. first level of control under the screen
                                    {
                                        ctl = screen.Controls[canonicalPathParts[i]];
                                        if (i + 1 == canonicalPathParts.Length)
                                            ctl.GetAllMatchingControls = true;
                                        if (WaitForever)
                                        {
                                            ctl.DoEntityCaching = false;
                                            ctl.DoTabTracking = false;
                                        }
                                        if ((!allControlIdentifiersMustMatch) && (i == canonicalPathParts.Length - 1))
                                        {
                                            ctl.UpdateCondition(ctl.AutomationId, ctl.AutomationName, false);
                                        }
                                        ctl.RefreshControlHandle(app.WindowsHandle, screen.WindowsHandle);
                                    }
                                    else //i.e. for controls under control
                                    {
                                        ctl = ctl.Controls[canonicalPathParts[i]];
                                        if (i + 1 == canonicalPathParts.Length)
                                            ctl.GetAllMatchingControls = true;
                                        if (WaitForever)
                                        {
                                            ctl.DoEntityCaching = false;
                                            ctl.DoTabTracking = false;
                                        }
                                        if ((!allControlIdentifiersMustMatch) && (i == canonicalPathParts.Length - 1))
                                        {
                                            ctl.UpdateCondition(ctl.AutomationId, ctl.AutomationName, false);
                                        }
                                        if (!ctl.IsControlAvailable() || forceControlRefresh)
                                        {
                                            ctl.RefreshControlHandle(app.WindowsHandle, screen.WindowsHandle);
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
                if (ctl != null)
                {
                    if (ctl.DiscoveryMode == ElementDiscovertyMode.API || ctl.DiscoveryMode == ElementDiscovertyMode.APIAndImage)
                    {
                        if (app != null && screen != null && ctl.ControlElementCollectionFound == null && ctl.ControlElementFound == null)
                        {
                            try
                            {
                                pSourceImage = sourceImage;
                                ctl = RefreshControl(app, screen, ctl);
                            }
                            finally
                            {
                                pSourceImage = null;
                            }
                        }
                        else
                        {
                            if (ctl.ControlElementCollectionFound != null && ctl.ControlElementCollectionFound.Count > 0)
                            {
                                foreach (System.Windows.Automation.AutomationElement element in ctl.ControlElementCollectionFound)
                                {
                                    //ctl.ControlElementFound = element;
                                    //Control tempctl = ctl;
                                    //ctrls.Add(tempctl);
                                    //the following approach is used to create control instances to avoid shallow copy
                                    Control tempctl = new Control(IntPtr.Zero, IntPtr.Zero);

                                    tempctl.ApplicationType = ctl.ApplicationType;
                                    tempctl.AutomationId = ctl.AutomationId;
                                    tempctl.AutomationName = ctl.AutomationName;
                                    tempctl.ControlPath = ctl.ControlPath;
                                    tempctl.ControlElementFound = element;
                                    tempctl.Controls = ctl.Controls;
                                    tempctl.DiscoveryMode = ctl.DiscoveryMode;
                                    tempctl.DoEntityCaching = ctl.DoEntityCaching;
                                    tempctl.DoTabTracking = ctl.DoTabTracking;
                                    tempctl.FullControlQualifier = ctl.FullControlQualifier;
                                    tempctl.GetAllMatchingControls = ctl.GetAllMatchingControls;
                                    tempctl.Name = ctl.Name;
                                    ctrls.Add(tempctl);
                                }
                            }
                            else if (ctl.ControlElementFound != null)
                            {
                                //Control tempctl = new Control(IntPtr.Zero, IntPtr.Zero);
                                //tempctl.ControlElementFound = ctl.ControlElementFound;
                                //ctrls.Add(tempctl);
                                ctrls.Add(ctl);
                            }
                        }
                    }
                    else if (ctrls.Count == 0 && (ctl.DiscoveryMode == ElementDiscovertyMode.Image || ctl.DiscoveryMode == ElementDiscovertyMode.APIAndImage))
                    {
                        if (ctl.ImageReference != null && ctl.ImageReference.SupportedStates != null && ctl.ImageReference.SupportedStates.Count > 0)
                        {
                            foreach (var item in ctl.ImageReference.SupportedStates)
                            {
                                //Sid: 25 Dec 2017 - Updated logic to load all identified control in one state to a tempControl list
                                // and then load the controls instance. This fixes a bug in which controls identified across states were 
                                //overwritting the controls identified in previous state when the FindControlInMultipleControlStates property was
                                //set to true
                                List<Control> tempControl = null;
                                tempControl = FindControls(item.ImagePath, true, sourceImage);

                                if (tempControl != null && tempControl.Count > 0)
                                {
                                    //update the state of control
                                    tempControl.ForEach(c =>
                                    {
                                        c.ImageReference.CurrentState = item.State;
                                    });
                                    for (int iCount = 0; iCount <= tempControl.Count - 1; iCount++)
                                    {
                                        ctrls.Add(tempControl[iCount]);
                                    }
                                    if (!findControlInMultipleControlStates)
                                        break;
                                }
                            }
                        }
                    }
                }

                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.FINDCONTROLS);

                return ctrls;
            }
        }

        /// <summary>
        /// Interface to identify the control based on the image template provided
        /// </summary>
        /// <param name="elementImagePath">the path of the Element image which will be used to locate within the Source image</param>
        /// <param name="imageRecog">set it to true</param>
        /// <param name="sourceImage">Optional - Source Image in which the element is to be located. 
        /// To be specified if the control recognition is to be handled in the background processing mode only. Applicable only in ImageMode.</param>
        /// <returns></returns>
        public Control FindControl(string elementImagePath, bool imageRecog, Stream sourceImage = null)
        {
            getAllMatchingControls = false;
            LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.FINDCONTROL);
            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "elementImagePath", Logging.Constants.PARAMDIRECTION_IN, elementImagePath);
            Control ctl = null;
            if (!string.IsNullOrEmpty(elementImagePath) && File.Exists(elementImagePath))
            {
                ctl = new Control(IntPtr.Zero, IntPtr.Zero);
                ctl.ImageReference = new ControlImageReference();
                ctl.ImageReference.SupportedStates = new List<ControlStateReference>();

                ControlStateReference state = new ControlStateReference();
                state.ImagePath = elementImagePath;
                state.State = "";

                ctl.ImageReference.SupportedStates.Add(state);
                Core.Utilities.ScaleStep = imageMatchScaleStepSize;
                Core.Utilities.MaxScaleSteps = imageMatchMaxScaleStepCount;
                ctl.ImageReference = Core.Utilities.GetBoundingRectangle(ctl.ImageReference, templateMachingInOriginalScale, waitForeverForTemplate, useTrueColorTemplateMatching, null, ATRFileDirectory, sourceImage);
                ctl.ImageBoundingRectangle = ctl.ImageReference.CurrentBoundingRectangle;

            }
            else
                LogHandler.LogError(Logging.InformationMessages.RUNTIMEWRAPPER_INVALID_DATA, LogHandler.Layer.Business, "Element image path", elementImagePath);

            //highlight the matched region is asked to do so
            if (highlightElement && ctl != null && ctl.ImageBoundingRectangle != System.Windows.Rect.Empty)
            {
                ctl.Highlight();
            }

            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.FINDCONTROL);
            return ctl;
        }

        /// <summary>
        /// Interface to identify all the controls similar to the provided template image
        /// </summary>
        /// <param name="elementImagePath">the path of the Element image which will be used to locate within the Source image</param>
        /// <param name="ForImagePath">set it to true</param>
        /// <param name="sourceImage">Optional - Source Image in which the element is to be located. 
        /// To be specified if the control recognition is to be handled in the background processing mode only. Applicable only in ImageMode.</param>
        /// <returns>all the maching controls</returns>
        public List<Control> FindControls(string elementImagePath, bool ForImagePath, Stream sourceImage = null)
        {
            LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.FINDCONTROLS);
            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "elementImagePath", Logging.Constants.PARAMDIRECTION_IN, elementImagePath);
            int timeout = 0;
            if (WaitForever)
                timeout = -1;

            List<Control> controls = null;
            if (!string.IsNullOrEmpty(elementImagePath) && File.Exists(elementImagePath))
            {
                controls = new List<Control>();
                List<System.Windows.Rect> rects = new List<System.Windows.Rect>();
                Core.Utilities.ScaleStep = imageMatchScaleStepSize;
                Core.Utilities.MaxScaleSteps = imageMatchMaxScaleStepCount;
                Core.Utilities.TemplateMatchMapBorderColor = templateMatchMappingBorderColor;
                Core.Utilities.TemplateMatchMapBorderThickness = templateMatchMappingBorderThickness;
                if (useTrueColorTemplateMatching)
                    rects = Core.Utilities.FindAllInstancesInTrueColor(elementImagePath, timeout, imageMatchConfidenceThreshold, !templateMachingInOriginalScale, sourceImage, enableTemplateMatchMapping);
                else
                    rects = Core.Utilities.FindAllInstances(elementImagePath, timeout, imageMatchConfidenceThreshold, !templateMachingInOriginalScale, sourceImage, enableTemplateMatchMapping);
                if (rects != null && rects.Count > 0)
                {
                    rects.ForEach(rect =>
                    {
                        Control ctl = new Control(IntPtr.Zero, IntPtr.Zero);
                        ctl.ImageReference = new ControlImageReference();
                        ctl.ImageReference.SupportedStates = new List<ControlStateReference>();

                        ControlStateReference state = new ControlStateReference();
                        state.ImagePath = elementImagePath;
                        state.State = "";

                        ctl.ImageReference.SupportedStates.Add(state);
                        ctl.ImageReference.CurrentBoundingRectangle = rect;
                        ctl.ImageBoundingRectangle = rect;

                        controls.Add(ctl);
                    });
                }
            }
            else
                LogHandler.LogError(Logging.InformationMessages.RUNTIMEWRAPPER_INVALID_DATA, LogHandler.Layer.Business, "Element image path", elementImagePath);

            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.FINDCONTROLS);
            return controls;
        }

        /// <summary>
        /// This method is used to find Application based on name passed.
        /// </summary>
        /// <param name="name">Name of the Application</param>
        /// <param name="windowsTitle">[Optional] the title of the application window to overwrite the old value</param>
        /// /// <param name="timeOut">[Optional] in seconds, the duration for which it would be tried to get the windows handle based on the process id or windows title</param>
        /// <returns>Application Object</returns>
        public Application FindApplication(string name, string windowsTitle = "", int timeOut = 0)
        {
            Application app = null;

            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.FIND_APPLICATION))
            {
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "application name", Logging.Constants.PARAMDIRECTION_IN, name);
                if (!string.IsNullOrEmpty(name) && Applications != null && Applications.ContainsKey(name))
                {
                    app = Applications[name];
                    //try to overwrite the application control name, if provided
                    if (!string.IsNullOrEmpty(windowsTitle))
                    {
                        Applications[name].AutomationName = windowsTitle;
                    }

                    if (timeOut > 0)
                        Applications[name].TimeOut = timeOut;

                    //check if there is value for the application windows handle, if not, then try to get it
                    if (!app.IsAvailable)
                    {
                        app.GetWindowsHandle();
                    }
                }
                else
                {
                    LogHandler.LogError(Logging.InformationMessages.RUNTIMEWRAPPER_INVALID_DATA, LogHandler.Layer.Business, "application name", name);
                    throw new Exception(string.Format("Application- {0} provided is invalid", name));
                }
            }

            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.FIND_APPLICATION);
            return app;
        }

        /// <summary>
        /// This method is used to find Screen under the Application in the control mopdel (atr) based on name passed.
        /// </summary>
        /// <param name="applicationName">Name of the Application having the screen</param>
        /// <param name="screenName">Name of the Screen</param>
        /// <returns>Screen Object</returns>
        public Screen FindScreen(string applicationName, string screenName)
        {
            Application app = null;
            Screen screen = null;

            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.FIND_SCREEN))
            {
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "screen name", Logging.Constants.PARAMDIRECTION_IN, screenName);
                if (!string.IsNullOrEmpty(applicationName) && Applications != null && Applications.ContainsKey(applicationName))
                {
                    app = Applications[applicationName];
                    if (!string.IsNullOrEmpty(screenName) && app.Screens.ContainsKey(screenName))
                    {
                        screen = app.Screens[screenName];
                    }
                    else
                    {
                        LogHandler.LogError(Logging.InformationMessages.RUNTIMEWRAPPER_INVALID_DATA, LogHandler.Layer.Business, "screen name", screenName);
                        throw new Exception(string.Format("Screen- {0} provided is invalid", screenName));
                    }

                    //check if there is value for the screen windows handle, if not, then try to get it
                    if (!screen.IsAvailable)
                    {
                        screen.GetWindowsHandle();
                    }
                }
                else
                {
                    LogHandler.LogError(Logging.InformationMessages.RUNTIMEWRAPPER_INVALID_DATA, LogHandler.Layer.Business, "application name", applicationName);
                    throw new Exception(string.Format("Application- {0} provided is invalid while calling FindScreen", applicationName));
                }
            }

            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.FIND_APPLICATION);
            return screen;
        }

        /// <summary>
        /// This method is used to refresh the control.
        /// </summary>
        /// <param name="app">Application object</param>
        /// <param name="screen">Screen object</param>
        /// <param name="ctl">Control object</param>
        /// <returns>Control object</returns>
        private Control RefreshControl(Application app, Screen screen, Control ctl)
        {
            int counter = numOfTrials;
            //System.Threading.Thread.Sleep(1000); // need to the initial warm up i.e. automation element to inialize which happen asynchronous way. need not to increase.
            if (ctl.DiscoveryMode == ElementDiscovertyMode.None)
            {
                ctl = null;
            }
            else if (ctl.DiscoveryMode == ElementDiscovertyMode.API || ctl.DiscoveryMode == ElementDiscovertyMode.APIAndImage)
            {
                do
                {
                    //check if stop requested, if show then throw exception
                    if (Core.Utilities.IsStopRequested())
                        throw new Core.IAPExceptions.StopRequested();

                    counter--;

                    //first try refreshing thru the control itself
                    ctl.RefreshControlHandle(app.WindowsHandle, screen.WindowsHandle);
                    if (ctl.IsControlAvailable())
                        break;
                    //return ctl;
                    //if yet the control is not availbale then try thru the screen
                    //if (!ctl.IsControlAvailable())
                    screen.RefreshScreenHandle(app.WindowsHandle);
                    if (ctl.IsControlAvailable())
                        break;
                    //return ctl;
                    //if yet the control is not availbale then try thru the application
                    //if (!ctl.IsControlAvailable())
                    app.RefreshAppHandle();
                    if (ctl.IsControlAvailable())
                        break;
                    //return ctl;
                    //doubt- will refreshing the screen or app, will also refresh the control?    
                    System.Threading.Thread.Sleep(timeGapBetweenTrials);
                } while (!ctl.IsControlAvailable() && (counter > 0 || WaitForever));

                //if yet the control is not available then return null
                if (ctl.DiscoveryMode == ElementDiscovertyMode.API && !ctl.IsControlAvailable())
                {
                    ctl = null;
                    return ctl;
                }
            }

            if (!ctl.IsControlAvailable() && (ctl.DiscoveryMode == ElementDiscovertyMode.Image || (ctl.DiscoveryMode == ElementDiscovertyMode.APIAndImage)) && !getAllMatchingControls)
            {
                DateTime startTime = DateTime.Now;
                do
                {
                    //check if stop requested, if show then throw exception
                    if (Core.Utilities.IsStopRequested())
                        throw new Core.IAPExceptions.StopRequested();

                    counter--; //to be used for the mmixed approach
                    //check if the parent of this control is also a control
                    //if so, then if there is any state provided for the parent control to be used for restricted image match
                    System.Windows.Rect searchRegion = System.Windows.Rect.Empty;
                    if (parentControl != null && parentControl.ImageReference != null)
                    {
                        Core.Utilities.ScaleStep = imageMatchScaleStepSize;
                        Core.Utilities.MaxScaleSteps = imageMatchMaxScaleStepCount;
                        parentControl.ImageReference = Core.Utilities.GetBoundingRectangle(parentControl.ImageReference, templateMachingInOriginalScale, waitForeverForTemplate, useTrueColorTemplateMatching, null, ATRFileDirectory, pSourceImage);
                        parentControl.ImageBoundingRectangle = parentControl.ImageReference.CurrentBoundingRectangle;
                        searchRegion = parentControl.ImageBoundingRectangle;
                        //use this search region for the identifying the bounding rectangle of the final control
                        if (parentControl != null && highlightElement)
                            parentControl.Highlight();
                    }

                    //try to check if the bounding rectangle can be identified. i.e. in case of image based control discovery
                    if (ctl != null && ctl.ImageReference != null)
                    {
                        Core.Utilities.ScaleStep = imageMatchScaleStepSize;
                        Core.Utilities.MaxScaleSteps = imageMatchMaxScaleStepCount;
                        ctl.ImageReference = Core.Utilities.GetBoundingRectangle(ctl.ImageReference, templateMachingInOriginalScale, waitForeverForTemplate, useTrueColorTemplateMatching, searchRegion, ATRFileDirectory, pSourceImage);
                        ctl.ImageBoundingRectangle = ctl.ImageReference.CurrentBoundingRectangle;
                        //if (ctl.ImageBoundingRectangle != System.Windows.Rect.Empty)
                        //    return ctl;
                        //else
                        //    return null;
                        //if (ctl.ImageBoundingRectangle == System.Windows.Rect.Empty)
                        //    ctl = null;
                    }
                    else
                        ctl = null;
                    System.Threading.Thread.Sleep(timeGapBetweenTrials);
                }
                while ((ctl == null || (ctl != null && ctl.ImageBoundingRectangle == System.Windows.Rect.Empty)) && (DateTime.Now - startTime).TotalSeconds <= timeoutInSec);

                if (ctl.ImageBoundingRectangle == System.Windows.Rect.Empty)
                    ctl = null;
            }
            return ctl;
        }

        private void TranslateAutomationConfig(string xmlString, bool launchApps = false)
        {
            Applications = Core.Translation.PopulateApplications(xmlString, launchApps, showWaitBox, firstAppToStart);
        }

        /// <summary>
        /// This method is used to populate certain configurations from the config file.
        /// </summary>
        private void PopulateRetrySettings()
        {

            string trialCount = "", timeGap = "";
            trialCount = System.Configuration.ConfigurationManager.AppSettings["NumberOfTrials"];
            if (!string.IsNullOrEmpty(trialCount))
                int.TryParse(trialCount, out numOfTrials);

            timeGap = System.Configuration.ConfigurationManager.AppSettings["TimeGapInMillisecondsBetweenTrials"];
            if (!string.IsNullOrEmpty(timeGap))
                int.TryParse(timeGap, out timeGapBetweenTrials);
        }

        /// <summary>
        /// This method returns the absoluate path based on relativePath.
        /// </summary>
        /// <param name="relativePath">Relative path</param>
        /// <returns>Absolute Path</returns>
        private string GetProbableAbsolutePath(string relativePath)
        {
            string absPath = Path.Combine(System.Windows.Forms.Application.ExecutablePath, relativePath);
            return absPath;
        }

        public bool HighlightElement
        {
            get
            {
                return highlightElement;
            }
            set
            {
                highlightElement = value;
            }
        }

        public bool FindControlInMultipleControlStates
        {
            get
            {
                return findControlInMultipleControlStates;
            }
            set
            {
                findControlInMultipleControlStates = value;
            }
        }

        public bool MultipleScaleTemplateMatching
        {
            get
            {
                return !templateMachingInOriginalScale;
            }
            set
            {
                templateMachingInOriginalScale = !value;
            }
        }

        public int ImageRecognitionTimeout
        {
            get
            {
                return timeoutInSec;
            }
            set
            {
                timeoutInSec = value;
            }
        }

        public bool WaitForever
        {
            get
            {
                return waitForeverForTemplate;
            }
            set
            {
                waitForeverForTemplate = value;
            }
        }

        public bool UseTrueColorTemplateMatching
        {
            get
            {
                return useTrueColorTemplateMatching;
            }
            set
            {
                useTrueColorTemplateMatching = value;
            }
        }

        public int ImageMatchConfidenceThreshold
        {
            get
            {
                return imageMatchConfidenceThreshold;
            }
            set
            {
                imageMatchConfidenceThreshold = value;
            }
        }

        private double imageMatchScaleStepSize = 0.2;
        public double ImageMatchScaleStepSize
        {
            get
            {
                return imageMatchScaleStepSize;
            }
            set
            {
                imageMatchScaleStepSize = value;
            }
        }

        private int imageMatchMaxScaleStepCount = 20;
        public int ImageMatchMaxScaleStepCount
        {
            get
            {
                return imageMatchMaxScaleStepCount;
            }
            set
            {
                imageMatchMaxScaleStepCount = value;
            }
        }

        public bool ShowApplicationStartingWaitBox
        {
            get
            {
                return showWaitBox;
            }
            set
            {
                showWaitBox = value;
                if (Applications != null && Applications.Count > 0)
                {
                    foreach (string app in Applications.Keys)
                    {
                        Applications[app].ShowAppStartWaitBox = value;
                    }
                }
            }
        }

        /// <summary>
        /// 'Select all' action (ctrl + a). For this to work first highlight the text area from where the text is to selected.
        /// </summary>
        public void SelectAll()
        {
            Core.Utilities.KeyPress("A", Core.KeyModifier.ControlKey);
        }

        /// <summary>
        /// Fires 'ctrl + c' action. For this to work first select the text to be copied.
        /// </summary>
        public void Copy()
        {
            Core.Utilities.KeyPress("C", Core.KeyModifier.ControlKey);
        }

        /// <summary>
        /// Clears the content from the clipboard
        /// </summary>
        public void ClearClipboard()
        {
            Thread thread = new Thread(() => System.Windows.Forms.Clipboard.Clear());
            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            thread.Start();
            thread.Join(); //Wait for the thread to end
            //System.Windows.Forms.Clipboard.Clear();
        }

        /// <summary>
        /// Gets the text from the clipboard
        /// </summary>
        /// <returns>text copied in the clipboard</returns>
        public string Read()
        {
            string _Read = null;
            Thread thread = new Thread(() => { _Read = System.Windows.Forms.Clipboard.GetText(); });
            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            thread.Start();
            thread.Join();//Wait for the thread to end
            return _Read;
            //return System.Windows.Forms.Clipboard.GetText();
        }

        /// <summary>
        /// Fires ctrl+v to paste the applicable content to the target.
        /// For this method to work, the destination must have been  selected in some way, (probably by calling click()). 
        /// </summary>
        public void Paste()
        {
            Core.Utilities.KeyPress("V", Core.KeyModifier.ControlKey);
        }

        /// <summary>
        /// Attempts to paste text by copying it to the clipboard and pasting it into the destination. 
        /// For this method to work, the destination must have been  selected in some way, (probably by calling click()). 
        /// The destination must also support input from the clipboard.
        /// </summary>
        /// <param name="text">the string to pasted</param>
        public void Paste(string text)
        {
            //first add the text to the clip board
            Thread thread = new Thread(() => System.Windows.Forms.Clipboard.SetText(text));
            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            thread.Start();
            thread.Join(); //Wait for the thread to end

            Core.Utilities.SendText(text);
        }

        /// <summary>
        /// This method is used to copy text to clipboard.
        /// </summary>
        /// <param name="text">Text to be copied</param>
        public void ReadToClipboard(string text)
        {
            //Thread thread = new Thread(() => System.Windows.Forms.Clipboard.SetText(text)); // Commented for SetText, beacuse it don't place data on clipboard while performing clipboad opertion on VDI machin. 

            Thread thread = new Thread(() => System.Windows.Forms.Clipboard.SetDataObject(text)); //Clears the Clipboard and then places nonpersistent data on it. 
            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            thread.Start();
            thread.Join(); //Wait for the thread to end
        }

        /// <summary>
        /// Interface to be used in env like IronPython to make the thread for the provided seconds
        /// </summary>
        /// <param name="seconds">number of seconds to sleep</param>
        public void Sleep(int seconds)
        {
            System.Threading.Thread.Sleep(seconds * 1000);
        }

        /// <summary>
        /// The intergrace to drag i.e. with left mouse down move from one control position to other control position.
        /// Could be leveraged to drag a entity from one location to another.
        /// </summary>
        /// <param name="startCanonicalPath">the control/place holder to depict the start of drag</param>
        /// <param name="destCanonicalPath">the control/place holder to depict the end of drag</param>
        public void Drag(string startCanonicalPath, string destCanonicalPath)
        {
            if (!string.IsNullOrEmpty(startCanonicalPath) && !string.IsNullOrEmpty(destCanonicalPath))
            {
                Control startCtl = FindControl(startCanonicalPath);
                Control destCtrl = FindControl(destCanonicalPath);
                if (startCtl != null && destCtrl != null)
                {
                    //then move mouse with left key down
                    startCtl.Hover();
                    Sleep(2);
                    startCtl.MouseDown();
                    destCtrl.Hover();
                    Sleep(2);
                    startCtl.MouseUp();
                }
            }
        }

        public void Drag(int startX, int startY, int endX, int endY)
        {
            //first hover to the start point
            Core.Utilities.PlaceMouseCursor((double)startX, (double)startY);
            Sleep(2);
            //mouse down
            Core.Utilities.DoMouseDown();
            //move to end point
            Core.Utilities.PlaceMouseCursor((double)endX, (double)endY);
            Sleep(2);
            //mouse up
            Core.Utilities.DoMouseUp();
        }

        /// <summary>
        /// The intergrace to drag i.e. with left mouse down move from one control position to other control position.
        /// Could be leveraged to drag a entity from one location to another.
        /// </summary>
        /// <param name="startCanonicalPath">the control/place holder to depict the start of drag</param>
        /// <param name="destX">the destination position x coordinates</param>
        /// <param name="destY">the destination position y coordinates</param>
        /// <param name="positionType">to tell if the destination x and y coordinates are absolute or relative</param>
        public void Drag(string startCanonicalPath, int destX, int destY, Core.DragDestinationType positionType)
        {
            if (!string.IsNullOrEmpty(startCanonicalPath))
            {
                Control startCtl = FindControl(startCanonicalPath);
                if (startCtl != null)
                {
                    startCtl.Hover();
                    startCtl.MouseDown();
                    switch (positionType)
                    {
                        case Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.DragDestinationType.AbsolutePosition:
                            startCtl.SetRegion(destX, destY, 1, 1);
                            break;
                        case Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.DragDestinationType.RelativePosition:
                            startCtl.OffsetRegion(destX, destY);
                            break;
                    }
                    Sleep(1);
                    startCtl.Hover();
                    startCtl.MouseUp();
                }
            }
        }

        public void SubscribeToFocusChangeEvent(FocusHasChangedEventHandler focusChangedCallBack)
        {
            FocusHasChanged += focusChangedCallBack;
            AutomationFocusChangedEventHandler focusHandler = new AutomationFocusChangedEventHandler(OnFocusChange);
            Automation.AddAutomationFocusChangedEventHandler(focusHandler);
        }

        public void DesubscribeToFocusChangeEvent(FocusHasChangedEventHandler focusChangedCallBack)
        {
            FocusHasChanged -= focusChangedCallBack;
            AutomationFocusChangedEventHandler focusHandler = new AutomationFocusChangedEventHandler(OnFocusChange);
            Automation.RemoveAutomationFocusChangedEventHandler(focusHandler);
        }

        private void OnFocusChange(object src, AutomationFocusChangedEventArgs e)
        {
            //from here raise the event to be used by the client code
            if (FocusHasChanged != null)
            {
                Control ctl = new Control(IntPtr.Zero, IntPtr.Zero);
                ctl.ControlElementFound = src as AutomationElement;
                if (ctl.ControlElementFound != null)
                {
                    ctl.AutomationId = ctl.ControlElementFound.Current.AutomationId;
                    ctl.AutomationName = ctl.ControlElementFound.Current.Name;
                    //ctl.ControlTypeName = ctl.ControlElementFound.Current.ControlType.ProgrammaticName; /// not needed at the property ControlTypeName looks into the under lining automation element
                }
                FocusHasChanged(new FocusHasChangedArgs() { Control = ctl });
            }
        }

        /// <summary>
        /// Reads text from an image. Image is captured using the input parameters provided
        /// </summary>
        /// <param name="x">X coordinate on the screen</param>
        /// <param name="y">Y coordinate on the screen</param>
        /// <param name="height">height of the image to be captured</param>
        /// <param name="width">width of the image to be captured</param>
        /// <param name="filter">filter string consisting of characters that could possibly occur in the image. It can be alphanumeric or especial characters too</param>
        /// <param name="imageResizeCoeff">Coefficient used to resize the original image. A positive coeff will result increasing image size and negative coeff will result in decreasing the image size</param>
        /// <returns>Text in the image captured using input parameters</returns>
        public string ReadTextArea(double x, double y, double height, double width, string filter = "", float imageResizeCoeff=1)
        {
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.READTEXTAREA))
            {
                string text = TextRecognitionManager.ReadTextArea(x, y, height, width, filter, imageResizeCoeff);

                return text;
            }
        }

        /// <summary>
        /// Reads text from an image. Image is captured using the input parameters provided
        /// </summary>
        /// <param name="x">X coordinate on the screen</param>
        /// <param name="y">Y coordinate on the screen</param>
        /// <param name="height">height of the image to be captured</param>
        /// <param name="width">width of the image to be captured</param>
        /// <param name="filter">Enum which specifies what type of text is being read</param>
        /// <param name="imageResizeCoeff">Coefficient used to resize the original image. A positive coeff will result increasing image size and negative coeff will result in decreasing the image size</param>
        /// <returns>Text in the image captured using input parameters</returns>
        public string ReadTextArea(double x, double y, double height, double width, TextType filter, float imageResizeCoeff = 1)
        {
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.READTEXTAREA))
            {
                string text = TextRecognitionManager.ReadTextArea(x, y, height, width, filter, imageResizeCoeff);

                return text;
            }
        }

        public void DoInteractiveCheck(string message, string directory = "")
        {
            if (InteractiveCheckExists == -1)
            {
                InteractiveCheckExists = CheckInteractiveCheckExists(directory);
            }
            {
                if (InteractiveCheckExists != -1)
                {
                    if (InteractiveCheckExists != 0)
                    {
                        DialogResult result = MessageBox.Show(message + "\r\nShould the script continue?", "IAP", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                        if (result == DialogResult.Cancel)
                        {
                            throw new InteractiveCheckFailedException();
                        }
                    }
                }
            }
        }

        private int CheckInteractiveCheckExists(string directory)
        {
            int value = -1;

            if (!string.IsNullOrEmpty(directory))
            {
                //Load XMl settings  
                string settingsFilePath = directory + @"\Infosys.ATR.WinUIAutomationRuntimeWrapper.xml";
                if (File.Exists(settingsFilePath))
                {
                    XElement settings = XElement.Load(settingsFilePath);
                    if (settings != null)
                    {
                        IEnumerable<XElement> elements = settings.Elements().Where(e => e.Name.LocalName == "EnableInteractiveCheck");
                        if (elements.Count() != 0)
                        {
                            string val = settings.Elements().Where(e => e.Name.LocalName == "EnableInteractiveCheck").Single().Value;
                            if (val.ToLower() == "true")
                                value = 1;
                            else value = 0;
                        }
                    }
                }
                else
                {
                    value = 0;
                }
            }
            else
            {
                //Load app settings  
                var obj = ConfigurationManager.AppSettings["EnableInteractiveCheck"];
                if (obj != null)
                {
                    value = 1;
                }
                else
                {
                    value = 0;
                }
            }
            return value;
        }

        private bool IsValidUrlFormat(string url)
        {
            Uri uriTryResult;
            bool isValid = Uri.TryCreate(url, UriKind.Absolute, out uriTryResult) && (uriTryResult.Scheme == Uri.UriSchemeHttp || uriTryResult.Scheme == Uri.UriSchemeHttps);
            return isValid;
        }

        private string StreamToString(Stream fileContent)
        {
            StreamReader reader = new StreamReader(fileContent);
            string fileString = reader.ReadToEnd();
            return fileString;
        }

        private string GetValidPackage(string path)
        {
            string[] iappackages = iapPackageTypes.Split(',');
            string validPackage = "";
            foreach (string iappackage in iappackages)
            {
                if (path.Contains(iappackage))
                {
                    validPackage = iappackage;
                    break;
                }
            }
            return validPackage;
        }

        public bool EnableTemplateMatchMapping
        {
            get
            {
                return enableTemplateMatchMapping;
            }
            set
            {
                enableTemplateMatchMapping = value;
            }
        }

        private bool enableTemplateMatchMapping=false;

        public byte[] TemplateMatchesMapScreen
        {
            get
            {
                return Core.Utilities.TemplateMatchMapScreen;
            }

        }
        public int TemplateMatchMappingBorderThickness
        {
            get
            {
                return templateMatchMappingBorderThickness;
            }
            set
            {
                templateMatchMappingBorderThickness = value;
            }
        }

        private int templateMatchMappingBorderThickness = 2;

        public Core.Utilities.ImageBgr TemplateMatchMappingBorderColor
        {
            get
            {
                return templateMatchMappingBorderColor;
            }
            set
            {
                templateMatchMappingBorderColor = value;
            }
        }

        private Core.Utilities.ImageBgr templateMatchMappingBorderColor;
    }
}

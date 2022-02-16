using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;
using System.Collections.Specialized;
using System.Windows.Automation;
using System.Windows.Forms;
using mshtml;
using SHDocVw;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Infosys.ATR.UIAutomation.SEE
{
    public class PlayActions
    {
        //user32 interfaces
        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        //user32 related constants
        const int KEYEVENTF_KEYUP = 0x2;
        const int KEYEVENTF_KEYDOWN = 0x0;

        static System.Windows.Forms.WebBrowser _Browser;
        /// <summary>
        /// To assign the browser instance to be used to pin point the html element. 
        /// Also make sure to nullify te browser when the form/control hosting the browser object is closed.
        /// </summary>
        public System.Windows.Forms.WebBrowser Browser
        {
            set
            {
                _Browser = value;
                Tracking.Browser = _Browser;
            }
            get
            {
                return _Browser;
            }
        }

        public PlayActions()
        {
        }
        public PlayActions(Entities.UseCase UseCases)
        {
            UseCaseCaptured = UseCases;
            tempUseCaseCaptured = UseCases;
            GetAppList();
        }

        //events and delegates
        public class ReadyToExecuteEventArgs : EventArgs
        {
            public string Application { get; set; }
            public bool NewApplicationInstance { get; set; }
        }
        public delegate void ReadyToExecuteEventHandler(ReadyToExecuteEventArgs e);
        /// <summary>
        /// Event raised giving a reference to the application in event agrs on which the 
        /// the execution engine is ready to execute the play back
        /// </summary>
        public event ReadyToExecuteEventHandler ReadyToExecute;

        public class UseCaseExecutedEventArgs : EventArgs
        {
            public List<Entities.NameValueAtribute> Data { get; set; }
            public string OtherInfo { get; set; }
        }
        public delegate void UseCaseExecutedEventHandler(UseCaseExecutedEventArgs e);
        /// <summary>
        /// Event to be raised once all the activities of an use case are executed
        /// </summary>
        public event UseCaseExecutedEventHandler UseCaseExecuted;

        //public properties
        /// <summary>
        /// The property to hold the complete use case details
        /// </summary>
        public Entities.UseCase UseCaseCaptured { get; set; }

        //private properties/ constants
        const int MaxElementIdenficationTrials = 3;
        const int GapBetweenTrials = 500; //in milliseconds
        int numberOfTrialsCounter = 0, collectiveKeyValue;
        bool executedTask = false, isACollectiveKey = false;
        Hashtable appToTaskMapping = new Hashtable();
        AutomationElement lastElement; //to be used in case keyboard key press
        IHTMLElement lastHTMLElement; //to be used in case keyboard key press
        HtmlElement lastHtmlElement; //to be used in case keyboard key press
        string lastKeyPress;//to be used in case keyboard key press
        AutomationElement appWindow;
        Entities.UseCase tempUseCaseCaptured; //to keep the record of the initial use case details as
        //in UseCaseCaptured the activities will be deleted once executed
        Entities.Activity activityBeingExecuted = new Entities.Activity(); //to hold the activity being executed

        //public interfaces
        /// <summary>
        /// Initiate the play back engine and keep it ready with the first application to be executed.
        /// </summary>
        public void InitiatePlayBack()
        {
            if (UseCaseCaptured != null && UseCaseCaptured.Activities != null && UseCaseCaptured.Activities.Count > 0)
            {
                ReadyToExecuteEventArgs args = new ReadyToExecuteEventArgs();
                //Entities.Activity act = UseCaseCaptured.Activities.First();
                Entities.Activity act = GetFirstActivityAndRemoveUnknown();

                if (act == null)
                    return;
                //args.Application = UseCaseCaptured.Activities.First().TargetApplication.ApplicationExe.ToLower();

                //check if this activity is a child activity, if so then first execute the parent activity
                if (string.IsNullOrEmpty(act.ParentId))
                {
                    args.Application = act.TargetApplication.ApplicationExe.ToLower();
                    if (string.IsNullOrEmpty(act.ParentId))
                        args.NewApplicationInstance = true;
                    else
                        args.NewApplicationInstance = false;
                    UseCaseCaptured.Activities.Remove(act);
                    activityBeingExecuted = act;
                }
                else
                {
                    //get the parent activity
                    //var tempAct = UseCaseCaptured.Activities.Where(a => a.Id == act.ParentId).SingleOrDefault();
                    var tempAct = GetChildLessActivity(act.ParentId);
                    if (tempAct != null)// && act.TargetApplication != null && !string.IsNullOrEmpty(act.TargetApplication.ApplicationExe))
                    {
                        args.Application = tempAct.TargetApplication.ApplicationExe.ToLower();
                        if (string.IsNullOrEmpty(tempAct.ParentId))
                            args.NewApplicationInstance = true;
                        else
                            args.NewApplicationInstance = false;
                        UseCaseCaptured.Activities.Remove(tempAct);
                        activityBeingExecuted = tempAct;
                    }
                    else //i.e. parent is already executed 
                    {
                        args.Application = act.TargetApplication.ApplicationExe.ToLower();
                        if (string.IsNullOrEmpty(act.ParentId))
                            args.NewApplicationInstance = true;
                        else
                            args.NewApplicationInstance = false;
                        UseCaseCaptured.Activities.Remove(act);
                        activityBeingExecuted = act;
                    }
                }

                //UseCaseCaptured.Activities.Remove(UseCaseCaptured.Activities.First());
                if (ReadyToExecute != null)
                    ReadyToExecute(args);
            }
            else if (UseCaseCaptured != null && UseCaseCaptured.Activities != null && UseCaseCaptured.Activities.Count == 0)
            {
                //this part is useful only for html based applications
                //raise the event to depict the completion of the execution of all the activities
                UseCaseExecutedEventArgs ucArgs = new UseCaseExecutedEventArgs();
                //populate the event args as intended
                if (_Browser != null)
                {
                    ucArgs.Data = ExtractDataFromHtml(_Browser.Document);
                }
                else
                    ucArgs.OtherInfo = "Web Browser instance is missing";
                if (UseCaseExecuted != null)
                    UseCaseExecuted(ucArgs);
            }
        }

        private Entities.Activity GetChildLessActivity(string parentId)
        {
            Entities.Activity act = new Entities.Activity();
            Entities.Activity tempAct = null;
            while (true)
            {
                act = UseCaseCaptured.Activities.Where(a => a.Id == parentId).SingleOrDefault();
                if (act == null)
                {
                    act = tempAct;
                    break;
                }
                else if (string.IsNullOrEmpty(act.ParentId))
                {
                    break;
                }
                else
                {
                    parentId = act.ParentId;
                    tempAct = act;
                }
            }
            return act;
        }

        /// <summary>
        /// The interface to get the list of application(s) associated with the usecase
        /// </summary>
        /// <param name="useCasePath">the path to the use case file. only XML file should be provided.</param>
        /// <returns>the list of string having paths to the application executable
        /// e.g.  C:\Windows\System32\notepad.exe</returns>
        public List<string> GetApplications(string useCasePath)
        {
            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.Load(useCasePath);
            UseCaseCaptured = SerializeAndDeserialize.Deserialize(xmlDoc.OuterXml, typeof(Entities.UseCase)) as Entities.UseCase;
            tempUseCaseCaptured = UseCaseCaptured;
            return GetAppList();
        }

        /// <summary>
        /// The interface to get the list of application(s) associated with the usecase. To be used if the use case details are
        /// provided using the property UseCaseCaptured
        /// </summary>
        /// <returns>the list of string having paths to the application executable
        /// e.g.  C:\Windows\System32\notepad.exe</returns>
        public List<string> GetApplications()
        {
            return GetAppList();
        }

        /// <summary>
        /// The interface to execute the application whose file path is provided as appPointer. Make sure that
        /// the UseCaseCaptured is passed to the constructor of the class- PlayActions
        /// </summary>
        /// <param name="appPointer">the path to the application executable</param>
        /// <param name="handle">the windows handle for the application in case process id is not enough, mainly
        /// used when the application is hosted internally to a different application</param>
        /// <param name="processId"> optional, the process id of the application hosted internally/externally</param>
        public void ExecuteActionsOn(string appPointer, IntPtr handle, int processId = 0)
        {
            _Browser = null;
            if (processId != 0)
                appWindow = GetAppWindow(processId);
            if (appWindow == null)
                appWindow = AutomationElement.FromHandle(handle);

            //List<Entities.Task> tasks = appToTaskMapping[appPointer.ToLower()] as List<Entities.Task>;
            List<Entities.Task> tasks = activityBeingExecuted.Tasks;//.OrderBy(t => t.Order).ToList(); 
            if (tasks != null)
            {
                tasks.ForEach(t =>
                {
                    numberOfTrialsCounter = 0;
                    executedTask = false;
                    while (numberOfTrialsCounter < MaxElementIdenficationTrials && !executedTask)
                    {
                        try
                        {
                            AutomateTask(t);
                        }
                        catch
                        {
                            //log exception                                
                        }
                        numberOfTrialsCounter++;
                        //wait for certain duration between subsequent trials
                        if (!executedTask)
                            System.Threading.Thread.Sleep(GapBetweenTrials);
                    }
                    System.Threading.Thread.Sleep(500);
                });
            }
            //AutomateTask(new Entities.Task());
            //check if there is any more activities to be executed
            InitiatePlayBack();
        }

        /// <summary>
        /// The interface to execute the application whose file path is provided as appPointer. Make sure that
        /// the UseCaseCaptured is passed to the constructor of the class- PlayActions
        /// Call this once the web browser has completed loading the web application
        /// </summary>
        /// <param name="appPointer">the path to the web application</param>
        /// <param name="browser">the browser instance in which the web application is loaded</param>
        public void ExecuteActionsOn(string appPointer, System.Windows.Forms.WebBrowser browser)
        {
            //if (executeTask)
            //{
            List<Entities.Task> tasks = activityBeingExecuted.Tasks;//.OrderBy(t=> t.Order).ToList();
            _Browser = browser;
            if (tasks != null)
            {
                tasks.ForEach(t =>
                {
                    numberOfTrialsCounter = 0;
                    executedTask = false;
                    while (numberOfTrialsCounter < MaxElementIdenficationTrials && !executedTask)
                    {
                        try
                        {
                            AutomateTask(t);
                        }
                        catch
                        {
                            //log exception                                
                        }
                        numberOfTrialsCounter++;
                        //wait for certain duration between subsequent trials
                        if (!executedTask)
                            System.Threading.Thread.Sleep(GapBetweenTrials);
                    }
                });
            }
            //check if there is any more activities to be executed
            InitiatePlayBack();
            //}
        }

        /// <summary>
        /// Interface to handle the execution of all the applications in an usecase. Make sure that
        /// the UseCaseCaptured is passed to the constructor of the class- PlayActions
        /// </summary>
        /// <param name="mappings">collection holding the app, app handle, app process id and the container tab handle</param>
        public void ExecuteActionsOn(List<Entities.TabToAppMapping> mappings)
        {
            mappings.ForEach(m =>
            {
                //first navigate to to the tab
                if (NavigateToTab(m))
                {
                    System.Threading.Thread.Sleep(700);
                    //execute tasks on the application in the tab
                    ExecuteActionsOn(m.AppPointer, m.AppHandle, m.AppProcessId);
                    System.Threading.Thread.Sleep(700);
                }
            });
        }

        //private methods
        private Entities.Activity GetFirstActivityAndRemoveUnknown()
        {
            Entities.Activity app = null;
            UseCaseCaptured.Activities = UseCaseCaptured.Activities.Where(a => !string.IsNullOrEmpty(a.TargetApplication.ApplicationExe)).ToList();
            if (UseCaseCaptured.Activities.Count > 0)
                app = UseCaseCaptured.Activities[0];
            return app;
        }

        private bool NavigateToTab(Entities.TabToAppMapping mapping)
        {
            bool isSuccess = true;
            try
            {
                AutomationElement tabItem = null, parentTab = null;
                if (mapping.TabItemHandle != IntPtr.Zero)
                    tabItem = AutomationElement.FromHandle(mapping.TabItemHandle);
                else if (mapping.BoundingRectangle != null)
                {
                    //first get the tab and then the tab item from the rectangle
                    parentTab = AutomationElement.FromHandle(mapping.TabHandle);
                    if (parentTab != null)
                    {
                        PropertyCondition boundingRectangleCondition = new PropertyCondition(AutomationElement.BoundingRectangleProperty, mapping.BoundingRectangle);
                        tabItem = parentTab.FindFirst(TreeScope.Descendants, boundingRectangleCondition);
                    }

                }
                if (tabItem != null)
                {
                    object pattern;
                    if (tabItem.TryGetCurrentPattern(SelectionItemPattern.Pattern, out pattern))
                    {
                        SelectionItemPattern selectPattern = pattern as SelectionItemPattern;
                        selectPattern.Select();
                    }
                }
            }
            catch (Exception ex)
            {
                //log exception
                isSuccess = false;
            }
            return isSuccess;
        }

        private void AutomateTask(Entities.Task task)
        {
            if (task.Name == Entities.TaskNames.PauseForInput.ToString())
            {
                HighlightAndPauseForInput(task);
                return;
            }
            switch (task.Event)
            {
                case Entities.EventTypes.MouseLeftClick:
                    if (task.ControlOnApplication == Entities.ApplicationTypes.WebApplication)
                    {
                        //automate html control event pattern
                        //AutomateHTMLPattern(task);
                        AutomateHTMLPattern2(task);
                    }
                    else //default
                        AutomatePattern(task);
                    break;
                case Entities.EventTypes.KeyboardKeyPress:
                    //if last automation element is not null and 
                    //1. of type text then assign the text
                    //2. also need to check if the key is some functional key then dont assign
                    if (_Browser != null)
                    {
                        //automate key in to html control
                        AutomateHTMLKeyIn(task);
                    }
                    else //default
                        AutomateKeyIn(task);
                    break;
            }
        }

        private void AutomateHTMLKeyIn(Entities.Task task)
        {
            if (lastHtmlElement != null)
            {
                lastKeyPress = task.TargetControlAttributes[1].Value;
                if (task.Name == "GroupedKeys")
                {
                    //first change the text to the uppper so that the right ascii is used to raise keyboard key press
                    task.TargetControlAttributes[1].Value = task.TargetControlAttributes[1].Value.ToUpper();
                    switch (lastHtmlElement.GetAttribute("type").ToLower())
                    {
                        case "text":                            
                            //HTMLInputTextElement text = lastHTMLElement as HTMLInputTextElement;
                            string lastValue = lastHtmlElement.GetAttribute("value");
                            if (!isACollectiveKey)
                                //if (Control.IsKeyLocked(Keys.CapsLock))
                                //    lastHtmlElement.SetAttribute("value", lastValue + task.TargetControlAttributes[1].Value.ToUpper());
                                //else
                                //    lastHtmlElement.SetAttribute("value", lastValue + task.TargetControlAttributes[1].Value.ToLower());
                                AutomateStringKeyIn(task.TargetControlAttributes[1].Value);
                            else
                            {
                                //get the first key to be associated with the collective key
                                //int fistKey = int.Parse(task.TargetControlAttributes[2].Value.Split(',')[0]);
                                int firstKey = task.TargetControlAttributes[1].Value[0];
                                AutomateSpecialKeyIn(collectiveKeyValue);
                                AutomateSpecialKeyIn(firstKey);
                                AutomateSpecialKeyIn(collectiveKeyValue, false);
                                isACollectiveKey = false;
                                //then the remaining text
                                string str = task.TargetControlAttributes[1].Value.Substring(1, task.TargetControlAttributes[1].Value.Length - 1);
                                //if (Control.IsKeyLocked(Keys.CapsLock))
                                //    lastHtmlElement.SetAttribute("value", (lastValue + str).ToUpper());
                                //else
                                //    lastHtmlElement.SetAttribute("value", (lastValue + str).ToLower());
                                //last value is not needed as currently keyboard key press event is fired
                                AutomateStringKeyIn(str);
                            }
                            break;
                        //case "select-one": //will be handled by the section handling the special key presses
                        //    switch (task.TargetControlAttributes[1].Value.ToLower())
                        //    {
                        //        case "down":
                        //        case "right":
                        //            lastHtmlElement.SetAttribute("selectedIndex", (int.Parse(lastHtmlElement.GetAttribute("selectedIndex")) + 1).ToString());
                        //            break;
                        //        case "up":
                        //        case "left":
                        //            lastHtmlElement.SetAttribute("selectedIndex", (int.Parse(lastHtmlElement.GetAttribute("selectedIndex")) - 1).ToString());
                        //            break;
                        //    }
                        //    break;
                    }
                }
                else if (task.Name == "CollectiveKey")
                {
                    isACollectiveKey = true;
                    collectiveKeyValue = int.Parse(task.TargetControlAttributes[2].Value);
                }
                else
                    AutomateSpecialKeyIn(int.Parse(task.TargetControlAttributes[2].Value));
                executedTask = true;
            }
            Application.DoEvents();
            System.Threading.Thread.Sleep(500);
        }

        private void AutomateKeyIn(Entities.Task task)
        {
            if (lastElement != null)
            {
                //isACollectiveKey = false;
                lastKeyPress = task.TargetControlAttributes[1].Value;
                if (task.Name == "GroupedKeys")
                {
                    //first change the text to the uppper so that the right ascii is used to raise keyboard key press
                    task.TargetControlAttributes[1].Value = task.TargetControlAttributes[1].Value.ToUpper();
                    switch (lastElement.Current.ControlType.ProgrammaticName)
                    {
                        case "ControlType.Document":
                            lastElement.SetFocus();
                            System.Threading.Thread.Sleep(100);
                            if (!isACollectiveKey)
                                //if(Control.IsKeyLocked(Keys.CapsLock))
                                //    SendKeys.SendWait(task.TargetControlAttributes[1].Value.ToUpper());
                                //else
                                //    SendKeys.SendWait(task.TargetControlAttributes[1].Value.ToLower());
                                AutomateStringKeyIn(task.TargetControlAttributes[1].Value);
                            else
                            {
                                //int fistKey = int.Parse(task.TargetControlAttributes[2].Value.Split(',')[0]);
                                int firstKey = task.TargetControlAttributes[1].Value[0];
                                AutomateSpecialKeyIn(collectiveKeyValue);
                                AutomateSpecialKeyIn(firstKey);
                                AutomateSpecialKeyIn(collectiveKeyValue, false);
                                isACollectiveKey = false;
                                //then the remaining text
                                string str = task.TargetControlAttributes[1].Value.Substring(1, task.TargetControlAttributes[1].Value.Length - 1);
                                //if (Control.IsKeyLocked(Keys.CapsLock))
                                //    SendKeys.SendWait(str.ToUpper());
                                //else
                                //    SendKeys.SendWait(str.ToLower());
                                AutomateStringKeyIn(str);
                            }
                            break;
                        case "ControlType.Text":
                        case "ControlType.Edit":
                            //first get the existing text (if any)
                            object objTextPattern;
                            string lastText = "";
                            if (lastElement.TryGetCurrentPattern(TextPattern.Pattern, out objTextPattern))
                            {
                                TextPattern textPattern = objTextPattern as TextPattern;
                                lastText = textPattern.DocumentRange.GetText(-1);
                            }
                            ValuePattern valuePattern = lastElement.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
                            if (!isACollectiveKey)
                                //if (Control.IsKeyLocked(Keys.CapsLock))
                                //    valuePattern.SetValue(lastText + task.TargetControlAttributes[1].Value.ToUpper());
                                //else
                                //    valuePattern.SetValue(lastText + task.TargetControlAttributes[1].Value.ToLower());
                                AutomateStringKeyIn(task.TargetControlAttributes[1].Value);
                            else
                            {
                                //int fistKey = int.Parse(task.TargetControlAttributes[2].Value.Split(',')[0]);
                                int firstKey = task.TargetControlAttributes[1].Value[0];
                                AutomateSpecialKeyIn(collectiveKeyValue);
                                AutomateSpecialKeyIn(firstKey);
                                AutomateSpecialKeyIn(collectiveKeyValue, false);
                                isACollectiveKey = false;
                                //then the remaining text
                                string str = task.TargetControlAttributes[1].Value.Substring(1, task.TargetControlAttributes[1].Value.Length - 1);
                                //if (Control.IsKeyLocked(Keys.CapsLock))
                                //    valuePattern.SetValue((lastText + str).ToUpper());
                                //else
                                //    valuePattern.SetValue((lastText + str).ToLower());
                                AutomateStringKeyIn(str);
                            }
                            break;
                    }
                }
                else if (task.Name == "CollectiveKey")
                {
                    isACollectiveKey = true;
                    collectiveKeyValue = int.Parse(task.TargetControlAttributes[2].Value);
                }
                else
                    AutomateSpecialKeyIn(int.Parse(task.TargetControlAttributes[2].Value));
                executedTask = true;
            }
            Application.DoEvents();
            System.Threading.Thread.Sleep(500);
        }

        private void TryGettingWebPageFromAutomationElement(out SHDocVw.WebBrowser page)
        {
            page = null;
            AutomationElement htmlDocPanel = appWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ClassNameProperty, "Internet Explorer_Server"));
            if (htmlDocPanel != null)
            {
                Control ctl = Control.FromHandle((IntPtr)htmlDocPanel.Current.NativeWindowHandle);
                page = ctl as SHDocVw.WebBrowser;
            }
        }

        private HtmlElement GetHtmlElementfrom(string xPath, string id)
        {
            if (_Browser == null)
                return null;

            HtmlElement element = null;
            try
            {

                string[] tags = xPath.Split('/');
                for (int i = 0; i < tags.Length; i++)
                {
                    if (!string.IsNullOrEmpty(tags[i]))
                    {
                        //check if index is present
                        int index = 0;
                        string tagName = "";
                        if (tags[i].Contains('['))
                        {
                            index = int.Parse(tags[i].Substring(tags[i].IndexOf('[') + 1, tags[i].IndexOf(']') - tags[i].IndexOf('[') - 1));
                            tagName = tags[i].Substring(0, tags[i].IndexOf('['));
                        }
                        else
                            tagName = tags[i];
                        if (tagName.ToLower() == "iframe")
                        { }

                        if (element == null) //only when the tag is html
                            element = _Browser.Document.GetElementsByTagName(tagName)[index];
                        else
                        {
                            int tagTracker = -1;
                            if (element.TagName.ToLower() == "iframe")
                            {
                                foreach (HtmlWindow frame in _Browser.Document.Window.Frames)
                                {
                                    try
                                    {
                                        if (frame.WindowFrameElement == element)
                                        {
                                            HtmlElement tempel = frame.Document.GetElementsByTagName(tagName)[index];
                                            if (tempel != null)
                                                element = tempel;
                                            break;
                                        }
                                    }
                                    catch (System.UnauthorizedAccessException)
                                    {
                                        //when the source of the iframe belongs to a different domain
                                        //actions on such part has to be done manually
                                    }
                                }
                            }
                            else
                                for (int j = 0; j < element.Children.Count; j++)
                                {
                                    if (element.Children[j].TagName == tagName)
                                        tagTracker++;
                                    if (tagTracker == index)
                                    {
                                        element = element.Children[j];
                                        break;
                                    }
                                }
                            //the folowing approach will not work as GetElementsByTagName
                            //gets all the child elements at all the levels, i.e. not just the first level
                            //element = element.GetElementsByTagName(tagName)[index];
                        }
                    }
                }
            }
            catch
            {
                //log exception
                element = null;
            }

            //double check, if the html element identified is the right one or not
            if (element != null)
            {
                if (!string.IsNullOrEmpty(id) && element.Id != id)
                    element = null;
            }
            return element;
        }

        private void AutomateHTMLPattern(Entities.Task task)
        {
            if (appWindow != null)
            {
                //appWindow.Current.NativeWindowHandle shud be same as the handle received from work bench
                //to use the following, for the dll- Interop.SHDocVw.dll (Microsoft Internet Controls)
                //change the "Embed Interop Types" to False
                var webPage = new SHDocVw.ShellWindowsClass().Cast<SHDocVw.WebBrowser>().Where(wb => wb.HWND == appWindow.Current.NativeWindowHandle).SingleOrDefault();

                //or try to find if from the appwindow automation element if we can get the web browser instance
                if (webPage == null)
                    TryGettingWebPageFromAutomationElement(out webPage);

                if (webPage != null)
                {
                    mshtml.HTMLDocument doc = webPage.Document as mshtml.HTMLDocument;
                    mshtml.IHTMLDocument2 doc2 = doc as mshtml.IHTMLDocument2;

                    foreach (IHTMLElement element in doc2.all)
                    {
                        if (element.sourceIndex == task.SourceIndex)
                        {
                            lastHTMLElement = element;
                            switch (task.ControlType)
                            {
                                case "submit":
                                    HTMLInputButtonElement button = element as HTMLInputButtonElement;
                                    button.click();
                                    break;
                                case "checkbox":
                                    HTMLInputElement check = element as HTMLInputElement;
                                    check.setAttribute("checked", bool.Parse(task.CurrentState));
                                    break;
                                case "radio":
                                    HTMLInputElement radio = element as HTMLInputElement;
                                    radio.setAttribute("checked", true);
                                    break;
                                case "text":
                                    HTMLInputTextElement text = element as HTMLInputTextElement;
                                    text.focus();
                                    break;
                                default:
                                    element.click();
                                    break;
                            }
                            break;
                        }
                    }
                }
            }
        }

        private void AutomateHTMLPattern2(Entities.Task task)
        {
            if (_Browser != null && !string.IsNullOrEmpty(task.XPath))
            {
                if (_Browser.InvokeRequired)
                {
                    _Browser.BeginInvoke(new Action<Entities.Task>(AutomateHTMLPattern2), new object[] { task });
                    return;
                }

                HtmlElement element = null;
                element = GetHtmlElementfrom(task.XPath, task.ControlId);
                lastHtmlElement = element;
                if (element != null)
                {
                    switch (task.TriggeredPattern)
                    {
                        case "Invoke":
                            element.InvokeMember("click");
                            break;
                        case "Check":
                            element.SetAttribute("checked", task.CurrentState);
                            break;
                        case "Radio":
                            element.SetAttribute("checked", "true");
                            break;
                        case "Focus":
                            element.InvokeMember("focus");
                            break;
                    }
                    executedTask = true;
                }
                else
                    executedTask = false;
            }
        }

        private void AutomatePattern(Entities.Task task)
        {
            if (appWindow != null)
            {
                //get the automation element
                //also populate the last element from this method
                AutomationElement element = null;
                if (!string.IsNullOrEmpty(task.ControlId) && !string.IsNullOrEmpty(task.AccessKey))
                {
                    PropertyCondition automationIdCondition = new PropertyCondition(AutomationElement.AutomationIdProperty, task.ControlId);
                    PropertyCondition accessKeyCondition = new PropertyCondition(AutomationElement.AccessKeyProperty, task.AccessKey);
                    AndCondition andCondition = new AndCondition(automationIdCondition, accessKeyCondition);
                    element = appWindow.FindFirst(TreeScope.Descendants, andCondition);
                }
                else if (!string.IsNullOrEmpty(task.ControlId))
                {
                    PropertyCondition automationIdCondition = new PropertyCondition(AutomationElement.AutomationIdProperty, task.ControlId);
                    element = appWindow.FindFirst(TreeScope.Descendants, automationIdCondition);
                }
                else if (!string.IsNullOrEmpty(task.AccessKey))
                {
                    PropertyCondition accessKeyCondition = new PropertyCondition(AutomationElement.AccessKeyProperty, task.AccessKey);
                    element = appWindow.FindFirst(TreeScope.Descendants, accessKeyCondition);
                }
                else if(!string.IsNullOrEmpty(task.ControlName))
                {
                    PropertyCondition nameCondition = new PropertyCondition(AutomationElement.NameProperty, task.ControlName);
                    element = appWindow.FindFirst(TreeScope.Descendants, nameCondition);
                }
                else if (!string.IsNullOrEmpty(task.ApplictionTreePath))
                {
                    //get the element from the application tree path
                    element = GetElementFromAppTreePath(task.ApplictionTreePath);
                }
                if (element == null)
                {
                    executedTask = false;
                    return;
                }
                lastElement = element;

                object pattern;
                switch (task.TriggeredPattern)
                {
                    case "InvokePatternIdentifiers.Pattern":
                        //double check
                        if (element.Current.ControlType.ProgrammaticName == "ControlType.Button") //add others having same pattern
                        {
                            if (element.TryGetCurrentPattern(InvokePattern.Pattern, out pattern))
                            {
                                InvokePattern invokePattern = pattern as InvokePattern;
                                invokePattern.Invoke();
                            }
                        }
                        break;
                    case "TogglePatternIdentifiers.Pattern":
                        if (element.TryGetCurrentPattern(TogglePattern.Pattern, out pattern))
                        {
                            TogglePattern togglePattern = pattern as TogglePattern;
                            if (bool.Parse(task.CurrentState))
                            {
                                //check the check box
                                if (togglePattern.Current.ToggleState == ToggleState.Off)
                                    togglePattern.Toggle();
                            }
                            else
                            {
                                //uncheck the check box
                                if (togglePattern.Current.ToggleState == ToggleState.On)
                                    togglePattern.Toggle();
                            }
                        }
                        break;
                    case "SelectionItemPatternIdentifiers.Pattern":
                        if (element.TryGetCurrentPattern(SelectionItemPattern.Pattern, out pattern))
                        {
                            if (bool.Parse(task.CurrentState))
                            {
                                //select the radio button, item from dropdown
                                SelectionItemPattern selectionItemPattern = pattern as SelectionItemPattern;
                                selectionItemPattern.Select();
                            }
                        }
                        break;
                    case "ValuePatternIdentifiers.Pattern":
                        //just set the focus
                        TrySettingFocus(element);
                        break;
                    case "ExpandCollapsePatternIdentifiers.Pattern, InvokePatternIdentifiers.Pattern":
                        //first check if the expand-collapse pattern is supported then invoke pattern
                        if (element.TryGetCurrentPattern(ExpandCollapsePattern.Pattern, out pattern))
                        {
                            ExpandCollapsePattern expPattern = pattern as ExpandCollapsePattern;
                            expPattern.Expand();
                        }
                        else if (element.TryGetCurrentPattern(InvokePattern.Pattern, out pattern))
                        {
                            InvokePattern invPattern = pattern as InvokePattern;
                            invPattern.Invoke();
                        }
                        break;
                    case "ExpandCollapsePatternIdentifiers.Pattern, SelectionPatternIdentifiers.Pattern":
                        //first check if the expand-collapse pattern is supported then selection pattern
                        if (element.TryGetCurrentPattern(ExpandCollapsePattern.Pattern, out pattern))
                        {
                            ExpandCollapsePattern expPattern = pattern as ExpandCollapsePattern;
                            expPattern.Expand();
                        }
                        //else if (element.TryGetCurrentPattern(SelectionPattern.Pattern, out pattern))
                        //{
                        //    SelectionPattern selectPattern = pattern as SelectionPattern;
                        //commented because there is no action to be called
                        //this patten is mainly used to identify a automation element with some poperty condition
                        //like is multiple selection allowed, etc
                        //}
                        break;
                }
                executedTask = true;
            }
        }

        private void TrySettingFocus(AutomationElement element)
        {
            //this approach is needed as setfocus works very differently
            try
            {
                element.SetFocus();
            }
            catch { }
        }

        private AutomationElement GetAppWindow(int processId)
        {
            //PropertyCondition typeCondition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window);
            //PropertyCondition processIdCondition = new PropertyCondition(AutomationElement.ProcessIdProperty, processId);
            //AndCondition andCondition = new AndCondition(typeCondition, processIdCondition);
            //return AutomationElement.RootElement.FindFirst(TreeScope.Children, andCondition);
            IntPtr handle = Process.GetProcessById(processId).MainWindowHandle;
            if (handle != IntPtr.Zero)
                return AutomationElement.FromHandle(Process.GetProcessById(processId).MainWindowHandle);
            else
                return null;
        }

        private List<string> GetAppList()
        {
            List<string> apps = new List<string>();
            if (UseCaseCaptured != null && UseCaseCaptured.Activities != null)
            {
                UseCaseCaptured.Activities.ForEach(act =>
                {
                    if (!string.IsNullOrEmpty(act.TargetApplication.ApplicationExe))
                    {
                        if (!apps.Contains(act.TargetApplication.ApplicationExe.ToLower()))
                        {
                            apps.Add(act.TargetApplication.ApplicationExe.ToLower());
                            //dont simply add the list as done below, this will lead to the shallow copy problem
                            //List<Entities.Task> tasks = act.Tasks;
                            //appToTaskMapping.Add(act.TargetApplication.ApplicationExe.ToLower(), tasks);
                        }
                        //else
                        //{
                        //    (appToTaskMapping[act.TargetApplication.ApplicationExe.ToLower()] as List<Entities.Task>).AddRange(act.Tasks);
                        //}
                    }
                });
            }
            return apps;
        }

        private void AutomateSpecialKeyIn(int value, bool down = true)
        {
            System.Byte bValue = 0;
            if (System.Byte.TryParse(value.ToString(), out bValue))
            {
                switch (bValue)
                {
                    case 160:
                    case 161:
                    case 162:
                    case 163:
                        if (down)
                            keybd_event(bValue, 0, KEYEVENTF_KEYDOWN, 0);
                        else
                            keybd_event(bValue, 0, KEYEVENTF_KEYUP, 0);
                        break;
                    default:
                        keybd_event(bValue, 0, KEYEVENTF_KEYDOWN, 0);
                        keybd_event(bValue, 0, KEYEVENTF_KEYUP, 0);
                        break;
                }
            }
            #region previous switch

            //switch (value)
            //{
            //    //case 20:
            //    //    keybd_event(KEYEVENTF_CAPSLOCK, 0, KEYEVENTF_KEYDOWN, 0);
            //    //    keybd_event(KEYEVENTF_CAPSLOCK, 0, KEYEVENTF_KEYUP, 0);
            //    //    break;
            //    //case 32:
            //    //    keybd_event(KEYEVENTF_SPACE, 0, KEYEVENTF_KEYDOWN, 0);
            //    //    keybd_event(KEYEVENTF_SPACE, 0, KEYEVENTF_KEYUP, 0);
            //    //    break;
            //    //case 8:
            //    //    keybd_event(KEYEVENTF_BACK, 0, KEYEVENTF_KEYDOWN, 0);
            //    //    keybd_event(KEYEVENTF_BACK, 0, KEYEVENTF_KEYUP, 0);
            //    //    break;
            //    //case 9:
            //    //    keybd_event(KEYEVENTF_TAB, 0, KEYEVENTF_KEYDOWN, 0);
            //    //    keybd_event(KEYEVENTF_TAB, 0, KEYEVENTF_KEYUP, 0);
            //    //    break;
            //    //case 27:
            //    //    keybd_event(KEYEVENTF_ESCAPE, 0, KEYEVENTF_KEYDOWN, 0);
            //    //    keybd_event(KEYEVENTF_ESCAPE, 0, KEYEVENTF_KEYUP, 0);
            //    //    break;
            //    //case 44:
            //    //    keybd_event(KEYEVENTF_PRINTSCREEN, 0, KEYEVENTF_KEYDOWN, 0);
            //    //    keybd_event(KEYEVENTF_PRINTSCREEN, 0, KEYEVENTF_KEYUP, 0);
            //    //    break;
            //    //case 38:
            //    //    keybd_event(KEYEVENTF_UP, 0, KEYEVENTF_KEYDOWN, 0);
            //    //    keybd_event(KEYEVENTF_UP, 0, KEYEVENTF_KEYUP, 0);
            //    //    break;
            //    //case 40:
            //    //    keybd_event(KEYEVENTF_DOWN, 0, KEYEVENTF_KEYDOWN, 0);
            //    //    keybd_event(KEYEVENTF_DOWN, 0, KEYEVENTF_KEYUP, 0);
            //    //    break;
            //    //case 37:
            //    //    keybd_event(KEYEVENTF_LEFT, 0, KEYEVENTF_KEYDOWN, 0);
            //    //    keybd_event(KEYEVENTF_LEFT, 0, KEYEVENTF_KEYUP, 0);
            //    //    break;
            //    //case 39:
            //    //    keybd_event(KEYEVENTF_RIGHT, 0, KEYEVENTF_KEYDOWN, 0);
            //    //    keybd_event(KEYEVENTF_RIGHT, 0, KEYEVENTF_KEYUP, 0);
            //    //    break;
            //    //case 46:
            //    //    keybd_event(KEYEVENTF_DELETE, 0, KEYEVENTF_KEYDOWN, 0);
            //    //    keybd_event(KEYEVENTF_DELETE, 0, KEYEVENTF_KEYUP, 0);
            //    //    break;
            //    //case 160:
            //    //case 161:
            //    //case 162:
            //    //case 163:
            //    //    if (down)
            //    //        keybd_event(KEYEVENTF_LSHIFT, 0, KEYEVENTF_KEYDOWN, 0);
            //    //    else
            //    //        keybd_event(KEYEVENTF_LSHIFT, 0, KEYEVENTF_KEYUP, 0);
            //    //    break;
            //    //default:
            //    //    System.Byte bValue = 0;
            //    //    if (System.Byte.TryParse(value.ToString(), out bValue))
            //    //    {
            //    //        keybd_event(bValue, 0, KEYEVENTF_KEYDOWN, 0);
            //    //        keybd_event(bValue, 0, KEYEVENTF_KEYUP, 0);
            //    //    }
            //    //    break;
            //}
            #endregion
        }

        private void AutomateStringKeyIn(string stringToBeKeyed)
        {
            if (!string.IsNullOrEmpty(stringToBeKeyed))
            {
                foreach (char c in stringToBeKeyed)
                {
                    switch (c)
                    {
                        case '.':
                            AutomateSpecialKeyIn(190);
                            break;
                        case ',':
                            AutomateSpecialKeyIn(188);
                            break;
                        case '-':
                            AutomateSpecialKeyIn(189);
                            break;
                        default:
                            AutomateSpecialKeyIn(c);
                            break;
                    }

                }
            }
        }

        private List<Entities.NameValueAtribute> ExtractDataFromHtml(HtmlDocument doc)
        {
            List<Entities.NameValueAtribute> data = new List<Entities.NameValueAtribute>();
            //get all the input kind of element of type 'text'
            foreach (HtmlElement element in doc.GetElementsByTagName("input"))
            {
                if (element.GetAttribute("type").ToLower() == "text")
                    data.Add(new Entities.NameValueAtribute() { Name = element.Id, Value = element.GetAttribute("value") });
            }
            //get all the textarea element
            foreach (HtmlElement element in doc.GetElementsByTagName("textarea"))
            {
                data.Add(new Entities.NameValueAtribute() { Name = element.Id, Value = element.InnerText });
            }
            //get selection from all the dropdown element
            foreach (HtmlElement element in doc.GetElementsByTagName("select"))
            {
                foreach (HtmlElement child in element.All)
                {
                    if (child.GetAttribute("tagName").ToLower() == "option")
                    {
                        if (child.GetAttribute("selected").ToLower() == "true")
                        {
                            data.Add(new Entities.NameValueAtribute() { Name = element.Id, Value = child.GetAttribute("text") });
                            break;
                        }
                    }
                }
            }

            //get the similar details or the document inside any iframe
            foreach (HtmlWindow frame in doc.Window.Frames)
            {
                try
                {
                    data.AddRange(ExtractDataFromHtml(frame.Document));
                }
                catch (System.UnauthorizedAccessException)
                {
                    //when the source of the iframe belongs to a different domain
                }
            }
            return data;
        }

        private void HighlightAndPauseForInput(Entities.Task task)
        {
            //first try highlighting the control
            //and then popup a modal window to collect input

            //for html element
            if (task.ControlOnApplication == Entities.ApplicationTypes.WebApplication && lastHtmlElement != null)
            {
                lastHtmlElement.Style += ";background-color: rgb(165, 150, 238);";
                Input textInput = new Input();
                textInput.ShowDialog();
                string userText = textInput.TextInput;
                if (!string.IsNullOrEmpty(userText))
                {
                    task.TargetControlAttributes.Add(new Entities.NameValueAtribute() { Name = "KeyCode", Value = "" });
                    task.TargetControlAttributes.Add(new Entities.NameValueAtribute() { Name = "KeyData", Value = userText.ToUpper() });
                    task.TargetControlAttributes.Add(new Entities.NameValueAtribute() { Name = "KeyValue", Value = "" });
                    task.Name = Entities.TaskNames.GroupedKeys.ToString();
                    AutomateHTMLKeyIn(task);
                }
                executedTask = true;
            }
            //for winform kind of element
            else if(lastElement != null)
            {
                //Process proc = Process.GetProcessById(lastElement.Current.ProcessId);
                //var form = (Form)(Control.FromHandle(appWindow.Current.NativeWindowHandle));

                Input textInput = new Input();
                textInput.ShowDialog();
                string userText = textInput.TextInput;
                if (!string.IsNullOrEmpty(userText))
                {
                    task.TargetControlAttributes.Add(new Entities.NameValueAtribute() { Name = "KeyCode", Value = "" });
                    task.TargetControlAttributes.Add(new Entities.NameValueAtribute() { Name = "KeyData", Value = userText.ToUpper() });
                    task.TargetControlAttributes.Add(new Entities.NameValueAtribute() { Name = "KeyValue", Value = "" });
                    task.Name = Entities.TaskNames.GroupedKeys.ToString();
                    AutomateKeyIn(task);
                }
                executedTask = true;
            }
        }

        private AutomationElement GetElementFromAppTreePath (string appTreePath)
        {
            AutomationElement tempElement = appWindow;
            string[] pathParts = appTreePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (pathParts.Length > 0)
            {
                for (int i = 1; i < pathParts.Length; i++)
                {
                    //starting from second path part i.e. i=1 because the appWindow corresponds to the the first path part i.e. 0[0]
                    if (tempElement != null)
                    {
                        string[] sts = pathParts[i].Substring(0, pathParts[i].Length - 1).Split('[');
                        int index = int.Parse(sts[1]);
                        AutomationElementCollection children = tempElement.FindAll(TreeScope.Children, Condition.TrueCondition);
                        if (index < children.Count)
                            tempElement = children[index];
                        else
                            tempElement = null;
                    }
                }
            }
            else
                tempElement = null;
            return tempElement;
        }
    }
}

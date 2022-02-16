/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Configuration;

using System.Management;
using System.Net;
using System.Net.NetworkInformation;


namespace Infosys.ATR.UIAutomation.SEE
{
    public class RecordActions
    {
        //public properties
        /// <summary>
        /// To get the complete list of tasks captured so far
        /// </summary>
        public List<Entities.Task> TasksTracked { get; set; }

        //public List<Tracking.KeyBoardKeyPresedEventArgs> EleminateKeyPresedTask { get; set; }

        public bool IsSendKeyPressed { get; set; }
        public int Height
        {
            get { return Tracking.Height; }
            set { Tracking.Height = value; }
        }
        public int Width
        {
            get { return Tracking.Width; }
            set { Tracking.Width = value; }
        }
        public string TaskId
        {
            get { return Tracking.TaskId; }
            set { Tracking.TaskId = value; }
        }

        static WebBrowser _Browser;
        /// <summary>
        /// To assign the browser instance to be used to extract html element details. 
        /// Also make sure to nullify te browser when the form/control hosting the browser object is closed.
        /// </summary>
        public WebBrowser Browser
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

        public Entities.UseCase UseCase { get { return useCase; } }

        //public ObservableCollection<Entities.Task> TasksTracked { get; set; }

        //private properties
        public Entities.UseCase useCase;
        bool trakingEvent = false;
        class oemChar
        {
            public bool IsOemCode { get; set; }
            public string Character { get; set; }
        }

        //event and delegate
        public delegate void HTMLContentEventHandler();
        /// <summary>
        /// Event raised when HTML content is detected
        /// </summary>
        public event HTMLContentEventHandler HTMLContent;

        public class AnyUserEventArgs : EventArgs
        {
            public string EventDesc { get; set; }
            public string EventId { get; set; }
        }
        public delegate void AnyUserEventHandler(AnyUserEventArgs e);
        /// <summary>
        /// Event raised when any user action happens like mouse click, keyboard press, etc. 
        /// </summary>
        public event AnyUserEventHandler AnyUserEvent;

        // public bool pause;


        //public interface

        /// <summary>
        /// Interface to delete any captured task
        /// </summary>
        /// <param name="taskId">Id of the task to be deleted</param>
        public void DeleteTask(string taskId)
        {
            bool found = false;
            foreach (var act in useCase.Activities)
            {
                if (found)
                    break;
                //foreach (var task in act.Tasks)
                //{
                //    if (task.Id == taskId)
                //    {
                //        found = true;
                //        act.Tasks.Remove(task);
                //        //also if there are any keyborad press with the same group script id,
                //        //then for these keyboard presses, the group script id needs to be changed to immediate previous click event if any
                //        break;
                //    }
                //}
                for (int i = 0; i < act.Tasks.Count; i++)
                {
                    if (act.Tasks[i].Id == taskId)
                    {
                        found = true;
                        string oldScriptId = act.Tasks[i].GroupScriptId;
                        if (act.Tasks.Remove(act.Tasks[i]))
                            //also if there are any keyborad press with the same group script id,
                            //then for these keyboard presses, the group script id needs to be changed to immediate previous click event if any
                            RefreshGroupScriptId(i, act, oldScriptId);
                        break;
                    }
                }
            }
        }

        private void RefreshGroupScriptId(int deletedTaskIndex, Entities.Activity act, string oldScriptId)
        {
            //check if any previous task exists
            if (deletedTaskIndex == 0)
                return;
            foreach (var activity in useCase.Activities)
            {
                if (act == activity)
                {
                    if (act.Tasks[deletedTaskIndex - 1].Event == Entities.EventTypes.MouseDoubleClick || act.Tasks[deletedTaskIndex - 1].Event == Entities.EventTypes.MouseLeftClick || act.Tasks[deletedTaskIndex - 1].Event == Entities.EventTypes.MouseMiddleClick || act.Tasks[deletedTaskIndex - 1].Event == Entities.EventTypes.MouseRightClick)
                    {
                        string newScriptId = act.Tasks[deletedTaskIndex - 1].GroupScriptId;
                        for (int i = 0; i < act.Tasks.Count; i++)
                        {
                            if (act.Tasks[i].GroupScriptId == oldScriptId)
                                act.Tasks[i].GroupScriptId = newScriptId;
                        }
                    }
                    else
                        RefreshGroupScriptId(deletedTaskIndex - 1, act, oldScriptId);
                }
            }
        }

        /// <summary>
        /// Reset wait default interval to user defined value
        /// </summary>
        /// <param name="interval"></param>
        public void UpdateWait(string interval, string taskId)
        {
            bool found = false;
            foreach (var act in useCase.Activities)
            {
                if (found)
                    break;
                foreach (var task in act.Tasks)
                {
                    if (task.Id == taskId)
                    {
                        found = true;
                        var temp = act.Tasks.FirstOrDefault(t => t.Id == taskId);
                        temp.Description = "Wait(" + interval + ")";
                        temp.TargetControlAttributes[0].Value = interval;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// The Interface to be called to start recording the user actions. Recording will toggle if called again.
        /// </summary>
        /// <param name="start">true to start and false to stop</param>
        /// <param name="height">the optional height of the image to be taken on click event</param>
        /// <param name="width">the optional width of the image to be taken on click event</param>
        /// <returns>when asked to stop recording, the complete recorded data is serialized and returned back</returns>
        public string Start(bool start = true, int height = 0, int width = 0)
        {
            string serializedData = "";
            if (start && !trakingEvent)
            {
                serializedData = "";
                //TasksTracked = new ObservableCollection<Entities.Task>();
                TasksTracked = new List<Entities.Task>();
                trakingEvent = true;
                useCase = new Entities.UseCase()
                {
                    CreatedBy = Environment.UserName,
                    MachineName = Environment.MachineName,
                    MachineType = Environment.Is64BitOperatingSystem ? "64 Bit Operating System" : "32 Bit Operating System",
                    Domain = Environment.UserDomainName,
                    OSVersion = Environment.OSVersion.VersionString,
                    OS = GetOSName(),
                    MachineIP = GetMachineIp(),
                    ScreenResolution = GetScreenResolution()
                };
                useCase.Id = Guid.NewGuid().ToString();
                useCase.CreatedOn = DateTime.Now;
                useCase.Activities = new List<Entities.Activity>();

                Tracking.RecordKeyboardKeydown(start);
                Tracking.RecordMouseClick(start, height, width);
                Tracking.RecordMouseDoubleClick(start, height, width);
                Tracking.KeyBoardKeyPresed += new Tracking.KeyBoardKeyPresedEventHandler(Tracking_KeyBoardKeyPresed);
                Tracking.MouseClicked += new Tracking.MouseClickedEventHandler(Tracking_MouseClicked);
                Tracking.IsSendKey += new Tracking.IsSendKeyHandler(Tracking_IsSendKey);
            }
            else
            {
                trakingEvent = false;
                Tracking.RecordKeyboardKeydown(start);
                Tracking.RecordMouseClick(start);
                Tracking.RecordMouseDoubleClick(start);
                Tracking.KeyBoardKeyPresed -= new Tracking.KeyBoardKeyPresedEventHandler(Tracking_KeyBoardKeyPresed);
                Tracking.MouseClicked -= new Tracking.MouseClickedEventHandler(Tracking_MouseClicked);
                Tracking.IsSendKey -= new Tracking.IsSendKeyHandler(Tracking_IsSendKey);
                //update the usecase to group all key presses asociated with the a control in a single task 
                GroupKeyPresses();
                //sanitize the usecase captured for double click i.e. remove the corresponding single clicks
                SanitizeForDoubleClicks();
                //roder the tasks in an activities
                for (int i = 0; i < useCase.Activities.Count; i++)
                {
                    useCase.Activities[i].Tasks = useCase.Activities[i].Tasks.OrderBy(t => t.Order).ToList();
                }
                serializedData = SerializeAndDeserialize.Serialize(useCase);
            }
            return serializedData;
        }

        private void Tracking_IsSendKey(bool e)
        {
            IsSendKeyPressed = e;
        }

        public void GroupKeyPresses(Entities.UseCase usecase)
        {
            usecase.Activities.ForEach(act =>
            {
                var groupedTasks = act.Tasks.GroupBy(t => t.GroupScriptId);
                foreach (var group in groupedTasks)
                {
                    string keyCode = "", keyData = "", keyValue = "", scriptGroup = "", windowTitle = "";
                    int parentOrder = 0;
                    List<Entities.Task> summaryCollection = new List<Entities.Task>();
                    //TBD- maintain a collection to create more than one summary
                    //when the special keys are pressed in between e.g. r,a,h,u,l,caps lock, b,a,n,d => rahulBAND
                    //i.e. first summary task for "rahul" 
                    //then a task for "caps lock"
                    //and then second summary task for "band"
                    foreach (var task in group)
                    {
                        if (task.Event == Entities.EventTypes.KeyboardKeyPress)
                        {
                            //exclude those keys of special meaning like "space", etc
                            if (IsInAllowedRanges(int.Parse(task.TargetControlAttributes[2].Value)))
                            {
                                var digit = GetDigit(int.Parse(task.TargetControlAttributes[2].Value));
                                keyCode += task.TargetControlAttributes[0].Value + ",";
                                if (digit != null)
                                    keyData += digit;
                                else
                                    keyData += ConvertOemToChar(task.TargetControlAttributes[1].Value).Character;
                                keyValue += task.TargetControlAttributes[2].Value + ",";
                                act.Tasks.Remove(task);
                            }
                            else //i.e. some special key
                            {
                                //if there is any data for summary, then create one collective summary and add to the summary collection
                                //and then add this task to the summary collection
                                if (keyData != "")
                                {
                                    Entities.Task tempTask = new Entities.Task() { GroupScriptId = scriptGroup, Order = parentOrder + 1, WindowTitle = windowTitle };
                                    tempTask.Id = System.Guid.NewGuid().ToString();
                                    tempTask.Name = "GroupedKeys";
                                    tempTask.Description = keyData + "- keys are pressed from Keyboard";
                                    tempTask.Event = Entities.EventTypes.KeyboardKeyPress;
                                    tempTask.TargetControlAttributes = new List<Entities.NameValueAtribute>();
                                    tempTask.TargetControlAttributes.Add(new Entities.NameValueAtribute() { Name = "KeyCode", Value = keyCode });
                                    tempTask.TargetControlAttributes.Add(new Entities.NameValueAtribute() { Name = "KeyData", Value = keyData });
                                    tempTask.TargetControlAttributes.Add(new Entities.NameValueAtribute() { Name = "KeyValue", Value = keyValue });
                                    summaryCollection.Add(tempTask);
                                    //then reset data
                                    keyCode = keyData = keyValue = "";
                                }
                                //and then add the task for the special key
                                parentOrder = task.Order;
                                if (IsACollectiveKey(int.Parse(task.TargetControlAttributes[2].Value)))
                                    task.Name = "CollectiveKey";
                                summaryCollection.Add(task);
                                act.Tasks.Remove(task);

                            }
                        }
                        else if (task.Event == Entities.EventTypes.MouseLeftClick || task.Event == Entities.EventTypes.MouseRightClick)
                        {
                            parentOrder = task.Order;
                            scriptGroup = task.GroupScriptId;
                            windowTitle = task.WindowTitle;
                        }
                    }
                    if (keyData != "")
                    {
                        Entities.Task tempTask = new Entities.Task() { GroupScriptId = scriptGroup, Order = parentOrder + 1, WindowTitle = windowTitle };
                        tempTask.Id = System.Guid.NewGuid().ToString();
                        tempTask.Name = "GroupedKeys";
                        tempTask.Description = keyData + "- keys are pressed from Keyboard";
                        tempTask.Event = Entities.EventTypes.KeyboardKeyPress;
                        tempTask.TargetControlAttributes = new List<Entities.NameValueAtribute>();
                        tempTask.TargetControlAttributes.Add(new Entities.NameValueAtribute() { Name = "KeyCode", Value = keyCode });
                        tempTask.TargetControlAttributes.Add(new Entities.NameValueAtribute() { Name = "KeyData", Value = keyData });
                        tempTask.TargetControlAttributes.Add(new Entities.NameValueAtribute() { Name = "KeyValue", Value = keyValue });
                        summaryCollection.Add(tempTask);
                    }
                    if (summaryCollection.Count > 0)
                        act.Tasks.AddRange(summaryCollection);
                }
            });
        }

        public void Wait()
        {
            UpdateUseCase(Tracking.AddWaitEvent());
        }

        public void Pause()
        {
            Tracking.RecordKeyboardKeydown(false);
            Tracking.RecordMouseClick(false);
            Tracking.RecordMouseDoubleClick(false);
        }

        public void Resume()
        {
            Tracking.RecordKeyboardKeydown(true);
            Tracking.RecordMouseClick(true);
            Tracking.RecordMouseDoubleClick(true);
        }

        void Tracking_MouseClicked(Tracking.MouseClickedEventArgs e)
        {
            //if (!pause)
            {
                try
                {
                    //add the tasks to the TasksTracked abd also the main object- usecase->activities
                    UpdateUseCase(e);
                }
                catch { }
            }
        }

        void Tracking_KeyBoardKeyPresed(Tracking.KeyBoardKeyPresedEventArgs e)
        {
            if (IsSendKeyPressed)
                return;
            //if ((e.Task.Name.Equals("LControlKey") && e.Task.TargetControlAttributes[2].Value.Equals("162")) ||
            //    e.Task.Name.Equals("L") && e.Task.TargetControlAttributes[2].Value.Equals("76"))
            //{
            //    if (EleminateKeyPresedTask == null)
            //        EleminateKeyPresedTask = new List<Tracking.KeyBoardKeyPresedEventArgs>();

            //    if (EleminateKeyPresedTask.Count >= 0 && EleminateKeyPresedTask.Count < 2)
            //    {
            //        EleminateKeyPresedTask.Add(e);

            //        if (EleminateKeyPresedTask.Count == 2)
            //        {
            //            EleminateKeyPresedTask.Clear();
            //            EleminateKeyPresedTask = null;
            //        }
            //    }
            //}
            //else
            //  if (!pause)
            {
                try
                {
                    //if (EleminateKeyPresedTask != null)
                    //{
                    //    if (EleminateKeyPresedTask.Count == 1)
                    //    {
                    //        UpdateUseCase(EleminateKeyPresedTask[0]);
                    //        EleminateKeyPresedTask.Clear();
                    //        EleminateKeyPresedTask = null;
                    //    }
                    //}
                    //add the tasks to the TasksTracked abd also the main object- usecase->activities
                    UpdateUseCase(e);
                }
                catch { }
            }
        }

        void UpdateUseCase(Tracking.BaseTaskArguements e)
        {
            //if (e.IsHTMLContent)
            //{
            //    //check if the app is html based, raise the event
            //    if (HTMLContent != null)
            //    {
            //        HTMLContent();
            //        //remove return once the html content handling is done as then also the use case needs to updated

            //        //return will happen only if the web-browser is null
            //        if (Browser == null)
            //            return;
            //    }
            //}

            //check if the task info is null then return
            if (e.Task == null || TasksTracked == null)
                return;
            e.Task.CreatedOn = DateTime.Now;
            TasksTracked.Add(e.Task);
            //if (AnyUserEvent != null)
            AnyUserEvent(new AnyUserEventArgs() { EventDesc = e.Task.Description, EventId = e.Task.Id });

            if (useCase.Activities.Count == 0)
            {
                Entities.ActivityIdDetails act = Entities.ManageActivity.GetActivityId(e.ProcessId);
                useCase.Activities.Add(new Entities.Activity()
                {
                    Id = act.Id,
                    ParentId = act.ParentId,
                    Name = GetDefaultActivityName(),
                    CreatedOn = DateTime.Now
                });
                useCase.Activities[0].Description = e.AdditionalInfo;
                useCase.Activities[0].Tasks = new List<Entities.Task>();
                useCase.Activities[0].Tasks.Add(e.Task);

                useCase.Activities[0].TargetApplication = new Entities.ApplicationDetails();
                useCase.Activities[0].TargetApplication.ApplicationType = e.ApplicationType;
                useCase.Activities[0].TargetApplication.ApplicationExe = e.FileName;
                useCase.Activities[0].TargetApplication.TargetApplicationAttributes = new List<Entities.NameValueAtribute>();
                useCase.Activities[0].TargetApplication.TargetApplicationAttributes.Add(new Entities.NameValueAtribute() { Name = "ApplicationName", Value = e.ApplicationName });
                useCase.Activities[0].TargetApplication.TargetApplicationAttributes.Add(new Entities.NameValueAtribute() { Name = "FileName", Value = e.FileName });
                useCase.Activities[0].TargetApplication.TargetApplicationAttributes.Add(new Entities.NameValueAtribute() { Name = "ModuleName", Value = e.ModuleName });
            }
            else if (!e.IsDifferentActivity)
            {
                useCase.Activities[useCase.Activities.Count - 1].Tasks.Add(e.Task);
            }
            else if (e.IsDifferentActivity)
            {
                Entities.ActivityIdDetails act = Entities.ManageActivity.GetActivityId(e.ProcessId);
                useCase.Activities.Add(new Entities.Activity()
                {
                    Id = act.Id,
                    ParentId = act.ParentId,
                    Name = GetDefaultActivityName(),
                    CreatedOn = DateTime.Now
                });
                useCase.Activities[useCase.Activities.Count - 1].Description = e.AdditionalInfo;
                useCase.Activities[useCase.Activities.Count - 1].Tasks = new List<Entities.Task>();
                useCase.Activities[useCase.Activities.Count - 1].Tasks.Add(e.Task);

                useCase.Activities[useCase.Activities.Count - 1].TargetApplication = new Entities.ApplicationDetails();
                useCase.Activities[useCase.Activities.Count - 1].TargetApplication.ApplicationType = e.ApplicationType;
                useCase.Activities[useCase.Activities.Count - 1].TargetApplication.ApplicationExe = e.FileName;
                useCase.Activities[useCase.Activities.Count - 1].TargetApplication.TargetApplicationAttributes = new List<Entities.NameValueAtribute>();
                useCase.Activities[useCase.Activities.Count - 1].TargetApplication.TargetApplicationAttributes.Add(new Entities.NameValueAtribute() { Name = "ApplicationName", Value = e.ApplicationName });
                useCase.Activities[useCase.Activities.Count - 1].TargetApplication.TargetApplicationAttributes.Add(new Entities.NameValueAtribute() { Name = "FileName", Value = e.FileName });
                useCase.Activities[useCase.Activities.Count - 1].TargetApplication.TargetApplicationAttributes.Add(new Entities.NameValueAtribute() { Name = "ModuleName", Value = e.ModuleName });
            }
        }

        void SanitizeForDoubleClicks()
        {
            string location = ConfigurationManager.AppSettings["TaskImageLocation"];
            useCase.Activities.ForEach(act =>
            {
                List<Entities.Task> tasks = new List<Entities.Task>();
                for (int i = 0; i < act.Tasks.Count; i++)
                {
                    tasks.Add(act.Tasks[i]);
                    if (act.Tasks[i].Name == Entities.EventTypes.MouseDoubleClick.ToString())
                    {
                        //then remove previous two tasks
                        string imageLoc1 = "", imageLoc2 = "";
                        if (tasks.Count - 2 > 0 && tasks[tasks.Count - 2].ApplictionTreePath == act.Tasks[i].ApplictionTreePath)
                            imageLoc1 = location + @"\" + tasks[tasks.Count - 2].Id + ".jpg";
                        if (tasks.Count - 3 > 0 && tasks[tasks.Count - 3].ApplictionTreePath == act.Tasks[i].ApplictionTreePath)
                            imageLoc2 = location + @"\" + tasks[tasks.Count - 3].Id + ".jpg";
                        if (tasks.Count - 2 > 0 && !string.IsNullOrEmpty(imageLoc1))
                            tasks.RemoveAt(tasks.Count - 2);
                        if (tasks.Count - 2 > 0 && !string.IsNullOrEmpty(imageLoc2))
                            tasks.RemoveAt(tasks.Count - 2);

                        //remove the corresponding images (if any)                        
                        if (!string.IsNullOrEmpty(location))
                        {
                            if (System.IO.File.Exists(imageLoc1))
                                System.IO.File.Delete(imageLoc1);
                            if (System.IO.File.Exists(imageLoc2))
                                System.IO.File.Delete(imageLoc2);
                        }
                    }
                }
                act.Tasks = tasks;
            });
        }

        void GroupKeyPresses()
        {
            useCase.Activities.ForEach(act =>
            {
                var groupedTasks = act.Tasks.GroupBy(t => t.GroupScriptId);
                foreach (var group in groupedTasks)
                {
                    string keyCode = "", keyData = "", keyValue = "", scriptGroup = "", windowTitle = "";
                    int parentOrder = 0;
                    List<Entities.Task> summaryCollection = new List<Entities.Task>();
                    //to track the keypress task created time
                    DateTime taskTime = new DateTime();

                    //TBD- maintain a collection to create more than one summary
                    //when the special keys are pressed in between e.g. r,a,h,u,l,caps lock, b,a,n,d => rahulBAND
                    //i.e. first summary task for "rahul" 
                    //then a task for "caps lock"
                    //and then second summary task for "band"
                    foreach (var task in group)
                    {
                        if (task.Event == Entities.EventTypes.KeyboardKeyPress)
                        {
                            ////to track the keypress task created time
                            //DateTime taskTime = new DateTime();
                            //exclude those keys of special meaning like "space", etc
                            if (IsInAllowedRanges(int.Parse(task.TargetControlAttributes[2].Value)))
                            {
                                taskTime = task.CreatedOn;
                                var digit = GetDigit(int.Parse(task.TargetControlAttributes[2].Value));
                                keyCode += task.TargetControlAttributes[0].Value + ",";
                                if (digit != null)
                                    keyData += digit;
                                else
                                    keyData += ConvertOemToChar(task.TargetControlAttributes[1].Value).Character;
                                keyValue += task.TargetControlAttributes[2].Value + ",";
                                act.Tasks.Remove(task);
                            }
                            else //i.e. some special key
                            {
                                //if there is any data for summary, then create one collective summary and add to the summary collection
                                //and then add this task to the summary collection
                                if (keyData != "")
                                {
                                    Entities.Task tempTask = new Entities.Task() { GroupScriptId = scriptGroup, Order = parentOrder + 1, WindowTitle = windowTitle, TriggeredPattern = "KeyPress", ControlName = "Text Area", ControlType = "text area", CreatedOn = taskTime };
                                    tempTask.Id = System.Guid.NewGuid().ToString();
                                    tempTask.Name = "GroupedKeys";
                                    tempTask.Description = keyData + "- keys are pressed from Keyboard";
                                    tempTask.Event = Entities.EventTypes.KeyboardKeyPress;
                                    tempTask.TargetControlAttributes = new List<Entities.NameValueAtribute>();
                                    tempTask.TargetControlAttributes.Add(new Entities.NameValueAtribute() { Name = "KeyCode", Value = keyCode });
                                    tempTask.TargetControlAttributes.Add(new Entities.NameValueAtribute() { Name = "KeyData", Value = keyData });
                                    tempTask.TargetControlAttributes.Add(new Entities.NameValueAtribute() { Name = "KeyValue", Value = keyValue });
                                    summaryCollection.Add(tempTask);
                                    //then reset data
                                    keyCode = keyData = keyValue = "";
                                }
                                //and then add the task for the special key
                                parentOrder = task.Order;
                                if (IsACollectiveKey(int.Parse(task.TargetControlAttributes[2].Value)))
                                    task.Name = "CollectiveKey";
                                summaryCollection.Add(task);
                                act.Tasks.Remove(task);

                            }
                        }
                        else if (task.Event == Entities.EventTypes.MouseLeftClick || task.Event == Entities.EventTypes.MouseRightClick || task.Event == Entities.EventTypes.MouseDoubleClick || task.Event == Entities.EventTypes.MouseMiddleClick)
                        {
                            parentOrder = task.Order;
                            scriptGroup = task.GroupScriptId;
                            windowTitle = task.WindowTitle;
                        }
                    }
                    if (keyData != "")
                    {
                        Entities.Task tempTask = new Entities.Task() { GroupScriptId = scriptGroup, Order = parentOrder + 1, WindowTitle = windowTitle, TriggeredPattern = "KeyPress", ControlName = "Text Area", ControlType = "text area", CreatedOn = taskTime };
                        tempTask.Id = System.Guid.NewGuid().ToString();
                        tempTask.Name = "GroupedKeys";
                        tempTask.Description = keyData + "- keys are pressed from Keyboard";
                        tempTask.Event = Entities.EventTypes.KeyboardKeyPress;
                        tempTask.TargetControlAttributes = new List<Entities.NameValueAtribute>();
                        tempTask.TargetControlAttributes.Add(new Entities.NameValueAtribute() { Name = "KeyCode", Value = keyCode });
                        tempTask.TargetControlAttributes.Add(new Entities.NameValueAtribute() { Name = "KeyData", Value = keyData });
                        tempTask.TargetControlAttributes.Add(new Entities.NameValueAtribute() { Name = "KeyValue", Value = keyValue });
                        summaryCollection.Add(tempTask);
                    }
                    if (summaryCollection.Count > 0)
                        act.Tasks.AddRange(summaryCollection);
                }
            });
        }

        bool IsInAllowedRanges(int value)
        {
            bool isInRange = false;
            //add the range intended in the from(s) and to(s) arrays
            //counts of items in froms and tos have to be same
            int[] froms = { 48, 65, 97, 190, 188, 189 }; //0,A,a,dot,comma, minus
            int[] tos = { 57, 90, 122, 190, 188, 189 }; //9,Z,z,dot,comma, minus
            for (int i = 0; i < froms.Length; i++)
            {
                if (value >= froms[i] && value <= tos[i])
                {
                    isInRange = true;
                    break;
                }
            }
            return isInRange;
        }

        bool IsACollectiveKey(int value)
        {
            bool isCollective = false;
            List<int> collectiveKeys = new List<int>() { 160, 161, 162, 163 }; //e.g. LShiftKey(160), LControlKey(162), etc
            isCollective = collectiveKeys.Contains(value);
            return isCollective;
        }

        int? GetDigit(int value)
        {
            int number;
            if (int.TryParse(Convert.ToChar(value).ToString(), out number))
                return number;
            else
                return null;
        }

        oemChar ConvertOemToChar(string oemCode)
        {
            oemChar oemchar = new oemChar() { IsOemCode = false, Character = oemCode };
            switch (oemCode.ToLower())
            {
                case "oemperiod":
                    oemchar.IsOemCode = true;
                    oemchar.Character = ".";
                    break;
                case "oemcomma":
                    oemchar.IsOemCode = true;
                    oemchar.Character = ",";
                    break;
                case "oemminus":
                    oemchar.IsOemCode = true;
                    oemchar.Character = "-";
                    break;
            }
            return oemchar;
        }

        string GetDefaultActivityName()
        {
            string name = "Activity- ";
            if (useCase != null && useCase.Activities != null)
            {
                int newCount = useCase.Activities.Count + 1;
                name += newCount.ToString();
            }
            return name;
        }

        string GetOSName()
        {
            string osName = "";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");
            foreach (ManagementObject os in searcher.Get())
            {
                osName = os["Caption"].ToString();
                break;
            }
            return osName;
        }

        string GetMachineIp()
        {
            string ip = "";
            //the below returns just IPv6
            //Ping ping = new Ping();
            //PingReply reply = ping.Send(Environment.MachineName);
            //if (reply.Status == IPStatus.Success)
            //{
            //    ip = reply.Address.ToString();
            //}

            //this below code will get all the assigned ipv4 and ipv6 addresses
            IPHostEntry hostEntry = Dns.GetHostEntry(Environment.MachineName);
            hostEntry.AddressList.ToList().ForEach(ad =>
            {
                if (ip == "")
                    ip = ad.ToString();
                else
                    ip += ", " + ad.ToString();
            });
            return ip;
        }

        string GetScreenResolution()
        {
            string res = "";
            res = SystemInformation.PrimaryMonitorSize.Width.ToString() + " x " + SystemInformation.PrimaryMonitorSize.Height.ToString();
            return res;
        }


    }
}

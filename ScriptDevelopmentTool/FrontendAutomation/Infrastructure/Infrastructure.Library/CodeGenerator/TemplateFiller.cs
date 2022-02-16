/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infosys.Solutions.CodeGeneration.Framework;

namespace IMSWorkBench.Infrastructure.Library.CodeGenerator
{
    /// <summary>
    /// Filler class to generate sikuli code based on Templates\sikulitemplate.txt   
    /// </summary>
    public class Filler : ContentProvider
    {
        Activity _a;
        StringBuilder app = new StringBuilder();

        public Filler(Activity a)
        {
            //Hi,there !
            _a = a;
        }

        [PlaceHolder("app")]
        public string App
        {
            get
            {
                AppFiller appTemplate = new AppFiller(_a);
                appTemplate.ContentTemplate = ContentTemplate.RepeatingTemplate("appTemplate");
                TaskFiller._repeatingTemplate = ContentTemplate.RepeatingTemplate("taskTemplate");
                EventFiller._repeatingTemplate = ContentTemplate.RepeatingTemplate("eventTemplate");
                app.Append(appTemplate.GenerateContent());
                return app.ToString();
            }
        }
    }

    public class AppFiller : ContentProvider
    {
        Activity _a;
        internal static Template _repeatingTemplate;
        StringBuilder tasks = new StringBuilder();

        public AppFiller(Activity a)
        {
            _a = a;
            ContentTemplate = _repeatingTemplate;
        }

        [PlaceHolder("application")]
        public string Application
        {
            get
            {
                return _a.Application;
            }
        }

        [PlaceHolder("tasks")]
        public string Tasks
        {
            get
            {
                _a.Tasks.ForEach(t =>
                {
                    TaskFiller taskTemplate = new TaskFiller(t);
                    tasks.Append(taskTemplate.GenerateContent());
                });

                return tasks.ToString();
            }
        }
    }

    public class TaskFiller : ContentProvider
    {
        private Task _t;

        internal static Template _repeatingTemplate;

        public TaskFiller(Task t)
        {
            _t = t;
            ContentTemplate = _repeatingTemplate;
        }

        [PlaceHolder("task")]
        public string Task
        {
            get
            {
                EventFiller d = new EventFiller(_t);
                return d.GenerateContent();
                //string e = d.GenerateContent();
                //char[] lineconstants = {'\r','\t','\n'};
                //e = e.TrimStart(lineconstants);
                //e = e.TrimEnd(lineconstants);
                //return e;
            }
        }

    }

    public class EventFiller : ContentProvider
    {

        private Task _t;
        internal static Template _repeatingTemplate;

        public EventFiller(Task t)
        {
            _t = t;
            ContentTemplate = _repeatingTemplate;

        }


        [PlaceHolder("eventType")]
        public string EventType
        {
            get
            {
                //temporary fix
                if (Sikuli.Mapper[_t.TaskName] == null)
                    return "#<Not mapped>";
                return Sikuli.Mapper[_t.TaskName];
            }
        }

        [PlaceHolder("eventParameter")]
        public string EventParameter
        {
            get
            {
                return _t.TaskInfo;
            }
        }
    }

    public class Sikuli
    {
        public static System.Collections.Specialized.NameValueCollection Mapper =
            new System.Collections.Specialized.NameValueCollection();

        static Sikuli()
        {
            Mapper.Add("MouseLeftClick", "click");
            Mapper.Add("MouseDoubleClick", "doubleClick");
            Mapper.Add("MouseRightClick", "rightClick");
            Mapper.Add("Wait", "sleep");
            Mapper.Add("GroupedKeys", "");
            Mapper.Add("CollectiveKey", "");
            Mapper.Add("Space", "type");
            Mapper.Add("Return", "keyDown");
            Mapper.Add("Tab", "");
            Mapper.Add("LMenu", "");
        }

    }
}

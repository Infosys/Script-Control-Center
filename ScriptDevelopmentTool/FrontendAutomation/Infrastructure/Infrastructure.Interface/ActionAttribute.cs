/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;

namespace IMSWorkBench.Infrastructure.Interface
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ActionAttribute : Attribute
    {
        private string _actionName;

        public ActionAttribute(string actionName)
        {
            _actionName = actionName;
        }

        public string ActionName
        {
            get { return _actionName; }
            set { _actionName = value; }
        }
    }
}

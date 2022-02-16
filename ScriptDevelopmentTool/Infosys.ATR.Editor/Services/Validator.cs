/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Infosys.ATR.Editor.Services
{
    internal class Validator
    {
        internal static void Validate(object obj)
        {
            var properties = obj.GetType().GetProperties();

            foreach (var p in properties)
            {
                var attributes = p.GetCustomAttributes(false);

                var attr = attributes.FirstOrDefault(a => a.GetType() == typeof(RequiredAttribute));

                if (attr == null)
                    continue;
                else
                {
                    var value = p.GetValue(obj, null) as String;
                    if (String.IsNullOrEmpty(value))
                        throw new ArgumentNullException(p.Name);
                }
            }

        }
            
    }
}

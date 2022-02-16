/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Automation;
using System.Windows.Automation.Text;
using System.Threading;

namespace DetectElement
{
    class Program
    {
        static void Main(string[] args)
        {
            do
            {
                System.Drawing.Point mouse = System.Windows.Forms.Cursor.Position; // use Windows forms mouse code instead of WPF
                AutomationElement element = AutomationElement.FromPoint(new System.Windows.Point(mouse.X, mouse.Y));
                if (element == null)
                {
                    // no element under mouse
                    return;
                }

                Console.WriteLine("Element at position " + mouse + " is '" + element.Current.Name + "'");

                object pattern;
                // the "Value" pattern is supported by many application (including IE & FF)
                if (element.TryGetCurrentPattern(ValuePattern.Pattern, out pattern))
                {
                    ValuePattern valuePattern = (ValuePattern)pattern;
                    Console.WriteLine(" Value=" + valuePattern.Current.Value);
                }

                // the "Text" pattern is supported by some applications (including Notepad)and returns the current selection for example
                if (element.TryGetCurrentPattern(TextPattern.Pattern, out pattern))
                {
                    TextPattern textPattern = (TextPattern)pattern;
                    foreach (TextPatternRange range in textPattern.GetSelection())
                    {
                        Console.WriteLine(" SelectionRange=" + range.GetText(-1));
                    }
                }
                Thread.Sleep(1000);
                Console.WriteLine(); Console.WriteLine();
            }
            while (true);

        }
    }
}

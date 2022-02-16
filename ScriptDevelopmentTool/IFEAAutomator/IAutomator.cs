/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageBasedAutomator
{
    public interface IAutomator
    {
        Region find(String filename, int timeout, double confidence);
        Region find(String filename);
        Region wait(String filename);
        Region exists(String filename, int timeout, double confidence);
        List<Region> findAllImages(List<String> files, int timeout, double confidence);
        Region findAnyImage(List<String> files, int timeout, double confidence);
        List<Region> findAllInstances(String filename, int timeout, double confidence);
        void openApp(String appName);
        void paste(String text);
        void paste();
        void selectAll();
        void copy();
        string read();
        void clearClipboard();
        void type(String text, params int[] modifiers);
        void sleep(int time);
        void highlight();
        bool multipleScaleMatching { get; set; }
    }
}

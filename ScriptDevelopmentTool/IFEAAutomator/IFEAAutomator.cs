/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Drawing;
using System.Diagnostics;

namespace ImageBasedAutomator
{
    public class IFEAAutomator : IAutomator
    {
        #region Constants and private variables
        const double SCALE_STEP = 0.5;
        const int MAX_SCALE_STEPS = 20;
        const bool MULTIPLE_SCALE = true;
        const int DEFAULT_TIMEOUT = 10;
        const int THREAD_SLEEP_DURATION = 100;
        const int DEFAULT_CONFIDENCE = 80;

        bool _multipleScaleMatching = true, _highlightElement = false;
        #endregion

        #region IAutomator Members

        /// <summary>
        /// The method tries to identify the provided template in the screen shot to be taken.
        /// And then accordingly returns the region. It uses multiple scale template matching.
        /// </summary>
        /// <param name="filename">The file name depicting the template image file</param>
        /// <param name="timeout">The period within which if the element is not identified, it throws a exception. Unit- Seconds</param>
        /// <param name="confidence">The minimum confidence depicint the threshold of the image match. Value should be in the range - 1-100</param>
        /// <returns>The Region object if match is found</returns>
        public Region find(string filename, int timeout, double confidence)
        {
            Region elementRegion = null;
            DateTime startTime = DateTime.Now;
            try
            {
                if (timeout <= 0)
                    timeout = DEFAULT_TIMEOUT; //i.e. assigning the default time period to say 10 sec
                if (System.IO.File.Exists(filename))
                {
                    Image<Gray, byte> template = new Image<Gray, byte>(filename);
                    Point[] box = null;
                    while ((System.DateTime.Now - startTime).TotalMilliseconds <= timeout * 1000 && box == null)
                    {
                        Image<Gray, byte> source = Utilities.GetGrayScreenShot();
                        if (_multipleScaleMatching)
                        {
                            //scale up/down the template
                            int direction = 1;
                            for (int i = 0; i <= MAX_SCALE_STEPS; i++)
                            {
                                //scale up and verify for confidence
                                double scale = 1 + direction * i * SCALE_STEP;
                                Image<Gray, byte> templateTemp = ResizeTemplate(template, scale);
                                box = GetElementBox(source, templateTemp, confidence);
                                if (box != null)
                                {
                                    elementRegion = new Region(box, confidence);
                                    elementRegion.setImages(templateTemp, source);
                                    break;
                                }

                                //then scale down and verify for confidence
                                scale = 1 + (-direction) * i * SCALE_STEP;
                                templateTemp = ResizeTemplate(template, scale);
                                box = GetElementBox(source, templateTemp, confidence);
                                if (box != null)
                                {
                                    elementRegion = new Region(box, confidence);
                                    elementRegion.setImages(templateTemp, source);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            box = GetElementBox(source, template, confidence);
                            if (box != null)
                            {
                                elementRegion = new Region(box, confidence);
                                elementRegion.setImages(template, source);
                            }
                        }
                        System.Threading.Thread.Sleep(THREAD_SLEEP_DURATION);
                    }
                    if (box == null)
                        HandleErrors("Failed to match the template provided.");
                }
                else
                    HandleErrors(string.Format("File- {0} doesnt exist.", filename));
            }
            catch (Exception ex)
            {
                HandleErrors(ex);
            }
            if (elementRegion != null && _highlightElement)
            {
                elementRegion.highlight();
            }

            return elementRegion;
        }

        /// <summary>
        /// The method tries to identify the provided template in the screen shot to be taken.
        /// And then accordingly returns the region. It uses only original scale template matching for default confidence.
        /// </summary>
        /// <param name="filename">The file name depicting the template image file</param>
        /// <returns>The Region object if match is found</returns>
        public Region find(string filename)
        {
            Region elementRegion = null;
            int confidence = DEFAULT_CONFIDENCE;
            try
            {
                if (System.IO.File.Exists(filename))
                {
                    Image<Gray, byte> template = new Image<Gray, byte>(filename);
                    Image<Gray, byte> source = Utilities.GetGrayScreenShot();
                    Point[] box = GetElementBox(source, template, confidence);
                    if (box != null)
                    {
                        elementRegion = new Region(box, confidence);
                        elementRegion.setImages(template, source);
                    }
                    //Point[] box = null;
                    //if (_multipleScaleMatching)
                    //{
                    //    //scale up/down the template
                    //    int direction = 1;
                    //    for (int i = 0; i <= MAX_SCALE_STEPS; i++)
                    //    {
                    //        //scale up and verify for confidence
                    //        double scale = 1 + direction * i * SCALE_STEP;
                    //        Image<Gray, byte> templateTemp = ResizeTemplate(template, scale);
                    //        box = GetElementBox(source, templateTemp, confidence);
                    //        if (box != null)
                    //        {
                    //            elementRegion = new Region(box, confidence);
                    //            elementRegion.setImages(templateTemp, source);
                    //            break;
                    //        }

                    //        //then scale down and verify for confidence
                    //        scale = 1 + (-direction) * i * SCALE_STEP;
                    //        templateTemp = ResizeTemplate(template, scale);
                    //        box = GetElementBox(source, templateTemp, confidence);
                    //        if (box != null)
                    //        {
                    //            elementRegion = new Region(box, confidence);
                    //            elementRegion.setImages(templateTemp, source);
                    //            break;
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    box = GetElementBox(source, template, confidence);
                    //    if (box != null)
                    //    {
                    //        elementRegion = new Region(box, confidence);
                    //        elementRegion.setImages(template, source);
                    //    }
                    //}
                }
                else
                    HandleErrors(string.Format("File- {0} doesnt exist.", filename));
            }
            catch (Exception ex)
            {
                HandleErrors(ex);
            }

            if (elementRegion != null && _highlightElement)
            {
                elementRegion.highlight();
            }
            return elementRegion;
        }

        /// <summary>
        /// The method keeps trying to identify the provided template in the screen shot to be taken, until a valid match is found.
        /// </summary>
        /// <param name="filename">The file name depicting the template image file</param>
        /// <returns>The Region object if match is found</returns>
        public Region wait(string filename)
        {
            Region elementRegion = null;
            while (elementRegion == null)
            {
                try
                {
                    elementRegion = find(filename,DEFAULT_TIMEOUT,DEFAULT_CONFIDENCE);
                }
                catch
                {
                    //just kill the exception and keep trying
                }
                System.Threading.Thread.Sleep(THREAD_SLEEP_DURATION);
            }
            return elementRegion;
        } 

        /// <summary>
        /// Serves the same purpose as 'find' except it returns null rather than throwing an exception in the unhappy scenario(s)
        /// Deprecated, instead use find
        /// </summary>
        /// <param name="filename">The file name depicting the template image file</param>
        /// <param name="timeout">The period within which if the element is not identified, it throws a exception. Unit- Seconds</param>
        /// <param name="confidence">The minimum confidence depicint the threshold of the image match. Value should be in the range - 1-100</param>
        /// <returns>The Region object if match is found</returns>
        public Region exists(string filename, int timeout, double confidence)
        {
            Region elementRegion = null;
            try
            {
                elementRegion = find(filename, timeout, confidence);
            }
            catch (Exception ex)
            {
                //log exception if needed
            }
            return elementRegion;
        }

        /// <summary>
        /// The interface tries to identify allthe provided template in the screen shot to be taken.
        /// If any is not found, raises an exception.
        /// </summary>
        /// <param name="files">The file names depicting all the template image files</param>
        /// <param name="timeout">The period within which if the element is not identified, it throws a exception. Unit- Seconds</param>
        /// <param name="confidence">The minimum confidence depicint the threshold of the image match. Value should be in the range - 1-100</param>
        /// <returns>The List of Region object if all matches are found</returns>
        public List<Region> findAllImages(List<string> files, int timeout, double confidence)
        {
            List<Region> elementRegions = new List<Region>();
            Region elementRegion = null;
            DateTime startTime = DateTime.Now;
            bool confidenceAchieved = false; //once the confidence is achieved, the existing screen shot is used, otherwise scrren shopt is again taken.
            try
            {
                if (timeout <= 0)
                    timeout = DEFAULT_TIMEOUT; //i.e. assigning the default time period to say 10 sec
                foreach (string filename in files)
                {
                    if (System.IO.File.Exists(filename))
                    {
                        Image<Gray, byte> template = new Image<Gray, byte>(filename);
                        Point[] box = null;
                        Image<Gray, byte> source = null;
                        while ((System.DateTime.Now - startTime).TotalMilliseconds <= timeout * 1000 && box == null)
                        {
                            if (!confidenceAchieved || source == null)
                                source = Utilities.GetGrayScreenShot();
                            if (_multipleScaleMatching)
                            {
                                //scale up/down the template
                                int direction = 1;
                                for (int i = 0; i <= MAX_SCALE_STEPS; i++)
                                {
                                    //scale up and verify for confidence
                                    double scale = 1 + direction * i * SCALE_STEP;
                                    Image<Gray, byte> templateTemp = ResizeTemplate(template, scale);
                                    box = GetElementBox(source, templateTemp, confidence);
                                    if (box != null)
                                    {
                                        elementRegion = new Region(box, confidence);
                                        elementRegion.setImages(templateTemp, source);
                                        elementRegions.Add(elementRegion);
                                        confidenceAchieved = true;
                                        break;
                                    }

                                    //then scale down and verify for confidence
                                    scale = 1 + (-direction) * i * SCALE_STEP;
                                    templateTemp = ResizeTemplate(template, scale);
                                    box = GetElementBox(source, templateTemp, confidence);
                                    if (box != null)
                                    {
                                        elementRegion = new Region(box, confidence);
                                        elementRegion.setImages(templateTemp, source);
                                        elementRegions.Add(elementRegion);
                                        confidenceAchieved = true;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                box = GetElementBox(source, template, confidence);
                                if (box != null)
                                {
                                    elementRegion = new Region(box, confidence);
                                    elementRegion.setImages(template, source);
                                    elementRegions.Add(elementRegion);
                                    confidenceAchieved = true;
                                }
                            }
                            System.Threading.Thread.Sleep(THREAD_SLEEP_DURATION);
                        }
                        if (box == null)
                            HandleErrors("Failed to match at least one of the templates provided.");
                    }
                    else
                    {
                        HandleErrors(string.Format("File- {0} doesnt exist.", filename));
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                HandleErrors(ex);
            }
            return elementRegions;
        }

        /// <summary>
        /// Finds any one image out of the list of images (templates) and returns the region that holds the bounding box for that image.
        /// </summary>
        /// <param name="files">The file names depicting all the template image files</param>
        /// <param name="timeout">The period within which if the element is not identified, it throws a exception. Unit- Seconds</param>
        /// <param name="confidence">The minimum confidence depicint the threshold of the image match. Value should be in the range - 1-100</param>
        /// <returns>A region object that stores the match result for the first match found</returns>
        public Region findAnyImage(List<string> files, int timeout, double confidence)
        {
            Region elementRegion = null;
            DateTime startTime = DateTime.Now;
            try
            {
                if (timeout <= 0)
                    timeout = DEFAULT_TIMEOUT; //i.e. assigning the default time period to say 10 sec
                while ((System.DateTime.Now - startTime).TotalMilliseconds <= timeout * 1000)
                {
                    Image<Gray, byte> source = Utilities.GetGrayScreenShot();
                    foreach (string filename in files)
                    {
                        if (System.IO.File.Exists(filename))
                        {
                            Image<Gray, byte> template = new Image<Gray, byte>(filename);
                            if (_multipleScaleMatching)
                            {
                                //scale up/down the template
                                int direction = 1;
                                for (int i = 0; i <= MAX_SCALE_STEPS; i++)
                                {
                                    //scale up and verify for confidence
                                    double scale = 1 + direction * i * SCALE_STEP;
                                    Image<Gray, byte> templateTemp = ResizeTemplate(template, scale);
                                    Point[] box = GetElementBox(source, templateTemp, confidence);
                                    if (box != null)
                                    {
                                        elementRegion = new Region(box, confidence);
                                        elementRegion.setImages(templateTemp, source);
                                        return elementRegion;
                                    }

                                    //then scale down and verify for confidence
                                    scale = 1 + (-direction) * i * SCALE_STEP;
                                    templateTemp = ResizeTemplate(template, scale);
                                    box = GetElementBox(source, templateTemp, confidence);
                                    if (box != null)
                                    {
                                        elementRegion = new Region(box, confidence);
                                        elementRegion.setImages(templateTemp, source);
                                        return elementRegion;
                                    }
                                }
                            }
                            else
                            {
                                Point[] box = GetElementBox(source, template, confidence);
                                if (box != null)
                                {
                                    elementRegion = new Region(box, confidence);
                                    elementRegion.setImages(template, source);
                                    return elementRegion;
                                }
                            }
                        }
                        else
                        {
                            HandleErrors(string.Format("File- {0} doesnt exist.", filename));
                            break;
                        }
                    }
                    System.Threading.Thread.Sleep(THREAD_SLEEP_DURATION);
                }
                //reached this code line because no template is matched, hence raise exception.
                HandleErrors("Could not match any of the provided templates.");
            }
            catch (Exception ex)
            {
                HandleErrors(ex);
            }
            return elementRegion;
        }

        /// <summary>
        /// Tries to find all the occurance of the template image in the screen shot to be taken.
        /// </summary>
        /// <param name="filename">The file name depicting the template image file</param>
        /// <param name="timeout">The period within which if the element is not identified, it throws a exception. Unit- Seconds</param>
        /// <param name="confidence">The minimum confidence depicint the threshold of the image match. Value should be in the range - 1-100</param>
        /// <returns>List of Region objects for the matches found</returns>
        public List<Region> findAllInstances(string filename, int timeout, double confidence)
        {
            List<Region> allInstances = new List<Region>();
            Region elementRegion = null;
            DateTime startTime = DateTime.Now;
            try
            {
                if (timeout <= 0)
                    timeout = DEFAULT_TIMEOUT; //i.e. assigning the default time period to say 10 sec
                Image<Gray, byte> template = new Image<Gray, byte>(filename);
                while ((System.DateTime.Now - startTime).TotalMilliseconds <= timeout * 1000)
                {
                    Image<Gray, byte> source = Utilities.GetGrayScreenShot();
                    //then on the source hide all the instances found so far
                    if (allInstances != null && allInstances.Count > 0)
                    {
                        allInstances.ForEach(i => {
                            source.FillConvexPoly(i.getBox(), new Gray(0));
                        });
                    }
                    if (_multipleScaleMatching)
                    {
                        //scale up/down the template
                        int direction = 1;
                        for (int i = 0; i <= MAX_SCALE_STEPS; i++)
                        {
                            //scale up and verify for confidence
                            double scale = 1 + direction * i * SCALE_STEP;
                            Image<Gray, byte> templateTemp = ResizeTemplate(template, scale);
                            Point[] box = GetElementBox(source, templateTemp, confidence);
                            if (box != null)
                            {
                                elementRegion = new Region(box, confidence);
                                elementRegion.setImages(templateTemp, source);
                                allInstances.Add(elementRegion);
                                ////then hide the corresponding region in the source image
                                ////and continue finding for other instances
                                //source.FillConvexPoly(box, new Gray(0));
                                break;
                            }

                            //then scale down and verify for confidence
                            scale = 1 + (-direction) * i * SCALE_STEP;
                            templateTemp = ResizeTemplate(template, scale);
                            box = GetElementBox(source, templateTemp, confidence);
                            if (box != null)
                            {
                                elementRegion = new Region(box, confidence);
                                elementRegion.setImages(templateTemp, source);
                                allInstances.Add(elementRegion);
                                ////then hide the corresponding region in the source image
                                ////and continue finding for other instances
                                //source.FillConvexPoly(box, new Gray(0));
                                break;
                            }
                        }
                    }
                    else
                    {
                        Point[] box = GetElementBox(source, template, confidence);
                        if (box != null)
                        {
                            elementRegion = new Region(box, confidence);
                            elementRegion.setImages(template, source);
                            allInstances.Add(elementRegion);
                            ////then hide the corresponding region in the source image
                            ////and continue finding for other instances
                            //source.FillConvexPoly(box, new Gray(0));
                        }
                    }
                    System.Threading.Thread.Sleep(THREAD_SLEEP_DURATION);
                }
                if (allInstances.Count == 0)
                    HandleErrors("Could not find any match for the provided templates in the expected time period (time out)");
            }
            catch (Exception ex)
            {
                HandleErrors(ex);
            }
            return allInstances;
        }

        /// <summary>
        /// Starts the application intended.
        /// </summary>
        /// <param name="appName">Name of the application to be started</param>
        public void openApp(string appName)
        {
            Process process = Process.Start(appName);
        }

        /// <summary>
        /// Attempts to paste text by copying it to the clipboard and pasting it into the destination. 
        /// For this method to work, the destination must have been  selected in some way, (probably by calling click()). 
        /// The destination must also support input from the clipboard.
        /// </summary>
        /// <param name="text">the string to pasted</param>
        public void paste(string text)
        {
            //first add the text to the clip board
            System.Windows.Forms.Clipboard.SetText(text);

            //then fire ctrl+v using the 'type' interface
            type("v", KeyModifier.CTRL);
        }

        /// <summary>
        /// Fires ctrl+v to paste the applicable content to the target.
        /// For this method to work, the destination must have been  selected in some way, (probably by calling click()). 
        /// </summary>
        public void paste()
        {
            type("v", KeyModifier.CTRL);
        }

        /// <summary>
        /// Gets the text from the clipboard
        /// </summary>
        /// <returns>text copied in the clipboard</returns>
        public string read()
        {
            return System.Windows.Forms.Clipboard.GetText();
        }

        /// <summary>
        /// Clears the content from the clipboard
        /// </summary>
        public void clearClipboard()
        {
            System.Windows.Forms.Clipboard.Clear();
        }

        /// <summary>
        /// 'Select all' action (ctrl + a). For this to work first highlight the text area from where the text is to selected.
        /// </summary>
        public void selectAll()
        {
            type("a", KeyModifier.CTRL);
        }

        /// <summary>
        /// Fires 'ctrl + c' action. For this to work first select the text to be copied.
        /// </summary>
        public void copy()
        {
            type("c", KeyModifier.CTRL);
        }

        /// <summary>
        /// Sleeps the thred for the specified amount fo seconds
        /// </summary>
        /// <param name="time">Sleep duration in 'Second'</param>
        public void sleep(int time)
        {
            System.Threading.Thread.Sleep(time * 1000);
        }

        /// <summary>
        /// Imitate the key board button press for the provided text and modifier (e.g. ctrl, shift, etc)
        /// </summary>
        /// <param name="text">The alpha nuemeric keys to be pressed</param>
        /// <param name="modifiers">modifier: 0 for shift, 1 for ctrl, 2 for windows, 3 for alt, 4 for caps, 5 for enter, 6 for tab, 7 for back space, 8 for del, 9 for space. NB- in case of CAPS/ENTER, etc first call with text= blank or null and modifiers= 4, then again call with the required text and modifier = -1.
        /// Alternatively use enums- KeyModifier.SHIFT/CTRL/WINDOWS/ALT/CAPITAL/ENTER/TAB/BACKSPACE/DEL/SPACE</param>
        public void type(string text, params int[] modifiers)
        {
            Utilities.KeyPress(modifiers: modifiers, text: text);
        }

        /// <summary>
        /// To tell to highlight the detected region. By default the region is not highlighted
        /// </summary>
        public void highlight()
        {
            _highlightElement = true;
        }

        /// <summary>
        /// To dictate if multiple scale template matching is to be done. Default is true.
        /// </summary>
        public bool multipleScaleMatching
        {
            get
            {
                return _multipleScaleMatching;
            }
            set
            {
                _multipleScaleMatching = value;
            }
        }

        #endregion

        #region privare methods
        void HandleErrors(string errorMessage, bool throwException = true)
        {
            if (throwException)
                throw new Exception(errorMessage);
            else
            {
                //handle it as needed, e.g. log it, etc
            }
        }

        void HandleErrors(Exception ex, bool throwException = true)
        {
            if (throwException)
                throw ex;
            else
            {
                //handle it as needed, e.g. log it, etc
            }
        }

        private Image<Gray, byte> ResizeTemplate(Image<Gray, byte> template, double scale)
        {
            Image<Gray, byte> resizedTemplate = null;
            try
            {
                if (scale == 0)
                    scale = 1.0;
                else if (scale < 0)
                {
                    scale = 1 / (-scale);
                }
                resizedTemplate = template.Resize(scale, Inter.Lanczos4);
            }
            catch
            {
                //exception cud be for many reasons, like the region of interest is not found, etc
                //but we need to keep trying untill the timeout happens
            }
            return resizedTemplate;
        }

        private Point[] GetElementBox(Image<Gray, byte> source, Image<Gray, byte> template, double threshold)
        {
            Point[] box = null;
            try
            {
                if (template != null && source != null)
                {
                    threshold = threshold / 100; //i.e. from 1-100 form to 0.01-1 form
                    using (Image<Gray, float> result = source.MatchTemplate(template, TemplateMatchingType.CcoeffNormed))
                    {
                        double[] minValues, maxValues;
                        Point[] minLocations, maxLocations;
                        result.MinMax(out minValues, out  maxValues, out  minLocations, out  maxLocations);
                        if (maxValues[0] > threshold)
                        {
                            int w = template.Size.Width, h = template.Size.Height;
                            Point topLeft = maxLocations[0];
                            Point bottomRight = new Point(topLeft.X + w, topLeft.Y + h);
                            Point topRight = new Point(bottomRight.X, topLeft.Y);
                            Point bottomLeft = new Point(topLeft.X, bottomRight.Y);
                            box = new Point[] { topRight, topLeft, bottomLeft, bottomRight };
                        }
                    }
                }
            }
            catch
            {
                //exception cud be for many reasons, like the region of interest is not found, etc
                //but we need to keep trying untill the timeout happens
            }
            return box;
        }

        #endregion
    }
}

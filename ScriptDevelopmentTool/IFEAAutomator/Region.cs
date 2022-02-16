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

namespace ImageBasedAutomator
{
    public class Region
    {
        #region private variables and constants
        double _confidence;
        Point[] _box;
        Image<Gray, byte> _uiElement, _screenShot;
        Rectangle _elementRect;
        List<Rectangle> toBeHighlightedRects;
        double centroidX = 0, centroidY = 0;
        bool _highlightElement = false;
        Highlighter highlighter = null;

        const int THREAD_SLEEP_DURATION = 150;
        #endregion

        public Region()
        {

        }

        //public Region(bool highlightElement)
        //{
        //    _highlightElement = highlightElement;
        //}

        /// <summary>
        /// Initializes a result object using a Point array rather than an existing
        /// </summary>
        /// <param name="bound">A Point array that stores coordinates of vertices</param>
        /// <param name="confidence">The confidence of the match</param>
        public Region(Point[] bound, double confidence)
        {
            _confidence = confidence;
            _box = bound;
            if (_box != null && _box.Length == 4)
            {
                centroidX = _box[1].X + (_box[0].X - _box[1].X) / 2;
                centroidY = _box[1].Y + (_box[2].Y - _box[1].Y) / 2;

                //identify the rectangle and assign
                _elementRect = new Rectangle(_box[1], new Size(_box[0].X - _box[1].X, _box[2].Y - _box[1].Y));

                //populate the - toBeHighlightedRects
                if (toBeHighlightedRects == null)
                    toBeHighlightedRects = new List<Rectangle>();
                toBeHighlightedRects.Add(_elementRect);
            }
        }

        public Region(int x, int y, int width, int height)
        {
            _box = new Point[4];
            _box[0] = new Point(x + width, y);//top right
            _box[1] = new Point(x, y);//top left
            _box[2] = new Point(x, y + height);//bottom left
            _box[3] = new Point(x + width, y + height);//bottom right

            centroidX = (double)(x + width / 2);
            centroidY = (double)(y + height / 2);

            _elementRect = new Rectangle(x, y, width, height);
            if (toBeHighlightedRects == null)
                toBeHighlightedRects = new List<Rectangle>();
            toBeHighlightedRects.Add(_elementRect);
        }

        /// <summary>
        /// Gets the confidence of the result of match
        /// </summary>
        /// <returns>the confidence of the result of match</returns>
        public double getConfidence()
        {
            return _confidence;
        }

        /// <summary>
        /// Gets the detected region
        /// </summary>
        /// <returns>A Point array that represents the bounding box around the detected region</returns>
        public Point[] getBox()
        {
            //the position of the vertices in the array is as:
            //Point[] vertices = {topRight, topLeft, bottomLeft, bottomRight};
            return _box;
        }

        /// <summary>
        /// Gets the rectangle bounding the region
        /// </summary>
        /// <returns>the rectangle object</returns>
        public Rectangle getRectangle()
        {
            return _elementRect;
        }

        /// <summary>
        /// Creates a deep copy of the ui element and the screen shot before storing these.
        /// </summary>
        /// <param name="uiElem">the ui element</param>
        /// <param name="scrShot">the screen shot</param>
        public void setImages(Image<Gray, byte> uiElem, Image<Gray, byte> scrShot)
        {
            _uiElement = uiElem.Clone();
            _screenShot = scrShot.Clone();
        }

        /// <summary>
        /// Gets the ui element
        /// </summary>
        /// <returns>Reference to the OpenCV Image that holds the ui element</returns>
        public Image<Gray, byte> getUIElem()
        {
            return _uiElement;
        }

        /// <summary>
        /// Gets the screen shot
        /// </summary>
        /// <returns>Reference to the OpenCV Image that holds the screen shot</returns>
        public Image<Gray, byte> getScrShot()
        {
            return _screenShot;
        }

        /// <summary>
        /// To tell if the region is to be highlighted. By default the region is not highlighted
        /// </summary>
        public void highlight()
        {
            _highlightElement = true;
            //open a tranparent form with minimum or no border and then highlight the region/rectangle
            //and then on each event like click, type, etc, close this form
            highlighter = new Highlighter(toBeHighlightedRects);
            highlighter.ShowDialog(new System.Windows.Forms.Form());
        }

        /// <summary>
        /// To be used to highlight peer regions. Useful when more than one regions are to be identified.
        /// </summary>
        /// <param name="rects"></param>
        public void peerRegionsToBeHighlighted(List<Rectangle> rects)
        {
            if (toBeHighlightedRects != null)
                toBeHighlightedRects.AddRange(rects);
        }

        /// <summary>
        /// Left-Clicks the centroid of the bounding box whose corners are stored in the Point array
        /// </summary>
        public virtual void click()
        {
            if (highlighter != null)
            {
                highlighter.Close();
                highlighter = null;
            }
            Utilities.PlaceMouseCursor(centroidX, centroidY);
            Utilities.DoMouseClick();
        }

        /// <summary>
        /// Double clicks the centroid of the bounding box whose corners are stored in the Point array box
        /// </summary>
        public virtual void doubleClick()
        {
            if (highlighter != null)
            {
                highlighter.Close();
                highlighter = null;
            }
            Utilities.PlaceMouseCursor(centroidX, centroidY);
            Utilities.DoMouseClick();
            System.Threading.Thread.Sleep(THREAD_SLEEP_DURATION);
            Utilities.DoMouseClick();
        }

        /// <summary>
        /// Right clicks the centroid of the bounding box whose corners are stored in the Point array box
        /// </summary>
        public virtual void rightClick()
        {
            if (highlighter != null)
            {
                highlighter.Close();
                highlighter = null;
            }
            Utilities.PlaceMouseCursor(centroidX, centroidY);
            Utilities.DoMouseRightClick();
        }

        /// <summary>
        ///  Moves the mouse pointer onto the centroid of the detected region and leaves it there
        /// </summary>
        public virtual void hover()
        {
            if (highlighter != null)
            {
                highlighter.Close();
                highlighter = null;
            }
            Utilities.PlaceMouseCursor(centroidX, centroidY);
        }

        /// <summary>
        /// Raises Mouse left key down
        /// </summary>
        public virtual void mouseDown()
        {
            if (highlighter != null)
            {
                highlighter.Close();
                highlighter = null;
            }
            Utilities.DoMouseDown();
        }

        /// <summary>
        /// Raises Mouse left key up
        /// </summary>
        public virtual void mouseUp()
        {
            if (highlighter != null)
            {
                highlighter.Close();
                highlighter = null;
            }
            Utilities.DoMouseUp();
        }

        /// <summary>
        /// Offsets the region by the x, y values
        /// </summary>
        /// <param name="x">displacement in the x axis</param>
        /// <param name="y">displacement in the y axis</param>
        public virtual void targetOffset(int x, int y)
        {
            if (_box != null)
            {
                for (int i = 0; i < _box.Length; i++)
                {
                    _box[i].X += x;
                    _box[i].Y += y;
                }
            }
            if (_elementRect != null)
            {
                _elementRect.X += x;
                _elementRect.Y += y;
            }

            centroidX += x;
            centroidY += y;

            toBeHighlightedRects = new List<Rectangle>();
            toBeHighlightedRects.Add(_elementRect);
        }
    }
}

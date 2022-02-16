using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infosys.WEM.Infrastructure.Common
{
    public class ActivityEvents
    {
        public const string CLICK = "Click";
        public const string DOUBLE_CLICK = "DoubleClick";
        public const string HOVER = "Hover";
        public const string STATE = "State";
        public const string CHECK = "Check";
        public const string UNCHECK = "UnCheck";
        public const string COLLAPSE = "Collapse";
        public const string EXPAND = "Expand";
        public const string KEYPRESS = "KeyPress";
        public const string READ_SINGLE_ITEM = "Read Single Item";
        public const string READ_MULTIPLE_ITEMS = "Read Multiple Items";
        public const string READ_ALL_ITEMS = "Read All Items";
        public const string SELECT_SINGLE_ITEM = "Select Single Item";
        public const string SELECT_MULTIPLE_ITEMS = "Select Multiple Items";
        public const string SELECT_ALL_ITEMS = "Select All Items";

        public const string RECEIVE_TEXT= "Receive Text";
        public const string SEND_TEXT = "Send Text";
        public const string SELECT = "Select";
        public const string IS_SELECTED = "Is Selected";

        public const string ALERT = "Alert";
    }

    public class ActivityControls
    {
        public const string BUTTON = "Button";
        public const string COMBOBOX = "ComboBox";
        public const string CHECKBOX = "CheckBox";
        public const string CUSTOM = "Custom";
        public const string EDIT = "Edit";
        public const string DOCUMENT = "Document";
        public const string TEXTBOX = "TextBox";
        public const string IMAGE = "Image";
        public const string HYPERLINK = "Hyperlink";
        public const string LIST = "List";
        public const string LISTITEM = "ListItem";
        public const string MENU = "ButtoMenun";
        public const string RADIOBUTTON = "RadioButton";
        public const string ALERT = "Alert";
    }

    class Helper
    {
        public static string ConvertArrayToString(string[] array)
        {

            StringBuilder builder = new StringBuilder();
            int i = 1;
            foreach (string value in array)
            {
                builder.Append(i + ':' + value + ',');
                i++;
            }
            return builder.ToString();
        }

        public static string ConvertArrayToString(int[] array)
        {

            StringBuilder builder = new StringBuilder();
            int i = 1;
            foreach (int value in array)
            {
                builder.Append(i + ':' + value + ',');
                i++;
            }
            return builder.ToString();
        }

    }
}

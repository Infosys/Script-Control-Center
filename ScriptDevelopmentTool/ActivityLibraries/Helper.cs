/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infosys.WEM.Infrastructure.Common
{
    public class ActivityEvents
    {
        public const string CLICK = "Click";
        public const string CLICK_WITH_OFFSET = "Click with Offset";
        public const string RIGHT_CLICK = "Right Click";
        public const string DOUBLE_CLICK = "DoubleClick";
        public const string HOVER = "Hover";
        public const string MOUSE_DOWN = "MouseDown";
        public const string MOUSE_UP = "MouseUp";
        public const string SET_REGION = "SetRegion";
        public const string OFFSET_REGION = "OffsetRegion";
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
        public const string GET_SELECTED = "Get Selected";
        public const string IS_SELECTED = "Is Selected";

        public const string ALERT = "Alert";
        public const string PAUSEANDRESUME = "PauseAndResume";
        public const string GET_ALL_ROWS = "Get All Rows";
        public const string SET_CELLS_PER_ROW = "Set Cells per row";
        public const string GET_ROWS_WITH_CELLTEXT = "Get Rows With Cell Text";
        public const string GET_ROW_AT_INDEX = "Get Row At Index";
        public const string GET_ALL_CELLS = "Get All Cells";
        public const string GET_CELL_AT_INDEX = "Get Cell At Index";
        public const string GET_CELL_INDEXES_WITH_TEXT = "Get Cell Indexes With text";
        public const string LOGERROR = "LogError";
        public const string LOGAUDIT = "LogAudit";
        public const string NOTIFY = "Notify";
        public const string ADDTICKET = "AddTicket";
        public const string READTICKETS = "ReadTickets";
        public const string PAUSETICKET = "PauseTicket";
        public const string PAUSE_TICKET_PROCESSING = "PauseTicketProcessing";
        public const string RESUME_TICKET_PROCESSING = "ResumeTicketProcessing";
        public const string ENCRYPT = "Encrypt";
        public const string DECRYPT = "Decrypt";
        public const string INTERACTIVECHECK = "InteractiveCheck";
    }

    public class ActivityControls
    {
        public const string BASE_CONTROL = "Control";
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
        public const string TAB = "Tab";
        public const string TABITEM = "Tab Item";
        public const string DATAGRID = "Data Grid";
        public const string DATAITEM = "Data Item";
        public const string TABLE = "Table";
        public const string TABLEROW = "Table Row";
        public const string TABLECELL = "Table Cell";
        public const string UTILITIES = "Utilities";
        public const string TICKETHANDLER = "TicketHandler";
        public const string PAUSEANDRESUME = "PauseAndResume";
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

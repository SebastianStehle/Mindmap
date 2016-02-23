// ==========================================================================
// SupportedIcons.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;

namespace Hercules.Model
{
    public class SupportedIcons
    {
        private readonly List<INodeIcon> none = new List<INodeIcon> { null };

        private readonly List<IconCategory> categories = new List<IconCategory>
        {
            new IconCategory("Arrows",
                "Arrow_Up",
                "Arrow_Up_Right",
                "Arrow_Right",
                "Arrow_Down_Right",
                "Arrow_Down",
                "Arrow_Down_Left",
                "Arrow_Left",
                "Arrow_Up_Left"),
            new IconCategory("Months",
                "Calendar",
                "Calendar_Jan",
                "Calendar_Feb",
                "Calendar_Mar",
                "Calendar_Apr",
                "Calendar_May",
                "Calendar_Jun",
                "Calendar_Jul",
                "Calendar_Aug",
                "Calendar_Sep",
                "Calendar_Oct",
                "Calendar_Nov",
                "Calendar_Dez"),
            new IconCategory("Priorities",
                "Priority_1",
                "Priority_2",
                "Priority_3",
                "Priority_4",
                "Priority_5",
                "Priority_6",
                "Priority_7",
                "Priority_8",
                "Priority_9"),
            new IconCategory("Progress",
                "Progress_1_8",
                "Progress_1_4",
                "Progress_3_8",
                "Progress_1_2",
                "Progress_5_8",
                "Progress_3_4",
                "Progress_7_8",
                "Progress_Full"),
            new IconCategory("Stars",
                "Star_DarkRed",
                "Star_Red",
                "Star_Orange",
                "Star_Purple",
                "Star_Cyan",
                "Star_Blue",
                "Star_Green"),
            new IconCategory("Users",
                "User_DarkRed",
                "User_Red",
                "User_Orange",
                "User_Purple",
                "User_Cyan",
                "User_Blue",
                "User_Green"),
            new IconCategory("Flags",
                "Flag_DarkRed",
                "Flag_Red",
                "Flag_Orange",
                "Flag_Purple",
                "Flag_Cyan",
                "Flag_Blue",
                "Flag_Green"),
            new IconCategory("Symbols",
                "Cancel",
                "Cancel_Filled",
                "Check",
                "Check_Filled",
                "Important",
                "Important_Filled",
                "Information",
                "Information_Filled",
                "Question",
                "Question_Filled",
                "Warning",
                "Warning_Filled"),
            new IconCategory("Emoticons",
                "Emoticon_Angry",
                "Emoticon_Confused",
                "Emoticon_Cool",
                "Emoticon_Happy",
                "Emoticon_LOL",
                "Emoticon_Neutral",
                "Emoticon_Sad"),
            new IconCategory("Communication",
                "Calendar",
                "Phone",
                "Conference",
                "Contact",
                "Fax",
                "Message"),
            new IconCategory("Applications",
                "MS_Excel",
                "MS_OneNote",
                "MS_PowerPoint",
                "MS_Word"),
            new IconCategory("Others",
                "Alerts",
                "Book",
                "Buy",
                "Cash",
                "CD",
                "Clock",
                "Comments",
                "Games",
                "Gift",
                "Globe",
                "Home",
                "Key",
                "Like",
                "Note",
                "Picture",
                "Symbol_Dollar",
                "Symbol_Euro",
                "Traffic_Light",
                "Trash",
                "Video")
        };

        public List<INodeIcon> None
        {
            get { return none; }
        }

        public List<IconCategory> Categories
        {
            get { return categories; }
        }
    }
}

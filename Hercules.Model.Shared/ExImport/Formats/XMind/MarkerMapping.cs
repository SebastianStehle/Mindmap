// ==========================================================================
// MarkerMapping.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;

namespace Hercules.Model.ExImport.Formats.XMind
{
    public static class MarkerMapping
    {
        private static readonly Dictionary<string, string> XMindToMindapp = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> MindappToXmind = new Dictionary<string, string>();

        static MarkerMapping()
        {
            Add("priority-1", "Priority_1");
            Add("priority-2", "Priority_2");
            Add("priority-3", "Priority_3");
            Add("priority-4", "Priority_4");
            Add("priority-5", "Priority_5");
            Add("priority-6", "Priority_6");
            Add("priority-7", "Priority_7");
            Add("priority-8", "Priority_8");
            Add("priority-9", "Priority_9");

            Add("smiley-smile", "Emoticon_Happy");
            Add("smiley-laugh", "Emoticon_LOL");
            Add("smiley-angry", "Emoticon_Angry");
            Add("smiley-cry", "Emoticon_Sad");
            Add("smiley-surprise", "Emoticon_Confused");
            Add("smiley-boring", "Emoticon_Neutral");

            Add("task-oct", "Progress_1_8");
            Add("task-quarter", "Progress_1_4");
            Add("task-3oct", "Progress_3_8");
            Add("task-half", "Progress_1_2");
            Add("task-5oct", "Progress_5_8");
            Add("task-3quar", "Progress_3_4");
            Add("task-7oct", "Progress_7_8");
            Add("task-done", "Progress_Full");

            Add("flag-red", "Flag_DarkRed");
            Add("flag-orange", "Flag_Red");
            Add("flag-yellow", "Flag_Orange");
            Add("flag-blue", "Flag_Blue");
            Add("flag-green", "Flag_Green");
            Add("flag-purple", "Flag_Purple");
            Add("flag-gray", "Flag_Cyan");

            Add("star-red", "Star_DarkRed");
            Add("star-orange", "Star_Red");
            Add("star-yellow", "Star_Orange");
            Add("star-blue", "Star_Blue");
            Add("star-green", "Star_Green");
            Add("star-purple", "Star_Purple");
            Add("star-gray", "Star_Cyan");

            Add("people-red", "User_DarkRed");
            Add("people-orange", "User_Red");
            Add("people-yellow", "User_Orange");
            Add("people-blue", "User_Blue");
            Add("people-green", "User_Green");
            Add("people-purple", "User_Purple");
            Add("people-gray", "User_Cyan");

            Add("arrow-up", "Arrow_Up");
            Add("arrow-up-right", "Arrow_Up_Right");
            Add("arrow-right", "Arrow_Right");
            Add("arrow-down-right", "Arrow_Down_Right");
            Add("arrow-down", "Arrow_Down");
            Add("arrow-down-left", "Arrow_Down_Left");
            Add("arrow-left", "Arrow_Left");
            Add("arrow-up-left", "Arrow_Up_Left");

            Add("symbol-question", "Question");
            Add("symbol-exclam", "Warning");
            Add("symbol-info", "Information");
            Add("symbol-wrong", "Cancel");
            Add("symbol-right", "Check");
            Add("c_simbol-question", "Question_Filled");
            Add("c_simbol-exclam", "Warning_Filled");
            Add("c_simbol-info", "Information_Filled");
            Add("c_simbol-wrong", "Cancel_Filled");
            Add("c_simbol-right", "Check_Filled");

            Add("month-jan", "Calendar_Jan");
            Add("month-feb", "Calendar_Feb");
            Add("month-mar", "Calendar_Mar");
            Add("month-apr", "Calendar_Apr");
            Add("month-may", "Calendar_May");
            Add("month-jun", "Calendar_Jun");
            Add("month-jul", "Calendar_Jul");
            Add("month-aug", "Calendar_Aug");
            Add("month-sep", "Calendar_Sep");
            Add("month-oct", "Calendar_Oct");
            Add("month-nov", "Calendar_Nov");
            Add("month-dec", "Calendar_Dec");
        }

        public static void Add(string xmind, string mindapp)
        {
            XMindToMindapp[xmind] = mindapp;

            MindappToXmind[mindapp] = xmind;
        }

        public static string ResolveMindapp(string xmind)
        {
            string result;

            XMindToMindapp.TryGetValue(xmind, out result);

            return result;
        }

        public static string ResolveXmind(string mindapp)
        {
            string result;

            MindappToXmind.TryGetValue(mindapp, out result);

            return result;
        }
    }
}

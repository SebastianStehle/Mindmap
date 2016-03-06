// ==========================================================================
// LegacyIconMapper.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using GP.Utils;

namespace Hercules.Model
{
    public static class LegacyIconMapper
    {
        private const string Prefix = "/Assets/Icons/";
        private const string Suffix = ".png";

        private static readonly IDictionary<string, string> Names = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Encrypt", "Key" },
            { "Favorites", "Star_Orange" },
            { "Calendar_Dez", "Calendar_Dec" },
            { "Calendar_Public", "Calendar" },
            { "Function_Logical", "Book" },
            { "Status_Flag_Red", "Flag_Red" },
            { "Status_Flag_Blue", "Flag_Blue" },
            { "Status_Flag_Green", "Flag_Green" },
            { "Status_Flag_White", "Flag_Cyan" },
            { "Status_Flag_Yellow", "Flag_Orange" }
        };

        public static string Map(string name)
        {
            Guard.NotNullOrEmpty(name, nameof(name));

            if (name.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase))
            {
                name = name.Substring(Prefix.Length);
            }

            if (name.EndsWith(Suffix, StringComparison.OrdinalIgnoreCase))
            {
                name = name.Substring(0, name.Length - Suffix.Length);
            }

            string temp;

            if (Names.TryGetValue(name, out temp))
            {
                name = temp;
            }

            return name;
        }
    }
}

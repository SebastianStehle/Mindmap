// ==========================================================================
// ResourceManager.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Globalization;
using Windows.ApplicationModel.Resources;

namespace Hercules.Model.Utils
{
    internal static class ResourceManager
    {
        public static string GetString(string key)
        {
            ResourceLoader resourceLoader = new ResourceLoader();
            
            return resourceLoader.GetString(key) ?? key;
        }

        public static string FormatString(string key, params object[] args)
        {
            ResourceLoader resourceLoader = new ResourceLoader();

            return string.Format(CultureInfo.CurrentCulture, resourceLoader.GetString(key), args) ?? key;
        }
    }
}

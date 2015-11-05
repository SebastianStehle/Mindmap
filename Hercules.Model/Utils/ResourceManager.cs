// ==========================================================================
// ResourceManager.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Globalization;
using Windows.ApplicationModel.Resources;

namespace Hercules.Model.Utils
{
    public static class ResourceManager
    {
        public static Func<string, string> StringProvider { get; set; }

        static ResourceManager()
        {
            StringProvider = key =>
            {
                ResourceLoader resourceLoader = new ResourceLoader();

                return resourceLoader.GetString(key);
            };
        }

        public static string GetString(string key)
        {
            string result = StringProvider(key);

            if (string.IsNullOrWhiteSpace(result))
            {
                throw new ArgumentException($"Cannot find text with key '{key}'.");
            }

            return result;
        }

        public static string GetString(string key, params object[] args)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString(key), args) ?? key;
        }
    }
}

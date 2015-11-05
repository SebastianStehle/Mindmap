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
        public static string GetString(string key)
        {
            ResourceLoader resourceLoader = new ResourceLoader();

            string result = resourceLoader.GetString(key);

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

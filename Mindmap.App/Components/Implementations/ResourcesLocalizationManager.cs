// ==========================================================================
// ResourcesLocalizationManager.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Globalization;
using Windows.ApplicationModel.Resources;

namespace MindmapApp.Components.Implementations
{
    public sealed class ResourcesLocalizationManager : ILocalizationManager
    {
        public string GetString(string key)
        {
            ResourceLoader resourceLoader = new ResourceLoader();

            return resourceLoader.GetString(key);
        }

        public string FormatString(string key, params object[] args)
        {
            ResourceLoader resourceLoader = new ResourceLoader();

            return string.Format(CultureInfo.CurrentCulture, resourceLoader.GetString(key), args);
        }
    }
}

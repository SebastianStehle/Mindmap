﻿// ==========================================================================
// ResourcesLocalizationManager.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Composition;
using System.Globalization;
using Windows.ApplicationModel.Resources;

namespace RavenMind.Components
{
    [Export]
    [Export(typeof(ILocalizationManager))]
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
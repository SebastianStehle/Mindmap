﻿// ==========================================================================
// DefaultSettingsProvider.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.Storage;

namespace Hercules.App.Components.Implementations
{
    public sealed class DefaultSettingsProvider : ISettingsProvider
    {
        private readonly ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;

        public bool IsAlreadyStarted
        {
            get
            {
                return GetBoolean(nameof(IsAlreadyStarted));
            }
            set
            {
                SetBoolean(nameof(IsAlreadyStarted), value);
            }
        }

        private bool GetBoolean(string key)
        {
            return ToBoolean(settings.Values[key]);
        }

        private void SetBoolean(string key, bool value)
        {
            settings.Values[key] = value;
        }

        private static bool ToBoolean(object value)
        {
            return value != null && (bool)value;
        }
    }
}

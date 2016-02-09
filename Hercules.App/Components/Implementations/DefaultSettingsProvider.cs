// ==========================================================================
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

        public bool ShowIconSection
        {
            get
            {
                return GetBoolean(nameof(ShowIconSection), true);
            }
            set
            {
                SetBoolean(nameof(ShowIconSection), value);
            }
        }

        public bool ShowColorSection
        {
            get
            {
                return GetBoolean(nameof(ShowColorSection), true);
            }
            set
            {
                SetBoolean(nameof(ShowColorSection), value);
            }
        }

        public bool ShowShapeSection
        {
            get
            {
                return GetBoolean(nameof(ShowShapeSection), true);
            }
            set
            {
                SetBoolean(nameof(ShowShapeSection), value);
            }
        }

        public bool ShowCheckBoxesSection
        {
            get
            {
                return GetBoolean(nameof(ShowCheckBoxesSection), true);
            }
            set
            {
                SetBoolean(nameof(ShowCheckBoxesSection), value);
            }
        }

        private bool GetBoolean(string key, bool defaultValue = false)
        {
            return ToBoolean(settings.Values[key], defaultValue);
        }

        private void SetBoolean(string key, bool value)
        {
            settings.Values[key] = value;
        }

        private static bool ToBoolean(object value, bool defaultValue)
        {
            return (bool?)value ?? defaultValue;
        }
    }
}

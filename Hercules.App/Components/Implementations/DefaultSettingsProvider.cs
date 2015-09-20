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
        public bool IsAlreadyStarted
        {
            get
            {
                ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;

                return ToBoolean(settings.Values["IsAlreadyStarted"]);
            }
            set
            {
                ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;

                settings.Values["IsAlreadyStarted"] = value;
            }
        }

        private static bool ToBoolean(object value)
        {
            return value != null && (bool)value;
        }
    }
}

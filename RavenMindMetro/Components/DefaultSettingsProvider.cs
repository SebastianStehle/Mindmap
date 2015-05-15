// ==========================================================================
// RoamingSettingsProvider.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.Storage;

namespace RavenMind.Components
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

        public bool IsTutorialShown
        {
            get
            {
                ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;

                return ToBoolean(settings.Values["TutorialShown"]);
            }
            set
            {
                ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;

                settings.Values["TutorialShown"] = value;
            }
        }

        private static bool ToBoolean(object value)
        {
            return value == null ? false : (bool)value;
        }
    }
}

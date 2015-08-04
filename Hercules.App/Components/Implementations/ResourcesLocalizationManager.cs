// ==========================================================================
// ResourcesLocalizationManager.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace Hercules.App.Components.Implementations
{
    public sealed class ResourcesLocalizationManager : ILocalizationManager
    {
        public string GetString(string key)
        {
            return ResourceManager.GetString(key);
        }

        public string FormatString(string key, params object[] args)
        {
            return ResourceManager.FormatString(key, args);
        }
    }
}

// ==========================================================================
// ILocalizationManager.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace Hercules.App.Components
{
    public interface ILocalizationManager
    {
        string GetString(string key);

        string FormatString(string key, params object[] args);
    }
}

// ==========================================================================
// ILocalizationManager.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace Mindmap.Components
{
    public interface ILocalizationManager
    {
        string GetString(string key);

        string FormatString(string key, params object[] args);
    }
}

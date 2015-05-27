// ==========================================================================
// ISettingsProvider.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace Mindmap.Components
{
    public interface ISettingsProvider
    {
        bool IsAlreadyStarted { get; set; }

        bool IsTutorialShown { get; set; }
    }
}

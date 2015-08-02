// ==========================================================================
// ISettingsProvider.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace Hercules.App.Components
{
    public interface ISettingsProvider
    {
        bool IsAlreadyStarted { get; set; }

        bool IsTutorialShown { get; set; }
    }
}

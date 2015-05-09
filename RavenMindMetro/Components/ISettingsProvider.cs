// ==========================================================================
// ISettingsProvider.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace RavenMind.Components
{
    public interface ISettingsProvider
    {
        bool IsAlreadyStarted { get; set; }

        bool IsTutorialShown { get; set; }
    }
}

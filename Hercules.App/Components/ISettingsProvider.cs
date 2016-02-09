// ==========================================================================
// ISettingsProvider.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace Hercules.App.Components
{
    public interface ISettingsProvider
    {
        bool IsAlreadyStarted { get; set; }

        bool ShowIconSection { get; set; }

        bool ShowColorSection { get; set; }

        bool ShowShapeSection { get; set; }

        bool ShowCheckBoxesSection { get; set; }
    }
}

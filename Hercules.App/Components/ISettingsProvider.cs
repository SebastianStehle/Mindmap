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
    }
}

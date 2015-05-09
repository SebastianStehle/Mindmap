// ==========================================================================
// ISelectable.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace RavenMind.Model
{
    public interface ISelectable
    {
        event EventHandler SelectionChanged;

        bool IsSelected { get; set; }
    }
}

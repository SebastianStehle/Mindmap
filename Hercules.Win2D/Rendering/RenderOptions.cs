// ==========================================================================
// RenderOptions.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace Hercules.Win2D.Rendering
{
    [Flags]
    public enum RenderOptions
    {
        Plain = 0,

        RenderControls = 1,
        RenderCustoms = 2,

        Full = RenderControls | RenderCustoms
    }
}

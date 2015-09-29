// ==========================================================================
// RenderFlags.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace Hercules.Model.Rendering.Win2D
{
    [Flags]
    public enum RenderFlags
    {
        Plain = 0,
        RenderControls = 1,
        RenderCustoms = 2,
        Full = RenderControls | RenderCustoms
    }
}

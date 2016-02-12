// ==========================================================================
// IClickablePart.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Numerics;

namespace Hercules.Win2D.Rendering.Parts
{
    public interface IClickablePart
    {
        bool HandleClick(Win2DRenderable renderable, Vector2 hitPosition);
    }
}

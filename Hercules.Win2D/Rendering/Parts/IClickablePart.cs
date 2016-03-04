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
        HitResult HitTest(Win2DRenderNode renderNode, Vector2 hitPosition);
    }
}

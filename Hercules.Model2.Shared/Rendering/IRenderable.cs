// ==========================================================================
// IRenderable.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Numerics;
using GP.Utils.Mathematics;

namespace Hercules.Model2.Rendering
{
    public interface IRenderable
    {
        Vector2 RenderPosition { get; }

        Vector2 RenderSize { get; }

        Rect2 RenderBounds { get; }

        Node Node { get; }
    }
}

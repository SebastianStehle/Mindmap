// ==========================================================================
// IRenderable.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Numerics;
using GP.Utils.Mathematics;

namespace Hercules.Model.Rendering
{
    public interface IRenderable
    {
        NodeBase Node { get; }

        Vector2 RenderPosition { get; }

        Vector2 RenderSize { get; }

        Rect2 RenderBounds { get; }
    }
}

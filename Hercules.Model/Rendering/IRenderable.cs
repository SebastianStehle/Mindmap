// ==========================================================================
// IRenderable.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Numerics;
using Hercules.Model.Utils;

namespace Hercules.Model.Rendering
{
    public interface IRenderable
    {
        Vector2 RenderPosition { get; }

        Vector2 RenderSize { get; }

        Rect2 RenderBounds { get; }

        NodeBase Node { get; }
    }
}

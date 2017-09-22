// ==========================================================================
// IAdornerRenderNode.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Numerics;

namespace Hercules.Model2.Rendering
{
    public interface IAdornerRenderNode : IRenderable
    {
        void MoveTo(Vector2 position);

        void MoveBy(Vector2 offset);
    }
}

// ==========================================================================
// IRenderNode.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Numerics;

namespace Hercules.Model.Rendering
{
    public interface IRenderNode : IRenderable
    {
        Vector2 LayoutPosition { get; }

        bool IsVisible { get; }

        void Hide();

        void Show();

        void MoveToLayout(Vector2 position, NodeSide anchor);
    }
}

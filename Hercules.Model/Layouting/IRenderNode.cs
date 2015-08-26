// ==========================================================================
// IRenderNode.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Numerics;
using Hercules.Model.Utils;

namespace Hercules.Model.Layouting
{
    public interface IRenderNode
    {
        Vector2 Position { get; }

        Vector2 RenderPosition { get; }

        Vector2 RenderSize { get; }

        Rect2 Bounds { get; }

        bool IsVisible { get; }

        void Hide();

        void Show();

        void MoveToLayout(Vector2 layoutPosition, AnchorPoint anchor);
    }
}

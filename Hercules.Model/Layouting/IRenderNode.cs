// ==========================================================================
// IRenderNode.cs
// Hercules Application
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

        Vector2 Size { get; }

        Rect2 Bounds { get; }

        bool IsVisible { get; }

        void Hide();

        void Show();

        void MoveTo(Vector2 position, AnchorPoint anchor);
    }
}

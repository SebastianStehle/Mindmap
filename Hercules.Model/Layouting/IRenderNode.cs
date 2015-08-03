// ==========================================================================
// IRenderNode.cs
// Hercules Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.Foundation;

namespace Hercules.Model.Layouting
{
    public interface IRenderNode
    {
        Point Position { get; }

        Rect Bounds { get; }

        Size Size { get; }

        bool IsVisible { get; }

        void Hide();

        void Show();

        void MoveTo(Point position, AnchorPoint anchor);
    }
}

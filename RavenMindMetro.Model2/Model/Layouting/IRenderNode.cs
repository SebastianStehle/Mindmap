// ==========================================================================
// IRenderNode.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.Foundation;

namespace RavenMind.Model.Layouting
{
    public interface IRenderNode
    {
        Point Position { get; }

        Size Size { get; }

        void Move(Point position, AnchorPoint anchor);
    }
}

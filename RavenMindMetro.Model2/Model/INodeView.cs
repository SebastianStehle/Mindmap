// ==========================================================================
// INodeView.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.Foundation;

namespace RavenMind.Model
{
    public interface INodeView
    {
        Size Size { get; }

        void SetPosition(Point position, AnchorPoint anchor);
    }
}

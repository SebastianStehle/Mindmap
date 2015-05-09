// ==========================================================================
// ILayout.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Windows.Foundation;

namespace RavenMind.Model.Layouting
{
    public interface ILayout
    {
        AttachTarget CalculateAttachTarget(NodeBase parent, IRenderer renderer, Node movingNode, Rect movementBounds, Point mindmapCenter);

        void UpdateLayout(Document document, IRenderer renderer, Size availableSize);
    }
}

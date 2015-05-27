// ==========================================================================
// ILayout.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.Foundation;

namespace Mindmap.Model.Layouting
{
    public interface ILayout
    {
        AttachTarget CalculateAttachTarget(Document document, IRenderer renderer, Node movingNode, Rect movementBounds);

        void UpdateLayout(Document document, IRenderer renderer);
    }
}

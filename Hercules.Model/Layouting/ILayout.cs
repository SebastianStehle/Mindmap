// ==========================================================================
// ILayout.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Hercules.Model.Utils;

namespace Hercules.Model.Layouting
{
    public interface ILayout
    {
        AttachTarget CalculateAttachTarget(Document document, IRenderer renderer, Node movingNode, Rect2 movementBounds);

        void UpdateLayout(Document document, IRenderer renderer);

        void UpdateVisibility(Document document, IRenderer renderer);
    }
}

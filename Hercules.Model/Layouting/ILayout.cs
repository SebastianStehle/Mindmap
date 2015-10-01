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
        AttachTarget CalculateAttachTarget(Document document, IRenderScene scene, Node movingNode, Rect2 movementBounds);

        void UpdateLayout(Document document, IRenderScene scene);

        void UpdateVisibility(Document document, IRenderScene scene);
    }
}

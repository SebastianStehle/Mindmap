// ==========================================================================
// DefaultLayout.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GP.Utils.Mathematics;
using Hercules.Model.Rendering;

namespace Hercules.Model.Layouting.Default
{
    public sealed class DefaultLayout : ILayout
    {
        public int HorizontalMargin { get; set; }

        public int ElementMargin { get; set; }

        public void UpdateLayout(Document document, IRenderScene scene)
        {
            new DefaultLayoutProcess(this, scene, document).UpdateLayout();
        }

        public void UpdateVisibility(Document document, IRenderScene scene)
        {
            new VisibilityUpdater<DefaultLayout>(this, scene, document).UpdateVisibility();
        }

        public AttachTarget CalculateAttachTarget(Document document, IRenderScene scene, Node movingNode, Rect2 movementBounds)
        {
            return new DefaultAttachTargetProcess(this, scene, document, movingNode, movementBounds).CalculateAttachTarget();
        }
    }
}

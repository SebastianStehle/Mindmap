// ==========================================================================
// HorizontalStraightLayout.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GP.Utils.Mathematics;
using Hercules.Model.Rendering;

namespace Hercules.Model.Layouting.HorizontalStraight
{
    public sealed class HorizontalStraightLayout : ILayout
    {
        public static readonly HorizontalStraightLayout Instance = new HorizontalStraightLayout();

        public int HorizontalMargin { get; set; } = 10;

        public int ElementMargin { get; set; } = 50;

        public void UpdateLayout(Document document, IRenderScene scene)
        {
            new HorizontalStraightLayoutProcess(this, scene, document).UpdateLayout();
        }

        public void UpdateVisibility(Document document, IRenderScene scene)
        {
            new VisibilityUpdater<HorizontalStraightLayout>(this, scene, document).UpdateVisibility();
        }

        public AttachTarget CalculateAttachTarget(Document document, IRenderScene scene, Node movingNode, Rect2 movementBounds)
        {
            return new HorizontalStraightAttachTargetProcess(this, scene, document, movingNode, movementBounds).CalculateAttachTarget();
        }
    }
}

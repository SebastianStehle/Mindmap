// ==========================================================================
// HorizontalStraightLayout.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GP.Utils.Mathematics;
using Hercules.Model2.Rendering;

namespace Hercules.Model2.Layouting.HorizontalStraight
{
    public sealed class HorizontalStraightLayout : ILayout
    {
        public static readonly HorizontalStraightLayout Instance = new HorizontalStraightLayout();

        public int HorizontalMargin { get; } = 50;

        public int ElementMargin { get; } = 6;

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

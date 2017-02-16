// ==========================================================================
// ExpandButton.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Numerics;
using Windows.UI;
using GP.Utils.Mathematics;
using Microsoft.Graphics.Canvas;

// ReSharper disable SuggestBaseTypeForParameter

namespace Hercules.Win2D.Rendering.Utils
{
    public sealed class ExpandButton
    {
        private float renderRadius = 20;
        private Rect2 renderBounds;
        private Vector2 renderCenter;

        public void Arrange(Vector2 center, float radius)
        {
            renderRadius = radius;
            renderCenter = center;

            renderBounds = Rect2.Inflate(new Rect2(center, Vector2.Zero), radius, radius);
        }

        public HitResult HitTest(Win2DRenderNode renderNode, Vector2 hitPosition)
        {
            return renderBounds.Contains(hitPosition) ? new HitResult(renderNode, HitTarget.ExpandButton) : null;
        }

        public void Render(Win2DRenderable renderable, CanvasDrawingSession session)
        {
            RenderCircle(session);

            var halfRadius = 0.5f * renderRadius;

            RenderHorizontal(session, halfRadius);

            if (renderable.Node.IsCollapsed)
            {
                RenderVertical(session, halfRadius);
            }
        }

        private void RenderCircle(CanvasDrawingSession session)
        {
#if DRAW_OUTLINE
            session.DrawRectangle(renderBounds.ToRect(), Colors.Orange);
#endif
            session.FillCircle(renderCenter, renderRadius, Colors.White);

            session.DrawCircle(renderCenter, renderRadius, Colors.DarkGray);
        }

        private void RenderVertical(CanvasDrawingSession session, float halfRadius)
        {
            var verticalTop = new Vector2(
                renderCenter.X,
                renderCenter.Y - halfRadius);
            var verticalBottom = new Vector2(
                renderCenter.X,
                renderCenter.Y + halfRadius);

            session.DrawLine(verticalTop, verticalBottom, Colors.DarkGray, 2f);
        }

        private void RenderHorizontal(CanvasDrawingSession session, float halfRadius)
        {
            var horizontalLeft = new Vector2(
                renderCenter.X - halfRadius,
                renderCenter.Y);

            var horizontalRight = new Vector2(
                renderCenter.X + halfRadius,
                renderCenter.Y);

            session.DrawLine(horizontalLeft, horizontalRight, Colors.DarkGray, 2f);
        }
    }
}

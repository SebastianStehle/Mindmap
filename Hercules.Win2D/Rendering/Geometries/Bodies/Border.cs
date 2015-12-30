// ==========================================================================
// Border.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Numerics;
using Windows.UI;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;

namespace Hercules.Win2D.Rendering.Geometries.Bodies
{
    public sealed class Border : BodyBase
    {
        private const float VerticalOffsetPadding = 8;
        private static readonly CanvasStrokeStyle StrokeStyle = new CanvasStrokeStyle { StartCap = CanvasCapStyle.Round, EndCap = CanvasCapStyle.Round };
        private readonly Color pathColor;
        private float verticalOffset;

        public override float VerticalPathOffset
        {
            get { return verticalOffset; }
        }

        public override Vector2 RenderPositionOffset
        {
            get { return new Vector2(0, -verticalOffset); }
        }

        public Border(Color pathColor)
        {
            this.pathColor = pathColor;
        }

        public override Vector2 Measure(Win2DRenderable renderable, CanvasDrawingSession session, Vector2 textSize)
        {
            Vector2 size = base.Measure(renderable, session, textSize);

            verticalOffset = (size.Y - VerticalOffsetPadding) - (0.5f * size.Y);

            return size;
        }

        protected override Vector2 CalculatePadding(Win2DRenderable renderable, Vector2 contentSize)
        {
            return new Vector2(15, 10);
        }

        public override void Render(Win2DRenderable renderable, CanvasDrawingSession session, Win2DColor color, bool renderControls)
        {
            ICanvasBrush borderBrush = renderable.Resources.ThemeDarkBrush(color);

            ICanvasBrush lineBrush = renderable.Resources.Brush(pathColor, 1);

            Vector2 left = new Vector2(
                (float)Math.Round(renderable.RenderBounds.Left - 1),
                (float)Math.Round(renderable.RenderBounds.CenterY) + verticalOffset);

            Vector2 right = new Vector2(
                (float)Math.Round(renderable.RenderBounds.Right + 1),
                (float)Math.Round(renderable.RenderBounds.CenterY) + verticalOffset);

            session.DrawLine(left, right, lineBrush, 2, StrokeStyle);

            RenderIcon(renderable, session);

            if (renderControls && renderable.Node.IsSelected)
            {
                session.DrawRoundedRectangle(renderable.RenderBounds, 5, 5, borderBrush, 2f, SelectionStrokeStyle);
            }
        }

        public override IBodyGeometry Clone()
        {
            return new Border(pathColor);
        }
    }
}

// ==========================================================================
// Border.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Hercules.Model;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;

namespace Hercules.Win2D.Rendering.Geometries.Bodies
{
    public sealed class Border : BodyBase
    {
        private const float MinHeight = 40;
        private const float VerticalOffsetPadding = 8;
        private static readonly Vector2 ContentPadding = new Vector2(15, 10);
        private static readonly CanvasStrokeStyle StrokeStyle = new CanvasStrokeStyle { StartCap = CanvasCapStyle.Round, EndCap = CanvasCapStyle.Round };
        private readonly Color pathColor;
        private Vector2 textRenderSize;
        private Vector2 textRenderPosition;
        private float verticalOffset;
        private float textOffset;

        public override Vector2 TextRenderPosition
        {
            get { return textRenderPosition; }
        }

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

        public override void Arrange(Win2DRenderable renderable, CanvasDrawingSession session)
        {
            float x = renderable.RenderPosition.X, y = renderable.RenderBounds.CenterY;

            x += ContentPadding.X;
            x += textOffset;
            y -= textRenderSize.Y * 0.5f;

            textRenderPosition = new Vector2(x, y);
        }

        public override Vector2 Measure(Win2DRenderable renderable, CanvasDrawingSession session, Vector2 textSize)
        {
            textRenderSize = textSize;

            Vector2 size = textSize + (2 * ContentPadding);

            if (renderable.Node.Icon != null)
            {
                if (renderable.Node.IconSize == IconSize.Small)
                {
                    textOffset = ImageSizeSmall.X + ImageMargin;
                }
                else
                {
                    textOffset = ImageSizeLarge.X + ImageMargin;
                }
            }
            else
            {
                textOffset = 0;
            }

            size.X += textOffset;
            size.Y = Math.Max(size.Y, MinHeight);

            verticalOffset = (size.Y - VerticalOffsetPadding) - 0.5f * size.Y;

            return size;
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

            if (renderable.Node.Icon != null)
            {
                ICanvasImage image = renderable.Resources.Image(renderable.Node);

                if (image != null)
                {
                    Vector2 size = renderable.Node.IconSize == IconSize.Large ? ImageSizeLarge : ImageSizeSmall;

                    float x = textRenderPosition.X - textOffset;
                    float y = textRenderPosition.Y + ((textRenderSize.Y - size.Y) * 0.5f);

                    session.DrawImage(image, new Rect(x, y, 32, 32), image.GetBounds(session), 1, CanvasImageInterpolation.HighQualityCubic);
                }
            }

            if (renderControls)
            {
                if (renderable.Node.IsSelected)
                {
                    session.DrawRoundedRectangle(renderable.RenderBounds, 5, 5, borderBrush, 2f, SelectionStrokeStyle);
                }
            }
        }

        public override IBodyGeometry Clone()
        {
            return new Border(pathColor);
        }
    }
}

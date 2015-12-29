// ==========================================================================
// Ellipse.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Numerics;
using Windows.Foundation;
using Hercules.Model;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;

namespace Hercules.Win2D.Rendering.Geometries.Bodies
{
    public sealed class Ellipse : BodyBase
    {
        private const float MinHeight = 50;
        private static readonly Vector2 ContentPadding = new Vector2(15, 5);
        private static readonly Vector2 SelectionMargin = new Vector2(-5, -5);
        private Vector2 textRenderSize;
        private Vector2 textRenderPosition;
        private float textOffset;

        public override Vector2 TextRenderPosition
        {
            get { return textRenderPosition; }
        }

        public Ellipse()
        {
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
                    textOffset = ImageSizeSmall.X + (ImageMargin * 2);
                }
                else
                {
                    textOffset = ImageSizeLarge.X + (ImageMargin * 2);
                }
            }
            else
            {
                textOffset = 0;
            }

            size.X += textOffset;
            size.Y = Math.Max(Math.Max(size.Y, size.X / 3f), MinHeight);

            return size;
        }

        public override void Render(Win2DRenderable renderable, CanvasDrawingSession session, Win2DColor color, bool renderControls)
        {
            ICanvasBrush borderBrush = renderable.Resources.ThemeDarkBrush(color);

            ICanvasBrush backgroundBrush =
                renderable.Node.IsSelected ?
                    renderable.Resources.ThemeLightBrush(color) :
                    renderable.Resources.ThemeNormalBrush(color);

            float radiusX = 0.5f * renderable.RenderSize.X;
            float radiusY = 0.5f * renderable.RenderSize.Y;

            session.FillEllipse(
                renderable.RenderBounds.Center,
                radiusX,
                radiusY,
                backgroundBrush);

            session.DrawEllipse(
                renderable.RenderBounds.Center,
                radiusX,
                radiusY,
                borderBrush);

            if (renderable.Node.Icon != null)
            {
                ICanvasImage image = renderable.Resources.Image(renderable.Node);

                if (image != null)
                {
                    Vector2 size = renderable.Node.IconSize == IconSize.Large ? ImageSizeLarge : ImageSizeSmall;

                    float x = textRenderPosition.X - textOffset + ImageMargin;
                    float y = textRenderPosition.Y + ((textRenderSize.Y - size.Y) * 0.5f);

                    session.DrawImage(image, new Rect(x, y, 32, 32), image.GetBounds(session), 1, CanvasImageInterpolation.HighQualityCubic);
                }
            }

            if (renderControls)
            {
                if (renderable.Node.IsSelected)
                {
                    radiusX -= SelectionMargin.X;
                    radiusY -= SelectionMargin.Y;

                    session.DrawEllipse(
                        renderable.RenderBounds.Center,
                        radiusX,
                        radiusY,
                        borderBrush, 2f, SelectionStrokeStyle);
                }
            }
        }

        public override IBodyGeometry Clone()
        {
            return new Ellipse();
        }
    }
}

// ==========================================================================
// EllipseNode.cs
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
    public class Ellipse : Body
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

        public override void Arrange(Win2DRenderNode renderNode, CanvasDrawingSession session)
        {
            float x = renderNode.RenderPosition.X, y = renderNode.Bounds.CenterY;

            x += ContentPadding.X;
            x += textOffset;
            y -= textRenderSize.Y * 0.5f;

            textRenderPosition = new Vector2(x, y);
        }

        public override Vector2 Measure(Win2DRenderNode renderNode, CanvasDrawingSession session, Vector2 textSize)
        {
            textRenderSize = textSize;

            Vector2 size = textSize + (2 * ContentPadding);

            if (renderNode.Node.Icon != null)
            {
                if (renderNode.Node.IconSize == IconSize.Small)
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

        public override void Render(Win2DRenderNode renderNode, CanvasDrawingSession session, Win2DColor color, bool renderControls)
        {
            ICanvasBrush borderBrush = renderNode.Resources.ThemeDarkBrush(color);

            ICanvasBrush backgroundBrush =
                renderNode.Node.IsSelected ?
                    renderNode.Resources.ThemeLightBrush(color) :
                    renderNode.Resources.ThemeNormalBrush(color);

            float radiusX = 0.5f * renderNode.RenderSize.X;
            float radiusY = 0.5f * renderNode.RenderSize.Y;

            session.FillEllipse(
                renderNode.Bounds.Center,
                radiusX,
                radiusY,
                backgroundBrush);

            session.DrawEllipse(
                renderNode.Bounds.Center,
                radiusX,
                radiusY,
                borderBrush);

            if (renderNode.Node.Icon != null)
            {
                ICanvasImage image = renderNode.Resources.Image(renderNode.Node);

                if (image != null)
                {
                    Vector2 size = renderNode.Node.IconSize == IconSize.Large ? ImageSizeLarge : ImageSizeSmall;

                    float x = textRenderPosition.X - textOffset + ImageMargin;
                    float y = textRenderPosition.Y + ((textRenderSize.Y - size.Y) * 0.5f);

                    session.DrawImage(image, new Rect(x, y, 32, 32), image.GetBounds(session), 1, CanvasImageInterpolation.HighQualityCubic);
                }
            }

            if (renderControls)
            {
                if (renderNode.Node.IsSelected)
                {
                    radiusX -= SelectionMargin.X;
                    radiusY -= SelectionMargin.Y;

                    session.DrawEllipse(
                        renderNode.Bounds.Center,
                        radiusX,
                        radiusY,
                        borderBrush, 2f, SelectionStrokeStyle);
                }
            }
        }
    }
}

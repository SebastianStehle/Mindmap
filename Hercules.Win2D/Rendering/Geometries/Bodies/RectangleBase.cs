// ==========================================================================
// RectangleNodeBase.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Numerics;
using Windows.Foundation;
using Hercules.Model;
using Hercules.Model.Utils;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;

namespace Hercules.Win2D.Rendering.Geometries.Bodies
{
    public abstract class RectangleBase : Body
    {
        private const float MinHeight = 40;
        private static readonly Vector2 ContentPadding = new Vector2(15, 5);
        private static readonly Vector2 SelectionMargin = new Vector2(-5, -5);
        private readonly float borderRadius;
        private Vector2 textRenderSize;
        private Vector2 textRenderPosition;
        private float textOffset;

        public override Vector2 TextRenderPosition
        {
            get { return textRenderPosition; }
        }

        protected RectangleBase(float borderRadius)
        {
            this.borderRadius = borderRadius;
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

            return size;
        }

        public override void Render(Win2DRenderNode renderNode, CanvasDrawingSession session, Win2DColor color, bool renderSelection)
        {
            ICanvasBrush borderBrush = renderNode.Resources.ThemeDarkBrush(color);

            ICanvasBrush backgroundBrush =
                renderNode.Node.IsSelected ?
                    renderNode.Resources.ThemeLightBrush(color) :
                    renderNode.Resources.ThemeNormalBrush(color);

            if (borderRadius > 0)
            {
                session.FillRoundedRectangle(renderNode.Bounds, borderRadius, borderRadius, backgroundBrush);

                session.DrawRoundedRectangle(renderNode.Bounds, borderRadius, borderRadius, borderBrush);
            }
            else
            {
                session.FillRectangle(renderNode.Bounds, backgroundBrush);

                session.DrawRectangle(renderNode.Bounds, borderBrush);
            }

            if (renderNode.Node.Icon != null)
            {
                ICanvasImage image = renderNode.Resources.Image(renderNode.Node);

                if (image != null)
                {
                    Vector2 size = renderNode.Node.IconSize == IconSize.Large ? ImageSizeLarge : ImageSizeSmall;

                    float x = textRenderPosition.X - textOffset;
                    float y = textRenderPosition.Y + ((textRenderSize.Y - size.Y) * 0.5f);

                    session.DrawImage(image, new Rect(x, y, 32, 32), image.GetBounds(session), 1, CanvasImageInterpolation.HighQualityCubic);
                }
            }

            if (renderSelection)
            {
                if (renderNode.Node.IsSelected)
                {
                    Rect2 rect = Rect2.Deflate(renderNode.Bounds, SelectionMargin);

                    if (borderRadius > 0)
                    {
                        session.DrawRoundedRectangle(rect, borderRadius * 1.4f, borderRadius * 1.4f, borderBrush, 2f, SelectionStrokeStyle);
                    }
                    else
                    {
                        session.DrawRectangle(rect, borderBrush, 2f, SelectionStrokeStyle);
                    }
                }
            }
        }
    }
}

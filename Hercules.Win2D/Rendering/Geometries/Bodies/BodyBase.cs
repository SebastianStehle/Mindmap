// ==========================================================================
// BodyBase.cs
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
using Microsoft.Graphics.Canvas.Geometry;

// ReSharper disable ConvertIfStatementToConditionalTernaryExpression

namespace Hercules.Win2D.Rendering.Geometries.Bodies
{
    public abstract class BodyBase : IBodyGeometry
    {
        protected const float MinHeight = 36;
        protected const float IconSizeLarge = 160;
        protected const float IconSizeMedium = 64;
        protected const float IconSizeSmall = 32;
        protected const float IconMargin = 6;
        protected static readonly CanvasStrokeStyle SelectionStrokeStyle = new CanvasStrokeStyle { DashStyle = CanvasDashStyle.Dash };
        private Vector2 textRenderSize;
        private Vector2 textRenderPosition;
        private Vector2 iconRenderSize;
        private Vector2 iconRenderPosition;
        private Vector2 contentRenderSize;
        private Vector2 contentPadding;

        public virtual Vector2 TextRenderPosition
        {
            get { return textRenderPosition; }
        }

        public virtual Vector2 RenderPositionOffset
        {
            get { return Vector2.Zero; }
        }

        public virtual bool HasText
        {
            get { return true; }
        }

        public virtual float VerticalPathOffset
        {
            get { return 0; }
        }

        public virtual void ClearResources()
        {
        }

        public virtual void Arrange(Win2DRenderable renderable, CanvasDrawingSession session)
        {
            Vector2 iconOffset = Vector2.Zero;
            Vector2 textOffset = Vector2.Zero;

            if (renderable.Node.Icon == null)
            {
                textOffset.Y = 0.5f * (contentRenderSize.Y - textRenderSize.Y);
            }
            else if (renderable.Node.IconPosition == IconPosition.Left)
            {
                textOffset.Y = 0.5f * (contentRenderSize.Y - textRenderSize.Y);
                textOffset.X = iconRenderSize.X + IconMargin;

                iconOffset.Y = 0.5f * (contentRenderSize.Y - iconRenderSize.Y);
            }
            else if (renderable.Node.IconPosition == IconPosition.Right)
            {
                textOffset.Y = 0.5f * (contentRenderSize.Y - textRenderSize.Y);

                iconOffset.X = textRenderSize.X + IconMargin;
                iconOffset.Y = 0.5f * (contentRenderSize.Y - iconRenderSize.Y);
            }
            else if (renderable.Node.IconPosition == IconPosition.Top)
            {
                textOffset.X = 0.5f * (contentRenderSize.X - textRenderSize.X);
                textOffset.Y = iconRenderSize.Y + IconMargin;

                iconOffset.X = 0.5f * (contentRenderSize.X - iconRenderSize.X);
            }
            else if (renderable.Node.IconPosition == IconPosition.Bottom)
            {
                textOffset.X = 0.5f * (contentRenderSize.X - textRenderSize.X);

                iconOffset.X = 0.5f * (contentRenderSize.X - iconRenderSize.X);
                iconOffset.Y = textRenderSize.Y + IconMargin;
            }

            textRenderPosition = renderable.RenderPosition + contentPadding + textOffset;
            iconRenderPosition = renderable.RenderPosition + contentPadding + iconOffset;
        }

        public virtual Vector2 Measure(Win2DRenderable renderable, CanvasDrawingSession session, Vector2 textSize)
        {
            textRenderSize = textSize;

            contentRenderSize = textSize;

            if (renderable.Node.Icon != null)
            {
                float targetSize = IconSizeSmall;

                if (renderable.Node.IconSize == IconSize.Medium)
                {
                    targetSize = IconSizeMedium;
                }
                else if (renderable.Node.IconSize == IconSize.Large)
                {
                    targetSize = IconSizeLarge;
                }

                iconRenderSize = new Vector2(renderable.Node.Icon.PixelWidth, renderable.Node.Icon.PixelHeight);

                float ratio = iconRenderSize.X / iconRenderSize.Y;

                if (iconRenderSize.X > iconRenderSize.Y)
                {
                    iconRenderSize = new Vector2(targetSize, targetSize / ratio);
                }
                else
                {
                    iconRenderSize = new Vector2(targetSize * ratio, targetSize);
                }

                if (renderable.Node.IconPosition == IconPosition.Left || renderable.Node.IconPosition == IconPosition.Right)
                {
                    contentRenderSize.X += iconRenderSize.X;
                    contentRenderSize.X += IconMargin;

                    contentRenderSize.Y = Math.Max(contentRenderSize.Y, iconRenderSize.Y);
                }
                else
                {
                    contentRenderSize.Y += iconRenderSize.Y;
                    contentRenderSize.Y += IconMargin;

                    contentRenderSize.X = Math.Max(contentRenderSize.X, iconRenderSize.X);
                }
            }

            contentRenderSize.Y = Math.Max(MinHeight, contentRenderSize.Y);
            contentPadding = CalculatePadding(renderable, contentRenderSize);

            return contentRenderSize + (2 * contentPadding);
        }

        protected void RenderIcon(Win2DRenderable renderable, CanvasDrawingSession session)
        {
            if (renderable.Node.Icon != null)
            {
                ICanvasImage image = renderable.Resources.Image(renderable.Node);

                if (image != null)
                {
                    Rect rect = new Rect(
                        iconRenderPosition.X,
                        iconRenderPosition.Y,
                        iconRenderSize.X,
                        iconRenderSize.Y);

                    session.DrawImage(image, rect, image.GetBounds(session), 1, CanvasImageInterpolation.HighQualityCubic);
                }
            }
        }

        protected virtual Vector2 CalculatePadding(Win2DRenderable renderable, Vector2 contentSize)
        {
            return Vector2.Zero;
        }

        public abstract void Render(Win2DRenderable renderable, CanvasDrawingSession session, Win2DColor color, bool renderSelection);

        public abstract IBodyGeometry Clone();
    }
}

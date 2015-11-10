// ==========================================================================
// ModernPastelRootNode.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Numerics;
using Hercules.Model;
using Hercules.Model.Rendering;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;

namespace Hercules.Win2D.Rendering.Geometries
{
    public class EllipseNode : RenderNodeBase
    {
        private const float MinHeight = 50;
        private static readonly Vector2 ContentPadding = new Vector2(15, 5);
        private static readonly Vector2 SelectionMargin = new Vector2(-5, -5);
        private readonly Win2DTextRenderer textRenderer;
        private float textOffset;

        public override Win2DTextRenderer TextRenderer
        {
            get { return textRenderer; }
        }

        public EllipseNode(NodeBase node, Win2DRenderer renderer)
            : base(node, renderer)
        {
            textRenderer = new Win2DTextRenderer(node) { FontSize = 20, MinWidth = 80 };
        }

        protected override void ArrangeInternal(CanvasDrawingSession session)
        {
            float x = RenderPosition.X, y = Bounds.CenterY;

            x += ContentPadding.X;
            x += textOffset;
            y -= textRenderer.RenderSize.Y * 0.5f;

            textRenderer.Arrange(new Vector2(x, y));

            base.ArrangeInternal(session);
        }

        protected override Vector2 MeasureInternal(CanvasDrawingSession session)
        {
            textRenderer.Measure(session);

            Vector2 size = textRenderer.RenderSize + (2 * ContentPadding);

            if (Node.Icon != null)
            {
                if (Node.IconSize == IconSize.Small)
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

        protected override void RenderInternal(CanvasDrawingSession session, LayoutThemeColor color, bool renderControls)
        {
            ICanvasBrush borderBrush = Resources.ThemeDarkBrush(color);

            ICanvasBrush backgroundBrush =
                Node.IsSelected ?
                    Resources.ThemeLightBrush(color) :
                    Resources.ThemeNormalBrush(color);

            float radiusX = 0.5f * RenderSize.X;
            float radiusY = 0.5f * RenderSize.Y;

            session.FillEllipse(
                Bounds.Center,
                radiusX,
                radiusY,
                backgroundBrush);

            session.DrawEllipse(
                Bounds.Center,
                radiusX,
                radiusY,
                borderBrush);

            if (Node.Icon != null)
            {
                ICanvasImage image = Resources.Image(Node.Icon);

                if (image != null)
                {
                    Vector2 size = Node.IconSize == IconSize.Large ? ImageSizeLarge : ImageSizeSmall;

                    float x = textRenderer.RenderPosition.X - textOffset + ImageMargin;
                    float y = textRenderer.RenderPosition.Y + ((textRenderer.RenderSize.Y - size.Y) * 0.5f);

                    session.DrawImage(image, x, y);
                }
            }

            textRenderer.Render(session);

            if (renderControls)
            {
                if (Node.IsSelected)
                {
                    radiusX -= SelectionMargin.X;
                    radiusY -= SelectionMargin.Y;

                    session.DrawEllipse(
                        Bounds.Center,
                        radiusX,
                        radiusY,
                        borderBrush, 2f, SelectionStrokeStyle);
                }

                Button.Render(session);
            }
        }

        protected override Win2DRenderNode CloneInternal()
        {
            return new EllipseNode(Node, Renderer);
        }
    }
}

// ==========================================================================
// ModernPastelLevel1Node.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Numerics;
using Hercules.Model;
using Hercules.Model.Rendering;
using Hercules.Model.Utils;
using Hercules.Win2D.Rendering.Utils;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;

namespace Hercules.Win2D.Rendering.Themes.ModernPastel
{
    public sealed class ModernPastelLevel1Node : ModernPastelRenderNode
    {
        private const float MinHeight = 40;
        private static readonly Vector2 ContentPadding = new Vector2(15, 5);
        private static readonly Vector2 SelectionMargin = new Vector2(-5, -5);
        private readonly Win2DTextRenderer textRenderer;
        private float textOffset;
        private CanvasGeometry pathGeometry;

        public override Win2DTextRenderer TextRenderer
        {
            get { return textRenderer; }
        }

        public ModernPastelLevel1Node(NodeBase node, Win2DRenderer renderer)
            : base(node, renderer)
        {
            textRenderer = new Win2DTextRenderer(node) { FontSize = 16, MinWidth = 50 };
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

            if (!string.IsNullOrWhiteSpace(Node.IconKey))
            {
                if (Node.IconSize == IconSize.Small)
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

        public override void ClearResources()
        {
            base.ClearResources();

            ClearPath();
        }

        private void ClearPath()
        {
            if (pathGeometry != null)
            {
                pathGeometry.Dispose();
                pathGeometry = null;
            }
        }

        public override void ComputePath(CanvasDrawingSession session)
        {
            ClearPath();

            pathGeometry = GeometryBuilder.ComputeFilledPath(this, Parent, session);
        }

        protected override void RenderInternal(CanvasDrawingSession session, LayoutThemeColor color, bool renderControls)
        {
            ICanvasBrush borderBrush = Resources.ThemeDarkBrush(color);

            ICanvasBrush backgroundBrush =
                Node.IsSelected ?
                    Resources.ThemeLightBrush(color) :
                    Resources.ThemeNormalBrush(color);

            session.FillRoundedRectangle(Bounds, 10, 10, backgroundBrush);
            session.DrawRoundedRectangle(Bounds, 10, 10, borderBrush);

            if (!string.IsNullOrWhiteSpace(Node.IconKey))
            {
                ICanvasImage image = Resources.Image(Node.IconKey);

                if (image != null)
                {
                    Vector2 size = Node.IconSize == IconSize.Large ? ImageSizeLarge : ImageSizeSmall;

                    float x = textRenderer.RenderPosition.X - textOffset;
                    float y = textRenderer.RenderPosition.Y + ((textRenderer.RenderSize.Y - size.Y) * 0.5f);

                    session.DrawImage(image, x, y);
                }
            }

            textRenderer.Render(session);

            if (renderControls)
            {
                if (Node.IsSelected)
                {
                    Rect2 rect = Rect2.Deflate(Bounds, SelectionMargin);

                    session.DrawRoundedRectangle(rect, 14, 14, borderBrush, 2f, SelectionStrokeStyle);
                }

                if (Node.HasChildren)
                {
                    Button.Render(session);
                }
            }
        }

        protected override void RenderPathInternal(CanvasDrawingSession session)
        {
            if (pathGeometry != null)
            {
                ICanvasBrush brush = Resources.Brush(PathColor, 1);

                session.FillGeometry(pathGeometry, brush);
            }
        }

        protected override Win2DRenderNode CloneInternal()
        {
            return new ModernPastelLevel1Node(Node, (ModernPastelRenderer)Renderer);
        }
    }
}

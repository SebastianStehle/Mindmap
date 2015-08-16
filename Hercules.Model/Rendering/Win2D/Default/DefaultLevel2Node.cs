// ==========================================================================
// DefaultLevel2Node.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
using System;
using System.Numerics;
using Hercules.Model.Utils;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;

namespace Hercules.Model.Rendering.Win2D.Default
{
    public sealed class DefaultLevel2Node : DefaultRenderNode
    {
        private static readonly Vector2 ContentPadding = new Vector2(15, 5);
        private static readonly float MinHeight = 40;
        private static readonly float VerticalOffset = 15;
        private static readonly CanvasStrokeStyle StrokeStyle = new CanvasStrokeStyle { StartCap = CanvasCapStyle.Round, EndCap = CanvasCapStyle.Round };
        private readonly Win2DTextRenderer textRenderer;
        private float textOffset;

        public override Win2DTextRenderer TextRenderer
        {
            get
            {
                return textRenderer;
            }
        }

        public override float VerticalPathOffset
        {
            get
            {
                return VerticalOffset;
            }
        }

        public override Vector2 RenderPositionOffset
        {
            get
            {
                return new Vector2(0, -VerticalOffset);
            }
        }

        public DefaultLevel2Node(NodeBase node, DefaultRenderer renderer) 
            : base(node, renderer)
        {

            textRenderer = new Win2DTextRenderer(14, node, 60);
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

            Vector2 size = textRenderer.RenderSize + 2 * ContentPadding;

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

        protected override void RenderInternal(CanvasDrawingSession session, ThemeColor color)
        {
            ICanvasBrush borderBrush = Resources.ThemeDarkBrush(color);

            ICanvasBrush lineBrush = Resources.Brush(PathColor, 1);
            
            Vector2 left = new Vector2(
                (float)Math.Round(Bounds.Left -1) ,
                (float)Math.Round(Bounds.CenterY) + VerticalOffset);

            Vector2 right = new Vector2(
                (float)Math.Round(Bounds.Right + 1),
                (float)Math.Round(Bounds.CenterY) + VerticalOffset);

            session.DrawLine(left, right, lineBrush, 2, StrokeStyle);

            if (!string.IsNullOrWhiteSpace(Node.IconKey))
            {
                ICanvasImage image = Resources.Image(Node.IconKey);

                if (image != null)
                {
                    Vector2 size = Node.IconSize == IconSize.Large ? ImageSizeLarge : ImageSizeSmall;

                    float x = textRenderer.RenderPosition.X - textOffset;
                    float y = textRenderer.RenderPosition.Y + (textRenderer.RenderSize.Y - size.Y) * 0.5f;

                    session.DrawImage(image, x, y);
                }
            }

            textRenderer.Render(session);

            if (!HideControls)
            {
                if (Node.IsSelected)
                {
                    session.DrawRoundedRectangle(Bounds, 14, 14, borderBrush, 2f, SelectionStrokeStyle);
                }

                if (Node.HasChildren)
                {
                    Button.Render(session);
                }
            }
        }

        protected override Win2DRenderNode CloneInternal()
        {
            return new DefaultLevel2Node(Node, (DefaultRenderer)Renderer);
        }
    }
}

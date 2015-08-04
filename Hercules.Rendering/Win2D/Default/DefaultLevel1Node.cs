using System;
using System.Numerics;
using Hercules.Model;
using Hercules.Model.Utils;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;

namespace Hercules.Rendering.Win2D.Default
{
    public sealed class DefaultLevel1Node : DefaultRenderNode
    {
        private static readonly Vector2 ContentPadding = new Vector2(15, 5);
        private static readonly Vector2 SelectionMargin = new Vector2(-5, -5);
        private static readonly float MinHeight = 40;
        private static readonly float MinWidth = 80;
        private readonly TextRenderer textRenderer;

        public override TextRenderer TextRenderer
        {
            get
            {
                return textRenderer;
            }
        }

        public DefaultLevel1Node(NodeBase node, DefaultRenderer renderer) 
            : base(node, renderer)
        {
            textRenderer = new TextRenderer(14, node);
        }

        protected override void ArrangeInternal(CanvasDrawingSession session)
        {
            Vector2 textPosition = Bounds.Center;

            textPosition.X -= textRenderer.Size.X * 0.5f;
            textPosition.Y -= textRenderer.Size.Y * 0.5f;

            textRenderer.Arrange(textPosition);

            base.ArrangeInternal(session);
        }

        protected override Vector2 MeasureInternal(CanvasDrawingSession session)
        {
            textRenderer.Measure(session);

            Vector2 size = textRenderer.Size + 2 * ContentPadding;

            size.X = Math.Max(size.X, MinWidth);
            size.Y = Math.Max(size.Y, MinHeight);

            return size;
        }

        protected override void RenderInternal(CanvasDrawingSession session, ThemeColor color)
        {
            ICanvasBrush borderBrush = color.DarkBrush(session);

            ICanvasBrush backgroundBrush =
                Node.IsSelected ?
                    color.LightBrush(session) :
                    color.NormalBrush(session);
            
            session.FillRoundedRectangle(Bounds, 10, 10, backgroundBrush);
            session.DrawRoundedRectangle(Bounds, 10, 10, borderBrush);

            textRenderer.Render(session);

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
}

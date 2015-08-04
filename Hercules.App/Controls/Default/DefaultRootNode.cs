using System.Numerics;
using Hercules.Model;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using System;
using Windows.UI;
using Windows.Foundation;

namespace Hercules.App.Controls.Default
{
    public sealed class DefaultRootNode : DefaultRenderNode
    {
        private static readonly Vector2 ContentPadding = new Vector2(15, 5);
        private static readonly Vector2 SelectionMargin = new Vector2(-5, -5);
        private static readonly CanvasStrokeStyle StrokeStyle = new CanvasStrokeStyle { DashStyle = CanvasDashStyle.Dash };
        private static readonly float MinHeight = 50;
        private static readonly float MinWidth = 100;
        private readonly CanvasTextFormat textFormat = new CanvasTextFormat { FontSize = 16.0f, LineSpacing = 24, LineSpacingBaseline = 20, WordWrapping = CanvasWordWrapping.NoWrap, VerticalAlignment = CanvasVerticalAlignment.Center };
        private CanvasTextLayout textLayout;
        private Vector2 textSize;
        private Vector2 textOffset;
        private TextRenderer textRenderer;
        
        public DefaultRootNode(NodeBase node, DefaultRenderer renderer) 
            : base(node, renderer)
        {
            textRenderer = new TextRenderer(16, node);
        }

        protected override void ArrangeInternal(CanvasDrawingSession session)
        {
            base.ArrangeInternal(session);
        }

        protected override Vector2 MeasureInternal(CanvasDrawingSession session)
        {
            Vector2 size = Vector2.Zero;

            if (!string.IsNullOrWhiteSpace(Node.Text))
            {
                textLayout = new CanvasTextLayout(session, Node.Text, textFormat, 0.0f, 0.0f);

                textSize = new Vector2(
                    (float)textLayout.DrawBounds.Width, 
                    (float)textLayout.DrawBounds.Height);

                textOffset = new Vector2(
                    (float)-textLayout.DrawBounds.X,
                    (float)-textLayout.DrawBounds.Y);

                size = textSize;
            }

            size = size + 2 * ContentPadding;

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

            float radiusX = 0.5f * Size.X;
            float radiusY = 0.5f * Size.Y;

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
            
            if (!string.IsNullOrWhiteSpace(Node.Text))
            {
                Vector2 textPosition = Bounds.Center;

                textPosition.X += -textSize.X * 0.5f + textOffset.X;
                textPosition.Y += -textSize.Y * 0.5f + textOffset.Y;
                
                session.DrawText(Node.Text, textPosition, Colors.Black, textFormat);
            }

            if (Node.IsSelected)
            {
                radiusX = 0.5f * Size.X - SelectionMargin.X;
                radiusY = 0.5f * Size.Y - SelectionMargin.Y;

                session.DrawEllipse(
                    Bounds.Center,
                    radiusX,
                    radiusY,
                    borderBrush, 2f, StrokeStyle);
            }

            Button.Render(session);
        }
    }
}

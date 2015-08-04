using System;
using System.Numerics;
using Hercules.Model;
using Hercules.Model.Utils;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Windows.UI;

namespace Hercules.App.Controls.Default
{
    public sealed class DefaultLevel2Node : DefaultRenderNode
    {
        private static readonly Vector2 ContentPadding = new Vector2(15, 5);
        private static readonly Vector2 SelectionMargin = new Vector2(-5, -5);
        private static readonly CanvasStrokeStyle StrokeStyle = new CanvasStrokeStyle { DashStyle = CanvasDashStyle.Dash };
        private static readonly float MinHeight = 40;
        private static readonly float MinWidth = 80;
        private readonly CanvasTextFormat textFormat = new CanvasTextFormat { FontSize = 14.0f, WordWrapping = CanvasWordWrapping.NoWrap, VerticalAlignment = CanvasVerticalAlignment.Top };
        private Vector2 textSize;
        
        public DefaultLevel2Node(NodeBase node, DefaultRenderer renderer) 
            : base(node, renderer)
        {
        }

        protected override Vector2 MeasureInternal(CanvasDrawingSession session)
        {
            Vector2 size = Vector2.Zero;

            if (!string.IsNullOrWhiteSpace(Node.Text))
            {
                CanvasTextLayout textLayout = new CanvasTextLayout(session, Node.Text, textFormat, 0.0f, 0.0f);

                textSize = new Vector2(
                    (float)textLayout.DrawBounds.Width,
                    (float)textLayout.DrawBounds.Height);

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
            
            Vector2 l = new Vector2(Bounds.Left, Bounds.CenterY);
            Vector2 r = new Vector2(Bounds.Right, Bounds.CenterY);

            session.DrawLine(l, r, Colors.Black, 2);

            if (!string.IsNullOrWhiteSpace(Node.Text))
            {
                Vector2 textPosition = Bounds.Center;

                textPosition.X -= textSize.X * 0.5f;
                textPosition.Y -= textSize.Y * 1.8f;

                session.DrawText(Node.Text, textPosition, Colors.Black, textFormat);
            }

            if (Node.IsSelected)
            {
                Rect2 rect = Rect2.Deflate(Bounds, SelectionMargin);

                rect = new Rect2(new Vector2(rect.X, rect.Y - 10), rect.Size);

                session.DrawRoundedRectangle(rect, 14, 14, borderBrush, 2f, StrokeStyle);
            }

            if (Node.HasChildren)
            {
                Button.Render(session);
            }
        }
    }
}

using System;
using System.Numerics;
using Hercules.Model;
using Hercules.Model.Utils;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;

namespace Hercules.App.Controls.Default
{
    public abstract class DefaultRenderNode : ThemeRenderNode
    {
        protected static readonly CanvasStrokeStyle SelectionStrokeStyle = new CanvasStrokeStyle { DashStyle = CanvasDashStyle.Dash };

        private readonly ExpandButton button;

        protected ExpandButton Button
        {
            get
            {
                return button;
            }
        }

        public DefaultRenderNode(NodeBase node, DefaultRenderer renderer)
            : base(node, renderer)
        {
            button = new ExpandButton(node);
        }

        public override bool HitTest(Vector2 position)
        {
            return Bounds.Contains(position);
        }

        public override bool HandleClick(Vector2 position)
        {
            return button.HitTest(position) || base.HandleClick(position);
        }

        protected override void ArrangeInternal(CanvasDrawingSession session)
        {
            Vector2 position;

            if (Node.NodeSide == NodeSide.Left)
            {
                position = new Vector2(
                    Position.X - 2,
                    Position.Y + Size.Y * 0.5f);
            }
            else
            {
                position = new Vector2(
                    Position.X + Size.X + 2,
                    Position.Y + Size.Y * 0.5f);
            }

            button.Arrange(position, 10);

            base.ArrangeInternal(session);
        }

        protected override void RenderPathInternal(CanvasDrawingSession session)
        {
            if (Parent != null && IsVisible)
            {
                Rect2 targetRect = Bounds;
                Rect2 parentRect = Parent.Bounds;
                
                Vector2 point1 = Vector2.Zero;
                Vector2 point2 = Vector2.Zero;

                if (CalculateCenterX(targetRect) > CalculateCenterX(parentRect))
                {
                    CalculateCenterL(targetRect, ref point1);
                    CalculateCenterR(parentRect, ref point2);
                }
                else
                {
                    CalculateCenterR(targetRect, ref point1);
                    CalculateCenterL(parentRect, ref point2);
                }

                point1.X = (float)Math.Round(point1.X);
                point1.Y = (float)Math.Round(point1.Y);
                point2.X = (float)Math.Round(point2.X);
                point2.Y = (float)Math.Round(point2.Y);

                float halfX = (point1.X + point2.X) * 0.5f;

                using (CanvasPathBuilder builder = new CanvasPathBuilder(session.Device))
                {
                    builder.BeginFigure(new Vector2(point1.X, point1.Y));

                    builder.AddCubicBezier(
                        new Vector2(halfX, point1.Y),
                        new Vector2(halfX, point2.Y),
                        new Vector2(point2.X, point2.Y));

                    builder.EndFigure(CanvasFigureLoop.Open);

                    using (CanvasGeometry pathGeometry = CanvasGeometry.CreatePath(builder))
                    {
                        ICanvasBrush brush = Renderer.PathBrush(session);

                        session.DrawGeometry(pathGeometry, brush, 2);
                    }
                }
            }
        }

        private static void CalculateCenterL(Rect2 rect, ref Vector2 point)
        {
            point.X = rect.X;
            point.Y = rect.Y + (rect.Height * 0.5f);
        }

        private static void CalculateCenterR(Rect2 rect, ref Vector2 point)
        {
            point.X = rect.X + (rect.Width);
            point.Y = rect.Y + (rect.Height * 0.5f);
        }

        private static double CalculateCenterX(Rect2 rect)
        {
            return rect.X + (rect.Width * 0.5);
        }
    }
}

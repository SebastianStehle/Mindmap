using System;
using System.Numerics;
using GP.Windows;
using Hercules.Model.Utils;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;

namespace Hercules.Model.Rendering.Win2D.Default
{
    public abstract class DefaultRenderNode : Win2DRenderNode
    {
        protected static readonly Vector2 ImageSizeLarge = new Vector2(64, 64);
        protected static readonly Vector2 ImageSizeSmall = new Vector2(32, 32);
        protected static readonly float ImageMargin = 10;
        protected static readonly CanvasStrokeStyle SelectionStrokeStyle = new CanvasStrokeStyle { DashStyle = CanvasDashStyle.Dash };

        private readonly ExpandButton button;

        protected ExpandButton Button
        {
            get
            {
                return button;
            }
        }

        protected DefaultRenderNode(NodeBase node, DefaultRenderer renderer)
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
            Vector2 buttonPosition;

            if (Node.NodeSide == NodeSide.Left)
            {
                buttonPosition = new Vector2(
                    RenderPosition.X - 2,
                    RenderPosition.Y + RenderSize.Y * 0.5f);
            }
            else
            {
                buttonPosition = new Vector2(
                    RenderPosition.X + RenderSize.X + 2,
                    RenderPosition.Y + RenderSize.Y * 0.5f);
            }

            button.Arrange(buttonPosition);

            base.ArrangeInternal(session);
        }

        protected override void RenderPathInternal(CanvasDrawingSession session)
        {
            ICanvasBrush brush = Renderer.PathBrush(session);

            RenderPath(session, brush);
        }

        protected void RenderPath(CanvasDrawingSession session, ICanvasBrush brush)
        {
            Guard.NotNull(session, nameof(session));
            Guard.NotNull(brush, nameof(brush));

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

                point1.X = (float) Math.Round(point1.X);
                point1.Y = (float) Math.Round(point1.Y);
                point2.X = (float) Math.Round(point2.X);
                point2.Y = (float) Math.Round(point2.Y);

                float halfX = (point1.X + point2.X)*0.5f;

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

using System;
using System.Numerics;
using Hercules.Model;
using Hercules.Model.Utils;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;

namespace Hercules.App.Controls.Default
{
    public sealed class DefaultLevel1Node : ThemeRenderNode
    {
        private static readonly Vector2 ContentMargin = new Vector2(15, 5);
        private static readonly Vector2 ContentPadding = new Vector2(15, 5);
        private static readonly Vector2 SelectionMargin = new Vector2(10, 0);
        private static readonly CanvasStrokeStyle StrokeStyle = new CanvasStrokeStyle { DashStyle = CanvasDashStyle.Dash };

        public override Vector2 AnchorPosition
        {
            get
            {
                return new Vector2(15, 0);
            }
        }
        
        public DefaultLevel1Node(NodeBase node, DefaultRenderer renderer) 
            : base(node, renderer)
        {
        }

        public override bool HitTest(Vector2 position)
        {
            return Bounds.Contains(position);
        }

        protected override Vector2 MeasureInternal(CanvasDrawingSession session)
        {
            return new Vector2(200, 100);
        }

        protected override void RenderInternal(CanvasDrawingSession session, ThemeColor color)
        {
            ICanvasBrush borderBrush = color.DarkBrush(session);

            ICanvasBrush backgroundBrush =
                Node.IsSelected ?
                    color.LightBrush(session) :
                    color.NormalBrush(session);

            Rect2 rect = Rect2.Deflate(Bounds, ContentMargin);

            session.FillRoundedRectangle(rect, 10, 10, backgroundBrush);
            session.DrawRoundedRectangle(rect, 10, 10, borderBrush);

            if (Node.IsSelected)
            {
                rect = Rect2.Deflate(Bounds, SelectionMargin);

                session.DrawRoundedRectangle(rect, 14, 14, borderBrush, 2f, StrokeStyle);
            }
        }

        protected override void RenderPathInternal(CanvasDrawingSession session)
        {
            if (Parent != null && IsVisible)
            {
                Rect2 targetRect = Bounds;
                Rect2 parentRect = Parent.Bounds;

                Vector2 targetAnchorPosition = AnchorPosition;
                Vector2 parentAnchorPosition = Parent.AnchorPosition;

                Vector2 point1 = Vector2.Zero;
                Vector2 point2 = Vector2.Zero;

                if (CalculateCenterX(targetRect) > CalculateCenterX(parentRect))
                {
                    CalculateCenterL(targetRect, targetAnchorPosition, ref point1);
                    CalculateCenterR(parentRect, parentAnchorPosition, ref point2);
                }
                else
                {
                    CalculateCenterR(targetRect, targetAnchorPosition, ref point1);
                    CalculateCenterL(parentRect, parentAnchorPosition, ref point2);
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

        private static void CalculateCenterL(Rect2 rect, Vector2 anchorPosition, ref Vector2 point)
        {
            point.X = rect.X + anchorPosition.X;
            point.Y = rect.Y + anchorPosition.Y + (rect.Height * 0.5f);
        }

        private static void CalculateCenterR(Rect2 rect, Vector2 anchorPosition, ref Vector2 point)
        {
            point.X = rect.X - anchorPosition.X + (rect.Width);
            point.Y = rect.Y + anchorPosition.Y + (rect.Height * 0.5f);
        }

        private static double CalculateCenterX(Rect2 rect)
        {
            return rect.X + (rect.Width * 0.5);
        }
    }
}

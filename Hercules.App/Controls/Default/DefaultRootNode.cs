using System.Numerics;
using Hercules.Model;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;

namespace Hercules.App.Controls.Default
{
    public sealed class DefaultRootNode : ThemeRenderNode
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
        
        public DefaultRootNode(NodeBase node, DefaultRenderer renderer) 
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

            float radiusX = 0.5f * Size.X - ContentMargin.X;
            float radiusY = 0.5f * Size.Y - ContentMargin.Y;

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
        }

        protected override void RenderPathInternal(CanvasDrawingSession session)
        {
        }
    }
}

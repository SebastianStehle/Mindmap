using System.Numerics;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;

namespace Hercules.Model.Rendering.Win2D.Default
{
    public sealed class DefaultPreviewNode : DefaultRootNode
    {
        private static readonly Vector2 Size = new Vector2(80, 20);
        private ICanvasBrush brush;

        public DefaultPreviewNode(NodeBase node, DefaultRenderer renderer) 
            : base(node, renderer)
        {
        }

        protected override Vector2 MeasureInternal(CanvasDrawingSession session)
        {
            return Size;
        }

        protected override void RenderPathInternal(CanvasDrawingSession session)
        {
            RenderPath(session, CreateBrush(session));
        }

        protected override void RenderInternal(CanvasDrawingSession session, ThemeColor color)
        {
            if (Parent != null)
            {
                session.FillRoundedRectangle(Bounds, 4, 4, CreateBrush(session));
            }
        }

        private ICanvasBrush CreateBrush(CanvasDrawingSession session)
        {
            if (brush == null)
            {
                brush = Renderer.PathBrush(session, true);

                brush.Opacity = 0.5f;
            }

            return brush;
        }
    }
}

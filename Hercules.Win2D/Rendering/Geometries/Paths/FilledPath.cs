// ==========================================================================
// FilledPath.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI;
using Hercules.Win2D.Rendering.Utils;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;

namespace Hercules.Win2D.Rendering.Geometries.Paths
{
    public class FilledPath : GeometryPathBase
    {
        public FilledPath(Color color, float opacity = 1)
            : base(color, opacity)
        {
        }

        protected override CanvasGeometry CreateGeometry(CanvasDrawingSession session, Win2DRenderNode renderNode)
        {
            return GeometryBuilder.ComputeFilledPath(renderNode, renderNode.Parent, session);
        }

        protected override void RenderGeometry(Win2DRenderable renderable, CanvasDrawingSession session, CanvasGeometry geometry, ICanvasBrush brush)
        {
            session.FillGeometry(geometry, brush);
        }
    }
}

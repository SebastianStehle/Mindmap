// ==========================================================================
// LinePath.cs
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
    public class LinePath : IPathGeometry
    {
        private readonly Color color;
        private readonly float opacity;
        private CanvasGeometry pathGeometry;

        public LinePath(Color color, float opacity = 1)
        {
            this.color = color;

            this.opacity = opacity;
        }

        public void ClearResources()
        {
            if (pathGeometry != null)
            {
                pathGeometry.Dispose();
                pathGeometry = null;
            }
        }

        public void Arrange(Win2DRenderable renderable, CanvasDrawingSession session)
        {
            ClearResources();

            Win2DRenderNode renderNode = renderable as Win2DRenderNode;

            if (renderNode?.Parent != null)
            {
                pathGeometry = GeometryBuilder.ComputeLinePath(renderNode, renderNode.Parent, session);
            }
        }

        public void Render(Win2DRenderable renderable, CanvasDrawingSession session)
        {
            if (pathGeometry != null)
            {
                ICanvasBrush brush = renderable.Resources.Brush(color, opacity);

                session.DrawGeometry(pathGeometry, brush, 2);
            }
        }
    }
}

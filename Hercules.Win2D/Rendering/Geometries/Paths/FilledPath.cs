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
    public class FilledPath : IPathGeometry
    {
        private readonly Color color;
        private readonly float opacity;
        private ICanvasBrush brush;
        private CanvasGeometry pathGeometry;

        public FilledPath(Color color, float opacity = 1)
        {
            this.color = color;

            this.opacity = opacity;
        }

        public void ClearResources()
        {
            ClearGeometry();

            if (brush != null)
            {
                brush.Dispose();
                brush = null;
            }
        }

        private void ClearGeometry()
        {
            if (pathGeometry != null)
            {
                pathGeometry.Dispose();
                pathGeometry = null;
            }
        }

        public void Arrange(Win2DRenderNode renderNode, CanvasDrawingSession session)
        {
            ClearGeometry();

            if (renderNode?.Parent != null)
            {
                pathGeometry = GeometryBuilder.ComputeFilledPath(renderNode, renderNode.Parent, session);
            }
        }

        public void Render(Win2DRenderNode renderNode, CanvasDrawingSession session)
        {
            if (pathGeometry != null)
            {
                if (brush == null)
                {
                    brush = renderNode.Resources.Brush(color, opacity);
                }

                session.FillGeometry(pathGeometry, brush);
            }
        }
    }
}

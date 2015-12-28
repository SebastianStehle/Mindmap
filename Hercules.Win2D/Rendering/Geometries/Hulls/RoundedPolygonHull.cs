// ==========================================================================
// RoundedPolygonHull.cs
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

namespace Hercules.Win2D.Rendering.Geometries.Hulls
{
    public class RoundedPolygonHull : IHullGeometry
    {
        private Color lastColor;
        private ICanvasBrush brush;
        private CanvasGeometry hullGeometry;

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
            if (hullGeometry != null)
            {
                hullGeometry.Dispose();
                hullGeometry = null;
            }
        }

        public void Arrange(Win2DRenderNode renderNode, CanvasDrawingSession session)
        {
            ClearGeometry();

            hullGeometry = GeometryBuilder.ComputeHullGeometry(session, renderNode.Scene, renderNode);
        }

        public void Render(Win2DRenderNode renderNode, CanvasDrawingSession session, Color color)
        {
            if (hullGeometry != null)
            {
                if (brush == null || lastColor != color)
                {
                    if (brush != null)
                    {
                        brush.Dispose();
                    }

                    brush = renderNode.Resources.Brush(color, 1);

                    lastColor = color;
                }

                session.DrawGeometry(hullGeometry, brush, 2);
            }
        }
    }
}

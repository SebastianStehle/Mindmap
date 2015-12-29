// ==========================================================================
// RoundedPolygonHull.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Hercules.Win2D.Rendering.Utils;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;

namespace Hercules.Win2D.Rendering.Geometries.Hulls
{
    public class RoundedPolygonHull : IHullGeometry
    {
        private CanvasGeometry hullGeometry;

        public void ClearResources()
        {
            if (hullGeometry != null)
            {
                hullGeometry.Dispose();
                hullGeometry = null;
            }
        }

        public void Arrange(Win2DRenderable renderable, CanvasDrawingSession session)
        {
            ClearResources();

            Win2DRenderNode renderNode = renderable as Win2DRenderNode;

            if (renderNode != null)
            {
                hullGeometry = GeometryBuilder.ComputeHullGeometry(session, renderable.Scene, renderNode);
            }
        }

        public void Render(Win2DRenderable renderable, CanvasDrawingSession session, Win2DColor color)
        {
            if (hullGeometry != null)
            {
                session.DrawGeometry(hullGeometry, renderable.Resources.Brush(color.Normal, 1.0f), 1f);

                session.FillGeometry(hullGeometry, renderable.Resources.Brush(color.Lighter, 0.5f));
            }
        }
    }
}

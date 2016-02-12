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

namespace Hercules.Win2D.Rendering.Parts.Hulls
{
    public class RoundedPolygonHull : IHullPart
    {
        private CanvasGeometry hullGeometry;

        public void ClearResources()
        {
            hullGeometry?.Dispose();
            hullGeometry = null;
        }

        public void Arrange(Win2DRenderable renderable, ICanvasResourceCreator resourceCreator)
        {
            ClearResources();

            Win2DRenderNode renderNode = renderable as Win2DRenderNode;

            if (renderNode != null)
            {
                hullGeometry = GeometryBuilder.ComputeHullGeometry(resourceCreator, renderable.Scene, renderNode);
            }
        }

        public void Render(Win2DRenderable renderable, CanvasDrawingSession session, Win2DColor color, bool renderControls)
        {
            if (hullGeometry == null)
            {
                return;
            }

            session.DrawGeometry(hullGeometry, renderable.Resources.Brush(color.Normal, 1.0f), 1f);
            session.FillGeometry(hullGeometry, renderable.Resources.Brush(color.Lighter, 0.5f));
        }
    }
}

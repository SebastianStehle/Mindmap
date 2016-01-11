// ==========================================================================
// GeometryPathBase.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Numerics;
using Windows.UI;
using GP.Utils.Mathematics;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;

namespace Hercules.Win2D.Rendering.Geometries.Paths
{
    public abstract class GeometryPathBase : IPathGeometry
    {
        private readonly Color color;
        private readonly float opacity;
        private Vector2 lastActualPosition = MathHelper.PositiveInfinityVector;
        private Vector2 lastParentPosition = MathHelper.PositiveInfinityVector;
        private CanvasGeometry pathGeometry;

        protected GeometryPathBase(Color color, float opacity = 1)
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

        public void Arrange(Win2DRenderable renderable, ICanvasResourceCreator resourceCreator)
        {
            Win2DRenderNode renderNode = renderable as Win2DRenderNode;

            if (renderNode?.Parent != null)
            {
                Vector2 currentPosition = renderable.RenderPosition;
                Vector2 parentPosition = renderNode.Parent.RenderPosition;

                if ((lastActualPosition != currentPosition) || (lastParentPosition != parentPosition))
                {
                    lastActualPosition = currentPosition;
                    lastParentPosition = parentPosition;

                    ClearResources();

                    pathGeometry = CreateGeometry(renderNode, resourceCreator);
                }
            }
        }

        protected abstract CanvasGeometry CreateGeometry(Win2DRenderNode renderNode, ICanvasResourceCreator resourceCreator);

        public void Render(Win2DRenderable renderable, CanvasDrawingSession session)
        {
            if (pathGeometry != null)
            {
                RenderGeometry(renderable, session, pathGeometry, renderable.Resources.Brush(color, opacity));
            }
        }

        protected abstract void RenderGeometry(Win2DRenderable renderable, CanvasDrawingSession session, CanvasGeometry geometry, ICanvasBrush brush);
    }
}

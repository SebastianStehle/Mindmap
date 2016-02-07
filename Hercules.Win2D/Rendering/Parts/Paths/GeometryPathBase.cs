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

namespace Hercules.Win2D.Rendering.Parts.Paths
{
    public abstract class GeometryPathBase : IPathPart
    {
        private readonly Color strokeColor;
        private readonly float opacity;
        private Vector2 lastActualPosition = MathHelper.PositiveInfinityVector2;
        private Vector2 lastParentPosition = MathHelper.PositiveInfinityVector2;
        private CanvasGeometry pathGeometry;

        protected GeometryPathBase(Color strokeColor, float opacity = 1)
        {
            this.strokeColor = strokeColor;

            this.opacity = opacity;
        }

        public void ClearResources()
        {
            pathGeometry?.Dispose();
            pathGeometry = null;
        }

        public void Arrange(Win2DRenderable renderable, ICanvasResourceCreator resourceCreator)
        {
            Win2DRenderNode renderNode = renderable as Win2DRenderNode;

            if (renderNode?.Parent == null)
            {
                return;
            }

            Vector2 currentPosition = renderable.RenderPosition;
            Vector2 parentPosition = renderNode.Parent.RenderPosition;

            if ((lastActualPosition == currentPosition) && (lastParentPosition == parentPosition))
            {
                return;
            }

            lastActualPosition = currentPosition;
            lastParentPosition = parentPosition;

            ClearResources();

            pathGeometry = CreateGeometry(renderNode, resourceCreator);
        }

        protected abstract CanvasGeometry CreateGeometry(Win2DRenderNode renderNode, ICanvasResourceCreator resourceCreator);

        public void Render(Win2DRenderable renderable, CanvasDrawingSession session, Win2DColor color, bool renderControls)
        {
            if (pathGeometry != null)
            {
                RenderInternal(renderable, session, pathGeometry, renderable.Resources.Brush(strokeColor, opacity));
            }
        }

        protected abstract void RenderInternal(Win2DRenderable renderable, CanvasDrawingSession session, CanvasGeometry geometry, ICanvasBrush brush);
    }
}

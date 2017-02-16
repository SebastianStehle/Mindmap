// ==========================================================================
// GeometryPathBase.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.UI;
using GP.Utils;
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
        private CanvasGeometry[] pathGeometries;

        protected GeometryPathBase(Color strokeColor, float opacity = 1)
        {
            this.strokeColor = strokeColor;

            this.opacity = opacity;
        }

        public void ClearResources()
        {
            pathGeometries?.Foreach(x => x.Dispose());
            pathGeometries = null;
        }

        public void Arrange(Win2DRenderable renderable, ICanvasResourceCreator resourceCreator)
        {
            var renderNode = renderable as Win2DRenderNode;

            if (renderNode?.Parent == null)
            {
                return;
            }

            var currentPosition = renderable.RenderPosition;
            var parentPosition = renderNode.Parent.RenderPosition;

            if ((lastActualPosition == currentPosition) && (lastParentPosition == parentPosition))
            {
                return;
            }

            lastActualPosition = currentPosition;
            lastParentPosition = parentPosition;

            ClearResources();

            pathGeometries = CreateGeometries(renderNode, resourceCreator).ToArray();
        }

        protected abstract IEnumerable<CanvasGeometry> CreateGeometries(Win2DRenderNode renderNode, ICanvasResourceCreator resourceCreator);

        public void Render(Win2DRenderable renderable, CanvasDrawingSession session, Win2DColor color, bool renderControls)
        {
            if (pathGeometries != null)
            {
                RenderInternal(session, pathGeometries, renderable.Resources.Brush(strokeColor, opacity));
            }
        }

        protected abstract void RenderInternal(CanvasDrawingSession session, CanvasGeometry[] geometries, ICanvasBrush brush);
    }
}

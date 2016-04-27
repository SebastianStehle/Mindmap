// ==========================================================================
// LinePath.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using Windows.UI;
using Hercules.Win2D.Rendering.Utils;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;

namespace Hercules.Win2D.Rendering.Parts.Paths
{
    public class LinePath : GeometryPathBase
    {
        public LinePath(Color strokeColor, float opacity = 1)
            : base(strokeColor, opacity)
        {
        }

        protected override IEnumerable<CanvasGeometry> CreateGeometries(Win2DRenderNode renderNode, ICanvasResourceCreator resourceCreator)
        {
            yield return GeometryBuilder.ComputeLinePath(renderNode, renderNode.Parent, resourceCreator);
        }

        protected override void RenderInternal(Win2DRenderable renderable, CanvasDrawingSession session, CanvasGeometry[] geometries, ICanvasBrush brush)
        {
            session.DrawGeometry(geometries[0], brush, 2);
        }
    }
}

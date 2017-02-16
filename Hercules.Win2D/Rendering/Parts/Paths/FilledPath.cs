// ==========================================================================
// FilledPath.cs
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
    public class FilledPath : GeometryPathBase
    {
        public FilledPath(Color strokeColor, float opacity = 1)
            : base(strokeColor, opacity)
        {
        }

        protected override IEnumerable<CanvasGeometry> CreateGeometries(Win2DRenderNode renderNode, ICanvasResourceCreator resourceCreator)
        {
            yield return GeometryBuilder.ComputeFilledPath(renderNode, renderNode.Parent, resourceCreator);
            yield return GeometryBuilder.ComputeLinePath(renderNode, renderNode.Parent, resourceCreator, true);
        }

        protected override void RenderInternal(CanvasDrawingSession session, CanvasGeometry[] geometries, ICanvasBrush brush)
        {
            session.FillGeometry(geometries[0], brush);
            session.DrawGeometry(geometries[1], brush, 2);
        }
    }
}

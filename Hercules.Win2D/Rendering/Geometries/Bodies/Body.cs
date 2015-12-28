// ==========================================================================
// Body.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Numerics;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;

namespace Hercules.Win2D.Rendering.Geometries.Bodies
{
    public abstract class Body : IBodyGeometry
    {
        protected static readonly Vector2 ImageSizeLarge = new Vector2(64, 64);
        protected static readonly Vector2 ImageSizeSmall = new Vector2(32, 32);
        protected static readonly float ImageMargin = 6;
        protected static readonly CanvasStrokeStyle SelectionStrokeStyle = new CanvasStrokeStyle { DashStyle = CanvasDashStyle.Dash };

        public abstract Vector2 TextRenderPosition { get; }

        public virtual Vector2 RenderPositionOffset
        {
            get { return Vector2.Zero; }
        }

        public virtual bool HasText
        {
            get { return true; }
        }

        public virtual float VerticalPathOffset
        {
            get { return 0; }
        }

        public virtual void ClearResources()
        {
        }

        public abstract void Arrange(Win2DRenderNode renderNode, CanvasDrawingSession session);

        public abstract void Render(Win2DRenderNode renderNode, CanvasDrawingSession session, Win2DColor color, bool renderSelection);

        public abstract Vector2 Measure(Win2DRenderNode renderNode, CanvasDrawingSession session, Vector2 textSize);
    }
}

// ==========================================================================
// SimpleRectangle.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Numerics;
using Windows.UI;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;

namespace Hercules.Win2D.Rendering.Geometries.Bodies
{
    public sealed class SimpleRectangle : Body
    {
        private readonly Vector2 size;

        public override Vector2 TextRenderPosition
        {
            get { return Vector2.Zero; }
        }

        public override bool HasText
        {
            get { return false; }
        }

        public SimpleRectangle(Vector2 size)
        {
            this.size = size;
        }

        public override void Arrange(Win2DRenderable renderable, CanvasDrawingSession session)
        {
        }

        public override Vector2 Measure(Win2DRenderable renderable, CanvasDrawingSession session, Vector2 textSize)
        {
            return size;
        }

        public override void Render(Win2DRenderable renderable, CanvasDrawingSession session, Win2DColor color, bool renderSelection)
        {
            ICanvasBrush brush = renderable.Resources.Brush(Colors.Black, 0.5f);

            session.FillRectangle(renderable.RenderBounds, brush);
        }

        public override IBodyGeometry Clone()
        {
            return new SimpleRectangle(size);
        }
    }
}

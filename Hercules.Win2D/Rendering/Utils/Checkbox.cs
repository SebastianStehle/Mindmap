// ==========================================================================
// CheckBox.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Numerics;
using Windows.UI;
using GP.Utils.Mathematics;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;

// ReSharper disable SuggestBaseTypeForParameter

namespace Hercules.Win2D.Rendering.Utils
{
    public sealed class CheckBox : IResourceHolder
    {
        private Vector2 renderSize;
        private Vector2 renderPosition;
        private Rect2 renderBounds;
        private static CanvasGeometry checkGeometry;

        public void ClearResources()
        {
            checkGeometry?.Dispose();
            checkGeometry = null;
        }

        public void Arrange(Vector2 center, float size)
        {
            renderSize = new Vector2(size);
            renderPosition = center - (0.5f * renderSize);

            renderBounds = new Rect2(renderPosition, renderSize);
        }

        public HitResult HitTest(Win2DRenderNode renderNode, Vector2 hitPosition)
        {
            return renderBounds.Contains(hitPosition) ? new HitResult(renderNode, HitTarget.CheckBox) : null;
        }

        public void Render(Win2DRenderable renderable, CanvasDrawingSession session)
        {
            using (session.StackTransform(Matrix3x2.CreateTranslation(renderPosition)))
            {
#if DRAW_OUTLINE
                session.DrawRectangle(0, 0, renderSize.X, renderSize.Y, Colors.Purple);
#endif
                session.FillRectangle(0, 0, renderSize.X, renderSize.Y, Colors.White);
                session.DrawRectangle(0, 0, renderSize.X, renderSize.Y, Colors.Black);

                if (renderable.Node.IsChecked)
                {
                    RenderCheck(session);
                }
            }
        }

        private void RenderCheck(CanvasDrawingSession session)
        {
            if (checkGeometry == null)
            {
                checkGeometry = CreateGeometry(session);
            }

            session.DrawGeometry(checkGeometry, Colors.Black, 2);
        }

        private CanvasGeometry CreateGeometry(ICanvasResourceCreator session)
        {
            var w = renderSize.X;
            var h = renderSize.Y;

            using (var builder = new CanvasPathBuilder(session.Device))
            {
                builder.BeginFigure(new Vector2(0.15f * w, 0.5f * h));

                builder.AddLine(0.40f * w, 0.75f * h);
                builder.AddLine(0.85f * w, 0.25f * h);

                builder.EndFigure(CanvasFigureLoop.Open);

                return CanvasGeometry.CreatePath(builder);
            }
        }
    }
}

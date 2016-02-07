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
using Hercules.Model;
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

        public bool HandleClick(Win2DRenderable renderable, Vector2 mousePosition)
        {
            bool isHit = renderBounds.Contains(mousePosition);

            if (isHit)
            {
                renderable.Node.ToggleCheckedTransactional();
            }

            return isHit;
        }

        public void Render(Win2DRenderable renderable, CanvasDrawingSession session)
        {
            using (session.Transform(Matrix3x2.CreateTranslation(renderPosition)))
            {
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
            float w = renderSize.X;
            float h = renderSize.Y;

            using (CanvasPathBuilder builder = new CanvasPathBuilder(session.Device))
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

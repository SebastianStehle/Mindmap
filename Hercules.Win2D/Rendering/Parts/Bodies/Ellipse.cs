// ==========================================================================
// Ellipse.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Numerics;
using Microsoft.Graphics.Canvas;

namespace Hercules.Win2D.Rendering.Parts.Bodies
{
    public sealed class Ellipse : BodyBase
    {
        private static readonly Vector2 SelectionMargin = new Vector2(-5, -5);

        protected override Vector2 CalculatePadding(Vector2 contentSize)
        {
            var sqrt2 = (float)Math.Sqrt(2);

            var a = (contentSize.X + 5) / sqrt2;
            var b = (contentSize.Y + 5) / sqrt2;

            return new Vector2(a - (contentSize.X * 0.5f), b - (contentSize.Y * 0.5f));
        }

        public override void Render(Win2DRenderable renderable, CanvasDrawingSession session, Win2DColor color, bool renderControls)
        {
            var borderBrush = renderable.Resources.ThemeDarkBrush(color);

            var backgroundBrush =
                renderable.Node.IsSelected ?
                    renderable.Resources.ThemeLightBrush(color) :
                    renderable.Resources.ThemeNormalBrush(color);

            var radiusX = 0.5f * renderable.RenderSize.X;
            var radiusY = 0.5f * renderable.RenderSize.Y;

            session.FillEllipse(
                renderable.RenderBounds.Center,
                radiusX,
                radiusY,
                backgroundBrush);

            session.DrawEllipse(
                renderable.RenderBounds.Center,
                radiusX,
                radiusY,
                borderBrush);

            RenderIcon(renderable, session);
            RenderText(renderable, session);

            RenderCheckBox(renderable, session);

            if (!renderControls)
            {
                return;
            }

            if (renderable.Node.IsSelected)
            {
                radiusX -= SelectionMargin.X;
                radiusY -= SelectionMargin.Y;

                session.DrawEllipse(
                    renderable.RenderBounds.Center,
                    radiusX,
                    radiusY,
                    borderBrush, 2f, SelectionStrokeStyle);
            }

            RenderExpandButton(renderable, session);
            RenderNotesButton(renderable, session);
        }

        public override IBodyPart Clone()
        {
            return new Ellipse();
        }
    }
}

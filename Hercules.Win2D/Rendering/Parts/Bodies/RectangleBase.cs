// ==========================================================================
// RectangleBase.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Numerics;
using GP.Utils.Mathematics;
using Microsoft.Graphics.Canvas;

namespace Hercules.Win2D.Rendering.Parts.Bodies
{
    public abstract class RectangleBase : BodyBase
    {
        private static readonly Vector2 SelectionMargin = new Vector2(-5, -5);
        private readonly float borderRadius;

        protected RectangleBase(float borderRadius)
        {
            this.borderRadius = borderRadius;
        }

        protected override Vector2 CalculatePadding(Vector2 contentSize)
        {
            return new Vector2(12, 4);
        }

        public override void Render(Win2DRenderable renderable, CanvasDrawingSession session, Win2DColor color, bool renderSelection)
        {
            var borderBrush = renderable.Resources.ThemeDarkBrush(color);

            var backgroundBrush =
                renderable.Node.IsSelected ?
                    renderable.Resources.ThemeLightBrush(color) :
                    renderable.Resources.ThemeNormalBrush(color);

            var bounds = renderable.RenderBounds.ToRect();

            if (borderRadius > 0)
            {
                session.FillRoundedRectangle(bounds, borderRadius, borderRadius, backgroundBrush);

                session.DrawRoundedRectangle(bounds, borderRadius, borderRadius, borderBrush);
            }
            else
            {
                session.FillRectangle(bounds, backgroundBrush);

                session.DrawRectangle(bounds, borderBrush);
            }

            RenderIcon(renderable, session);
            RenderText(renderable, session);

            RenderCheckBox(renderable, session);

            if (!renderSelection)
            {
                return;
            }

            if (renderable.Node.IsSelected)
            {
                var rect = Rect2.Deflate(renderable.RenderBounds, SelectionMargin).ToRect();

                if (borderRadius > 0)
                {
                    session.DrawRoundedRectangle(rect, borderRadius * 1.4f, borderRadius * 1.4f, borderBrush, 2f, SelectionStrokeStyle);
                }
                else
                {
                    session.DrawRectangle(rect, borderBrush, 2f, SelectionStrokeStyle);
                }
            }

            RenderExpandButton(renderable, session);
            RenderNotesButton(renderable, session);
        }
    }
}

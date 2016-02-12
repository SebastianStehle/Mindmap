﻿// ==========================================================================
// ExpandButton.cs
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

// ReSharper disable SuggestBaseTypeForParameter

namespace Hercules.Win2D.Rendering.Utils
{
    public sealed class ExpandButton
    {
        private float renderRadius = 20;
        private Rect2 renderBounds;
        private Vector2 renderCenter;

        public void Arrange(Vector2 center, float radius)
        {
            renderRadius = radius;
            renderCenter = center;

            renderBounds = Rect2.Inflate(new Rect2(center, Vector2.Zero), radius, radius);
        }

        public bool HandleClick(Win2DRenderable renderable, Vector2 mousePosition)
        {
            bool isHit = renderBounds.Contains(mousePosition);

            if (isHit)
            {
                renderable.Node.ToggleCollapseTransactional();
            }

            return isHit;
        }

        public void Render(Win2DRenderable renderable, CanvasDrawingSession session)
        {
            RenderCircle(session);

            float halfRadius = 0.5f * renderRadius;

            RenderHorizontal(session, halfRadius);

            if (renderable.Node.IsCollapsed)
            {
                RenderVertical(session, halfRadius);
            }
        }

        private void RenderCircle(CanvasDrawingSession session)
        {
            session.FillCircle(renderCenter, renderRadius, Colors.White);

            session.DrawCircle(renderCenter, renderRadius, Colors.DarkGray);
        }

        private void RenderVertical(CanvasDrawingSession session, float halfRadius)
        {
            Vector2 verticalTop = new Vector2(
                renderCenter.X,
                renderCenter.Y - halfRadius);
            Vector2 verticalBottom = new Vector2(
                renderCenter.X,
                renderCenter.Y + halfRadius);

            session.DrawLine(verticalTop, verticalBottom, Colors.DarkGray, 2f);
        }

        private void RenderHorizontal(CanvasDrawingSession session, float halfRadius)
        {
            Vector2 horizontalLeft = new Vector2(
                renderCenter.X - halfRadius,
                renderCenter.Y);

            Vector2 horizontalRight = new Vector2(
                renderCenter.X + halfRadius,
                renderCenter.Y);

            session.DrawLine(horizontalLeft, horizontalRight, Colors.DarkGray, 2f);
        }
    }
}

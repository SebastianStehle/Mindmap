// ==========================================================================
// PathRenderer.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Numerics;
using GP.Windows;
using Hercules.Model.Utils;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;

namespace Hercules.Model.Rendering.Win2D
{
    public static class PathRenderer
    {
        public static void RenderFilledPath(Win2DRenderNode target, Win2DRenderNode parent, CanvasDrawingSession session, ICanvasBrush brush)
        {
            Guard.NotNull(brush, nameof(brush));
            Guard.NotNull(target, nameof(target));
            Guard.NotNull(session, nameof(session));

            if (parent != null && target.IsVisible)
            {
                Rect2 targetRect = target.Bounds;
                Rect2 parentRect = parent.Bounds;

                float targetOffset = target.VerticalPathOffset;

                Vector2 point1 = Vector2.Zero;
                Vector2 point2 = Vector2.Zero;

                CalculateCenter(parentRect, ref point1);

                if (targetRect.CenterX > parentRect.CenterX)
                {
                    CalculateCenterL(targetRect, ref point2, targetOffset);
                }
                else
                {
                    CalculateCenterR(targetRect, ref point2, targetOffset);
                }

                point2.X = (float)Math.Round(point2.X);
                point2.Y = (float)Math.Round(point2.Y);
                point1.X = (float)Math.Round(point1.X);
                point1.Y = (float)Math.Round(point1.Y);

                RenderFilledPath(session, brush, point1, point2);
            }
        }

        private static void RenderFilledPath(CanvasDrawingSession session, ICanvasBrush brush, Vector2 point1, Vector2 point2)
        {
            float halfX = (point1.X + point2.X) * 0.5f;

            using (CanvasPathBuilder builder = new CanvasPathBuilder(session.Device))
            {
                builder.BeginFigure(new Vector2(point1.X, point1.Y - 20));

                builder.AddCubicBezier(
                    new Vector2(halfX, point1.Y - 1),
                    new Vector2(halfX, point2.Y - 1),
                    new Vector2(point2.X, point2.Y));
                builder.AddCubicBezier(
                    new Vector2(halfX, point2.Y + 1),
                    new Vector2(halfX, point1.Y + 1),
                    new Vector2(point1.X, point1.Y + 20));

                builder.EndFigure(CanvasFigureLoop.Closed);

                using (CanvasGeometry pathGeometry = CanvasGeometry.CreatePath(builder))
                {
                    session.DrawGeometry(pathGeometry, brush, 2);
                    session.FillGeometry(pathGeometry, brush);
                }
            }
        }

        public static void RenderLinePath(Win2DRenderNode target, Win2DRenderNode parent, CanvasDrawingSession session, ICanvasBrush brush)
        {
            Guard.NotNull(brush, nameof(brush));
            Guard.NotNull(target, nameof(target));
            Guard.NotNull(session, nameof(session));

            if (parent != null && target.IsVisible)
            {
                Rect2 targetRect = target.Bounds;
                Rect2 parentRect = parent.Bounds;

                float targetOffset = target.VerticalPathOffset;
                float parentOffset = parent.VerticalPathOffset;

                Vector2 point1 = Vector2.Zero;
                Vector2 point2 = Vector2.Zero;

                if (targetRect.CenterX > parentRect.CenterX)
                {
                    CalculateCenterR(parentRect, ref point1, parentOffset);
                    CalculateCenterL(targetRect, ref point2, targetOffset);
                }
                else
                {
                    CalculateCenterL(parentRect, ref point1, parentOffset);
                    CalculateCenterR(targetRect, ref point2, targetOffset);
                }

                point2.X = (float)Math.Round(point2.X);
                point2.Y = (float)Math.Round(point2.Y);
                point1.X = (float)Math.Round(point1.X);
                point1.Y = (float)Math.Round(point1.Y);

                RenderLinePath(session, brush, point1, point2);
            }
        }

        private static void RenderLinePath(CanvasDrawingSession session, ICanvasBrush brush, Vector2 point1, Vector2 point2)
        {
            float halfX = (point1.X + point2.X) * 0.5f;

            using (CanvasPathBuilder builder = new CanvasPathBuilder(session.Device))
            {
                builder.BeginFigure(point1);

                builder.AddCubicBezier(
                    new Vector2(halfX, point1.Y),
                    new Vector2(halfX, point2.Y),
                    point2);

                builder.EndFigure(CanvasFigureLoop.Open);

                using (CanvasGeometry pathGeometry = CanvasGeometry.CreatePath(builder))
                {
                    session.DrawGeometry(pathGeometry, brush, 2);
                }
            }
        }

        private static void CalculateCenterL(Rect2 rect, ref Vector2 point, float offset)
        {
            point.X = rect.X;
            point.Y = rect.Y + (rect.Height * 0.5f) + offset;
        }

        private static void CalculateCenterR(Rect2 rect, ref Vector2 point, float offset)
        {
            point.X = rect.X + (rect.Width);
            point.Y = rect.Y + (rect.Height * 0.5f) + offset;
        }

        private static void CalculateCenter(Rect2 rect, ref Vector2 point)
        {
            point.X = rect.X + (rect.Width * 0.5f);
            point.Y = rect.Y + (rect.Height * 0.5f);
        }
    }
}

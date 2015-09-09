// ==========================================================================
// HullRenderer.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using GP.Windows;
using Hercules.Model.Utils;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;

namespace Hercules.Model.Rendering.Win2D
{
    public sealed class GeometryBuilder
    {
        private const float Radius = 10;
        private const float Padding = 10;

        public static CanvasGeometry ComputeHullGeometry(CanvasDrawingSession session, Win2DRenderer renderer, Win2DRenderNode renderNode)
        {
            Guard.NotNull(session, nameof(session));
            Guard.NotNull(renderer, nameof(renderer));
            Guard.NotNull(renderNode, nameof(renderNode));

            NodeBase node = renderNode.Node;

            if (node.HasChildren && !node.IsCollapsed)
            {
                IEnumerable<Rect2> childBounds =
                   node.AllChildren()
                       .Select(x => (Win2DRenderNode)renderer.FindRenderNode(x)).Union(new Win2DRenderNode[] { renderNode }).Where(x => x.IsVisible)
                       .Select(x => Rect2.Inflate(new Rect2(x.TargetPosition, x.RenderSize), new Vector2(Padding, Padding)));

                if (childBounds.Any())
                {
                    ConvexHull hull = ConvexHull.Compute(childBounds);

                    List<Vector2> points = RoundCorners(hull);

                    return BuildGeometry(session, points);
                }
            }

            return null;
        }

        private static List<Vector2> RoundCorners(ConvexHull hull)
        {
            int size = hull.Points.Count;

            List<Vector2> points = new List<Vector2>(size * 3);

            int e = size - 1;

            for (int i = 0; i < size; i++)
            {
                Vector2 c = hull.Points[i], next, prev, back, forw;

                prev = i > 0 ? hull.Points[i - 1] : hull.Points[e];
                next = i < e ? hull.Points[i + 1] : hull.Points[0];

                back = prev - c;
                forw = next - c;

                float lengthNext = Math.Min(Radius, back.Length() * 0.5f);
                float lengthForw = Math.Min(Radius, forw.Length() * 0.5f);

                points.Add(c + (Vector2.Normalize(back) * lengthNext));
                points.Add(c);
                points.Add(c + (Vector2.Normalize(forw) * lengthForw));
            }

            return points;
        }

        private static CanvasGeometry BuildGeometry(CanvasDrawingSession session, List<Vector2> points)
        {
            using (CanvasPathBuilder builder = new CanvasPathBuilder(session.Device))
            {
                builder.BeginFigure(points[0]);

                for (int i = 0; i < points.Count / 3; i++)
                {
                    builder.AddQuadraticBezier(points[(i * 3) + 1], points[(i * 3) + 2]);

                    int lastIndex = (i * 3) + 3;

                    if (lastIndex < points.Count - 1)
                    {
                        builder.AddLine(points[lastIndex]);
                    }
                }

                builder.EndFigure(CanvasFigureLoop.Closed);

                return CanvasGeometry.CreatePath(builder);
            }
        }
        public static CanvasGeometry ComputeFilledPath(Win2DRenderNode target, Win2DRenderNode parent, CanvasDrawingSession session)
        {
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

                return CreateFilledPath(session, point1, point2);
            }

            return null;
        }

        private static CanvasGeometry CreateFilledPath(CanvasDrawingSession session, Vector2 point1, Vector2 point2)
        {
            float halfX = (point1.X + point2.X) * 0.5f;

            using (CanvasPathBuilder builder = new CanvasPathBuilder(session.Device))
            {
                builder.BeginFigure(new Vector2(point1.X, point1.Y - 20));

                builder.AddCubicBezier(
                    new Vector2(halfX, point1.Y - 2),
                    new Vector2(halfX, point2.Y - 2),
                    new Vector2(point2.X, point2.Y - 1));
                builder.AddLine(point2.X, point2.Y + 1);
                builder.AddCubicBezier(
                    new Vector2(halfX, point2.Y + 2),
                    new Vector2(halfX, point1.Y + 2),
                    new Vector2(point1.X, point1.Y + 20));

                builder.EndFigure(CanvasFigureLoop.Closed);

                return CanvasGeometry.CreatePath(builder);
            }
        }

        public static CanvasGeometry ComputeLinePath(Win2DRenderNode target, Win2DRenderNode parent, CanvasDrawingSession session)
        {
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

                return CreateLinePath(session, point1, point2);
            }

            return null;
        }

        private static CanvasGeometry CreateLinePath(CanvasDrawingSession session, Vector2 point1, Vector2 point2)
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

                return CanvasGeometry.CreatePath(builder);
            }
        }

        private static void CalculateCenterL(Rect2 rect, ref Vector2 point, float offset)
        {
            point.X = rect.X;
            point.Y = rect.Y + (rect.Height * 0.5f) + offset;
        }

        private static void CalculateCenterR(Rect2 rect, ref Vector2 point, float offset)
        {
            point.X = rect.X + rect.Width;
            point.Y = rect.Y + (rect.Height * 0.5f) + offset;
        }

        private static void CalculateCenter(Rect2 rect, ref Vector2 point)
        {
            point.X = rect.X + (rect.Width * 0.5f);
            point.Y = rect.Y + (rect.Height * 0.5f);
        }
    }
}

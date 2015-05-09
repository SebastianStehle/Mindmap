// ==========================================================================
// DefaultPath.cs
// Metro Library SE
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using SE.Metro.UI;
using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace RavenMind.Controls
{
    public sealed class DefaultPath : IPathHolder
    {
        private readonly BezierSegment bezierSegment = new BezierSegment();
        private readonly PathFigure pathFigure = new PathFigure();
        private readonly Path path = new Path();

        public Path Path
        {
            get
            {
                return path;
            }
        }

        public DefaultPath(Style style)
        {
            pathFigure.Segments.Add(bezierSegment);

            path = new Path 
            {
                Data = new PathGeometry { Figures = { pathFigure } }, Style = style
            };
        }

        public void Render(NodeContainer container)
        {
            Rect targetRect = container.Bounds;
            Rect parentRect = container.Parent.Bounds;

            Point targetAnchorPosition = container.NodeControl.AnchorPosition;
            Point parentAnchorPosition = container.Parent.NodeControl.AnchorPosition;

            Point point1 = VisualTreeExtensions.PointZero;
            Point point2 = VisualTreeExtensions.PointZero;

            if (CalculateCenterX(targetRect) > CalculateCenterX(parentRect))
            {
                CalculateCenterL(targetRect, targetAnchorPosition, ref point1);
                CalculateCenterR(parentRect, parentAnchorPosition, ref point2);
            }
            else
            {
                CalculateCenterR(targetRect, targetAnchorPosition, ref point1);
                CalculateCenterL(parentRect, parentAnchorPosition, ref point2);
            }

            point1.X = Math.Round(point1.X);
            point1.Y = Math.Round(point1.Y);
            point2.X = Math.Round(point2.X);
            point2.Y = Math.Round(point2.Y);

            double halfX = (point1.X + point2.X) * 0.5;

            pathFigure.StartPoint = point1;

            bezierSegment.Point1 = new Point(halfX, point1.Y);
            bezierSegment.Point2 = new Point(halfX, point2.Y);
            bezierSegment.Point3 = point2;
        }

        private static void CalculateCenterL(Rect rect, Point anchorPosition, ref Point point)
        {
            point.X = rect.X + anchorPosition.X;
            point.Y = rect.Y + anchorPosition.Y + (rect.Height * 0.5);
        }

        private static void CalculateCenterR(Rect rect, Point anchorPosition, ref Point point)
        {
            point.X = rect.X - anchorPosition.X + rect.Width;
            point.Y = rect.Y + anchorPosition.Y + (rect.Height * 0.5);
        }

        private static double CalculateCenterX(Rect rect)
        {
            return rect.X + (rect.Width * 0.5);
        }
    }
}

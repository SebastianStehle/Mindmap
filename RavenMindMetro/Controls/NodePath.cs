// ==========================================================================
// NodePath.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using SE.Metro.UI;
using SE.Metro.UI.Controls;
using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace RavenMind.Controls
{
    [TemplatePart(Name = PathPart, Type = typeof(Path))]
    public sealed class NodePath : LoadableControl
    {
        #region Constants

        private const string PathPart = "Path";

        #endregion

        #region Fields

        private NodeControl parentControl;
        private NodeControl targetControl;
        private readonly BezierSegment bezierSegment = new BezierSegment();
        private readonly PathFigure pathFigure;

        #endregion

        #region Constructors

        internal NodePath()
        {
            DefaultStyleKey = typeof(NodePath);

            pathFigure = new PathFigure { Segments = { bezierSegment } };
        }

        #endregion

        #region Methods

        protected override void OnApplyTemplate()
        {
            Path path = (Path)GetTemplateChild("Path");

            path.Data = new PathGeometry { Figures = { pathFigure } };
        }

        protected override void OnLoaded()
        {
            DrawLine();
        }

        public void RebindParent(NodeControl parent)
        {
            parentControl.PositionChanged -= new EventHandler(nodeControl_PositionChanged);
            parentControl.SizeChanged -= new SizeChangedEventHandler(nodeControl_SizeChanged);

            parentControl = parent;

            parentControl.PositionChanged += new EventHandler(nodeControl_PositionChanged);
            parentControl.SizeChanged += new SizeChangedEventHandler(nodeControl_SizeChanged);
        }

        public void BindEvents(NodeControl parent, NodeControl target)
        {
            parentControl = parent;
            parentControl.PositionChanged += new EventHandler(nodeControl_PositionChanged);
            parentControl.SizeChanged += new SizeChangedEventHandler(nodeControl_SizeChanged);

            targetControl = target;
            targetControl.PositionChanged += new EventHandler(nodeControl_PositionChanged);
            targetControl.SizeChanged += new SizeChangedEventHandler(nodeControl_SizeChanged);
        }

        public void UnbindEvents()
        {
            parentControl.PositionChanged -= new EventHandler(nodeControl_PositionChanged);
            parentControl.SizeChanged -= new SizeChangedEventHandler(nodeControl_SizeChanged);
            parentControl = null;

            targetControl.PositionChanged -= new EventHandler(nodeControl_PositionChanged);
            targetControl.SizeChanged -= new SizeChangedEventHandler(nodeControl_SizeChanged);
            targetControl = null;
        }

        private void nodeControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawLine();
        }

        private void nodeControl_PositionChanged(object sender, EventArgs e)
        {
            DrawLine();
        }

        public void DrawLine()
        {
            Point point1 = VisualTreeExtensions.PointZero;
            Point point2 = VisualTreeExtensions.PointZero;

            if (targetControl.CalculateCenterX() > parentControl.CalculateCenterX())
            {
                targetControl.CalculateCenterLeft(ref point1);
                parentControl.CalculateCenterRight(ref point2);
            }
            else
            {
                targetControl.CalculateCenterRight(ref point1);
                parentControl.CalculateCenterLeft(ref point2);
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

        #endregion
    }
}

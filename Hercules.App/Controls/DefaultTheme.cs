// ==========================================================================
// DefaultRenderer.cs
// Metro Library SE
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Hercules.Model;
using System;
using Windows.UI.Xaml;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Windows.Foundation;
using GP.Windows.UI;
using System.Numerics;
using Microsoft.Graphics.Canvas.Brushes;
using Windows.UI;

namespace Hercules.App.Controls
{
    public sealed class DefaultTheme : ThemeBase
    {
        private readonly ResourceDictionary resources = new ResourceDictionary();
        private Style pathStyle;
        private Style pathPreviewStyle;
        private Style nodePreviewStyle;
        private Style nodeLevel0Style;
        private Style nodeLevel1Style;
        private Style nodeLevel2Style;

        private Style PathStyle
        {
            get
            {
                return pathStyle ?? (pathStyle = (Style)resources["PathStyle"]);
            }
        }

        private Style PathPreviewStyle
        {
            get
            {
                return pathPreviewStyle ?? (pathPreviewStyle = (Style)resources["PathPreviewStyle"]);
            }
        }

        private Style NodePreviewStyle
        {
            get
            {
                return nodePreviewStyle ?? (nodePreviewStyle = (Style)resources["NodePreviewStyle"]);
            }
        }

        private Style NodeLevel0Style
        {
            get
            {
                return nodeLevel0Style ?? (nodeLevel0Style = (Style)resources["NodeLevel0Style"]);
            }
        }

        private Style NodeLevel1Style
        {
            get
            {
                return nodeLevel1Style ?? (nodeLevel1Style = (Style)resources["NodeLevel1Style"]);
            }
        }

        private Style NodeLevel2Style
        {
            get
            {
                return nodeLevel2Style ?? (nodeLevel2Style = (Style)resources["NodeLevel2Style"]);
            }
        }

        public DefaultTheme()
        {
            Application.LoadComponent(resources, new Uri("ms-appx:///Themes/Theme.Default.xaml"));

            AddColors(
                0xF7977A,
                0xF9AD81,
                0xFDC68A,
                0xFFF79A,
                0xC4DF9B,
                0xF26C4F,
                0xF68E55,
                0xFBAF5C,
                0xFFF467,
                0xACD372,
                0xA2D39C,
                0x82CA9D,
                0x7BCDC8,
                0x6ECFF6,
                0x7EA7D8,
                0x7CC576,
                0x3BB878,
                0x1ABBB4,
                0x00BFF3,
                0x438CCA,
                0x8493CA,
                0x8882BE,
                0xBC8DBF,
                0xF49AC2,
                0xF6989D,
                0x605CA8,
                0x855FA8,
                0xA763A8,
                0xF06EA9,
                0xF26D7D);
        }

        public override void UpdateStyle(NodeContainer renderContainer, bool isPreview)
        {
            NodeControl nodeControl = renderContainer.NodeControl;
            
            if (isPreview)
            {
                UpdateStyle(nodeControl, (Style)resources["NodePreviewStyle"]);
            }
            else
            {
                if (nodeControl.AssociatedNode is RootNode)
                {
                    UpdateStyle(nodeControl, NodeLevel0Style);
                }
                else if (nodeControl.AssociatedNode.Parent is RootNode)
                {
                    UpdateStyle(nodeControl, NodeLevel1Style);
                }
                else
                {
                    UpdateStyle(nodeControl, NodeLevel2Style);
                }

                nodeControl.ThemeColor = Colors[nodeControl.AssociatedNode.Color];
            }
        }

        private static void UpdateStyle(NodeControl nodeControl, Style style)
        {
            if (!object.ReferenceEquals(nodeControl.Style, style))
            {
                nodeControl.Style = style;
            }
        }

        public override IPathHolder CreatePreviewPath()
        {
            return new DefaultThemePath(PathPreviewStyle);
        }

        public override IPathHolder CreatePath()
        {
            return new DefaultThemePath(PathStyle);
        }

        public override void RenderNode(NodeContainer node, CanvasDrawingSession session)
        {
            ThemeColor color = Colors[node.NodeControl.AssociatedNode.Color];

            ICanvasBrush brush = node.NodeControl.AssociatedNode.IsSelected ?
                new CanvasSolidColorBrush(session.Device, color.Light.Color) :
                new CanvasSolidColorBrush(session.Device, color.Normal.Color);
            ICanvasBrush darkBrush = new CanvasSolidColorBrush(session.Device, color.Dark.Color);

            if (node.NodeControl.AssociatedNode is RootNode)
            {
                session.FillEllipse(
                    new Vector2(
                        (float)node.Bounds.CenterX(), 
                        (float)node.Bounds.CenterY()),
                    0.5f * (float)node.Size.Width,
                    0.5f * (float)node.Size.Height,
                    brush);
            }
            else if (node.NodeControl.AssociatedNode.Parent is RootNode)
            {
                Vector2 padding = new Vector2(10, 5);

                session.FillRectangle(
                    (float)node.Position.X + padding.X,
                    (float)node.Bounds.Y + padding.Y,
                    (float)node.Size.Width - padding.X * 2,
                    (float)node.Size.Height - padding.Y * 2,
                    brush);

                if (node.NodeControl.AssociatedNode.IsSelected)
                {
                    session.DrawRectangle(
                        (float)node.Position.X,
                        (float)node.Bounds.Y,
                        (float)node.Size.Width,
                        (float)node.Size.Height,
                        darkBrush);
                }
            }
        }

        public override void RenderPath(IPathHolder path, NodeContainer container, CanvasDrawingSession session)
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

            CanvasPathBuilder builder = new CanvasPathBuilder(session.Device);

            builder.BeginFigure(new Vector2((float)point1.X, (float)point1.Y));

            builder.AddCubicBezier(
                new Vector2((float)halfX, (float)point1.Y),
                new Vector2((float)halfX, (float)point2.Y),
                new Vector2((float)point2.X, (float)point2.Y));

            builder.EndFigure(CanvasFigureLoop.Open);

            CanvasGeometry pathGeometry = CanvasGeometry.CreatePath(builder);

            ICanvasBrush brush = new CanvasSolidColorBrush(session.Device, Color.FromArgb(255, 0, 0, 0));

            session.DrawGeometry(pathGeometry, brush, 2);

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

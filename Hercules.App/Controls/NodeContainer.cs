// ==========================================================================
// NodeContainer.cs
// Metro Library SE
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Hercules.Model.Layouting;
using GP.Windows.UI;
using System;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace Hercules.App.Controls
{
    public sealed class NodeContainer : IRenderNode
    {
        private static readonly Point EmptyPoint = new Point(double.PositiveInfinity, double.PositiveInfinity);

        private readonly NodeControl node;
        private Point layoutPosition;
        private Point renderPosition;
        private AnchorPoint anchorPoint;

        public IPathHolder PathHolder { get; set; }

        public Size Size
        {
            get
            {
                return node.DesiredSize;
            }
        }

        public Point Position
        {
            get 
            {
                return layoutPosition;
            }
        }

        public Rect Bounds
        {
            get
            {
                return new Rect(renderPosition, node.DesiredSize);
            }
        }

        public NodeControl NodeControl
        {
            get
            {
                return node;
            }
        }

        public bool IsVisible
        {
            get
            {
                return node.Visibility == Visibility.Visible;
            }
        }

        public NodeContainer Parent { get; set; }

        public NodeContainer(NodeControl node)
        {
            this.node = node;
        }

        public void Hide()
        {
            if (IsVisible)
            {
                ChangeVisibility(Visibility.Collapsed);
            }
        }

        public void Show()
        {
            if (!IsVisible)
            {
                ChangeVisibility(Visibility.Visible);
            }
        }

        private void ChangeVisibility(Visibility visible)
        {
            NodeControl.Visibility = visible;

            if (PathHolder != null && PathHolder.Path != null)
            {
                PathHolder.Path.Visibility = visible;
            }
        }

        public void MoveTo(Point position, AnchorPoint anchor)
        {
            anchorPoint = anchor;

            layoutPosition = position;
        }

        public void UpdateRenderPosition()
        {
            renderPosition = layoutPosition;

            Size size = NodeControl.DesiredSize;

            renderPosition.Y -= 0.5 * size.Height;

            if (anchorPoint == AnchorPoint.Right)
            {
                renderPosition.X -= size.Width;
            }
            else if (anchorPoint == AnchorPoint.Center)
            {
                renderPosition.X -= 0.5 * size.Width;
            }
            
            node.Arrange(new Rect(renderPosition, node.DesiredSize));
        }
    }
}

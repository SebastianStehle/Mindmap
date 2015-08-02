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
        private Point startingRenderingPosition = EmptyPoint;
        private Point currentPosition;
        private Point targetPosition;
        private Point layoutPosition;
        private AnchorPoint anchorPoint;
        private DateTime? animatingEnd;

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
                return new Rect(currentPosition, node.DesiredSize);
            }
        }

        public NodeControl NodeControl
        {
            get
            {
                return node;
            }
        }

        public IPathHolder PathHolder { get; set; }

        public NodeContainer Parent { get; set; }

        public NodeContainer(NodeControl node)
        {
            this.node = node;
        }

        public void Hide()
        {
            ChangeVisibility(Visibility.Collapsed);

            startingRenderingPosition = EmptyPoint;
        }

        public void Show()
        {
            ChangeVisibility(Visibility.Visible);
        }

        public void MoveTo(Point position, AnchorPoint anchor)
        {
            anchorPoint = anchor;

            layoutPosition = position;
        }

        private void ChangeVisibility(Visibility visible)
        {
            NodeControl.Visibility = visible;

            if (PathHolder != null && PathHolder.Path != null)
            {
                PathHolder.Path.Visibility = visible;
            }
        }

        public bool UpdateRenderPosition(bool isAnimating, DateTime now, double animationSpeed)
        {
            Point renderPosition = layoutPosition;

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

            targetPosition = renderPosition;

            if (isAnimating && startingRenderingPosition != EmptyPoint)
            {
                if (startingRenderingPosition != renderPosition)
                {
                    animatingEnd = now.AddMilliseconds(animationSpeed);

                    startingRenderingPosition = renderPosition;
                }
            }
            else
            {
                animatingEnd = null;
            }

            startingRenderingPosition = renderPosition;

            double fractionComplete = 1;

            if (animatingEnd.HasValue)
            {
                double timeRemaining = (animatingEnd.Value - now).TotalMilliseconds;

                fractionComplete -= Math.Min(1, Math.Max(0, timeRemaining / animationSpeed));
            }

            currentPosition = new Point(
                MathHelper.Interpolate(fractionComplete, currentPosition.X, targetPosition.X),
                MathHelper.Interpolate(fractionComplete, currentPosition.Y, targetPosition.Y));

            node.Arrange(new Rect(currentPosition, node.DesiredSize));

            return !MathHelper.AboutEqual(targetPosition, currentPosition);
        }
    }
}

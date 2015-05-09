using RavenMind.Model;
using SE.Metro.UI;
using System;
using Windows.Foundation;

namespace RavenMind.Controls
{
    public sealed class NodeContainer : INodeView
    {
        private static readonly Point EmptyPoint = new Point(double.PositiveInfinity, double.PositiveInfinity);

        private readonly NodeControl node;
        private Point startingRenderingPosition = EmptyPoint;
        private DateTime? animatingEnd;

        public Size Size
        {
            get
            {
                return node.DesiredSize;
            }
        }

        public Rect Rect
        {
            get
            {
                return new Rect(CurrentPosition, node.DesiredSize);
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

        public Point CurrentPosition { get; set; }

        public Point TargetPosition { get; set; }

        public NodeContainer Parent { get; set; }

        public NodeContainer(NodeControl node)
        {
            this.node = node;
        }

        public void SetPosition(Point position, AnchorPoint anchor)
        {
            node.Anchor = anchor;

            node.Position = position;
        }

        public bool UpdateRenderPosition(bool isAnimating, DateTime now, double animationSpeed)
        {
            Point renderPosition = node.Position;

            Size size = NodeControl.DesiredSize;

            renderPosition.Y -= 0.5 * size.Height;

            if (node.Anchor == AnchorPoint.Right)
            {
                renderPosition.X -= size.Width;
            }
            else if (node.Anchor == AnchorPoint.Center)
            {
                renderPosition.X -= 0.5 * size.Width;
            }

            TargetPosition = renderPosition;

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

            CurrentPosition = new Point(
                MathHelper.Interpolate(fractionComplete, CurrentPosition.X, TargetPosition.X),
                MathHelper.Interpolate(fractionComplete, CurrentPosition.Y, TargetPosition.Y));

            node.Arrange(new Rect(CurrentPosition, node.DesiredSize));

            return !MathHelper.AboutEqual(TargetPosition, CurrentPosition);
        }
    }
}

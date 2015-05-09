// ==========================================================================
// NodeMovingBehavior.cs
// Metro Library SE
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using RavenMind.Model;
using RavenMind.Model.Layouting;
using SE.Metro;
using SE.Metro.UI;
using SE.Metro.UI.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace RavenMind.Controls
{
    public sealed class NodeMovingBehavior : Behavior<Mindmap>
    {
        private Image clone;
        private Point initialPosition;
        private Node movingNode;
        private NodeControl nodeControl;
        private TranslateTransform transform;

        protected override void OnAttached()
        {
            AssociatedElement.ManipulationStarted += AssociatedElement_ManipulationStarted;
            AssociatedElement.ManipulationDelta += AssociatedElement_ManipulationDelta;
            AssociatedElement.ManipulationCompleted += AssociatedElement_ManipulationCompleted;
        }

        protected override void OnDetaching()
        {
            AssociatedElement.ManipulationStarted -= AssociatedElement_ManipulationStarted;
            AssociatedElement.ManipulationDelta -= AssociatedElement_ManipulationDelta;
            AssociatedElement.ManipulationCompleted -= AssociatedElement_ManipulationCompleted;
        }

        private void AssociatedElement_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            nodeControl = e.OriginalSource as NodeControl;

            if (nodeControl != null)
            {
                movingNode = nodeControl.AssociatedNode as Node;

                if (movingNode != null && movingNode.IsSelected && !nodeControl.IsTextEditing)
                {
                    Rect bounds = AssociatedElement.GetBounds(nodeControl.AssociatedNode);

                    initialPosition = bounds.Position();

                    transform = new TranslateTransform
                    {
                        X = initialPosition.X,
                        Y = initialPosition.Y
                    };

                    RenderAndSet(nodeControl);
                }
            }
        }

        private void RenderAndSet(NodeControl control)
        {
            nodeControl = control;

            RenderTargetBitmap bitmap = new RenderTargetBitmap();

            bitmap.RenderAsync(nodeControl).AsTask().ContinueWith(async t =>
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    if (control == nodeControl)
                    {
                        clone = new Image { Width = bitmap.PixelWidth, Height = bitmap.PixelHeight, Source = bitmap, RenderTransform = transform, Stretch = Stretch.Fill };

                        AssociatedElement.AddAdorner(clone);
                    }
                });
            });
        }

        private void AssociatedElement_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (clone != null)
            {
                double dx = e.Delta.Translation.X / AssociatedElement.ScrollViewer.ZoomFactor;
                double dy = e.Delta.Translation.Y / AssociatedElement.ScrollViewer.ZoomFactor;

                transform.X += dx;
                transform.Y += dy;

                AttachTarget target = FindAttachTarget();

                if (target != null)
                {
                    AssociatedElement.ShowPreviewElement(target.Position, target.Parent, target.Anchor);
                }
                else
                {
                    AssociatedElement.ShowPreviewElement(null, null, AnchorPoint.Center);
                }
            }
        }

        private void AssociatedElement_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (clone != null)
            {
                try
                {
                    if (MathHelper.LengthSquared(transform.Position(), initialPosition) > 100)
                    {
                        AttachTarget target = FindAttachTarget();

                        if (target != null)
                        {
                            AssociatedElement.Document.MakeTransaction("MoveNode", d =>
                            {
                                d.Apply(new RemoveChildCommand(movingNode.Parent, movingNode));

                                d.Apply(new InsertChildCommand(target.Parent, target.Index, target.NodeSide, movingNode));
                            });
                        }
                    }
                }
                finally
                {
                    AssociatedElement.ShowPreviewElement(null, null, AnchorPoint.Center);
                    AssociatedElement.RemoveAdorner(clone);

                    clone = null;

                    movingNode = null;
                    nodeControl = null;
                }
            }
        }

        private AttachTarget FindAttachTarget()
        {
            NodeControl other = FindOtherNodeControl();

            AttachTarget target = AssociatedElement.CalculateAttachTarget(other != null ? other.AssociatedNode : null, movingNode, new Rect(transform.Position(), new Size(clone.Width, clone.Height)));

            return target;
        }

        private NodeControl FindOtherNodeControl()
        {
            Rect rect = new Rect(clone.TransformToVisual(AssociatedElement).TransformPoint(new Point(0, 0)), nodeControl.RenderSize);

            IEnumerable<NodeControl> otherNodes = VisualTreeHelper.FindElementsInHostCoordinates(rect, AssociatedElement, true).OfType<NodeControl>().Where(x => x != nodeControl).ToList();

            if (otherNodes.Any())
            {
                double rectArea = rect.Width * rect.Height;

                foreach (NodeControl otherNodeControl in otherNodes)
                {
                    Rect otherRect = new Rect(otherNodeControl.TransformToVisual(AssociatedElement).TransformPoint(new Point(0, 0)), otherNodeControl.RenderSize);

                    double minArea = Math.Min(rectArea, otherRect.Width * otherRect.Height);

                    otherRect.Intersect(rect);

                    double newArea = otherRect.Width * otherRect.Height;

                    if (!double.IsInfinity(newArea) && newArea > 0.5 * minArea)
                    {
                        NodeBase otherNode = otherNodeControl.AssociatedNode;

                        if (otherNode != movingNode && otherNode != movingNode.Parent && !movingNode.HasChild(otherNode as Node))
                        {
                            return otherNodeControl;
                        }
                    }
                }
            }

            return null;
        }
    }
}

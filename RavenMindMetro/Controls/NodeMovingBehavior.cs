using RavenMind.Model;
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

                NodeControl other = FindOtherNodeControl();

                if (other != null)
                {
                    RootNode root = other.AssociatedNode as RootNode;

                    if (root != null)
                    {
                        if (movingNode.NodeSide == NodeSide.Right)
                        {
                            ShowPreviewElement(other, root.RightChildren, int.MaxValue, NodeSide.Right);
                        }
                        else
                        {
                            ShowPreviewElement(other, root.LeftChildren, int.MaxValue, NodeSide.Left);
                        }
                    }
                    else
                    {
                        ShowPreviewElement(other, ((Node)other.AssociatedNode).Children, int.MaxValue, other.AssociatedNode.NodeSide);
                    }
                }
                else
                {
                    AssociatedElement.ShowPreviewElement(null, null, AnchorPoint.Center);
                }
            }
        }

        private void ShowPreviewElement(NodeControl other)
        {
            double margin = AssociatedElement.Layout.HorizontalMargin;

            double y = other.Position.Y;
            double x = other.Position.X + (1.0 * other.ActualWidth) + margin;

            AnchorPoint anchor = AnchorPoint.Left;

            Action<IReadOnlyList<Node>> ajustWithChildren = children =>
            {
                if (children.Count > 0)
                {
                    Rect bounds = AssociatedElement.GetBounds(children.Last());

                    y = bounds.Y + 60 + (bounds.Height * 0.5);
                }
            };

            Node node = other.AssociatedNode as Node;

            if (node != null)
            {
                if (node.NodeSide == NodeSide.Right)
                {
                    y = other.Position.Y;
                    x = other.Position.X + other.ActualWidth + margin;
                }
                else
                {
                    y = other.Position.Y;
                    x = other.Position.X - other.ActualWidth - margin;

                    anchor = AnchorPoint.Right;
                }

                ajustWithChildren(node.Children);
            }
            else
            {
                RootNode root = (RootNode)other.AssociatedNode;

                Rect rootBounds = AssociatedElement.GetBounds(root);

                if (root.LeftChildren.Count > root.RightChildren.Count)
                {
                    y = rootBounds.Y + (rootBounds.Height * 0.5);
                    x = rootBounds.X + rootBounds.Width + margin;

                    ajustWithChildren(root.RightChildren);
                }
                else
                {
                    y = rootBounds.Y + (rootBounds.Height * 0.5);
                    x = rootBounds.X - margin;

                    anchor = AnchorPoint.Right;

                    ajustWithChildren(root.LeftChildren);
                }
            }

            AssociatedElement.ShowPreviewElement(new Point(x, y), other.AssociatedNode, anchor);
        }

        private void ShowPreviewElement(NodeControl other, IReadOnlyList<Node> children, int index, NodeSide side)
        {
            double margin = AssociatedElement.Layout.HorizontalMargin;

            double y = other.Position.Y;
            double x = other.Position.X + (1.0 * other.ActualWidth) + margin;

            AnchorPoint anchor = AnchorPoint.Left;

            Action ajustWithChildren = () =>
            {
                if (children.Count > 0)
                {
                    if (index >= children.Count)
                    {
                        Rect bounds = AssociatedElement.GetBounds(children.Last());

                        y = bounds.Y + 60 + (bounds.Height * 0.5);
                    }
                    else if (index == 0)
                    {
                        Rect bounds = AssociatedElement.GetBounds(children.First());

                        y = bounds.Y - 60 - (bounds.Height * 0.5);
                    }
                    else
                    {
                        Rect bounds1 = AssociatedElement.GetBounds(children[index]);
                        Rect bounds2 = AssociatedElement.GetBounds(children[index + 1]);

                        y = (bounds1.CenterY() + bounds2.CenterY()) * 0.5;
                    }
                }
            };

            Node node = other.AssociatedNode as Node;

            if (node != null)
            {
                if (side == NodeSide.Right)
                {
                    y = other.Position.Y;
                    x = other.Position.X + other.ActualWidth + margin;
                }
                else
                {
                    y = other.Position.Y;
                    x = other.Position.X - other.ActualWidth - margin;

                    anchor = AnchorPoint.Right;
                }

                ajustWithChildren();
            }
            else
            {
                RootNode root = (RootNode)other.AssociatedNode;

                Rect rootBounds = AssociatedElement.GetBounds(root);

                if (side == NodeSide.Right)
                {
                    y = rootBounds.Y + (rootBounds.Height * 0.5);
                    x = rootBounds.X + rootBounds.Width + margin;

                    ajustWithChildren();
                }
                else
                {
                    y = rootBounds.Y + (rootBounds.Height * 0.5);
                    x = rootBounds.X - margin;

                    anchor = AnchorPoint.Right;

                    ajustWithChildren();
                }
            }

            AssociatedElement.ShowPreviewElement(new Point(x, y), other.AssociatedNode, anchor);
        }

        private void AssociatedElement_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (clone != null)
            {
                try
                {
                    if (MathHelper.LengthSquared(transform.Position(), initialPosition) > 100)
                    {
                        NodeControl other = FindOtherNodeControl();

                        if (other != null)
                        {
                            AssociatedElement.Document.MakeTransaction("MoveNode", d =>
                            {
                                d.Apply(new RemoveChildCommand(movingNode.Parent, movingNode));
                                d.Apply(new InsertChildCommand(other.AssociatedNode, null, movingNode.NodeSide, movingNode));
                            });
                        }
                        else
                        {
                            Rect bounds = AssociatedElement.GetBounds(nodeControl.AssociatedNode);

                            double nodeCenterX = transform.X + (0.5 * clone.ActualWidth);
                            double nodeCenterY = transform.Y + (0.5 * clone.ActualHeight);

                            RootNode root = AssociatedElement.Document.Root;

                            if (movingNode.Parent == root)
                            {
                                double mindmapCenter = AssociatedElement.NodePanel.ActualWidth * 0.5;

                                if (movingNode.NodeSide == NodeSide.Right && nodeCenterX < mindmapCenter)
                                {
                                    Reorder(root.LeftChildren, nodeCenterY, side: NodeSide.Left);
                                }
                                else if (movingNode.NodeSide == NodeSide.Right)
                                {
                                    Reorder(root.RightChildren, nodeCenterY, side: NodeSide.Right);
                                }
                                else if (movingNode.NodeSide == NodeSide.Left && nodeCenterX > mindmapCenter)
                                {
                                    Reorder(root.RightChildren, nodeCenterY, side: NodeSide.Right);
                                }
                                else
                                {
                                    Reorder(root.LeftChildren, nodeCenterY, side: NodeSide.Left);
                                }
                            }
                            else
                            {
                                Reorder(((Node)movingNode.Parent).Children, nodeCenterY);
                            }
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

        private bool Reorder(IReadOnlyList<Node> collection, double nodeCenterY, NodeSide side = NodeSide.Undefined)
        {
            int index = GetIndex(collection, movingNode, nodeCenterY);

            if (index != -1)
            {
                NodeBase parent = movingNode.Parent;

                int currentIndex = collection.IndexOf(movingNode);

                if (currentIndex != index)
                {
                    AssociatedElement.Document.MakeTransaction("MoveNode", d =>
                    {
                        d.Apply(new RemoveChildCommand(parent, movingNode));
                        d.Apply(new InsertChildCommand(parent, index, side, movingNode));
                    });
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private NodeControl FindOtherNodeControl()
        {
            Rect rect = new Rect(clone.TransformToVisual(AssociatedElement).TransformPoint(new Point(0, 0)), nodeControl.RenderSize);

            double rectArea = rect.Width * rect.Height;

            IEnumerable<NodeControl> otherNodes = VisualTreeHelper.FindElementsInHostCoordinates(rect, AssociatedElement, true).OfType<NodeControl>().Where(x => x != nodeControl).ToList();

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

            return null;
        }

        private int GetIndex(IReadOnlyList<Node> parentCollection, Node node, double centerY)
        {
            int index = 0;
            int nodeIndex = parentCollection.IndexOf(node);

            for (int i = parentCollection.Count - 1; i >= 0; i--)
            {
                Node otherNode = parentCollection[i];

                Rect? bounds = AssociatedElement.GetBounds(otherNode);

                if (bounds != null && centerY > bounds.Value.CenterY())
                {
                    index = i + 1;

                    if (nodeIndex >= 0 && i >= nodeIndex)
                    {
                        index--;
                    }
                    break;
                }
            }

            return index;
        }
    }
}

// ==========================================================================
// NodeMovingBehavior.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using RavenMind.Model;
using SE.Metro.UI;
using SE.Metro.UI.Interactivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace RavenMind.Controls
{
    public sealed class NodeMovingBehavior : Behavior<NodeControl>
    {
        #region Fields

        private static Mindmap mindmap;
        private static Canvas canvas;
        private static Point initialPosition;
        private static NodeControl previousHighlightedControl;
        private static Node node;
        private static NodePath path;
        private static bool isCancelled;
        private static ScrollViewer scroll;

        #endregion

        #region Methods

        protected override void OnAttached()
        {
            AssociatedObject.ManipulationStarted += AssociatedObject_ManipulationStarted;
            AssociatedObject.ManipulationDelta += AssociatedObject_ManipulationDelta;
            AssociatedObject.ManipulationCompleted += AssociatedObject_ManipulationCompleted;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.ManipulationStarted -= AssociatedObject_ManipulationStarted;
            AssociatedObject.ManipulationDelta -= AssociatedObject_ManipulationDelta;
            AssociatedObject.ManipulationCompleted -= AssociatedObject_ManipulationCompleted;
        }

        private void AssociatedObject_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            canvas  = VisualTreeExtensions.FindParent<Canvas>(AssociatedObject);
            scroll  = VisualTreeExtensions.FindParent<ScrollViewer>(canvas);
            mindmap = VisualTreeExtensions.FindParent<Mindmap>(scroll);

            node = AssociatedObject.DataContext as Node;

            if (node != null && node.IsSelected && !AssociatedObject.IsTextEditing)
            {
                isCancelled = false;

                path = mindmap.GetPath(node);

                initialPosition = AssociatedObject.Position;

                AssociatedObject.SetScale(1.05, true);
                AssociatedObject.BringFront();
            }
            else
            {
                isCancelled = true;
                e.Complete();
            }
        }

        private void AssociatedObject_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.Delta.Expansion == 0 && e.Delta.Rotation == 0 && e.Delta.Scale == 1)
            {
                mindmap.MoveNode(node, new Point(e.Delta.Translation.X / scroll.ZoomFactor, e.Delta.Translation.Y / scroll.ZoomFactor), false);

                if (node.Parent is RootNode)
                {
                    mindmap.StayAtSide(node);
                }

                var other = FindOtherNodeControl(node);

                if (other != null && (previousHighlightedControl == null || previousHighlightedControl != other))
                {
                    Unmark();

                    previousHighlightedControl = other;
                    previousHighlightedControl.IsHighlighted = true;

                    path.Visibility = Visibility.Collapsed;
                }
                else if (other == null)
                {
                    path.Visibility = Visibility.Visible;

                    Unmark();
                }

                e.Handled = true;
            }
        }

        private void AssociatedObject_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (isCancelled)
            {
                return;
            }

            try
            {

                Point position = AssociatedObject.Position;

                double dmx = position.X - initialPosition.X;
                double dmy = position.Y - initialPosition.Y;

                double nodeCenterX = position.X + (0.5 * AssociatedObject.RenderSize.Width);
                double nodeCenterY = position.Y + (0.5 * AssociatedObject.RenderSize.Height);

                bool isHandled = false;

                if ((dmx * dmx) + (dmy * dmy) > 100)
                {
                    var other = FindOtherNodeControl(node);

                    if (other != null)
                    {
                        mindmap.Document.BeginTransaction("NodeMoved");
                        mindmap.Document.Apply(new RemoveChildCommand { OldNode = node, Node = node.Parent });
                        mindmap.Document.Apply(new InsertChildCommand { NewNode = node, Node = other.AssociatedNode });
                        mindmap.Document.CommitTransaction();

                        path.RebindParent(other);
                        return;
                    }

                    RootNode root = mindmap.Document.Root;

                    if (node.Parent == root)
                    {
                        double mindmapCenter = canvas.RenderSize.Width * 0.5;

                        if (node.Side == NodeSide.Right && nodeCenterX < mindmapCenter)
                        {
                            Reorder(root.LeftChildren, nodeCenterY, side: NodeSide.Left);
                        }
                        else if (node.Side == NodeSide.Right)
                        {
                            Reorder(root.RightChildren, nodeCenterY, side: NodeSide.Right);
                        }
                        else if (node.Side == NodeSide.Left && nodeCenterX > mindmapCenter)
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
                        isHandled = Reorder(((Node)node.Parent).Children, nodeCenterY);
                    }
                }

                if (!isHandled)
                {
                    mindmap.MoveNode(node, new Point(initialPosition.X - position.X, initialPosition.Y - position.Y), true);
                }
            }
            finally
            {
                Unmark();

                AssociatedObject.SetScale(1, true);
                AssociatedObject.BringBack();

                if (path != null)
                {
                    path.Visibility = Visibility.Visible;
                }

                scroll = null;
                mindmap = null;
                canvas = null;
            }
        }

        private static bool Reorder(ReadOnlyCollection<Node> collection, double nodeCenterY, NodeSide side = NodeSide.Undefined)
        {
            int index = GetIndex(collection, node, nodeCenterY);

            if (index != -1)
            {
                NodeBase parent = node.Parent;
                
                mindmap.Document.BeginTransaction("NodeMoved");
                mindmap.Document.Apply(new RemoveChildCommand { Node = parent, OldNode = node });
                mindmap.Document.Apply(new InsertChildCommand { Node = parent, NewNode = node, Index = index, Side = side });
                mindmap.Document.CommitTransaction();
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void Unmark()
        {
            if (previousHighlightedControl != null)
            {
                previousHighlightedControl.IsHighlighted = false;
                previousHighlightedControl = null;
            }
        }

        private NodeControl FindOtherNodeControl(Node node)
        {
            Point absolutePosition = AssociatedObject.TransformToVisual(Window.Current.Content).TransformPoint(new Point(0, 0));

            Rect rect = new Rect(absolutePosition, AssociatedObject.RenderSize);

            double rectArea = rect.Width * rect.Height;

            IEnumerable<NodeControl> otherNodes = VisualTreeHelper.FindElementsInHostCoordinates(rect, Window.Current.Content, true).OfType<NodeControl>().Where(x => x != AssociatedObject);

            rect.X = AssociatedObject.Position.X;
            rect.Y = AssociatedObject.Position.Y;

            foreach (NodeControl otherNodeControl in otherNodes)
            {
                Rect otherRect = new Rect(otherNodeControl.Position, otherNodeControl.RenderSize);

                double minArea = Math.Min(rectArea, otherRect.Width * otherRect.Height);

                otherRect.Intersect(rect);

                double newArea = otherRect.Width * otherRect.Height;

                if (!double.IsInfinity(newArea) && newArea > 0.5 * minArea)
                {
                    if (otherNodeControl.AssociatedNode is Node && otherNodeControl.AssociatedNode != node)
                    {
                        return otherNodeControl;
                    }
                }
            }

            return null;
        }

        private static int GetIndex(ReadOnlyCollection<Node> parentCollection, Node node, double centerY)
        {
            int index = 0;
            int nodeIndex = parentCollection.IndexOf(node);

            for (int i = parentCollection.Count - 1; i >= 0; i--)
            {
                Node otherNode = parentCollection[i];

                Rect? bounds = mindmap.Layout.GetBounds(otherNode);

                if (bounds != null && centerY > bounds.Value.CenterY())
                {
                    index = i + 1;

                    if (nodeIndex >= 0 && i > nodeIndex)
                    {
                        index--;
                    }
                    break;
                }
            }

            return index;
        }

        #endregion
    }
}
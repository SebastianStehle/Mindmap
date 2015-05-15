// ==========================================================================
// PreviewCalculationProcess.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using SE.Metro;
using SE.Metro.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;

namespace RavenMind.Model.Layouting.Default
{
    public sealed class PreviewCalculationProcess
    {
        private readonly IRenderer renderer;
        private readonly Node movingNode;
        private readonly Point movementCenter;
        private readonly Rect movementBounds;
        private readonly Document document;
        private Point mindmapCenter;
        private DefaultLayout layout;
        private IRenderNode parentRenderNode;
        private IReadOnlyList<NodeBase> children;
        private AnchorPoint anchor;
        private NodeBase parent;
        private NodeSide side;
        private Point position;
        private int renderIndex;
        private int? insertIndex;

        public PreviewCalculationProcess(Document document, DefaultLayout layout, IRenderer renderer, Node movingNode, Rect movementBounds)
        {
            this.layout = layout;
            this.document = document;
            this.renderer = renderer;
            this.movingNode = movingNode;
            this.movementBounds = movementBounds;
            this.movementCenter = movementBounds.Center();
        }

        public AttachTarget CalculateAttachTarget()
        {
            CalculateCenter();

            FindAttachOnParent();

            if (parent != null)
            {
                CalculateParentAttachTarget();
            }
            else
            {
                CalculateReorderTarget();
            }

            if (children != null)
            {
                CalculatePreviewPoint();

                return new AttachTarget(parent, side, insertIndex, position, anchor);
            }
            
            return null;
        }

        private void CalculateCenter()
        {
            double x = 0.5 * document.Size.Width;
            double y = 0.5 * document.Size.Height;

            mindmapCenter = new Point(x, y);
        }

        private void CalculateParentAttachTarget()
        {
            IRenderNode parentRenderNode = renderer.FindRenderNode(parent);

            RootNode root = parent as RootNode;

            if (root != null)
            {
                if (movingNode.NodeSide == NodeSide.Right)
                {
                    side = NodeSide.Right;

                    children = root.RightChildren;
                }
                else
                {
                    side = NodeSide.Left;

                    children = root.LeftChildren;
                }
            }
            else
            {
                side = parent.NodeSide;

                children = ((Node)parent).Children;
            }
        }

        private void CalculateReorderTarget()
        {
            RootNode root = movingNode.Document.Root;

            parent = movingNode.Parent;

            if (movingNode.Parent == root)
            {
                if (movingNode.NodeSide == NodeSide.Right && movementCenter.X < mindmapCenter.X)
                {
                    CalculateReorderTarget(root.LeftChildren, NodeSide.Left);
                }
                else if (movingNode.NodeSide == NodeSide.Right)
                {
                    CalculateReorderTarget(root.RightChildren, NodeSide.Right);
                }
                else if (movingNode.NodeSide == NodeSide.Left && movementCenter.X > mindmapCenter.X)
                {
                    CalculateReorderTarget(root.RightChildren, NodeSide.Right);
                }
                else
                {
                    CalculateReorderTarget(root.LeftChildren, NodeSide.Left);
                }
            }
            else
            {
                CalculateReorderTarget(((Node)movingNode.Parent).Children);
            }
        }

        private void CalculateReorderTarget(IReadOnlyList<Node> collection, NodeSide targetSide = NodeSide.Undefined)
        {
            CalculateIndex(collection);

            if (insertIndex.HasValue && insertIndex >= 0)
            {
                NodeBase parent = movingNode.Parent;

                int currentIndex = collection.IndexOf(movingNode);

                if (currentIndex != insertIndex)
                {
                    children = collection;

                    side = targetSide;
                }
            }
        }

        private void CalculateIndex(IReadOnlyList<Node> collection)
        {
            double centerY = movementCenter.Y;

            int nodeIndex = collection.IndexOf(movingNode);

            insertIndex = 0;

            for (int i = collection.Count - 1; i >= 0; i--)
            {
                Node otherNode = collection[i];

                Rect bounds = renderer.FindRenderNode(otherNode).Bounds;

                if (centerY > bounds.CenterY())
                {
                    renderIndex = i + 1;
                    insertIndex = renderIndex;

                    if (nodeIndex >= 0 && i >= nodeIndex)
                    {
                        insertIndex--;
                    }
                    break;
                }
            }
        }

        private void CalculatePreviewPoint()
        {
            parentRenderNode = renderer.FindRenderNode(parent);

            double y = parentRenderNode.Position.Y;
            double x = parentRenderNode.Position.X + (1.0 * parentRenderNode.Size.Width) + layout.HorizontalMargin;

            anchor = AnchorPoint.Left;

            Action ajustWithChildren = () =>
            {
                if (children.Count > 0)
                {
                    if (!insertIndex.HasValue || insertIndex >= children.Count - 1)
                    {
                        Rect bounds = renderer.FindRenderNode(children.Last()).Bounds;

                        y = bounds.Bottom + (layout.ElementMargin * 2) + (movementBounds.Height * 0.5);
                    }
                    else if (insertIndex == 0)
                    {
                        Rect bounds = renderer.FindRenderNode(children.First()).Bounds;

                        y = bounds.Top - layout.ElementMargin - (movementBounds.Height * 0.5);
                    }
                    else
                    {
                        Rect bounds1 = renderer.FindRenderNode(children[renderIndex - 1]).Bounds;
                        Rect bounds2 = renderer.FindRenderNode(children[renderIndex + 0]).Bounds;

                        y = (bounds1.CenterY() + bounds2.CenterY()) * 0.5;
                    }
                }
            };

            Node node = parent as Node;

            if (node != null)
            {
                if (side == NodeSide.Right)
                {
                    x = parentRenderNode.Position.X + parentRenderNode.Size.Width + layout.HorizontalMargin;
                }
                else
                {
                    x = parentRenderNode.Position.X - parentRenderNode.Size.Width - layout.HorizontalMargin;

                    anchor = AnchorPoint.Right;
                }

                ajustWithChildren();
            }
            else
            {
                RootNode root = (RootNode)parent;

                if (side == NodeSide.Right)
                {
                    x = parentRenderNode.Position.X + (parentRenderNode.Size.Width * 0.5) + layout.HorizontalMargin;

                    ajustWithChildren();
                }
                else
                {
                    x = parentRenderNode.Position.X - (parentRenderNode.Size.Width * 0.5) - layout.HorizontalMargin;

                    anchor = AnchorPoint.Right;

                    ajustWithChildren();
                }
            }

            position = new Point(x, y);
        }

        private void FindAttachOnParent()
        {
            double rectArea = movementBounds.Width * movementBounds.Height;

            foreach (NodeBase node in document.Nodes)
            {
                if (node != movingNode && node != movingNode.Parent && !movingNode.HasChild(node as Node))
                {
                    Rect nodeBounds = renderer.FindRenderNode(node).Bounds;

                    double minArea = Math.Min(rectArea, nodeBounds.Width * nodeBounds.Height);

                    nodeBounds.Intersect(movementBounds);

                    double newArea = nodeBounds.Width * nodeBounds.Height;

                    if (!double.IsInfinity(newArea) && newArea > 0.5 * minArea)
                    {
                        parent = node;
                    }
                }
            }
        }
    }
}

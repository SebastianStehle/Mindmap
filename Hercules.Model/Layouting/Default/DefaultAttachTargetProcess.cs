﻿// ==========================================================================
// DefaultAttachTargetProcess.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using GP.Windows;
using Hercules.Model.Rendering;
using Hercules.Model.Utils;

// ReSharper disable ArrangeThisQualifier

// ReSharper disable LoopCanBePartlyConvertedToQuery

namespace Hercules.Model.Layouting.Default
{
    internal sealed class DefaultAttachTargetProcess : LayoutOperation<DefaultLayout>
    {
        private readonly Node movingNode;
        private readonly Vector2 movementCenter;
        private readonly Rect2 movementBounds;
        private IRenderNode parentRenderNode;
        private IReadOnlyList<NodeBase> children;
        private AnchorPoint anchor;
        private NodeSide side;
        private NodeBase parent;
        private Vector2 mindmapCenter;
        private Vector2 position;
        private int? insertIndex;

        public DefaultAttachTargetProcess(DefaultLayout layout, IRenderScene scene, Document document, Node movingNode, Rect2 movementBounds)
            : base(layout, scene, document)
        {
            Guard.NotNull(movingNode, nameof(movingNode));

            this.movingNode = movingNode;
            this.movementBounds = movementBounds;
            this.movementCenter = movementBounds.Center;
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
            float x = 0.5f * Document.Size.X;
            float y = 0.5f * Document.Size.Y;

            mindmapCenter = new Vector2(x, y);
        }

        private void CalculateParentAttachTarget()
        {
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
                CalculateReorderTarget(((Node)movingNode.Parent).Children, movingNode.Parent.NodeSide);
            }
        }

        private void CalculateReorderTarget(IReadOnlyList<Node> collection, NodeSide targetSide)
        {
            CalculateIndex(collection);

            if (insertIndex.HasValue && insertIndex >= 0)
            {
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

                Rect2 bounds = Scene.FindRenderNode(otherNode).RenderBounds;

                if (centerY > bounds.CenterY)
                {
                    insertIndex = i + 1;

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
            parentRenderNode = Scene.FindRenderNode(parent);

            float y = parentRenderNode.LayoutPosition.Y;
            float x;

            anchor = AnchorPoint.Left;

            Action ajustWithChildren = () =>
            {
                if (children.Count > 0 && !parent.IsCollapsed)
                {
                    if (!insertIndex.HasValue || insertIndex >= children.Count)
                    {
                        Rect2 bounds = Scene.FindRenderNode(children.Last()).RenderBounds;

                        y = bounds.Bottom + (Layout.ElementMargin * 2f) + (movementBounds.Height * 0.5f);
                    }
                    else if (insertIndex == 0)
                    {
                        Rect2 bounds = Scene.FindRenderNode(children.First()).RenderBounds;

                        y = bounds.Top - Layout.ElementMargin - (movementBounds.Height * 0.5f);
                    }
                    else
                    {
                        Rect2 bounds1 = Scene.FindRenderNode(children[insertIndex.Value - 1]).RenderBounds;
                        Rect2 bounds2 = Scene.FindRenderNode(children[insertIndex.Value + 0]).RenderBounds;

                        y = (bounds1.CenterY + bounds2.CenterY) * 0.5f;
                    }
                }
            };

            Node node = parent as Node;

            if (node != null)
            {
                if (side == NodeSide.Right)
                {
                    x = parentRenderNode.LayoutPosition.X + parentRenderNode.RenderSize.X + Layout.HorizontalMargin;
                }
                else
                {
                    x = parentRenderNode.LayoutPosition.X - parentRenderNode.RenderSize.X - Layout.HorizontalMargin;

                    anchor = AnchorPoint.Right;
                }

                ajustWithChildren();
            }
            else
            {
                if (side == NodeSide.Right)
                {
                    x = parentRenderNode.LayoutPosition.X + (parentRenderNode.RenderSize.X * 0.5f) + Layout.HorizontalMargin;

                    ajustWithChildren();
                }
                else
                {
                    x = parentRenderNode.LayoutPosition.X - (parentRenderNode.RenderSize.X * 0.5f) - Layout.HorizontalMargin;

                    anchor = AnchorPoint.Right;

                    ajustWithChildren();
                }
            }

            position = new Vector2(x, y);
        }

        private void FindAttachOnParent()
        {
            double rectArea = movementBounds.Width * movementBounds.Height;

            foreach (NodeBase node in Document.Nodes)
            {
                if (node != movingNode && node != movingNode.Parent && !movingNode.HasChild(node as Node))
                {
                    Rect2 nodeBounds = Scene.FindRenderNode(node).RenderBounds;

                    Rect2 intersection = nodeBounds.Intersect(movementBounds);

                    double intersectionArea = intersection.Width * intersection.Height;

                    if (!double.IsInfinity(intersectionArea))
                    {
                        double minArea = Math.Min(rectArea, nodeBounds.Width * nodeBounds.Height);

                        if (intersectionArea > 0.5f * minArea)
                        {
                            parent = node;
                        }
                    }
                }
            }
        }
    }
}

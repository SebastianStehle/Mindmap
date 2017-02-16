// ==========================================================================
// HorizontalStraightAttachTargetProcess.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using GP.Utils;
using GP.Utils.Mathematics;
using Hercules.Model.Rendering;

// ReSharper disable ArrangeThisQualifier
// ReSharper disable LoopCanBePartlyConvertedToQuery

namespace Hercules.Model.Layouting.HorizontalStraight
{
    internal sealed class HorizontalStraightAttachTargetProcess : LayoutOperation<HorizontalStraightLayout>
    {
        private readonly Node movingNode;
        private readonly Vector2 movementCenter;
        private readonly Rect2 movementBounds;
        private IRenderNode parentRenderNode;
        private IReadOnlyList<NodeBase> children;
        private NodeSide side;
        private NodeBase parent;
        private Vector2 position;
        private int renderIndex;
        private int? insertIndex;

        public HorizontalStraightAttachTargetProcess(HorizontalStraightLayout layout, IRenderScene scene, Document document, Node movingNode, Rect2 movementBounds)
            : base(layout, scene, document)
        {
            Guard.NotNull(movingNode, nameof(movingNode));

            this.movingNode = movingNode;
            this.movementBounds = movementBounds;
            this.movementCenter = movementBounds.Center;
        }

        public AttachTarget CalculateAttachTarget()
        {
            FindAttachOnParent();

            if (parent == null)
            {
                FindReorderTarget();
            }

            if (children == null)
            {
                return null;
            }

            CalculatePreviewPosition();

            return new AttachTarget(parent, side, insertIndex, position);
        }

        private void FindAttachOnParent()
        {
            double rectArea = movementBounds.Area;

            foreach (var node in Document.Nodes)
            {
                if (node == movingNode || node == movingNode.Parent || movingNode.HasChild(node as Node))
                {
                    continue;
                }

                var renderNode = Scene.FindRenderNode(node);

                if (renderNode == null || !renderNode.IsVisible)
                {
                    continue;
                }

                var intersection = renderNode.RenderBounds.Intersect(movementBounds);

                if (intersection == Rect2.Empty)
                {
                    continue;
                }

                var minArea = Math.Min(rectArea, renderNode.RenderBounds.Area);

                if (intersection.Area > 0.5f * minArea)
                {
                    parent = node;
                    break;
                }
            }

            if (parent != null)
            {
                CalculateAttachOnTargetChildren();
            }
        }

        private void CalculateAttachOnTargetChildren()
        {
            var root = parent as RootNode;

            if (root != null)
            {
                side = movingNode.NodeSide;

                children = movingNode.NodeSide == NodeSide.Right ? root.RightChildren : root.LeftChildren;
            }
            else
            {
                side = parent.NodeSide;

                children = ((Node)parent).Children;
            }
        }

        private void FindReorderTarget()
        {
            var root = movingNode.Document.Root;

            parent = movingNode.Parent;

            if (movingNode.Parent == root)
            {
                if (movingNode.NodeSide == NodeSide.Right && movementCenter.X < MindmapCenter.X)
                {
                    FindReorderTarget(root.LeftChildren, NodeSide.Left);
                }
                else if (movingNode.NodeSide == NodeSide.Right)
                {
                    FindReorderTarget(root.RightChildren, NodeSide.Right);
                }
                else if (movingNode.NodeSide == NodeSide.Left && movementCenter.X > MindmapCenter.X)
                {
                    FindReorderTarget(root.RightChildren, NodeSide.Right);
                }
                else
                {
                    FindReorderTarget(root.LeftChildren, NodeSide.Left);
                }
            }
            else
            {
                FindReorderTarget(((Node)movingNode.Parent).Children, movingNode.Parent.NodeSide);
            }
        }

        private void FindReorderTarget(IReadOnlyList<Node> collection, NodeSide targetSide)
        {
            double centerY = movementCenter.Y;

            var nodeIndex = collection.IndexOf(movingNode);

            insertIndex = 0;

            for (var i = collection.Count - 1; i >= 0; i--)
            {
                var otherNode = collection[i];

                var bounds = Scene.FindRenderNode(otherNode).RenderBounds;

                if (centerY <= bounds.CenterY)
                {
                    continue;
                }

                renderIndex = i + 1;
                insertIndex = i + 1;

                if (nodeIndex >= 0 && i >= nodeIndex)
                {
                    insertIndex--;
                }

                break;
            }

            if (nodeIndex != insertIndex)
            {
                children = collection;

                side = targetSide;
            }
        }

        private void CalculatePreviewPosition()
        {
            parentRenderNode = Scene.FindRenderNode(parent);

            var factor = parent is RootNode ? 0.5f : 1.0f;

            var x = CalculateX(factor);
            var y = CalculateY();

            position = new Vector2(x, y);
        }

        private float CalculateX(float factor)
        {
            var x = parentRenderNode.LayoutPosition.X;

            if (side == NodeSide.Right)
            {
                x += (parentRenderNode.RenderSize.X * factor) + Layout.HorizontalMargin;
            }
            else
            {
                x -= (parentRenderNode.RenderSize.X * factor) + Layout.HorizontalMargin;
            }

            return x;
        }

        private float CalculateY()
        {
            var y = parentRenderNode.LayoutPosition.Y;

            if (children.Count > 0 && !parent.IsCollapsed)
            {
                if (!insertIndex.HasValue || renderIndex >= children.Count)
                {
                    y = CalculateYBeforeFirstChild();
                }
                else if (insertIndex == 0)
                {
                    y = CalculateYAfterLastChild();
                }
                else
                {
                    y = CalculateYBetweenChildren();
                }
            }

            return y;
        }

        private float CalculateYBeforeFirstChild()
        {
            var bounds = Scene.FindRenderNode(children.Last()).RenderBounds;

            return bounds.Bottom + (Layout.ElementMargin * 2f) + (movementBounds.Height * 0.5f);
        }

        private float CalculateYAfterLastChild()
        {
            var bounds = Scene.FindRenderNode(children.First()).RenderBounds;

            return bounds.Top - Layout.ElementMargin - (movementBounds.Height * 0.5f);
        }

        private float CalculateYBetweenChildren()
        {
            var bounds1 = Scene.FindRenderNode(children[renderIndex - 1]).RenderBounds;
            var bounds2 = Scene.FindRenderNode(children[renderIndex + 0]).RenderBounds;

            return (bounds1.CenterY + bounds2.CenterY) * 0.5f;
        }
    }
}

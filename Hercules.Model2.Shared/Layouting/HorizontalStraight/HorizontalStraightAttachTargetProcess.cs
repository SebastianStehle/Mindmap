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
using Hercules.Model2.Rendering;

// ReSharper disable ArrangeThisQualifier
// ReSharper disable LoopCanBePartlyConvertedToQuery

namespace Hercules.Model2.Layouting.HorizontalStraight
{
    internal sealed class HorizontalStraightAttachTargetProcess : LayoutOperation<HorizontalStraightLayout>
    {
        private readonly Node movingNode;
        private readonly Vector2 movementCenter;
        private readonly Rect2 movementBounds;
        private IRenderNode parentRenderNode;
        private IReadOnlyList<Node> children;
        private NodeSide side;
        private Node parent;
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
                if (node == movingNode || node.ParentId == movingNode.Id || Document.IsChildOf(node, movingNode))
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
            if (parent.IsRoot)
            {
                side = movingNode.Side;

                children = movingNode.Side == NodeSide.Right ? Document.GetRightChildren() : Document.GetLeftChildren();
            }
            else
            {
                side = parent.Side;

                children = Document.GetChildren(parent);
            }
        }

        private void FindReorderTarget()
        {
            var root = Document.GetRoot();

            parent = Document.GetParent(movingNode);

            if (parent == root)
            {
                if (movingNode.Side == NodeSide.Right && movementCenter.X < MindmapCenter.X)
                {
                    FindReorderTarget(Document.GetLeftChildren(), NodeSide.Left);
                }
                else if (movingNode.Side == NodeSide.Right)
                {
                    FindReorderTarget(Document.GetRightChildren(), NodeSide.Right);
                }
                else if (movingNode.Side == NodeSide.Left && movementCenter.X > MindmapCenter.X)
                {
                    FindReorderTarget(Document.GetRightChildren(), NodeSide.Right);
                }
                else
                {
                    FindReorderTarget(Document.GetLeftChildren(), NodeSide.Left);
                }
            }
            else
            {
                FindReorderTarget(Document.GetChildren(parent), parent.Side);
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

            var factor = parent.IsRoot ? 0.5f : 1.0f;

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

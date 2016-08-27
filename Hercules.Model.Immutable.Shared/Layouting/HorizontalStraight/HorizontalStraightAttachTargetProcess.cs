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
        private readonly NodeSide movingNodeSide;
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

            movingNodeSide = movingNode.Side(document);
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

            foreach (NodeBase node in Document.Nodes().Values)
            {
                if (node == movingNode || node == Document.Parent(movingNode)|| movingNode.HasDescentant(Document, node as Node))
                {
                    continue;
                }

                IRenderNode renderNode = Scene.FindRenderNode(node);

                if (renderNode == null || !renderNode.IsVisible)
                {
                    continue;
                }

                Rect2 intersection = renderNode.RenderBounds.Intersect(movementBounds);

                if (intersection == Rect2.Empty)
                {
                    continue;
                }

                double minArea = Math.Min(rectArea, renderNode.RenderBounds.Area);

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
            RootNode root = parent as RootNode;

            if (root != null)
            {
                side = movingNodeSide;

                children = side == NodeSide.Right ? Document.RightMainNodes() : Document.LeftMainNodes();
            }
            else
            {
                side = parent.Side(Document);

                children = Document.Children(parent as Node);
            }
        }

        private void FindReorderTarget()
        {
            parent = Document.Parent(movingNode);

            if (parent == Document.Root())
            {
                if (movingNodeSide == NodeSide.Right && movementCenter.X < MindmapCenter.X)
                {
                    FindReorderTarget(Document.LeftMainNodes(), NodeSide.Left);
                }
                else if (movingNodeSide == NodeSide.Right)
                {
                    FindReorderTarget(Document.RightMainNodes(), NodeSide.Right);
                }
                else if (movingNodeSide == NodeSide.Left && movementCenter.X > MindmapCenter.X)
                {
                    FindReorderTarget(Document.RightMainNodes(), NodeSide.Right);
                }
                else
                {
                    FindReorderTarget(Document.LeftMainNodes(), NodeSide.Left);
                }
            }
            else
            {
                FindReorderTarget(Document.Children((Node)parent), parent.Side(Document));
            }
        }

        private void FindReorderTarget(IReadOnlyList<Node> collection, NodeSide targetSide)
        {
            double centerY = movementCenter.Y;

            int nodeIndex = collection.IndexOf(movingNode);

            insertIndex = 0;

            for (int i = collection.Count - 1; i >= 0; i--)
            {
                Node otherNode = collection[i];

                Rect2 bounds = Scene.FindRenderNode(otherNode).RenderBounds;

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

            float factor = parent is RootNode ? 0.5f : 1.0f;

            float x = CalculateX(factor);
            float y = CalculateY();

            position = new Vector2(x, y);
        }

        private float CalculateX(float factor)
        {
            float x = parentRenderNode.LayoutPosition.X;

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
            float y = parentRenderNode.LayoutPosition.Y;

            if (children.Count <= 0 || parent.IsCollapsed)
            {
                return y;
            }

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

            return y;
        }

        private float CalculateYBeforeFirstChild()
        {
            Rect2 bounds = Scene.FindRenderNode(children.Last()).RenderBounds;

            return bounds.Bottom + (Layout.ElementMargin * 2f) + (movementBounds.Height * 0.5f);
        }

        private float CalculateYAfterLastChild()
        {
            Rect2 bounds = Scene.FindRenderNode(children.First()).RenderBounds;

            return bounds.Top - Layout.ElementMargin - (movementBounds.Height * 0.5f);
        }

        private float CalculateYBetweenChildren()
        {
            Rect2 bounds1 = Scene.FindRenderNode(children[renderIndex - 1]).RenderBounds;
            Rect2 bounds2 = Scene.FindRenderNode(children[renderIndex + 0]).RenderBounds;

            return (bounds1.CenterY + bounds2.CenterY) * 0.5f;
        }
    }
}

// ==========================================================================
// HorizontalStraightLayoutProcess.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Numerics;
using Hercules.Model.Rendering;

namespace Hercules.Model.Layouting.HorizontalStraight
{
    internal sealed class HorizontalStraightLayoutProcess : LayoutOperation<HorizontalStraightLayout>
    {
        private readonly Dictionary<NodeBase, HorizontalStraightLayoutNode> layoutNodes = new Dictionary<NodeBase, HorizontalStraightLayoutNode>();

        public HorizontalStraightLayoutProcess(HorizontalStraightLayout layout, IRenderScene scene, Document document)
            : base(layout, scene, document)
        {
        }

        public void UpdateLayout()
        {
            try
            {
                ArrangeRoot();
            }
            finally
            {
                layoutNodes.Clear();
            }
        }

        private void ArrangeRoot()
        {
            HorizontalStraightLayoutNode rootLayoutNode = new HorizontalStraightLayoutNode(Scene.FindRenderNode(Document.Root()), null);

            layoutNodes.Add(Document.Root(), rootLayoutNode);

            rootLayoutNode.MoveTo(MindmapCenter, NodeSide.Auto);

            Arrange(rootLayoutNode, Document.LeftMainNodes(), -1.0f, NodeSide.Right);
            Arrange(rootLayoutNode, Document.RightMainNodes(), 1.0f, NodeSide.Left);
        }

        private void Arrange(HorizontalStraightLayoutNode root, IReadOnlyCollection<Node> children, float factor, NodeSide anchor)
        {
            bool rootIsCollapsed = Document.Root().IsCollapsed;

            UpdateSizeWithChildren(root, children, rootIsCollapsed);

            float x = MindmapCenter.X - (factor * 0.5f * root.RenderWidth);
            float y = MindmapCenter.Y;

            root.Position = new Vector2(x, y);

            ArrangeNodes(root, children, factor, anchor, rootIsCollapsed);
        }

        private void ArrangeNodes(HorizontalStraightLayoutNode parent, IReadOnlyCollection<Node> children, float factor, NodeSide anchor, bool isCollapsed)
        {
            if (children.Count <= 0)
            {
                return;
            }

            float x = 0, y = 0;

            if (!isCollapsed)
            {
                x = parent.Position.X
                    + (factor * parent.RenderWidth)
                    + (factor * Layout.HorizontalMargin);
                y = parent.Position.Y - (parent.TreeHeight * 0.5f) + Layout.ElementMargin;
            }

            foreach (Node child in children)
            {
                HorizontalStraightLayoutNode childLayout = layoutNodes[child];

                if (!isCollapsed)
                {
                    float childX = x;
                    float childY = y + (0.5f * childLayout.TreeHeight);

                    childLayout.MoveTo(new Vector2(childX, childY), anchor);

                    y += childLayout.TreeSize.Y;
                }

                ArrangeNodes(childLayout, Document.Children(child), factor, anchor, isCollapsed || child.IsCollapsed);
            }
        }

        private void UpdateSizeWithChildren(HorizontalStraightLayoutNode parent, IReadOnlyCollection<Node> children, bool isCollapsed)
        {
            float treeW = parent.RenderSize.X;
            float treeH = parent.RenderSize.Y;

            if (children.Count > 0)
            {
                if (isCollapsed)
                {
                    foreach (Node child in children)
                    {
                        HorizontalStraightLayoutNode childData = new HorizontalStraightLayoutNode(Scene.FindRenderNode(child), parent);

                        layoutNodes.Add(child, childData);

                        UpdateSizeWithChildren(childData, Document.Children(child), child.IsCollapsed);
                    }
                }
                else
                {
                    float childsW = 0;
                    float childsH = 0;

                    foreach (Node child in children)
                    {
                        HorizontalStraightLayoutNode childData = new HorizontalStraightLayoutNode(Scene.FindRenderNode(child), parent);

                        layoutNodes.Add(child, childData);

                        UpdateSizeWithChildren(childData, Document.Children(child), child.IsCollapsed);

                        childsH += childData.TreeHeight;
                        childsW = Math.Max(childData.TreeWidth, childsW);
                    }

                    treeW += Layout.HorizontalMargin;
                    treeW += childsW;
                    treeH = childsH;
                }
            }

            treeH += 2 * Layout.ElementMargin;

            parent.TreeSize = new Vector2(treeW, treeH);
        }
    }
}

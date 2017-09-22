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
using Hercules.Model2.Rendering;

namespace Hercules.Model2.Layouting.HorizontalStraight
{
    internal sealed class HorizontalStraightLayoutProcess : LayoutOperation<HorizontalStraightLayout>
    {
        public HorizontalStraightLayoutProcess(HorizontalStraightLayout layout, IRenderScene scene, Document document)
            : base(layout, scene, document)
        {
        }

        public void UpdateLayout()
        {
            ArrangeRoot();

            ReleaseLayoutNodes();
        }

        private void ArrangeRoot()
        {
            var root = Document.GetRoot();

            var rootLayoutNode = HorizontalStraightLayoutNode.AttachTo(root, Scene.FindRenderNode(root), null);

            rootLayoutNode.MoveTo(MindmapCenter, NodeSide.Auto);

            Arrange(rootLayoutNode, root, Document.GetLeftChildren(), -1.0f, NodeSide.Right);
            Arrange(rootLayoutNode, root, Document.GetRightChildren(), 1.0f, NodeSide.Left);
        }

        private void Arrange(HorizontalStraightLayoutNode rootLayoutNode, Node root, IReadOnlyCollection<Node> children, float factor, NodeSide anchor)
        {
            UpdateSizeWithChildren(rootLayoutNode, children, root.IsCollapsed);

            var x = MindmapCenter.X - (factor * 0.5f * rootLayoutNode.RenderWidth);
            var y = MindmapCenter.Y;

            rootLayoutNode.Position = new Vector2(x, y);

            ArrangeNodes(rootLayoutNode, children, factor, anchor, root.IsCollapsed);
        }

        private void ArrangeNodes(HorizontalStraightLayoutNode parent, IReadOnlyCollection<Node> children, float factor, NodeSide anchor, bool isCollapsed)
        {
            if (children.Count > 0)
            {
                float x = 0, y = 0;

                if (!isCollapsed)
                {
                    x = parent.Position.X
                     + (factor * parent.RenderWidth)
                     + (factor * Layout.HorizontalMargin);
                    y = parent.Position.Y - (parent.TreeHeight * 0.5f) + Layout.ElementMargin;
                }

                foreach (var child in children)
                {
                    var childLayout = (HorizontalStraightLayoutNode)child.LayoutData;

                    if (!isCollapsed)
                    {
                        var childX = x;
                        var childY = y + (0.5f * childLayout.TreeHeight);

                        childLayout.MoveTo(new Vector2(childX, childY), anchor);

                        y += childLayout.TreeSize.Y;
                    }

                    ArrangeNodes(childLayout, Document.GetChildren(child), factor, anchor, isCollapsed || child.IsCollapsed);
                }
            }
        }

        private void UpdateSizeWithChildren(HorizontalStraightLayoutNode parent, IReadOnlyCollection<Node> children, bool isCollapsed)
        {
            var treeW = parent.RenderSize.X;
            var treeH = parent.RenderSize.Y;

            if (children.Count > 0)
            {
                if (isCollapsed)
                {
                    foreach (var child in children)
                    {
                        var childData = HorizontalStraightLayoutNode.AttachTo(child, Scene.FindRenderNode(child), parent);

                        UpdateSizeWithChildren(childData, Document.GetChildren(child), child.IsCollapsed);
                    }
                }
                else
                {
                    float childsW = 0;
                    float childsH = 0;

                    foreach (var child in children)
                    {
                        var childData = HorizontalStraightLayoutNode.AttachTo(child, Scene.FindRenderNode(child), parent);

                        UpdateSizeWithChildren(childData, Document.GetChildren(child), child.IsCollapsed);

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

        private void ReleaseLayoutNodes()
        {
            foreach (var node in Document.Nodes)
            {
                node.LayoutData = null;
            }
        }
    }
}

// ==========================================================================
// DefaultLayoutProcess.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Numerics;

namespace Hercules.Model.Layouting.Default
{
    internal sealed class DefaultLayoutProcess : LayoutOperation<DefaultLayout>
    {
        private Vector2 minmapCenter;

        public DefaultLayoutProcess(DefaultLayout layout, IRenderScene scene, Document document)
            : base(layout, scene, document)
        {
        }

        public void UpdateLayout()
        {
            CalculateCenter();

            ArrangeRoot();

            ReleaseLayoutNodes();
        }

        private void CalculateCenter()
        {
            float x = 0.5f * Document.Size.X;
            float y = 0.5f * Document.Size.Y;

            minmapCenter = new Vector2(x, y);
        }

        private void ArrangeRoot()
        {
            DefaultLayoutNode rootLayoutNode = DefaultLayoutNode.AttachTo(Document.Root, Scene.FindRenderNode(Document.Root), null);

            rootLayoutNode.MoveTo(minmapCenter, AnchorPoint.Center);

            Arrange(rootLayoutNode, Document.Root.LeftChildren, -1.0f, AnchorPoint.Right);
            Arrange(rootLayoutNode, Document.Root.RightChildren, 1.0f, AnchorPoint.Left);
        }

        private void Arrange(DefaultLayoutNode root, IReadOnlyCollection<Node> children, float factor, AnchorPoint anchor)
        {
            UpdateSizeWithChildren(root, children, Document.Root.IsCollapsed);

            float x = minmapCenter.X - (factor * 0.5f * root.NodeWidth);
            float y = minmapCenter.Y;

            root.Position = new Vector2(x, y);

            ArrangeNodes(root, children, factor, anchor, Document.Root.IsCollapsed);
        }

        private void ArrangeNodes(DefaultLayoutNode parent, IReadOnlyCollection<Node> children, float factor, AnchorPoint anchor, bool isCollapsed)
        {
            if (children.Count > 0)
            {
                float x = 0, y = 0;

                if (!isCollapsed)
                {
                    x = parent.Position.X
                     + (factor * parent.NodeWidth)
                     + (factor * Layout.HorizontalMargin);
                    y = parent.Position.Y - (parent.TreeHeight * 0.5f) + Layout.ElementMargin;
                }

                foreach (Node child in children)
                {
                    DefaultLayoutNode childLayout = (DefaultLayoutNode)child.LayoutData;

                    if (!isCollapsed)
                    {
                        float childX = x;
                        float childY = y + (0.5f * childLayout.TreeHeight);

                        childLayout.MoveTo(new Vector2(childX, childY), anchor);

                        y += childLayout.TreeSize.Y;
                    }

                    ArrangeNodes(childLayout, child.Children, factor, anchor, isCollapsed || child.IsCollapsed);
                }
            }
        }

        private void UpdateSizeWithChildren(DefaultLayoutNode parent, IReadOnlyCollection<Node> children, bool isCollapsed)
        {
            float treeW = parent.NodeSize.X;
            float treeH = parent.NodeSize.Y;

            if (children.Count > 0)
            {
                if (isCollapsed)
                {
                    foreach (Node child in children)
                    {
                        DefaultLayoutNode childData = DefaultLayoutNode.AttachTo(child, Scene.FindRenderNode(child), parent);

                        UpdateSizeWithChildren(childData, child.Children, child.IsCollapsed);
                    }
                }
                else
                {
                    float childsW = 0;
                    float childsH = 0;

                    foreach (Node child in children)
                    {
                        DefaultLayoutNode childData = DefaultLayoutNode.AttachTo(child, Scene.FindRenderNode(child), parent);

                        UpdateSizeWithChildren(childData, child.Children, child.IsCollapsed);

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
            foreach (NodeBase node in Document.Nodes)
            {
                node.LayoutData = null;
            }
        }
    }
}

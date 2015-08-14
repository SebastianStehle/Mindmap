// ==========================================================================
// LayoutProcess.cs
// Hercules Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Numerics;

namespace Hercules.Model.Layouting.Default
{
    internal sealed class LayoutProcess
    {
        private readonly Document document;
        private readonly IRenderer renderer;
        private readonly DefaultLayout layout;
        private Vector2 minmapCenter;

        public LayoutProcess(Document document, DefaultLayout layout, IRenderer renderer)
        {
            this.layout = layout;
            this.document = document;
            this.renderer = renderer;
        }

        public void UpdateLayout()
        {
            CalculateCenter();

            ArrangeRoot();

            ReleaseLayoutNodes();
        }

        private void CalculateCenter()
        {
            float x = 0.5f * document.Size.X;
            float y = 0.5f * document.Size.Y;

            minmapCenter = new Vector2(x, y);
        }

        private void ArrangeRoot()
        {
            DefaultLayoutNode rootLayoutNode = DefaultLayoutNode.AttachTo(document.Root, renderer.FindRenderNode(document.Root), null);
            
            rootLayoutNode.MoveTo(minmapCenter, AnchorPoint.Center);

            Arrange(rootLayoutNode, document.Root.LeftChildren, -1.0f, AnchorPoint.Right);
            Arrange(rootLayoutNode, document.Root.RightChildren, 1.0f, AnchorPoint.Left);
        }

        private void Arrange(DefaultLayoutNode root, IReadOnlyList<Node> children, float factor, AnchorPoint anchor)
        {
            UpdateSizeWithChildren(root, children, document.Root.IsCollapsed);

            float x = minmapCenter.X - (factor * 0.5f * root.NodeWidth);
            float y = minmapCenter.Y;

            root.Position = new Vector2(x, y);

            ArrangeNodes(root, children, factor, anchor, document.Root.IsCollapsed);
        }

        private void ArrangeNodes(DefaultLayoutNode parent, IReadOnlyList<Node> children, float factor, AnchorPoint anchor, bool isCollapsed)
        {
            if (children.Count > 0)
            {
                float x = 0, y = 0;

                if (!isCollapsed)
                {
                    x = parent.Position.X
                     + (factor * parent.NodeWidth)
                     + (factor * layout.HorizontalMargin);
                    y = parent.Position.Y - (parent.TreeHeight * 0.5f) + layout.ElementMargin;
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

        private void UpdateSizeWithChildren(DefaultLayoutNode parent, IReadOnlyList<Node> children, bool isCollapsed)
        {
            float treeW = parent.NodeSize.X;
            float treeH = parent.NodeSize.Y;

            if (children.Count > 0)
            {
                if (isCollapsed)
                {
                    foreach (Node child in children)
                    {
                        DefaultLayoutNode childData = DefaultLayoutNode.AttachTo(child, renderer.FindRenderNode(child), parent);

                        UpdateSizeWithChildren(childData, child.Children, child.IsCollapsed);
                    }
                }
                else
                {
                    float childsW = 0;
                    float childsH = 0;

                    foreach (Node child in children)
                    {
                        DefaultLayoutNode childData = DefaultLayoutNode.AttachTo(child, renderer.FindRenderNode(child), parent);

                        UpdateSizeWithChildren(childData, child.Children, child.IsCollapsed);

                        childsH += childData.TreeHeight;
                        childsW = Math.Max(childData.TreeWidth, childsW);
                    }

                    treeW += layout.HorizontalMargin;
                    treeW += childsW;
                    treeH = childsH;
                }
            }

            treeH += 2 * layout.ElementMargin;

            parent.TreeSize = new Vector2(treeW, treeH);
        }

        private void ReleaseLayoutNodes()
        {
            foreach (NodeBase node in document.Nodes)
            {
                node.LayoutData = null;
            }
        }
    }
}

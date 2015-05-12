// ==========================================================================
// LayoutProcess.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using Windows.Foundation;

namespace RavenMind.Model.Layouting.Default
{
    public sealed class LayoutProcess
    {
        private readonly Document document;
        private readonly IRenderer renderer;
        private readonly DefaultLayout layout;
        private readonly Size availableSize;
        private Point mindmapCenter;

        public LayoutProcess(Document document, DefaultLayout layout, IRenderer renderer, Size availableSize)
        {
            this.layout = layout;
            this.document = document;
            this.renderer = renderer;
            this.availableSize = availableSize;
        }

        public void UpdateLayout()
        {
            CalculateCenter();

            ArrangeRoot();

            ReleaseLayoutNodes();
        }

        private void CalculateCenter()
        {
            double x = 0.5 * availableSize.Width;
            double y = 0.5 * availableSize.Height;

            mindmapCenter = new Point(x, y);
        }

        private void ArrangeRoot()
        {
            DefaultLayoutNode layoutNode = new DefaultLayoutNode(document.Root, renderer.FindRenderNode(document.Root), null);
            
            layoutNode.MoveTo(mindmapCenter, AnchorPoint.Center);

            Arrange(layoutNode, document.Root.LeftChildren, -1.0, AnchorPoint.Right);
            Arrange(layoutNode, document.Root.RightChildren, 1.0, AnchorPoint.Left);
        }

        private void Arrange(DefaultLayoutNode rootLayout, IReadOnlyList<Node> children, double factor, AnchorPoint anchor)
        {
            UpdateSizeWithChildren(rootLayout, children);

            double x = mindmapCenter.X - (factor * 0.5 * rootLayout.NodeWidth);
            double y = mindmapCenter.Y;

            rootLayout.Position = new Point(x, y);

            ArrangeNodes(rootLayout, children, factor, anchor);
        }

        private void ArrangeNodes(DefaultLayoutNode parent, IReadOnlyList<Node> children, double factor, AnchorPoint anchor)
        {
            double x = parent.Position.X
                    + (factor * parent.NodeWidth)
                    + (factor * layout.HorizontalMargin);
            double y = parent.Position.Y - (parent.TreeHeight * 0.5) + layout.ElementMargin;

            foreach (Node child in children)
            {
                DefaultLayoutNode childData = (DefaultLayoutNode)child.Tag;

                double childX = x;
                double childY = y + (0.5 * childData.TreeHeight);

                childData.MoveTo(new Point(childX, childY), anchor);

                ArrangeNodes(childData, child.Children, factor, anchor);

                y += childData.TreeSize.Height;
            }
        }

        private void UpdateSizeWithChildren(DefaultLayoutNode parent, IReadOnlyList<Node> children)
        {
            double treeW = parent.NodeSize.Width;
            double treeH = parent.NodeSize.Height;

            if (children.Count > 0)
            {
                double childsW = 0;
                double childsH = 0;

                foreach (Node child in children)
                {
                    DefaultLayoutNode childData = new DefaultLayoutNode(child, renderer.FindRenderNode(child), parent);

                    UpdateSizeWithChildren(childData, child.Children);

                    childsH += childData.TreeHeight;
                    childsW = Math.Max(childData.TreeWidth, childsW);
                }

                treeW += layout.HorizontalMargin;
                treeW += childsW;
                treeH = childsH;
            }

            treeH += 2 * layout.ElementMargin;

            parent.TreeSize = new Size(treeW, treeH);
        }

        private void ReleaseLayoutNodes()
        {
            foreach (NodeBase node in document.Nodes)
            {
                node.Tag = null;
            }
        }
    }
}

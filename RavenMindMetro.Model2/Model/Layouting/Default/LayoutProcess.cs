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
        private Point mindmapCenter;

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
            double x = 0.5 * document.Size.Width;
            double y = 0.5 * document.Size.Height;

            mindmapCenter = new Point(x, y);
        }

        private void ArrangeRoot()
        {
            DefaultLayoutNode rootLayoutNode = DefaultLayoutNode.AttachTo(document.Root, renderer.FindRenderNode(document.Root), null);
            
            rootLayoutNode.MoveTo(mindmapCenter, AnchorPoint.Center);

            Arrange(rootLayoutNode, document.Root.LeftChildren, -1.0, AnchorPoint.Right);
            Arrange(rootLayoutNode, document.Root.RightChildren, 1.0, AnchorPoint.Left);
        }

        private void Arrange(DefaultLayoutNode root, IReadOnlyList<Node> children, double factor, AnchorPoint anchor)
        {
            UpdateSizeWithChildren(root, children, document.Root.IsCollapsed);

            double x = mindmapCenter.X - (factor * 0.5 * root.NodeWidth);
            double y = mindmapCenter.Y;

            root.Position = new Point(x, y);

            ArrangeNodes(root, children, factor, anchor, document.Root.IsCollapsed);
        }

        private void ArrangeNodes(DefaultLayoutNode parent, IReadOnlyList<Node> children, double factor, AnchorPoint anchor, bool isCollapsed)
        {
            if (children.Count > 0)
            {
                double x = 0, y = 0;

                if (!isCollapsed)
                {
                    x = parent.Position.X
                     + (factor * parent.NodeWidth)
                     + (factor * layout.HorizontalMargin);
                    y = parent.Position.Y - (parent.TreeHeight * 0.5) + layout.ElementMargin;
                }

                foreach (Node child in children)
                {
                    DefaultLayoutNode childLayout = (DefaultLayoutNode)child.LayoutData;

                    if (!isCollapsed)
                    {
                        double childX = x;
                        double childY = y + (0.5 * childLayout.TreeHeight);

                        childLayout.MoveTo(new Point(childX, childY), anchor);

                        y += childLayout.TreeSize.Height;

                        childLayout.Show();
                    }
                    else
                    {
                        childLayout.Hide();
                    }

                    ArrangeNodes(childLayout, child.Children, factor, anchor, isCollapsed || child.IsCollapsed);
                }
            }
        }

        private void UpdateSizeWithChildren(DefaultLayoutNode parent, IReadOnlyList<Node> children, bool isCollapsed)
        {
            double treeW = parent.NodeSize.Width;
            double treeH = parent.NodeSize.Height;

            if (children.Count > 0)
            {
                if (isCollapsed)
                {
                    foreach (Node child in children)
                    {
                        DefaultLayoutNode childData = DefaultLayoutNode.AttachTo(child, renderer.FindRenderNode(child), parent);

                        UpdateSizeWithChildren(childData, child.Children, isCollapsed || child.IsCollapsed);
                    }
                }
                else
                {
                    double childsW = 0;
                    double childsH = 0;

                    foreach (Node child in children)
                    {
                        DefaultLayoutNode childData = DefaultLayoutNode.AttachTo(child, renderer.FindRenderNode(child), parent);

                        UpdateSizeWithChildren(childData, child.Children, isCollapsed || child.IsCollapsed);

                        childsH += childData.TreeHeight;
                        childsW = Math.Max(childData.TreeWidth, childsW);
                    }

                    treeW += layout.HorizontalMargin;
                    treeW += childsW;
                    treeH = childsH;
                }
            }

            treeH += 2 * layout.ElementMargin;

            parent.TreeSize = new Size(treeW, treeH);
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

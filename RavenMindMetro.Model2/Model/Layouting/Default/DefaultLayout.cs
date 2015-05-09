// ==========================================================================
// Layout.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using SE.Metro;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using SE.Metro.UI;
using Windows.Foundation;

namespace RavenMind.Model.Layouting.Default
{
    [Export]
    [Export(typeof(ILayout))]
    public sealed class DefaultLayout : ILayout
    {
        public double HorizontalMargin { get; set; }

        public double ElementMargin { get; set; }

        public void UpdateLayout(Document document, IRenderer renderer, Size availableSize)
        {
            Guard.NotNull(document, "document");
            Guard.NotNull(renderer, "renderer");

            Arrange(document.Root, availableSize, renderer);

            ReleaseTag(document);
        }

        private static void ReleaseTag(Document document)
        {
            foreach (NodeBase node in document.Nodes)
            {
                node.Tag = null;
            }
        }

        private void Arrange(RootNode root, Size availableSize, IRenderer renderer)
        {
            DefaultLayoutNode layoutNode = new DefaultLayoutNode(root, renderer.FindRenderNode(root), null);

            double x = 0.5 * availableSize.Width;
            double y = 0.5 * availableSize.Height;

            Point center = new Point(x, y);

            layoutNode.MoveTo(center, AnchorPoint.Center);

            Arrange(renderer, layoutNode, center, root.LeftChildren, -1.0, AnchorPoint.Right);
            Arrange(renderer, layoutNode, center, root.RightChildren, 1.0, AnchorPoint.Left);
        }

        private void Arrange(IRenderer renderer, DefaultLayoutNode node, Point center, IReadOnlyList<Node> children, double factor, AnchorPoint anchor)
        {
            UpdateSizeWithChildren(node, renderer, children);

            double x = center.X - (factor * 0.5 * node.NodeSize.Width);
            double y = center.Y;

            node.Position = new Point(x, y);

            ArrangeNodes(node, children, factor, anchor);
        }

        private void ArrangeNodes(DefaultLayoutNode parent, IReadOnlyList<Node> children, double factor, AnchorPoint anchor)
        {
            double x = parent.Position.X + ((parent.NodeSize.Width + HorizontalMargin) * factor);
            double y = parent.Position.Y - (parent.TreeSize.Height * 0.5) + ElementMargin;

            foreach (Node child in children)
            {
                DefaultLayoutNode childData = (DefaultLayoutNode)child.Tag;

                double childX = x;
                double childY = y + (0.5 * (childData.TreeSize.Height - childData.NodeSize.Height)) + (0.5 * childData.NodeSize.Height);

                childData.MoveTo(new Point(childX, childY), anchor);

                ArrangeNodes(childData, child.Children, factor, anchor);

                y += childData.TreeSize.Height;
            }
        }

        private void UpdateSizeWithChildren(DefaultLayoutNode parent, IRenderer renderer, IReadOnlyList<Node> children)
        {
            Size treeSize = parent.RenderNode.Size;

            if (children.Count > 0)
            {
                double childrensWidth = 0;
                double childrensHeight = 0;

                foreach (Node child in children)
                {
                    DefaultLayoutNode childData = new DefaultLayoutNode(child, renderer.FindRenderNode(child), parent);

                    UpdateSizeWithChildren(childData, renderer, child.Children);

                    childrensHeight += childData.TreeSize.Height;
                    childrensWidth = Math.Max(childData.TreeSize.Width, childrensWidth);
                }

                treeSize.Width += HorizontalMargin;
                treeSize.Width += childrensWidth;
                treeSize.Height = childrensHeight;
            }

            treeSize.Height += 2 * ElementMargin;

            parent.TreeSize = treeSize;
        }

        public AttachTarget CalculateAttachTarget(NodeBase parent, IRenderer renderer, Node movingNode, Rect movementBounds, Point mindmapCenter)
        {
            PreviewCalculationProcess process = new PreviewCalculationProcess(parent, this, renderer, movingNode, movementBounds, mindmapCenter);

            return process.CalculateAttachTarget();
        }
    }
}

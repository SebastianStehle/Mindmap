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
using System.Collections.ObjectModel;
using System.Composition;
using Windows.Foundation;

namespace RavenMind.Model
{
    [Export]
    [Export(typeof(ILayout))]
    public sealed class Layout : ILayout
    {
        public double HorizontalMargin { get; set; }

        public double ElementMargin { get; set; }

        internal sealed class NodeData
        {
            private readonly INodeView nodeView;
            private readonly NodeData parent;
            private readonly NodeBase node;

            public Size TreeSize { get; set; }

            public Point Position { get; set; }

            public Node Node
            {
                get
                {
                    return node as Node;
                }
            }

            public NodeData Parent
            {
                get
                {
                    return parent;
                }
            }

            public INodeView NodeView
            {
                get
                {
                    return nodeView;
                }
            }

            public Size NodeSize
            {
                get
                {
                    return nodeView.Size;
                }
            }

            public NodeData(NodeBase node, Func<NodeBase, INodeView> views, NodeData parent)
            {
                this.node = node;
                this.parent = parent;
                this.nodeView = views(node);

                node.Tag = this;
               
                TreeSize = nodeView.Size;
            }

            public void UpdatePosition(Point position, AnchorPoint anchor)
            {
                Position = position;

                nodeView.SetPosition(position, anchor);
            }
        }

        public void UpdateLayout(Document document, Func<NodeBase, INodeView> views, Size availableSize)
        {
            Guard.NotNull(document, "document");
            Guard.NotNull(views, "views");

            Arrange(document.Root, availableSize, views);

            ReleaseTag(document);
        }

        private static void ReleaseTag(Document document)
        {
            foreach (NodeBase node in document.Nodes)
            {
                node.Tag = null;
            }
        }

        private void Arrange(RootNode root, Size availableSize, Func<NodeBase, INodeView> views)
        {
            NodeData data = new NodeData(root, views, null);

            double x = 0.5 * availableSize.Width;
            double y = 0.5 * availableSize.Height;

            Point center = new Point { X = x, Y = y };

            data.UpdatePosition(center, AnchorPoint.Center);

            Arrange(data, center, root.LeftChildren, views, -1.0, AnchorPoint.Right);
            Arrange(data, center, root.RightChildren, views, 1.0, AnchorPoint.Left);
        }

        private void Arrange(NodeData node, Point center, IReadOnlyList<Node> children, Func<NodeBase, INodeView> views, double factor, AnchorPoint anchor)
        {
            UpdateSizeWithChildren(node, views, children);

            node.Position = new Point(
                center.X - (factor * 0.5 * node.NodeSize.Width),
                center.Y);

            ArrangeNodes(node, children, factor, anchor);
        }

        private void ArrangeNodes(NodeData parent, IReadOnlyList<Node> children, double factor, AnchorPoint anchor)
        {
            double x = parent.Position.X + (parent.NodeSize.Width + HorizontalMargin) * factor;
            double y = parent.Position.Y - (parent.TreeSize.Height * 0.5) + ElementMargin;

            foreach (Node child in children)
            {
                NodeData childData = (NodeData)child.Tag;

                double childX = x;
                double childY = y + (0.5 * (childData.TreeSize.Height - childData.NodeSize.Height)) + 0.5 * childData.NodeSize.Height;
                
                childData.UpdatePosition(new Point(childX, childY), anchor);

                ArrangeNodes(childData, child.Children, factor, anchor);

                y += childData.TreeSize.Height;
            }
        }

        private void UpdateSizeWithChildren(NodeData parent, Func<NodeBase, INodeView> views, IReadOnlyList<Node> children)
        {
            Size treeSize = parent.NodeView.Size;

            if (children.Count > 0)
            {
                double childrensWidth = 0;
                double childrensHeight = 0;

                foreach (Node child in children)
                {
                    NodeData childData = new NodeData(child, views, parent);

                    UpdateSizeWithChildren(childData, views, child.Children);

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
    }
}

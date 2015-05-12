// ==========================================================================
// DefaultLayoutNode.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.Foundation;

namespace RavenMind.Model.Layouting.Default
{
    internal sealed class DefaultLayoutNode
    {
        private readonly DefaultLayoutNode parent;
        private readonly IRenderNode renderNode;
        private readonly NodeBase node;
        private readonly Size nodeSize;

        public Point Position { get; set; }

        public Size TreeSize { get; set; }

        public double TreeWidth
        {
            get
            {
                return TreeSize.Width;
            }
        }

        public double TreeHeight
        {
            get
            {
                return TreeSize.Height;
            }
        }

        public Node Node
        {
            get
            {
                return node as Node;
            }
        }

        public DefaultLayoutNode Parent
        {
            get
            {
                return parent;
            }
        }

        public Size NodeSize
        {
            get
            {
                return nodeSize;
            }
        }

        public double NodeWidth
        {
            get
            {
                return nodeSize.Width;
            }
        }

        public double NodeHeight
        {
            get
            {
                return nodeSize.Height;
            }
        }

        public DefaultLayoutNode(NodeBase node, IRenderNode renderNode, DefaultLayoutNode parent)
        {
            this.node = node;
            this.parent = parent;
            this.nodeSize = renderNode.Size;
            this.renderNode = renderNode;

            node.Tag = this;

            TreeSize = renderNode.Size;
        }

        public void MoveTo(Point position, AnchorPoint anchor)
        {
            Position = position;

            renderNode.MoveTo(position, anchor);
        }
    }
}

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

        public Size TreeSize { get; set; }

        public Point Position { get; set; }

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

        public IRenderNode RenderNode
        {
            get
            {
                return renderNode;
            }
        }

        public Size NodeSize
        {
            get
            {
                return renderNode.Size;
            }
        }

        public DefaultLayoutNode(NodeBase node, IRenderNode renderNode, DefaultLayoutNode parent)
        {
            this.node = node;
            this.parent = parent;
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

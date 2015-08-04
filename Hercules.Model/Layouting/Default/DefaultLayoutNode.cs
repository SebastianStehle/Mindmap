// ==========================================================================
// DefaultLayoutNode.cs
// Hercules Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Numerics;

namespace Hercules.Model.Layouting.Default
{
    internal sealed class DefaultLayoutNode
    {
        private readonly DefaultLayoutNode parent;
        private readonly IRenderNode renderNode;
        private readonly Vector2 nodeSize;

        public Vector2 Position { get; set; }

        public Vector2 TreeSize { get; set; }

        public float TreeWidth
        {
            get
            {
                return TreeSize.X;
            }
        }

        public float TreeHeight
        {
            get
            {
                return TreeSize.Y;
            }
        }

        public DefaultLayoutNode Parent
        {
            get
            {
                return parent;
            }
        }

        public Vector2 NodeSize
        {
            get
            {
                return nodeSize;
            }
        }

        public float NodeWidth
        {
            get
            {
                return nodeSize.X;
            }
        }

        public float NodeHeight
        {
            get
            {
                return nodeSize.Y;
            }
        }

        public static DefaultLayoutNode AttachTo(NodeBase node, IRenderNode renderNode, DefaultLayoutNode parent)
        {
            DefaultLayoutNode layoutNode = new DefaultLayoutNode(node, renderNode, parent);

            node.LayoutData = layoutNode;

            return layoutNode;
        }

        private DefaultLayoutNode(NodeBase node, IRenderNode renderNode, DefaultLayoutNode parent)
        {
            this.parent = parent;
            this.nodeSize = renderNode.Size;
            this.renderNode = renderNode;

            TreeSize = renderNode.Size;
        }

        public void MoveTo(Vector2 position, AnchorPoint anchor)
        {
            Position = position;

            renderNode.MoveTo(position, anchor);
        }
    }
}

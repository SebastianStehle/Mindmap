// ==========================================================================
// DefaultLayoutNode.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.Foundation;

namespace Mindmap.Model.Layouting.Default
{
    internal sealed class DefaultLayoutNode
    {
        private readonly DefaultLayoutNode parent;
        private readonly IRenderNode renderNode;
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

        public void Show()
        {
            renderNode.Show();
        }

        public void Hide()
        {
            renderNode.Hide();
        }

        public void MoveTo(Point position, AnchorPoint anchor)
        {
            Position = position;

            renderNode.MoveTo(position, anchor);
        }
    }
}

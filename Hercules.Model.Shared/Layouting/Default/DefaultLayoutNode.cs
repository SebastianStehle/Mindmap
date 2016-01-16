﻿// ==========================================================================
// DefaultLayoutNode.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Numerics;
using Hercules.Model.Rendering;

// ReSharper disable ArrangeThisQualifier

namespace Hercules.Model.Layouting.Default
{
    internal sealed class DefaultLayoutNode
    {
        private readonly DefaultLayoutNode parent;
        private readonly Vector2 renderSize;
        private readonly IRenderNode renderNode;

        public Vector2 Position { get; set; }

        public Vector2 TreeSize { get; set; }

        public float TreeWidth
        {
            get { return TreeSize.X; }
        }

        public float TreeHeight
        {
            get { return TreeSize.Y; }
        }

        public DefaultLayoutNode Parent
        {
            get { return parent; }
        }

        public Vector2 RenderSize
        {
            get { return renderSize; }
        }

        public float RenderWidth
        {
            get { return renderSize.X; }
        }

        public float RenderHeight
        {
            get { return renderSize.Y; }
        }

        public static DefaultLayoutNode AttachTo(NodeBase node, IRenderNode renderNode, DefaultLayoutNode parent)
        {
            DefaultLayoutNode layoutNode = new DefaultLayoutNode(renderNode, parent);

            node.LayoutData = layoutNode;

            return layoutNode;
        }

        private DefaultLayoutNode(IRenderNode renderNode, DefaultLayoutNode parent)
        {
            this.parent = parent;
            this.renderSize = renderNode.RenderSize;
            this.renderNode = renderNode;

            TreeSize = renderNode.RenderSize;
        }

        public void MoveTo(Vector2 position, NodeSide anchor)
        {
            Position = position;

            renderNode.MoveToLayout(position, anchor);
        }
    }
}
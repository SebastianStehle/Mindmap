// ==========================================================================
// AttachTarget.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using Windows.Foundation;

namespace RavenMind.Model.Layouting
{
    public sealed class AttachTarget
    {
        private readonly Point position;
        private readonly NodeBase node;
        private readonly NodeSide nodeSide;
        private readonly AnchorPoint anchor;
        private readonly IReadOnlyList<NodeBase> children;
        private readonly int? index;

        public Point Position
        {
            get
            {
                return position;
            }
        }

        public AnchorPoint Anchor
        {
            get
            {
                return anchor;
            }
        }

        public NodeBase Parent
        {
            get
            {
                return node;
            }
        }

        public NodeSide NodeSide
        {
            get
            {
                return nodeSide;
            }
        }

        public int? Index
        {
            get
            {
                return index;
            }
        }

        public IReadOnlyList<NodeBase> Children
        {
            get
            {
                return children;
            }
        }

        public AttachTarget(NodeBase node, NodeSide nodeSide, int? index, IReadOnlyList<NodeBase> children, Point position, AnchorPoint anchor)
        {
            this.node = node;
            this.index = index;
            this.anchor = anchor;
            this.nodeSide = nodeSide;
            this.children = children;
            this.position = position;
        }
    }
}

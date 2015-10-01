// ==========================================================================
// AttachTarget.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Numerics;

namespace Hercules.Model.Layouting
{
    public sealed class AttachTarget
    {
        private readonly Vector2 position;
        private readonly NodeBase node;
        private readonly NodeSide nodeSide;
        private readonly AnchorPoint anchor;
        private readonly int? index;

        public AnchorPoint Anchor
        {
            get { return anchor; }
        }

        public NodeSide NodeSide
        {
            get { return nodeSide; }
        }

        public Vector2 Position
        {
            get { return position; }
        }

        public NodeBase Parent
        {
            get { return node; }
        }

        public int? Index
        {
            get { return index; }
        }

        public AttachTarget(NodeBase node, NodeSide nodeSide, int? index, Vector2 position, AnchorPoint anchor)
        {
            this.node = node;
            this.index = index;
            this.anchor = anchor;
            this.nodeSide = nodeSide;
            this.position = position;
        }
    }
}

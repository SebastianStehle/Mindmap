// ==========================================================================
// AttachTarget.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Numerics;

namespace Hercules.Model2.Layouting
{
    public sealed class AttachTarget
    {
        private readonly Vector2 position;
        private readonly Node node;
        private readonly NodeSide nodeSide;
        private readonly int? index;

        public NodeSide Anchor
        {
            get { return nodeSide.OppositeSide(); }
        }

        public NodeSide NodeSide
        {
            get { return nodeSide; }
        }

        public Vector2 Position
        {
            get { return position; }
        }

        public Node Parent
        {
            get { return node; }
        }

        public int? Index
        {
            get { return index; }
        }

        public AttachTarget(Node node, NodeSide nodeSide, int? index, Vector2 position)
        {
            this.node = node;
            this.index = index;
            this.nodeSide = nodeSide;
            this.position = position;
        }
    }
}

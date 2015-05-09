// ==========================================================================
// Node.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace RavenMind.Model
{
    public class Node : NodeBase
    {
        private readonly List<Node> children = new List<Node>();

        public IReadOnlyList<Node> Children
        {
            get
            {
                return children;
            }
        }

        public Node(Guid id)
            : base(id)
        {
        }

        public override void Insert(Node child, int? index, NodeSide side)
        {
            Add(children, child, index, NodeSide);
        }

        public override bool Remove(Node child, out int oldIndex)
        {
            return Remove(children, child, out oldIndex);
        }

        public override bool HasChild(Node child)
        {
            return child != null && (children.Contains(child) || children.Any(n => n.HasChild(child)));
        }
    }
}

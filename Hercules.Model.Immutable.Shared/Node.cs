// ==========================================================================
// Node.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using GP.Utils;

namespace Hercules.Model
{
    public sealed class Node : NodeBase
    {
        private ImmutableList<Guid> childIds = ImmutableList<Guid>.Empty;
        private NodeSide? calculatedSide;

        public IReadOnlyList<Guid> ChildIds
        {
            get { return childIds; }
        }

        public Node(Guid id) : base(id)
        {
        }

        public override NodeSide Side(Document document)
        {
            if (calculatedSide.HasValue)
            {
                return calculatedSide.Value;
            }

            Guard.NotNull(document, nameof(document));

            NodeBase parent = null;
            while (true)
            {
                NodeBase newParent = document.Parent(this);

                if (newParent is RootNode)
                {
                    break;
                }

                parent = newParent;
            }

            calculatedSide = document.LeftMainNodes().Contains(parent) ? NodeSide.Left : NodeSide.Right;

            return calculatedSide.Value;
        }

        public override bool HasChild(Node child)
        {
            Guard.NotNull(child, nameof(child));

            return childIds.Contains(child.Id);
        }

        public override bool HasDescentant(Document document, Node child)
        {
            Guard.NotNull(child, nameof(child));

            return HasChild(child) || document.Children(this).Any(c => c.HasDescentant(document, child));
        }

        public override NodeBase Insert(Guid nodeId, int? index = null, NodeSide side = NodeSide.Auto)
        {
            return Cloned<Node>(clone => clone.childIds = childIds.Insert(nodeId, index));
        }

        public override NodeBase Remove(Guid nodeId)
        {
            return Cloned<Node>(clone => clone.childIds = childIds.Remove(nodeId));
        }
    }
}

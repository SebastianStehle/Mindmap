// ==========================================================================
// RootNode.cs
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

// ReSharper disable ConvertIfStatementToReturnStatement

namespace Hercules.Model
{
    public sealed class RootNode : NodeBase
    {
        private ImmutableList<Guid> leftChildIds = ImmutableList<Guid>.Empty;
        private ImmutableList<Guid> rightChildIds = ImmutableList<Guid>.Empty;

        public IReadOnlyList<Guid> LeftChildIds
        {
            get { return leftChildIds; }
        }

        public IReadOnlyList<Guid> RightChildIds
        {
            get { return rightChildIds; }
        }

        public RootNode(Guid id)
            : base(id)
        {
        }

        public override NodeSide Side(Document document)
        {
            return NodeSide.Auto;
        }

        public override bool HasChild(Node child)
        {
            Guard.NotNull(child, nameof(child));

            return leftChildIds.Contains(child.Id) || rightChildIds.Contains(child.Id);
        }

        public override bool HasDescentant(Document document, Node child)
        {
            Guard.NotNull(child, nameof(child));

            return HasChild(child) || document.LeftMainNodes().Any(c => c.HasDescentant(document, child)) || document.RightMainNodes().Any(c => c.HasDescentant(document, child));
        }

        public override NodeBase Insert(Guid nodeId, int? index = null, NodeSide side = NodeSide.Auto)
        {
            if (side == NodeSide.Left)
            {
                return Cloned<RootNode>(clone => clone.leftChildIds = leftChildIds.Insert(nodeId, index));
            }
            else if (side == NodeSide.Right)
            {
                return Cloned<RootNode>(clone => clone.rightChildIds = rightChildIds.Insert(nodeId, index));
            }
            else if (rightChildIds.Count > leftChildIds.Count)
            {
                return Cloned<RootNode>(clone => clone.leftChildIds = leftChildIds.Add(nodeId));
            }
            else
            {
                return Cloned<RootNode>(clone => clone.leftChildIds = rightChildIds.Add(nodeId));
            }
        }

        public override NodeBase Remove(Guid nodeId)
        {
            if (leftChildIds.Contains(nodeId))
            {
                return Cloned<RootNode>(clone => clone.rightChildIds = rightChildIds.Remove(nodeId));
            }
            else
            {
                return Cloned<RootNode>(clone => clone.rightChildIds = rightChildIds.Remove(nodeId));
            }
        }
    }
}

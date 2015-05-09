// ==========================================================================
// InsertChildCommand.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace RavenMind.Model
{
    public sealed class InsertChildCommand : CommandBase
    {
        private NodeSide side;
        private Node child;
        private int? index;

        public InsertChildCommand(NodeBase parent, int? index, NodeSide side)
            : base(parent)
        {
            this.side = side;
            this.index = index;
        }

        public InsertChildCommand(NodeBase parent, int? index, NodeSide side, Node child)
            : base(parent)
        {
            this.side = side;
            this.index = index;
            this.child = child;
        }

        public override void Execute()
        {
            if (child == null)
            {
                child = Node.Document.GetOrCreateNode<Node>(Guid.NewGuid(), id => new Node(id));
            }

            Node.Insert(child, index, side);

            child.Select();
        }

        public override void Revert()
        {
            int index = 0;

            Node.Remove(child, out index);

            child.Select();
        }
    }
}

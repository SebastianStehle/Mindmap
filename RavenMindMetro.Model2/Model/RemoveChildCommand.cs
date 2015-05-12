// ==========================================================================
// RemoveChildCommand.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace RavenMind.Model
{
    public sealed class RemoveChildCommand : CommandBase
    {
        private Node child;
        private NodeSide oldSide;
        private int oldIndex;

        public RemoveChildCommand(NodeBase node, Node child)
            : base(node)
        {
            this.child = child;
        }

        protected override void Execute(bool isRedo)
        {
            oldSide = child.NodeSide;
            
            Node.Remove(child, out oldIndex);

            child.Select();
        }

        protected override void Revert()
        {
            Node.Insert(child, oldIndex, oldSide);

            child.Select();
        }
    }
}

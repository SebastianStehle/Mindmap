// ==========================================================================
// RemoveChildCommand.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
namespace Hercules.Model
{
    public sealed class RemoveChildCommand : ChildNodeCommandBase
    {
        private NodeSide oldSide;
        private int oldIndex;

        public RemoveChildCommand(NodeBase node, Node child)
            : base(node, child)
        {
        }

        public RemoveChildCommand(PropertiesBag properties, Document document)
            : base(properties, document)
        {
        }

        protected override void Execute(bool isRedo)
        {
            oldSide = Child.NodeSide;

            Node.Remove(Child, out oldIndex);

            if (isRedo)
            {
                Node.Select();
            }
        }

        protected override void Revert()
        {
            Node.Insert(Child, oldIndex, oldSide);

            Child.Select();
        }
    }
}

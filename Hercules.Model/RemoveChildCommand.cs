// ==========================================================================
// RemoveChildCommand.cs
// Hercules Application
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

        public override void Save(PropertiesBag properties)
        {
            base.Save(properties);
        }

        protected override void Execute(bool isRedo)
        {
            oldSide = Child.NodeSide;

            Node.Remove(Child, out oldIndex);
            Node.Select();
        }

        protected override void Revert()
        {
            Node.Insert(Child, oldIndex, oldSide);

            Child.Select();
        }
    }
}

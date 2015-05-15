// ==========================================================================
// InsertChildCommand.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace RavenMind.Model
{
    public sealed class InsertChildCommand : ChildNodeCommandBase
    {
        private NodeSide side;
        private int? index;

        public InsertChildCommand(CommandProperties properties, Document document)
            : base(properties, document)
        {
            side = (NodeSide)properties.GetInteger("NodeSide");

            index = properties.GetNullableInteger("Index");
        }

        public InsertChildCommand(NodeBase parent, int? index, NodeSide side)
            : this(parent, index, side, null)
        {
        }

        public InsertChildCommand(NodeBase parent, int? index, NodeSide side, Node child)
            : base(parent, child)
        {
            this.side = side;

            this.index = index;
        }

        public override void Save(CommandProperties properties)
        {
            properties.Set("Index", index);
            properties.Set("NodeSide", (int)side);

            base.Save(properties);
        }

        protected override void Execute(bool isRedo)
        {
            Node.Insert(Child, index, side);

            Child.Select();
        }

        protected override void Revert()
        {
            int index = 0;

            Node.Remove(Child, out index);

            Child.Select();
        }
    }
}

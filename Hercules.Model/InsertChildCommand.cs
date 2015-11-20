// ==========================================================================
// InsertChildCommand.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Globalization;

namespace Hercules.Model
{
    public sealed class InsertChildCommand : ChildNodeCommandBase
    {
        private const string PropertyKey_NodeSide = "NodeSide";
        private const string PropertyKey_Index = "Index";
        private readonly NodeSide side;
        private readonly int? index;

        public InsertChildCommand(PropertiesBag properties, Document document)
            : base(properties, document)
        {
            if (properties.Contains(PropertyKey_NodeSide))
            {
                side = (NodeSide)properties[PropertyKey_NodeSide].ToInt32(CultureInfo.InvariantCulture);
            }

            if (properties.Contains(PropertyKey_Index))
            {
                index = properties[PropertyKey_Index].ToNullableInt32(CultureInfo.InvariantCulture);
            }
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

        public override void Save(PropertiesBag properties)
        {
            properties.Set(PropertyKey_Index, index);
            properties.Set(PropertyKey_NodeSide, (int)side);

            base.Save(properties);
        }

        protected override void Execute(bool isRedo)
        {
            Node.Insert(Child, index, side);

            Child.Select();
        }

        protected override void Revert()
        {
            int tempIndex;

            Node.Remove(Child, out tempIndex);

            Node.Select();
        }
    }
}

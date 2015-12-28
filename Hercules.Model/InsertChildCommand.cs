// ==========================================================================
// InsertChildCommand.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace Hercules.Model
{
    public sealed class InsertChildCommand : ChildNodeCommandBase
    {
        private const string PropertyNodeSide = "NodeSide";
        private const string PropertyIndex = "Index";
        private const string PropertyIndexOld = "Key";
        private readonly NodeSide side;
        private readonly int? index;

        public InsertChildCommand(PropertiesBag properties, Document document)
            : base(properties, document)
        {
            int value;

            if (properties.TryParseInt32(PropertyNodeSide, out value) && Enum.IsDefined(typeof(NodeShape), value))
            {
                side = (NodeSide)value;
            }
            else
            {
                side = NodeSide.Right;
            }

            bool isIndexParsed =
                properties.TryParseNullableInt32(PropertyIndex, out index) ||
                properties.TryParseNullableInt32(PropertyIndexOld, out index);

            if (!isIndexParsed)
            {
                index = null;
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
            if (index.HasValue)
            {
                properties.Set(PropertyIndex, index);
            }

            properties.Set(PropertyNodeSide, (int)side);

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

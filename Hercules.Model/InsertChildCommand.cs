// ==========================================================================
// InsertChildCommand.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Globalization;

namespace Hercules.Model
{
    [LegacyProperty("Key", "Index")]
    public sealed class InsertChildCommand : ChildNodeCommandBase
    {
        private const string PropertyKeyForNodeSide = "NodeSide";
        private const string PropertyKeyForIndex = "Index";
        private readonly NodeSide side;
        private readonly int? index;

        public InsertChildCommand(PropertiesBag properties, Document document)
            : base(properties, document)
        {
            if (properties.Contains(PropertyKeyForNodeSide))
            {
                try
                {
                    side = (NodeSide)properties[PropertyKeyForNodeSide].ToInt32(CultureInfo.InvariantCulture);
                }
                catch (InvalidCastException)
                {
                    side = NodeSide.Right;
                }
            }

            if (properties.Contains(PropertyKeyForIndex))
            {
                try
                {
                    index = properties[PropertyKeyForIndex].ToNullableInt32(CultureInfo.InvariantCulture);
                }
                catch (InvalidCastException)
                {
                    index = null;
                }
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
                properties.Set(PropertyKeyForIndex, index);
            }

            properties.Set(PropertyKeyForNodeSide, (int)side);

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

﻿// ==========================================================================
// InsertChildCommand.cs
// Hercules Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Globalization;

namespace Hercules.Model
{
    public sealed class InsertChildCommand : ChildNodeCommandBase
    {
        private NodeSide side;
        private int? index;

        public InsertChildCommand(PropertiesBag properties, Document document)
            : base(properties, document)
        {
            side = (NodeSide)properties["NodeSide"].ToInt32(CultureInfo.InvariantCulture);

            index = properties["Index"].ToNullableInt32(CultureInfo.InvariantCulture);
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
// ==========================================================================
// ChangeIconPositionCommand.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using GP.Utils;

namespace Hercules.Model
{
    public sealed class ChangeIconPositionCommand : CommandBase<NodeBase>
    {
        private const string PropertyIconPosition = "IconPosition";
        private readonly IconPosition newIconPosition;
        private IconPosition oldIconPosition;

        public IconPosition NewIconPosition
        {
            get { return newIconPosition; }
        }

        public ChangeIconPositionCommand(PropertiesBag properties, Document document)
            : base(properties, document)
        {
            IconPosition value;

            int intValue;

            if (properties.TryParseEnum(PropertyIconPosition, out value))
            {
                newIconPosition = value;
            }
            else if (properties.TryParseInt32(PropertyIconPosition, out intValue) && Enum.IsDefined(typeof(NodeSide), intValue))
            {
                newIconPosition = (IconPosition)intValue;
            }
            else
            {
                newIconPosition = IconPosition.Left;
            }
        }

        public ChangeIconPositionCommand(NodeBase node, IconPosition newIconPosition)
            : base(node)
        {
            this.newIconPosition = newIconPosition;
        }

        public override void Save(PropertiesBag properties)
        {
            properties.Set(PropertyIconPosition, newIconPosition.ToString());

            base.Save(properties);
        }

        protected override void Execute(bool isRedo)
        {
            oldIconPosition = Node.IconPosition;

            Node.ChangeIconPosition(newIconPosition);

            if (isRedo)
            {
                Node.Select();
            }
        }

        protected override void Revert()
        {
            Node.ChangeIconPosition(oldIconPosition);
            Node.Select();
        }
    }
}

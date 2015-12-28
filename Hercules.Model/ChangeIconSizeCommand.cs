// ==========================================================================
// ChangeIconSizeCommand.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace Hercules.Model
{
    public sealed class ChangeIconSizeCommand : CommandBase
    {
        private const string PropertyIconSize = "IconSize";
        private readonly IconSize newIconSize;
        private IconSize oldIconSize;

        public IconSize NewIconSize
        {
            get { return newIconSize; }
        }

        public ChangeIconSizeCommand(PropertiesBag properties, Document document)
            : base(properties, document)
        {
            IconSize value;

            int intValue;

            if (properties.TryParseEnum(PropertyIconSize, out value))
            {
                newIconSize = value;
            }
            else if (properties.TryParseInt32(PropertyIconSize, out intValue) && Enum.IsDefined(typeof(NodeSide), intValue))
            {
                newIconSize = (IconSize)intValue;
            }
            else
            {
                newIconSize = IconSize.Small;
            }
        }

        public ChangeIconSizeCommand(NodeBase node, IconSize newIconSize)
            : base(node)
        {
            this.newIconSize = newIconSize;
        }

        public override void Save(PropertiesBag properties)
        {
            properties.Set(PropertyIconSize, newIconSize.ToString());

            base.Save(properties);
        }

        protected override void Execute(bool isRedo)
        {
            oldIconSize = Node.IconSize;

            Node.SetupIconSize(newIconSize);
            Node.Select();
        }

        protected override void Revert()
        {
            Node.SetupIconSize(oldIconSize);
            Node.Select();
        }
    }
}

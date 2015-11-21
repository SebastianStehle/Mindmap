// ==========================================================================
// ChangeIconSizeCommand.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

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
            int value;

            if (properties.TryParseInt32(PropertyIconSize, out value))
            {
                newIconSize = (IconSize)value;
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
            properties.Set(PropertyIconSize, (int)newIconSize);

            base.Save(properties);
        }

        protected override void Execute(bool isRedo)
        {
            oldIconSize = Node.IconSize;

            Node.ChangeIconSize(newIconSize);
            Node.Select();
        }

        protected override void Revert()
        {
            Node.ChangeIconSize(oldIconSize);
            Node.Select();
        }
    }
}

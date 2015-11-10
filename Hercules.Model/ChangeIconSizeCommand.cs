// ==========================================================================
// ChangeIconSizeCommand.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Globalization;

namespace Hercules.Model
{
    public sealed class ChangeIconSizeCommand : CommandBase
    {
        private readonly IconSize newIconSize;

        private IconSize oldIconSize;

        public ChangeIconSizeCommand(PropertiesBag properties, Document document)
            : base(properties, document)
        {
            newIconSize = (IconSize)properties["IconSize"].ToInt32(CultureInfo.InvariantCulture);
        }

        public ChangeIconSizeCommand(NodeBase node, IconSize newIconSize)
            : base(node)
        {
            this.newIconSize = newIconSize;
        }

        public override void Save(PropertiesBag properties)
        {
            properties.Set("IconSize", (int)newIconSize);

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

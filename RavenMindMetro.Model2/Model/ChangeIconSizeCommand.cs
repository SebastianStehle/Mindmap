// ==========================================================================
// ChangeIconSizeCommand.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace RavenMind.Model
{
    public sealed class ChangeIconSizeCommand : CommandBase
    {
        private IconSize newIconSize;
        private IconSize oldIconSize;

        public ChangeIconSizeCommand(CommandProperties properties, Document document)
            : base(properties, document)
        {
            newIconSize = properties.Get<IconSize>("IconSize");
        }

        public ChangeIconSizeCommand(NodeBase node, IconSize newIconSize)
            : base(node)
        {
            this.newIconSize = newIconSize;
        }

        public override void Save(CommandProperties properties)
        {
            properties.Set("IconSize", newIconSize);

            base.Save(properties);
        }

        public override void Execute()
        {
            oldIconSize = Node.IconSize;

            Node.ChangeIconSize(newIconSize);
            Node.Select();
        }

        public override void Revert()
        {
            Node.ChangeIconSize(oldIconSize);
            Node.Select();
        }
    }
}

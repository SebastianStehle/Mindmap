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
            newIconSize = (IconSize)properties.GetInteger("IconSize");
        }

        public ChangeIconSizeCommand(NodeBase node, IconSize newIconSize)
            : base(node)
        {
            this.newIconSize = newIconSize;
        }

        public override void Save(CommandProperties properties)
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

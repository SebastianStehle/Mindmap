// ==========================================================================
// ChangeIconCommand.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
namespace Hercules.Model
{
    public sealed class ChangeIconCommand : CommandBase
    {
        private readonly INodeIcon newIcon;

        private INodeIcon oldIcon;

        public ChangeIconCommand(PropertiesBag properties, Document document)
            : base(properties, document)
        {
            newIcon = KeyIcon.TryParse(properties) ?? AttachmentIcon.TryParse(properties);
        }

        public ChangeIconCommand(NodeBase nodeId, INodeIcon newIcon)
            : base(nodeId)
        {
            this.newIcon = newIcon;
        }

        public override void Save(PropertiesBag properties)
        {
            newIcon.Save(properties);

            base.Save(properties);
        }

        protected override void Execute(bool isRedo)
        {
            oldIcon = Node.Icon;

            Node.ChangeIcon(newIcon);
            Node.Select();
        }

        protected override void Revert()
        {
            Node.ChangeIcon(oldIcon);
            Node.Select();
        }
    }
}

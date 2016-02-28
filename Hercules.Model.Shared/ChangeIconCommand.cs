// ==========================================================================
// ChangeIconCommand.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GP.Utils;
using Hercules.Model.Storing;

namespace Hercules.Model
{
    [LegacyName("Hercules.Model.ChangeIconKeyCommand, Hercules.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    public sealed class ChangeIconCommand : CommandBase<NodeBase>
    {
        private readonly INodeIcon newIcon;
        private INodeIcon oldIcon;

        public INodeIcon NewIcon
        {
            get { return newIcon; }
        }

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
            if (newIcon != null)
            {
                newIcon.Save(properties);
            }

            base.Save(properties);
        }

        protected override void Execute(bool isRedo)
        {
            oldIcon = Node.Icon;

            Node.ChangeIcon(newIcon);

            if (isRedo)
            {
                Node.Select();
            }
        }

        protected override void Revert()
        {
            Node.ChangeIcon(oldIcon);
            Node.Select();
        }
    }
}

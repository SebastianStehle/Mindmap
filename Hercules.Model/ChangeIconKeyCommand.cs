// ==========================================================================
// ChangeIconKeyCommand.cs
// Hercules Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace Hercules.Model
{
    public sealed class ChangeIconKeyCommand : CommandBase
    {
        private readonly string newIconKey;
        private string oldIconKey;

        public ChangeIconKeyCommand(PropertiesBag properties, Document document)
            : base(properties, document)
        {
            newIconKey = properties["IconKey"].ToString();
        }

        public ChangeIconKeyCommand(NodeBase nodeId, string newIconKey)
            : base(nodeId)
        {
            this.newIconKey = newIconKey;
        }

        public override void Save(PropertiesBag properties)
        {
            properties.Set("IconKey", newIconKey);

            base.Save(properties);
        }

        protected override void Execute(bool isRedo)
        {
            oldIconKey = Node.IconKey;

            Node.ChangeIconKey(newIconKey);
            Node.Select();
        }

        protected override void Revert()
        {
            Node.ChangeIconKey(oldIconKey);
            Node.Select();
        }
    }
}

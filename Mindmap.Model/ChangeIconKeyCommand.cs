// ==========================================================================
// ChangeIconKeyCommand.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace MindmapApp.Model
{
    public sealed class ChangeIconKeyCommand : CommandBase
    {
        private string newIconKey;
        private string oldIconKey;

        public ChangeIconKeyCommand(CommandProperties properties, Document document)
            : base(properties, document)
        {
            newIconKey = properties.GetString("IconKey");
        }

        public ChangeIconKeyCommand(NodeBase nodeId, string newIconKey)
            : base(nodeId)
        {
            this.newIconKey = newIconKey;
        }

        public override void Save(CommandProperties properties)
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

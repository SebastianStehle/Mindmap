// ==========================================================================
// ChangeTextCommand.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace RavenMind.Model
{
    public sealed class ChangeTextCommand : CommandBase
    {
        private string newText;
        private string oldText;

        public ChangeTextCommand(CommandProperties properties, Document document)
            : base(properties, document)
        {
            newText = properties.Get<string>("Text");
        }

        public ChangeTextCommand(NodeBase nodeId, string newText)
            : base(nodeId)
        {
            this.newText = newText;
        }

        public override void Save(CommandProperties properties)
        {
            properties.Set("Text", newText);

            base.Save(properties);
        }

        protected override void Execute(bool isRedo)
        {
            oldText = Node.Text;

            Node.ChangeText(newText);

            if (isRedo)
            {
                Node.Select();
            }
        }

        protected override void Revert()
        {
            Node.ChangeText(oldText);
            Node.Select();
        }
    }
}

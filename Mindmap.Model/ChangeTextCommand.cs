// ==========================================================================
// ChangeTextCommand.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace Mindmap.Model
{
    public sealed class ChangeTextCommand : CommandBase
    {
        private string newText;
        private string oldText;
        private bool disableSelection;

        public ChangeTextCommand(CommandProperties properties, Document document)
            : base(properties, document)
        {
            newText = properties.GetString("Text");
        }

        public ChangeTextCommand(NodeBase nodeId, string newText, bool disableSelection)
            : base(nodeId)
        {
            this.newText = newText;

            this.disableSelection = disableSelection;
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

            if (!disableSelection)
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

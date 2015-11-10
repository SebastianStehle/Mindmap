// ==========================================================================
// ChangeTextCommand.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
namespace Hercules.Model
{
    public sealed class ChangeTextCommand : CommandBase
    {
        private readonly string newText;
        private readonly bool disableSelection;

        private string oldText;

        public ChangeTextCommand(PropertiesBag properties, Document document)
            : base(properties, document)
        {
            newText = properties["Text"].ToString();
        }

        public ChangeTextCommand(NodeBase nodeId, string newText, bool disableSelection)
            : base(nodeId)
        {
            this.newText = newText;

            this.disableSelection = disableSelection;
        }

        public override void Save(PropertiesBag properties)
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

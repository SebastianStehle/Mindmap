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
        private const string PropertyKey_Text = "Text";
        private readonly string newText;
        private readonly bool disableSelection;

        private string oldText;

        public string NewText
        {
            get { return newText; }
        }

        public ChangeTextCommand(PropertiesBag properties, Document document)
            : base(properties, document)
        {
            newText = properties.Contains(PropertyKey_Text) ? properties[PropertyKey_Text].ToString() : string.Empty;
        }

        public ChangeTextCommand(NodeBase nodeId, string newText, bool disableSelection)
            : base(nodeId)
        {
            this.newText = newText;

            this.disableSelection = disableSelection;
        }

        public override void Save(PropertiesBag properties)
        {
            properties.Set(PropertyKey_Text, newText);

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

// ==========================================================================
// ChangeTextCommand.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
namespace Hercules.Model
{
    public sealed class ChangeTextCommand : CommandBase<NodeBase>
    {
        private const string PropertyText = "Text";
        private readonly string newText;
        private string oldText;

        public string NewText
        {
            get { return newText; }
        }

        public ChangeTextCommand(PropertiesBag properties, Document document)
            : base(properties, document)
        {
            properties.TryParseString(PropertyText, out newText);
        }

        public ChangeTextCommand(NodeBase nodeId, string newText)
            : base(nodeId)
        {
            this.newText = newText;
        }

        public override void Save(PropertiesBag properties)
        {
            properties.Set(PropertyText, newText);

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

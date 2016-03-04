// ==========================================================================
// ChangeNotesCommand.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GP.Utils;

namespace Hercules.Model
{
    public sealed class ChangeNotesCommand : CommandBase<NodeBase>
    {
        private const string PropertyNotes = "Notes";
        private readonly string newNotes;
        private string oldNotes;

        public string NewNotes
        {
            get { return newNotes; }
        }

        public ChangeNotesCommand(PropertiesBag properties, Document document)
            : base(properties, document)
        {
            properties.TryParseString(PropertyNotes, out newNotes);
        }

        public ChangeNotesCommand(NodeBase nodeId, string newNotes)
            : base(nodeId)
        {
            this.newNotes = newNotes;
        }

        public override void Save(PropertiesBag properties)
        {
            properties.Set(PropertyNotes, newNotes);

            base.Save(properties);
        }

        protected override void Execute(bool isRedo)
        {
            oldNotes = Node.Notes;

            Node.ChangeNotes(newNotes);

            if (isRedo)
            {
                Node.Select();
            }
        }

        protected override void Revert()
        {
            Node.ChangeNotes(oldNotes);
            Node.Select();
        }
    }
}

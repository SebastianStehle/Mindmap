// ==========================================================================
// ToggleNotesCommand.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GP.Utils;

namespace Hercules.Model
{
    public sealed class ToggleNotesCommand : CommandBase<NodeBase>
    {
        public ToggleNotesCommand(PropertiesBag properties, Document document)
            : base(properties, document)
        {
        }

        public ToggleNotesCommand(NodeBase node)
            : base(node)
        {
        }

        protected override void Execute(bool isRedo)
        {
            Node.ChangeIsNotesEnabled(!Node.IsNotesEnabled);

            if (isRedo)
            {
                Node.Select();
            }
        }

        protected override void Revert()
        {
            Execute(true);
        }
    }
}

// ==========================================================================
// ToggleCheckableCommand.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
namespace Hercules.Model
{
    public sealed class ToggleCheckableCommand : CommandBase<NodeBase>
    {
        public ToggleCheckableCommand(PropertiesBag properties, Document document)
            : base(properties, document)
        {
        }

        public ToggleCheckableCommand(NodeBase node)
            : base(node)
        {
        }

        protected override void Execute(bool isRedo)
        {
            Node.ChangeIsCheckable(!Node.IsCheckable);

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

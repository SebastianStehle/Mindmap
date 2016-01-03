// ==========================================================================
// ToggleCollapseCommand.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
namespace Hercules.Model
{
    public sealed class ToggleCollapseCommand : CommandBase<NodeBase>
    {
        public ToggleCollapseCommand(PropertiesBag properties, Document document)
            : base(properties, document)
        {
        }

        public ToggleCollapseCommand(NodeBase node)
            : base(node)
        {
        }

        protected override void Execute(bool isRedo)
        {
            Node.ChangeIsCollapsed(!Node.IsCollapsed);

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

// ==========================================================================
// ToggleCollapseCommand.cs
// Hercules Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace Hercules.Model
{
    public sealed class ToggleCollapseCommand : CommandBase
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
        }

        protected override void Revert()
        {
            Node.ChangeIsCollapsed(!Node.IsCollapsed);
        }
    }
}

﻿// ==========================================================================
// ToggleCollapseCommand.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace RavenMind.Model
{
    public sealed class ToggleCollapseCommand : CommandBase
    {
        public ToggleCollapseCommand(CommandProperties properties, Document document)
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

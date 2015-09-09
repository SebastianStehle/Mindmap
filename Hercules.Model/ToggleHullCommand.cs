// ==========================================================================
// ToggleHullCommand.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
namespace Hercules.Model
{
    public sealed class ToggleHullCommand : CommandBase
    {
        public ToggleHullCommand(PropertiesBag properties, Document document)
            : base(properties, document)
        {
        }

        public ToggleHullCommand(NodeBase node)
            : base(node)
        {
        }

        protected override void Execute(bool isRedo)
        {
            Node.ChangeIsShowingHull(!Node.IsShowingHull);
        }

        protected override void Revert()
        {
            Node.ChangeIsShowingHull(!Node.IsShowingHull);
        }
    }
}

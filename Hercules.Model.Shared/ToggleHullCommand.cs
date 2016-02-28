// ==========================================================================
// ToggleHullCommand.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GP.Utils;

namespace Hercules.Model
{
    public sealed class ToggleHullCommand : CommandBase<NodeBase>
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

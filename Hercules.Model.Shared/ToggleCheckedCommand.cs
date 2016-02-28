// ==========================================================================
// ToggleCheckedCommand.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GP.Utils;

namespace Hercules.Model
{
    public sealed class ToggleCheckedCommand : CommandBase<NodeBase>
    {
        public ToggleCheckedCommand(PropertiesBag properties, Document document)
            : base(properties, document)
        {
        }

        public ToggleCheckedCommand(NodeBase node)
            : base(node)
        {
        }

        protected override void Execute(bool isRedo)
        {
            Node.ChangeIsChecked(!Node.IsChecked);

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

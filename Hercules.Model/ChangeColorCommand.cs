// ==========================================================================
// ChangeColorCommand.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace Hercules.Model
{
    public sealed class ChangeColorCommand : CommandBase
    {
        private readonly IColor newColor;
        private IColor oldColor;

        public ChangeColorCommand(PropertiesBag properties, Document document)
            : base(properties, document)
        {
            newColor = ThemeColor.TryParse(properties) ?? CustomColor.TryParse(properties);
        }

        public ChangeColorCommand(NodeBase node, IColor newColor)
            : base(node)
        {
            this.newColor = newColor;
        }

        public override void Save(PropertiesBag properties)
        {
            newColor.Save(properties);

            base.Save(properties);
        }

        protected override void Execute(bool isRedo)
        {
            oldColor = Node.Color;

            Node.ChangeColor(newColor);
            Node.Select();
        }

        protected override void Revert()
        {
            Node.ChangeColor(oldColor);
            Node.Select();
        }
    }
}

// ==========================================================================
// ChangeColorCommand.cs
// Hercules Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Globalization;

namespace Hercules.Model
{
    public sealed class ChangeColorCommand : CommandBase
    {
        private int newColor;
        private int oldColor;

        public ChangeColorCommand(PropertiesBag properties, Document document)
            : base(properties, document)
        {
            newColor = properties["Color"].ToInt32(CultureInfo.InvariantCulture);
        }

        public ChangeColorCommand(NodeBase node, int newColor)
            : base(node)
        {
            this.newColor = newColor;
        }

        public override void Save(PropertiesBag properties)
        {
            properties.Set("Color", newColor);

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

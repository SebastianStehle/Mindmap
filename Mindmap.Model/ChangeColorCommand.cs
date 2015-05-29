// ==========================================================================
// ChangeColorCommand.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace MindmapApp.Model
{
    public sealed class ChangeColorCommand : CommandBase
    {
        private int newColor;
        private int oldColor;

        public ChangeColorCommand(CommandProperties properties, Document document)
            : base(properties, document)
        {
            newColor = properties.GetInteger("Color");
        }

        public ChangeColorCommand(NodeBase node, int newColor)
            : base(node)
        {
            this.newColor = newColor;
        }

        public override void Save(CommandProperties properties)
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

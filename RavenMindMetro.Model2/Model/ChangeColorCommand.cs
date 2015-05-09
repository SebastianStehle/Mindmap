// ==========================================================================
// ChangeColorCommand.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace RavenMind.Model
{
    public sealed class ChangeColorCommand : CommandBase
    {
        private int newColor;
        private int oldColor;

        public ChangeColorCommand(CommandProperties properties, Document document)
            : base(properties, document)
        {
            newColor = properties.Get<int>("Color");
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

        public override void Execute()
        {
            oldColor = Node.Color;

            Node.ChangeColor(newColor);
            Node.Select();
        }

        public override void Revert()
        {
            Node.ChangeColor(oldColor);
            Node.Select();
        }
    }
}

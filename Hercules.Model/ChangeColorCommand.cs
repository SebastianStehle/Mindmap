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
        private readonly INodeColor newColor;

        private INodeColor oldColor;

        public INodeColor NewColor
        {
            get { return newColor; }
        }

        public ChangeColorCommand(PropertiesBag properties, Document document)
            : base(properties, document)
        {
            newColor = ThemeColor.TryParse(properties) ?? ValueColor.TryParse(properties);
        }

        public ChangeColorCommand(NodeBase node, INodeColor newColor)
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

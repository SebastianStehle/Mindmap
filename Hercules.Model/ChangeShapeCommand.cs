// ==========================================================================
// ChangeShapeCommand.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
// ReSharper disable SuggestBaseTypeForParameter

namespace Hercules.Model
{
    public sealed class ChangeShapeCommand : CommandBase
    {
        private const string PropertyShape = "Shape";
        private readonly NodeShape? newShape;
        private NodeShape? oldShape;

        public NodeShape? NewShape
        {
            get { return newShape; }
        }

        public ChangeShapeCommand(PropertiesBag properties, Document document)
            : base(properties, document)
        {
            NodeShape value;

            int intValue;

            if (properties.TryParseEnum(PropertyShape, out value))
            {
                newShape = value;
            }
            else if (properties.TryParseInt32(PropertyShape, out intValue) && Enum.IsDefined(typeof(NodeShape), intValue))
            {
                newShape = (NodeShape)intValue;
            }
            else
            {
                newShape = null;
            }
        }

        public ChangeShapeCommand(Node node, NodeShape? newShape)
            : base(node)
        {
            this.newShape = newShape;
        }

        public override void Save(PropertiesBag properties)
        {
            if (newShape != null)
            {
                properties.Set(PropertyShape, newShape.ToString());
            }

            base.Save(properties);
        }

        protected override void Execute(bool isRedo)
        {
            Node node = Node as Node;

            if (node != null)
            {
                oldShape = node.Shape;

                node.SetupShape(newShape);
                node.Select();
            }
        }

        protected override void Revert()
        {
            Node node = Node as Node;

            if (node != null)
            {
                node.SetupShape(oldShape);
                node.Select();
            }
        }
    }
}

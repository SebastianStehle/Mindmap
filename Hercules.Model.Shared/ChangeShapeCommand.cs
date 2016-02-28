// ==========================================================================
// ChangeShapeCommand.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using GP.Utils;

// ReSharper disable SuggestBaseTypeForParameter

namespace Hercules.Model
{
    public sealed class ChangeShapeCommand : CommandBase<Node>
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
            oldShape = Node.Shape;

            Node.ChangeShape(newShape);

            if (isRedo)
            {
                Node.Select();
            }
        }

        protected override void Revert()
        {
            Node.ChangeShape(oldShape);
            Node.Select();
        }
    }
}

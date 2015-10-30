// ==========================================================================
// ChildNodeCommandBase.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Globalization;

namespace Hercules.Model
{
    public abstract class ChildNodeCommandBase : CommandBase
    {
        private readonly Node child;

        public Node Child
        {
            get { return child; }
        }

        protected ChildNodeCommandBase(NodeBase node, Node child)
            : base(node)
        {
            if (child == null)
            {
                child = (Node)Node.Document.GetOrCreateNode(Guid.NewGuid(), id => new Node(id));
            }

            this.child = child;
        }

        protected ChildNodeCommandBase(PropertiesBag properties, Document document)
            : base(properties, document)
        {
            Guid childId = properties["ChildId"].ToGuid(CultureInfo.InvariantCulture);

            child = (Node)document.GetOrCreateNode(childId, i => new Node(i));
        }

        public override void Save(PropertiesBag properties)
        {
            properties.Set("ChildId", child.Id);

            base.Save(properties);
        }
    }
}

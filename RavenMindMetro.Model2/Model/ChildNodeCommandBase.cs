// ==========================================================================
// ChildNodeCommandBase.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace RavenMind.Model
{
    public abstract class ChildNodeCommandBase : CommandBase
    {
        private readonly Node child;

        protected Node Child
        {
            get
            {
                return child;
            }
        }

        protected ChildNodeCommandBase(NodeBase node, Node child)
            : base(node)
        {
            if (child == null)
            {
                child = (Node)Node.Document.GetOrCreateNode<Node>(Guid.NewGuid(), id => new Node(id));
            }

            this.child = child;
        }

        protected ChildNodeCommandBase(CommandProperties properties, Document document)
            : base(properties, document)
        {
            child = (Node)document.GetOrCreateNode(properties.GetGuid("ChildId"), i => new Node(i));
        }

        public override void Save(CommandProperties properties)
        {
            properties.Set("ChildId", child.Id);

            base.Save(properties);
        }
    }
}

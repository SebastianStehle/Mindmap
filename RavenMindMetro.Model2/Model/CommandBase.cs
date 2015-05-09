// ==========================================================================
// CommandBase.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using SE.Metro;
using System;

namespace RavenMind.Model
{
    public abstract class CommandBase : IUndoRedoAction
    {
        private readonly NodeBase node;

        protected NodeBase Node
        {
            get
            {
                return node;
            }
        }

        public CommandBase(NodeBase node)
        {
            Guard.NotNull(node, "node");

            this.node = node;
        }

        public CommandBase(CommandProperties properties, Document document)
        {
            Guard.NotNull(properties, "properties");
            Guard.NotNull(document, "document");

            Guid id = properties.Get<Guid>("NodeId");

            if (id != Guid.Empty)
            {
                node = document.GetOrCreateNode(id, x => new Node(id));
            }
        }

        public virtual void Save(CommandProperties properties)
        {
            Guard.NotNull(properties, "properties");

            if (node != null)
            {
                properties.Set("NodeId", node.Id);
            }
        }

        public abstract void Execute();

        public abstract void Revert();

        public void Undo()
        {
            Revert();
        }

        public void Redo()
        {
            Execute();
        }
    }
}

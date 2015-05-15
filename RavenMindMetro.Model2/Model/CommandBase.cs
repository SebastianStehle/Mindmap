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

            node = document.GetOrCreateNode(properties.GetGuid("NodeId"), i => new Node(i));
        }

        public virtual void Save(CommandProperties properties)
        {
            Guard.NotNull(properties, "properties");

            properties.Set("NodeId", node.Id);
        }

        protected abstract void Execute(bool isRedo);

        protected abstract void Revert();

        public void Execute()
        {
            Execute(false);
        }

        public void Undo()
        {
            Revert();
        }

        public void Redo()
        {
            Execute(true);
        }
    }
}

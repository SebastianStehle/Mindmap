// ==========================================================================
// CommandBase.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Globalization;
using GP.Windows;

namespace Hercules.Model
{
    public abstract class CommandBase : IUndoRedoAction
    {
        private const string PropertyKeyForNodeId = "NodeId";
        private readonly NodeBase node;

        public NodeBase Node
        {
            get { return node; }
        }

        protected CommandBase(NodeBase node)
        {
            Guard.NotNull(node, nameof(node));

            this.node = node;
        }

        protected CommandBase(PropertiesBag properties, Document document)
        {
            Guard.NotNull(properties, nameof(properties));
            Guard.NotNull(document, nameof(document));

            Guid nodeId = properties[PropertyKeyForNodeId].ToGuid(CultureInfo.InvariantCulture);

            node = document.GetOrCreateNode(nodeId, i => new Node(i));
        }

        public virtual void Save(PropertiesBag properties)
        {
            Guard.NotNull(properties, nameof(properties));

            properties.Set(PropertyKeyForNodeId, node.Id);
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

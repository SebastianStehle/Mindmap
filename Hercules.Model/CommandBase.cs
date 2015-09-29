// ==========================================================================
// CommandBase.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Globalization;
using GP.Windows;

namespace Hercules.Model
{
    public abstract class CommandBase : IUndoRedoAction
    {
        private readonly NodeBase node;

        protected NodeBase Node
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

            node = document.GetOrCreateNode(properties["NodeId"].ToGuid(CultureInfo.InvariantCulture), i => new Node(i));
        }

        public virtual void Save(PropertiesBag properties)
        {
            Guard.NotNull(properties, nameof(properties));

            properties.Set("NodeId", node.Id);
        }

        protected abstract void Execute(bool isRedo);

        protected abstract void Revert();

        public virtual void Cleanup()
        {
        }

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

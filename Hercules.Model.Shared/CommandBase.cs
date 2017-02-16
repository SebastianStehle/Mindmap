// ==========================================================================
// CommandBase.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Globalization;
using GP.Utils;

namespace Hercules.Model
{
    public abstract class CommandBase<TNode> : IUndoRedoCommand where TNode : NodeBase
    {
        private const string PropertyNodeId = "NodeId";
        private readonly TNode node;

        public TNode Node
        {
            get { return node; }
        }

        protected CommandBase(TNode node)
        {
            Guard.NotNull(node, nameof(node));

            this.node = node;
        }

        protected CommandBase(PropertiesBag properties, Document document)
        {
            Guard.NotNull(properties, nameof(properties));
            Guard.NotNull(document, nameof(document));

            var nodeId = properties[PropertyNodeId].ToGuid(CultureInfo.InvariantCulture);

            node = (TNode)document.GetOrCreateNode(nodeId, i => new Node(i));
        }

        public virtual void Save(PropertiesBag properties)
        {
            Guard.NotNull(properties, nameof(properties));

            properties.Set(PropertyNodeId, node.Id);
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

// ==========================================================================
// NodeBase.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace Hercules.Model
{
    public abstract class NodeBase : ImmutableObject<NodeBase>
    {
        private readonly Guid id;
        private string text;

        public Guid Id
        {
            get { return id; }
        }

        protected NodeBase(Guid id)
        {
            this.id = id;
        }

        public bool IsCollapsed
        {
            get { return true; }
        }

        public string Text
        {
            get { return text; }
        }

        public NodeBase ChangeText(string newText)
        {
            return Cloned<NodeBase>(clone => clone.text = newText);
        }

        public abstract NodeSide Side(Document document);

        public abstract bool HasDescentant(Document document, Node child);

        public abstract bool HasChild(Node child);

        public abstract NodeBase Insert(Guid nodeId, int? index = null, NodeSide side = NodeSide.Auto);

        public abstract NodeBase Remove(Guid nodeId);
    }
}

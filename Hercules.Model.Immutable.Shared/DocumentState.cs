// ==========================================================================
// DocumentState.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using GP.Utils;

namespace Hercules.Model
{
    public sealed class DocumentState : ImmutableObject<DocumentState>
    {
        private readonly RootNode root;
        private bool isCheckableDefault;
        private Guid? selectedNodeId;
        private ImmutableDictionary<Guid, NodeBase> nodes;

        public DocumentState(Guid rootId)
        {
            root = new RootNode(rootId);

            nodes = ImmutableDictionary<Guid, NodeBase>.Empty.Add(rootId, root);
        }

        public RootNode Root
        {
            get { return root; }
        }

        public Guid? SelectedNodeId
        {
            get { return selectedNodeId; }
        }

        public IReadOnlyDictionary<Guid, NodeBase> Nodes
        {
            get { return nodes; }
        }

        public DocumentState Dispatch(SelectNode action, Document document)
        {
            Guard.NotNull(action, nameof(action));

            if ((action.NodeId.HasValue && !nodes.ContainsKey(action.NodeId.Value)) || selectedNodeId == action.NodeId)
            {
                return this;
            }

            return Cloned<DocumentState>(clone => clone.selectedNodeId = action.NodeId);
        }

        public DocumentState Dispatch(MoveNode action, Document document)
        {
            Guard.NotNull(action, nameof(action));

            Node node = nodes.GetOrDefault(action.NodeId) as Node;

            if (node == null)
            {
                return this;
            }

            NodeBase oldParent = document.Parent(node);
            NodeBase newParent = nodes.GetOrDefault(action.ParentId);

            if (newParent == null || newParent.HasChild(node) || oldParent == null || !oldParent.HasChild(node))
            {
                return this;
            }

            var newNodes =
                nodes
                    .SetItem(oldParent.Id, oldParent.Remove(node.Id))
                    .SetItem(newParent.Id, newParent.Insert(action.NodeId, action.Index, action.Side));

            return Cloned<DocumentState>(clone => clone.nodes = newNodes);
        }

        public DocumentState Dispatch(AddChild action, Document document)
        {
            Guard.NotNull(action, nameof(action));

            NodeBase newParent = nodes.GetOrDefault(action.ParentId);

            if (newParent == null || nodes.ContainsKey(action.NodeId))
            {
                return this;
            }

            Node node = new Node(action.ParentId);

            var newNodes =
                nodes
                    .SetItem(node.Id, node)
                    .SetItem(newParent.Id, newParent.Insert(action.NodeId));

            return Cloned<DocumentState>(clone => clone.nodes = newNodes);
        }

        public DocumentState Dispatch(AddSibling action, Document document)
        {
            Guard.NotNull(action, nameof(action));

            NodeBase newParent = nodes.GetOrDefault(action.ParentId);

            if (newParent == null || nodes.ContainsKey(action.NodeId))
            {
                return this;
            }

            Node node = new Node(action.ParentId);

            var newNodes =
                nodes
                    .SetItem(node.Id, node)
                    .SetItem(newParent.Id, newParent.Insert(action.NodeId));

            return Cloned<DocumentState>(clone => clone.nodes = newNodes);
        }
    }
}

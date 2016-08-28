// ==========================================================================
// DocumentStateProjections.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using GP.Utils;

namespace Hercules.Model
{
    public sealed class DocumentStateProjections
    {
        private readonly DocumentState state;
        private Dictionary<Guid, NodeBase> parents;
        private RootNode root;
        private List<Node> rightMainNodes;
        private List<Node> leftMainNodes;

        public DocumentStateProjections(DocumentState state)
        {
            this.state = state;
        }

        public RootNode Root()
        {
            return root ?? (root = state.Root);
        }

        public IReadOnlyDictionary<Guid, NodeBase> Nodes()
        {
            return state.Nodes;
        }

        public IReadOnlyList<Node> LeftMainNodes()
        {
            return leftMainNodes ?? (leftMainNodes = Children(state.Root.LeftChildIds));
        }

        public IReadOnlyList<Node> RightMainNodes()
        {
            return rightMainNodes ?? (rightMainNodes = Children(state.Root.RightChildIds));
        }

        public IReadOnlyList<Node> Children(Node node)
        {
            return Children(node.ChildIds);
        }

        public NodeBase Parent(Guid nodeId)
        {
            BuildParents();

            return parents.GetOrDefault(nodeId);
        }

        private List<Node> Children(IEnumerable<Guid> ids)
        {
            return ids.Select(id => state.Nodes[id]).OfType<Node>().ToList();
        }

        private void BuildParents()
        {
            if (parents != null)
            {
                return;
            }

            parents = new Dictionary<Guid, NodeBase>();

            Action<NodeBase, IEnumerable<Guid>> addParents = null;

            addParents = (parent, childIds) =>
            {
                foreach (var childId in childIds)
                {
                    parents[childId] = parent;

                    Node node = (Node)state.Nodes[childId];

                    addParents(node, node.ChildIds);
                }
            };

            addParents(state.Root, state.Root.LeftChildIds.Union(state.Root.RightChildIds));
        }
    }
}

// ==========================================================================
// Document.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Hercules.Model2
{
    public sealed partial class Document : Cloneable<Document>
    {
        private readonly Guid id;
        private ImmutableDictionary<Guid, Node> nodes = ImmutableDictionary<Guid, Node>.Empty;
        private ILookup<Guid, Node> children;
        private Guid? selectedNodeId;

        public IEnumerable<Node> Nodes
        {
            get { return nodes; }
        }

        public Document(Guid id)
        {
            nodes = nodes.Add(id, new Node(id, this));

            this.id = id;

            OnCloned();
        }

        public Document AddNode(Node parent, Guid nodeId, int index = -1, NodeSide side = NodeSide.Auto)
        {
            if (!IsValidNode(parent))
            {
                return this;
            }

            return AddNode(parent, index, side, () => new Node(nodeId, this));
        }

        public Document MoveNode(Node node, Guid parentId, int index = -1, NodeSide side = NodeSide.Auto)
        {
            if (!IsValidNode(node) || nodes.TryGetValue(parentId, out var parent))
            {
                return this;
            }

            return RemoveNode(node).AddNode(parent, index, side, () => node);
        }

        private Document AddNode(Node parent, int index, NodeSide side, Func<Node> factory)
        {
            return Clone(d =>
            {
                Node node = null;

                if (parent.Id == id)
                {
                    var l = GetLeftChildren();
                    var r = GetRightChildren();

                    if (l.Count > r.Count)
                    {
                        node = factory().WithPosition(parent.Id, r.Count, NodeSide.Right);
                    }
                    else
                    {
                        node = factory().WithPosition(parent.Id, l.Count, NodeSide.Left);
                    }
                }

                if (node == null)
                {
                    node = factory().WithPosition(parent.Id, index, parent.IsRoot ? side : parent.Side);
                }

                var nodes = d.nodes.Add(node.Id, node);

                d.nodes = AdjustIndices(nodes, node, 1);
            });
        }

        public Document RemoveNode(Node node)
        {
            if (!IsValidNode(node))
            {
                return this;
            }

            return Clone(d =>
            {
                var nodes = d.nodes.Remove(node.Id);

                void RemoveChildren(Node n)
                {
                    foreach (var c in GetChildren(n))
                    {
                        nodes = nodes.Remove(c.Id);

                        RemoveChildren(c);
                    }
                }

                var parentList = GetParentList(node);

                d.nodes = AdjustIndices(nodes, node, -1);
            });
        }

        public Document WithSelectedNode(Guid? value)
        {
            if (value != selectedNodeId || (selectedNodeId.HasValue && !nodes.ContainsKey(selectedNodeId.Value)))
            {
                return this;
            }

            return Clone(d => d.selectedNodeId = value);
        }

        public Document UpdateNode(Guid nodeId, Func<Node, Node> updater)
        {
            if (!nodes.TryGetValue(nodeId, out var node))
            {
                return this;
            }

            var newNode = updater(node);

            if (newNode == node)
            {
                return this;
            }

            return Clone(d => d.nodes = d.nodes.SetItem(node.Id, newNode));
        }

        public override void OnCloned()
        {
            foreach (var node in nodes.Values)
            {
                if (node.ParentId.HasValue && nodes.ContainsKey(node.ParentId.Value))
                {
                    throw new InvalidOperationException($"Node {node.Id} has no parent");
                }
            }

            children = nodes.Values.OrderBy(x => x.Index).ToLookup(x => x.ParentId ?? id);
        }

        private ImmutableDictionary<Guid, Node> AdjustIndices(ImmutableDictionary<Guid, Node> nodes, Node node, int offset)
        {
            var parentList = GetParentList(node);

            foreach (var child in parentList)
            {
                if (child.Index >= node.Index)
                {
                    nodes = nodes.SetItem(child.Id, child.WithPosition(child.ParentId.Value, child.Index + offset, child.Side));
                }
            }

            return nodes;
        }
    }
}

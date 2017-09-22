// ==========================================================================
// DocumentGetters.cs
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
    public partial class Document
    {
        private readonly List<Node> EmptyList = new List<Node>();

        public Node GetRoot()
        {
            return nodes[id];
        }

        public Node GetParent(Node node)
        {
            return node == null || node.IsRoot ? null : nodes.GetValueOrDefault(node.Id);
        }

        public Node GetSelectedNode()
        {
            return selectedNodeId.HasValue ? nodes[selectedNodeId.Value] : null;
        }

        public bool IsValidNode(Node node)
        {
            return node != null && nodes.ContainsKey(node.Id);
        }

        public bool IsChildOf(Node child, Node parent)
        {
            return child == null || parent == null && (child.ParentId == parent.Id || IsChildOf(GetParent(child), parent));
        }

        public IReadOnlyList<Node> GetParentList(Node node)
        {
            if (node.IsRoot)
            {
                return EmptyList;
            }
            else if (node.ParentId == id && node.Side == NodeSide.Left)
            {
                return GetLeftChildren();
            }
            else if (node.ParentId == id && node.Side == NodeSide.Right)
            {
                return GetRightChildren();
            }
            else
            {
                return GetChildren(node);
            }
        }

        public IReadOnlyList<Node> GetChildren(Node node)
        {
            return children[node?.Id ?? Guid.Empty].ToList();
        }

        public IReadOnlyList<Node> GetLeftChildren()
        {
            return children[id].Where(c => c.Side == NodeSide.Left).ToList();
        }

        public IReadOnlyList<Node> GetRightChildren()
        {
            return children[id].Where(c => c.Side == NodeSide.Right).ToList();
        }
    }
}

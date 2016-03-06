// ==========================================================================
// NodeExtensions.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using System.Linq;
using GP.Utils;

// ReSharper disable InvertIf
// ReSharper disable ConvertIfStatementToReturnStatement

namespace Hercules.Model
{
    public static class NodeExtensions
    {
        public static IList<Node> AllChildren(this NodeBase nodeBase)
        {
            RootNode root = nodeBase as RootNode;

            if (root != null)
            {
                return AllChildren(root);
            }

            Node node = nodeBase as Node;

            return node != null ? AllChildren(node) : null;
        }

        public static IList<Node> AllChildren(this Node node)
        {
            List<Node> allChildren = new List<Node>();

            AddChildren(allChildren, node);

            return allChildren;
        }

        public static IList<Node> AllChildren(this RootNode root)
        {
            List<Node> allChildren = new List<Node>();

            foreach (Node child in root.RightChildren)
            {
                allChildren.Add(child);

                AddChildren(allChildren, child);
            }

            foreach (Node child in root.LeftChildren)
            {
                allChildren.Add(child);

                AddChildren(allChildren, child);
            }

            return allChildren;
        }

        private static void AddChildren(ICollection<Node> allChildren, Node node)
        {
            foreach (Node child in node.Children)
            {
                allChildren.Add(child);

                AddChildren(allChildren, child);
            }
        }

        public static NodeSide OppositeSide(this NodeSide side)
        {
            if (side == NodeSide.Right)
            {
                return NodeSide.Left;
            }

            if (side == NodeSide.Left)
            {
                return NodeSide.Right;
            }

            return NodeSide.Auto;
        }

        public static IReadOnlyList<Node> RetrieveParentCollection(this Node node)
        {
            Guard.NotNull(node, nameof(node));

            IReadOnlyList<Node> result = null;

            RootNode parent = node.Parent as RootNode;

            if (parent != null)
            {
                result = parent.RightChildren.Contains(node) ? parent.RightChildren : parent.LeftChildren;
            }

            if (result == null)
            {
                Node parentNormal = node.Parent as Node;

                if (parentNormal != null)
                {
                    result = parentNormal.Children;
                }
            }

            return result;
        }
    }
}

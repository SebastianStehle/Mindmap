// ==========================================================================
// NodeExtensions.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using System.Linq;

// ReSharper disable InvertIf
// ReSharper disable ConvertIfStatementToReturnStatement

namespace Hercules.Model
{
    public static class NodeExtensions
    {
        public static IList<Node> AllChildren(this NodeBase nodeBase)
        {
            List<Node> allChildren = new List<Node>();

            RootNode root = nodeBase as RootNode;

            if (root != null)
            {
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
            }

            Node node = nodeBase as Node;

            if (node != null)
            {
                AddChildren(allChildren, node);
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

        public static IReadOnlyList<Node> RetrieveParentCollection(this Node node)
        {
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
